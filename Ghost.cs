using System;
using ExitGames.Client.Photon;
using Il2CppSystem.Collections.Generic;
using KernelClient.Wrapper;
using MelonLoader;
using Photon.Realtime;
using ReMod.Core.UI.QuickMenu;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KernelClient
{
	// Token: 0x0200004D RID: 77
	internal class Ghost : KernelModule
	{
		// Token: 0x060001CB RID: 459 RVA: 0x0000BF1C File Offset: 0x0000A11C
		public override void OnUiManagerInit()
		{
			ReMenuCategory category = MenuSetup._uiManager.QMMenu.GetCategoryPage(PageNames.Utility).GetCategory(CatagoryNames.Movement);
			category.AddToggle("Ghost Mode", "SHHHH you're a ghost (T)", delegate(bool s)
			{
				this.ToggleGhost(s);
			});
		}

		// Token: 0x060001CC RID: 460 RVA: 0x00002A96 File Offset: 0x00000C96
		public override void OnUpdate()
		{
			this.ghostbind();
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000BF68 File Offset: 0x0000A168
		private void ghostbind()
		{
			bool keyDown = Input.GetKeyDown(116);
			if (keyDown)
			{
				bool flag = this.ghostMode;
				if (flag)
				{
					this.ToggleGhost(false);
					ToastNotif.Toast("Ghost Mode is disabled", null, null, 5f);
				}
				else
				{
					this.ToggleGhost(true);
					ToastNotif.Toast("Ghost Mode is enabled", null, null, 5f);
				}
			}
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000BFC8 File Offset: 0x0000A1C8
		public override bool OnEventSent(byte code, object data, RaiseEventOptions options, SendOptions sendOptions)
		{
			bool flag = code == 12;
			if (flag)
			{
				bool flag2 = this.ghostMode;
				if (flag2)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000BFF8 File Offset: 0x0000A1F8
		private void ToggleGhost(bool enable)
		{
			this.ghostMode = enable;
			MelonLogger.Msg("Ghost mode: " + enable.ToString());
			bool flag = enable;
			if (flag)
			{
				GameObject gameObject = null;
				foreach (GameObject gameObject2 in SceneManager.GetActiveScene().GetRootGameObjects())
				{
					bool flag2 = gameObject2.name.StartsWith("VRCPlayer[Local]");
					if (flag2)
					{
						gameObject = gameObject2;
						break;
					}
				}
				bool flag3 = gameObject == null;
				if (!flag3)
				{
					this.origGhostPos = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
					this.origGhostRot = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.rotation;
					try
					{
						GameObject gameObject3 = Object.Instantiate<GameObject>(VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCAvatarManager_0.field_Private_GameObject_0, null, true);
						Animator component = gameObject3.GetComponent<Animator>();
						bool flag4 = component != null && component.isHuman;
						if (flag4)
						{
							Transform boneTransform = component.GetBoneTransform(10);
							bool flag5 = boneTransform != null;
							if (flag5)
							{
								boneTransform.localScale = Vector3.one;
							}
						}
						gameObject3.name = "Cloned Avatar";
						component.enabled = false;
						gameObject3.GetComponent<VRIK>().enabled = false;
						gameObject3.transform.position = gameObject.transform.position;
						gameObject3.transform.rotation = gameObject.transform.rotation;
						this.clonedAvatar.Add(gameObject3);
					}
					catch (Exception ex)
					{
						MelonLogger.Error("Failed to create ghost clone: " + ex.Message);
					}
				}
			}
			else
			{
				foreach (GameObject gameObject4 in this.clonedAvatar)
				{
					bool flag6 = gameObject4 != null;
					if (flag6)
					{
						Object.Destroy(gameObject4);
					}
				}
				this.clonedAvatar.Clear();
			}
		}

		// Token: 0x04000135 RID: 309
		private bool ghostMode = false;

		// Token: 0x04000136 RID: 310
		private Vector3 origGhostPos;

		// Token: 0x04000137 RID: 311
		private Quaternion origGhostRot;

		// Token: 0x04000138 RID: 312
		private List<GameObject> clonedAvatar = new List<GameObject>();
	}
}
