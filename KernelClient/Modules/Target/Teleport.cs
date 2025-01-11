using System;
using ReMod.Core.UI.MainMenu;
using ReMod.Core.UI.QuickMenu;
using ReMod.Core.VRChat;
using VRC;

namespace KernelClient.Modules.Target
{
	// Token: 0x0200009A RID: 154
	internal class Teleport : KernelModule
	{
		// Token: 0x06000434 RID: 1076 RVA: 0x00017F08 File Offset: 0x00016108
		public override void OnUiManagerInit()
		{
			IButtonPage targetMenu = MenuSetup._uiManager.TargetMenu;
			Action action = delegate
			{
				bool flag = MenuEx.QMSelectedUserLocal.field_Private_IUser_0 != null;
				if (flag)
				{
					this._target = PlayerExtensions.GetPlayer(MenuEx.QMSelectedUserLocal.field_Private_IUser_0.Method_Public_Abstract_Virtual_New_get_String_0());
					this.TeleportToIUser(this._target);
				}
			};
			Action action2 = delegate
			{
				bool flag2 = OtherUtil.GetMainMenuSelectedUser().field_Private_IUser_0 != null;
				if (flag2)
				{
					this._target = PlayerExtensions.GetPlayer(OtherUtil.GetMainMenuSelectedUser().field_Private_IUser_0.Method_Public_Abstract_Virtual_New_get_String_0());
					this.TeleportToIUser(this._target);
					MenuEx.MMInstance.Method_Private_Void_0();
				}
			};
			targetMenu.AddButton("Teleport", "", action, null, "#ffffff");
			ReMMUserButton reMMUserButton = new ReMMUserButton("Teleport to them", "Teleport to them", action2, null, MMenuPrefabs.MMUserDetailButton.transform.parent);
		}

		// Token: 0x06000435 RID: 1077 RVA: 0x00017F74 File Offset: 0x00016174
		private void TeleportToIUser(Player user)
		{
			VRCPlayer vrcplayer = user._vrcplayer;
			bool flag = !(vrcplayer == null);
			if (flag)
			{
				VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position = vrcplayer.transform.position;
			}
		}

		// Token: 0x040002D4 RID: 724
		private Player _target;
	}
}
