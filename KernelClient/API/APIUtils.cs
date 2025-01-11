using System;
using Il2CppSystem;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements;
using VRC.UI.Elements.Controls;

namespace KernelClient.API
{
	// Token: 0x0200007A RID: 122
	internal static class APIUtils
	{
		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600032E RID: 814 RVA: 0x00011438 File Offset: 0x0000F638
		public static UserInterface UserInterface
		{
			get
			{
				bool flag = APIUtils._userInterface == null;
				if (flag)
				{
					APIUtils._userInterface = Object.FindObjectsOfType<UserInterface>()[0];
				}
				return APIUtils._userInterface;
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600032F RID: 815 RVA: 0x00011470 File Offset: 0x0000F670
		public static QuickMenu QuickMenuInstance
		{
			get
			{
				bool flag = APIUtils._quickMenu == null;
				if (flag)
				{
					APIUtils._quickMenu = Object.FindObjectsOfType<QuickMenu>()[0];
				}
				return APIUtils._quickMenu;
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000330 RID: 816 RVA: 0x000114A8 File Offset: 0x0000F6A8
		public static MainMenu SocialMenuInstance
		{
			get
			{
				bool flag = APIUtils._socialMenu == null;
				if (flag)
				{
					APIUtils._socialMenu = Object.FindObjectsOfType<MainMenu>()[0];
				}
				return APIUtils._socialMenu;
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000331 RID: 817 RVA: 0x000114E0 File Offset: 0x0000F6E0
		public static MenuStateController MenuStateControllerInstance
		{
			get
			{
				bool flag = APIUtils._menuStateController == null;
				if (flag)
				{
					APIUtils._menuStateController = APIUtils.QuickMenuInstance.GetComponent<MenuStateController>();
				}
				return APIUtils._menuStateController;
			}
		}

		// Token: 0x06000332 RID: 818 RVA: 0x00011518 File Offset: 0x0000F718
		public static GameObject GetQMMenuTemplate()
		{
			bool flag = APIUtils._qmMenuTemplate == null;
			if (flag)
			{
				APIUtils._qmMenuTemplate = APIUtils.QuickMenuInstance.transform.Find("CanvasGroup/Container/Window/QMParent/Menu_Dashboard").gameObject;
			}
			return APIUtils._qmMenuTemplate;
		}

		// Token: 0x06000333 RID: 819 RVA: 0x00011560 File Offset: 0x0000F760
		public static GameObject GetQMTabButtonTemplate()
		{
			bool flag = APIUtils._qmTabTemplate == null;
			if (flag)
			{
				APIUtils._qmTabTemplate = APIUtils.QuickMenuInstance.transform.Find("CanvasGroup/Container/Window/Page_Buttons_QM/HorizontalLayoutGroup/Page_Settings").gameObject;
			}
			return APIUtils._qmTabTemplate;
		}

		// Token: 0x06000334 RID: 820 RVA: 0x000115A8 File Offset: 0x0000F7A8
		public static GameObject GetQMButtonTemplate()
		{
			bool flag = APIUtils._qmButtonTemplate == null;
			if (flag)
			{
				APIUtils._qmButtonTemplate = APIUtils.QuickMenuInstance.transform.Find("CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickLinks/Button_Worlds").gameObject;
			}
			return APIUtils._qmButtonTemplate;
		}

		// Token: 0x06000335 RID: 821 RVA: 0x000115F0 File Offset: 0x0000F7F0
		public static GameObject GetQMInfoPanelTemplate()
		{
			bool flag = APIUtils._qmInfoPanelTemplate == null;
			if (flag)
			{
				APIUtils._qmInfoPanelTemplate = APIUtils.SocialMenuInstance.transform.Find("Container/MMParent/Menu_WorldDetail/ScrollRect/Viewport/VerticalLayoutGroup/AboutText/Panel_Description_Expandable").gameObject;
			}
			return APIUtils._qmInfoPanelTemplate;
		}

		// Token: 0x06000336 RID: 822 RVA: 0x00011638 File Offset: 0x0000F838
		public static Sprite OnIconSprite()
		{
			bool flag = APIUtils._onSprite == null;
			if (flag)
			{
				APIUtils._onSprite = APIUtils.QuickMenuInstance.transform.Find("CanvasGroup/Container/Window/QMParent/Menu_Notifications/Panel_NoNotifications_Message/Icon").GetComponent<Image>().sprite;
			}
			return APIUtils._onSprite;
		}

		// Token: 0x06000337 RID: 823 RVA: 0x00011684 File Offset: 0x0000F884
		public static Sprite OffIconSprite()
		{
			bool flag = APIUtils._offSprite == null;
			if (flag)
			{
				APIUtils._offSprite = APIUtils.QuickMenuInstance.transform.Find("CanvasGroup/Container/Window/QMParent/Menu_Notifications/Panel_Notification_Tabs/Button_ClearNotifications/Text_FieldContent/Icon").GetComponent<Image>().sprite;
			}
			return APIUtils._offSprite;
		}

		// Token: 0x06000338 RID: 824 RVA: 0x000116D0 File Offset: 0x0000F8D0
		public static int RandomNumbers()
		{
			return APIUtils.rnd.Next(100000, 999999);
		}

		// Token: 0x06000339 RID: 825 RVA: 0x000031AC File Offset: 0x000013AC
		public static void DestroyChildren(Transform transform)
		{
			APIUtils.DestroyChildren(transform, null);
		}

		// Token: 0x0600033A RID: 826 RVA: 0x000116F8 File Offset: 0x0000F8F8
		public static void DestroyChildren(Transform transform, Func<Transform, bool> exclude)
		{
			for (int i = transform.childCount - 1; i >= 0; i--)
			{
				bool flag = exclude == null || exclude(transform.GetChild(i));
				if (flag)
				{
					Object.DestroyImmediate(transform.GetChild(i).gameObject);
				}
			}
		}

		// Token: 0x0400020A RID: 522
		public const string Identifier = "Kernel";

		// Token: 0x0400020B RID: 523
		private static readonly Random rnd = new Random();

		// Token: 0x0400020C RID: 524
		private static QuickMenu _quickMenu;

		// Token: 0x0400020D RID: 525
		private static MainMenu _socialMenu;

		// Token: 0x0400020E RID: 526
		private static UserInterface _userInterface;

		// Token: 0x0400020F RID: 527
		private static MenuStateController _menuStateController;

		// Token: 0x04000210 RID: 528
		private static GameObject _qmMenuTemplate;

		// Token: 0x04000211 RID: 529
		private static GameObject _qmTabTemplate;

		// Token: 0x04000212 RID: 530
		private static GameObject _qmButtonTemplate;

		// Token: 0x04000213 RID: 531
		private static GameObject _qmInfoPanelTemplate;

		// Token: 0x04000214 RID: 532
		private static Sprite _onSprite;

		// Token: 0x04000215 RID: 533
		private static Sprite _offSprite;
	}
}
