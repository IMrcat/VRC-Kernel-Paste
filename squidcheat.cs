using System;
using KernelClient.Wrapper;
using ReMod.Core.UI.QuickMenu;
using UnityEngine;

namespace KernelClient
{
	// Token: 0x0200005C RID: 92
	public class squidcheat : KernelModule
	{
		// Token: 0x06000255 RID: 597 RVA: 0x0000E850 File Offset: 0x0000CA50
		public override void OnUiManagerInit()
		{
			ReMenuCategory category = MenuSetup._uiManager.QMMenu.GetCategoryPage(PageNames.WorldCheats).GetCategory(CatagoryNames.GameWorlds);
			ReMenuPage reMenuPage = category.AddMenuPage("Squid Game RL/GL", "", null, "#ffffff");
			reMenuPage.AddButton("Remove Barriers", "Disable barriers", delegate
			{
				squidcheat.SG("Start Barrier", false);
				squidcheat.SG("End Barrier", false);
			}, null, "#ffffff");
			reMenuPage.AddButton("Remove Triggers", "Disable Triggers", delegate
			{
				squidcheat.SG("End Trigger", false);
				squidcheat.SG("End Trigger", false);
			}, null, "#ffffff");
		}

		// Token: 0x06000256 RID: 598 RVA: 0x0000E900 File Offset: 0x0000CB00
		public static void SG(string gameObjectName, bool state)
		{
			bool flag = RoomManager.field_Internal_Static_ApiWorld_0.id == "wrld_c8c1c373-dd05-4095-a5f9-57cc1e2dcc33";
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
			}
			else
			{
				ToastNotif.Toast("Not in Squid Game RL/GL", null, null, 5f);
			}
		}
	}
}
