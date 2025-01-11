using System;
using UnityEngine;

namespace KernelClient
{
	// Token: 0x0200004B RID: 75
	public static class GameObjectExtensions
	{
		// Token: 0x060001C8 RID: 456 RVA: 0x0000BEF4 File Offset: 0x0000A0F4
		public static bool IsDestroyed(this GameObject gameObject)
		{
			return gameObject == null || !gameObject;
		}
	}
}
