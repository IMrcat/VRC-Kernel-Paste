using System;
using KernelClient;
using KernelClient.Wrapper;
using MelonLoader;
using ReMod.Core.Managers;
using ReMod.Core.UI.QuickMenu;
using UnityEngine;

// Token: 0x02000035 RID: 53
public static class MenuSetup
{
	// Token: 0x06000131 RID: 305 RVA: 0x00009448 File Offset: 0x00007648
	public static void OnUiManagerInit()
	{
		MelonLogger.Msg("Initializing UI...", new object[] { "Menu" });
		Patches.InitializeNetworkManager();
		Patches.RegisterPatches();
		MenuSetup.buttonSprite = EmbeddedResourceLoader.LoadEmbeddedSprite("KernelClient.assets.Kernel-icon-2025.png");
		if (MenuSetup.buttonSprite == null)
		{
			MelonLogger.Error("Failed to load button sprite.");
			return;
		}
		MenuSetup._uiManager = new UiManager("Kernel Client Menu | Pasted<3", MenuSetup.buttonSprite, true, true, false, "#ffffff");
		if (MenuSetup._uiManager == null || MenuSetup._uiManager.QMMenu == null)
		{
			MelonLogger.Error("UiManager or QMMenu is null.");
			return;
		}
		MenuSetup._uiManager.QMMenu.AddCategoryPage("Kernel Client Config.", "Licence & config", EmbeddedResourceLoader.LoadEmbeddedSprite("KernelClient.assets.config.png"), "#ffffff");
		ReCategoryPage reCategoryPage = MenuSetup._uiManager.QMMenu.AddCategoryPage(PageNames.WorldCheats, "All the world cheats!", EmbeddedResourceLoader.LoadEmbeddedSprite("KernelClient.assets.worldcheat.png"), "#ffffff");
		ReCategoryPage reCategoryPage2 = MenuSetup._uiManager.QMMenu.AddCategoryPage(PageNames.Exploits, "Exploits to exploit!", EmbeddedResourceLoader.LoadEmbeddedSprite("KernelClient.assets.EI.png"), "#ffffff");
		ReCategoryPage reCategoryPage3 = MenuSetup._uiManager.QMMenu.AddCategoryPage(PageNames.Utility, "Utilities and other things", EmbeddedResourceLoader.LoadEmbeddedSprite("KernelClient.assets.utility.png"), "#ffffff");
		ReCategoryPage reCategoryPage4 = MenuSetup._uiManager.QMMenu.AddCategoryPage(PageNames.Security, "Security features", EmbeddedResourceLoader.LoadEmbeddedSprite("KernelClient.assets.security.png"), "#ffffff");
		ReCategoryPage reCategoryPage5 = MenuSetup._uiManager.QMMenu.AddCategoryPage(PageNames.Stupid, "Experimental features.", EmbeddedResourceLoader.LoadEmbeddedSprite("KernelClient.assets.Test.png"), "#ffffff");
		ReCategoryPage reCategoryPage6 = MenuSetup._uiManager.QMMenu.AddCategoryPage(PageNames.Movement, "Movement Manipulation", EmbeddedResourceLoader.LoadEmbeddedSprite("KernelClient.assets.Movement.png"), "#ffffff");
		if (reCategoryPage == null || reCategoryPage2 == null || reCategoryPage3 == null || reCategoryPage4 == null || reCategoryPage5 == null || reCategoryPage6 == null)
		{
			MelonLogger.Error("One or more category pages failed to initialize.");
			return;
		}
		reCategoryPage3.AddCategory(CatagoryNames.Movement);
		reCategoryPage3.AddCategory(CatagoryNames.ESP);
		reCategoryPage2.AddCategory(CatagoryNames.EONEcollection);
		reCategoryPage2.AddCategory(CatagoryNames.OtherExploits);
		reCategoryPage2.AddCategory(CatagoryNames.Spoofers);
		reCategoryPage4.AddCategory(CatagoryNames.Alerts);
		reCategoryPage.AddCategory(CatagoryNames.GameWorlds);
		reCategoryPage.AddCategory(CatagoryNames.MovieWorlds);
		reCategoryPage.AddCategory(CatagoryNames.NormalWorlds);
		reCategoryPage3.AddCategory(CatagoryNames.Avatar);
		reCategoryPage3.AddCategory(CatagoryNames.Other);
		reCategoryPage5.AddCategory(CatagoryNames.Stupid);
		reCategoryPage6.AddCategory(CatagoryNames.Movement);
		reCategoryPage6.AddCategory(CatagoryNames.Camera);
		reCategoryPage4.AddCategory(CatagoryNames.Security);
		foreach (KernelModule kernelModule in Loader.Modules)
		{
			try
			{
				MelonLogger.Msg("Initializing module: " + kernelModule.GetType().Name);
				kernelModule.OnUiManagerInit();
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("{0} had an error during initialization:\n{1}", kernelModule.GetType().Name, ex));
			}
		}
		Patches.ModulesLoaded = true;
	}

	// Token: 0x040000D2 RID: 210
	public static UiManager _uiManager;

	// Token: 0x040000D3 RID: 211
	public static Sprite buttonSprite;
}
