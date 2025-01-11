using System;
using System.Collections.Generic;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

// Token: 0x0200000E RID: 14
public static class HighlightsFXExtensions
{
	// Token: 0x06000040 RID: 64 RVA: 0x00004FB4 File Offset: 0x000031B4
	public static void AddRenderer(this HighlightsFX fx, Renderer renderer, bool enabled = true)
	{
		bool flag = fx == null || renderer == null;
		if (!flag)
		{
			try
			{
				SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
				bool flag2 = skinnedMeshRenderer != null;
				if (flag2)
				{
					HighlightsFXExtensions.HandleSkinnedMeshRenderer(fx, skinnedMeshRenderer, enabled);
				}
				else
				{
					HighlightsFXExtensions.HandleRegularRenderer(fx, renderer, enabled);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("[HighlightsFXExtensions] Error in AddRenderer: {0}", ex));
			}
		}
	}

	// Token: 0x06000041 RID: 65 RVA: 0x00005034 File Offset: 0x00003234
	public static void RemoveRenderer(this HighlightsFX fx, Renderer renderer)
	{
		bool flag = fx == null || renderer == null;
		if (!flag)
		{
			try
			{
				SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
				bool flag2 = skinnedMeshRenderer != null;
				if (flag2)
				{
					HighlightsFXExtensions.SkinnedMeshData skinnedMeshData;
					bool flag3 = HighlightsFXExtensions._skinnedMeshData.TryGetValue(skinnedMeshRenderer, out skinnedMeshData);
					if (flag3)
					{
						bool flag4 = skinnedMeshData.TempFilter != null;
						if (flag4)
						{
							HashSet<MeshFilter> field_Protected_HashSet_1_MeshFilter_ = fx.field_Protected_HashSet_1_MeshFilter_0;
							if (field_Protected_HashSet_1_MeshFilter_ != null)
							{
								field_Protected_HashSet_1_MeshFilter_.Remove(skinnedMeshData.TempFilter);
							}
							Object.Destroy(skinnedMeshData.TempFilter);
						}
						HighlightsFXExtensions._skinnedMeshData.Remove(skinnedMeshRenderer);
					}
				}
				else
				{
					MeshFilter component = renderer.GetComponent<MeshFilter>();
					bool flag5 = component != null;
					if (flag5)
					{
						HashSet<MeshFilter> field_Protected_HashSet_1_MeshFilter_2 = fx.field_Protected_HashSet_1_MeshFilter_0;
						if (field_Protected_HashSet_1_MeshFilter_2 != null)
						{
							field_Protected_HashSet_1_MeshFilter_2.Remove(component);
						}
						HighlightsFXExtensions.RestoreOriginalMesh(component);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("[HighlightsFXExtensions] Error in RemoveRenderer: {0}", ex));
			}
		}
	}

	// Token: 0x06000042 RID: 66 RVA: 0x00005134 File Offset: 0x00003334
	private static void HandleSkinnedMeshRenderer(HighlightsFX fx, SkinnedMeshRenderer smr, bool enabled)
	{
		bool flag = !enabled;
		if (flag)
		{
			fx.RemoveRenderer(smr);
		}
		else
		{
			HighlightsFXExtensions.SkinnedMeshData skinnedMeshData;
			bool flag2 = HighlightsFXExtensions._skinnedMeshData.TryGetValue(smr, out skinnedMeshData);
			if (flag2)
			{
				bool flag3 = skinnedMeshData.TempFilter != null;
				if (flag3)
				{
					HashSet<MeshFilter> field_Protected_HashSet_1_MeshFilter_ = fx.field_Protected_HashSet_1_MeshFilter_0;
					if (field_Protected_HashSet_1_MeshFilter_ != null)
					{
						field_Protected_HashSet_1_MeshFilter_.Add(skinnedMeshData.TempFilter);
					}
					return;
				}
			}
			GameObject gameObject = new GameObject("HighlightMesh_" + smr.name);
			gameObject.transform.SetParent(smr.transform, false);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			Mesh mesh = new Mesh();
			smr.BakeMesh(mesh);
			meshFilter.sharedMesh = mesh;
			HighlightsFXExtensions._skinnedMeshData[smr] = new HighlightsFXExtensions.SkinnedMeshData(smr.sharedMesh, meshFilter);
			HashSet<MeshFilter> field_Protected_HashSet_1_MeshFilter_2 = fx.field_Protected_HashSet_1_MeshFilter_0;
			if (field_Protected_HashSet_1_MeshFilter_2 != null)
			{
				field_Protected_HashSet_1_MeshFilter_2.Add(meshFilter);
			}
		}
	}

	// Token: 0x06000043 RID: 67 RVA: 0x00005244 File Offset: 0x00003444
	private static void HandleRegularRenderer(HighlightsFX fx, Renderer renderer, bool enabled)
	{
		MeshFilter component = renderer.GetComponent<MeshFilter>();
		bool flag = component == null;
		if (!flag)
		{
			if (enabled)
			{
				bool flag2 = !HighlightsFXExtensions._originalMeshes.ContainsKey(component);
				if (flag2)
				{
					HighlightsFXExtensions._originalMeshes[component] = component.sharedMesh;
				}
				HashSet<MeshFilter> field_Protected_HashSet_1_MeshFilter_ = fx.field_Protected_HashSet_1_MeshFilter_0;
				if (field_Protected_HashSet_1_MeshFilter_ != null)
				{
					field_Protected_HashSet_1_MeshFilter_.Add(component);
				}
			}
			else
			{
				HashSet<MeshFilter> field_Protected_HashSet_1_MeshFilter_2 = fx.field_Protected_HashSet_1_MeshFilter_0;
				if (field_Protected_HashSet_1_MeshFilter_2 != null)
				{
					field_Protected_HashSet_1_MeshFilter_2.Remove(component);
				}
				HighlightsFXExtensions.RestoreOriginalMesh(component);
			}
		}
	}

	// Token: 0x06000044 RID: 68 RVA: 0x000052C4 File Offset: 0x000034C4
	private static void RestoreOriginalMesh(MeshFilter meshFilter)
	{
		Mesh mesh;
		bool flag = meshFilter != null && HighlightsFXExtensions._originalMeshes.TryGetValue(meshFilter, out mesh);
		if (flag)
		{
			meshFilter.sharedMesh = mesh;
			HighlightsFXExtensions._originalMeshes.Remove(meshFilter);
		}
	}

	// Token: 0x06000045 RID: 69 RVA: 0x00005308 File Offset: 0x00003508
	public static void ClearAll(this HighlightsFX fx)
	{
		bool flag = fx == null;
		if (!flag)
		{
			try
			{
				foreach (KeyValuePair<MeshFilter, Mesh> keyValuePair in HighlightsFXExtensions._originalMeshes)
				{
					bool flag2 = keyValuePair.Key != null;
					if (flag2)
					{
						keyValuePair.Key.sharedMesh = keyValuePair.Value;
					}
				}
				HighlightsFXExtensions._originalMeshes.Clear();
				foreach (KeyValuePair<SkinnedMeshRenderer, HighlightsFXExtensions.SkinnedMeshData> keyValuePair2 in HighlightsFXExtensions._skinnedMeshData)
				{
					bool flag3 = keyValuePair2.Value.TempFilter != null;
					if (flag3)
					{
						HashSet<MeshFilter> field_Protected_HashSet_1_MeshFilter_ = fx.field_Protected_HashSet_1_MeshFilter_0;
						if (field_Protected_HashSet_1_MeshFilter_ != null)
						{
							field_Protected_HashSet_1_MeshFilter_.Remove(keyValuePair2.Value.TempFilter);
						}
						Object.Destroy(keyValuePair2.Value.TempFilter.gameObject);
					}
				}
				HighlightsFXExtensions._skinnedMeshData.Clear();
				HashSet<MeshFilter> field_Protected_HashSet_1_MeshFilter_2 = fx.field_Protected_HashSet_1_MeshFilter_0;
				if (field_Protected_HashSet_1_MeshFilter_2 != null)
				{
					field_Protected_HashSet_1_MeshFilter_2.Clear();
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("[HighlightsFXExtensions] Error in ClearAll: {0}", ex));
			}
		}
	}

	// Token: 0x04000060 RID: 96
	private static readonly Dictionary<SkinnedMeshRenderer, HighlightsFXExtensions.SkinnedMeshData> _skinnedMeshData = new Dictionary<SkinnedMeshRenderer, HighlightsFXExtensions.SkinnedMeshData>();

	// Token: 0x04000061 RID: 97
	private static readonly Dictionary<MeshFilter, Mesh> _originalMeshes = new Dictionary<MeshFilter, Mesh>();

	// Token: 0x0200000F RID: 15
	private class SkinnedMeshData
	{
		// Token: 0x06000047 RID: 71 RVA: 0x00002215 File Offset: 0x00000415
		public SkinnedMeshData(Mesh originalMesh, MeshFilter tempFilter)
		{
			this.OriginalMesh = originalMesh;
			this.TempFilter = tempFilter;
		}

		// Token: 0x04000062 RID: 98
		public Mesh OriginalMesh;

		// Token: 0x04000063 RID: 99
		public MeshFilter TempFilter;
	}
}
