using System;
using UnityEngine;
using UnityEngine.UI;

namespace KernelClient.Modules.Target
{
	// Token: 0x02000097 RID: 151
	public class ReMenuLabel
	{
		// Token: 0x06000424 RID: 1060 RVA: 0x00017B94 File Offset: 0x00015D94
		public ReMenuLabel(string text, string tooltip, Transform parent)
		{
			this._labelObject = new GameObject("ReMenuLabel");
			this._labelObject.transform.SetParent(parent, false);
			this._textComponent = this._labelObject.AddComponent<Text>();
			this._textComponent.text = text;
			this._textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			this._textComponent.fontSize = 14;
			this._textComponent.alignment = 3;
			this._textComponent.color = Color.white;
			LayoutElement layoutElement = this._labelObject.AddComponent<LayoutElement>();
			layoutElement.minHeight = 20f;
			layoutElement.preferredHeight = 20f;
			bool flag = !string.IsNullOrEmpty(tooltip);
			if (flag)
			{
				Tooltip tooltip2 = this._labelObject.AddComponent<Tooltip>();
				tooltip2.SetTooltip(tooltip);
			}
			RectTransform component = this._labelObject.GetComponent<RectTransform>();
			component.sizeDelta = new Vector2(200f, 20f);
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x00017C98 File Offset: 0x00015E98
		public void SetText(string newText)
		{
			bool flag = this._textComponent != null;
			if (flag)
			{
				this._textComponent.text = newText;
			}
		}

		// Token: 0x06000426 RID: 1062 RVA: 0x00017CC8 File Offset: 0x00015EC8
		public void SetTooltip(string newTooltip)
		{
			Tooltip component = this._labelObject.GetComponent<Tooltip>();
			bool flag = component != null;
			if (flag)
			{
				component.SetTooltip(newTooltip);
			}
		}

		// Token: 0x040002CD RID: 717
		private Text _textComponent;

		// Token: 0x040002CE RID: 718
		private GameObject _labelObject;
	}
}
