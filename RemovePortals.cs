using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KernelClient.Wrapper;
using MelonLoader;
using ReMod.Core.UI.QuickMenu;
using UnityEngine;
using VRC.Core;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace KernelClient
{
	// Token: 0x02000059 RID: 89
	public class RemovePortals : KernelModule
	{
		// Token: 0x06000233 RID: 563 RVA: 0x0000E10C File Offset: 0x0000C30C
		private void CheckAndStartRemovals()
		{
			try
			{
				bool globalPortalRemoval = RemovePortals._globalPortalRemoval;
				if (globalPortalRemoval)
				{
					this.StartGlobalPortalRemoval();
				}
				bool flag = RemovePortals._globalPortalRemovalForAll && Networking.IsMaster;
				if (flag)
				{
					this.StartGlobalPortalRemovalForAll();
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Error in CheckAndStartRemovals: " + ex.Message);
			}
		}

		// Token: 0x06000234 RID: 564 RVA: 0x0000E174 File Offset: 0x0000C374
		public override void OnUiManagerInit()
		{
			ReMenuCategory category = MenuSetup._uiManager.QMMenu.GetCategoryPage(PageNames.Utility).GetCategory(CatagoryNames.Other);
			category.AddToggle("Instant Remove", "Removes portals immediately when spawned", delegate(bool state)
			{
				RemovePortals._instantRemoval = state;
				if (state)
				{
					ObjectHandler.OnObjectInstantiated += this.OnPortalSpawned;
					ToastNotif.Toast("Instant Portal Removal", "Portals will be removed on spawn", null, 5f);
				}
				else
				{
					ObjectHandler.OnObjectInstantiated -= this.OnPortalSpawned;
					ToastNotif.Toast("Instant Portal Removal", "Disabled", null, 5f);
				}
			}, RemovePortals._instantRemoval);
			category.AddToggle("Global Remove", "Continuously removes portals for you", delegate(bool state)
			{
				RemovePortals._globalPortalRemoval = state;
				if (state)
				{
					this.StartGlobalPortalRemoval();
					ToastNotif.Toast("Global Portal Removal", "Enabled", null, 5f);
				}
				else
				{
					this.StopGlobalPortalRemoval();
					ToastNotif.Toast("Global Portal Removal", "Disabled", null, 5f);
				}
			}, RemovePortals._globalPortalRemoval);
			category.AddToggle("Remove For All", "Removes portals for everyone (Master Only)", delegate(bool state)
			{
				bool flag = state && !Networking.IsMaster;
				if (flag)
				{
					ToastNotif.Toast("Error", "Must be instance master", null, 5f);
				}
				else
				{
					RemovePortals._globalPortalRemovalForAll = state;
					if (state)
					{
						this.StartGlobalPortalRemovalForAll();
						ToastNotif.Toast("Global Portal Removal", "Enabled for all users", null, 5f);
					}
					else
					{
						this.StopGlobalPortalRemovalForAll();
						ToastNotif.Toast("Global Portal Removal", "Disabled for all users", null, 5f);
					}
				}
			}, RemovePortals._globalPortalRemovalForAll);
			category.AddButton("Clear All", "Manually removes all portals", delegate
			{
				bool flag2 = RemovePortals._globalPortalRemovalForAll && Networking.IsMaster;
				if (flag2)
				{
					this.RemoveAllPortalsNetworked();
				}
				else
				{
					this.RemoveAllPortals();
				}
			}, null, "#ffffff");
		}

		// Token: 0x06000235 RID: 565 RVA: 0x0000E22C File Offset: 0x0000C42C
		private void OnPortalSpawned(GameObject obj)
		{
			try
			{
				bool flag = !RemovePortals._instantRemoval || obj == null;
				if (!flag)
				{
					bool flag2 = this.IsPortalObject(obj);
					if (flag2)
					{
						bool flag3 = RemovePortals._globalPortalRemovalForAll && Networking.IsMaster;
						if (flag3)
						{
							this.DeletePortalNetworked(obj);
						}
						else
						{
							this.DeletePortalLocal(obj);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Error in OnPortalSpawned: " + ex.Message);
			}
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0000E2B8 File Offset: 0x0000C4B8
		private bool IsPortalObject(GameObject obj)
		{
			bool flag = obj == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = RemovePortals._portalNames.Contains(obj.name);
				flag2 = flag3 || obj.GetComponent<VRCPortalMarker>() != null || obj.GetComponent<PortalTrigger>() != null || obj.GetComponent<VRC_PortalMarker>() != null;
			}
			return flag2;
		}

		// Token: 0x06000237 RID: 567 RVA: 0x00002CAF File Offset: 0x00000EAF
		private void StartGlobalPortalRemoval()
		{
			this.StopPortalCheckRoutine();
			RemovePortals._portalCheckRoutine = MelonCoroutines.Start(this.PortalCheckRoutine());
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000E31C File Offset: 0x0000C51C
		private void StartGlobalPortalRemovalForAll()
		{
			bool flag = !Networking.IsMaster;
			if (!flag)
			{
				this.StopPortalCheckRoutine();
				RemovePortals._portalCheckRoutine = MelonCoroutines.Start(this.PortalCheckRoutineForAll());
			}
		}

		// Token: 0x06000239 RID: 569 RVA: 0x00002CC9 File Offset: 0x00000EC9
		private void StopGlobalPortalRemoval()
		{
			this.StopPortalCheckRoutine();
		}

		// Token: 0x0600023A RID: 570 RVA: 0x00002CC9 File Offset: 0x00000EC9
		private void StopGlobalPortalRemovalForAll()
		{
			this.StopPortalCheckRoutine();
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000E350 File Offset: 0x0000C550
		private void StopPortalCheckRoutine()
		{
			bool flag = RemovePortals._portalCheckRoutine != null;
			if (flag)
			{
				MelonCoroutines.Stop(RemovePortals._portalCheckRoutine);
				RemovePortals._portalCheckRoutine = null;
			}
		}

		// Token: 0x0600023C RID: 572 RVA: 0x00002CD3 File Offset: 0x00000ED3
		private IEnumerator PortalCheckRoutine()
		{
			WaitForSeconds wait = new WaitForSeconds(RemovePortals._checkInterval);
			while (RemovePortals._globalPortalRemoval)
			{
				this.RemoveAllPortals();
				yield return wait;
			}
			yield break;
		}

		// Token: 0x0600023D RID: 573 RVA: 0x00002CE2 File Offset: 0x00000EE2
		private IEnumerator PortalCheckRoutineForAll()
		{
			WaitForSeconds wait = new WaitForSeconds(RemovePortals._checkInterval);
			while (RemovePortals._globalPortalRemovalForAll && Networking.IsMaster)
			{
				this.RemoveAllPortalsNetworked();
				yield return wait;
			}
			yield break;
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0000E380 File Offset: 0x0000C580
		private void RemoveAllPortals()
		{
			try
			{
				int num = 0;
				List<GameObject> list = Object.FindObjectsOfType<GameObject>().Where(new Func<GameObject, bool>(this.IsPortalObject)).ToList<GameObject>();
				foreach (GameObject gameObject in list)
				{
					bool flag = gameObject != null;
					if (flag)
					{
						this.DeletePortalLocal(gameObject);
						num++;
					}
				}
				bool flag2 = num > 0;
				if (flag2)
				{
					ToastNotif.Toast(string.Format("Removed {0} portals", num), "", null, 5f);
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Error in RemoveAllPortals: " + ex.Message);
			}
		}

		// Token: 0x0600023F RID: 575 RVA: 0x0000E460 File Offset: 0x0000C660
		private void RemoveAllPortalsNetworked()
		{
			try
			{
				bool flag = !Networking.IsMaster;
				if (!flag)
				{
					int num = 0;
					List<GameObject> list = Object.FindObjectsOfType<GameObject>().Where(new Func<GameObject, bool>(this.IsPortalObject)).ToList<GameObject>();
					foreach (GameObject gameObject in list)
					{
						bool flag2 = gameObject != null;
						if (flag2)
						{
							this.DeletePortalNetworked(gameObject);
							num++;
						}
					}
					bool flag3 = num > 0;
					if (flag3)
					{
						ToastNotif.Toast(string.Format("Removed {0} portals for all", num), "", null, 5f);
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Error in RemoveAllPortalsNetworked: " + ex.Message);
			}
		}

		// Token: 0x06000240 RID: 576 RVA: 0x0000E554 File Offset: 0x0000C754
		private void DeletePortalLocal(GameObject portal)
		{
			bool flag = portal == null;
			if (!flag)
			{
				Object.DestroyImmediate(portal);
			}
		}

		// Token: 0x06000241 RID: 577 RVA: 0x0000E578 File Offset: 0x0000C778
		private void DeletePortalNetworked(GameObject portal)
		{
			try
			{
				bool flag = !Networking.IsMaster || portal == null;
				if (!flag)
				{
					Networking.Destroy(portal);
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Error in DeletePortalNetworked: " + ex.Message);
			}
		}

		// Token: 0x06000242 RID: 578 RVA: 0x00002CF1 File Offset: 0x00000EF1
		public override void OnEnterWorld(ApiWorld world, ApiWorldInstance instance)
		{
			this.CheckAndStartRemovals();
		}

		// Token: 0x0400015E RID: 350
		private static bool _globalPortalRemoval;

		// Token: 0x0400015F RID: 351
		private static bool _globalPortalRemovalForAll;

		// Token: 0x04000160 RID: 352
		private static bool _instantRemoval;

		// Token: 0x04000161 RID: 353
		private static float _checkInterval = 0.5f;

		// Token: 0x04000162 RID: 354
		private static object _portalCheckRoutine;

		// Token: 0x04000163 RID: 355
		private static readonly HashSet<string> _portalNames = new HashSet<string> { "PortalInternal(Clone)", "BasePortal(Clone)", "Default Portal(Clone)" };
	}
}
