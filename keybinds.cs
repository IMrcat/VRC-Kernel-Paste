using System;
using UnityEngine;

namespace KernelClient
{
	// Token: 0x02000057 RID: 87
	internal class keybinds : KernelModule
	{
		// Token: 0x06000227 RID: 551 RVA: 0x00002C8E File Offset: 0x00000E8E
		public override void OnUpdate()
		{
			this.quickrespawn();
		}

		// Token: 0x06000228 RID: 552 RVA: 0x0000DE08 File Offset: 0x0000C008
		private void quickrespawn()
		{
			bool flag = Input.GetKey(306) && Input.GetKey(304) && Input.GetKeyDown(114);
			if (flag)
			{
				GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickActions/Button_Respawn").GetComponent<VRCButtonHandle>().onClick.Invoke();
				ToastNotif.Toast("Quick Respawn", "Respawned", null, 5f);
			}
		}
	}
}
