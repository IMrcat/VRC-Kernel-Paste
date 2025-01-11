using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MelonLoader;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using VRC.UI.Elements;

namespace KernelClient.Modules
{
	// Token: 0x02000086 RID: 134
	public class KernelLogger : KernelModule
	{
		// Token: 0x06000397 RID: 919 RVA: 0x000131DC File Offset: 0x000113DC
		public KernelLogger()
		{
			this.logEntries = new Queue<KernelLogger.LogEntry>();
			this.categoryToggles = new Dictionary<string, Toggle>();
			this.enabledCategories = new HashSet<string>(this.CategoryColors.Keys);
		}

		// Token: 0x06000398 RID: 920 RVA: 0x000132B4 File Offset: 0x000114B4
		public override void OnUiManagerInit()
		{
			try
			{
				bool flag = this.initialized;
				if (flag)
				{
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("Failed to initialize KernelLogger: {0}", ex));
			}
		}

		// Token: 0x06000399 RID: 921 RVA: 0x00003399 File Offset: 0x00001599
		private IEnumerator InitializeUICoroutine()
		{
			float waitTime = 0f;
			float maxWaitTime = 10f;
			while (this.quickMenu == null && waitTime < maxWaitTime)
			{
				this.quickMenu = Object.FindObjectOfType<QuickMenu>();
				bool flag = this.quickMenu != null;
				if (flag)
				{
					break;
				}
				waitTime += Time.deltaTime;
				yield return null;
			}
			bool flag2 = this.quickMenu == null;
			if (flag2)
			{
				MelonLogger.Error("Could not find QuickMenu after waiting. KernelLogger UI initialization aborted.");
				yield break;
			}
			this.CreateLogWindow();
			this.SetupMelonLogger();
			this.initialized = true;
			yield break;
		}

		// Token: 0x0600039A RID: 922 RVA: 0x000132F8 File Offset: 0x000114F8
		private void CreateLogWindow()
		{
			try
			{
				Transform transform = this.quickMenu.transform.Find("Canvas_QuickMenu(Clone)/CanvasGroup/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Header_KernelClientMenu");
				bool flag = transform == null;
				if (flag)
				{
					MelonLogger.Error("Could not find Header_KernelClientMenu in QuickMenu hierarchy.");
				}
				else
				{
					this.logWindow = new GameObject("KernelLogWindow");
					this.logWindow.transform.SetParent(transform.transform.parent, false);
					RectTransform rectTransform = this.logWindow.AddComponent<RectTransform>();
					rectTransform.anchorMin = new Vector2(0f, 0f);
					rectTransform.anchorMax = new Vector2(1f, 0.5f);
					rectTransform.pivot = new Vector2(0.5f, 1f);
					rectTransform.sizeDelta = Vector2.zero;
					rectTransform.anchoredPosition = new Vector2(0f, 0f);
					VerticalLayoutGroup verticalLayoutGroup = this.logWindow.AddComponent<VerticalLayoutGroup>();
					verticalLayoutGroup.padding = new RectOffset(5, 5, 5, 5);
					verticalLayoutGroup.spacing = 5f;
					verticalLayoutGroup.childControlHeight = false;
					verticalLayoutGroup.childControlWidth = true;
					verticalLayoutGroup.childForceExpandHeight = false;
					verticalLayoutGroup.childForceExpandWidth = true;
					ContentSizeFitter contentSizeFitter = this.logWindow.AddComponent<ContentSizeFitter>();
					contentSizeFitter.verticalFit = 2;
					contentSizeFitter.horizontalFit = 0;
					CanvasGroup canvasGroup = this.logWindow.AddComponent<CanvasGroup>();
					canvasGroup.alpha = 1f;
					canvasGroup.interactable = true;
					canvasGroup.blocksRaycasts = true;
					this.CreateControls();
					this.CreateLogPanel();
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("Exception in CreateLogWindow: {0}", ex));
			}
		}

