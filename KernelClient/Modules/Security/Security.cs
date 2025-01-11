using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExitGames.Client.Photon;
using Il2CppSystem;
using KernelClient.Wrapper;
using MelonLoader;
using Photon.Realtime;
using ReMod.Core.UI.QuickMenu;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.Udon;

namespace KernelClient.Modules.Security
{
	// Token: 0x020000A5 RID: 165
	internal class Security : KernelModule
	{
		// Token: 0x0600045E RID: 1118 RVA: 0x00018CC4 File Offset: 0x00016EC4
		public override void OnUiManagerInit()
		{
			this.LoadConfig();
			ReMenuCategory category = MenuSetup._uiManager.QMMenu.GetCategoryPage(PageNames.Security).GetCategory(CatagoryNames.Security);
			category.AddToggle("Enable Data Validation", "Enable or disable data validation.", delegate(bool s)
			{
				this.enableDataValidation = s;
				this.config.EnableDataValidation = s;
				this.SaveConfig();
			}, this.config.EnableDataValidation);
			category.AddToggle("Enable Asset Checks", "Enable or disable asset checks.", delegate(bool s)
			{
				this.enableAssetChecks = s;
				this.config.EnableAssetChecks = s;
				this.SaveConfig();
			}, this.config.EnableAssetChecks);
			category.AddToggle("Enable Shader Checks", "Enable or disable shader checks.", delegate(bool s)
			{
				this.enableShaderChecks = s;
				this.config.EnableShaderChecks = s;
				this.SaveConfig();
			}, this.config.EnableShaderChecks);
			category.AddToggle("Enable Event Monitoring", "Enable or disable event monitoring.", delegate(bool s)
			{
				this.enableEventMonitoring = s;
				this.config.EnableEventMonitoring = s;
				this.SaveConfig();
			}, this.config.EnableEventMonitoring);
			category.AddToggle("Enable E1 Prevention", "Block known E1 exploit events (event code 1).", delegate(bool s)
			{
				this.enableE1Prevention = s;
				this.config.EnableE1Prevention = s;
				this.SaveConfig();
			}, this.config.EnableE1Prevention);
			category.AddToggle("Replace Malicious Shaders", "Replace malicious shaders with a standard shader.", delegate(bool s)
			{
				this.replaceMaliciousShaders = s;
				this.config.ReplaceMaliciousShaders = s;
				this.SaveConfig();
			}, this.config.ReplaceMaliciousShaders);
			category.AddToggle("Remove Objects with Malicious Shaders", "Remove objects that use malicious shaders.", delegate(bool s)
			{
				this.removeObjectsWithMaliciousShaders = s;
				this.config.RemoveObjectsWithMaliciousShaders = s;
				this.SaveConfig();
			}, this.config.RemoveObjectsWithMaliciousShaders);
			category.AddToggle("Enable Toasts", "Enable or disable toast notifications.", delegate(bool s)
			{
				this.config.EnableToasts = s;
				this.SaveConfig();
			}, this.config.EnableToasts);
		}

		// Token: 0x0600045F RID: 1119 RVA: 0x0000246F File Offset: 0x0000066F
		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
		}

