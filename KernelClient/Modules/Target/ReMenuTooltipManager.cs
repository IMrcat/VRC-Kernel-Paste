using System;
using UnityEngine;
using UnityEngine.UI;

namespace KernelClient.Modules.Target
{
	// Token: 0x02000099 RID: 153
	public class ReMenuTooltipManager : MonoBehaviour
	{
		// Token: 0x1700006C RID: 108
		// (get) Token: 0x0600042E RID: 1070 RVA: 0x000036C2 File Offset: 0x000018C2
		// (set) Token: 0x0600042F RID: 1071 RVA: 0x000036C9 File Offset: 0x000018C9
		public static ReMenuTooltipManager Instance { get; private set; }

		// Token: 0x06000430 RID: 1072 RVA: 0x00017D3C File Offset: 0x00015F3C
		private void Awake()
		{
			bool flag = ReMenuTooltipManager.Instance == null;
			if (flag)
			{
				ReMenuTooltipManager.Instance = this;
				Object.DontDestroyOnLoad(base.gameObject);
				this._tooltipObject = new GameObject("ReMenuTooltip");
				this._tooltipObject.transform.SetParent(base.transform, false);
				this._tooltipObject.SetActive(false);
				this._tooltipText = this._tooltipObject.AddComponent<Text>();
				this._tooltipText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
				this._tooltipText.fontSize = 12;
				this._tooltipText.color = Color.yellow;
				this._tooltipText.alignment = 4;
				RectTransform component = this._tooltipObject.GetComponent<RectTransform>();
				component.sizeDelta = new Vector2(200f, 30f);
				component.anchorMin = new Vector2(0.5f, 0f);
				component.anchorMax = new Vector2(0.5f, 0f);
				component.pivot = new Vector2(0.5f, 1f);
				component.anchoredPosition = new Vector2(0f, 10f);
				this._tooltipObject.AddComponent<CanvasRenderer>();
				Image image = this._tooltipObject.AddComponent<Image>();
				image.color = new Color(0f, 0f, 0f, 0.7f);
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06000431 RID: 1073 RVA: 0x00017EBC File Offset: 0x000160BC
		public void ShowTooltip(string text)
		{
			this._tooltipObject.SetActive(true);
			this._tooltipText.text = text;
			Vector2 vector = Input.mousePosition;
			this._tooltipObject.GetComponent<RectTransform>().position = vector;
		}

		// Token: 0x06000432 RID: 1074 RVA: 0x000036D1 File Offset: 0x000018D1
		public void HideTooltip()
		{
			this._tooltipObject.SetActive(false);
		}

		// Token: 0x040002D1 RID: 721
		private GameObject _tooltipObject;

		// Token: 0x040002D2 RID: 722
		private Text _tooltipText;
	}
}
