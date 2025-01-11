using System;
using UnityEngine;

namespace KernelClient.Modules.Target
{
	// Token: 0x0200009B RID: 155
	public class Tooltip : MonoBehaviour
	{
		// Token: 0x06000439 RID: 1081 RVA: 0x000036E1 File Offset: 0x000018E1
		public void SetTooltip(string tooltip)
		{
			this._tooltipText = tooltip;
		}

		// Token: 0x0600043A RID: 1082 RVA: 0x000036EB File Offset: 0x000018EB
		private void OnMouseEnter()
		{
			ReMenuTooltipManager.Instance.ShowTooltip(this._tooltipText);
		}

		// Token: 0x0600043B RID: 1083 RVA: 0x000036FF File Offset: 0x000018FF
		private void OnMouseExit()
		{
			ReMenuTooltipManager.Instance.HideTooltip();
		}

		// Token: 0x040002D5 RID: 725
		private string _tooltipText;
	}
}
