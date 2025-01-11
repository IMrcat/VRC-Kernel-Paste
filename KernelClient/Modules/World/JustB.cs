using System;
using KernelClient.Wrapper;
using MelonLoader;
using ReMod.Core.UI.QuickMenu;
using UnityEngine;

namespace KernelClient.Modules.World
{
	// Token: 0x020000B3 RID: 179
	internal class JustB : KernelModule
	{
		// Token: 0x060004D1 RID: 1233 RVA: 0x0001BD20 File Offset: 0x00019F20
		public override void OnUiManagerInit()
		{
			ReMenuPage reMenuPage = MenuSetup._uiManager.QMMenu.GetCategoryPage(PageNames.WorldCheats).GetCategory(CatagoryNames.NormalWorlds).AddMenuPage("Just B (Original)", "", null, "#ffffff");
			reMenuPage.AddButton("Room 1", "", delegate
			{
				JustB.JustBCheat("Bedroom 1", new Vector3(-223.7f, -11.3f, 151.1f));
			}, null, "#ffffff");
			reMenuPage.AddButton("Room 2", "", delegate
			{
				JustB.JustBCheat("Bedroom 2", new Vector3(-211.2f, 55.7f, -91.3f));
			}, null, "#ffffff");
			reMenuPage.AddButton("Room 3", "", delegate
			{
				JustB.JustBCheat("Bedroom 3", new Vector3(-124.6f, -11.3f, 150.3f));
			}, null, "#ffffff");
			reMenuPage.AddButton("Room 4", "", delegate
			{
				JustB.JustBCheat("Bedroom 4", new Vector3(-111.3f, 55.7f, -90.5f));
			}, null, "#ffffff");
			reMenuPage.AddButton("Room 5", "", delegate
			{
				JustB.JustBCheat("Bedroom 5", new Vector3(-24.7f, -11.3f, 150.6f));
			}, null, "#ffffff");
			reMenuPage.AddButton("Room 6", "", delegate
			{
				JustB.JustBCheat("Bedroom 6", new Vector3(-11.3f, 55.7f, -90.5f));
			}, null, "#ffffff");
			reMenuPage.AddButton("Room VIP", "", delegate
			{
				JustB.JustBCheat("Bedroom VIP", new Vector3(57.9796f, 62.8633f, 0.001f));
			}, null, "#ffffff");
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x0001BEDC File Offset: 0x0001A0DC
		public static void JustBCheat(string gameObjectName, Vector3 teleportLocation)
		{
			bool flag = RoomManager.field_Internal_Static_ApiWorld_0.id == "wrld_1b3b3259-0a1f-4311-984e-826abab6f481";
			if (flag)
			{
				foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
				{
					bool flag2 = gameObject.name.Contains(gameObjectName);
					if (flag2)
					{
						gameObject.SetActive(true);
					}
				}
				VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position = teleportLocation;
				MelonLogger.Msg("Teleported to Room " + gameObjectName);
			}
			else
			{
				MelonLogger.Msg("You are not in the Just B club");
			}
		}
	}
}
