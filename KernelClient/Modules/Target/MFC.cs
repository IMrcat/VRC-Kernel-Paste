using System;
using ReMod.Core.UI.QuickMenu;
using ReMod.Core.VRChat;
using VRC;

namespace KernelClient.Modules.Target
{
	// Token: 0x0200009C RID: 156
	internal class MFC : KernelModule
	{
		// Token: 0x0600043D RID: 1085 RVA: 0x00018050 File Offset: 0x00016250
		public override void OnUiManagerInit()
		{
			IButtonPage targetMenu = MenuSetup._uiManager.TargetMenu;
			targetMenu.AddButton("Force clone avatar", "Force clone's the persons current avatar.", delegate
			{
				this._target = PlayerExtensions.GetPlayer(MenuEx.QMSelectedUserLocal.field_Private_IUser_0.Method_Public_Abstract_Virtual_New_get_String_0());
				bool flag = this._target == null;
				if (!flag)
				{
					AvatarUtil.ChangeAvatar(this._target.Method_Public_get_ApiAvatar_PDM_0().id);
					ToastNotif.Toast("Avatar Cloned", "You have cloned the avatar of " + this._target.Method_Internal_get_APIUser_0().displayName, null, 5f);
				}
			}, null, "#ffffff");
		}

		// Token: 0x040002D6 RID: 726
		private Player _target;
	}
}
