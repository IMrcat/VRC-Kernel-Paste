using System;
using System.Collections.Generic;

namespace KernelClient.Settings
{
	// Token: 0x02000069 RID: 105
	public class Setting
	{
		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060002AC RID: 684 RVA: 0x00002EF5 File Offset: 0x000010F5
		// (set) Token: 0x060002AD RID: 685 RVA: 0x00002EFD File Offset: 0x000010FD
		public bool Tags { get; set; } = true;

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060002AE RID: 686 RVA: 0x00002F06 File Offset: 0x00001106
		// (set) Token: 0x060002AF RID: 687 RVA: 0x00002F0E File Offset: 0x0000110E
		public bool JoinLeave { get; set; } = true;

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060002B0 RID: 688 RVA: 0x00002F17 File Offset: 0x00001117
		// (set) Token: 0x060002B1 RID: 689 RVA: 0x00002F1F File Offset: 0x0000111F
		public bool NameplateColours { get; set; } = true;

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060002B2 RID: 690 RVA: 0x00002F28 File Offset: 0x00001128
		// (set) Token: 0x060002B3 RID: 691 RVA: 0x00002F30 File Offset: 0x00001130
		public bool MenuMusic { get; set; } = true;

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060002B4 RID: 692 RVA: 0x00002F39 File Offset: 0x00001139
		// (set) Token: 0x060002B5 RID: 693 RVA: 0x00002F41 File Offset: 0x00001141
		public bool Logger { get; set; } = true;

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060002B6 RID: 694 RVA: 0x00002F4A File Offset: 0x0000114A
		// (set) Token: 0x060002B7 RID: 695 RVA: 0x00002F52 File Offset: 0x00001152
		public bool ESP { get; set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060002B8 RID: 696 RVA: 0x00002F5B File Offset: 0x0000115B
		// (set) Token: 0x060002B9 RID: 697 RVA: 0x00002F63 File Offset: 0x00001163
		public bool ItemESP { get; set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060002BA RID: 698 RVA: 0x00002F6C File Offset: 0x0000116C
		// (set) Token: 0x060002BB RID: 699 RVA: 0x00002F74 File Offset: 0x00001174
		public bool AntiBlock { get; set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060002BC RID: 700 RVA: 0x00002F7D File Offset: 0x0000117D
		// (set) Token: 0x060002BD RID: 701 RVA: 0x00002F85 File Offset: 0x00001185
		public bool Fly { get; set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060002BE RID: 702 RVA: 0x00002F8E File Offset: 0x0000118E
		// (set) Token: 0x060002BF RID: 703 RVA: 0x00002F96 File Offset: 0x00001196
		public bool ModAlert { get; set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060002C0 RID: 704 RVA: 0x00002F9F File Offset: 0x0000119F
		// (set) Token: 0x060002C1 RID: 705 RVA: 0x00002FA7 File Offset: 0x000011A7
		public bool LoudMicrophone { get; set; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060002C2 RID: 706 RVA: 0x00002FB0 File Offset: 0x000011B0
		// (set) Token: 0x060002C3 RID: 707 RVA: 0x00002FB8 File Offset: 0x000011B8
		public bool FPSLimit { get; set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060002C4 RID: 708 RVA: 0x00002FC1 File Offset: 0x000011C1
		// (set) Token: 0x060002C5 RID: 709 RVA: 0x00002FC9 File Offset: 0x000011C9
		public bool OSC { get; set; } = true;

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060002C6 RID: 710 RVA: 0x00002FD2 File Offset: 0x000011D2
		// (set) Token: 0x060002C7 RID: 711 RVA: 0x00002FDA File Offset: 0x000011DA
		public bool OSCTime { get; set; } = true;

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060002C8 RID: 712 RVA: 0x00002FE3 File Offset: 0x000011E3
		// (set) Token: 0x060002C9 RID: 713 RVA: 0x00002FEB File Offset: 0x000011EB
		public bool OSCMusic { get; set; } = true;

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060002CA RID: 714 RVA: 0x00002FF4 File Offset: 0x000011F4
		// (set) Token: 0x060002CB RID: 715 RVA: 0x00002FFC File Offset: 0x000011FC
		public bool OSCMessage { get; set; } = true;

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060002CC RID: 716 RVA: 0x00003005 File Offset: 0x00001205
		// (set) Token: 0x060002CD RID: 717 RVA: 0x0000300D File Offset: 0x0000120D
		public List<string> OSCMessageList { get; set; } = new List<string>();

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060002CE RID: 718 RVA: 0x00003016 File Offset: 0x00001216
		// (set) Token: 0x060002CF RID: 719 RVA: 0x0000301E File Offset: 0x0000121E
		public bool RamCollection { get; set; } = true;

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060002D0 RID: 720 RVA: 0x00003027 File Offset: 0x00001227
		// (set) Token: 0x060002D1 RID: 721 RVA: 0x0000302F File Offset: 0x0000122F
		public Dictionary<string, string> Hwid { get; set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060002D2 RID: 722 RVA: 0x00003038 File Offset: 0x00001238
		// (set) Token: 0x060002D3 RID: 723 RVA: 0x00003040 File Offset: 0x00001240
		public bool TagsDates { get; set; } = true;

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060002D4 RID: 724 RVA: 0x00003049 File Offset: 0x00001249
		// (set) Token: 0x060002D5 RID: 725 RVA: 0x00003051 File Offset: 0x00001251
		public bool VRCPayWallRemove { get; set; }
	}
}
