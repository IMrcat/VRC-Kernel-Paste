using System;
using KernelClient.Wrapper;
using ReMod.Core.UI.QuickMenu;

namespace KernelClient.Auth
{
	// Token: 0x02000078 RID: 120
	public class KernelAuth : KernelModule
	{
		// Token: 0x06000328 RID: 808 RVA: 0x000113D8 File Offset: 0x0000F5D8
		public override void OnApplicationStart()
		{
			ReMenuCategory category = MenuSetup._uiManager.QMMenu.GetCategoryPage(PageNames.Utility).GetCategory(CatagoryNames.Movement);
			category.AddToggle("Ghost Mode", "SHHHH you're a ghost (T)", delegate(bool s)
			{
			});
		}

		// Token: 0x06000329 RID: 809 RVA: 0x0000246F File Offset: 0x0000066F
		public override void OnUiManagerInit()
		{
		}

		// Token: 0x04000207 RID: 519
		public bool authenticated = false;
	}
}
