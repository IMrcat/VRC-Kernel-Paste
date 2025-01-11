using System;
using ReMod.Core.UI.QuickMenu;
using ReMod.Core.VRChat;
using UnityEngine;
using VRC;

// Token: 0x02000018 RID: 24
internal class SitOn : KernelModule
{
	// Token: 0x06000090 RID: 144 RVA: 0x000075C8 File Offset: 0x000057C8
	public override void OnUiManagerInit()
	{
		ReCategoryPage reCategoryPage = new ReCategoryPage(MenuEx.QMSelectedUserLocal.transform);
		ReMenuCategory reMenuCategory = reCategoryPage.AddCategory("Sit On Menu");
		Action action = delegate
		{
			bool flag = MenuEx.QMSelectedUserLocal.field_Private_IUser_0 != null;
			if (flag)
			{
				this._target = PlayerExtensions.GetPlayer(MenuEx.QMSelectedUserLocal.field_Private_IUser_0.Method_Public_Abstract_Virtual_New_get_String_0());
				this.TeleportToIUser(this._target);
			}
		};
		reMenuCategory.AddButton("Sit On Head", "Sit on target (press jump to stop).", new Action(this.TeleportTargetButtonOnClick), null, "#ffffff");
		reMenuCategory.AddButton("Sit On Left Hand", "Sit on target (press jump to stop).", new Action(this.TeleportTargetLeftHandButtonOnClick), null, "#ffffff");
		reMenuCategory.AddButton("Sit On Right Hand", "Sit on target (press jump to stop).", new Action(this.TeleportTargetRightHandButtonOnClick), null, "#ffffff");
		reMenuCategory.AddButton("Sit On Right Leg", "Sit on target (press jump to stop).", new Action(this.TeleportTargetRightLegButtonOnClick), null, "#ffffff");
		reMenuCategory.AddButton("Sit On Left Leg", "Sit on target (press jump to stop).", new Action(this.TeleportTargetLeftLegButtonOnClick), null, "#ffffff");
		reMenuCategory.AddButton("Sit On Hips", "Sit on target (press jump to stop).", new Action(this.TeleportTargetHipsButtonOnClick), null, "#ffffff");
		reMenuCategory.AddButton("Stop Sit", "Stop sitting on people.", new Action(this.StopSit), null, "#ffffff");
	}

	// Token: 0x06000091 RID: 145 RVA: 0x000076F4 File Offset: 0x000058F4
	private void StandardSetup()
	{
		Player player = PlayerExtensions.GetPlayer(MenuEx.QMSelectedUserLocal.field_Private_IUser_0.Method_Public_Abstract_Virtual_New_get_String_0());
		bool flag = !(player == null);
		if (flag)
		{
			this._target = player;
			this.SetGravity();
			this.TeleportToIUser(player);
		}
	}

	// Token: 0x06000092 RID: 146 RVA: 0x00002355 File Offset: 0x00000555
	private void TeleportTargetButtonOnClick()
	{
		this._bodyPart = "Head";
		this.StandardSetup();
	}

	// Token: 0x06000093 RID: 147 RVA: 0x0000236A File Offset: 0x0000056A
	private void TeleportTargetLeftHandButtonOnClick()
	{
		this._bodyPart = "LeftHand";
		this.StandardSetup();
	}

	// Token: 0x06000094 RID: 148 RVA: 0x0000237F File Offset: 0x0000057F
	private void TeleportTargetRightHandButtonOnClick()
	{
		this._bodyPart = "RightHand";
		this.StandardSetup();
	}

	// Token: 0x06000095 RID: 149 RVA: 0x00002394 File Offset: 0x00000594
	private void TeleportTargetRightLegButtonOnClick()
	{
		this._bodyPart = "RightLeg";
		this.StandardSetup();
	}

	// Token: 0x06000096 RID: 150 RVA: 0x000023A9 File Offset: 0x000005A9
	private void TeleportTargetLeftLegButtonOnClick()
	{
		this._bodyPart = "LeftLeg";
		this.StandardSetup();
	}

