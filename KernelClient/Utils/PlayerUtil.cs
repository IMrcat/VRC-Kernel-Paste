using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Il2CppSystem.Collections.Generic;
using ReMod.Core.VRChat;
using UnityEngine;
using UnityEngine.XR;
using VRC;
using VRC.Core;
using VRC.SDKBase;

namespace KernelClient.Utils
{
	// Token: 0x02000063 RID: 99
	public static class PlayerUtil
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000275 RID: 629 RVA: 0x00002E01 File Offset: 0x00001001
		public static bool IsInRoom
		{
			get
			{
				return RoomManager.field_Internal_Static_ApiWorld_0 != null && RoomManager.field_Private_Static_ApiWorldInstance_0 != null;
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000276 RID: 630 RVA: 0x0000EE50 File Offset: 0x0000D050
		public static bool IsInVR
		{
			get
			{
				bool cachedVRState = PlayerUtil._cachedVRState;
				bool flag;
				if (cachedVRState)
				{
					flag = PlayerUtil._isInVrCache;
				}
				else
				{
					PlayerUtil._cachedVRState = true;
					List<XRDisplaySubsystem> list = new List<XRDisplaySubsystem>();
					SubsystemManager.GetInstances<XRDisplaySubsystem>(list);
					List<XRDisplaySubsystem>.Enumerator enumerator = list.GetEnumerator();
					while (enumerator.MoveNext())
					{
						IntegratedSubsystem current = enumerator._current;
						bool running = current.running;
						if (running)
						{
							PlayerUtil._isInVrCache = true;
							break;
						}
					}
					flag = PlayerUtil._isInVrCache;
				}
				return flag;
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000277 RID: 631 RVA: 0x00002E15 File Offset: 0x00001015
		public static string Current_World_ID
		{
			get
			{
				return RoomManager.field_Internal_Static_ApiWorld_0.id + ":" + RoomManager.field_Private_Static_ApiWorldInstance_0.instanceId;
			}
		}

		// Token: 0x06000278 RID: 632 RVA: 0x0000EEC4 File Offset: 0x0000D0C4
		internal static bool AnyActionMenuesOpen()
		{
			return ActionMenuController.field_Public_Static_ActionMenuController_0.field_Public_ActionMenuOpener_0.field_Private_Boolean_0 || ActionMenuController.field_Public_Static_ActionMenuController_0.field_Public_ActionMenuOpener_1.field_Private_Boolean_0;
		}

		// Token: 0x06000279 RID: 633 RVA: 0x0000EEFC File Offset: 0x0000D0FC
		public static Player LocalPlayer()
		{
			return Player.Method_Internal_Static_get_Player_0();
		}

		// Token: 0x0600027A RID: 634 RVA: 0x0000EF14 File Offset: 0x0000D114
		public static string GetAvatarStatus(Player player)
		{
			string text = player.Method_Public_get_ApiAvatar_PDM_0().releaseStatus.ToLower();
			return (text == "public") ? ("<color=green>" + text + "</color>") : ("<color=red>" + text + "</color>");
		}

		// Token: 0x0600027B RID: 635 RVA: 0x0000EF68 File Offset: 0x0000D168
		public static string GetPlatform(Player player)
		{
			bool flag = !(player != null);
			string text;
			if (flag)
			{
				text = "";
			}
			else
			{
				bool isOnMobile = player.field_Private_APIUser_0.IsOnMobile;
				if (isOnMobile)
				{
					text = "<color=green>Quest</color>";
				}
				else
				{
					text = (PlayerUtil.GetVrcPlayerApi(player).IsUserInVR() ? "<color=#CE00D5>VR</color>" : "<color=grey>PC</color>");
				}
			}
			return text;
		}

		// Token: 0x0600027C RID: 636 RVA: 0x0000EFC4 File Offset: 0x0000D1C4
		public static string GetFramesColord(Player player)
		{
			float frames = PlayerUtil.GetFrames(player);
			bool flag = (double)frames > 80.0;
			string text;
			if (flag)
			{
				text = "<color=green>" + frames.ToString() + "</color>";
			}
			else
			{
				text = (((double)frames > 30.0) ? ("<color=yellow>" + frames.ToString() + "</color>") : ("<color=red>" + frames.ToString() + "</color>"));
			}
			return text;
		}

		// Token: 0x0600027D RID: 637 RVA: 0x0000F044 File Offset: 0x0000D244
		public static string GetPlayerRankTextColoured(Player player)
		{
			bool flag = player == null;
			string text;
			if (flag)
			{
				text = "";
			}
			else
			{
				text = "";
			}
			return text;
		}

		// Token: 0x0600027E RID: 638 RVA: 0x0000F070 File Offset: 0x0000D270
		public static string GetPingColord(Player player)
		{
			short ping = PlayerUtil.GetPing(player);
			bool flag = ping > 150;
			string text;
			if (flag)
			{
				text = "<color=red>" + ping.ToString() + "</color>";
			}
			else
			{
				text = ((ping > 75) ? ("<color=yellow>" + ping.ToString() + "</color>") : ("<color=green>" + ping.ToString() + "</color>"));
			}
			return text;
		}

		// Token: 0x0600027F RID: 639 RVA: 0x0000F0E4 File Offset: 0x0000D2E4
		public static Color Friend()
		{
			return VRCPlayer.field_Internal_Static_Color_7;
		}

		// Token: 0x06000280 RID: 640 RVA: 0x0000F0FC File Offset: 0x0000D2FC
		public static Color Trusted()
		{
			return VRCPlayer.field_Internal_Static_Color_6;
		}

		// Token: 0x06000281 RID: 641 RVA: 0x0000F114 File Offset: 0x0000D314
		public static Color Known()
		{
			return VRCPlayer.field_Internal_Static_Color_5;
		}

		// Token: 0x06000282 RID: 642 RVA: 0x0000F12C File Offset: 0x0000D32C
		public static Color User()
		{
			return VRCPlayer.field_Internal_Static_Color_4;
		}

		// Token: 0x06000283 RID: 643 RVA: 0x0000F144 File Offset: 0x0000D344
		public static Color NewUser()
		{
			return VRCPlayer.field_Internal_Static_Color_3;
		}

		// Token: 0x06000284 RID: 644 RVA: 0x0000F15C File Offset: 0x0000D35C
		public static Color Visitor()
		{
			return VRCPlayer.field_Internal_Static_Color_2;
		}

		// Token: 0x06000285 RID: 645 RVA: 0x0000F174 File Offset: 0x0000D374
		public static Color Troll()
		{
			return VRCPlayer.field_Internal_Static_Color_0;
		}

		// Token: 0x06000286 RID: 646 RVA: 0x0000F18C File Offset: 0x0000D38C
		public static float GetFrames(Player player)
		{
			return (player._playerNet.Method_Public_get_Byte_0() == 0) ? (-1f) : Mathf.Floor(1000f / (float)player._playerNet.Method_Public_get_Byte_0());
		}

		// Token: 0x06000287 RID: 647 RVA: 0x0000F1CC File Offset: 0x0000D3CC
		public static short GetPing(Player player)
		{
			return player._playerNet.field_Private_Int16_0;
		}

		// Token: 0x06000288 RID: 648 RVA: 0x0000F1EC File Offset: 0x0000D3EC
		public static bool ClientDetect(Player player)
		{
			return (double)PlayerUtil.GetFrames(player) > 200.0 || (double)PlayerUtil.GetFrames(player) < 1.0 || PlayerUtil.GetPing(player) > 665 || PlayerUtil.GetPing(player) < 0;
		}

		// Token: 0x06000289 RID: 649 RVA: 0x0000F23C File Offset: 0x0000D43C
		public static bool GetIsMaster(Player player)
		{
			bool flag = !(player != null);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				try
				{
					flag2 = PlayerUtil.GetVrcPlayerApi(player).isMaster;
				}
				catch
				{
					flag2 = false;
				}
			}
			return flag2;
		}

		// Token: 0x0600028A RID: 650 RVA: 0x0000F284 File Offset: 0x0000D484
		public static VRCPlayer GetLocalVRCPlayer()
		{
			return VRCPlayer.field_Internal_Static_VRCPlayer_0;
		}

		// Token: 0x0600028B RID: 651 RVA: 0x0000F29C File Offset: 0x0000D49C
		public static VRCPlayerApi GetVrcPlayerApi(Player player)
		{
			return (player != null) ? player.Method_Public_get_VRCPlayerApi_0() : null;
		}

		// Token: 0x0600028C RID: 652 RVA: 0x0000F284 File Offset: 0x0000D484
		public static VRCPlayer GetVRCPlayer()
		{
			return VRCPlayer.field_Internal_Static_VRCPlayer_0;
		}

		// Token: 0x0600028D RID: 653 RVA: 0x0000F2BC File Offset: 0x0000D4BC
		public static Player[] GetAllPlayers()
		{
			return PlayerManager.Method_Public_Static_get_PlayerManager_0().field_Private_List_1_Player_0.ToArray();
		}

		// Token: 0x0600028E RID: 654 RVA: 0x0000F2E4 File Offset: 0x0000D4E4
		public static Player GetPlayerNewtworkedId(this int id)
		{
			return (from player in PlayerUtil.GetAllPlayers()
				where player.Method_Internal_get_PlayerNet_0().Method_Public_get_Int32_0() == id
				select player).FirstOrDefault<Player>();
		}

		// Token: 0x0600028F RID: 655 RVA: 0x0000F320 File Offset: 0x0000D520
		public static APIUser GetUserById(string userid)
		{
			return (from player in PlayerManager.Method_Public_Static_get_PlayerManager_0().field_Private_List_1_Player_0.ToArray()
				where player.field_Private_APIUser_0.id == userid
				select player).FirstOrDefault<Player>().field_Private_APIUser_0;
		}

		// Token: 0x06000290 RID: 656 RVA: 0x0000F36C File Offset: 0x0000D56C
		public static bool IsFriend(Player player)
		{
			return APIUser.CurrentUser.friendIDs.Contains(player.field_Private_APIUser_0.id);
		}

		// Token: 0x06000291 RID: 657 RVA: 0x0000F398 File Offset: 0x0000D598
		public static Player GetPlayer(PlayerManager playerManager, int actorId)
		{
			Player[] players = PlayerExtensions.GetPlayers(playerManager);
			Player[] array = players;
			foreach (Player player in array)
			{
				bool flag = !(player == null) && player.Method_Public_get_Int32_PDM_0() == actorId;
				if (flag)
				{
					return player;
				}
			}
			return null;
		}

		// Token: 0x06000292 RID: 658 RVA: 0x0000F3F4 File Offset: 0x0000D5F4
		internal static APIUser GetAPIUser(Player player)
		{
			return player.field_Private_APIUser_0;
		}

		// Token: 0x06000293 RID: 659 RVA: 0x0000F40C File Offset: 0x0000D60C
		public static Color GetPlayerTrustColor(List<string> tags)
		{
			bool flag = tags == null;
			Color color;
			if (flag)
			{
				color = Color.white;
			}
			else
			{
				foreach (string text in tags)
				{
					bool flag2 = text.Equals("system_trust_troll", StringComparison.OrdinalIgnoreCase);
					if (flag2)
					{
						return PlayerUtil.Troll();
					}
					bool flag3 = text.Equals("system_trust_veteran", StringComparison.OrdinalIgnoreCase);
					if (flag3)
					{
						return PlayerUtil.Trusted();
					}
					bool flag4 = text.Equals("system_trust_trusted", StringComparison.OrdinalIgnoreCase);
					if (flag4)
					{
						return PlayerUtil.Known();
					}
					bool flag5 = text.Equals("system_trust_known", StringComparison.OrdinalIgnoreCase);
					if (flag5)
					{
						return PlayerUtil.User();
					}
					bool flag6 = text.Equals("system_trust_basic", StringComparison.OrdinalIgnoreCase);
					if (flag6)
					{
						return PlayerUtil.NewUser();
					}
				}
				color = PlayerUtil.Visitor();
			}
			return color;
		}

		// Token: 0x06000294 RID: 660 RVA: 0x0000F4DC File Offset: 0x0000D6DC
		public static Player GetPlayerInformationById(int index)
		{
			Player[] allPlayers = PlayerUtil.GetAllPlayers();
			Player[] array = allPlayers;
			foreach (Player player in array)
			{
				bool flag = player._playerNet.Method_Public_get_Int32_0() == index;
				if (flag)
				{
					return player;
				}
			}
			return null;
		}

		// Token: 0x06000295 RID: 661 RVA: 0x00002E35 File Offset: 0x00001035
		public static IEnumerator NameplateColours(Player player, bool bypass = false, float time = 10f)
		{
			bool flag = !(player != null);
			if (flag)
			{
				yield break;
			}
			bool flag2 = !bypass;
			if (flag2)
			{
				yield return new WaitForSecondsRealtime(time);
			}
			try
			{
				PlayerNameplate nameplate = player._vrcplayer.field_Public_PlayerNameplate_0;
				List<string> rank = player.field_Private_APIUser_0.tags;
				bool troll = false;
				bool trusted = false;
				bool known = false;
				bool user = false;
				bool newUser = false;
				List<string>.Enumerator enumerator = rank.GetEnumerator();
				while (enumerator.MoveNext())
				{
					string tag = enumerator._current;
					bool flag3 = tag == "system_trust_troll";
					if (flag3)
					{
						troll = true;
					}
					bool flag4 = tag == "system_trust_veteran";
					if (flag4)
					{
						trusted = true;
					}
					bool flag5 = tag == "system_trust_trusted";
					if (flag5)
					{
						known = true;
					}
					bool flag6 = tag == "system_trust_known";
					if (flag6)
					{
						user = true;
					}
					bool flag7 = tag == "system_trust_basic";
					if (flag7)
					{
						newUser = true;
					}
					tag = null;
				}
				bool flag8 = troll;
				if (flag8)
				{
					nameplate.field_Public_Color_0 = PlayerUtil.Troll();
				}
				else
				{
					bool flag9 = trusted;
					if (flag9)
					{
						nameplate.field_Public_Color_0 = PlayerUtil.Trusted();
					}
					else
					{
						bool flag10 = known;
						if (flag10)
						{
							nameplate.field_Public_Color_0 = PlayerUtil.Known();
						}
						else
						{
							bool flag11 = user;
							if (flag11)
							{
								nameplate.field_Public_Color_0 = PlayerUtil.User();
							}
							else
							{
								bool flag12 = newUser;
								if (flag12)
								{
									nameplate.field_Public_Color_0 = PlayerUtil.NewUser();
								}
							}
						}
					}
				}
				nameplate = null;
				rank = null;
				enumerator = null;
				yield break;
			}
			catch
			{
				yield break;
			}
			yield break;
		}

		// Token: 0x0400018B RID: 395
		private static bool _isInVrCache;

		// Token: 0x0400018C RID: 396
		private static bool _cachedVRState;

		// Token: 0x0400018D RID: 397
		public static Color defaultNameplateColor;

		// Token: 0x0400018E RID: 398
		public static List<string> knownBlocks = new List<string>();

		// Token: 0x0400018F RID: 399
		public static List<string> knownMutes = new List<string>();

		// Token: 0x02000064 RID: 100
		// (Invoke) Token: 0x06000298 RID: 664
		internal delegate void AlignTrackingToPlayerDelegate();
	}
}
