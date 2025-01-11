using System;
using System.Linq;
using UnityEngine;
using VRC;
using VRC.Networking;
using VRC.UI.Elements.Controls;

// Token: 0x0200001E RID: 30
public static class VrcExtensions
{
	// Token: 0x060000B4 RID: 180 RVA: 0x00007F7C File Offset: 0x0000617C
	private static ModalAlert GetHudNotification()
	{
		bool flag = VrcExtensions._hudNotification == null;
		if (flag)
		{
			VrcExtensions._hudNotification = Object.FindObjectsOfType<ModalAlert>().FirstOrDefault<ModalAlert>();
			bool flag2 = VrcExtensions._hudNotification == null;
			if (flag2)
			{
				VrcExtensions._hudNotification = new ModalAlert();
			}
		}
		return VrcExtensions._hudNotification;
	}

	// Token: 0x060000B5 RID: 181 RVA: 0x0000243D File Offset: 0x0000063D
	internal static void ToggleCharacterController(bool toggle)
	{
		Player.Method_Internal_Static_get_Player_0().gameObject.GetComponent<CharacterController>().enabled = toggle;
	}

	// Token: 0x060000B6 RID: 182 RVA: 0x00002456 File Offset: 0x00000656
	internal static void ToggleNetworkSerializer(bool value)
	{
		Player.Method_Internal_Static_get_Player_0().gameObject.GetComponent<FlatBufferNetworkSerializer>().enabled = value;
	}

	// Token: 0x060000B7 RID: 183 RVA: 0x00007FD0 File Offset: 0x000061D0
	public static void Render(this HighlightsFX fx, Renderer renderer, bool enabled)
	{
		bool flag = !(fx == null) && !(renderer == null);
		if (flag)
		{
			if (enabled)
			{
				fx.field_Protected_HashSet_1_MeshFilter_0.Add(renderer.GetComponent<MeshFilter>());
			}
			else
			{
				fx.field_Protected_HashSet_1_MeshFilter_0.Remove(renderer.GetComponent<MeshFilter>());
			}
		}
	}

	// Token: 0x060000B8 RID: 184 RVA: 0x0000802C File Offset: 0x0000622C
	public static void Render(this HighlightsFX fx, MeshFilter renderer, bool enabled)
	{
		bool flag = !(fx == null) && !(renderer == null);
		if (flag)
		{
			if (enabled)
			{
				fx.field_Protected_HashSet_1_MeshFilter_0.Add(renderer);
			}
			else
			{
				fx.field_Protected_HashSet_1_MeshFilter_0.Remove(renderer);
			}
		}
	}

	// Token: 0x060000B9 RID: 185 RVA: 0x00007FD0 File Offset: 0x000061D0
	public static void Render(this HighlightsFXStandalone fx, Renderer renderer, bool enabled)
	{
		bool flag = !(fx == null) && !(renderer == null);
		if (flag)
		{
			if (enabled)
			{
				fx.field_Protected_HashSet_1_MeshFilter_0.Add(renderer.GetComponent<MeshFilter>());
			}
			else
			{
				fx.field_Protected_HashSet_1_MeshFilter_0.Remove(renderer.GetComponent<MeshFilter>());
			}
		}
	}

	// Token: 0x060000BA RID: 186 RVA: 0x0000802C File Offset: 0x0000622C
	public static void Render(this HighlightsFXStandalone fx, MeshFilter renderer, bool enabled)
	{
		bool flag = !(fx == null) && !(renderer == null);
		if (flag)
		{
			if (enabled)
			{
				fx.field_Protected_HashSet_1_MeshFilter_0.Add(renderer);
			}
			else
			{
				fx.field_Protected_HashSet_1_MeshFilter_0.Remove(renderer);
			}
		}
	}

	// Token: 0x060000BB RID: 187 RVA: 0x0000246F File Offset: 0x0000066F
	public static void Toast(string content, string type = "standard")
	{
	}

	// Token: 0x04000098 RID: 152
	private static ModalAlert _hudNotification;
}
