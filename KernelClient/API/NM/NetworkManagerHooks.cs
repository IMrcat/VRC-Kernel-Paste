using System;
using System.Diagnostics;
using MelonLoader;
using UnityEngine.Events;
using VRC;
using VRC.Core;

namespace KernelClient.API.NM
{
	// Token: 0x0200007B RID: 123
	public static class NetworkManagerHooks
	{
		// Token: 0x14000002 RID: 2
		// (add) Token: 0x0600033C RID: 828 RVA: 0x00011750 File Offset: 0x0000F950
		// (remove) Token: 0x0600033D RID: 829 RVA: 0x00011784 File Offset: 0x0000F984
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<Player> OnJoin;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x0600033E RID: 830 RVA: 0x000117B8 File Offset: 0x0000F9B8
		// (remove) Token: 0x0600033F RID: 831 RVA: 0x000117EC File Offset: 0x0000F9EC
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<Player> OnLeave;

		// Token: 0x06000340 RID: 832 RVA: 0x00011820 File Offset: 0x0000FA20
		public static void EventHandlerA(Player player)
		{
			bool flag = !NetworkManagerHooks.SeenFire;
			if (flag)
			{
				NetworkManagerHooks.AFiredFirst = true;
				NetworkManagerHooks.SeenFire = true;
			}
			bool flag2 = !(player == null);
			if (flag2)
			{
				Action<Player> action = (NetworkManagerHooks.AFiredFirst ? NetworkManagerHooks.OnJoin : NetworkManagerHooks.OnLeave);
				if (action != null)
				{
					action(player);
				}
			}
		}

		// Token: 0x06000341 RID: 833 RVA: 0x00011878 File Offset: 0x0000FA78
		public static void EventHandlerB(Player player)
		{
			bool flag = !NetworkManagerHooks.SeenFire;
			if (flag)
			{
				NetworkManagerHooks.AFiredFirst = false;
				NetworkManagerHooks.SeenFire = true;
			}
			bool flag2 = !(player == null);
			if (flag2)
			{
				Action<Player> action = (NetworkManagerHooks.AFiredFirst ? NetworkManagerHooks.OnLeave : NetworkManagerHooks.OnJoin);
				if (action != null)
				{
					action(player);
				}
			}
		}

		// Token: 0x06000342 RID: 834 RVA: 0x000118D0 File Offset: 0x0000FAD0
		public static void Initialize()
		{
			MelonLogger.Msg("NetworkManagerHooks: initializing");
			bool isInitialized = NetworkManagerHooks.IsInitialized;
			if (isInitialized)
			{
				MelonLogger.Msg("NetworkManagerHooks: Already initialized");
			}
			else
			{
				bool flag = NetworkManager.field_Internal_Static_NetworkManager_0 == null;
				if (flag)
				{
					MelonLogger.Error("NetworkManagerHooks: NetworkManager instance is null");
				}
				else
				{
					VRCEventDelegate<Player> onPlayerJoinedDelegate = NetworkManager.field_Internal_Static_NetworkManager_0.OnPlayerJoinedDelegate;
					MelonLogger.Msg("NetworkManagerHooks: Initialized");
					VRCEventDelegate<Player> onPlayerLeaveDelegate = NetworkManager.field_Internal_Static_NetworkManager_0.OnPlayerLeaveDelegate;
					NetworkManagerHooks.AddDelegate(onPlayerJoinedDelegate, new Action<Player>(NetworkManagerHooks.EventHandlerA));
					NetworkManagerHooks.AddDelegate(onPlayerLeaveDelegate, new Action<Player>(NetworkManagerHooks.EventHandlerB));
					NetworkManagerHooks.IsInitialized = true;
				}
			}
		}

		// Token: 0x06000343 RID: 835 RVA: 0x000031C3 File Offset: 0x000013C3
		private static void AddDelegate(VRCEventDelegate<Player> field, UnityAction<Player> eventHandlerA)
		{
			field.field_Private_HashSet_1_UnityAction_1_T_0.Add(eventHandlerA);
		}

		// Token: 0x04000216 RID: 534
		private static bool IsInitialized;

		// Token: 0x04000217 RID: 535
		private static bool SeenFire;

		// Token: 0x04000218 RID: 536
		private static bool AFiredFirst;
	}
}
