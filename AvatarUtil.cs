using System;
using VRC;
using VRC.Core;
using VRC.UI;

// Token: 0x02000003 RID: 3
public static class AvatarUtil
{
	// Token: 0x06000007 RID: 7 RVA: 0x00003FE8 File Offset: 0x000021E8
	public static ApiAvatar GetAvatarInfo(this Player player)
	{
		return (player != null) ? player.Method_Public_get_ApiAvatar_PDM_0() : null;
	}

	// Token: 0x06000008 RID: 8 RVA: 0x0000400C File Offset: 0x0000220C
	public static void ChangeAvatar(string avatar_id)
	{
		PageAvatar.Method_Public_Static_Void_ApiAvatar_String_0(new ApiAvatar
		{
			id = avatar_id
		}, "AvatarMenu");
	}
}