		// Token: 0x0600039B RID: 923 RVA: 0x000134B0 File Offset: 0x000116B0
		private void CreateControls()
		{
			GameObject gameObject = new GameObject("Controls");
			gameObject.transform.SetParent(this.logWindow.transform, false);
			HorizontalLayoutGroup horizontalLayoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
			horizontalLayoutGroup.spacing = 5f;
			horizontalLayoutGroup.childAlignment = 3;
			horizontalLayoutGroup.childControlHeight = true;
			horizontalLayoutGroup.childControlWidth = false;
			horizontalLayoutGroup.childForceExpandHeight = false;
			horizontalLayoutGroup.childForceExpandWidth = false;
			this.autoScrollToggle = this.CreateToggle(gameObject, "AutoScroll", this.autoScroll, delegate(bool value)
			{
				this.autoScroll = value;
			});
			this.timestampToggle = this.CreateToggle(gameObject, "Timestamps", this.showTimestamps, delegate(bool value)
			{
				this.showTimestamps = value;
				this.UpdateText();
			});
			this.stackTraceToggle = this.CreateToggle(gameObject, "StackTrace", this.showStackTraces, delegate(bool value)
			{
				this.showStackTraces = value;
				this.UpdateText();
			});
			using (Dictionary<string, string>.KeyCollection.Enumerator enumerator = this.CategoryColors.Keys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string category = enumerator.Current;
					Toggle toggle = this.CreateToggle(gameObject, category, true, delegate(bool value)
					{
						if (value)
						{
							this.enabledCategories.Add(category);
						}
						else
						{
							this.enabledCategories.Remove(category);
						}
						this.UpdateText();
					});
					this.categoryToggles[category] = toggle;
				}
			}
			this.CreateButton(gameObject, "Clear", new Action(this.ClearLogs));
		}

		// Token: 0x0600039C RID: 924 RVA: 0x0001362C File Offset: 0x0001182C
		private Toggle CreateToggle(GameObject parent, string label, bool defaultValue, Action<bool> onValueChanged)
		{
			GameObject gameObject = new GameObject("Toggle_" + label);
			gameObject.transform.SetParent(parent.transform, false);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.sizeDelta = new Vector2(120f, 20f);
			Toggle toggle = gameObject.AddComponent<Toggle>();
			toggle.isOn = defaultValue;
			GameObject gameObject2 = new GameObject("Background");
			gameObject2.transform.SetParent(gameObject.transform, false);
			RectTransform rectTransform2 = gameObject2.AddComponent<RectTransform>();
			rectTransform2.anchorMin = new Vector2(0f, 0.5f);
			rectTransform2.anchorMax = new Vector2(0f, 0.5f);
			rectTransform2.sizeDelta = new Vector2(20f, 20f);
			rectTransform2.anchoredPosition = new Vector2(0f, 0f);
			Image image = gameObject2.AddComponent<Image>();
			image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
			toggle.targetGraphic = image;
			GameObject gameObject3 = new GameObject("Checkmark");
			gameObject3.transform.SetParent(gameObject2.transform, false);
			RectTransform rectTransform3 = gameObject3.AddComponent<RectTransform>();
			rectTransform3.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform3.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform3.sizeDelta = new Vector2(12f, 12f);
			rectTransform3.anchoredPosition = Vector2.zero;
			Image image2 = gameObject3.AddComponent<Image>();
			image2.color = Color.white;
			toggle.graphic = image2;
			GameObject gameObject4 = new GameObject("Label");
			gameObject4.transform.SetParent(gameObject.transform, false);
			RectTransform rectTransform4 = gameObject4.AddComponent<RectTransform>();
			rectTransform4.anchorMin = new Vector2(0f, 0f);
			rectTransform4.anchorMax = new Vector2(1f, 1f);
			rectTransform4.sizeDelta = new Vector2(0f, 0f);
			rectTransform4.anchoredPosition = new Vector2(30f, 0f);
			TextMeshProUGUI textMeshProUGUI = gameObject4.AddComponent<TextMeshProUGUI>();
			textMeshProUGUI.text = label;
			textMeshProUGUI.fontSize = 12f;
			textMeshProUGUI.color = Color.white;
			textMeshProUGUI.alignment = 513;
			textMeshProUGUI.verticalAlignment = 512;
			return toggle;
		}

