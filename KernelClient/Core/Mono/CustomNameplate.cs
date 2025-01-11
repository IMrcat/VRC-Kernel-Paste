using System;
using KernelClient.Utils;
using MelonLoader;
using TMPro;
using UnityEngine;
using VRC;

namespace KernelClient.Core.Mono
{
	// Token: 0x02000073 RID: 115
	public class CustomNameplate : MonoBehaviour, IDisposable
	{
		// Token: 0x060002FA RID: 762 RVA: 0x0000FF5C File Offset: 0x0000E15C
		public CustomNameplate(IntPtr ptr)
			: base(ptr)
		{
		}

		// Token: 0x060002FB RID: 763 RVA: 0x0000FFC4 File Offset: 0x0000E1C4
		public void Dispose()
		{
			bool flag = this._statsText != null;
			if (flag)
			{
				this._statsText.text = null;
				this._statsText.OnDisable();
			}
			base.enabled = false;
		}

		// Token: 0x060002FC RID: 764 RVA: 0x00010008 File Offset: 0x0000E208
		private void Start()
		{
			bool flag = !base.enabled;
			if (!flag)
			{
				try
				{
					this.SetupNameplate();
				}
				catch (Exception ex)
				{
					MelonLogger.Msg("CustomNameplate Start Error: " + ex.Message);
				}
			}
		}

		// Token: 0x060002FD RID: 765 RVA: 0x00010060 File Offset: 0x0000E260
		private void SetupNameplate()
		{
			PlayerNameplate field_Public_PlayerNameplate_ = this.Player._vrcplayer.field_Public_PlayerNameplate_0;
			Transform transform = field_Public_PlayerNameplate_.field_Public_GameObject_5.transform;
			TextMeshProUGUI component = transform.Find("Trust Text").GetComponent<TextMeshProUGUI>();
			Transform transform2 = Object.Instantiate<Transform>(transform, transform);
			transform2.parent = field_Public_PlayerNameplate_.field_Public_GameObject_0.transform;
			transform2.localPosition = new Vector3(0f, 55f, 0f);
			transform2.gameObject.SetActive(true);
			this._statsText = transform2.Find("Trust Text").GetComponent<TextMeshProUGUI>();
			this.CustomizeTextAppearance(this._statsText);
			bool flag = this.OverRender && this.Enabled;
			this._statsText.isOverlay = flag;
			component.isOverlay = flag;
			this.DisableUnusedElements(transform2);
		}

		// Token: 0x060002FE RID: 766 RVA: 0x00010134 File Offset: 0x0000E334
		private void CustomizeTextAppearance(TextMeshProUGUI textElement)
		{
			textElement.color = Color.white;
			textElement.fontSize += 2f;
			textElement.enableAutoSizing = true;
			textElement.fontStyle = 1;
			textElement.characterSpacing = 1f;
			textElement.enableWordWrapping = false;
			textElement.richText = true;
		}

