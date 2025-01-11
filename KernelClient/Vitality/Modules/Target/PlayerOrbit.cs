using System;
using ReMod.Core.UI.MainMenu;
using ReMod.Core.UI.QuickMenu;
using ReMod.Core.VRChat;
using UnityEngine;
using VRC;

namespace Vitality.Modules.Target
{
	// Token: 0x02000043 RID: 67
	internal class PlayerOrbit : KernelModule
	{
		// Token: 0x0600017F RID: 383 RVA: 0x0000A8E8 File Offset: 0x00008AE8
		public override void OnUiManagerInit()
		{
			ReMenuCategory reMenuCategory = new ReCategoryPage(MenuEx.QMSelectedUserLocal.transform).AddCategory("Player Orbit Menu");
			reMenuCategory.AddButton("Start Orbit", "Begin orbiting target (Press Jump to stop)", new Action(this.ToggleOrbitQuickMenu), null, "#ffffff");
			reMenuCategory.AddButton("Speed +", "Increase orbit speed", delegate
			{
				this._orbitSpeed = Mathf.Min(this._orbitSpeed + 0.5f, 5f);
			}, null, "#ffffff");
			reMenuCategory.AddButton("Speed -", "Decrease orbit speed", delegate
			{
				this._orbitSpeed = Mathf.Max(this._orbitSpeed - 0.5f, 0.5f);
			}, null, "#ffffff");
			reMenuCategory.AddButton("Radius +", "Increase orbit radius", delegate
			{
				this._orbitRadius = Mathf.Min(this._orbitRadius + 0.5f, 5f);
			}, null, "#ffffff");
			reMenuCategory.AddButton("Radius -", "Decrease orbit radius", delegate
			{
				this._orbitRadius = Mathf.Max(this._orbitRadius - 0.5f, 0.5f);
			}, null, "#ffffff");
			reMenuCategory.AddButton("Height +", "Increase orbit height", delegate
			{
				this._orbitHeight = Mathf.Min(this._orbitHeight + 0.5f, 3f);
			}, null, "#ffffff");
			reMenuCategory.AddButton("Height -", "Decrease orbit height", delegate
			{
				this._orbitHeight = Mathf.Max(this._orbitHeight - 0.5f, -3f);
			}, null, "#ffffff");
			Action<bool> action = delegate(bool state)
			{
				this._allowFreeLook = state;
			};
			reMenuCategory.AddToggle("Free Look", "Toggle ability to look around while orbiting", action, this._allowFreeLook);
			reMenuCategory.AddButton("Stop Orbit", "Stop orbiting target", new Action(this.StopOrbit), null, "#ffffff");
			Action action2 = delegate
			{
				bool flag = OtherUtil.GetMainMenuSelectedUser().field_Private_IUser_0 != null;
				if (flag)
				{
					this._target = PlayerExtensions.GetPlayer(OtherUtil.GetMainMenuSelectedUser().field_Private_IUser_0.Method_Public_Abstract_Virtual_New_get_String_0());
					this.ToggleOrbit();
					MenuEx.MMInstance.Method_Private_Void_0();
				}
			};
			new ReMMUserButton("Toggle Orbit", "Start/Stop orbiting around target", action2, null, MMenuPrefabs.MMUserDetailButton.transform.parent);
		}

		// Token: 0x06000180 RID: 384 RVA: 0x0000AA7C File Offset: 0x00008C7C
		private void ToggleOrbitQuickMenu()
		{
			bool flag = MenuEx.QMSelectedUserLocal.field_Private_IUser_0 != null;
			if (flag)
			{
				this._target = PlayerExtensions.GetPlayer(MenuEx.QMSelectedUserLocal.field_Private_IUser_0.Method_Public_Abstract_Virtual_New_get_String_0());
				this.ToggleOrbit();
			}
		}

		// Token: 0x06000181 RID: 385 RVA: 0x0000AAC0 File Offset: 0x00008CC0
		private void ToggleOrbit()
		{
			this._isOrbiting = !this._isOrbiting;
			bool isOrbiting = this._isOrbiting;
			if (isOrbiting)
			{
				this.StartOrbit();
			}
			else
			{
				this.StopOrbit();
			}
		}

		// Token: 0x06000182 RID: 386 RVA: 0x0000AAFC File Offset: 0x00008CFC
		private void StartOrbit()
		{
			bool flag = this._target == null || this._target._vrcplayer == null;
			if (!flag)
			{
				this._currentAngle = 0f;
				this.SetGravity();
				this._lastPlayerRotation = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.rotation;
				this.UpdatePosition();
			}
		}

