using System;
using Il2CppSystem;
using KernelClient.Core.Mono;
using KernelClient.Utils;
using MelonLoader;
using VRC;
using VRC.Core;

// Token: 0x0200003C RID: 60
internal class Tags : KernelModule
{
	// Token: 0x06000156 RID: 342 RVA: 0x0000246F File Offset: 0x0000066F
	public override void OnUiManagerInit()
	{
	}

	// Token: 0x06000157 RID: 343 RVA: 0x0000A0D0 File Offset: 0x000082D0
	public override void OnPlayerJoined(Player player)
	{
		try
		{
			bool flag = player != null;
			if (flag)
			{
				try
				{
					VRCApplication.Method_Internal_Static_get_VRCApplication_PDM_0().gameObject.AddComponent<CustomNameplate>().Player = player;
				}
				catch
				{
				}
			}
			CustomNameplateAccountAge nameplate2 = VRCApplication.Method_Internal_Static_get_VRCApplication_PDM_0().gameObject.AddComponent<CustomNameplateAccountAge>();
			nameplate2.Player = player;
			Action<string> action = delegate
			{
			};
			Action<APIUser> action2 = delegate(APIUser s)
			{
				nameplate2.playerAge = s.date_joined;
			};
			APIUser.FetchUser(player.field_Private_APIUser_0.id, action2, action);
			MelonCoroutines.Start(PlayerUtil.NameplateColours(player, true, 10f));
		}
		catch (Exception ex)
		{
			MelonLogger.Error(ex.ToString());
		}
	}
}