	// Token: 0x06000097 RID: 151 RVA: 0x000023BE File Offset: 0x000005BE
	private void TeleportTargetHipsButtonOnClick()
	{
		this._bodyPart = "Hips";
		this.StandardSetup();
	}

	// Token: 0x06000098 RID: 152 RVA: 0x000023D3 File Offset: 0x000005D3
	private void StopSit()
	{
		this._target = null;
		this.RemoveSetGravity();
	}

	// Token: 0x06000099 RID: 153 RVA: 0x00007740 File Offset: 0x00005940
	private void SetGravity()
	{
		bool flag = !(Physics.gravity == Vector3.zero);
		if (flag)
		{
			this._originalGravity = Physics.gravity;
			Physics.gravity = Vector3.zero;
		}
	}

	// Token: 0x0600009A RID: 154 RVA: 0x0000777C File Offset: 0x0000597C
	private void RemoveSetGravity()
	{
		bool flag = !(this._originalGravity == Vector3.zero);
		if (flag)
		{
			Physics.gravity = this._originalGravity;
		}
	}

	// Token: 0x0600009B RID: 155 RVA: 0x000077B0 File Offset: 0x000059B0
	private void TeleportToIUser(Player user)
	{
		try
		{
			VRCPlayer vrcplayer = user._vrcplayer;
			bool flag = !(vrcplayer == null);
			if (flag)
			{
				Vector3 vector = default(Vector3);
				bool flag2 = this._bodyPart == "Head";
				if (flag2)
				{
					vector = vrcplayer.field_Internal_Animator_0.GetBoneTransform(10).position + new Vector3(0f, 0.1f, 0f);
				}
				bool flag3 = this._bodyPart == "LeftHand";
				if (flag3)
				{
					vector = vrcplayer.field_Internal_Animator_0.GetBoneTransform(27).position + new Vector3(0f, 0.1f, 0f);
				}
				bool flag4 = this._bodyPart == "RightHand";
				if (flag4)
				{
					vector = vrcplayer.field_Internal_Animator_0.GetBoneTransform(42).position + new Vector3(0f, 0.1f, 0f);
				}
				bool flag5 = this._bodyPart == "RightLeg";
				if (flag5)
				{
					vector = vrcplayer.field_Internal_Animator_0.GetBoneTransform(6).position + new Vector3(0f, 0.1f, 0f);
				}
				bool flag6 = this._bodyPart == "LeftLeg";
				if (flag6)
				{
					vector = vrcplayer.field_Internal_Animator_0.GetBoneTransform(5).position + new Vector3(0f, 0.1f, 0f);
				}
				bool flag7 = this._bodyPart == "Hips";
				if (flag7)
				{
					vector = vrcplayer.field_Internal_Animator_0.GetBoneTransform(0).position + new Vector3(0f, 0.1f, 0f);
				}
				Transform transform = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform;
				Vector3 playerLastPos = this._playerLastPos;
				bool flag8 = this._playerLastPos != transform.position;
				if (flag8)
				{
					transform.position = vector;
				}
				this._playerLastPos = vector;
			}
		}
		catch
		{
			this._target = null;
			this.RemoveSetGravity();
		}
	}

	// Token: 0x0600009C RID: 156 RVA: 0x000079E4 File Offset: 0x00005BE4
	public override void OnUpdate()
	{
		bool flag = this._target != null;
		if (flag)
		{
			bool keyDown = Input.GetKeyDown(32);
			if (keyDown)
			{
				this._target = null;
				this.RemoveSetGravity();
			}
			else
			{
				bool button = Input.GetButton("Oculus_CrossPlatform_Button1");
				if (button)
				{
					this._target = null;
					this.RemoveSetGravity();
				}
				else
				{
					this.TeleportToIUser(this._target);
				}
			}
		}
	}

	// Token: 0x0400008D RID: 141
	private Player _target;

	// Token: 0x0400008E RID: 142
	private bool _orbitTarget;

	// Token: 0x0400008F RID: 143
	private string _bodyPart;

	// Token: 0x04000090 RID: 144
	private Vector3 _originalGravity;

	// Token: 0x04000091 RID: 145
	private Vector3 _playerLastPos;
}
