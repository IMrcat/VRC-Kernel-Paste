using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using Il2CppSystem.Collections.Generic;
using KernelClient.Utils;
using KernelClient.Wrapper;
using MelonLoader;
using ReMod.Core.UI.MainMenu;
using ReMod.Core.UI.QuickMenu;
using ReMod.Core.VRChat;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.SDKBase;

namespace KernelClient.Modules
{
	// Token: 0x0200007D RID: 125
	internal class AvatarSaver : KernelModule
	{
		// Token: 0x06000353 RID: 851
		[DllImport("user32.dll")]
		private static extern bool OpenClipboard(IntPtr hWndNewOwner);

		// Token: 0x06000354 RID: 852
		[DllImport("user32.dll")]
		private static extern bool EmptyClipboard();

		// Token: 0x06000355 RID: 853
		[DllImport("user32.dll")]
		private static extern IntPtr SetClipboardData(uint uFormat, IntPtr data);

		// Token: 0x06000356 RID: 854
		[DllImport("user32.dll")]
		private static extern bool CloseClipboard();

		// Token: 0x06000357 RID: 855
		[DllImport("kernel32.dll")]
		private static extern IntPtr GlobalAlloc(uint flags, int size);

		// Token: 0x06000358 RID: 856
		[DllImport("kernel32.dll")]
		private static extern IntPtr GlobalLock(IntPtr hMem);

		// Token: 0x06000359 RID: 857
		[DllImport("kernel32.dll")]
		private static extern bool GlobalUnlock(IntPtr hMem);

		// Token: 0x0600035A RID: 858
		[DllImport("kernel32.dll")]
		private static extern IntPtr GlobalFree(IntPtr hMem);

		// Token: 0x0600035B RID: 859 RVA: 0x00011FE8 File Offset: 0x000101E8
		public override void OnUiManagerInit()
		{
			try
			{
				ReMenuCategory category = MenuSetup._uiManager.QMMenu.GetCategoryPage(PageNames.Utility).GetCategory(CatagoryNames.Avatar);
				this.menuPage = category.AddMenuPage("Avatar Saver", "Save and manage avatars", null, "#ffffff");
				this.menuPage.AddButton("Save Current Avatar", "Save your current avatar", new Action(this.SaveCurrentAvatar), null, "#ffffff");
				this.menuPage.AddButton("Save All Room Avatars", "Save all visible avatars", new Action(this.ScanRoom), null, "#ffffff");
				this.menuPage.AddButton("View Saved Avatars", "Show saved avatars", new Action(this.BuildAndOpenAvatarList), null, "#ffffff");
				this.menuPage.AddToggle("Auto Save On Join", "Save avatars when players join", new Action<bool>(this.ToggleAutoSave));
				Directory.CreateDirectory(this.VRCA_PATH);
				ReMMUserButton reMMUserButton = new ReMMUserButton("Teleport to them", "Teleport to them", delegate
				{
				}, null, MMenuPrefabs.MMUserDetailButton.transform.parent);
				this.LoadAvatars();
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[AvatarSaver] Initialization error: {0}", ex));
			}
		}

		// Token: 0x0600035C RID: 860 RVA: 0x00012154 File Offset: 0x00010354
		private void BuildAndOpenAvatarList()
		{
			try
			{
				ReMenuPage reMenuPage = this.menuPage.AddMenuPage("Saved Avatars", "List of saved avatars", null, "#ffffff");
				using (List<AvatarSaver.SavedAvatar>.Enumerator enumerator = this.savedAvatars.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						AvatarSaver.SavedAvatar avatar = enumerator.Current;
						reMenuPage.AddMenuPage(avatar.AvatarName, "By: " + avatar.AuthorName + "\nSaved: " + avatar.SaveDate, delegate(ReMenuPage page)
						{
							this.CreateAvatarButtons(page, avatar);
						}, null, "#ffffff");
					}
				}
				reMenuPage.Open();
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[AvatarSaver] Menu build error: {0}", ex));
			}
		}

