using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MelonLoader;
using TMPro;
using UnityEngine;
using VRC;

namespace KernelClient.Core.Mono
{
	// Token: 0x02000075 RID: 117
	public class CustomNameplateAccountAge : MonoBehaviour, IDisposable
	{
		// Token: 0x0600030E RID: 782 RVA: 0x0001098C File Offset: 0x0000EB8C
		public CustomNameplateAccountAge(IntPtr ptr)
			: base(ptr)
		{
		}

		// Token: 0x0600030F RID: 783 RVA: 0x0000314E File Offset: 0x0000134E
		public void Dispose()
		{
			this._statsText.text = null;
			this._statsText.OnDisable();
			base.enabled = false;
		}

		// Token: 0x06000310 RID: 784 RVA: 0x000109E0 File Offset: 0x0000EBE0
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
					MelonLogger.Msg(ex.Message);
				}
			}
		}

		// Token: 0x06000311 RID: 785 RVA: 0x00010A2C File Offset: 0x0000EC2C
		private void SetupNameplate()
		{
			PlayerNameplate field_Public_PlayerNameplate_ = this.Player._vrcplayer.field_Public_PlayerNameplate_0;
			Transform transform = field_Public_PlayerNameplate_.field_Public_GameObject_5.transform;
			Transform transform2 = Object.Instantiate<Transform>(transform, transform);
			transform2.parent = field_Public_PlayerNameplate_.field_Public_GameObject_0.transform;
			transform2.gameObject.SetActive(true);
			transform2.localPosition = new Vector3(0f, 100f, 0f);
			this._statsText = transform2.Find("Trust Text").GetComponent<TextMeshProUGUI>();
			this.ConfigureTextComponent();
			this.SetupBackground(transform2);
			this.DisableUnusedElements(transform2);
		}

		// Token: 0x06000312 RID: 786 RVA: 0x00010AC8 File Offset: 0x0000ECC8
		private void ConfigureTextComponent()
		{
			this._statsText.color = Color.white;
			this._statsText.fontSize += 1f;
			this._statsText.enableAutoSizing = true;
			this._statsText.fontStyle = 1;
			this._statsText.characterSpacing = 1f;
			bool flag = this.OverRender && this.Enabled;
			if (flag)
			{
				this._statsText.isOverlay = true;
			}
			else
			{
				this._statsText.isOverlay = false;
			}
		}

		// Token: 0x06000313 RID: 787 RVA: 0x00010B60 File Offset: 0x0000ED60
		private void SetupBackground(Transform transform)
		{
			ImageThreeSlice component = transform.GetComponent<ImageThreeSlice>();
			component.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);
		}

		// Token: 0x06000314 RID: 788 RVA: 0x00010B98 File Offset: 0x0000ED98
		private void DisableUnusedElements(Transform transform)
		{
			transform.Find("Trust Icon").gameObject.SetActive(false);
			transform.Find("Performance Icon").gameObject.SetActive(false);
			transform.Find("Performance Text").gameObject.SetActive(false);
			transform.Find("Friend Anchor Stats").gameObject.SetActive(false);
		}

		// Token: 0x06000315 RID: 789 RVA: 0x00010C04 File Offset: 0x0000EE04
		private TimeSpan DetectTimeZoneOffset(DateTime accountCreationTime)
		{
			TimeSpan timeSpan3;
			try
			{
				DateTime now = DateTime.Now;
				DateTime utcNow = DateTime.UtcNow;
				TimeSpan utcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
				byte field_Private_Byte_ = this.Player._playerNet.field_Private_Byte_1;
				float num = (float)field_Private_Byte_ / 1000f;
				int num2 = this.CalculateRelativeOffset(field_Private_Byte_, utcOffset);
				TimeSpan timeSpan = this.AdjustForNetworkPath(field_Private_Byte_, utcOffset);
				TimeSpan timeSpan2 = TimeSpan.FromHours((double)num2) + timeSpan;
				bool flag = this.offsetSamples.Count >= 200;
				if (flag)
				{
					this.offsetSamples.Dequeue();
				}
				this.offsetSamples.Enqueue(timeSpan2);
				timeSpan3 = new TimeSpan((long)this.offsetSamples.Average((TimeSpan ts) => ts.Ticks));
			}
			catch
			{
				timeSpan3 = TimeSpan.Zero;
			}
			return timeSpan3;
		}

		// Token: 0x06000316 RID: 790 RVA: 0x00010CF8 File Offset: 0x0000EEF8
		private int CalculateRelativeOffset(byte ping, TimeSpan ourOffset)
		{
			int num = (int)ourOffset.TotalHours;
			bool flag = ping < 50;
			int num2;
			if (flag)
			{
				num2 = this.PingToLocalOffset(ping);
			}
			else
			{
				bool flag2 = ping < 100;
				if (flag2)
				{
					num2 = this.PingToNearbyOffset(ping, num);
				}
				else
				{
					bool flag3 = ping < 200;
					if (flag3)
					{
						num2 = this.PingToContinentOffset(ping, num);
					}
					else
					{
						num2 = this.PingToDistantOffset(ping, num);
					}
				}
			}
			return num2;
		}

		// Token: 0x06000317 RID: 791 RVA: 0x00010D60 File Offset: 0x0000EF60
		private int PingToLocalOffset(byte ping)
		{
			int num = (int)TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalHours;
			bool flag = ping < 20;
			int num2;
			if (flag)
			{
				num2 = num;
			}
			else
			{
				bool flag2 = ping < 35;
				if (flag2)
				{
					num2 = num + 1;
				}
				else
				{
					num2 = num - 1;
				}
			}
			return num2;
		}

		// Token: 0x06000318 RID: 792 RVA: 0x00010DB0 File Offset: 0x0000EFB0
		private int PingToNearbyOffset(byte ping, int ourOffset)
		{
			int num = (int)((ping - 50) / 10);
			bool flag = this.IsEuropeRegion();
			int num2;
			if (flag)
			{
				bool flag2 = ping < 80;
				if (flag2)
				{
					num2 = ourOffset + num;
				}
				else
				{
					num2 = ourOffset - num;
				}
			}
			else
			{
				bool flag3 = this.IsNorthAmericaRegion();
				if (flag3)
				{
					bool flag4 = ping < 80;
					if (flag4)
					{
						num2 = ourOffset - num;
					}
					else
					{
						num2 = ourOffset + num;
					}
				}
				else
				{
					bool flag5 = this.IsAsiaRegion();
					if (flag5)
					{
						bool flag6 = ping < 80;
						if (flag6)
						{
							num2 = ourOffset + num;
						}
						else
						{
							num2 = ourOffset - num;
						}
					}
					else
					{
						num2 = ourOffset + num * ((ping % 2 == 0) ? 1 : (-1));
					}
				}
			}
			return num2;
		}

		// Token: 0x06000319 RID: 793 RVA: 0x00010E44 File Offset: 0x0000F044
		private int PingToContinentOffset(byte ping, int ourOffset)
		{
			int num = (int)(ping / 40);
			bool flag = this.IsEuropeRegion();
			int num2;
			if (flag)
			{
				bool flag2 = ping < 150;
				if (flag2)
				{
					num2 = ourOffset + num;
				}
				else
				{
					num2 = ourOffset - num;
				}
			}
			else
			{
				bool flag3 = this.IsNorthAmericaRegion();
				if (flag3)
				{
					bool flag4 = ping < 150;
					if (flag4)
					{
						num2 = ourOffset - num;
					}
					else
					{
						num2 = ourOffset + num;
					}
				}
				else
				{
					bool flag5 = this.IsAsiaRegion();
					if (flag5)
					{
						bool flag6 = ping < 150;
						if (flag6)
						{
							num2 = ourOffset + num;
						}
						else
						{
							num2 = ourOffset - num;
						}
					}
					else
					{
						num2 = ourOffset + num * ((ping % 2 == 0) ? 1 : (-1));
					}
				}
			}
			return num2;
		}

		// Token: 0x0600031A RID: 794 RVA: 0x00010EDC File Offset: 0x0000F0DC
		private int PingToDistantOffset(byte ping, int ourOffset)
		{
			int num = 12;
			int num2 = Math.Min((int)(ping / 20), num);
			bool flag = ourOffset > 0;
			int num3;
			if (flag)
			{
				num3 = -num2;
			}
			else
			{
				num3 = num2;
			}
			return num3;
		}

		// Token: 0x0600031B RID: 795 RVA: 0x00010F0C File Offset: 0x0000F10C
		private TimeSpan AdjustForNetworkPath(byte ping, TimeSpan baseOffset)
		{
			float num = ((ping < 100) ? ((float)ping / 1000f) : ((ping >= 200) ? ((float)ping / 150f) : (0.5f * ((float)ping / 100f))));
			return TimeSpan.FromHours((double)num);
		}

		// Token: 0x0600031C RID: 796 RVA: 0x00010F5C File Offset: 0x0000F15C
		private bool IsEuropeRegion()
		{
			int num = (int)TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalHours;
			return num >= 0 && num <= 3;
		}

		// Token: 0x0600031D RID: 797 RVA: 0x00010F98 File Offset: 0x0000F198
		private bool IsNorthAmericaRegion()
		{
			int num = (int)TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalHours;
			return num >= -8 && num <= -4;
		}

		// Token: 0x0600031E RID: 798 RVA: 0x00010FD4 File Offset: 0x0000F1D4
		private bool IsAsiaRegion()
		{
			int num = (int)TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalHours;
			return num >= 5 && num <= 11;
		}

		// Token: 0x0600031F RID: 799 RVA: 0x00011010 File Offset: 0x0000F210
		private int GetLikelyOffset(int hour)
		{
			bool flag = hour >= 0 && hour < 8;
			int num;
			if (flag)
			{
				num = 8 - hour;
			}
			else
			{
				bool flag2 = hour >= 20;
				if (flag2)
				{
					num = -(hour - 20);
				}
				else
				{
					num = 0;
				}
			}
			return num;
		}

		// Token: 0x06000320 RID: 800 RVA: 0x00011050 File Offset: 0x0000F250
		private int GetActivityBasedOffset(int hour)
		{
			bool flag = hour >= 0 && hour < 8;
			int num;
			if (flag)
			{
				num = 8;
			}
			else
			{
				bool flag2 = hour >= 20;
				if (flag2)
				{
					num = -4;
				}
				else
				{
					num = 0;
				}
			}
			return num;
		}

		// Token: 0x06000321 RID: 801 RVA: 0x0001108C File Offset: 0x0000F28C
		private string GetLocalTime(DateTime accountCreationTime)
		{
			string text3;
			try
			{
				this.timeSinceUpdate += Time.deltaTime;
				bool flag = this.cachedLocalTime == null || this.timeSinceUpdate >= 5f;
				if (flag)
				{
					TimeSpan timeSpan = this.DetectTimeZoneOffset(accountCreationTime);
					DateTime utcNow = DateTime.UtcNow;
					float num = (float)this.Player._playerNet.field_Private_Byte_1 / 2000f;
					this.cachedLocalTime = new DateTime?(utcNow.AddSeconds((double)num).Add(timeSpan));
					this.timeSinceUpdate = 0f;
				}
				else
				{
					this.cachedLocalTime = new DateTime?(this.cachedLocalTime.Value.AddSeconds((double)Time.deltaTime));
				}
				string text = this.cachedLocalTime.Value.ToString("HH:mm");
				int num2 = (int)this.offsetSamples.Average((TimeSpan ts) => ts.TotalHours);
				string text2 = ((num2 >= 0) ? "+" : "");
				text3 = string.Format("{0} (UTC{1}{2})", text, text2, num2);
			}
			catch
			{
				text3 = "Unknown";
			}
			return text3;
		}

		// Token: 0x06000322 RID: 802 RVA: 0x000111E8 File Offset: 0x0000F3E8
		private void Update()
		{
			bool flag = !this.Enabled;
			if (!flag)
			{
				try
				{
					bool flag2 = this.skipX >= 20000;
					if (flag2)
					{
						bool flag3 = !string.IsNullOrEmpty(this.playerAge);
						if (flag3)
						{
							try
							{
								DateTime dateTime = DateTime.ParseExact(this.playerAge.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
								string text = OtherUtil.ToAgeString(dateTime);
								string localTime = this.GetLocalTime(dateTime);
								string text2 = string.Concat(new string[]
								{
									"<color=#",
									ColorUtility.ToHtmlStringRGB(CustomNameplateAccountAge.ACCENT_PURPLE),
									">\ud83d\udcc5</color> ",
									string.Format("<color=#{0}>{1:ddd dd MMM yyyy h:mm tt zzz}</color> | ", ColorUtility.ToHtmlStringRGB(CustomNameplateAccountAge.ACCENT_BLUE), dateTime),
									"<color=#",
									ColorUtility.ToHtmlStringRGB(CustomNameplateAccountAge.ACCENT_PURPLE),
									">⌛</color> <color=#",
									ColorUtility.ToHtmlStringRGB(CustomNameplateAccountAge.ACCENT_GREEN),
									">",
									text,
									"</color> | <color=#",
									ColorUtility.ToHtmlStringRGB(CustomNameplateAccountAge.ACCENT_PURPLE),
									"></color> "
								});
								this._statsText.text = text2;
								this.skipX = 0;
							}
							catch
							{
							}
						}
					}
					else
					{
						this.skipX++;
					}
				}
				catch (Exception ex)
				{
					MelonLogger.Msg(ex.Message);
				}
			}
		}

		// Token: 0x040001F2 RID: 498
		private TextMeshProUGUI _statsText;

		// Token: 0x040001F3 RID: 499
		public Player Player;

		// Token: 0x040001F4 RID: 500
		public string playerAge;

		// Token: 0x040001F5 RID: 501
		public bool OverRender;

		// Token: 0x040001F6 RID: 502
		public bool Enabled = true;

		// Token: 0x040001F7 RID: 503
		private int skipX = 20000;

		// Token: 0x040001F8 RID: 504
		private DateTime? cachedLocalTime = null;

		// Token: 0x040001F9 RID: 505
		private TimeZoneInfo detectedTimeZone = null;

		// Token: 0x040001FA RID: 506
		private float timeSinceUpdate = 0f;

		// Token: 0x040001FB RID: 507
		private Queue<TimeSpan> offsetSamples = new Queue<TimeSpan>();

		// Token: 0x040001FC RID: 508
		private const int MAX_SAMPLES = 200;

		// Token: 0x040001FD RID: 509
		private static readonly Color ACCENT_BLUE = new Color(0.012f, 0.322f, 1f);

		// Token: 0x040001FE RID: 510
		private static readonly Color ACCENT_GREEN = new Color(0.133f, 0.894f, 0.333f);

		// Token: 0x040001FF RID: 511
		private static readonly Color ACCENT_PURPLE = new Color(0.541f, 0.169f, 0.886f);

		// Token: 0x02000076 RID: 118
		private static class TimeIcons
		{
			// Token: 0x04000200 RID: 512
			public const string CLOCK = "\ud83d\udd50";

			// Token: 0x04000201 RID: 513
			public const string CALENDAR = "\ud83d\udcc5";

			// Token: 0x04000202 RID: 514
			public const string AGE = "⌛";

			// Token: 0x04000203 RID: 515
			public const string GLOBE = "\ud83c\udf0d";
		}
	}
}
