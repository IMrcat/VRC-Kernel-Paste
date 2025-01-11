using System;
using KernelClient.Wrapper;
using MelonLoader;
using ReMod.Core.UI.QuickMenu;
using UnityEngine;

// Token: 0x0200001C RID: 28
internal class MidnightRooftop : KernelModule
{
	// Token: 0x060000AE RID: 174 RVA: 0x00007E60 File Offset: 0x00006060
	public override void OnUiManagerInit()
	{
		ReMenuCategory category = MenuSetup._uiManager.QMMenu.GetCategoryPage(PageNames.WorldCheats).GetCategory(CatagoryNames.MovieWorlds);
		ReMenuPage reMenuPage = category.AddMenuPage("Midnight Rooftop", "", null, "#ffffff");
		reMenuPage.AddButton("Underwater filter Remove", "", delegate
		{
			MidnightRooftop.Midnight("Underwater PP", false);
		}, null, "#ffffff");
	}

	// Token: 0x060000AF RID: 175 RVA: 0x00007EDC File Offset: 0x000060DC
	public static void Midnight(string gameObjectName, bool state)
	{
		bool flag = RoomManager.field_Internal_Static_ApiWorld_0.id == "wrld_d29bb0d0-d268-42dc-8365-926f9d485505";
		if (flag)
		{
			foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
			{
				bool flag2 = gameObject.name.Contains(gameObjectName);
				if (flag2)
				{
					gameObject.SetActive(state);
				}
			}
			MelonLogger.Msg("Turning off " + gameObjectName);
		}
		else
		{
			MelonLogger.Msg("You are not in the midnight rooftop");
		}
	}
}
