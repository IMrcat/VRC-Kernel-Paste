using System;
using System.Collections;
using UnityEngine;

namespace KernelClient.Handlers
{
	// Token: 0x02000072 RID: 114
	public class CoroutineHandler : MonoBehaviour
	{
		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060002F4 RID: 756 RVA: 0x0000FDEC File Offset: 0x0000DFEC
		public static CoroutineHandler Instance
		{
			get
			{
				bool flag = CoroutineHandler._instance == null;
				if (flag)
				{
					GameObject gameObject = new GameObject("CoroutineHandler");
					CoroutineHandler._instance = gameObject.AddComponent<CoroutineHandler>();
					Object.DontDestroyOnLoad(gameObject);
					Debug.Log("[CoroutineHandler] Instance created.");
				}
				return CoroutineHandler._instance;
			}
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x0000FE44 File Offset: 0x0000E044
		private void Awake()
		{
			bool flag = CoroutineHandler._instance == null;
			if (flag)
			{
				CoroutineHandler._instance = this;
				Object.DontDestroyOnLoad(base.gameObject);
				Debug.Log("[CoroutineHandler] Instance set in Awake.");
			}
			else
			{
				bool flag2 = CoroutineHandler._instance != this;
				if (flag2)
				{
					Object.Destroy(base.gameObject);
					Debug.Log("[CoroutineHandler] Duplicate instance destroyed.");
				}
			}
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x0000FEB4 File Offset: 0x0000E0B4
		public Coroutine StartNewCoroutine(IEnumerator routine)
		{
			bool flag = routine == null;
			Coroutine coroutine;
			if (flag)
			{
				Debug.LogError("[CoroutineHandler] Attempted to start a null coroutine.");
				coroutine = null;
			}
			else
			{
				coroutine = base.StartCoroutine(routine.ToString());
			}
			return coroutine;
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x0000FEF0 File Offset: 0x0000E0F0
		public Coroutine StartNewCoroutine(string methodName)
		{
			bool flag = string.IsNullOrEmpty(methodName);
			Coroutine coroutine;
			if (flag)
			{
				Debug.LogError("[CoroutineHandler] Attempted to start a coroutine with a null or empty method name.");
				coroutine = null;
			}
			else
			{
				coroutine = base.StartCoroutine(methodName);
			}
			return coroutine;
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x0000FF28 File Offset: 0x0000E128
		public void StopExistingCoroutine(Coroutine routine)
		{
			bool flag = routine != null;
			if (flag)
			{
				base.StopCoroutine(routine);
			}
			else
			{
				Debug.LogWarning("[CoroutineHandler] Attempted to stop a null coroutine.");
			}
		}

		// Token: 0x040001D2 RID: 466
		private static CoroutineHandler _instance;
	}
}
