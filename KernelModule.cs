using System;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.Udon;

// Token: 0x0200001F RID: 31
public class KernelModule
{
	// Token: 0x060000BC RID: 188 RVA: 0x0000246F File Offset: 0x0000066F
	public virtual void OnSettingsInit()
	{
	}

	// Token: 0x060000BD RID: 189 RVA: 0x0000246F File Offset: 0x0000066F
	public virtual void OnUiManagerInit()
	{
	}

	// Token: 0x060000BE RID: 190 RVA: 0x0000246F File Offset: 0x0000066F
	public virtual void OnUpdate()
	{
	}

	// Token: 0x060000BF RID: 191 RVA: 0x0000246F File Offset: 0x0000066F
	public virtual void OnApiAvatar(ApiAvatar apiAvatar)
	{
	}

	// Token: 0x060000C0 RID: 192 RVA: 0x0000246F File Offset: 0x0000066F
	public virtual void OnRenderObject()
	{
	}

	// Token: 0x060000C1 RID: 193 RVA: 0x0000246F File Offset: 0x0000066F
	public virtual void OnSceneWasLoaded(int buildIndex, string sceneName)
	{
	}

	// Token: 0x060000C2 RID: 194 RVA: 0x0000807C File Offset: 0x0000627C
	public virtual bool OnEventPatch(EventData __0)
	{
		return true;
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x0000807C File Offset: 0x0000627C
	public virtual bool OnEventPatchVRC(ref EventData __0)
	{
		return true;
	}

	// Token: 0x060000C4 RID: 196 RVA: 0x0000807C File Offset: 0x0000627C
	public virtual bool OnEventSent(byte __0, object __1, RaiseEventOptions __2, SendOptions __3)
	{
		return true;
	}

	// Token: 0x060000C5 RID: 197 RVA: 0x0000807C File Offset: 0x0000627C
	public virtual bool OnUdonPatch(UdonBehaviour __instance, string __0)
	{
		return true;
	}

	// Token: 0x060000C6 RID: 198 RVA: 0x0000246F File Offset: 0x0000066F
	public virtual void OnGUI()
	{
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x0000246F File Offset: 0x0000066F
	public virtual void WaitForWorldToInitialize()
	{
	}

	// Token: 0x060000C8 RID: 200 RVA: 0x0000246F File Offset: 0x0000066F
	public virtual void OnPlayerJoined(Player player)
	{
	}

	// Token: 0x060000C9 RID: 201 RVA: 0x0000246F File Offset: 0x0000066F
	public virtual void OnPlayerLeft(Player player)
	{
	}

	// Token: 0x060000CA RID: 202 RVA: 0x0000246F File Offset: 0x0000066F
	public virtual void OnAvatarIsReady(VRCPlayer player)
	{
	}

	// Token: 0x060000CB RID: 203 RVA: 0x0000246F File Offset: 0x0000066F
	public virtual void OnAvatarInstantiate(GameObject avatar, VRCPlayer vrcPlayers)
	{
	}

	// Token: 0x060000CC RID: 204 RVA: 0x0000246F File Offset: 0x0000066F
	public virtual void OnPlayerAwake(Player __instance)
	{
	}

	// Token: 0x060000CD RID: 205 RVA: 0x0000246F File Offset: 0x0000066F
	public virtual void OnEnterWorld(ApiWorld world, ApiWorldInstance instance)
	{
	}

	// Token: 0x060000CE RID: 206 RVA: 0x0000246F File Offset: 0x0000066F
	public virtual void OnApplicationStart()
	{
	}
}
