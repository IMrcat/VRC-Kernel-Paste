using System;
using KernelClient.Wrapper;
using ReMod.Core.UI.QuickMenu;
using UnityEngine;
using VRC.SDKBase;

// Token: 0x02000017 RID: 23
internal class Movement : KernelModule
{
	// Token: 0x06000082 RID: 130 RVA: 0x00007030 File Offset: 0x00005230
	public override void OnUiManagerInit()
	{
		ReMenuCategory category = MenuSetup._uiManager.QMMenu.GetCategoryPage(PageNames.Movement).GetCategory(CatagoryNames.Movement);
		category.AddButton("Run Speed", "Adjust your running speed.", new Action(this.RunSpeed), null, "#ffffff");
		category.AddButton("Walk Speed", "Adjust your walking speed.", new Action(this.WalkSpeed), null, "#ffffff");
		category.AddButton("Jump Power", "Adjust your jump height.", new Action(this.JumpPower), null, "#ffffff");
		category.AddToggle("Enable Custom Gravity", "Toggle custom gravity settings.", new Action<bool>(this.ToggleCustomGravity));
		category.AddButton("Set Custom Gravity", "Adjust gravity strength.", new Action(this.CustomGravity), null, "#ffffff");
		category.AddButton("Reset Movement Settings", "Revert all movement settings to default.", new Action(this.ResetDefaults), null, "#ffffff");
	}

	// Token: 0x06000083 RID: 131 RVA: 0x0000712C File Offset: 0x0000532C
	private void GetDefaults()
	{
		bool flag = !this._defaultsSet;
		if (flag)
		{
			this._defaultWalkSpeed = Networking.LocalPlayer.GetWalkSpeed();
			this._defaultRunSpeed = Networking.LocalPlayer.GetRunSpeed();
			this._defaultJumpPower = Networking.LocalPlayer.GetJumpImpulse();
			this._defaultGravity = Physics.gravity;
			this._defaultsSet = true;
			this._walkSpeed = this._defaultWalkSpeed;
			this._runSpeed = this._defaultRunSpeed;
			this._jumpPower = this._defaultJumpPower;
			this._enableCustomGravity = false;
			this._customGravity = this._defaultGravity.y;
		}
	}

	// Token: 0x06000084 RID: 132 RVA: 0x000071CC File Offset: 0x000053CC
	private void ApplyCurrentSettings()
	{
		Networking.LocalPlayer.SetWalkSpeed(this._walkSpeed);
		Networking.LocalPlayer.SetRunSpeed(this._runSpeed);
		Networking.LocalPlayer.SetJumpImpulse(this._jumpPower);
		bool enableCustomGravity = this._enableCustomGravity;
		if (enableCustomGravity)
		{
			Physics.gravity = new Vector3(this._defaultGravity.x, this._customGravity, this._defaultGravity.z);
			Console.WriteLine(string.Format("Custom Gravity Applied: {0}", Physics.gravity.y));
		}
		else
		{
			Physics.gravity = this._defaultGravity;
			Console.WriteLine("Custom Gravity Disabled: Reverted to Default Gravity");
		}
	}

	// Token: 0x06000085 RID: 133 RVA: 0x0000727C File Offset: 0x0000547C
	private void RunSpeed()
	{
		this.GetDefaults();
		Action<string> action = delegate(string input)
		{
			float num;
			bool flag = float.TryParse(input, out num);
			if (flag)
			{
				this._runSpeed = Mathf.Clamp(num, 1f, 100f);
				Networking.LocalPlayer.SetRunSpeed(this._runSpeed);
				Console.WriteLine(string.Format("Run Speed set to {0}", this._runSpeed));
			}
			else
			{
				Console.WriteLine("Invalid input for Run Speed.");
			}
		};
		OtherUtil.InputPopup("Set Run Speed", this._runSpeed.ToString(), "Submit", null, action, null, false, 0, false);
	}

