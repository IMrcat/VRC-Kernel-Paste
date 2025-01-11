using System;
using KernelClient.Wrapper;
using MelonLoader;
using ReMod.Core.UI.QuickMenu;
using UnityEngine;
using VRC.Udon;

// Token: 0x02000019 RID: 25
internal class JustH : KernelModule
{
	// Token: 0x0600009F RID: 159 RVA: 0x00007A98 File Offset: 0x00005C98
	public override void OnUiManagerInit()
	{
		ReMenuCategory category = MenuSetup._uiManager.QMMenu.GetCategoryPage(PageNames.WorldCheats).GetCategory(CatagoryNames.NormalWorlds);
		ReMenuPage reMenuPage = category.AddMenuPage("Just H", "", null, "#ffffff");
		reMenuPage.AddButton("Remove Blocks", "", delegate
		{
			JustH.JustHToggle("BlockArea", false);
		}, null, "#ffffff");
		reMenuPage.AddButton("Tele to floor 2", "", delegate
		{
			JustH.JustHTeleportLocal(new Vector3(7.2999f, 53.495f, 11.3452f));
			JustH.JustHToggle("Elevator Variant", true);
			JustH.JustHToggle("Main2", true);
			JustH.JustHToggle("Main", true);
			JustH.JustHToggle("BlockArea", false);
		}, null, "#ffffff");
		reMenuPage.AddButton("Tele to floor 3", "", delegate
		{
			JustH.JustHTeleportLocal(new Vector3(4.0838f, 111.9247f, 11.4541f));
			JustH.JustHToggle("Elevator Variant", true);
			JustH.JustHToggle("Main3", true);
			JustH.JustHToggle("Main", true);
			JustH.JustHToggle("BlockArea", false);
		}, null, "#ffffff");
	}

	// Token: 0x060000A0 RID: 160 RVA: 0x00007B80 File Offset: 0x00005D80
	public static void JustHTeleportLocal(Vector3 teleportLocation)
	{
		bool flag = RoomManager.field_Internal_Static_ApiWorld_0.id == "wrld_e5c30b56-efa8-42d5-a8d4-a2cca2bf3403";
		if (flag)
		{
			VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position = teleportLocation;
			MelonLogger.Msg("Teleported to Room ");
		}
		else
		{
			MelonLogger.Msg("You are not in the Just H Party");
		}
	}

	// Token: 0x060000A1 RID: 161 RVA: 0x00007BD4 File Offset: 0x00005DD4
	public static void JustHTeleport(string floorCube)
	{
		bool flag = RoomManager.field_Internal_Static_ApiWorld_0.id == "wrld_e5c30b56-efa8-42d5-a8d4-a2cca2bf3403";
		if (flag)
		{
			MelonLogger.Msg("Teleported everyone to Room ");
			GameObject gameObject = GameObject.Find(floorCube);
			gameObject.GetComponent<UdonBehaviour>().SendCustomEvent("GrantControl");
		}
		else
		{
			MelonLogger.Msg("You are not in the Just H Party");
		}
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x00007C30 File Offset: 0x00005E30
	public static void JustHToggle(string gameObjectName, bool state)
	{
		bool flag = RoomManager.field_Internal_Static_ApiWorld_0.id == "wrld_e5c30b56-efa8-42d5-a8d4-a2cca2bf3403";
		if (flag)
		{
			foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
			{
				bool flag2 = gameObject.name == gameObjectName;
				if (flag2)
				{
					gameObject.SetActive(state);
					MelonLogger.Msg("turning " + gameObjectName + " state:" + state.ToString());
				}
			}
		}
		else
		{
			MelonLogger.Msg("You are not in the Just H Party");
		}
	}
}
