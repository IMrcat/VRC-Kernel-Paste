using System;
using System.IO;
using MelonLoader;

namespace KernelClient.Modules.Security
{
	// Token: 0x020000A8 RID: 168
	[Serializable]
	public class SecurityConfig
	{
		// Token: 0x06000482 RID: 1154 RVA: 0x0001A518 File Offset: 0x00018718
		public static SecurityConfig Load()
		{
			SecurityConfig securityConfig2;
			try
			{
				bool flag = !File.Exists(SecurityConfig.ConfigPath);
				if (flag)
				{
					SecurityConfig securityConfig = new SecurityConfig();
					securityConfig.Save();
					securityConfig2 = securityConfig;
				}
				else
				{
					string text = File.ReadAllText(SecurityConfig.ConfigPath);
					SecurityConfig securityConfig3 = JsonUtility.FromJson<SecurityConfig>(text);
					bool flag2 = securityConfig3 == null;
					if (flag2)
					{
						MelonLogger.Error("[Security] Config malformed. Using defaults.");
						securityConfig2 = new SecurityConfig();
					}
					else
					{
						MelonLogger.Msg("[Security] Configuration loaded from file.");
						securityConfig2 = securityConfig3;
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[Security] Failed to load config: {0}\n{1}", ex, ex.StackTrace));
				securityConfig2 = new SecurityConfig();
			}
			return securityConfig2;
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x0001A5C4 File Offset: 0x000187C4
		public void Save()
		{
			try
			{
				string text = JsonUtility.ToJson<SecurityConfig>(this, true);
				File.WriteAllText(SecurityConfig.ConfigPath, text);
				MelonLogger.Msg("[Security] Configuration saved successfully.");
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[Security] Failed to save config: {0}\n{1}", ex, ex.StackTrace));
			}
		}

		// Token: 0x0400030C RID: 780
		public bool EnableDataValidation = true;

		// Token: 0x0400030D RID: 781
		public bool EnableAssetChecks = true;

		// Token: 0x0400030E RID: 782
		public bool EnableEventMonitoring = true;

		// Token: 0x0400030F RID: 783
		public bool EnableShaderChecks = true;

		// Token: 0x04000310 RID: 784
		public bool EnableMaxMeshVerticesCheck = true;

		// Token: 0x04000311 RID: 785
		public bool EnableMaxAudioSourcesCheck = true;

		// Token: 0x04000312 RID: 786
		public bool EnableMaxParticleSystemsCheck = true;

		// Token: 0x04000313 RID: 787
		public int MaxMeshVertices = 5000;

		// Token: 0x04000314 RID: 788
		public int MaxAudioSources = 10;

		// Token: 0x04000315 RID: 789
		public int MaxParticleSystems = 20;

		// Token: 0x04000316 RID: 790
		private static readonly string ConfigPath = Path.Combine(MelonUtils.UserDataDirectory, "SecurityConfig.json");
	}
}
