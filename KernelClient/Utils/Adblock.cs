using System;
using UnityEngine;
using VRC;
using VRC.Core;

namespace KernelClient.Utils
{
	// Token: 0x02000062 RID: 98
	internal class Adblock : KernelModule
	{
		// Token: 0x06000270 RID: 624 RVA: 0x0000EE18 File Offset: 0x0000D018
		public override void OnEnterWorld(ApiWorld world, ApiWorldInstance instance)
		{
			try
			{
				GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Carousel_Banners").active = false;
			}
			catch
			{
			}
		}

		// Token: 0x06000271 RID: 625 RVA: 0x0000EE18 File Offset: 0x0000D018
		public override void OnUiManagerInit()
		{
			try
			{
				GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Carousel_Banners").active = false;
			}
			catch
			{
			}
		}

		// Token: 0x06000272 RID: 626 RVA: 0x0000EE18 File Offset: 0x0000D018
		public override void OnGUI()
		{
			try
			{
				GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Carousel_Banners").active = false;
			}
			catch
			{
			}
		}

		// Token: 0x06000273 RID: 627 RVA: 0x0000EE18 File Offset: 0x0000D018
		public override void OnPlayerJoined(Player player)
		{
			try
			{
				GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Carousel_Banners").active = false;
			}
			catch
			{
			}
		}
	}
}
