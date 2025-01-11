using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using MelonLoader;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements;

namespace KernelClient.Modules
{
	// Token: 0x0200008D RID: 141
	public class UIManagerr : KernelModule
	{
		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060003C3 RID: 963 RVA: 0x00014B20 File Offset: 0x00012D20
		public static UIManagerr Instance
		{
			get
			{
				object @lock = UIManagerr._lock;
				UIManagerr instance;
				lock (@lock)
				{
					bool flag2 = UIManagerr._instance == null;
					if (flag2)
					{
						UIManagerr._instance = new UIManagerr();
					}
					instance = UIManagerr._instance;
				}
				return instance;
			}
		}

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x060003C4 RID: 964 RVA: 0x00014B7C File Offset: 0x00012D7C
		// (remove) Token: 0x060003C5 RID: 965 RVA: 0x00014BB4 File Offset: 0x00012DB4
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnUIInitialized;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x060003C6 RID: 966 RVA: 0x00014BEC File Offset: 0x00012DEC
		// (remove) Token: 0x060003C7 RID: 967 RVA: 0x00014C24 File Offset: 0x00012E24
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<string> OnLogMessageAdded;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x060003C8 RID: 968 RVA: 0x00014C5C File Offset: 0x00012E5C
		// (remove) Token: 0x060003C9 RID: 969 RVA: 0x00014C94 File Offset: 0x00012E94
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnUICleanup;

		// Token: 0x060003CA RID: 970 RVA: 0x00014CCC File Offset: 0x00012ECC
		public override void OnUiManagerInit()
		{
			bool flag = this._isInitializing || this._initialized;
			if (flag)
			{
				MelonLogger.Warning("UI Manager initialization already in progress or completed.");
			}
			else
			{
				this._isInitializing = true;
				MelonCoroutines.Start(this.InitializeUIWithRetry());
			}
		}

		// Token: 0x060003CB RID: 971 RVA: 0x0000345A File Offset: 0x0000165A
		private IEnumerator InitializeUIWithRetry()
		{
			while (this._currentRetryCount < 3 && !this._initialized)
			{
				this._currentRetryCount++;
				MelonLogger.Msg(string.Format("UI initialization attempt {0}...", this._currentRetryCount));
				yield return this.InitializeUI();
				bool initialized = this._initialized;
				if (initialized)
				{
					MelonLogger.Msg("UI Manager initialized successfully.");
					Action onUIInitialized = this.OnUIInitialized;
					if (onUIInitialized != null)
					{
						onUIInitialized();
					}
					break;
				}
				MelonLogger.Error(string.Format("UI initialization attempt {0} failed.", this._currentRetryCount));
				yield return new WaitForSeconds(0.5f);
			}
			bool flag = !this._initialized;
			if (flag)
			{
				MelonLogger.Error("UI Manager failed to initialize after maximum retries.");
			}
			this._isInitializing = false;
			yield break;
		}

		// Token: 0x060003CC RID: 972 RVA: 0x00003469 File Offset: 0x00001669
		private IEnumerator InitializeUI()
		{
			MelonLogger.Msg("Starting UI initialization...");
			bool quickMenuFound = this.FindQuickMenu();
			bool flag = !quickMenuFound;
			if (flag)
			{
				yield break;
			}
			bool layoutGroupFound = this.FindLayoutGroup();
			bool flag2 = !layoutGroupFound;
			if (flag2)
			{
				yield break;
			}
			bool coreComponentsInitialized = this.InitializeCoreComponents();
			bool flag3 = !coreComponentsInitialized;
			if (flag3)
			{
				yield break;
			}
			this._initialized = true;
			yield return null;
			yield break;
		}

		// Token: 0x060003CD RID: 973 RVA: 0x00014D10 File Offset: 0x00012F10
		private bool FindQuickMenu()
		{
			float num = 0f;
			MelonLogger.Msg("Attempting to find QuickMenu...");
			while (this._quickMenu == null && num < 10f)
			{
				this._quickMenu = Object.FindObjectOfType<QuickMenu>();
				bool flag = this._quickMenu != null;
				if (flag)
				{
					MelonLogger.Msg("QuickMenu found successfully.");
					return true;
				}
				num += Time.deltaTime;
				MelonCoroutines.Start(this.YieldWait(0.1f));
			}
			bool flag2 = this._quickMenu == null;
			if (flag2)
			{
				MelonLogger.Error("Failed to find QuickMenu within timeout period.");
				return false;
			}
			return true;
		}

