using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using BestHTTP;
using BestHTTP.Cookies;
using ExitGames.Client.Photon;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using Photon.Realtime;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.Economy;
using VRC.Economy.Internal;
using VRC.SDKBase;
using VRC.Udon;

namespace KernelClient
{
	// Token: 0x02000050 RID: 80
	internal static class Patches
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060001DD RID: 477 RVA: 0x00002B2A File Offset: 0x00000D2A
		// (set) Token: 0x060001DE RID: 478 RVA: 0x00002B31 File Offset: 0x00000D31
		public static Harmony Harmony { get; set; }

		// Token: 0x060001DF RID: 479 RVA: 0x00002B39 File Offset: 0x00000D39
		private static void ForceClone(ref bool __0)
		{
			__0 = true;
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x0000C3F8 File Offset: 0x0000A5F8
		private static void NewTimeRealTimeDouble(ref double __0)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			__0 = stopwatch.Elapsed.TotalSeconds;
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x00002B3E File Offset: 0x00000D3E
		private static void FavStop(ref bool __0)
		{
			__0 = false;
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x00002B43 File Offset: 0x00000D43
		private static void PatchApiAvatar(ref ApiAvatar __0)
		{
			MelonCoroutines.Start(Patches.WaitForAviToInitialize(__0));
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x00002B53 File Offset: 0x00000D53
		private static IEnumerator WaitForAviToInitialize(ApiAvatar avi)
		{
			bool modulesLoaded = Patches.ModulesLoaded;
			if (modulesLoaded)
			{
				while (avi == null)
				{
					yield return new WaitForEndOfFrame();
				}
				while (string.IsNullOrEmpty(avi.name))
				{
					Dictionary<string, int> loadAvatars = Patches.LoadAvatars;
					IntPtr pointer = avi.Pointer;
					string key = pointer.ToString();
					bool flag = !loadAvatars.ContainsKey(key);
					if (flag)
					{
						Dictionary<string, int> loadAvatars2 = Patches.LoadAvatars;
						pointer = avi.Pointer;
						string key2 = pointer.ToString();
						loadAvatars2.Add(key2, 0);
						loadAvatars2 = null;
						key2 = null;
					}
					Dictionary<string, int> loadAvatars3 = Patches.LoadAvatars;
					pointer = avi.Pointer;
					string key3 = pointer.ToString();
					bool flag2 = loadAvatars3[key3] == 10;
					if (flag2)
					{
						yield break;
					}
					Dictionary<string, int> loadAvatars4 = Patches.LoadAvatars;
					pointer = avi.Pointer;
					string key4 = pointer.ToString();
					Dictionary<string, int> dictionary = loadAvatars4;
					string text = key4;
					int num = dictionary[text];
					dictionary[text] = num + 1;
					yield return new WaitForEndOfFrame();
					loadAvatars4 = null;
					key4 = null;
					loadAvatars = null;
					key = null;
					loadAvatars3 = null;
					key3 = null;
				}
				Patches.LoadAvatars.Remove(avi.Pointer.ToString());
				foreach (KernelModule i in Loader.Modules)
				{
					i.OnApiAvatar(avi);
					i = null;
				}
				List<KernelModule>.Enumerator enumerator = default(List<KernelModule>.Enumerator);
			}
			yield break;
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000C41C File Offset: 0x0000A61C
		private static bool FakeHWID(ref string __result)
		{
			string text = (from x in new HMACSHA256().ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}A-{1}{2}-{3}{4}-{5}{6}-3C-1F", new object[]
				{
					new Random().Next(0, 9),
					new Random().Next(0, 9),
					new Random().Next(0, 9),
					new Random().Next(0, 9),
					new Random().Next(0, 9),
					new Random().Next(0, 9),
					new Random().Next(0, 9)
				})))
				select x.ToString("x2")).Aggregate((string x, string y) => x + y);
			MelonLogger.Msg("[HWID] new " + text);
			__result = text;
			return false;
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0000C544 File Offset: 0x0000A744
		private static void GetVRCPlusStatus(ref Object1PublicTYBoTYUnique<bool> __result)
		{
			bool flag = __result == null;
			if (!flag)
			{
				__result.Method_Public_Virtual_New_set_Void_TYPE_0(true);
			}
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0000C568 File Offset: 0x0000A768
		public static void RegisterPatches()
		{
			Patches.Harmony = MelonHandler.Mods.First((MelonMod m) => m.Info.Name == "KernelClient").HarmonyInstance;
			try
			{
				Patches.Harmony.Patch(typeof(RoomManager).GetMethod("Method_Public_Static_Boolean_ApiWorld_ApiWorldInstance_String_Int32_0"), null, Patches.GetLocalPatch("EnterWorldPatch"), null, null, null);
				MelonLogger.Msg("patched RoomManager");
			}
			catch
			{
				MelonLogger.Msg("Error patching RoomManager");
			}
			try
			{
				Patches.Harmony.Patch(typeof(ApiUserPermissions).GetMethod("GetPermission_ExtraFavoriteAvatars"), null, Patches.GetLocalPatch("ExtraFav"), null, null, null);
				MelonLogger.Msg("patched ApiUserPermissions");
			}
			catch
			{
				MelonLogger.Msg("Error patching ApiUserPermissions");
			}
			try
			{
				Patches.Harmony.Patch(typeof(VRCPlusStatus).GetProperty("prop_Object1PublicTYBoTYUnique_1_Boolean_0").GetGetMethod(), null, Patches.GetLocalPatch("GetVRCPlusStatus"), null, null, null);
				MelonLogger.Msg("patched VRC+");
			}
			catch
			{
				MelonLogger.Msg("Error patching VRC+");
			}
			try
			{
				Patches.Harmony.Patch(typeof(UdonBehaviour).GetMethods().First((MethodInfo m) => m.Name.Equals("RunProgram") && m.GetParameters()[0].ParameterType == typeof(string)), Patches.GetLocalPublicPatch("OnUdonPatch"), null, null, null, null);
				MelonLogger.Msg("patched Udon");
			}
			catch
			{
				MelonLogger.Msg("Error patching UdonBehaviour");
			}
			try
			{
				Harmony harmony = Patches.Harmony;
				PropertyInfo property = typeof(APIUser).GetProperty("allowAvatarCopying");
				harmony.Patch((property != null) ? property.GetSetMethod() : null, new HarmonyMethod(typeof(Patches).GetMethod("ForceClone", BindingFlags.Static | BindingFlags.NonPublic)), null, null, null, null);
				MelonLogger.Msg("patched allowAvatarCopying");
			}
			catch
			{
				MelonLogger.Msg("Error patching allowAvatarCopying");
			}
			try
			{
				Patches.Harmony.Patch(typeof(Store).GetMethod("Method_Private_Boolean_VRCPlayerApi_IProduct_PDM_0"), new HarmonyMethod(typeof(Patches).GetMethod("MarketPatch", BindingFlags.Static | BindingFlags.NonPublic)), null, null, null, null);
				MelonLogger.Msg("patched store");
			}
			catch
			{
				MelonLogger.Msg("Error patching Store");
			}
			try
			{
				Patches.Harmony.Patch(typeof(SystemInfo).GetProperty("deviceUniqueIdentifier").GetGetMethod(), new HarmonyMethod(AccessTools.Method(typeof(Patches), "FakeHWID", null, null)), null, null, null, null);
				MelonLogger.Msg("patched deviceUniqueIdentifier");
			}
			catch
			{
				MelonLogger.Msg("Error patching SystemInfo");
			}
			try
			{
				Patches.Harmony.Patch(typeof(LoadBalancingClient).GetMethod("OnEvent"), new HarmonyMethod(typeof(Patches).GetMethod("OnEventPatch", BindingFlags.Static | BindingFlags.NonPublic)), null, null, null, null);
				MelonLogger.Msg("patched Events");
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Error on patching events\n" + ex.Message);
			}
			try
			{
				Patches.Harmony.Patch(typeof(LoadBalancingClient).GetMethod("Method_Public_Virtual_New_Boolean_Byte_Object_RaiseEventOptions_SendOptions_0"), Patches.GetLocalPatch("Patch_OnEventSent"), null, null, null, null);
				MelonLogger.Msg("patched Oneventsend");
			}
			catch (Exception ex2)
			{
				MelonLogger.Error("Failed to patch OnEventSent! | " + ex2.Message);
			}
			try
			{
				Patches.Harmony.Patch(typeof(VRCNetworkingClient).GetMethod("OnEvent"), new HarmonyMethod(typeof(Patches).GetMethod("OnEventPatchVRC", BindingFlags.Static | BindingFlags.NonPublic)), null, null, null, null);
				MelonLogger.Msg("patched VRC patching events");
			}
			catch (Exception ex3)
			{
				MelonLogger.Error("Error on VRC patching events\n" + ex3.Message);
			}
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x00002B62 File Offset: 0x00000D62
		private static void RequestSent(HTTPRequest __0)
		{
			MelonCoroutines.Start(Patches.RequestSentAsync(__0));
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x00002B71 File Offset: 0x00000D71
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static IEnumerator RequestSentAsync(HTTPRequest request)
		{
			while (request.State == 2)
			{
				yield return null;
			}
			bool flag = request.Uri.ToString().Contains("user");
			if (flag)
			{
				MelonLogger.Msg("----------Request captured----------");
				MelonLogger.Msg(request.Uri.ToString());
				Patches.GetHeaders(request);
				Patches.GetCookies(request);
				Patches.GetResponse(request);
			}
			yield break;
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000C9D4 File Offset: 0x0000ABD4
		private static void GetHeaders(HTTPRequest request)
		{
			bool flag = request.Headers == null;
			if (!flag)
			{
				MelonLogger.Msg("Headers:");
			}
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0000C9FC File Offset: 0x0000ABFC
		private static void GetCookies(HTTPRequest request)
		{
			bool flag = request.Cookies == null;
			if (!flag)
			{
				MelonLogger.Msg("Cookies:");
				foreach (Cookie cookie in request.Cookies)
				{
					MelonLogger.Msg(cookie.Name + " : " + cookie.Value);
				}
			}
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000CA60 File Offset: 0x0000AC60
		private static void GetResponse(HTTPRequest request)
		{
			bool flag = request.Response == null;
			if (flag)
			{
				MelonLogger.Msg("No response");
			}
			else
			{
				MelonLogger.Msg("Response:");
				bool flag2 = request.Response.dataAsText != null;
				if (flag2)
				{
					MelonLogger.Msg(request.Response.dataAsText);
				}
				else
				{
					MelonLogger.Msg("Is file with length: " + request.Response.Data.Length.ToString());
				}
			}
		}

		// Token: 0x060001EC RID: 492 RVA: 0x00002B39 File Offset: 0x00000D39
		private static void ExtraFav(ref bool __result)
		{
			__result = true;
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0000CAE4 File Offset: 0x0000ACE4
		private static bool CertPatch(ref string __result)
		{
			__result = "047670F97BA832B1F527D313FC5FA85D3AAFDB751763B8DC59C782E4AABD1F067B53295CCE1A3BD6CBD1611D688A75FBAFDB970276B51B3888BD4AD94264BC30F0";
			return false;
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000CB00 File Offset: 0x0000AD00
		private static bool MarketPatch(VRCPlayerApi __0, IProduct __1, ref bool __result)
		{
			__result = true;
			return false;
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0000CB18 File Offset: 0x0000AD18
		private static bool Patch_OnEventSent(byte __0, object __1, RaiseEventOptions __2, SendOptions __3)
		{
			bool flag = true;
			foreach (KernelModule kernelModule in Loader.Modules)
			{
				bool flag2 = !kernelModule.OnEventSent(__0, __1, __2, __3);
				if (flag2)
				{
					return false;
				}
			}
			return flag;
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000CB88 File Offset: 0x0000AD88
		private static void OnAvatarInstantiate(GameObject avatar, VRCPlayer vrcPlayer)
		{
			bool flag = avatar == null || vrcPlayer == null;
			if (!flag)
			{
				foreach (KernelModule kernelModule in Loader.Modules)
				{
					kernelModule.OnAvatarInstantiate(avatar, vrcPlayer);
				}
			}
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0000CBF8 File Offset: 0x0000ADF8
		public static void InitializeNetworkManager()
		{
			VRCEventDelegate<Player> onPlayerJoinedDelegate = NetworkManager.field_Internal_Static_NetworkManager_0.OnPlayerJoinedDelegate;
			VRCEventDelegate<Player> onPlayerAwakeDelegate = NetworkManager.field_Internal_Static_NetworkManager_0.OnPlayerAwakeDelegate;
			VRCEventDelegate<Player> onPlayerLeaveDelegate = NetworkManager.field_Internal_Static_NetworkManager_0.OnPlayerLeaveDelegate;
			onPlayerJoinedDelegate.field_Private_HashSet_1_UnityAction_1_T_0.Add(delegate(Player p)
			{
				bool flag = !(p != null);
				if (!flag)
				{
					Patches.OnPlayerJoined(p);
				}
			});
			onPlayerAwakeDelegate.field_Private_HashSet_1_UnityAction_1_T_0.Add(delegate(Player p)
			{
				bool flag2 = !(p != null);
				if (!flag2)
				{
					Patches.OnPlayerAwake(p);
				}
			});
			onPlayerLeaveDelegate.field_Private_HashSet_1_UnityAction_1_T_0.Add(delegate(Player p)
			{
				bool flag3 = !(p != null);
				if (!flag3)
				{
					Patches.OnPlayerLeft(p);
				}
			});
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0000CCB8 File Offset: 0x0000AEB8
		private static bool OnEventPatchVRC(ref EventData __0)
		{
			foreach (KernelModule kernelModule in Loader.Modules)
			{
				bool flag = !kernelModule.OnEventPatchVRC(ref __0);
				if (flag)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0000CD20 File Offset: 0x0000AF20
		private static bool OnEventPatch(EventData __0)
		{
			bool flag = !Enum.IsDefined(typeof(EventCodes), __0.Code);
			if (flag)
			{
				MelonLogger.Msg(string.Format("Unknown Event Found: {0} VRCNetworkingClientReceiveEventPatch", __0.Code));
			}
			foreach (KernelModule kernelModule in Loader.Modules)
			{
				bool flag2 = !kernelModule.OnEventPatch(__0);
				if (flag2)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000CDC8 File Offset: 0x0000AFC8
		private static HarmonyMethod GetLocalPatch(string name)
		{
			HarmonyMethod harmonyMethod;
			try
			{
				harmonyMethod = MelonUtils.ToNewHarmonyMethod(typeof(Patches).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic));
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("{0}: {1}", name, ex));
				harmonyMethod = null;
			}
			return harmonyMethod;
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000CE1C File Offset: 0x0000B01C
		private static HarmonyMethod GetLocalPublicPatch(string name)
		{
			return MelonUtils.ToNewHarmonyMethod(typeof(Patches).GetMethod(name, BindingFlags.Static | BindingFlags.Public));
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000CE48 File Offset: 0x0000B048
		private static void VRCPlayerAwakePatch(VRCPlayer __instance)
		{
			bool flag = !(__instance == null);
			if (flag)
			{
			}
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000CE68 File Offset: 0x0000B068
		private static void EnterWorldPatch(ApiWorld __0, ApiWorldInstance __1)
		{
			bool flag = __0 == null || __1 == null;
			if (!flag)
			{
				MelonCoroutines.Start(Patches.WaitForWorldToInitialize());
				foreach (KernelModule kernelModule in Loader.Modules)
				{
					kernelModule.OnEnterWorld(__0, __1);
				}
			}
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000CEDC File Offset: 0x0000B0DC
		private static void OnPlayerAwake(Player player)
		{
			bool flag = player == null;
			if (!flag)
			{
				foreach (KernelModule kernelModule in Loader.Modules)
				{
					kernelModule.OnPlayerAwake(player);
				}
			}
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000CF40 File Offset: 0x0000B140
		private static void OnPlayerJoined(Player player)
		{
			bool flag = player == null;
			if (!flag)
			{
				foreach (KernelModule kernelModule in Loader.Modules)
				{
					kernelModule.OnPlayerJoined(player);
				}
			}
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0000CFA4 File Offset: 0x0000B1A4
		private static void OnPlayerLeft(Player player)
		{
			bool flag = player == null;
			if (!flag)
			{
				foreach (KernelModule kernelModule in Loader.Modules)
				{
					kernelModule.OnPlayerLeft(player);
				}
			}
		}

		// Token: 0x060001FB RID: 507 RVA: 0x0000D008 File Offset: 0x0000B208
		public static bool OnUdonPatch(UdonBehaviour __instance, string __0)
		{
			foreach (KernelModule kernelModule in Loader.Modules)
			{
				bool flag = !kernelModule.OnUdonPatch(__instance, __0);
				if (flag)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060001FC RID: 508 RVA: 0x00002B80 File Offset: 0x00000D80
		private static IEnumerator WaitForWorldToInitialize()
		{
			while (Networking.LocalPlayer == null)
			{
				yield return new WaitForEndOfFrame();
			}
			yield break;
		}

		// Token: 0x0400013C RID: 316
		public static bool ModulesLoaded = false;

		// Token: 0x0400013D RID: 317
		private static Dictionary<string, int> LoadAvatars = new Dictionary<string, int>();
	}
}
