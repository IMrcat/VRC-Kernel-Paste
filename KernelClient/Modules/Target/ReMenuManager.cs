using System;
using UnityEngine;

namespace KernelClient.Modules.Target
{
	// Token: 0x02000098 RID: 152
	public class ReMenuManager : MonoBehaviour
	{
		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000427 RID: 1063 RVA: 0x00003697 File Offset: 0x00001897
		// (set) Token: 0x06000428 RID: 1064 RVA: 0x0000369E File Offset: 0x0000189E
		public static ReMenuManager Instance { get; private set; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000429 RID: 1065 RVA: 0x000036A6 File Offset: 0x000018A6
		// (set) Token: 0x0600042A RID: 1066 RVA: 0x000036AE File Offset: 0x000018AE
		public GameObject CurrentPage { get; private set; }

		// Token: 0x0600042B RID: 1067 RVA: 0x00017CF8 File Offset: 0x00015EF8
		private void Awake()
		{
			bool flag = ReMenuManager.Instance == null;
			if (flag)
			{
				ReMenuManager.Instance = this;
				Object.DontDestroyOnLoad(base.gameObject);
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x000036B7 File Offset: 0x000018B7
		public void SetCurrentPage(GameObject page)
		{
			this.CurrentPage = page;
		}
	}
}