		// Token: 0x0600035D RID: 861 RVA: 0x0001224C File Offset: 0x0001044C
		private void CreateAvatarButtons(ReMenuPage page, AvatarSaver.SavedAvatar avatar)
		{
			page.AddButton("Switch To Avatar", "Use this avatar", delegate
			{
				this.SwitchToAvatar(avatar);
			}, null, "#ffffff");
			page.AddButton("Save VRCA", "Download avatar file", delegate
			{
				this.DownloadVRCA(avatar);
			}, null, "#ffffff");
			page.AddButton("Copy Avatar ID", "Copy to clipboard", delegate
			{
				this.CopyToClipboard(avatar.AvatarId);
			}, null, "#ffffff");
			page.AddButton("Delete Avatar", "Remove from list", delegate
			{
				this.DeleteAvatar(avatar, page);
			}, null, "#ffffff");
		}

		// Token: 0x0600035E RID: 862 RVA: 0x00012318 File Offset: 0x00010518
		public override void OnPlayerJoined(Player player)
		{
			bool flag = !this.autoSave || player == null;
			if (!flag)
			{
				try
				{
					ApiAvatar apiAvatar = ((player != null) ? player.Method_Public_get_ApiAvatar_PDM_0() : null);
					bool flag2 = apiAvatar != null;
					if (flag2)
					{
						this.SaveAvatarInfo(apiAvatar, true);
					}
				}
				catch (Exception ex)
				{
					MelonLogger.Error(string.Format("[AvatarSaver] Player join error: {0}", ex));
				}
			}
		}

