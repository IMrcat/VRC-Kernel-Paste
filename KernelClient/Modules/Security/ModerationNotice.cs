using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExitGames.Client.Photon;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using KernelClient.Utils;
using KernelClient.Wrapper;
using MelonLoader;
using ReMod.Core.UI.QuickMenu;
using UnityEngine;
using VRC;
using VRC.Core;

namespace KernelClient.Modules.Security
{
	// Token: 0x0200009E RID: 158
	internal class ModerationNotice : KernelModule
	{
		// Token: 0x06000441 RID: 1089 RVA: 0x00018108 File Offset: 0x00016308
		public override void OnUiManagerInit()
		{
			this.LoadConfig();
			ReMenuCategory category = MenuSetup._uiManager.QMMenu.GetCategoryPage(PageNames.Security).GetCategory(CatagoryNames.Alerts);
			category.AddToggle("Moderation Alert", "Enable or disable moderation alerts.", delegate(bool s)
			{
				this.antiblock = s;
				this.config.Antiblock = s;
				this.SaveConfig();
			}, this.config.Antiblock);
			category.AddToggle("Save Blocks", "Enable to save block actions to file.", delegate(bool s)
			{
				this.saveBlocks = s;
				this.config.SaveBlocks = s;
				this.SaveConfig();
				bool flag = !this.saveBlocks;
				if (flag)
				{
					MelonLogger.Msg(ConsoleColor.Yellow, "Saving blocks has been disabled.");
				}
			}, this.config.SaveBlocks);
			category.AddToggle("Save Mutes", "Enable to save mute actions to file.", delegate(bool s)
			{
				this.saveMutes = s;
				this.config.SaveMutes = s;
				this.SaveConfig();
				bool flag2 = !this.saveMutes;
				if (flag2)
				{
					MelonLogger.Msg(ConsoleColor.Yellow, "Saving mutes has been disabled.");
				}
			}, this.config.SaveMutes);
			category.AddToggle("Enable Toasts", "Enable or disable toast notifications.", delegate(bool s)
			{
				this.enableToasts = s;
				this.config.EnableToasts = s;
				this.SaveConfig();
				bool flag3 = !this.enableToasts;
				if (flag3)
				{
					MelonLogger.Msg(ConsoleColor.Yellow, "Toast notifications have been disabled.");
				}
			}, this.config.EnableToasts);
		}

		// Token: 0x06000442 RID: 1090 RVA: 0x0000370D File Offset: 0x0000190D
		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			this.LoadModerationData();
		}