	// Token: 0x06000086 RID: 134 RVA: 0x000072C0 File Offset: 0x000054C0
	private void WalkSpeed()
	{
		this.GetDefaults();
		Action<string> action = delegate(string input)
		{
			float num;
			bool flag = float.TryParse(input, out num);
			if (flag)
			{
				this._walkSpeed = Mathf.Clamp(num, 1f, 100f);
				Networking.LocalPlayer.SetWalkSpeed(this._walkSpeed);
				Console.WriteLine(string.Format("Walk Speed set to {0}", this._walkSpeed));
			}
			else
			{
				Console.WriteLine("Invalid input for Walk Speed.");
			}
		};
		OtherUtil.InputPopup("Set Walk Speed", this._walkSpeed.ToString(), "Submit", null, action, null, false, 0, false);
	}

	// Token: 0x06000087 RID: 135 RVA: 0x00007304 File Offset: 0x00005504
	private void JumpPower()
	{
		this.GetDefaults();
		Action<string> action = delegate(string input)
		{
			float num;
			bool flag = float.TryParse(input, out num);
			if (flag)
			{
				this._jumpPower = Mathf.Clamp(num, 1f, 100f);
				Networking.LocalPlayer.SetJumpImpulse(this._jumpPower);
				Console.WriteLine(string.Format("Jump Power set to {0}", this._jumpPower));
			}
			else
			{
				Console.WriteLine("Invalid input for Jump Power.");
			}
		};
		OtherUtil.InputPopup("Set Jump Power", this._jumpPower.ToString(), "Submit", null, action, null, false, 0, false);
	}

	// Token: 0x06000088 RID: 136 RVA: 0x0000231E File Offset: 0x0000051E
	private void ToggleCustomGravity(bool state)
	{
		this.GetDefaults();
		this._enableCustomGravity = state;
		this.ApplyCurrentSettings();
		Console.WriteLine("Custom Gravity " + (state ? "Enabled" : "Disabled"));
	}

	// Token: 0x06000089 RID: 137 RVA: 0x00007348 File Offset: 0x00005548
	private void CustomGravity()
	{
		bool flag = !this._enableCustomGravity;
		if (flag)
		{
			Console.WriteLine("Custom Gravity is disabled. Enable it first to adjust gravity.");
		}
		else
		{
			Action<string> action = delegate(string input)
			{
				float num;
				bool flag2 = float.TryParse(input, out num);
				if (flag2)
				{
					this._customGravity = Mathf.Clamp(num, -20f, 0f);
					this.ApplyCurrentSettings();
					Console.WriteLine(string.Format("Custom Gravity set to {0}", this._customGravity));
				}
				else
				{
					Console.WriteLine("Invalid input for Custom Gravity.");
				}
			};
			OtherUtil.InputPopup("Set Custom Gravity", this._customGravity.ToString(), "Submit", null, action, null, false, 0, false);
		}
	}

	// Token: 0x0600008A RID: 138 RVA: 0x000073A0 File Offset: 0x000055A0
	private void ResetDefaults()
	{
		bool flag = !this._defaultsSet;
		if (flag)
		{
			this.GetDefaults();
		}
		this._walkSpeed = this._defaultWalkSpeed;
		this._runSpeed = this._defaultRunSpeed;
		this._jumpPower = this._defaultJumpPower;
		this._enableCustomGravity = false;
		this._customGravity = this._defaultGravity.y;
		this.ApplyCurrentSettings();
		Console.WriteLine("Movement settings have been reset to default.");
	}

	// Token: 0x04000083 RID: 131
	private float _defaultWalkSpeed;

	// Token: 0x04000084 RID: 132
	private float _defaultRunSpeed;

	// Token: 0x04000085 RID: 133
	private float _defaultJumpPower;

	// Token: 0x04000086 RID: 134
	private bool _defaultsSet;

	// Token: 0x04000087 RID: 135
	private float _walkSpeed;

	// Token: 0x04000088 RID: 136
	private float _runSpeed;

	// Token: 0x04000089 RID: 137
	private float _jumpPower;

	// Token: 0x0400008A RID: 138
	private bool _enableCustomGravity;

	// Token: 0x0400008B RID: 139
	private float _customGravity;

	// Token: 0x0400008C RID: 140
	private Vector3 _defaultGravity;
}
