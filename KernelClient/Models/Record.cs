using System;

namespace KernelClient.Models
{
	// Token: 0x02000070 RID: 112
	public class Record
	{
		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060002EA RID: 746 RVA: 0x0000310A File Offset: 0x0000130A
		// (set) Token: 0x060002EB RID: 747 RVA: 0x00003112 File Offset: 0x00001312
		public int Id { get; set; }

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060002EC RID: 748 RVA: 0x0000311B File Offset: 0x0000131B
		// (set) Token: 0x060002ED RID: 749 RVA: 0x00003123 File Offset: 0x00001323
		public string UserId { get; set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060002EE RID: 750 RVA: 0x0000312C File Offset: 0x0000132C
		// (set) Token: 0x060002EF RID: 751 RVA: 0x00003134 File Offset: 0x00001334
		public string Text { get; set; }
	}
}
