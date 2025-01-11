using System;
using KernelClient.Wrapper;
using ReMod.Core.UI.QuickMenu;
using UnityEngine;

namespace KernelClient
{
	// Token: 0x0200004E RID: 78
	internal class infinijump : KernelModule
	{
		// Token: 0x060001D2 RID: 466 RVA: 0x0000C228 File Offset: 0x0000A428
		public override void OnUiManagerInit()
		{
			ReMenuCategory category = MenuSetup._uiManager.QMMenu.GetCategoryPage(PageNames.Movement).GetCategory(CatagoryNames.Movement);
			category.AddToggle("Infinite Jump", "Jump as much as you want", delegate(bool s)
			{
				this.ToggleInfinijump(s);
			});
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x00002AC6 File Offset: 0x00000CC6
		private void ToggleInfinijump(bool enable)
		{
			this.infinitjump = enable;
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x0000C274 File Offset: 0x0000A474
		public override void OnUpdate()
		{
			bool flag = this.infinitjump && Input.GetKeyDown(32);
			if (flag)
			{
				VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponent<Rigidbody>().AddForce(Vector3.up * 1000f);
			}
		}

		// Token: 0x04000139 RID: 313
		public bool infinitjump = false;
	}
}
