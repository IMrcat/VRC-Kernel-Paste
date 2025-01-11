using System;
using UnityEngine;

namespace KernelClient.Settings.Wrappers
{
	// Token: 0x0200006E RID: 110
	public class HighlightsFXSettings
	{
		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060002E3 RID: 739 RVA: 0x000030C8 File Offset: 0x000012C8
		// (set) Token: 0x060002E4 RID: 740 RVA: 0x000030D0 File Offset: 0x000012D0
		public Color HighlightColor { get; set; } = Color.cyan;

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060002E5 RID: 741 RVA: 0x000030D9 File Offset: 0x000012D9
		// (set) Token: 0x060002E6 RID: 742 RVA: 0x000030E1 File Offset: 0x000012E1
		public float OutlineWidth { get; set; } = 2f;

		// Token: 0x060002E8 RID: 744 RVA: 0x0000FDBC File Offset: 0x0000DFBC
		public void ApplySettings(HighlightsFX highlightsFX)
		{
			bool flag = highlightsFX == null;
			if (!flag)
			{
				highlightsFX.field_Protected_Material_0.color = this.HighlightColor;
			}
		}
	}
}
