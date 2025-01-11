using System;
using System.Collections.Generic;
using UnityEngine;

namespace KernelClient.Modules
{
	// Token: 0x0200008B RID: 139
	public class UnityMainThreadDispatcher : MonoBehaviour
	{
		// Token: 0x060003B9 RID: 953 RVA: 0x00014378 File Offset: 0x00012578
		public static UnityMainThreadDispatcher Instance()
		{
			bool flag = !UnityMainThreadDispatcher._instance;
			if (flag)
			{
				GameObject gameObject = new GameObject("UnityMainThreadDispatcher");
				Object.DontDestroyOnLoad(gameObject);
				UnityMainThreadDispatcher._instance = gameObject.AddComponent<UnityMainThreadDispatcher>();
			}
			return UnityMainThreadDispatcher._instance;
		}

		// Token: 0x060003BA RID: 954 RVA: 0x000143C0 File Offset: 0x000125C0
		public void Enqueue(Action action)
		{
			Queue<Action> executionQueue = UnityMainThreadDispatcher._executionQueue;
			lock (executionQueue)
			{
				UnityMainThreadDispatcher._executionQueue.Enqueue(action);
			}
		}

		// Token: 0x060003BB RID: 955 RVA: 0x0001440C File Offset: 0x0001260C
		private void Update()
		{
			for (;;)
			{
				Action action = null;
				Queue<Action> executionQueue = UnityMainThreadDispatcher._executionQueue;
				lock (executionQueue)
				{
					bool flag2 = UnityMainThreadDispatcher._executionQueue.Count > 0;
					if (flag2)
					{
						action = UnityMainThreadDispatcher._executionQueue.Dequeue();
					}
				}
				bool flag3 = action == null;
				if (flag3)
				{
					break;
				}
				action();
			}
		}

		// Token: 0x04000260 RID: 608
		private static readonly Queue<Action> _executionQueue = new Queue<Action>();

		// Token: 0x04000261 RID: 609
		private static UnityMainThreadDispatcher _instance = null;
	}
}