		// Token: 0x06000460 RID: 1120 RVA: 0x00018E38 File Offset: 0x00017038
		public override bool OnEventSent(byte eventCode, object content, RaiseEventOptions options, SendOptions sendOptions)
		{
			try
			{
				Security.SecurityConfig securityConfig = this.config;
				bool flag = securityConfig != null && securityConfig.EnableDataValidation;
				if (flag)
				{
					bool flag2 = !this.IsValidEvent(eventCode);
					if (flag2)
					{
						string text = string.Format("Attempted to send invalid event code: {0}", eventCode);
						MelonLogger.Warning("[Security] " + text);
						this.ShowToast("Security Alert", text, this._warningColor);
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[Security] OnEventSent Error: {0}\n{1}", ex, ex.StackTrace));
			}
			return true;
		}

		// Token: 0x06000461 RID: 1121 RVA: 0x00018EE0 File Offset: 0x000170E0
		public override bool OnEventPatch(EventData eventData)
		{
			try
			{
				Security.SecurityConfig securityConfig = this.config;
				bool flag = securityConfig != null && securityConfig.EnableDataValidation;
				if (flag)
				{
					bool flag2 = !this.IsValidEvent(eventData.Code);
					if (flag2)
					{
						string text = string.Format("Incoming invalid event code: {0}", eventData.Code);
						MelonLogger.Warning("[Security] " + text);
						this.ShowToast("Security Alert", text, this._warningColor);
						return false;
					}
				}
				Security.SecurityConfig securityConfig2 = this.config;
				bool flag3 = securityConfig2 != null && securityConfig2.EnableE1Prevention && eventData.Code == 1;
				if (flag3)
				{
					Object customData = eventData.customData;
					bool flag4 = customData != null;
					if (flag4)
					{
						byte[] array = this.TryConvertToManagedByteArray(customData);
						bool flag5 = array != null;
						if (flag5)
						{
							string text2 = Convert.ToBase64String(array);
							bool flag6 = Security.E1_PAYLOADS.Contains(text2);
							if (flag6)
							{
								string text3 = string.Format("[E1 Prevention] Blocked incoming E1 exploit event (code {0}). Known payload detected.", eventData.Code);
								MelonLogger.Warning("[Security] " + text3);
								this.ShowToast("E1 Prevention", text3, this._warningColor);
								return false;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[Security] OnEventPatch Error: {0}\n{1}", ex, ex.StackTrace));
			}
			return true;
		}

		// Token: 0x06000462 RID: 1122 RVA: 0x00019054 File Offset: 0x00017254
		public override void OnPlayerJoined(Player player)
		{
			bool flag = player == null;
			if (flag)
			{
				MelonLogger.Error("[Security] OnPlayerJoined received a null player.");
			}
			else
			{
				try
				{
					MelonLogger.Msg("[Security] Player joined: " + this.GetPlayerName(player));
					Security.SecurityConfig securityConfig = this.config;
					bool flag2 = securityConfig != null && securityConfig.EnableAssetChecks;
					if (flag2)
					{
						this.CheckPlayerAssets(player);
					}
					Security.SecurityConfig securityConfig2 = this.config;
					bool flag3 = securityConfig2 != null && securityConfig2.EnableShaderChecks;
					if (flag3)
					{
						this.CheckPlayerShaders(player);
					}
				}
				catch (Exception ex)
				{
					MelonLogger.Error(string.Format("[Security] OnPlayerJoined Error: {0}\n{1}\nPlayerInfo: {2}", ex, ex.StackTrace, this.GetPlayerDebugInfo(player)));
				}
			}
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x00019108 File Offset: 0x00017308
		public override bool OnUdonPatch(UdonBehaviour instance, string programName)
		{
			try
			{
				Security.SecurityConfig securityConfig = this.config;
				bool flag = securityConfig != null && securityConfig.EnableEventMonitoring;
				if (flag)
				{
					this.MonitorUdonProgram(instance, programName);
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[Security] OnUdonPatch Error: {0}\n{1}\nProgramName: {2}", ex, ex.StackTrace, programName));
			}
			return true;
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x00019170 File Offset: 0x00017370
		public override void OnAvatarInstantiate(GameObject avatar, VRCPlayer vrcPlayer)
		{
			bool flag = avatar == null || vrcPlayer == null;
			if (!flag)
			{
				try
				{
					MelonLogger.Msg("[Security] Avatar instantiated: " + avatar.name);
					Security.SecurityConfig securityConfig = this.config;
					bool flag2 = securityConfig != null && securityConfig.EnableAssetChecks;
					if (flag2)
					{
						this.CheckAvatarAssets(avatar);
					}
					Security.SecurityConfig securityConfig2 = this.config;
					bool flag3 = securityConfig2 != null && securityConfig2.EnableShaderChecks;
					if (flag3)
					{
						this.CheckAvatarShaders(avatar);
					}
				}
				catch (Exception ex)
				{
					MelonLogger.Error(string.Format("[Security] OnAvatarInstantiate Error: {0}\n{1}\nAvatarInfo: {2}", ex, ex.StackTrace, this.GetAvatarDebugInfo(avatar)));
				}
			}
		}

		// Token: 0x06000465 RID: 1125 RVA: 0x00019224 File Offset: 0x00017424
		private byte[] TryConvertToManagedByteArray(Object il2cppObj)
		{
			try
			{
				bool flag = il2cppObj.GetIl2CppType().Equals(Il2CppType.Of<Il2CppStructArray<byte>>());
				if (flag)
				{
					Il2CppStructArray<byte> il2CppStructArray = il2cppObj.Cast<Il2CppStructArray<byte>>();
					return il2CppStructArray.ToArray<byte>();
				}
				Il2CppArrayBase<Byte> il2CppArrayBase = il2cppObj.TryCast<Il2CppArrayBase<Byte>>();
				bool flag2 = il2CppArrayBase != null;
				if (flag2)
				{
					int length = il2CppArrayBase.Length;
					byte[] array = new byte[length];
					for (int i = 0; i < length; i++)
					{
						array[i] = il2CppArrayBase[i].m_value;
					}
					return array;
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[Security] Byte array conversion error: {0}", ex));
			}
			return null;
		}

		// Token: 0x06000466 RID: 1126 RVA: 0x000192DC File Offset: 0x000174DC
		private bool IsValidEvent(byte eventCode)
		{
			bool flag = this._allowedEvents.Contains(eventCode);
			bool flag2 = !flag;
			if (flag2)
			{
				MelonLogger.Warning(string.Format("[Security] Invalid event code: {0}", eventCode));
			}
			return flag;
		}

		// Token: 0x06000467 RID: 1127 RVA: 0x0001931C File Offset: 0x0001751C
		private void CheckPlayerAssets(Player player)
		{
			try
			{
				VRCPlayer vrcplayer = player.Method_Internal_get_VRCPlayer_1();
				bool flag = vrcplayer == null;
				if (flag)
				{
					MelonLogger.Warning("[Security] Player '" + this.GetPlayerName(player) + "' has null VRCPlayer.");
				}
				else
				{
					VRCAvatarManager vrcavatarManager = vrcplayer.Method_Public_get_VRCAvatarManager_0();
					bool flag2 = vrcavatarManager == null;
					if (flag2)
					{
						MelonLogger.Warning("[Security] Player '" + this.GetPlayerName(player) + "' avatarManager is null.");
					}
					else
					{
						GameObject gameObject = vrcavatarManager.gameObject;
						bool flag3 = gameObject == null;
						if (flag3)
						{
							MelonLogger.Warning("[Security] Player '" + this.GetPlayerName(player) + "' avatar GameObject is null.");
						}
						else
						{
							this.CheckMeshLimits("Player", player, gameObject);
							this.CheckAudioSources("Player", player, gameObject);
							this.CheckParticleSystems("Player", player, gameObject);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[Security] CheckPlayerAssets Error: {0}\n{1}\nPlayerInfo: {2}", ex, ex.StackTrace, this.GetPlayerDebugInfo(player)));
				this.ShowToast("Security Error", "Error checking assets for '" + this.GetPlayerName(player) + "'.", this._criticalColor);
			}
		}

		// Token: 0x06000468 RID: 1128 RVA: 0x00019454 File Offset: 0x00017654
		private void CheckAvatarAssets(GameObject avatarGO)
		{
			try
			{
				this.CheckMeshLimits("Avatar", null, avatarGO);
				this.CheckAudioSources("Avatar", null, avatarGO);
				this.CheckParticleSystems("Avatar", null, avatarGO);
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[Security] CheckAvatarAssets Error: {0}\n{1}\nAvatarInfo: {2}", ex, ex.StackTrace, this.GetAvatarDebugInfo(avatarGO)));
				this.ShowToast("Security Error", "Error checking assets for avatar '" + avatarGO.name + "'.", this._criticalColor);
			}
		}

		// Token: 0x06000469 RID: 1129 RVA: 0x000194EC File Offset: 0x000176EC
		private void CheckMeshLimits(string entityName, Player player, GameObject avatarGO)
		{
			bool flag = !this.config.EnableDataValidation;
			if (!flag)
			{
				int num = 9000;
				Il2CppArrayBase<MeshFilter> componentsInChildren = avatarGO.GetComponentsInChildren<MeshFilter>(true);
				foreach (MeshFilter meshFilter in componentsInChildren)
				{
					bool flag2 = ((meshFilter != null) ? meshFilter.sharedMesh : null) == null;
					if (!flag2)
					{
						int vertexCount = meshFilter.sharedMesh.vertexCount;
						bool flag3 = vertexCount > num;
						if (flag3)
						{
							string text = string.Format("{0} '{1}' has mesh '{2}' with {3} vertices (limit {4}).", new object[]
							{
								entityName,
								(player != null) ? this.GetPlayerName(player) : avatarGO.name,
								meshFilter.sharedMesh.name,
								vertexCount,
								num
							});
							MelonLogger.Warning("[Security] " + text);
							this.ShowToast("Mesh Limit Exceeded", text, this._warningColor);
						}
					}
				}
			}
		}

		// Token: 0x0600046A RID: 1130 RVA: 0x00019614 File Offset: 0x00017814
		private void CheckAudioSources(string entityName, Player player, GameObject avatarGO)
		{
			bool flag = !this.config.EnableDataValidation;
			if (!flag)
			{
				int num = 10;
				Il2CppArrayBase<AudioSource> componentsInChildren = avatarGO.GetComponentsInChildren<AudioSource>(true);
				int length = componentsInChildren.Length;
				bool flag2 = length > num;
				if (flag2)
				{
					string text = string.Format("{0} '{1}' has {2} audio sources (limit {3}).", new object[]
					{
						entityName,
						(player != null) ? this.GetPlayerName(player) : avatarGO.name,
						length,
						num
					});
					MelonLogger.Warning("[Security] " + text);
					this.ShowToast("Audio Sources Limit Exceeded", text, this._warningColor);
				}
			}
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x000196C0 File Offset: 0x000178C0
		private void CheckParticleSystems(string entityName, Player player, GameObject avatarGO)
		{
			bool flag = !this.config.EnableDataValidation;
			if (!flag)
			{
				int num = 20;
				Il2CppArrayBase<ParticleSystem> componentsInChildren = avatarGO.GetComponentsInChildren<ParticleSystem>(true);
				int length = componentsInChildren.Length;
				bool flag2 = length > num;
				if (flag2)
				{
					string text = string.Format("{0} '{1}' has {2} particle systems (limit {3}).", new object[]
					{
						entityName,
						(player != null) ? this.GetPlayerName(player) : avatarGO.name,
						length,
						num
					});
					MelonLogger.Warning("[Security] " + text);
					this.ShowToast("Particle Systems Limit Exceeded", text, this._warningColor);
				}
			}
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x0001976C File Offset: 0x0001796C
		private void CheckPlayerShaders(Player player)
		{
			try
			{
				VRCPlayer vrcplayer = player.Method_Internal_get_VRCPlayer_1();
				bool flag = vrcplayer == null;
				if (flag)
				{
					MelonLogger.Warning("[Security] Player '" + this.GetPlayerName(player) + "' vrcPlayer is null. Skipping shader checks.");
				}
				else
				{
					VRCAvatarManager vrcavatarManager = vrcplayer.Method_Public_get_VRCAvatarManager_0();
					bool flag2 = vrcavatarManager == null;
					if (flag2)
					{
						MelonLogger.Warning("[Security] Player '" + this.GetPlayerName(player) + "' avatarManager is null. Skipping shader checks.");
					}
					else
					{
						GameObject gameObject = vrcavatarManager.gameObject;
						bool flag3 = gameObject == null;
						if (flag3)
						{
							MelonLogger.Warning("[Security] Player '" + this.GetPlayerName(player) + "' avatar GameObject is null. Skipping shader checks.");
						}
						else
						{
							this.CheckShaders("Player", player, gameObject);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[Security] CheckPlayerShaders Error: {0}\n{1}\nInfo: {2}", ex, ex.StackTrace, this.GetPlayerDebugInfo(player)));
				this.ShowToast("Security Error", "Error checking shaders for '" + this.GetPlayerName(player) + "'.", this._criticalColor);
			}
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x00019888 File Offset: 0x00017A88
		private void CheckAvatarShaders(GameObject avatarGO)
		{
			try
			{
				this.CheckShaders("Avatar", null, avatarGO);
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[Security] CheckAvatarShaders Error: {0}\n{1}\nAvatarInfo: {2}", ex, ex.StackTrace, this.GetAvatarDebugInfo(avatarGO)));
				this.ShowToast("Security Error", "Error checking shaders for avatar '" + avatarGO.name + "'.", this._criticalColor);
			}
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x00019904 File Offset: 0x00017B04
		private void CheckShaders(string entityName, Player player, GameObject avatarGO)
		{
			Il2CppArrayBase<Renderer> componentsInChildren = avatarGO.GetComponentsInChildren<Renderer>(true);
			foreach (Renderer renderer in componentsInChildren)
			{
				bool flag = ((renderer != null) ? renderer.sharedMaterials : null) == null;
				if (!flag)
				{
					foreach (Material material in renderer.sharedMaterials)
					{
						bool flag2 = material == null || material.shader == null;
						if (!flag2)
						{
							string name = material.shader.name;
							bool flag3 = this._maliciousShaders.Contains(name);
							bool flag4 = material.shader.renderQueue > 3000;
							if (flag4)
							{
								flag3 = true;
								MelonLogger.Warning(string.Format("[Security] Shader '{0}' has a high renderQueue: {1}", name, material.shader.renderQueue));
							}
							bool flag5 = material.shader.renderQueue < -3000;
							if (flag5)
							{
								flag3 = true;
								MelonLogger.Warning(string.Format("[Security] Shader '{0}' has a VERY low renderQueue: {1}", name, material.shader.renderQueue));
							}
							bool flag6 = flag3;
							if (flag6)
							{
								string text = ((player != null) ? ("Player '" + this.GetPlayerName(player) + "'") : ("Avatar '" + avatarGO.name + "'"));
								string text2 = text + " uses malicious shader: " + name;
								MelonLogger.Warning("[Security] " + text2);
								this.ShowToast("Malicious Shader Detected", text2, this._criticalColor);
								bool flag7 = this.config.ReplaceMaliciousShaders;
								if (flag7)
								{
									Shader shader = Shader.Find(this.config.ReplacementShaderName);
									bool flag8 = shader != null;
									if (flag8)
									{
										material.shader = shader;
										MelonLogger.Msg(string.Concat(new string[]
										{
											"[Security] Replaced shader '",
											name,
											"' with '",
											this.config.ReplacementShaderName,
											"'."
										}));
										this.ShowToast("Shader Replaced", string.Concat(new string[]
										{
											"Replaced '",
											name,
											"' with '",
											this.config.ReplacementShaderName,
											"'."
										}), this._warningColor);
									}
									else
									{
										MelonLogger.Error("[Security] Replacement shader '" + this.config.ReplacementShaderName + "' not found.");
										this.ShowToast("Shader Replacement Failed", "Replacement shader '" + this.config.ReplacementShaderName + "' not found.", this._criticalColor);
									}
								}
								bool flag9 = this.config.RemoveObjectsWithMaliciousShaders;
								if (flag9)
								{
									renderer.gameObject.SetActive(false);
									MelonLogger.Msg("[Security] Disabled GameObject '" + renderer.gameObject.name + "' due to malicious shader.");
									this.ShowToast("Object Disabled", "Disabled '" + renderer.gameObject.name + "' due to malicious shader.", this._criticalColor);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600046F RID: 1135 RVA: 0x00019C94 File Offset: 0x00017E94
		private void MonitorUdonProgram(UdonBehaviour instance, string programName)
		{
			try
			{
				bool flag = this._blockedUdonProgramNames.Contains(programName);
				if (flag)
				{
					string text = "Blocked Udon program executed: " + programName;
					MelonLogger.Warning("[Security] " + text);
					this.ShowToast("Blocked Udon Program", text, this._criticalColor);
					bool flag2 = instance != null;
					if (flag2)
					{
						instance.enabled = false;
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[Security] MonitorUdonProgram Error: {0}\n{1}\nProgramName: {2}", ex, ex.StackTrace, programName));
				this.ShowToast("Security Error", "Error monitoring Udon program.", this._criticalColor);
			}
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x00018AA0 File Offset: 0x00016CA0
		private string GetPlayerName(Player player)
		{
			string text;
			if (player == null)
			{
				text = null;
			}
			else
			{
				APIUser apiuser = player.Method_Internal_get_APIUser_0();
				text = ((apiuser != null) ? apiuser.displayName : null);
			}
			return text ?? "Unknown";
		}

		// Token: 0x06000471 RID: 1137 RVA: 0x00018AD4 File Offset: 0x00016CD4
		private string GetPlayerDebugInfo(Player player)
		{
			bool flag = player == null;
			string text;
			if (flag)
			{
				text = "Player is null";
			}
			else
			{
				APIUser apiuser = player.Method_Internal_get_APIUser_0();
				string text2 = ((apiuser != null) ? apiuser.displayName : null) ?? "Unknown";
				APIUser apiuser2 = player.Method_Internal_get_APIUser_0();
				string text3 = ((apiuser2 != null) ? apiuser2.id : null) ?? "UnknownID";
				bool flag2 = player.Method_Public_get_VRCPlayerApi_0() != null;
				text = string.Format("Name: {0}, ID: {1}, HasPlayerAPI: {2}", text2, text3, flag2);
			}
			return text;
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x00019D40 File Offset: 0x00017F40
		private string GetAvatarDebugInfo(GameObject avatarGO)
		{
			bool flag = avatarGO == null;
			string text;
			if (flag)
			{
				text = "Avatar GameObject is null";
			}
			else
			{
				text = "Avatar Name: " + avatarGO.name;
			}
			return text;
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x00019D78 File Offset: 0x00017F78
		private void ShowToast(string title, string message, Color color)
		{
			bool flag = !this.config.EnableToasts;
			if (!flag)
			{
				try
				{
					ToastNotif.Toast(title, message, EmbeddedResourceLoader.LoadEmbeddedSprite("KernelClient.assets.security.png"), this.config.ToastDuration);
				}
				catch (Exception ex)
				{
					MelonLogger.Error(string.Concat(new string[] { "[Security] ShowToast Error: ", ex.Message, "\n", ex.StackTrace, "\nTitle: ", title, ", Message: ", message }));
				}
			}
		}

		// Token: 0x06000474 RID: 1140 RVA: 0x00019E1C File Offset: 0x0001801C
		private void LoadConfig()
		{
			try
			{
				bool flag = File.Exists(this.configFilePath);
				if (flag)
				{
					string text = File.ReadAllText(this.configFilePath);
					Security.SecurityConfigWrapper securityConfigWrapper = JsonUtility.FromJson<Security.SecurityConfigWrapper>(text);
					bool flag2 = securityConfigWrapper != null && securityConfigWrapper.Config != null;
					if (flag2)
					{
						this.config = securityConfigWrapper.Config;
						this.enableDataValidation = this.config.EnableDataValidation;
						this.enableAssetChecks = this.config.EnableAssetChecks;
						this.enableShaderChecks = this.config.EnableShaderChecks;
						this.enableEventMonitoring = this.config.EnableEventMonitoring;
						this.enableE1Prevention = this.config.EnableE1Prevention;
						this.replaceMaliciousShaders = this.config.ReplaceMaliciousShaders;
						this.removeObjectsWithMaliciousShaders = this.config.RemoveObjectsWithMaliciousShaders;
						this.replacementShaderName = this.config.ReplacementShaderName;
						MelonLogger.Msg("[Security] Loaded security config from " + this.configFilePath);
					}
					else
					{
						this.config = new Security.SecurityConfig();
						this.SaveConfig();
					}
				}
				else
				{
					this.config = new Security.SecurityConfig();
					this.SaveConfig();
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[Security] Failed to load security config: {0}", ex));
				this.config = new Security.SecurityConfig();
			}
		}

		// Token: 0x06000475 RID: 1141 RVA: 0x00019F84 File Offset: 0x00018184
		private void SaveConfig()
		{
			try
			{
				Security.SecurityConfigWrapper securityConfigWrapper = new Security.SecurityConfigWrapper
				{
					Config = this.config
				};
				string text = JsonUtility.ToJson<Security.SecurityConfigWrapper>(securityConfigWrapper, true);
				File.WriteAllText(this.configFilePath, text);
				MelonLogger.Msg("[Security] Security config saved to " + this.configFilePath);
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[Security] Failed to save security config: {0}", ex));
			}
		}

		// Token: 0x040002F1 RID: 753
		private bool enableDataValidation = true;

		// Token: 0x040002F2 RID: 754
		private bool enableAssetChecks = true;

		// Token: 0x040002F3 RID: 755
		private bool enableShaderChecks = true;

		// Token: 0x040002F4 RID: 756
		private bool enableEventMonitoring = true;

		// Token: 0x040002F5 RID: 757
		private static readonly string[] E1_PAYLOADS = new string[]
		{
			"QwYAAEWhESjzrD0A+PTUA4+bi+0LaUxtDTCBf75zt9hhu0RMSn256S+Z5UFCa3TTpz7Vn+dqmsK22eM1c3QV2OkEnvb+V/VMgfSsPQD49NQ67K6//enjQ8caLBaso6feWZyjV1q6GQ09u6w6bw91CJzBBv8QxGNMEa8S0ZHgYsGLpNZYHzhn03iA9aw9APj0cbv6WD3sl6rbmZYvfDksrFMhDuaBoQeYWfXNDDFik9egcVcvAPfocJkwpJ7vRPS5QgCfiNUdn/AGbIH2rD0A+PTUNLlMaIau6JuUEFFVYpv/yWOVDLSshOI1mmUB9ujkr8KEmIu3keB87DekOFGRmaNgu8TWVvVXjTLogPesPQD49HQVoW8ADMH2KouFZ8eZB3tv/2X+ld6MklOeIE7HE+cY+m1QEkeUgdM0Fc+vQi5ZI21+sAEnmaXx1WqB", "AgAAAKWkyYm7hjsA+H3owFygUv4w5B67lcSx14zff9FCPADiNbSwYWgE+O7DrSy5tkRecs21ljjofvebe6xsYlA4cVmgrd0=", "CAAAADx3SG8eYzoAeJYLkm09XMKdEYOE+l/Enz9tFP5pplHGi/o+XDjCGrpnUeqSW8wWfhfzPXkD9ReI8ioo7fXi1GOdZB9jMgB4n64Y5JoaLUt0dZXF/XHDuQFpIj73SHaA0FyjQrj1oIU81HPlhGZy3NqVLFXilJVY4Q==", "AAAAAGfp+Lv2GRkA+DrJaWerbtZm+SX2//kATwCqqvu/z6rLog==", "AAAAAGfp+Lv2GRkA+DrDusChW99guelWc00gcgDuhh911CBUpe==", "AAAAAGfp+Lv2GRkA+DrDusChW99tttlWc00gcgDuhh911CBUpe==", "AAAAAGfp+Lv2GRkA+DrDussssss99wssstssssstttsswwsese==", "AAAAAGfp+Lv2GRkA+DrDusChW99guelWc00gcgDuhh911CBUpe==", "AAAAAGfp+Lv2GRkA+DrJaWerbistduJawerbistdujawerbist==", "AAAAAGfp+Lv2GRkA+DrDusChW9QFHJlWc00gcgDuhh911CBUpe==",
			"AAAAAGfp+Lv2GRkA+DrDufffffftrhehfgdhgdrgerer1CBUpe==", "AAAAAOlO/DO7hjsA+H3owFygUv4w5B67lcSx14zff9FCPADiNbSwYWgE+O7DrSy5tkRecs21ljjofvebe6xsYlA4cVmgrd0="
		};

		// Token: 0x040002F6 RID: 758
		private bool enableE1Prevention = false;

		// Token: 0x040002F7 RID: 759
		private bool replaceMaliciousShaders = true;

		// Token: 0x040002F8 RID: 760
		private bool removeObjectsWithMaliciousShaders = false;

		// Token: 0x040002F9 RID: 761
		private string replacementShaderName = "Standard";

		// Token: 0x040002FA RID: 762
		private readonly string configFilePath = Path.Combine(MelonUtils.UserDataDirectory, "LocalSecurity.json");

		// Token: 0x040002FB RID: 763
		private Security.SecurityConfig config;

		// Token: 0x040002FC RID: 764
		private Color _warningColor = Color.yellow;

		// Token: 0x040002FD RID: 765
		private Color _criticalColor = Color.red;

		// Token: 0x040002FE RID: 766
		private List<byte> _allowedEvents = new List<byte>
		{
			1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
			11, 12, 13, 16, 17, 20, 21, 24, 27, 26,
			28, 25, 37, 22, 23, 33, 34, 35, 40, 42,
			43, 44, 51, 53, 60, 66, 70, 71, 73, 74,
			200, 201, 202, 203, 204, 205, 206, 207, 208, 209,
			210, 211, 212, 220, 223, 224, 226, 227, 228, 229,
			230, 250, 251, 253, 254, byte.MaxValue
		};

		// Token: 0x040002FF RID: 767
		private List<string> _maliciousShaders = new List<string>
		{
			"AV/DEBUG/Distance Crash", "Custom/Oof", "FX/StarNest", "Custom/KYScotty", "Custom/Crashers/Sprythu (Why is my screen black)", "Custom/Custom", "Pretty", "Temmie/Basically Standard", "Slipknot/grim/Planet crash", "Yiing Shader Fract",
			"XxMAKOCHILLxX/CRASH/invisibleCrash", "XxMAKOCHILLxX/CRASH/RAPEGPU", "Woofaa/Crash", "Jelly/Component Frier", "Kortana/_{CRASH}/Tessellated Ass", "UMBRELLA KILLER", ".Star/Bacon", "Gówno/na/Sfere", "Nabe/School Shooter", "Bluethefox/Fatalcube/Weave",
			"PoH/Screenfreeze 1", "✞"
		};

		// Token: 0x04000300 RID: 768
		private List<string> _blockedUdonProgramNames = new List<string>();

		// Token: 0x020000A6 RID: 166
		[Serializable]
		public class SecurityConfig
		{
			// Token: 0x04000301 RID: 769
			public bool EnableDataValidation = true;

			// Token: 0x04000302 RID: 770
			public bool EnableAssetChecks = true;

			// Token: 0x04000303 RID: 771
			public bool EnableShaderChecks = true;

			// Token: 0x04000304 RID: 772
			public bool EnableEventMonitoring = true;

			// Token: 0x04000305 RID: 773
			public bool EnableE1Prevention = false;

			// Token: 0x04000306 RID: 774
			public bool ReplaceMaliciousShaders = true;

			// Token: 0x04000307 RID: 775
			public bool RemoveObjectsWithMaliciousShaders = false;

			// Token: 0x04000308 RID: 776
			public string ReplacementShaderName = "Standard";

			// Token: 0x04000309 RID: 777
			public bool EnableToasts = true;

			// Token: 0x0400030A RID: 778
			public float ToastDuration = 5f;
		}

		// Token: 0x020000A7 RID: 167
		[Serializable]
		public class SecurityConfigWrapper
		{
			// Token: 0x0400030B RID: 779
			public Security.SecurityConfig Config = new Security.SecurityConfig();
		}
	}
}
