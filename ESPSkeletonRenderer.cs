using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000016 RID: 22
internal class ESPSkeletonRenderer : MonoBehaviour
{
	// Token: 0x0600007A RID: 122 RVA: 0x000022EB File Offset: 0x000004EB
	private void Start()
	{
		this.EnsureLineMaterialExists();
	}

	// Token: 0x0600007B RID: 123 RVA: 0x00006E60 File Offset: 0x00005060
	private void EnsureLineMaterialExists()
	{
		bool flag = ESPSkeletonRenderer.lineMaterial == null;
		if (flag)
		{
			ESPSkeletonRenderer.lineMaterial = new Material(Shader.Find("Unlit/Color"))
			{
				hideFlags = 61
			};
			ESPSkeletonRenderer.lineMaterial.SetInt("_SrcBlend", 5);
			ESPSkeletonRenderer.lineMaterial.SetInt("_DstBlend", 10);
			ESPSkeletonRenderer.lineMaterial.SetInt("_ZWrite", 0);
			ESPSkeletonRenderer.lineMaterial.renderQueue = 5000;
		}
	}

	// Token: 0x0600007C RID: 124 RVA: 0x000022F5 File Offset: 0x000004F5
	public static void ClearAllPlayers()
	{
		ESPSkeletonRenderer.playerBones.Clear();
	}

	// Token: 0x0600007D RID: 125 RVA: 0x00002303 File Offset: 0x00000503
	public static void RemovePlayer(string id)
	{
		ESPSkeletonRenderer.playerBones.Remove(id);
	}

	// Token: 0x0600007E RID: 126 RVA: 0x00006EE0 File Offset: 0x000050E0
	public static void SetPlayerBones(string id, List<Tuple<Vector3, Vector3>> bones)
	{
		bool flag = !string.IsNullOrEmpty(id) && bones != null;
		if (flag)
		{
			ESPSkeletonRenderer.playerBones[id] = bones;
		}
	}

	// Token: 0x0600007F RID: 127 RVA: 0x00006F10 File Offset: 0x00005110
	private void OnPostRender()
	{
		bool flag = ESPSkeletonRenderer.playerBones.Count == 0 || ESPSkeletonRenderer.lineMaterial == null;
		if (!flag)
		{
			ESPSkeletonRenderer.lineMaterial.SetPass(0);
			GL.PushMatrix();
			GL.LoadProjectionMatrix(Camera.current.projectionMatrix);
			GL.modelview = Camera.current.worldToCameraMatrix;
			GL.Begin(1);
			GL.Color(Color.red);
			foreach (KeyValuePair<string, List<Tuple<Vector3, Vector3>>> keyValuePair in ESPSkeletonRenderer.playerBones)
			{
				foreach (Tuple<Vector3, Vector3> tuple in keyValuePair.Value)
				{
					GL.Vertex(tuple.Item1);
					GL.Vertex(tuple.Item2);
				}
			}
			GL.End();
			GL.PopMatrix();
		}
	}

	// Token: 0x04000081 RID: 129
	private static readonly Dictionary<string, List<Tuple<Vector3, Vector3>>> playerBones = new Dictionary<string, List<Tuple<Vector3, Vector3>>>();

	// Token: 0x04000082 RID: 130
	private static Material lineMaterial;
}
