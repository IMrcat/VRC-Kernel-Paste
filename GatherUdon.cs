using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VRC.Udon;

// Token: 0x0200000B RID: 11
internal class GatherUdon
{
	// Token: 0x06000022 RID: 34 RVA: 0x00004908 File Offset: 0x00002B08
	public static void UdonGather()
	{
		GatherUdon.udonObjects = null;
		ManualResetEvent udonGatherEvent = GatherUdon.UdonGatherEvent;
		lock (udonGatherEvent)
		{
			GatherUdon.udonObjects = Resources.FindObjectsOfTypeAll<UdonBehaviour>();
			GatherUdon.UdonGatherEvent.Set();
		}
	}

	// Token: 0x06000023 RID: 35 RVA: 0x00004968 File Offset: 0x00002B68
	public static UdonBehaviour[] GetUdonObjects()
	{
		ManualResetEvent udonGatherEvent = GatherUdon.UdonGatherEvent;
		UdonBehaviour[] array;
		lock (udonGatherEvent)
		{
			GatherUdon.UdonGatherEvent.WaitOne();
			array = GatherUdon.udonObjects;
		}
		return array;
	}

	// Token: 0x0400004F RID: 79
	public static ManualResetEvent UdonGatherEvent = new ManualResetEvent(false);

	// Token: 0x04000050 RID: 80
	private static UdonBehaviour[] udonObjects;

	// Token: 0x04000051 RID: 81
	private static Dictionary<UdonBehaviour, List<string>> udonBehaviourEntries = new Dictionary<UdonBehaviour, List<string>>();
}