		// Token: 0x060002FF RID: 767 RVA: 0x00010190 File Offset: 0x0000E390
		private void DisableUnusedElements(Transform customTransform)
		{
			string[] array = new string[] { "Trust Icon", "Performance Icon", "Performance Text", "Friend Anchor Stats" };
			string[] array2 = array;
			string[] array3 = array2;
			foreach (string text in array3)
			{
				Transform transform = customTransform.Find(text);
				bool flag = transform != null;
				if (flag)
				{
					transform.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06000300 RID: 768 RVA: 0x00010210 File Offset: 0x0000E410
		private void Update()
		{
			bool flag = !this.Enabled;
			if (!flag)
			{
				try
				{
					this.TrackStatusUpdates();
					bool flag2 = this._updateInterval >= 50;
					if (flag2)
					{
						this.UpdateNameplate();
						this._updateInterval = 0;
					}
					else
					{
						this._updateInterval++;
					}
					this.HandlePulseEffect();
				}
				catch (Exception ex)
				{
					MelonLogger.Msg("CustomNameplate Update Error: " + ex.Message);
				}
			}
		}

		// Token: 0x06000301 RID: 769 RVA: 0x000102A0 File Offset: 0x0000E4A0
		private void TrackStatusUpdates()
		{
			bool flag = this._currentFrame == this.Player._playerNet.field_Private_Byte_0 && this._currentPing == this.Player._playerNet.field_Private_Byte_1;
			if (flag)
			{
				this._noUpdateCounter++;
			}
			else
			{
				this._noUpdateCounter = 0;
			}
			this._currentFrame = this.Player._playerNet.field_Private_Byte_0;
			this._currentPing = this.Player._playerNet.field_Private_Byte_1;
		}

		// Token: 0x06000302 RID: 770 RVA: 0x0000246F File Offset: 0x0000066F
		private void TrustRankTextNP()
		{
		}

		// Token: 0x06000303 RID: 771 RVA: 0x0001032C File Offset: 0x0000E52C
		private void UpdateNameplate()
		{
			string platformIcon = this.GetPlatformIcon();
			string visibilityIcon = this.GetVisibilityIcon();
			string statusIcon = this.GetStatusIcon();
			string statusColor = this.GetStatusColor();
			string friendStatusIcon = this.GetFriendStatusIcon();
			string text = "[FPS: " + PlayerUtil.GetFramesColord(this.Player) + "]";
			string text2 = "[Ping: " + PlayerUtil.GetPingColord(this.Player) + "]";
			string text3 = (PlayerUtil.GetIsMaster(this.Player) ? "\ud83d\udc51" : "");
			string text4 = ((!PlayerUtil.knownBlocks.Contains(this.Player.field_Private_APIUser_0.displayName)) ? "" : "<color=#FF0000>Blocked you \ud83d\udeab</color>");
			string text5 = this.BuildNameplateText(platformIcon, visibilityIcon, statusIcon, statusColor, friendStatusIcon, text, text2, text3, text4);
			this._statsText.text = text5;
		}

		// Token: 0x06000304 RID: 772 RVA: 0x00010408 File Offset: 0x0000E608
		private string BuildNameplateText(string platformIcon, string visibilityIcon, string statusIcon, string statusColor, string friendStatus, string fpsStat, string pingStat, string host, string blocked)
		{
			bool flag = host == "";
			string text;
			if (flag)
			{
				bool flag2 = PlayerUtil.IsFriend(this.Player);
				Color color;
				if (flag2)
				{
					color = CustomNameplate.AccentGreen;
				}
				else
				{
					color = CustomNameplate.AccentRed;
				}
				text = string.Concat(new string[]
				{
					this.LabelWithColor("", platformIcon, CustomNameplate.AccentBlue),
					" | ",
					this.LabelWithColor("", statusColor + statusIcon, CustomNameplate.AccentGreen),
					" | ",
					this.LabelWithColor("", friendStatus, color),
					" | ",
					this.LabelWithColor("", fpsStat, CustomNameplate.AccentYellow),
					" | ",
					this.LabelWithColor("", pingStat, CustomNameplate.AccentYellow),
					this.LabelWithColor("", blocked, CustomNameplate.AccentRed)
				});
			}
			else
			{
				bool flag3 = PlayerUtil.IsFriend(this.Player);
				Color color2;
				if (flag3)
				{
					color2 = CustomNameplate.AccentGreen;
				}
				else
				{
					color2 = CustomNameplate.AccentRed;
				}
				text = string.Concat(new string[]
				{
					this.LabelWithColor("", platformIcon, CustomNameplate.AccentBlue),
					" | ",
					this.LabelWithColor("", statusColor + statusIcon, CustomNameplate.AccentGreen),
					" | ",
					this.LabelWithColor("", friendStatus, color2),
					" | ",
					this.LabelWithColor("", host, CustomNameplate.AccentPurple),
					" | ",
					this.LabelWithColor("", fpsStat, CustomNameplate.AccentYellow),
					" | ",
					this.LabelWithColor("", pingStat, CustomNameplate.AccentYellow),
					this.LabelWithColor("", blocked, CustomNameplate.AccentRed)
				});
			}
			return text;
		}

		// Token: 0x06000305 RID: 773 RVA: 0x000105F4 File Offset: 0x0000E7F4
		private string LabelWithColor(string label, string content, Color color)
		{
			return string.Concat(new string[]
			{
				label,
				" <color=#",
				ColorUtility.ToHtmlStringRGB(color),
				">",
				content,
				"</color>"
			});
		}

		// Token: 0x06000306 RID: 774 RVA: 0x0001063C File Offset: 0x0000E83C
		private string GetPlatformIcon()
		{
			string platform = PlayerUtil.GetPlatform(this.Player);
			bool flag = platform.Contains("VR");
			string text;
			if (flag)
			{
				text = "\ud83e\udd7d";
			}
			else
			{
				string text2 = platform;
				text = ((!text2.Contains("Quest")) ? "\ud83d\udda5\ufe0f" : "\ud83d\udcf1Q");
			}
			return text;
		}

		// Token: 0x06000307 RID: 775 RVA: 0x0001069C File Offset: 0x0000E89C
		private string GetVisibilityIcon()
		{
			string text = PlayerUtil.GetAvatarStatus(this.Player).ToLower();
			bool flag = text == "private";
			string text2;
			if (flag)
			{
				text2 = "\ud83d\udd12";
			}
			else
			{
				bool flag2 = text == "public";
				if (flag2)
				{
					text2 = "\ud83d\udd13";
				}
				else
				{
					text2 = "\ud83d\udeab";
				}
			}
			return text2;
		}

		// Token: 0x06000308 RID: 776 RVA: 0x000106F4 File Offset: 0x0000E8F4
		private string GetStatusIcon()
		{
			bool flag = this._noUpdateCounter > 260;
			string text;
			if (flag)
			{
				text = "\ud83d\udc80";
			}
			else
			{
				bool flag2 = this._noUpdateCounter > 100;
				if (flag2)
				{
					text = "⚠\ufe0f";
				}
				else
				{
					text = "✨";
				}
			}
			return text;
		}

		// Token: 0x06000309 RID: 777 RVA: 0x0001073C File Offset: 0x0000E93C
		private string GetStatusColor()
		{
			bool flag = this._noUpdateCounter > 260;
			string text;
			if (flag)
			{
				text = "<color=#" + ColorUtility.ToHtmlStringRGB(CustomNameplate.AccentRed) + ">";
			}
			else
			{
				bool flag2 = this._noUpdateCounter > 100;
				if (flag2)
				{
					text = "<color=#" + ColorUtility.ToHtmlStringRGB(CustomNameplate.AccentYellow) + ">";
				}
				else
				{
					text = "<color=#" + ColorUtility.ToHtmlStringRGB(CustomNameplate.AccentGreen) + ">";
				}
			}
			return text;
		}

		// Token: 0x0600030A RID: 778 RVA: 0x000107C0 File Offset: 0x0000E9C0
		private string GetFriendStatusIcon()
		{
			return PlayerUtil.IsFriend(this.Player) ? "\ud83d\udc9a" : "\ud83d\udc94";
		}

		// Token: 0x0600030B RID: 779 RVA: 0x000107EC File Offset: 0x0000E9EC
		private void HandlePulseEffect()
		{
			bool isPulsing = this._isPulsing;
			if (isPulsing)
			{
				this._pulseTimer += Time.deltaTime;
				float num = Mathf.Sin(this._pulseTimer * 4f) * 0.2f + 0.8f;
				this._statsText.alpha = num;
				bool flag = this._pulseTimer > 2f;
				if (flag)
				{
					this._isPulsing = false;
					this._statsText.alpha = 1f;
				}
			}
			bool flag2 = this.GetStatusIcon() == "\ud83d\udc80";
			if (flag2)
			{
				this.FlashCrashedStatus();
			}
		}

		// Token: 0x0600030C RID: 780 RVA: 0x0001088C File Offset: 0x0000EA8C
		private void FlashCrashedStatus()
		{
			this._pulseTimer += Time.deltaTime;
			float num = Mathf.Abs(Mathf.Sin(this._pulseTimer * this.flashSpeed)) * (this.flashMaxAlpha - this.flashMinAlpha) + this.flashMinAlpha;
			this._statsText.alpha = num;
		}

		// Token: 0x040001D3 RID: 467
		private TextMeshProUGUI _statsText;

		// Token: 0x040001D4 RID: 468
		public Player Player;

		// Token: 0x040001D5 RID: 469
		public bool OverRender = true;

		// Token: 0x040001D6 RID: 470
		public bool Enabled = true;

		// Token: 0x040001D7 RID: 471
		private byte _currentFrame;

		// Token: 0x040001D8 RID: 472
		private byte _currentPing;

		// Token: 0x040001D9 RID: 473
		private int _noUpdateCounter = 0;

		// Token: 0x040001DA RID: 474
		private int _updateInterval = 50;

		// Token: 0x040001DB RID: 475
		private float _pulseTimer = 0f;

		// Token: 0x040001DC RID: 476
		private bool _isPulsing = false;

		// Token: 0x040001DD RID: 477
		private float flashSpeed = 20f;

		// Token: 0x040001DE RID: 478
		private float flashMinAlpha = 0.1f;

		// Token: 0x040001DF RID: 479
		private float flashMaxAlpha = 1f;

		// Token: 0x040001E0 RID: 480
		private static readonly Color AccentBlue = new Color(0f, 0.5f, 1f);

		// Token: 0x040001E1 RID: 481
		private static readonly Color AccentGreen = new Color(0f, 0.8f, 0.4f);

		// Token: 0x040001E2 RID: 482
		private static readonly Color AccentRed = new Color(1f, 0.2f, 0.2f);

		// Token: 0x040001E3 RID: 483
		private static readonly Color AccentYellow = new Color(1f, 0.8f, 0.2f);

		// Token: 0x040001E4 RID: 484
		private static readonly Color AccentOrange = new Color(1f, 0.5f, 0f);

		// Token: 0x040001E5 RID: 485
		private static readonly Color AccentPurple = new Color(0.5f, 0f, 1f);

		// Token: 0x02000074 RID: 116
		private static class StatusIcons
		{
			// Token: 0x040001E6 RID: 486
			public const string DESKTOP = "\ud83d\udda5\ufe0f";

			// Token: 0x040001E7 RID: 487
			public const string QUEST = "\ud83d\udcf1Q";

			// Token: 0x040001E8 RID: 488
			public const string VR = "\ud83e\udd7d";

			// Token: 0x040001E9 RID: 489
			public const string STABLE = "✨";

			// Token: 0x040001EA RID: 490
			public const string LAGGING = "⚠\ufe0f";

			// Token: 0x040001EB RID: 491
			public const string CRASHED = "\ud83d\udc80";

			// Token: 0x040001EC RID: 492
			public const string HOST = "\ud83d\udc51";

			// Token: 0x040001ED RID: 493
			public const string BLOCKED = "\ud83d\udeab";

			// Token: 0x040001EE RID: 494
			public const string CLIENT = "⚡";

			// Token: 0x040001EF RID: 495
			public const string PRIVATE = "\ud83d\udd12";

			// Token: 0x040001F0 RID: 496
			public const string PUBLIC = "\ud83d\udd13";

			// Token: 0x040001F1 RID: 497
			public const string FRIEND = "\ud83d\udc9a";
		}
	}
}