		// Token: 0x06000183 RID: 387 RVA: 0x000027F3 File Offset: 0x000009F3
		private void StopOrbit()
		{
			this._isOrbiting = false;
			this._target = null;
			this.RemoveSetGravity();
		}

		// Token: 0x06000184 RID: 388 RVA: 0x0000AB60 File Offset: 0x00008D60
		private void SetGravity()
		{
			bool flag = Physics.gravity != Vector3.zero;
			if (flag)
			{
				this._originalGravity = Physics.gravity;
				Physics.gravity = Vector3.zero;
			}
		}

		// Token: 0x06000185 RID: 389 RVA: 0x0000AB9C File Offset: 0x00008D9C
		private void RemoveSetGravity()
		{
			bool flag = this._originalGravity != Vector3.zero;
			if (flag)
			{
				Physics.gravity = this._originalGravity;
			}
		}

		// Token: 0x06000186 RID: 390 RVA: 0x0000ABCC File Offset: 0x00008DCC
		private bool IsPlayerMoving()
		{
			bool flag = Input.GetKey(119) || Input.GetKey(97) || Input.GetKey(115) || Input.GetKey(100) || Input.GetKey(273) || Input.GetKey(276) || Input.GetKey(274) || Input.GetKey(275);
			bool flag2 = flag;
			bool flag3;
			if (flag2)
			{
				flag3 = true;
			}
			else
			{
				Vector3 vector;
				vector..ctor(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
				bool flag4 = Vector3.Distance(vector, this._lastInputVector) > this._movementThreshold;
				if (flag4)
				{
					this._lastInputVector = vector;
					flag3 = true;
				}
				else
				{
					this._lastInputVector = vector;
					flag3 = false;
				}
			}
			return flag3;
		}

		// Token: 0x06000187 RID: 391 RVA: 0x0000AC8C File Offset: 0x00008E8C
		private void UpdatePosition()
		{
			try
			{
				bool flag = this._target == null || this._target._vrcplayer == null;
				if (flag)
				{
					this.StopOrbit();
				}
				else
				{
					VRCPlayer field_Internal_Static_VRCPlayer_ = VRCPlayer.field_Internal_Static_VRCPlayer_0;
					bool flag2 = field_Internal_Static_VRCPlayer_ == null;
					if (!flag2)
					{
						Vector3 position = this._target._vrcplayer.transform.position;
						float num = Mathf.Cos(this._currentAngle * 0.017453292f) * this._orbitRadius;
						float num2 = Mathf.Sin(this._currentAngle * 0.017453292f) * this._orbitRadius;
						Vector3 vector = position + new Vector3(num, this._orbitHeight, num2);
						Transform transform = field_Internal_Static_VRCPlayer_.transform;
						transform.position = vector;
						this._lastPosition = vector;
						bool flag3 = !this._allowFreeLook;
						if (flag3)
						{
							Vector3 vector2 = position - vector;
							bool flag4 = vector2 != Vector3.zero;
							if (flag4)
							{
								transform.rotation = Quaternion.LookRotation(vector2);
							}
						}
						else
						{
							this._lastPlayerRotation = transform.rotation;
						}
					}
				}
			}
			catch
			{
				this.StopOrbit();
			}
		}

		// Token: 0x06000188 RID: 392 RVA: 0x0000ADDC File Offset: 0x00008FDC
		public override void OnUpdate()
		{
			bool flag = !this._isOrbiting;
			if (!flag)
			{
				bool flag2 = Input.GetKeyDown(32) || Input.GetButton("Oculus_CrossPlatform_Button1") || this.IsPlayerMoving();
				if (flag2)
				{
					this.StopOrbit();
				}
				else
				{
					this._currentAngle += Time.deltaTime * this._orbitSpeed * 360f;
					this.UpdatePosition();
				}
			}
		}

		// Token: 0x04000102 RID: 258
		private Player _target;

		// Token: 0x04000103 RID: 259
		private bool _isOrbiting;

		// Token: 0x04000104 RID: 260
		private Vector3 _originalGravity;

		// Token: 0x04000105 RID: 261
		private Vector3 _lastPosition;

		// Token: 0x04000106 RID: 262
		private Quaternion _lastPlayerRotation;

		// Token: 0x04000107 RID: 263
		private bool _allowFreeLook = true;

		// Token: 0x04000108 RID: 264
		private float _movementThreshold = 0.1f;

		// Token: 0x04000109 RID: 265
		private Vector3 _lastInputVector;

		// Token: 0x0400010A RID: 266
		private float _orbitSpeed = 2f;

		// Token: 0x0400010B RID: 267
		private float _orbitRadius = 1f;

		// Token: 0x0400010C RID: 268
		private float _orbitHeight = 0f;

		// Token: 0x0400010D RID: 269
		private float _currentAngle;
	}
}
