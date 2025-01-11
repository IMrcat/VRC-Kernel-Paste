using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using KernelClient.Utils;
using KernelClient.Wrapper;
using MelonLoader;
using ReMod.Core.UI.QuickMenu;
using VRC;

// Token: 0x02000012 RID: 18
internal class ModerationNotice : KernelModule
{
	// Token: 0x06000057 RID: 87 RVA: 0x00006104 File Offset: 0x00004304
	public override void OnUiManagerInit()
	{
		try
		{
			ReMenuCategory category = MenuSetup._uiManager.QMMenu.GetCategoryPage(PageNames.Security).GetCategory(CatagoryNames.Alerts);
			MelonLogger.Msg("Moderation Notice UI initialized successfully.");
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error initializing ModerationNotice UI: " + ex.Message);
		}
	}

	// Token: 0x06000058 RID: 88 RVA: 0x0000222D File Offset: 0x0000042D
	public override void OnSceneWasLoaded(int buildIndex, string sceneName)
	{
		PlayerUtil.knownBlocks.Clear();
		PlayerUtil.knownMutes.Clear();
		MelonLogger.Msg(string.Format("Scene loaded: {0} (Build Index: {1}). Known blocks and mutes cleared.", sceneName, buildIndex));
	}

	// Token: 0x06000059 RID: 89 RVA: 0x0000616C File Offset: 0x0000436C
	public override bool OnEventPatch(EventData eventData)
	{
		try
		{
			bool flag = !eventData.Parameters.ContainsKey(eventData.CustomDataKey);
			if (flag)
			{
				return true;
			}
			Dictionary<byte, Object> dictionary = eventData.Parameters[eventData.CustomDataKey].Cast<Dictionary<byte, Object>>();
			bool flag2 = !dictionary.ContainsKey(0) || !dictionary.ContainsKey(1);
			if (flag2)
			{
				return true;
			}
			byte b = dictionary[0].Unbox<byte>();
			bool flag3 = b != 21;
			if (flag3)
			{
				return true;
			}
			bool flag4 = dictionary.ContainsKey(10) && dictionary[10].Unbox<bool>();
			bool flag5 = dictionary.ContainsKey(11) && dictionary[11].Unbox<bool>();
			int num = dictionary[1].Unbox<int>();
			Player playerNewtworkedId = num.GetPlayerNewtworkedId();
			bool flag6 = playerNewtworkedId == null;
			if (flag6)
			{
				return true;
			}
			string displayName = playerNewtworkedId.Method_Internal_get_APIUser_0().displayName;
			string displayName2 = PlayerUtil.GetLocalVRCPlayer().Method_Public_get_VRCPlayerApi_0().displayName;
			return this.HandleModerationState(displayName, displayName2, flag4, flag5);
		}
		catch (Exception ex)
		{
			MelonLogger.Error("Error in OnEventPatch: " + ex.Message + "\n" + ex.StackTrace);
		}
		return true;
	}

	// Token: 0x0600005A RID: 90 RVA: 0x000062D8 File Offset: 0x000044D8
	private bool HandleModerationState(string playerName, string localPlayerName, bool isBlocked, bool isMuted)
	{
		bool flag;
		if (isBlocked)
		{
			this.AddToModerationList(PlayerUtil.knownBlocks, playerName, "<color=red>" + playerName + " has blocked you.</color>", ConsoleColor.Red, "Moderation Alert", playerName + " has blocked you");
			flag = false;
		}
		else
		{
			bool flag2 = PlayerUtil.knownBlocks.Contains(playerName);
			if (flag2)
			{
				this.RemoveFromModerationList(PlayerUtil.knownBlocks, playerName, "<color=green>" + playerName + " has unblocked you.</color>", ConsoleColor.Green, "Moderation Alert", playerName + " has unblocked you");
				flag = false;
			}
			else if (isMuted)
			{
				this.AddToModerationList(PlayerUtil.knownMutes, playerName, "<color=red>" + playerName + " has muted you.</color>", ConsoleColor.Red, "Moderation Alert", playerName + " has muted you");
				flag = false;
			}
			else
			{
				bool flag3 = PlayerUtil.knownMutes.Contains(playerName) && playerName != localPlayerName;
				if (flag3)
				{
					this.RemoveFromModerationList(PlayerUtil.knownMutes, playerName, "<color=green>" + playerName + " has unmuted you.</color>", ConsoleColor.Green, "Moderation Alert", playerName + " has unmuted you");
					flag = false;
				}
				else
				{
					flag = true;
				}
			}
		}
		return flag;
	}

	// Token: 0x0600005B RID: 91 RVA: 0x000063F8 File Offset: 0x000045F8
	private void AddToModerationList(List<string> list, string playerName, string uiMessage, ConsoleColor logColor, string toastTitle, string toastMessage)
	{
		bool flag = !list.Contains(playerName);
		if (flag)
		{
			list.Add(playerName);
			OtherUtil.processStrings.Add(uiMessage);
			MelonLogger.Msg(logColor, toastMessage);
			ToastNotif.Toast(toastTitle, toastMessage, null, 5f);
		}
	}

	// Token: 0x0600005C RID: 92 RVA: 0x00006444 File Offset: 0x00004644
	private void RemoveFromModerationList(List<string> list, string playerName, string uiMessage, ConsoleColor logColor, string toastTitle, string toastMessage)
	{
		bool flag = list.Contains(playerName);
		if (flag)
		{
			list.Remove(playerName);
			OtherUtil.processStrings.Add(uiMessage);
			MelonLogger.Msg(logColor, toastMessage);
			ToastNotif.Toast(toastTitle, toastMessage, null, 5f);
		}
	}
}