		// Token: 0x06000443 RID: 1091 RVA: 0x000181DC File Offset: 0x000163DC
		public override bool OnEventPatch(EventData eventData)
		{
			bool flag18;
			try
			{
				bool flag = this.antiblock && eventData.Code == 33;
				if (flag)
				{
					try
					{
						Dictionary<byte, Object> dictionary = eventData.Parameters[0].Cast<Dictionary<byte, Object>>();
						bool flag2 = dictionary[0].Unbox<byte>() == 21 && dictionary.ContainsKey(1);
						if (flag2)
						{
							bool flag3 = dictionary[10].Unbox<bool>();
							bool flag4 = dictionary[11].Unbox<bool>();
							Player playerNewtworkedId = dictionary[1].Unbox<int>().GetPlayerNewtworkedId();
							bool flag5 = playerNewtworkedId != null;
							if (flag5)
							{
								string displayName = playerNewtworkedId.Method_Internal_get_APIUser_0().displayName;
								string userId = playerNewtworkedId.Method_Internal_get_APIUser_0().id;
								bool flag6 = flag3;
								if (flag6)
								{
									ModerationNotice.ModerationEntry moderationEntry = new ModerationNotice.ModerationEntry
									{
										Username = displayName,
										Id = userId
									};
									bool flag7 = !this.knownBlocks.Any((ModerationNotice.ModerationEntry e) => e.Id == userId);
									if (flag7)
									{
										this.knownBlocks.Add(moderationEntry);
										bool flag8 = this.saveBlocks;
										if (flag8)
										{
											this.SaveModerationData(this.blocksFilePath, this.knownBlocks);
										}
									}
									OtherUtil.processStrings.Add("<color=red>" + displayName + " has blocked you.</color>");
									MelonLogger.Msg(ConsoleColor.Red, displayName + " has blocked you.");
									this.ShowToast("Blocked", displayName + " has blocked you.", Color.red);
									PlayerUtil.knownBlocks.Add(displayName);
									return false;
								}
								bool flag9 = flag4;
								if (flag9)
								{
									ModerationNotice.ModerationEntry moderationEntry2 = new ModerationNotice.ModerationEntry
									{
										Username = displayName,
										Id = userId
									};
									bool flag10 = !this.knownMutes.Any((ModerationNotice.ModerationEntry e) => e.Id == userId);
									if (flag10)
									{
										this.knownMutes.Add(moderationEntry2);
										bool flag11 = this.saveMutes;
										if (flag11)
										{
											this.SaveModerationData(this.mutesFilePath, this.knownMutes);
										}
									}
									OtherUtil.processStrings.Add("<color=red>" + displayName + " has muted you.</color>");
									MelonLogger.Msg(ConsoleColor.Red, displayName + " has muted you.");
									this.ShowToast("Muted", displayName + " has muted you.", Color.red);
									PlayerUtil.knownMutes.Add(displayName);
									return false;
								}
								bool flag12 = PlayerUtil.knownMutes.Contains(displayName) && displayName != PlayerUtil.GetLocalVRCPlayer().Method_Public_get_VRCPlayerApi_0().displayName;
								if (flag12)
								{
									ModerationNotice.ModerationEntry moderationEntry3 = this.knownMutes.FirstOrDefault((ModerationNotice.ModerationEntry e) => e.Id == userId);
									bool flag13 = moderationEntry3 != null;
									if (flag13)
									{
										this.knownMutes.Remove(moderationEntry3);
										bool flag14 = this.saveMutes;
										if (flag14)
										{
											this.SaveModerationData(this.mutesFilePath, this.knownMutes);
										}
									}
									this.ShowToast("Unmuted", displayName + " has unmuted you.", Color.green);
									MelonLogger.Msg(ConsoleColor.Green, displayName + " has unmuted you.");
								}
								bool flag15 = PlayerUtil.knownBlocks.Contains(displayName) && displayName != PlayerUtil.GetLocalVRCPlayer().Method_Public_get_VRCPlayerApi_0().displayName;
								if (flag15)
								{
									ModerationNotice.ModerationEntry moderationEntry4 = this.knownBlocks.FirstOrDefault((ModerationNotice.ModerationEntry e) => e.Id == userId);
									bool flag16 = moderationEntry4 != null;
									if (flag16)
									{
										this.knownBlocks.Remove(moderationEntry4);
										bool flag17 = this.saveBlocks;
										if (flag17)
										{
											this.SaveModerationData(this.blocksFilePath, this.knownBlocks);
										}
									}
									this.ShowToast("Unblocked", displayName + " has unblocked you.", Color.green);
									MelonLogger.Msg(ConsoleColor.Green, displayName + " has unblocked you.");
								}
							}
						}
					}
					catch (Exception ex)
					{
						MelonLogger.Error(string.Format("[ModerationNotice] OnEventPatch Error: {0}\n{1}", ex, ex.StackTrace));
					}
				}
				flag18 = true;
			}
			catch
			{
				flag18 = true;
			}
			return flag18;
		}

		// Token: 0x06000444 RID: 1092 RVA: 0x00018638 File Offset: 0x00016838
		public override void OnPlayerJoined(Player player)
		{
			bool flag = player == null;
			if (flag)
			{
				MelonLogger.Error("[ModerationNotice] OnPlayerJoined received a null player.");
			}
			else
			{
				try
				{
					MelonLogger.Msg("[ModerationNotice] Player joined: " + this.GetPlayerName(player));
				}
				catch (Exception ex)
				{
					MelonLogger.Error(string.Format("[ModerationNotice] OnPlayerJoined Error: {0}\n{1}\nPlayerInfo: {2}", ex, ex.StackTrace, this.GetPlayerDebugInfo(player)));
				}
			}
		}

