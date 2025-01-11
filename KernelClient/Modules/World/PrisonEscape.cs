using System;
using System.Linq;
using Il2CppSystem;
using KernelClient.Wrapper;
using MelonLoader;
using ReMod.Core.UI.QuickMenu;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

namespace KernelClient.Modules.World
{
	// Token: 0x020000B5 RID: 181
	internal class PrisonEscape : KernelModule
	{
		// Token: 0x060004DD RID: 1245 RVA: 0x0001BF8C File Offset: 0x0001A18C
		public override void OnUiManagerInit()
		{
			ReMenuPage reMenuPage = MenuSetup._uiManager.QMMenu.GetCategoryPage(PageNames.WorldCheats).GetCategory(CatagoryNames.GameWorlds).AddMenuPage("Prison Escape", "", null, "#ffffff");
			reMenuPage.AddButton("Force Pickup", "", delegate
			{
				this.ForcePickup();
			}, null, "#ffffff");
			reMenuPage.AddButton("Get all weapons", "", delegate
			{
				this.GetAllWeapons();
			}, null, "#ffffff");
			reMenuPage.AddButton("Give Pistol", "", delegate
			{
				this.GivePistol();
			}, null, "#ffffff");
			reMenuPage.AddButton("ive ShotGun", "", delegate
			{
				this.GiveShotgun();
			}, null, "#ffffff");
			reMenuPage.AddButton("Give SMG", "", delegate
			{
				this.GiveSMG();
			}, null, "#ffffff");
			reMenuPage.AddButton("Give Sniper", "", delegate
			{
				this.GiveSniper();
			}, null, "#ffffff");
			reMenuPage.AddButton("Give Magnum", "", delegate
			{
				this.GiveMagnum();
			}, null, "#ffffff");
			reMenuPage.AddButton("Give M4A1", "", delegate
			{
				this.GiveM4A1();
			}, null, "#ffffff");
			reMenuPage.AddButton("Give Revolver", "", delegate
			{
				this.GiveRevolver();
			}, null, "#ffffff");
			reMenuPage.AddButton("Give Knife", "", delegate
			{
				this.GiveKnife();
			}, null, "#ffffff");
			reMenuPage.AddButton("Give KeyCard", "", delegate
			{
				this.GiveKeyCard();
			}, null, "#ffffff");
		}

		// Token: 0x060004DE RID: 1246 RVA: 0x0001C150 File Offset: 0x0001A350
		public void SpecificTarget(string udonEvent, string gameObject)
		{
			foreach (GameObject gameObject2 in Resources.FindObjectsOfTypeAll<GameObject>())
			{
				bool flag = gameObject2.name.Contains(gameObject);
				if (flag)
				{
					gameObject2.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, udonEvent);
				}
			}
		}

		// Token: 0x060004DF RID: 1247 RVA: 0x0001C1BC File Offset: 0x0001A3BC
		public void GetAllWeapons()
		{
			MelonLogger.Msg("Gave yourself all weapons");
			PrisonEscape.MurderGive("ShotGun");
			PrisonEscape.MurderGive("Knife");
			PrisonEscape.MurderGive("Pistol");
			PrisonEscape.MurderGive("SMG");
			PrisonEscape.MurderGive("Sniper");
			PrisonEscape.MurderGive("M4A1");
			PrisonEscape.MurderGive("Magnum");
		}

		// Token: 0x060004E0 RID: 1248 RVA: 0x00003D75 File Offset: 0x00001F75
		public void GiveKeyCard()
		{
			MelonLogger.Msg("Gave yourself a KeyCard");
			PrisonEscape.MurderGive("Keycard");
		}

		// Token: 0x060004E1 RID: 1249 RVA: 0x00003D8E File Offset: 0x00001F8E
		public void GiveKnife()
		{
			MelonLogger.Msg("Gave yourself a Knife");
			PrisonEscape.MurderGive("Knife");
		}

