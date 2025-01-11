using System;
using System.Linq;
using KernelClient.Utils;
using KernelClient.Wrapper;
using MelonLoader;
using ReMod.Core.UI.QuickMenu;
using ReMod.Core.VRChat;
using UnityEngine;
using VRC.Core;
using VRC.SDKBase;

namespace KernelClient.Modules.World
{
	// Token: 0x020000B1 RID: 177
	internal class BlackCat : KernelModule
	{
		// Token: 0x1700006D RID: 109
		// (get) Token: 0x060004B2 RID: 1202 RVA: 0x00003B41 File Offset: 0x00001D41
		// (set) Token: 0x060004B3 RID: 1203 RVA: 0x00003B49 File Offset: 0x00001D49
		public VRC_Pickup[] _getAllPickups { get; set; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060004B4 RID: 1204 RVA: 0x00003B52 File Offset: 0x00001D52
		// (set) Token: 0x060004B5 RID: 1205 RVA: 0x00003B5A File Offset: 0x00001D5A
		public GameObject _plunger { get; private set; }

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x060004B6 RID: 1206 RVA: 0x00003B63 File Offset: 0x00001D63
		// (set) Token: 0x060004B7 RID: 1207 RVA: 0x00003B6B File Offset: 0x00001D6B
		public GameObject _goose { get; private set; }

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x060004B8 RID: 1208 RVA: 0x00003B74 File Offset: 0x00001D74
		// (set) Token: 0x060004B9 RID: 1209 RVA: 0x00003B7C File Offset: 0x00001D7C
		public GameObject _pancake { get; private set; }

		// Token: 0x060004BA RID: 1210 RVA: 0x0001B448 File Offset: 0x00019648
		public override void OnEnterWorld(ApiWorld world, ApiWorldInstance instance)
		{
			try
			{
				bool flag = this._getAllPickups != null;
				if (flag)
				{
					this._getAllPickups = null;
				}
				bool flag2 = this._plungerToggle != null;
				if (flag2)
				{
					this._plungerToggle.Toggle(new bool?(false), true);
				}
				bool flag3 = this._pancakeToggle != null;
				if (flag3)
				{
					this._pancakeToggle.Toggle(new bool?(false), true);
				}
				bool flag4 = this._gooseToggle != null;
				if (flag4)
				{
					this._gooseToggle.Toggle(new bool?(false), true);
				}
				bool flag5 = this._gooseToggleLocal != null;
				if (flag5)
				{
					this._gooseToggleLocal.Toggle(new bool?(false), true);
				}
				bool flag6 = this._pancakeToggleLocal != null;
				if (flag6)
				{
					this._pancakeToggleLocal.Toggle(new bool?(false), true);
				}
				bool flag7 = this._plungerToggleLocal != null;
				if (flag7)
				{
					this._plungerToggleLocal.Toggle(new bool?(false), true);
				}
				bool flag8 = world == null;
				if (!flag8)
				{
					bool flag9 = world.id == "wrld_4cf554b4-430c-4f8f-b53e-1f294eed230b" && this.targetMenu != null;
					if (flag9)
					{
						this.targetMenu.Active = true;
						this._plungerToggle.GameObject.active = true;
						this._pancakeToggle.GameObject.active = true;
						this._gooseToggle.GameObject.active = true;
					}
					else
					{
						this._plungerToggle.GameObject.active = false;
						this._pancakeToggle.GameObject.active = false;
						this._gooseToggle.GameObject.active = false;
						this.targetMenu.Active = false;
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error(ex);
			}
		}

		// Token: 0x060004BB RID: 1211 RVA: 0x0001B620 File Offset: 0x00019820
		public override void OnUpdate()
		{
			bool flag = !(this._target != null);
			if (!flag)
			{
				bool gooseChase = this._gooseChase;
				if (gooseChase)
				{
					this.GooseChase(this._target);
				}
				bool plungerHat = this._plungerHat;
				if (plungerHat)
				{
					this.PlungerHat(this._target);
				}
				bool pancakeHat = this._pancakeHat;
				if (pancakeHat)
				{
					this.PancakeHat(this._target);
				}
			}
		}

		// Token: 0x060004BC RID: 1212 RVA: 0x0001B688 File Offset: 0x00019888
		public void GooseChase(VRCPlayer player)
		{
			try
			{
				bool flag = Networking.GetOwner(this._goose) != Networking.LocalPlayer;
				if (flag)
				{
					Networking.SetOwner(Networking.LocalPlayer, this._goose);
				}
				Vector3 position = player.transform.position;
				position..ctor(position.x - 0.3f, position.y + 0.2f, position.z);
				this._goose.transform.transform.LookAt(position);
				this._goose.transform.rotation *= Quaternion.FromToRotation(Vector3.up, Vector3.back);
				this._goose.transform.position = Vector3.MoveTowards(this._goose.transform.position, position, 0.1f);
			}
			catch
			{
			}
		}

		// Token: 0x060004BD RID: 1213 RVA: 0x0001B778 File Offset: 0x00019978
		public void PlungerHat(VRCPlayer player)
		{
			try
			{
				bool flag = Networking.GetOwner(this._plunger) != Networking.LocalPlayer;
				if (flag)
				{
					Networking.SetOwner(Networking.LocalPlayer, this._plunger);
				}
				this._plunger.transform.position = player.field_Private_VRCPlayerApi_0.GetBonePosition(10) + Vector3.up * 0.35f;
				this._plunger.gameObject.transform.eulerAngles = new Vector3(270f, 240f, 0f);
			}
			catch
			{
			}
		}

		// Token: 0x060004BE RID: 1214 RVA: 0x0001B828 File Offset: 0x00019A28
		public void PancakeHat(VRCPlayer player)
		{
			try
			{
				bool flag = Networking.GetOwner(this._pancake) != Networking.LocalPlayer;
				if (flag)
				{
					Networking.SetOwner(Networking.LocalPlayer, this._pancake);
				}
				player.field_Private_VRCPlayerApi_0.GetBonePosition(10);
				this._pancake.transform.position = player.field_Private_VRCPlayerApi_0.GetBonePosition(10) + Vector3.up * 0.15f;
				this._pancake.gameObject.transform.eulerAngles = new Vector3(270f, 240f, 90f);
			}
			catch
			{
			}
		}

		// Token: 0x060004BF RID: 1215 RVA: 0x0001B8E4 File Offset: 0x00019AE4
		public void CheckPickups()
		{
			bool flag = !(RoomManager.field_Internal_Static_ApiWorld_0.id == "wrld_4cf554b4-430c-4f8f-b53e-1f294eed230b") || (this._getAllPickups != null && this._getAllPickups.Length != 0);
			if (!flag)
			{
				this._getAllPickups = Object.FindObjectsOfType<VRC_Pickup>().ToArray<VRC_Pickup>();
				this._plunger = this._getAllPickups.FirstOrDefault((VRC_Pickup x) => x.name == "storage_prop_plunger").gameObject;
				this._goose = this._getAllPickups.FirstOrDefault((VRC_Pickup x) => x.name == "Cube").gameObject;
				this._pancake = this._getAllPickups.FirstOrDefault((VRC_Pickup x) => x.name == "banana pancake02").gameObject;
			}
		}

		// Token: 0x060004C0 RID: 1216 RVA: 0x0001B9DC File Offset: 0x00019BDC
		public override void OnUiManagerInit()
		{
			ReMenuCategory category = MenuSetup._uiManager.QMMenu.GetCategoryPage(PageNames.WorldCheats).GetCategory(CatagoryNames.NormalWorlds);
			this.targetMenu = new ReCategoryPage(MenuEx.QMSelectedUserLocal.transform).AddCategory("Black Cat Toggles");
			ReMenuPage reMenuPage = category.AddMenuPage("The Black Cat", "", null, "#ffffff");
			reMenuPage.AddButton("Patreon Access", "", delegate
			{
				BlackCat.GameObjectToggle("patreon Toggles", true);
			}, null, "#ffffff");
			reMenuPage.AddButton("Own Room Decorations", "", delegate
			{
				BlackCat.GameObjectToggle("Sync Button", true);
				BlackCat.GameObjectToggle("Buy", false);
				BlackCat.GameObjectToggle("Owned", true);
			}, null, "#ffffff");
			reMenuPage.AddButton("Video Player Unlock", "", delegate
			{
				BlackCat.GameObjectToggle("Video Player Locked", false);
				BlackCat.GameObjectToggle("video player unlocked", true);
			}, null, "#ffffff");
			Action<bool> action = delegate(bool s)
			{
				this._target = PlayerUtil.GetLocalVRCPlayer();
				this.CheckPickups();
				this._gooseChase = s;
			};
			this._gooseToggleLocal = reMenuPage.AddToggle("Goose Chase", "", action, false);
			Action<bool> action2 = delegate(bool s)
			{
				this._target = PlayerUtil.GetLocalVRCPlayer();
				this.CheckPickups();
				this._pancakeHat = s;
			};
			this._pancakeToggleLocal = reMenuPage.AddToggle("Pancake hat", "", action2, false);
			Action<bool> action3 = delegate(bool s)
			{
				this._target = PlayerUtil.GetLocalVRCPlayer();
				this.CheckPickups();
				this._plungerHat = s;
			};
			this._plungerToggleLocal = reMenuPage.AddToggle("Plunger Hat", "", action3, false);
			this._gooseToggle = this.targetMenu.AddToggle("Goose Chase", "", delegate(bool s)
			{
				bool flag = MenuEx.QMSelectedUserLocal.field_Private_IUser_0 == null;
				if (!flag)
				{
					this._target = PlayerExtensions.GetPlayer(MenuEx.QMSelectedUserLocal.field_Private_IUser_0.Method_Public_Abstract_Virtual_New_get_String_0())._vrcplayer;
					this.CheckPickups();
					this._gooseChase = s;
				}
			}, false);
			this._pancakeToggle = this.targetMenu.AddToggle("Pancake hat", "", delegate(bool s)
			{
				bool flag2 = MenuEx.QMSelectedUserLocal.field_Private_IUser_0 == null;
				if (!flag2)
				{
					this._target = PlayerExtensions.GetPlayer(MenuEx.QMSelectedUserLocal.field_Private_IUser_0.Method_Public_Abstract_Virtual_New_get_String_0())._vrcplayer;
					this.CheckPickups();
					this._pancakeHat = s;
				}
			}, false);
			this._plungerToggle = this.targetMenu.AddToggle("Plunger hat", "", delegate(bool s)
			{
				bool flag3 = MenuEx.QMSelectedUserLocal.field_Private_IUser_0 == null;
				if (!flag3)
				{
					this._target = PlayerExtensions.GetPlayer(MenuEx.QMSelectedUserLocal.field_Private_IUser_0.Method_Public_Abstract_Virtual_New_get_String_0())._vrcplayer;
					this.CheckPickups();
					this._plungerHat = s;
				}
			}, false);
		}

		// Token: 0x060004C1 RID: 1217 RVA: 0x0001BBCC File Offset: 0x00019DCC
		public static void GameObjectToggle(string gameObjectName, bool toogle)
		{
			foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
			{
				bool flag = gameObject.name.Contains(gameObjectName);
				if (flag)
				{
					gameObject.active = toogle;
				}
			}
		}

		// Token: 0x04000331 RID: 817
		private VRCPlayer _target;

		// Token: 0x04000332 RID: 818
		public ReMenuToggle _gooseToggle;

		// Token: 0x04000333 RID: 819
		public ReMenuToggle _pancakeToggle;

		// Token: 0x04000334 RID: 820
		public ReMenuToggle _plungerToggle;

		// Token: 0x04000335 RID: 821
		public ReMenuToggle _gooseToggleLocal;

		// Token: 0x04000336 RID: 822
		public ReMenuToggle _pancakeToggleLocal;

		// Token: 0x04000337 RID: 823
		public ReMenuToggle _plungerToggleLocal;

		// Token: 0x04000338 RID: 824
		public bool _gooseChase;

		// Token: 0x04000339 RID: 825
		public bool _plungerHat;

		// Token: 0x0400033A RID: 826
		public bool _pancakeHat;

		// Token: 0x0400033B RID: 827
		private ReMenuCategory targetMenu;

		// Token: 0x0400033C RID: 828
		public bool _lavaspam = false;
	}
}