		// Token: 0x06000445 RID: 1093 RVA: 0x000186B0 File Offset: 0x000168B0
		private void SaveModerationData(string filePath, List<ModerationNotice.ModerationEntry> data)
		{
			try
			{
				ModerationNotice.ModerationListWrapper moderationListWrapper = new ModerationNotice.ModerationListWrapper
				{
					Entries = data
				};
				string text = JsonUtility.ToJson<ModerationNotice.ModerationListWrapper>(moderationListWrapper, true);
				File.WriteAllText(filePath, text);
				MelonLogger.Msg("[ModerationNotice] Moderation data saved to " + filePath);
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[ModerationNotice] Failed to save moderation data: {0}", ex));
			}
		}

		// Token: 0x06000446 RID: 1094 RVA: 0x00018718 File Offset: 0x00016918
		private void LoadModerationData()
		{
			try
			{
				bool flag = File.Exists(this.blocksFilePath);
				if (flag)
				{
					string text = File.ReadAllText(this.blocksFilePath);
					ModerationNotice.ModerationListWrapper moderationListWrapper = JsonUtility.FromJson<ModerationNotice.ModerationListWrapper>(text);
					this.knownBlocks = moderationListWrapper.Entries ?? new List<ModerationNotice.ModerationEntry>();
					PlayerUtil.knownBlocks = this.knownBlocks.Select((ModerationNotice.ModerationEntry e) => e.Username).ToList<string>();
					MelonLogger.Msg(string.Format("[ModerationNotice] Loaded {0} blocks from {1}", this.knownBlocks.Count, this.blocksFilePath));
				}
				bool flag2 = File.Exists(this.mutesFilePath);
				if (flag2)
				{
					string text2 = File.ReadAllText(this.mutesFilePath);
					ModerationNotice.ModerationListWrapper moderationListWrapper2 = JsonUtility.FromJson<ModerationNotice.ModerationListWrapper>(text2);
					this.knownMutes = moderationListWrapper2.Entries ?? new List<ModerationNotice.ModerationEntry>();
					PlayerUtil.knownMutes = this.knownMutes.Select((ModerationNotice.ModerationEntry e) => e.Username).ToList<string>();
					MelonLogger.Msg(string.Format("[ModerationNotice] Loaded {0} mutes from {1}", this.knownMutes.Count, this.mutesFilePath));
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[ModerationNotice] Failed to load moderation data: {0}", ex));
			}
		}

		// Token: 0x06000447 RID: 1095 RVA: 0x00018890 File Offset: 0x00016A90
		private void LoadConfig()
		{
			try
			{
				bool flag = File.Exists(this.configFilePath);
				if (flag)
				{
					string text = File.ReadAllText(this.configFilePath);
					ModerationNotice.ModerationConfigWrapper moderationConfigWrapper = JsonUtility.FromJson<ModerationNotice.ModerationConfigWrapper>(text);
					bool flag2 = moderationConfigWrapper != null && moderationConfigWrapper.Config != null;
					if (flag2)
					{
						this.config = moderationConfigWrapper.Config;
						this.antiblock = this.config.Antiblock;
						this.saveBlocks = this.config.SaveBlocks;
						this.saveMutes = this.config.SaveMutes;
						this.enableToasts = this.config.EnableToasts;
						this.toastDuration = this.config.ToastDuration;
						MelonLogger.Msg("[ModerationNotice] Loaded moderation config from " + this.configFilePath);
					}
					else
					{
						this.config = new ModerationNotice.ModerationConfig();
						this.SaveConfig();
					}
				}
				else
				{
					this.config = new ModerationNotice.ModerationConfig();
					this.SaveConfig();
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[ModerationNotice] Failed to load moderation config: {0}", ex));
				this.config = new ModerationNotice.ModerationConfig();
			}
		}

		// Token: 0x06000448 RID: 1096 RVA: 0x000189B4 File Offset: 0x00016BB4
		private void SaveConfig()
		{
			try
			{
				ModerationNotice.ModerationConfigWrapper moderationConfigWrapper = new ModerationNotice.ModerationConfigWrapper
				{
					Config = this.config
				};
				string text = JsonUtility.ToJson<ModerationNotice.ModerationConfigWrapper>(moderationConfigWrapper, true);
				File.WriteAllText(this.configFilePath, text);
				MelonLogger.Msg("[ModerationNotice] Moderation config saved to " + this.configFilePath);
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[ModerationNotice] Failed to save moderation config: {0}", ex));
			}
		}

		// Token: 0x06000449 RID: 1097 RVA: 0x00018A2C File Offset: 0x00016C2C
		private void ShowToast(string title, string message, Color color)
		{
			bool flag = !this.enableToasts;
			if (!flag)
			{
				try
				{
					ToastNotif.Toast(title, message, null, this.toastDuration);
				}
				catch (Exception ex)
				{
					MelonLogger.Error(string.Format("[ModerationNotice] ShowToast Error: {0}\n{1}\nTitle: {2}, Message: {3}", new object[] { ex, ex.StackTrace, title, message }));
				}
			}
		}

		// Token: 0x0600044A RID: 1098 RVA: 0x00018AA0 File Offset: 0x00016CA0
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

		// Token: 0x0600044B RID: 1099 RVA: 0x00018AD4 File Offset: 0x00016CD4
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

		// Token: 0x040002D7 RID: 727
		private bool antiblock = false;

		// Token: 0x040002D8 RID: 728
		private bool saveBlocks = false;

		// Token: 0x040002D9 RID: 729
		private bool saveMutes = false;

		// Token: 0x040002DA RID: 730
		private bool enableToasts = true;

		// Token: 0x040002DB RID: 731
		private float toastDuration = 5f;

		// Token: 0x040002DC RID: 732
		private readonly string blocksFilePath = Path.Combine(MelonUtils.UserDataDirectory, "blocks.json");

		// Token: 0x040002DD RID: 733
		private readonly string mutesFilePath = Path.Combine(MelonUtils.UserDataDirectory, "mutes.json");

		// Token: 0x040002DE RID: 734
		private readonly string configFilePath = Path.Combine(MelonUtils.UserDataDirectory, "LocalModeration.json");

		// Token: 0x040002DF RID: 735
		public List<ModerationNotice.ModerationEntry> knownBlocks = new List<ModerationNotice.ModerationEntry>();

		// Token: 0x040002E0 RID: 736
		public List<ModerationNotice.ModerationEntry> knownMutes = new List<ModerationNotice.ModerationEntry>();

		// Token: 0x040002E1 RID: 737
		private ModerationNotice.ModerationConfig config;

		// Token: 0x040002E2 RID: 738
		private Color _warningColor = Color.yellow;

		// Token: 0x040002E3 RID: 739
		private Color _criticalColor = Color.red;

		// Token: 0x0200009F RID: 159
		[Serializable]
		public class ModerationEntry
		{
			// Token: 0x040002E4 RID: 740
			public string Username;

			// Token: 0x040002E5 RID: 741
			public string Id;
		}

		// Token: 0x020000A0 RID: 160
		[Serializable]
		public class ModerationListWrapper
		{
			// Token: 0x040002E6 RID: 742
			public List<ModerationNotice.ModerationEntry> Entries = new List<ModerationNotice.ModerationEntry>();
		}

		// Token: 0x020000A1 RID: 161
		[Serializable]
		public class ModerationConfig
		{
			// Token: 0x040002E7 RID: 743
			public bool Antiblock = true;

			// Token: 0x040002E8 RID: 744
			public bool SaveBlocks = true;

			// Token: 0x040002E9 RID: 745
			public bool SaveMutes = true;

			// Token: 0x040002EA RID: 746
			public bool EnableToasts = true;

			// Token: 0x040002EB RID: 747
			public float ToastDuration = 5f;
		}

		// Token: 0x020000A2 RID: 162
		[Serializable]
		public class ModerationConfigWrapper
		{
			// Token: 0x040002EC RID: 748
			public ModerationNotice.ModerationConfig Config = new ModerationNotice.ModerationConfig();
		}
	}
}
