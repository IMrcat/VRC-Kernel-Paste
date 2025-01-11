using System;
using System.Collections;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;

namespace KernelClient.Utilities
{
	// Token: 0x0200005F RID: 95
	public class UnityMainThreadDispatcher : MonoBehaviour
	{
		// Token: 0x06000261 RID: 609 RVA: 0x0000E9EC File Offset: 0x0000CBEC
		public static UnityMainThreadDispatcher Instance()
		{
			bool initialized = UnityMainThreadDispatcher._initialized;
			UnityMainThreadDispatcher unityMainThreadDispatcher;
			if (initialized)
			{
				unityMainThreadDispatcher = UnityMainThreadDispatcher._instance;
			}
			else
			{
				object @lock = UnityMainThreadDispatcher._lock;
				lock (@lock)
				{
					bool initialized2 = UnityMainThreadDispatcher._initialized;
					if (initialized2)
					{
						return UnityMainThreadDispatcher._instance;
					}
					Object.DontDestroyOnLoad(new GameObject("UnityMainThreadDispatcher")
					{
						hideFlags = 61
					});
					UnityMainThreadDispatcher._instance = new UnityMainThreadDispatcher();
					MelonCoroutines.Start(UnityMainThreadDispatcher._instance.UpdateCoroutine());
					UnityMainThreadDispatcher._initialized = true;
					MelonLogger.Msg("UnityMainThreadDispatcher initialized successfully");
				}
				unityMainThreadDispatcher = UnityMainThreadDispatcher._instance;
			}
			return unityMainThreadDispatcher;
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000EAA4 File Offset: 0x0000CCA4
		public void Enqueue(Action action)
		{
			object @lock = UnityMainThreadDispatcher._lock;
			lock (@lock)
			{
				UnityMainThreadDispatcher._executeOnMainThread.Enqueue(action);
			}
		}

		// Token: 0x06000263 RID: 611 RVA: 0x00002DA7 File Offset: 0x00000FA7
		private IEnumerator UpdateCoroutine()
		{
			for (;;)
			{
				object obj = UnityMainThreadDispatcher._lock;
				Queue<Action> actions;
				lock (obj)
				{
					bool flag2 = UnityMainThreadDispatcher._executeOnMainThread.Count == 0;
					if (flag2)
					{
						yield return null;
						continue;
					}
					actions = new Queue<Action>(UnityMainThreadDispatcher._executeOnMainThread);
					UnityMainThreadDispatcher._executeOnMainThread.Clear();
				}
				goto JumpOutOfTryFinally-3;
				continue;
				JumpOutOfTryFinally-3:
				obj = null;
				foreach (Action action in actions)
				{
					try
					{
						action();
					}
					catch (Exception ex)
					{
						Exception e = ex;
						MelonLogger.Error(string.Format("UnityMainThreadDispatcher: Error executing action - {0}", e));
					}
					action = null;
				}
				Queue<Action>.Enumerator enumerator = default(Queue<Action>.Enumerator);
				yield return null;
				actions = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000EAF0 File Offset: 0x0000CCF0
		public static void RunOnMainThread(Action action)
		{
			bool flag = action == null;
			if (flag)
			{
				MelonLogger.Warning("UnityMainThreadDispatcher: Attempted to run null action");
			}
			else
			{
				UnityMainThreadDispatcher.Instance().Enqueue(action);
			}
		}

		// Token: 0x06000265 RID: 613 RVA: 0x0000EB20 File Offset: 0x0000CD20
		public static void StartCoroutine(IEnumerator coroutine)
		{
			bool flag = coroutine == null;
			if (flag)
			{
				MelonLogger.Warning("UnityMainThreadDispatcher: Attempted to start null coroutine");
			}
			else
			{
				MelonCoroutines.Start(coroutine);
			}
		}

		// Token: 0x0400016F RID: 367
		private static UnityMainThreadDispatcher _instance;

		// Token: 0x04000170 RID: 368
		private static readonly Queue<Action> _executeOnMainThread = new Queue<Action>();

		// Token: 0x04000171 RID: 369
		private static readonly object _lock = new object();

		// Token: 0x04000172 RID: 370
		private static bool _initialized;
	}
}