		// Token: 0x0600035F RID: 863 RVA: 0x0001238C File Offset: 0x0001058C
		private void SaveCurrentAvatar()
		{
			try
			{
				VRCPlayer localVRCPlayer = PlayerUtil.GetLocalVRCPlayer();
				VRCAvatarManager vrcavatarManager = ((localVRCPlayer != null) ? localVRCPlayer.Method_Public_get_VRCAvatarManager_0() : null);
				ApiAvatar apiAvatar = ((vrcavatarManager != null) ? vrcavatarManager.field_Private_ApiAvatar_0 : null);
				bool flag = apiAvatar == null;
				if (flag)
				{
					ToastNotif.Toast("Error", "No avatar found", null, 5f);
				}
				else
				{
					this.SaveAvatarInfo(apiAvatar, true);
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[AvatarSaver] Save current avatar error: {0}", ex));
			}
		}

		// Token: 0x06000360 RID: 864 RVA: 0x00012410 File Offset: 0x00010610
		private void ScanRoom()
		{
			try
			{
				int num = 0;
				PlayerManager playerManager = PlayerManager.Method_Public_Static_get_PlayerManager_0();
				List<Player> list = ((playerManager != null) ? playerManager.field_Private_List_1_Player_0 : null);
				bool flag = list == null;
				if (!flag)
				{
					foreach (Player player in list)
					{
						bool flag2 = ((player != null) ? player.Method_Public_get_ApiAvatar_PDM_0() : null) != null;
						if (flag2)
						{
							bool flag3 = this.SaveAvatarInfo(player.Method_Public_get_ApiAvatar_PDM_0(), false);
							if (flag3)
							{
								num++;
							}
						}
					}
					ToastNotif.Toast("Room Scan", string.Format("Saved {0} new avatars", num), null, 5f);
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[AvatarSaver] Room scan error: {0}", ex));
			}
		}

		// Token: 0x06000361 RID: 865 RVA: 0x000124DC File Offset: 0x000106DC
		private bool SaveAvatarInfo(ApiAvatar avatar, bool notify)
		{
			bool flag2;
			try
			{
				AvatarSaver.SavedAvatar savedAvatar = this.savedAvatars.FirstOrDefault((AvatarSaver.SavedAvatar a) => a.AvatarId == avatar.id);
				bool flag = savedAvatar != null;
				if (flag)
				{
					savedAvatar.UpdateFromApiAvatar(avatar);
					this.SaveToFile();
					if (notify)
					{
						ToastNotif.Toast("Updated", "Updated " + avatar.name, null, 5f);
					}
					flag2 = false;
				}
				else
				{
					AvatarSaver.SavedAvatar savedAvatar2 = new AvatarSaver.SavedAvatar
					{
						AvatarId = avatar.id,
						AvatarName = avatar.name,
						AuthorName = avatar.authorName,
						SaveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
						AssetUrl = avatar.assetUrl,
						ThumbnailUrl = avatar.thumbnailImageUrl,
						IsPublic = (avatar.releaseStatus == "public"),
						HasVRCA = false
					};
					this.savedAvatars.Add(savedAvatar2);
					this.SaveToFile();
					if (notify)
					{
						ToastNotif.Toast("Saved", "Added " + avatar.name, null, 5f);
					}
					flag2 = true;
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[AvatarSaver] Save avatar error: {0}", ex));
				flag2 = false;
			}
			return flag2;
		}

		// Token: 0x06000362 RID: 866 RVA: 0x00012680 File Offset: 0x00010880
		private void SwitchToAvatar(AvatarSaver.SavedAvatar avatar)
		{
			try
			{
				ApiAvatar apiAvatar = new ApiAvatar
				{
					id = avatar.AvatarId
				};
				apiAvatar.Get(delegate(ApiContainer container)
				{
					try
					{
						ApiAvatar apiAvatar2 = container.Model as ApiAvatar;
						bool flag = apiAvatar2 == null;
						if (flag)
						{
							MelonLogger.Error("[AvatarSaver] Downloaded avatar is null.");
							ToastNotif.Toast("Error", "Failed to download avatar.", null, 5f);
						}
						else
						{
							VRCPlayer localVRCPlayer = PlayerUtil.GetLocalVRCPlayer();
							VRCAvatarManager vrcavatarManager = ((localVRCPlayer != null) ? localVRCPlayer.Method_Public_get_VRCAvatarManager_0() : null);
							bool flag2 = vrcavatarManager == null;
							if (flag2)
							{
								MelonLogger.Error("[AvatarSaver] Avatar Manager is null.");
								ToastNotif.Toast("Error", "Avatar Manager not found.", null, 5f);
							}
							else
							{
								MethodInfo method = typeof(VRCAvatarManager).GetMethod("SetAvatar", BindingFlags.Instance | BindingFlags.Public);
								bool flag3 = method != null;
								if (flag3)
								{
									method.Invoke(vrcavatarManager, new object[] { apiAvatar2 });
									VRCPlayerApi localPlayer = Networking.LocalPlayer;
									bool flag4 = localPlayer != null && localPlayer.IsValid();
									if (flag4)
									{
										this.SetAvatarScale(localPlayer, 1f);
									}
									ToastNotif.Toast("Switched", "Changed to " + avatar.AvatarName, null, 5f);
								}
								else
								{
									MelonLogger.Error("[AvatarSaver] SetAvatar method not found in VRCAvatarManager.");
									ToastNotif.Toast("Error", "Unable to switch avatar.", null, 5f);
								}
							}
						}
					}
					catch (Exception ex2)
					{
						MelonLogger.Error(string.Format("[AvatarSaver] Switch avatar inner error: {0}", ex2));
						ToastNotif.Toast("Error", "Failed to switch avatar.", null, 5f);
					}
				}, null, null, false, 0);
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[AvatarSaver] Switch avatar error: {0}", ex));
				ToastNotif.Toast("Error", "Failed to switch avatar.", null, 5f);
			}
		}

		// Token: 0x06000363 RID: 867 RVA: 0x00012718 File Offset: 0x00010918
		private void DownloadVRCA(AvatarSaver.SavedAvatar avatar)
		{
			try
			{
				bool flag = string.IsNullOrEmpty(avatar.AssetUrl);
				if (flag)
				{
					ToastNotif.Toast("Error", "No asset URL", null, 5f);
				}
				else
				{
					string text = Path.Combine(this.VRCA_PATH, avatar.AvatarId + ".vrca");
					bool flag2 = File.Exists(text);
					if (flag2)
					{
						ToastNotif.Toast("Exists", "VRCA already saved", null, 5f);
					}
					else
					{
						APIUser currentUser = APIUser.CurrentUser;
						string text2 = ((currentUser != null) ? currentUser.authToken : null);
						bool flag3 = string.IsNullOrEmpty(text2);
						if (flag3)
						{
							ToastNotif.Toast("Error", "Not authenticated", null, 5f);
						}
						else
						{
							using (WebClient webClient = new WebClient())
							{
								webClient.Headers.Add(HttpRequestHeader.Cookie, "auth=" + text2);
								webClient.Headers.Add(HttpRequestHeader.UserAgent, "VRChat/1.0");
								webClient.Headers.Add("Accept", "application/json");
								ToastNotif.Toast("Downloading", avatar.AvatarName, null, 5f);
								webClient.DownloadFileCompleted += delegate(object s, AsyncCompletedEventArgs e)
								{
									bool flag4 = e.Error == null;
									if (flag4)
									{
										avatar.HasVRCA = true;
										this.SaveToFile();
										ToastNotif.Toast("Complete", "Saved " + avatar.AvatarName, null, 5f);
									}
									else
									{
										MelonLogger.Error(string.Format("[AvatarSaver] Download error: {0}", e.Error));
										ToastNotif.Toast("Error", "Download failed", null, 5f);
									}
								};
								webClient.DownloadFileAsync(new Uri(avatar.AssetUrl), text);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[AvatarSaver] Download error: {0}", ex));
				ToastNotif.Toast("Error", "Download failed", null, 5f);
			}
		}

		// Token: 0x06000364 RID: 868 RVA: 0x000128FC File Offset: 0x00010AFC
		private void DeleteAvatar(AvatarSaver.SavedAvatar avatar, ReMenuPage page)
		{
			try
			{
				this.savedAvatars.Remove(avatar);
				this.SaveToFile();
				string text = Path.Combine(this.VRCA_PATH, avatar.AvatarId + ".vrca");
				bool flag = File.Exists(text);
				if (flag)
				{
					File.Delete(text);
				}
				MethodInfo method = typeof(ReMenuPage).GetMethod("ClosePage", BindingFlags.Instance | BindingFlags.Public);
				bool flag2 = method != null;
				if (flag2)
				{
					method.Invoke(page, null);
				}
				else
				{
					MelonLogger.Warning("[AvatarSaver] ClosePage method not found in ReMenuPage. Attempting to destroy the page.");
				}
				this.BuildAndOpenAvatarList();
				ToastNotif.Toast("Deleted", avatar.AvatarName, null, 5f);
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[AvatarSaver] Delete error: {0}", ex));
				ToastNotif.Toast("Error", "Failed to delete avatar.", null, 5f);
			}
		}

		// Token: 0x06000365 RID: 869 RVA: 0x000129E8 File Offset: 0x00010BE8
		private void LoadAvatars()
		{
			try
			{
				this.savedAvatars.Clear();
				bool flag = !File.Exists(this.SAVE_PATH);
				if (!flag)
				{
					foreach (string text in File.ReadAllLines(this.SAVE_PATH))
					{
						try
						{
							bool flag2 = !string.IsNullOrEmpty(text);
							if (flag2)
							{
								this.savedAvatars.Add(AvatarSaver.SavedAvatar.FromString(text));
							}
						}
						catch (FormatException ex)
						{
							MelonLogger.Warning("[AvatarSaver] Skipping invalid line: " + text + ". Error: " + ex.Message);
						}
					}
				}
			}
			catch (Exception ex2)
			{
				MelonLogger.Error(string.Format("[AvatarSaver] Load avatars error: {0}", ex2));
			}
		}

		// Token: 0x06000366 RID: 870 RVA: 0x00012ABC File Offset: 0x00010CBC
		private void SaveToFile()
		{
			try
			{
				Directory.CreateDirectory(Path.GetDirectoryName(this.SAVE_PATH));
				File.WriteAllLines(this.SAVE_PATH, this.savedAvatars.Select((AvatarSaver.SavedAvatar a) => a.ToString()));
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[AvatarSaver] Save to file error: {0}", ex));
			}
		}

		// Token: 0x06000367 RID: 871 RVA: 0x000031F4 File Offset: 0x000013F4
		private void ToggleAutoSave(bool state)
		{
			this.autoSave = state;
			ToastNotif.Toast(this.autoSave ? "Auto Save On" : "Auto Save Off", null, null, 5f);
		}

		// Token: 0x06000368 RID: 872 RVA: 0x00012B3C File Offset: 0x00010D3C
		private void CopyToClipboard(string text)
		{
			bool flag = string.IsNullOrEmpty(text);
			if (!flag)
			{
				try
				{
					bool flag2 = !AvatarSaver.OpenClipboard(IntPtr.Zero);
					if (flag2)
					{
						MelonLogger.Warning("[AvatarSaver] Unable to open clipboard.");
						ToastNotif.Toast("Error", "Failed to access clipboard.", null, 5f);
					}
					else
					{
						AvatarSaver.EmptyClipboard();
						int num = (text.Length + 1) * 2;
						IntPtr intPtr = AvatarSaver.GlobalAlloc(2U, num);
						bool flag3 = intPtr == IntPtr.Zero;
						if (flag3)
						{
							MelonLogger.Warning("[AvatarSaver] GlobalAlloc failed.");
							ToastNotif.Toast("Error", "Failed to allocate clipboard memory.", null, 5f);
						}
						else
						{
							try
							{
								IntPtr intPtr2 = AvatarSaver.GlobalLock(intPtr);
								bool flag4 = intPtr2 == IntPtr.Zero;
								if (flag4)
								{
									MelonLogger.Warning("[AvatarSaver] GlobalLock failed.");
									ToastNotif.Toast("Error", "Failed to lock clipboard memory.", null, 5f);
								}
								else
								{
									try
									{
										Marshal.Copy(text.ToCharArray(), 0, intPtr2, text.Length);
										Marshal.WriteInt16(intPtr2, text.Length * 2, 0);
										bool flag5 = AvatarSaver.SetClipboardData(13U, intPtr) == IntPtr.Zero;
										if (flag5)
										{
											MelonLogger.Warning("[AvatarSaver] SetClipboardData failed.");
											ToastNotif.Toast("Error", "Failed to set clipboard data.", null, 5f);
										}
										else
										{
											ToastNotif.Toast("Copied", "Avatar ID copied to clipboard.", null, 5f);
										}
									}
									finally
									{
										AvatarSaver.GlobalUnlock(intPtr2);
									}
								}
							}
							finally
							{
								AvatarSaver.GlobalFree(intPtr);
							}
						}
					}
				}
				catch (Exception ex)
				{
					MelonLogger.Error(string.Format("[AvatarSaver] Clipboard error: {0}", ex));
					ToastNotif.Toast("Error", "Failed to copy to clipboard.", null, 5f);
				}
				finally
				{
					AvatarSaver.CloseClipboard();
				}
			}
		}

		// Token: 0x06000369 RID: 873 RVA: 0x00012D5C File Offset: 0x00010F5C
		private void SetAvatarScale(VRCPlayerApi player, float scale)
		{
			try
			{
				GameObject gameObject = player.gameObject;
				bool flag = gameObject != null;
				if (flag)
				{
					gameObject.transform.localScale = new Vector3(scale, scale, scale);
					MelonLogger.Msg(string.Format("[AvatarSaver] Set avatar scale to {0}", scale));
				}
				else
				{
					MelonLogger.Warning("[AvatarSaver] Player GameObject is null.");
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("[AvatarSaver] SetAvatarScale error: {0}", ex));
			}
		}

		// Token: 0x04000220 RID: 544
		private readonly string SAVE_PATH = Path.Combine(Environment.CurrentDirectory, "UserData", "SavedAvatars.txt");

		// Token: 0x04000221 RID: 545
		private readonly string VRCA_PATH = Path.Combine(Environment.CurrentDirectory, "UserData", "SavedVRCAs");

		// Token: 0x04000222 RID: 546
		private List<AvatarSaver.SavedAvatar> savedAvatars = new List<AvatarSaver.SavedAvatar>();

		// Token: 0x04000223 RID: 547
		private ReMenuPage menuPage;

		// Token: 0x04000224 RID: 548
		private bool autoSave;

		// Token: 0x04000225 RID: 549
		private const uint GMEM_MOVEABLE = 2U;

		// Token: 0x04000226 RID: 550
		private const uint CF_UNICODETEXT = 13U;

		// Token: 0x0200007E RID: 126
		private class SavedAvatar
		{
			// Token: 0x17000056 RID: 86
			// (get) Token: 0x0600036B RID: 875 RVA: 0x0000321F File Offset: 0x0000141F
			// (set) Token: 0x0600036C RID: 876 RVA: 0x00003227 File Offset: 0x00001427
			public string AvatarId { get; set; }

			// Token: 0x17000057 RID: 87
			// (get) Token: 0x0600036D RID: 877 RVA: 0x00003230 File Offset: 0x00001430
			// (set) Token: 0x0600036E RID: 878 RVA: 0x00003238 File Offset: 0x00001438
			public string AvatarName { get; set; }

			// Token: 0x17000058 RID: 88
			// (get) Token: 0x0600036F RID: 879 RVA: 0x00003241 File Offset: 0x00001441
			// (set) Token: 0x06000370 RID: 880 RVA: 0x00003249 File Offset: 0x00001449
			public string AuthorName { get; set; }

			// Token: 0x17000059 RID: 89
			// (get) Token: 0x06000371 RID: 881 RVA: 0x00003252 File Offset: 0x00001452
			// (set) Token: 0x06000372 RID: 882 RVA: 0x0000325A File Offset: 0x0000145A
			public string SaveDate { get; set; }

			// Token: 0x1700005A RID: 90
			// (get) Token: 0x06000373 RID: 883 RVA: 0x00003263 File Offset: 0x00001463
			// (set) Token: 0x06000374 RID: 884 RVA: 0x0000326B File Offset: 0x0000146B
			public string AssetUrl { get; set; }

			// Token: 0x1700005B RID: 91
			// (get) Token: 0x06000375 RID: 885 RVA: 0x00003274 File Offset: 0x00001474
			// (set) Token: 0x06000376 RID: 886 RVA: 0x0000327C File Offset: 0x0000147C
			public string ThumbnailUrl { get; set; }

			// Token: 0x1700005C RID: 92
			// (get) Token: 0x06000377 RID: 887 RVA: 0x00003285 File Offset: 0x00001485
			// (set) Token: 0x06000378 RID: 888 RVA: 0x0000328D File Offset: 0x0000148D
			public bool IsPublic { get; set; }

			// Token: 0x1700005D RID: 93
			// (get) Token: 0x06000379 RID: 889 RVA: 0x00003296 File Offset: 0x00001496
			// (set) Token: 0x0600037A RID: 890 RVA: 0x0000329E File Offset: 0x0000149E
			public bool HasVRCA { get; set; }

			// Token: 0x0600037B RID: 891 RVA: 0x00012E38 File Offset: 0x00011038
			public override string ToString()
			{
				return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}", new object[] { this.AvatarId, this.AvatarName, this.AuthorName, this.SaveDate, this.AssetUrl, this.ThumbnailUrl, this.IsPublic, this.HasVRCA });
			}

			// Token: 0x0600037C RID: 892 RVA: 0x00012EAC File Offset: 0x000110AC
			public static AvatarSaver.SavedAvatar FromString(string line)
			{
				string[] array = line.Split(new char[] { '|' });
				bool flag = array.Length < 8;
				if (flag)
				{
					throw new FormatException("Invalid saved avatar format.");
				}
				return new AvatarSaver.SavedAvatar
				{
					AvatarId = array[0],
					AvatarName = array[1],
					AuthorName = array[2],
					SaveDate = array[3],
					AssetUrl = array[4],
					ThumbnailUrl = array[5],
					IsPublic = bool.Parse(array[6]),
					HasVRCA = bool.Parse(array[7])
				};
			}

			// Token: 0x0600037D RID: 893 RVA: 0x00012F44 File Offset: 0x00011144
			public void UpdateFromApiAvatar(ApiAvatar avatar)
			{
				this.AvatarName = avatar.name;
				this.AuthorName = avatar.authorName;
				this.SaveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				this.AssetUrl = avatar.assetUrl;
				this.ThumbnailUrl = avatar.thumbnailImageUrl;
				this.IsPublic = avatar.releaseStatus == "public";
			}
		}
	}
}