		// Token: 0x060003CE RID: 974 RVA: 0x00014DBC File Offset: 0x00012FBC
		private bool FindLayoutGroup()
		{
			bool flag = this._quickMenu == null;
			bool flag2;
			if (flag)
			{
				MelonLogger.Error("QuickMenu is null. Cannot find LayoutGroup.");
				flag2 = false;
			}
			else
			{
				MelonLogger.Msg("Attempting to find VerticalLayoutGroup...");
				VerticalLayoutGroup componentInChildren = this._quickMenu.GetComponentInChildren<VerticalLayoutGroup>();
				this._layoutGroup = ((componentInChildren != null) ? componentInChildren.transform : null);
				bool flag3 = this._layoutGroup == null;
				if (flag3)
				{
					MelonLogger.Error("Failed to find VerticalLayoutGroup in QuickMenu using flexible search.");
					flag2 = false;
				}
				else
				{
					this.ConfigureLayoutGroup();
					MelonLogger.Msg("VerticalLayoutGroup found and configured successfully.");
					flag2 = true;
				}
			}
			return flag2;
		}

		// Token: 0x060003CF RID: 975 RVA: 0x00014E4C File Offset: 0x0001304C
		private void ConfigureLayoutGroup()
		{
			VerticalLayoutGroup component = this._layoutGroup.GetComponent<VerticalLayoutGroup>();
			bool flag = component != null;
			if (flag)
			{
				component.spacing = 10f;
				component.padding = new RectOffset(10, 10, 10, 10);
				component.childControlHeight = true;
				component.childForceExpandHeight = false;
				MelonLogger.Msg("VerticalLayoutGroup configured.");
			}
			else
			{
				MelonLogger.Warning("VerticalLayoutGroup component not found on the layout group transform.");
			}
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x00014EC0 File Offset: 0x000130C0
		private bool InitializeCoreComponents()
		{
			MelonLogger.Msg("Initializing core UI components...");
			bool flag = this.CreateMainWindow();
			bool flag2 = !flag;
			bool flag3;
			if (flag2)
			{
				flag3 = false;
			}
			else
			{
				bool flag4 = this.CreateLogWindow();
				bool flag5 = !flag4;
				if (flag5)
				{
					flag3 = false;
				}
				else
				{
					bool flag6 = this.CreateMenuButtons();
					bool flag7 = !flag6;
					if (flag7)
					{
						flag3 = false;
					}
					else
					{
						bool flag8 = this.CreateAdditionalElements();
						bool flag9 = !flag8;
						if (flag9)
						{
							flag3 = false;
						}
						else
						{
							MelonLogger.Msg("All core UI components initialized successfully.");
							flag3 = true;
						}
					}
				}
			}
			return flag3;
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x00014F44 File Offset: 0x00013144
		private bool CreateMainWindow()
		{
			bool flag;
			try
			{
				MelonLogger.Msg("Creating Main Window...");
				GameObject gameObject = new GameObject("KernelMainWindow");
				gameObject.transform.SetParent(this._layoutGroup, false);
				RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
				rectTransform.anchorMin = Vector2.zero;
				rectTransform.anchorMax = Vector2.one;
				rectTransform.sizeDelta = UIManagerr.DEFAULT_WINDOW_SIZE;
				Image image = gameObject.AddComponent<Image>();
				image.color = this._primaryColor;
				this._managedObjects.Add(gameObject);
				MelonLogger.Msg("Main Window created successfully.");
				flag = true;
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Exception in CreateMainWindow: " + ex.Message);
				flag = false;
			}
			return flag;
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x00015008 File Offset: 0x00013208
		private bool CreateLogWindow()
		{
			bool flag3;
			try
			{
				MelonLogger.Msg("Creating Log Window...");
				this._logWindow = new GameObject("KernelLogWindow");
				this._logWindow.transform.SetParent(this._layoutGroup, false);
				RectTransform rectTransform = this._logWindow.AddComponent<RectTransform>();
				rectTransform.anchorMin = new Vector2(0f, 0f);
				rectTransform.anchorMax = new Vector2(1f, 0.4f);
				rectTransform.sizeDelta = Vector2.zero;
				bool flag = this.CreateLogPanel(this._logWindow);
				bool flag2 = !flag;
				if (flag2)
				{
					MelonLogger.Error("Failed to create Log Panel.");
					flag3 = false;
				}
				else
				{
					this._managedObjects.Add(this._logWindow);
					MelonLogger.Msg("Log Window created successfully.");
					flag3 = true;
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Exception in CreateLogWindow: " + ex.Message);
				flag3 = false;
			}
			return flag3;
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x00015104 File Offset: 0x00013304
		private bool CreateLogPanel(GameObject parent)
		{
			bool flag3;
			try
			{
				MelonLogger.Msg("Creating Log Panel...");
				GameObject gameObject = new GameObject("LogPanel");
				gameObject.transform.SetParent(parent.transform, false);
				RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
				rectTransform.anchorMin = Vector2.zero;
				rectTransform.anchorMax = Vector2.one;
				rectTransform.sizeDelta = Vector2.zero;
				Image image = gameObject.AddComponent<Image>();
				image.color = this._secondaryColor;
				bool flag = this.CreateScrollView(gameObject);
				bool flag2 = !flag;
				if (flag2)
				{
					MelonLogger.Error("Failed to create ScrollView.");
					flag3 = false;
				}
				else
				{
					MelonLogger.Msg("Log Panel created successfully.");
					flag3 = true;
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Exception in CreateLogPanel: " + ex.Message);
				flag3 = false;
			}
			return flag3;
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x000151E0 File Offset: 0x000133E0
		private bool CreateScrollView(GameObject parent)
		{
			bool flag3;
			try
			{
				MelonLogger.Msg("Creating ScrollView...");
				GameObject gameObject = new GameObject("ScrollView");
				gameObject.transform.SetParent(parent.transform, false);
				RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
				rectTransform.anchorMin = Vector2.zero;
				rectTransform.anchorMax = Vector2.one;
				rectTransform.sizeDelta = Vector2.zero;
				this._scrollRect = gameObject.AddComponent<ScrollRect>();
				this._scrollRect.vertical = true;
				this._scrollRect.horizontal = false;
				this._scrollRect.scrollSensitivity = 15f;
				bool flag = this.CreateViewport(gameObject);
				bool flag2 = !flag;
				if (flag2)
				{
					MelonLogger.Error("Failed to create Viewport.");
					flag3 = false;
				}
				else
				{
					bool flag4 = this.CreateScrollContent(gameObject);
					bool flag5 = !flag4;
					if (flag5)
					{
						MelonLogger.Error("Failed to create Scroll Content.");
						flag3 = false;
					}
					else
					{
						MelonLogger.Msg("ScrollView created successfully.");
						flag3 = true;
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Exception in CreateScrollView: " + ex.Message);
				flag3 = false;
			}
			return flag3;
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x00015304 File Offset: 0x00013504
		private bool CreateViewport(GameObject scrollView)
		{
			bool flag;
			try
			{
				MelonLogger.Msg("Creating Viewport...");
				GameObject gameObject = new GameObject("Viewport");
				gameObject.transform.SetParent(scrollView.transform, false);
				RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
				rectTransform.anchorMin = Vector2.zero;
				rectTransform.anchorMax = Vector2.one;
				rectTransform.sizeDelta = Vector2.zero;
				Mask mask = gameObject.AddComponent<Mask>();
				mask.showMaskGraphic = false;
				Image image = gameObject.AddComponent<Image>();
				image.color = this._secondaryColor;
				this._scrollRect.viewport = rectTransform;
				MelonLogger.Msg("Viewport created successfully.");
				flag = true;
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Exception in CreateViewport: " + ex.Message);
				flag = false;
			}
			return flag;
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x000153D8 File Offset: 0x000135D8
		private bool CreateScrollContent(GameObject scrollView)
		{
			bool flag;
			try
			{
				MelonLogger.Msg("Creating Scroll Content...");
				GameObject gameObject = new GameObject("Content");
				gameObject.transform.SetParent(scrollView.transform.Find("Viewport"), false);
				RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
				rectTransform.anchorMin = new Vector2(0f, 1f);
				rectTransform.anchorMax = new Vector2(1f, 1f);
				rectTransform.pivot = new Vector2(0.5f, 1f);
				rectTransform.sizeDelta = Vector2.zero;
				this._logText = gameObject.AddComponent<TextMeshProUGUI>();
				this._logText.fontSize = 14f;
				this._logText.color = this._textColor;
				this._logText.richText = true;
				this._logText.raycastTarget = false;
				this._scrollRect.content = rectTransform;
				MelonLogger.Msg("Scroll Content created successfully.");
				flag = true;
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Exception in CreateScrollContent: " + ex.Message);
				flag = false;
			}
			return flag;
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x00015504 File Offset: 0x00013704
		private bool CreateMenuButtons()
		{
			bool flag;
			try
			{
				MelonLogger.Msg("Creating Menu Buttons...");
				GameObject gameObject = new GameObject("MenuButtons");
				gameObject.transform.SetParent(this._layoutGroup, false);
				RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
				rectTransform.anchorMin = new Vector2(0f, 0.4f);
				rectTransform.anchorMax = new Vector2(1f, 0.6f);
				rectTransform.sizeDelta = Vector2.zero;
				HorizontalLayoutGroup horizontalLayoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
				horizontalLayoutGroup.spacing = 10f;
				horizontalLayoutGroup.padding = new RectOffset(10, 10, 10, 10);
				horizontalLayoutGroup.childAlignment = 4;
				this._managedObjects.Add(gameObject);
				MelonLogger.Msg("Menu Buttons created successfully.");
				flag = true;
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Exception in CreateMenuButtons: " + ex.Message);
				flag = false;
			}
			return flag;
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x000155F8 File Offset: 0x000137F8
		private bool CreateAdditionalElements()
		{
			MelonLogger.Msg("Creating additional UI elements (if any)...");
			return true;
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x00003478 File Offset: 0x00001678
		private IEnumerator YieldWait(float seconds)
		{
			yield return new WaitForSeconds(seconds);
			yield break;
		}

		// Token: 0x060003DA RID: 986 RVA: 0x00015618 File Offset: 0x00013818
		public void AddLogMessage(string message)
		{
			bool flag = this._logText != null && this._initialized;
			if (flag)
			{
				string text = DateTime.Now.ToString("HH:mm:ss");
				TextMeshProUGUI logText = this._logText;
				logText.text = string.Concat(new string[] { logText.text, "[", text, "] ", message, "\n" });
				Action<string> onLogMessageAdded = this.OnLogMessageAdded;
				if (onLogMessageAdded != null)
				{
					onLogMessageAdded(message);
				}
			}
			else
			{
				MelonLogger.Warning("Attempted to add log message before UI was initialized.");
			}
		}

		// Token: 0x060003DB RID: 987 RVA: 0x000156BC File Offset: 0x000138BC
		public void CleanupUI()
		{
			try
			{
				foreach (GameObject gameObject in this._managedObjects)
				{
					Object.Destroy(gameObject);
				}
				this._managedObjects.Clear();
				this._initialized = false;
				Action onUICleanup = this.OnUICleanup;
				if (onUICleanup != null)
				{
					onUICleanup();
				}
				MelonLogger.Msg("UI cleaned up successfully.");
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Exception during CleanupUI: " + ex.Message);
			}
		}

		// Token: 0x04000266 RID: 614
		private const float BUTTON_HEIGHT = 85f;

		// Token: 0x04000267 RID: 615
		private const float MAX_INIT_WAIT_TIME = 10f;

		// Token: 0x04000268 RID: 616
		private const float RETRY_DELAY = 0.5f;

		// Token: 0x04000269 RID: 617
		private const int MAX_RETRIES = 3;

		// Token: 0x0400026A RID: 618
		private QuickMenu _quickMenu;

		// Token: 0x0400026B RID: 619
		private Transform _layoutGroup;

		// Token: 0x0400026C RID: 620
		private GameObject _logWindow;

		// Token: 0x0400026D RID: 621
		private ScrollRect _scrollRect;

		// Token: 0x0400026E RID: 622
		private TextMeshProUGUI _logText;

		// Token: 0x0400026F RID: 623
		private readonly List<GameObject> _managedObjects = new List<GameObject>();

		// Token: 0x04000270 RID: 624
		private bool _initialized;

		// Token: 0x04000271 RID: 625
		private bool _isInitializing;

		// Token: 0x04000272 RID: 626
		private int _currentRetryCount;

		// Token: 0x04000273 RID: 627
		private static UIManagerr _instance;

		// Token: 0x04000274 RID: 628
		private static readonly object _lock = new object();

		// Token: 0x04000275 RID: 629
		private readonly Color _primaryColor = new Color(0.1f, 0.1f, 0.1f, 0.95f);

		// Token: 0x04000276 RID: 630
		private readonly Color _secondaryColor = new Color(0.15f, 0.15f, 0.15f, 0.9f);

		// Token: 0x04000277 RID: 631
		private readonly Color _accentColor = new Color(0.2f, 0.6f, 1f, 1f);

		// Token: 0x04000278 RID: 632
		private readonly Color _textColor = new Color(0.9f, 0.9f, 0.9f, 1f);

		// Token: 0x04000279 RID: 633
		private static readonly Vector2 DEFAULT_BUTTON_SIZE = new Vector2(200f, 40f);

		// Token: 0x0400027A RID: 634
		private static readonly Vector2 DEFAULT_WINDOW_SIZE = new Vector2(400f, 300f);

		// Token: 0x0400027B RID: 635
		private static readonly Vector2 DEFAULT_PANEL_SIZE = new Vector2(380f, 250f);
	}
}
