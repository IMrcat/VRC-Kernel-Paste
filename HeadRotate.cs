using System;
using KernelClient.Wrapper;
using ReMod.Core.UI.QuickMenu;
using UnityEngine;

// Token: 0x0200000D RID: 13
public class HeadRotate : KernelModule
{
	// Token: 0x06000032 RID: 50 RVA: 0x00004B90 File Offset: 0x00002D90
	public override void OnUiManagerInit()
	{
		ReMenuCategory category = MenuSetup._uiManager.QMMenu.GetCategoryPage(PageNames.Movement).GetCategory(CatagoryNames.Camera);
		category.AddToggle("Head Spin", "Toggle head spinning functionality", delegate(bool isEnabled)
		{
			this.spinthyhead = isEnabled;
		});
		category.AddToggle("Head Flip", "Toggle head flip (180 degrees)", delegate(bool isEnabled)
		{
			this.ToggleHeadFlip(isEnabled);
		});
		category.AddToggle("Spin X-Axis", "Toggle spinning around the X-axis", delegate(bool isEnabled)
		{
			this.spinX = isEnabled;
		});
		category.AddToggle("Spin Y-Axis", "Toggle spinning around the Y-axis", delegate(bool isEnabled)
		{
			this.spinY = isEnabled;
		});
		category.AddToggle("Spin Z-Axis", "Toggle spinning around the Z-axis", delegate(bool isEnabled)
		{
			this.spinZ = isEnabled;
		});
		category.AddToggle("Manual Rotate", "Enable manual rotation via Ctrl + Scroll Wheel", delegate(bool isEnabled)
		{
			this.manualRotateEnabled = isEnabled;
		});
		category.AddButton("Reset Axis", "Reset head rotation to original orientation", new Action(this.ResetAxis), null, "#ff0000");
	}

	// Token: 0x06000033 RID: 51 RVA: 0x00004C90 File Offset: 0x00002E90
	public override void OnSceneWasLoaded(int buildIndex, string sceneName)
	{
		this.mainCamera = Camera.main;
		bool flag = this.mainCamera != null;
		if (flag)
		{
			this.originalRotation = this.mainCamera.transform.rotation;
		}
		else
		{
			Debug.LogError("HeadRotate: Main camera not found.");
		}
	}

	// Token: 0x06000034 RID: 52 RVA: 0x000021B1 File Offset: 0x000003B1
	public override void OnUpdate()
	{
		this.HandleHeadSpin();
		this.HandleManualRotation();
	}

	// Token: 0x06000035 RID: 53 RVA: 0x00004CE8 File Offset: 0x00002EE8
	private void HandleHeadSpin()
	{
		bool flag = !this.spinthyhead || this.mainCamera == null;
		if (!flag)
		{
			Vector3 zero = Vector3.zero;
			bool flag2 = this.spinX;
			if (flag2)
			{
				zero.x += this.rotationSpeed * Time.deltaTime;
			}
			bool flag3 = this.spinY;
			if (flag3)
			{
				zero.y += this.rotationSpeed * Time.deltaTime;
			}
			bool flag4 = this.spinZ;
			if (flag4)
			{
				zero.z += this.rotationSpeed * Time.deltaTime;
			}
			bool flag5 = zero != Vector3.zero;
			if (flag5)
			{
				this.mainCamera.transform.Rotate(zero, 1);
			}
		}
	}

	// Token: 0x06000036 RID: 54 RVA: 0x00004DA8 File Offset: 0x00002FA8
	private void HandleManualRotation()
	{
		bool flag = !this.manualRotateEnabled || this.mainCamera == null;
		if (!flag)
		{
			bool flag2 = !Input.GetKey(306);
			if (!flag2)
			{
				float axis = Input.GetAxis("Mouse ScrollWheel");
				bool flag3 = Mathf.Abs(axis) < 0.01f;
				if (!flag3)
				{
					float num = axis * this.manualRotationSpeed;
					this.mainCamera.transform.Rotate(0f, 0f, num, 1);
				}
			}
		}
	}

	// Token: 0x06000037 RID: 55 RVA: 0x00004E2C File Offset: 0x0000302C
	private void ToggleHeadFlip(bool enable)
	{
		bool flag = this.mainCamera == null;
		if (!flag)
		{
			bool flag2 = enable && !this.hasFlipped;
			if (flag2)
			{
				this.originalRotation = this.mainCamera.transform.rotation;
				this.mainCamera.transform.Rotate(0f, 180f, 0f, 1);
				this.hasFlipped = true;
			}
			else
			{
				bool flag3 = !enable && this.hasFlipped;
				if (flag3)
				{
					this.mainCamera.transform.rotation = this.originalRotation;
					this.hasFlipped = false;
				}
			}
		}
	}

	// Token: 0x06000038 RID: 56 RVA: 0x00004ED4 File Offset: 0x000030D4
	private void ResetAxis()
	{
		bool flag = this.mainCamera == null;
		if (flag)
		{
			Debug.LogError("HeadRotate: Main camera not found.");
		}
		else
		{
			this.mainCamera.transform.rotation = this.originalRotation;
			this.spinthyhead = false;
			this.spinX = false;
			this.spinY = false;
			this.spinZ = false;
			this.flipHead = false;
			this.manualRotateEnabled = false;
			Debug.Log("HeadRotate: Rotation reset to original orientation.");
		}
	}

	// Token: 0x04000055 RID: 85
	private bool spinthyhead = false;

	// Token: 0x04000056 RID: 86
	private bool spinX = false;

	// Token: 0x04000057 RID: 87
	private bool spinY = false;

	// Token: 0x04000058 RID: 88
	private bool spinZ = false;

	// Token: 0x04000059 RID: 89
	private bool flipHead = false;

	// Token: 0x0400005A RID: 90
	private bool manualRotateEnabled = false;

	// Token: 0x0400005B RID: 91
	private float rotationSpeed = 100f;

	// Token: 0x0400005C RID: 92
	private float manualRotationSpeed = 100f;

	// Token: 0x0400005D RID: 93
	private Quaternion originalRotation;

	// Token: 0x0400005E RID: 94
	private bool hasFlipped = false;

	// Token: 0x0400005F RID: 95
	private Camera mainCamera;
}
