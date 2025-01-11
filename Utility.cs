using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using KernelClient.Utils;
using MelonLoader;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.SDKBase;

// Token: 0x0200003A RID: 58
internal class Utility : KernelModule
{
	// Token: 0x06000144 RID: 324 RVA: 0x00009CA8 File Offset: 0x00007EA8
	public override void OnUiManagerInit()
	{
		Action<bool> action = delegate(bool s)
		{
			this._toggled = s;
			if (s)
			{
				this._myName = APIUser.CurrentUser.displayName;
				APIUser.CurrentUser.displayName = RoomManager.field_Internal_Static_ApiWorld_0.authorName;
				PlayerUtil.LocalPlayer()._vrcplayer.field_Private_VRCPlayerApi_0.displayName = RoomManager.field_Internal_Static_ApiWorld_0.authorName;
				ToastNotif.Toast("Fake World Owner Enabled", "Your name is now the world under the name " + RoomManager.field_Internal_Static_ApiWorld_0.authorName, null, 5f);
			}
			else
			{
				APIUser.CurrentUser.displayName = this._myName;
				PlayerUtil.LocalPlayer()._vrcplayer.field_Private_VRCPlayerApi_0.displayName = this._myName;
				ToastNotif.Toast("Fake World Owner Disabled", "Your name is now back to normal", null, 5f);
			}
		};
		MenuSetup._uiManager.LaunchPad.AddButton("Change Avatar By ID", "", delegate
		{
			this.ChangeAvatar();
		}, null, "#ffffff");
		MenuSetup._uiManager.LaunchPad.AddToggle("Fake World Owner", "", action, false, "#ffffff");
		MenuSetup._uiManager.LaunchPad.AddButton("Friend everyone", "", delegate
		{
			this.FriendEveryone();
		}, null, "#ffffff");
	}

	// Token: 0x06000145 RID: 325 RVA: 0x00009D3C File Offset: 0x00007F3C
	public override void OnPlayerJoined(Player player)
	{
		bool flag = this._toggled && player.field_Private_APIUser_0.id == APIUser.CurrentUser.id;
		if (flag)
		{
			player.field_Private_APIUser_0.displayName = this.GetCurrentWorldAuthorName();
		}
	}

	// Token: 0x06000146 RID: 326 RVA: 0x00009D88 File Offset: 0x00007F88
	private string GetCurrentWorldAuthorName()
	{
		ApiWorld field_Internal_Static_ApiWorld_ = RoomManager.field_Internal_Static_ApiWorld_0;
		return (field_Internal_Static_ApiWorld_ != null) ? field_Internal_Static_ApiWorld_.authorName : "No Name";
	}

	// Token: 0x06000147 RID: 327 RVA: 0x00009DB0 File Offset: 0x00007FB0
	public override void OnUpdate()
	{
		bool flag = this._toggled && RoomManager.field_Internal_Static_ApiWorld_0 != null && APIUser.CurrentUser.displayName != RoomManager.field_Internal_Static_ApiWorld_0.authorName;
		if (flag)
		{
			try
			{
				APIUser.CurrentUser.displayName = this.GetCurrentWorldAuthorName();
			}
			catch
			{
			}
			try
			{
				Networking.LocalPlayer.displayName = this.GetCurrentWorldAuthorName();
			}
			catch
			{
			}
		}
	}

	// Token: 0x06000148 RID: 328 RVA: 0x00009E40 File Offset: 0x00008040
	private void ChangeAvatar()
	{
		string systemCopyBuffer = GUIUtility.systemCopyBuffer;
		Regex regex = new Regex("avtr_[0-9a-fA-F]{8}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{12}");
		bool flag = regex.IsMatch(systemCopyBuffer);
		if (flag)
		{
			AvatarUtil.ChangeAvatar(systemCopyBuffer);
			ToastNotif.Toast("Changed Avatar to " + systemCopyBuffer, null, null, 5f);
		}
		else
		{
			MelonLogger.Msg("Invalid Avatar ID!");
			ToastNotif.Toast("Invalid Avatar ID, Please copy a proper avatar ID to the clipboard!", null, null, 5f);
		}
	}

	// Token: 0x06000149 RID: 329 RVA: 0x00002728 File Offset: 0x00000928
	private void FriendEveryone()
	{
		MelonCoroutines.Start(this.AddPeople());
	}

	// Token: 0x0600014A RID: 330 RVA: 0x00002737 File Offset: 0x00000937
	private IEnumerator AddPeople()
	{
		foreach (Player item in Object.FindObjectsOfType<Player>())
		{
			item.field_Private_Object1PublicOb1ApStBo1StLoBoSiUnique_0.Method_Public_Virtual_Final_New_Void_27();
			ToastNotif.Toast("Added " + item.Method_Internal_get_APIUser_0().username, "requested", null, 5f);
			yield return new WaitForSeconds(1f);
			item = null;
		}
		IEnumerator<Player> enumerator = null;
		ToastNotif.Toast("Added everyone", null, null, 5f);
		yield break;
		yield break;
	}

	// Token: 0x040000E9 RID: 233
	private string _myName;

	// Token: 0x040000EA RID: 234
	private bool _toggled;

	// Token: 0x040000EB RID: 235
	private static MethodInfo alignTrackingToPlayerMethod;
}