		// Token: 0x060004E2 RID: 1250 RVA: 0x00003DA7 File Offset: 0x00001FA7
		public void GivePistol()
		{
			MelonLogger.Msg("Gave yourself a Pistol");
			PrisonEscape.MurderGive("Pistol");
		}

		// Token: 0x060004E3 RID: 1251 RVA: 0x00003DC0 File Offset: 0x00001FC0
		public void GiveShotgun()
		{
			MelonLogger.Msg("Gave yourself a Shotgun");
			PrisonEscape.MurderGive("Shotgun");
		}

		// Token: 0x060004E4 RID: 1252 RVA: 0x00003DD9 File Offset: 0x00001FD9
		public void GiveSMG()
		{
			MelonLogger.Msg("Gave yourself a SMG");
			PrisonEscape.MurderGive("SMG");
		}

		// Token: 0x060004E5 RID: 1253 RVA: 0x00003DF2 File Offset: 0x00001FF2
		public void GiveSniper()
		{
			MelonLogger.Msg("Gave yourself a Sniper");
			PrisonEscape.MurderGive("Sniper");
		}

		// Token: 0x060004E6 RID: 1254 RVA: 0x00003E0B File Offset: 0x0000200B
		public void GiveMagnum()
		{
			MelonLogger.Msg("Gave yourself a Magnum");
			PrisonEscape.MurderGive("Magnum");
		}

		// Token: 0x060004E7 RID: 1255 RVA: 0x00003E24 File Offset: 0x00002024
		public void GiveM4A1()
		{
			MelonLogger.Msg("Killed Everyone");
			PrisonEscape.MurderGive("M4A1");
		}

		// Token: 0x060004E8 RID: 1256 RVA: 0x00003E3D File Offset: 0x0000203D
		public void GiveRevolver()
		{
			MelonLogger.Msg("Killed Everyone");
			PrisonEscape.MurderGive("Revolver");
		}

		// Token: 0x060004E9 RID: 1257 RVA: 0x00003E56 File Offset: 0x00002056
		public void ForcePickup()
		{
			PrisonEscape.PickupSteal();
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x0001C224 File Offset: 0x0001A424
		public static void MurderCheat(string udonEvent)
		{
			foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
			{
				bool flag = gameObject.name.Contains("Game Logic");
				if (flag)
				{
					gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, udonEvent);
				}
			}
		}

		// Token: 0x060004EB RID: 1259 RVA: 0x0001C294 File Offset: 0x0001A494
		public static void MurderGive(string ObjectName)
		{
			foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
			{
				bool flag = gameObject.name.Contains(ObjectName) && !gameObject.name.Contains("Give") && !gameObject.name.Contains("Teleport");
				if (flag)
				{
					Networking.SetOwner(VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0, gameObject);
					gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
				}
			}
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x0001C368 File Offset: 0x0001A568
		public static void PickupSteal()
		{
			VRC_Pickup[] array = Resources.FindObjectsOfTypeAll<VRC_Pickup>().ToArray<VRC_Pickup>();
			for (int i = 0; i < array.Length; i++)
			{
				bool flag = array[i].gameObject;
				if (flag)
				{
					array[i].DisallowTheft = false;
				}
			}
			VRC_Pickup[] array2 = Resources.FindObjectsOfTypeAll<VRC_Pickup>().ToArray<VRC_Pickup>();
			for (int j = 0; j < array2.Length; j++)
			{
				bool flag2 = array2[j].gameObject;
				if (flag2)
				{
					array2[j].DisallowTheft = false;
				}
			}
			VRCPickup[] array3 = Resources.FindObjectsOfTypeAll<VRCPickup>().ToArray<VRCPickup>();
			for (int k = 0; k < array3.Length; k++)
			{
				bool flag3 = array3[k].gameObject;
				if (flag3)
				{
					array3[k].DisallowTheft = false;
				}
			}
		}

		// Token: 0x04000350 RID: 848
		private readonly Object[] SyncKill = new Object[] { "SyncKill" };
	}
}
