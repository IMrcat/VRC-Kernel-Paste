using System;
using VRC;
using VRC.Core;
using VRC.SDKBase;

namespace KernelClient.Models
{
	// Token: 0x0200006F RID: 111
	public class PlayerDetails
	{
		// Token: 0x040001C3 RID: 451
		internal string id;

		// Token: 0x040001C4 RID: 452
		internal string displayName;

		// Token: 0x040001C5 RID: 453
		internal bool isLocalPlayer;

		// Token: 0x040001C6 RID: 454
		internal bool isInstanceMaster;

		// Token: 0x040001C7 RID: 455
		internal bool isVRUser;

		// Token: 0x040001C8 RID: 456
		internal bool isQuestUser;

		// Token: 0x040001C9 RID: 457
		internal bool blockedLocalPlayer;

		// Token: 0x040001CA RID: 458
		internal Player player;

		// Token: 0x040001CB RID: 459
		internal VRCPlayerApi playerApi;

		// Token: 0x040001CC RID: 460
		internal VRCPlayer vrcPlayer;

		// Token: 0x040001CD RID: 461
		internal APIUser apiUser;
	}
}
