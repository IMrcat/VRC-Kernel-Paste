using System;
using UnityEngine;

namespace KernelClient
{
	// Token: 0x0200004C RID: 76
	internal class ABS : KernelModule
	{
		// Token: 0x060001C9 RID: 457 RVA: 0x00002A82 File Offset: 0x00000C82
		public override void OnUiManagerInit()
		{
			GameObject.Find("UserInterface/PlayerDisplay/BlackFade/inverted_sphere").SetActive(false);
		}
	}
}
