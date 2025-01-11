using System;
using MelonLoader;
using UnityEngine;
using VRC.Core;

namespace KernelClient
{
	// Token: 0x0200005E RID: 94
	internal class UIBG : KernelModule
	{
		// Token: 0x0600025C RID: 604 RVA: 0x0000E994 File Offset: 0x0000CB94
		public override void OnUiManagerInit()
		{
			try
			{
				GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel").SetActive(false);
			}
			catch (Exception ex)
			{
				string text = "Failed to load ";
				Exception ex2 = ex;
				MelonLogger.Msg(text + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x0600025D RID: 605 RVA: 0x0000E994 File Offset: 0x0000CB94
		public override void OnRenderObject()
		{
			try
			{
				GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel").SetActive(false);
			}
			catch (Exception ex)
			{
				string text = "Failed to load ";
				Exception ex2 = ex;
				MelonLogger.Msg(text + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x0600025E RID: 606 RVA: 0x0000E994 File Offset: 0x0000CB94
		public override void OnGUI()
		{
			try
			{
				GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel").SetActive(false);
			}
			catch (Exception ex)
			{
				string text = "Failed to load ";
				Exception ex2 = ex;
				MelonLogger.Msg(text + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x0600025F RID: 607 RVA: 0x0000E994 File Offset: 0x0000CB94
		public override void OnEnterWorld(ApiWorld world, ApiWorldInstance instance)
		{
			try
			{
				GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel").SetActive(false);
			}
			catch (Exception ex)
			{
				string text = "Failed to load ";
				Exception ex2 = ex;
				MelonLogger.Msg(text + ((ex2 != null) ? ex2.ToString() : null));
			}
		}
	}
}