		// Token: 0x0600039D RID: 925 RVA: 0x000138AC File Offset: 0x00011AAC
		private void CreateButton(GameObject parent, string label, Action onClick)
		{
			GameObject gameObject = new GameObject("Button_" + label);
			gameObject.transform.SetParent(parent.transform, false);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.sizeDelta = new Vector2(80f, 25f);
			Button button = gameObject.AddComponent<Button>();
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
			button.targetGraphic = image;
			GameObject gameObject2 = new GameObject("Label");
			gameObject2.transform.SetParent(gameObject.transform, false);
			RectTransform rectTransform2 = gameObject2.AddComponent<RectTransform>();
			rectTransform2.anchorMin = new Vector2(0f, 0f);
			rectTransform2.anchorMax = new Vector2(1f, 1f);
			rectTransform2.sizeDelta = new Vector2(0f, 0f);
			rectTransform2.anchoredPosition = Vector2.zero;
			TextMeshProUGUI textMeshProUGUI = gameObject2.AddComponent<TextMeshProUGUI>();
			textMeshProUGUI.text = label;
			textMeshProUGUI.fontSize = 12f;
			textMeshProUGUI.color = Color.white;
			textMeshProUGUI.alignment = 514;
			textMeshProUGUI.verticalAlignment = 512;
		}

