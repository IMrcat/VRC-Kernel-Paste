using System;
using KernelClient.Wrapper;
using ReMod.Core.UI.QuickMenu;
using UnityEngine;

namespace DefaultNamespace
{
	// Token: 0x02000046 RID: 70
	public class clicktp : KernelModule
	{
		// Token: 0x060001A8 RID: 424 RVA: 0x0000B764 File Offset: 0x00009964
		public override void OnUiManagerInit()
		{
			ReMenuCategory category = MenuSetup._uiManager.QMMenu.GetCategoryPage(PageNames.Movement).GetCategory(CatagoryNames.Movement);
			category.AddToggle("Click TP", "Teleport to locations with Mouse 5", delegate(bool isEnabled)
			{
				this.clicktpEnabled = isEnabled;
			});
			category.AddToggle("Laptop Mode", "Enable Ctrl+Space for teleport", delegate(bool isEnabled)
			{
				this.laptopMode = isEnabled;
			});
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x0000B7CC File Offset: 0x000099CC
		public override void OnUpdate()
		{
			bool flag = !this.clicktpEnabled;
			if (!flag)
			{
				bool flag2 = this.laptopMode;
				if (flag2)
				{
					bool flag3 = Input.GetKey(306) && Input.GetKeyDown(32);
					if (flag3)
					{
						this.DoTeleport();
					}
				}
				else
				{
					bool mouseButtonDown = Input.GetMouseButtonDown(4);
					if (mouseButtonDown)
					{
						this.DoTeleport();
					}
				}
			}
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000B830 File Offset: 0x00009A30
		private void DoTeleport()
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit raycastHit;
			bool flag = Physics.Raycast(ray, ref raycastHit);
			if (flag)
			{
				VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position = raycastHit.point;
			}
		}

		// Token: 0x04000119 RID: 281
		private bool clicktpEnabled = false;

		// Token: 0x0400011A RID: 282
		private bool laptopMode = false;
	}
}
