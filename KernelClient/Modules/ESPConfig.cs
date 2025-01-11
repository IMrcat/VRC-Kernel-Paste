using System;
using System.Collections.Generic;

namespace KernelClient.Modules
{
	// Token: 0x02000085 RID: 133
	public class ESPConfig
	{
		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000390 RID: 912 RVA: 0x00003344 File Offset: 0x00001544
		// (set) Token: 0x06000391 RID: 913 RVA: 0x0000334C File Offset: 0x0000154C
		public bool ESPEnabled { get; set; } = true;

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000392 RID: 914 RVA: 0x00003355 File Offset: 0x00001555
		// (set) Token: 0x06000393 RID: 915 RVA: 0x0000335D File Offset: 0x0000155D
		public bool ItemESPEnabled { get; set; } = true;

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000394 RID: 916 RVA: 0x00003366 File Offset: 0x00001566
		// (set) Token: 0x06000395 RID: 917 RVA: 0x0000336E File Offset: 0x0000156E
		public List<string> HighlightedPlayerIds { get; set; } = new List<string>();
	}
}
