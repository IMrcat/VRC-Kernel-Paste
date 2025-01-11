using System;
using System.Collections.Generic;
using MelonLoader;
using VRC;

namespace KernelClient
{
	// Token: 0x02000055 RID: 85
	public class Plugin : MelonMod
	{
		// Token: 0x06000219 RID: 537 RVA: 0x00002C4F File Offset: 0x00000E4F
		public override void OnInitializeMelon()
		{
			Loader.Start();
			MelonLogger.Msg("KernelClient Plugin is loaded! | PASTED AS ALWAYS<333");
		}

		// Token: 0x0600021A RID: 538 RVA: 0x00002C60 File Offset: 0x00000E60
		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			Loader.OnSceneWasLoaded(buildIndex, sceneName);
		}

		// Token: 0x0600021B RID: 539 RVA: 0x00002C69 File Offset: 0x00000E69
		public override void OnUpdate()
		{
			Loader.OnUpdate();
		}

		// Token: 0x0400015A RID: 346
		private HashSet<Player> _knownPlayers = new HashSet<Player>();

		// Token: 0x0400015B RID: 347
		private Loader l = new Loader();
	}
}