		// Token: 0x0600039E RID: 926 RVA: 0x000139F4 File Offset: 0x00011BF4
		private void CreateLogPanel()
		{
			GameObject gameObject = new GameObject("LogPanel");
			gameObject.transform.SetParent(this.logWindow.transform, false);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.sizeDelta = Vector2.zero;
			rectTransform.anchoredPosition = Vector2.zero;
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0.8f);
			this.scrollRect = gameObject.AddComponent<ScrollRect>();
			this.scrollRect.horizontal = false;
			this.scrollRect.vertical = true;
			this.scrollRect.movementType = 1;
			this.scrollRect.scrollSensitivity = 20f;
			GameObject gameObject2 = new GameObject("Viewport");
			gameObject2.transform.SetParent(gameObject.transform, false);
			RectTransform rectTransform2 = gameObject2.AddComponent<RectTransform>();
			rectTransform2.anchorMin = Vector2.zero;
			rectTransform2.anchorMax = Vector2.one;
			rectTransform2.sizeDelta = Vector2.zero;
			rectTransform2.anchoredPosition = Vector2.zero;
			Image image2 = gameObject2.AddComponent<Image>();
			image2.color = new Color(0f, 0f, 0f, 0.8f);
			image2.sprite = null;
			image2.type = 1;
			this.scrollRect.viewport = rectTransform2;
			GameObject gameObject3 = new GameObject("Content");
			gameObject3.transform.SetParent(gameObject2.transform, false);
			RectTransform rectTransform3 = gameObject3.AddComponent<RectTransform>();
			rectTransform3.anchorMin = new Vector2(0f, 1f);
			rectTransform3.anchorMax = new Vector2(1f, 1f);
			rectTransform3.pivot = new Vector2(0.5f, 1f);
			rectTransform3.anchoredPosition = new Vector2(0f, 0f);
			rectTransform3.sizeDelta = new Vector2(0f, 0f);
			this.scrollRect.content = rectTransform3;
			this.logText = gameObject3.AddComponent<TextMeshProUGUI>();
			this.logText.fontSize = 14f;
			this.logText.color = Color.white;
			this.logText.richText = true;
			this.logText.alignment = 257;
			this.logText.enableWordWrapping = true;
			this.logText.rectTransform.anchorMin = new Vector2(0f, 1f);
			this.logText.rectTransform.anchorMax = new Vector2(1f, 1f);
			this.logText.rectTransform.pivot = new Vector2(0.5f, 1f);
			this.logText.rectTransform.anchoredPosition = new Vector2(0f, 0f);
			this.logText.rectTransform.sizeDelta = new Vector2(0f, 0f);
			this.logText.overflowMode = 0;
			ContentSizeFitter contentSizeFitter = this.logText.gameObject.AddComponent<ContentSizeFitter>();
			contentSizeFitter.verticalFit = 2;
			contentSizeFitter.horizontalFit = 0;
		}

		// Token: 0x0600039F RID: 927 RVA: 0x00013D40 File Offset: 0x00011F40
		private void AddLog(string message, string category = "Default", LogType type = 3)
		{
			bool flag = !this.initialized;
			if (!flag)
			{
				KernelLogger.LogEntry logEntry = new KernelLogger.LogEntry
				{
					Message = message,
					Category = category,
					Color = (this.CategoryColors.ContainsKey(category) ? this.CategoryColors[category] : this.CategoryColors["Default"]),
					Time = DateTime.Now,
					Type = type,
					StackTrace = ((type == null) ? Environment.StackTrace : null)
				};
				this.logEntries.Enqueue(logEntry);
				while (this.logEntries.Count > 1000)
				{
					this.logEntries.Dequeue();
				}
				this.UpdateText();
			}
		}

		// Token: 0x060003A0 RID: 928 RVA: 0x00013E04 File Offset: 0x00012004
		private void UpdateText()
		{
			bool flag = !this.initialized || this.logText == null;
			if (!flag)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (KernelLogger.LogEntry logEntry in this.logEntries.Where((KernelLogger.LogEntry e) => this.enabledCategories.Contains(e.Category)))
				{
					bool flag2 = this.showTimestamps;
					if (flag2)
					{
						stringBuilder.Append(string.Format("<color=#808080>[{0:HH:mm:ss}]</color> ", logEntry.Time));
					}
					stringBuilder.Append(string.Concat(new string[] { "<color=", logEntry.Color, ">[", logEntry.Category, "] ", logEntry.Message, "</color>" }));
					bool flag3 = this.showStackTraces && logEntry.StackTrace != null;
					if (flag3)
					{
						stringBuilder.Append("\n<color=#808080>" + logEntry.StackTrace + "</color>");
					}
					stringBuilder.AppendLine();
				}
				this.logText.text = stringBuilder.ToString();
				bool flag4 = this.autoScroll && this.scrollRect != null;
				if (flag4)
				{
					Canvas.ForceUpdateCanvases();
					this.scrollRect.verticalNormalizedPosition = 0f;
				}
			}
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x0000246F File Offset: 0x0000066F
		private void SetupMelonLogger()
		{
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x00013F88 File Offset: 0x00012188
		private void LogHandler(ConsoleColor textColor, ConsoleColor backgroundColor, string log, string stackTrace)
		{
			string category = this.DetermineCategory(log);
			LogType type = this.InferLogType(log, stackTrace);
			string cleanedMessage = this.CleanMessage(log);
			UnityMainThreadDispatcher.Instance().Enqueue(delegate
			{
				this.AddLog(cleanedMessage, category, type);
			});
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x00013FE4 File Offset: 0x000121E4
		private LogType InferLogType(string log, string stackTrace)
		{
			bool flag = log.ToLower().Contains("error") || !string.IsNullOrEmpty(stackTrace);
			LogType logType;
			if (flag)
			{
				logType = 0;
			}
			else
			{
				bool flag2 = log.ToLower().Contains("warning");
				if (flag2)
				{
					logType = 2;
				}
				else
				{
					logType = 3;
				}
			}
			return logType;
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x00014038 File Offset: 0x00012238
		private string CleanMessage(string message)
		{
			bool flag = string.IsNullOrEmpty(message);
			string text;
			if (flag)
			{
				text = string.Empty;
			}
			else
			{
				bool flag2 = message.StartsWith("[");
				if (flag2)
				{
					int num = message.IndexOf(']');
					bool flag3 = num > 0;
					if (flag3)
					{
						message = message.Substring(num + 1);
					}
				}
				text = message.Trim();
			}
			return text;
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x00014094 File Offset: 0x00012294
		private string DetermineCategory(string message)
		{
			message = message.ToLower();
			bool flag = message.Contains("error") || message.Contains("exception");
			string text;
			if (flag)
			{
				text = "Error";
			}
			else
			{
				bool flag2 = message.Contains("player") || message.Contains("joined") || message.Contains("left");
				if (flag2)
				{
					text = "Player";
				}
				else
				{
					bool flag3 = message.Contains("avatar");
					if (flag3)
					{
						text = "Avatar";
					}
					else
					{
						bool flag4 = message.Contains("world") || message.Contains("room");
						if (flag4)
						{
							text = "World";
						}
						else
						{
							bool flag5 = message.Contains("network") || message.Contains("connection");
							if (flag5)
							{
								text = "Network";
							}
							else
							{
								bool flag6 = message.Contains("event");
								if (flag6)
								{
									text = "Event";
								}
								else
								{
									text = "Default";
								}
							}
						}
					}
				}
			}
			return text;
		}

		// Token: 0x060003A6 RID: 934 RVA: 0x000033A8 File Offset: 0x000015A8
		private void ClearLogs()
		{
			this.logEntries.Clear();
			this.UpdateText();
			this.AddLog("Logs cleared", "Default", 3);
		}

		// Token: 0x060003A7 RID: 935 RVA: 0x00014198 File Offset: 0x00012398
		public override void OnPlayerJoined(Player player)
		{
			bool flag = ((player != null) ? player.field_Private_APIUser_0 : null) != null;
			if (flag)
			{
				this.AddLog("Player Joined: " + player.field_Private_APIUser_0.displayName, "Player", 3);
			}
		}

		// Token: 0x060003A8 RID: 936 RVA: 0x000141DC File Offset: 0x000123DC
		public override void OnPlayerLeft(Player player)
		{
			bool flag = ((player != null) ? player.field_Private_APIUser_0 : null) != null;
			if (flag)
			{
				this.AddLog("Player Left: " + player.field_Private_APIUser_0.displayName, "Player", 3);
			}
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x0000246F File Offset: 0x0000066F
		public override void OnUpdate()
		{
		}

		// Token: 0x0400023F RID: 575
		private GameObject logWindow;

		// Token: 0x04000240 RID: 576
		private TextMeshProUGUI logText;

		// Token: 0x04000241 RID: 577
		private Toggle autoScrollToggle;

		// Token: 0x04000242 RID: 578
		private Toggle timestampToggle;

		// Token: 0x04000243 RID: 579
		private Toggle stackTraceToggle;

		// Token: 0x04000244 RID: 580
		private Dictionary<string, Toggle> categoryToggles;

		// Token: 0x04000245 RID: 581
		private Queue<KernelLogger.LogEntry> logEntries;

		// Token: 0x04000246 RID: 582
		private bool initialized;

		// Token: 0x04000247 RID: 583
		private const int MaxLogs = 1000;

		// Token: 0x04000248 RID: 584
		private bool autoScroll = true;

		// Token: 0x04000249 RID: 585
		private bool showTimestamps = true;

		// Token: 0x0400024A RID: 586
		private bool showStackTraces = false;

		// Token: 0x0400024B RID: 587
		private HashSet<string> enabledCategories;

		// Token: 0x0400024C RID: 588
		private readonly Dictionary<string, string> CategoryColors = new Dictionary<string, string>
		{
			{ "Default", "#FFFFFF" },
			{ "Player", "#00FF00" },
			{ "Avatar", "#00FFFF" },
			{ "World", "#FF69B4" },
			{ "Network", "#4169E1" },
			{ "Event", "#FFD700" },
			{ "Error", "#FF0000" }
		};

		// Token: 0x0400024D RID: 589
		private ScrollRect scrollRect;

		// Token: 0x0400024E RID: 590
		private QuickMenu quickMenu;

		// Token: 0x02000087 RID: 135
		private class LogEntry
		{
			// Token: 0x0400024F RID: 591
			public string Message;

			// Token: 0x04000250 RID: 592
			public string Category;

			// Token: 0x04000251 RID: 593
			public string Color;

			// Token: 0x04000252 RID: 594
			public DateTime Time;

			// Token: 0x04000253 RID: 595
			public string StackTrace;

			// Token: 0x04000254 RID: 596
			public LogType Type;
		}
	}
}
