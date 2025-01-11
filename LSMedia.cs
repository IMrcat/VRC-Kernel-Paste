using System;
using KernelClient.Wrapper;
using MelonLoader;
using ReMod.Core.UI.QuickMenu;
using UnityEngine;
using VRC.Udon;

// Token: 0x0200001B RID: 27
internal class LSMedia : KernelModule
{
	// Token: 0x060000A9 RID: 169 RVA: 0x00007D88 File Offset: 0x00005F88
	public override void OnUiManagerInit()
	{
		ReMenuCategory category = MenuSetup._uiManager.QMMenu.GetCategoryPage(PageNames.WorldCheats).GetCategory(CatagoryNames.MovieWorlds);
		ReMenuPage reMenuPage = category.AddMenuPage("LS Media", "", null, "#ffffff");
		reMenuPage.AddButton("Toggle Video Lock", "", delegate
		{
			this.StartGame();
		}, null, "#ffffff");
	}

	// Token: 0x060000AA RID: 170 RVA: 0x000023FF File Offset: 0x000005FF
	public void StartGame()
	{
		MelonLogger.Msg("Toggled Lock State");
		LSMedia.LSMediaVideoPlayer("SetLocked");
	}

	// Token: 0x060000AB RID: 171 RVA: 0x00007DF0 File Offset: 0x00005FF0
	public static void LSMediaVideoPlayer(string udonEvent)
	{
		foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
		{
			bool flag = gameObject.name.Contains("USharpVideo");
			if (flag)
			{
				gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, udonEvent);
			}
		}
	}
}
