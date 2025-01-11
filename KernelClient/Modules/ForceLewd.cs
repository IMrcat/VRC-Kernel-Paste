using System;
using System.Collections.Generic;
using ReMod.Core.UI.QuickMenu;
using ReMod.Core.VRChat;
using UnityEngine;
using VRC;

namespace KernelClient.Modules
{
	// Token: 0x0200008C RID: 140
	internal class ForceLewd : KernelModule
	{
		// Token: 0x060003BE RID: 958 RVA: 0x00014488 File Offset: 0x00012688
		public override void OnUiManagerInit()
		{
			IButtonPage targetMenu = MenuSetup._uiManager.TargetMenu;
			Action action = delegate
			{
				bool flag = MenuEx.QMSelectedUserLocal.field_Private_IUser_0 != null;
				if (flag)
				{
					this._target = PlayerExtensions.GetPlayer(MenuEx.QMSelectedUserLocal.field_Private_IUser_0.Method_Public_Abstract_Virtual_New_get_String_0());
					this.LewdPlayer(this._target._vrcplayer);
				}
			};
			targetMenu.AddButton("Lewd em", "Make em lewd", action, null, "#ffffff");
		}

		// Token: 0x060003BF RID: 959 RVA: 0x000144C8 File Offset: 0x000126C8
		public void LewdPlayer(VRCPlayer target)
		{
			foreach (Transform transform in target.field_Private_VRCAvatarManager_0.gameObject.GetComponentsInChildren<Transform>(true))
			{
				bool flag = !transform.name.ToLower().Contains("avatar");
				if (!flag)
				{
					foreach (Renderer renderer in transform.GetComponentsInChildren<Renderer>(true))
					{
						foreach (string text in ForceLewd.TurnOn)
						{
							bool flag2 = renderer.name.ToLower().Contains(text);
							if (flag2)
							{
								renderer.gameObject.SetActive(true);
							}
						}
						foreach (string text2 in ForceLewd.TurnOff)
						{
							bool flag3 = renderer.name.ToLower().Contains(text2);
							if (flag3)
							{
								Object.Destroy(renderer.gameObject);
							}
						}
					}
				}
			}
		}

		// Token: 0x04000262 RID: 610
		public static bool autolewd = false;

		// Token: 0x04000263 RID: 611
		private Player _target;

		// Token: 0x04000264 RID: 612
		internal static List<string> TurnOff = new List<string>
		{
			"cloth", "apron", "shirt", "short", "pant", "under", "undi", "jacket", "pasties", "top",
			"bra", "skirt", "tail", "harness", "underwear", "belt", "jean", "trouser", "boxers", "hoodi",
			"bottom", "dress", "bandage", "bondage", "sweat", "cardig", "corset", "tiddy", "pastie", "google",
			"glasses", "suit", "stocking", "jewel", "frill", "gauze", "cover", "pubic", "sfw", "harn",
			"biki", "off", "disable", "sweater", "crasher", "nuke", "clapper", "nigger", "nogger", "gang",
			"coffin"
		};

		// Token: 0x04000265 RID: 613
		internal static List<string> TurnOn = new List<string>
		{
			"penis", "benis", "canine", "k9", "dick", "knotty", "uwu", "dynamic_penetration", "dynamic penetration", "cock",
			"futa", "dildo", "strap", "shlong", "dong", "vibrat", "lovense", "sex", "toy", "butt",
			"plug", "whip", "cum", "sperm", "facial", "nude", "naked", "nsfw", "blindfold", "colar",
			"dps", "pet", "vibrator", "tits", "willy", "nip", "tit"
		};
	}
}
