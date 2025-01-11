using System;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace KernelClient.Features
{
	// Token: 0x0200007C RID: 124
	public class HighlightsFXSkinned : PostEffectsBase
	{
		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000344 RID: 836 RVA: 0x00011974 File Offset: 0x0000FB74
		public static HighlightsFXSkinned Instance
		{
			get
			{
				HighlightsFXSkinned highlightsFXSkinned;
				try
				{
					bool flag = HighlightsFXSkinned._instance == null;
					if (flag)
					{
						GameObject gameObject = new GameObject("HighlightsFXSkinned");
						HighlightsFXSkinned._instance = gameObject.AddComponent<HighlightsFXSkinned>();
						Object.DontDestroyOnLoad(gameObject);
						Debug.Log("HighlightsFXSkinned instance created.");
					}
					highlightsFXSkinned = HighlightsFXSkinned._instance;
				}
				catch (Exception ex)
				{
					Debug.LogError(string.Format("Failed to create HighlightsFXSkinned instance: {0}", ex));
					highlightsFXSkinned = null;
				}
				return highlightsFXSkinned;
			}
		}

		// Token: 0x06000345 RID: 837 RVA: 0x000031D3 File Offset: 0x000013D3
		public HighlightsFXSkinned()
		{
			Debug.Log("HighlightsFXSkinned constructor called.");
		}

		// Token: 0x06000346 RID: 838 RVA: 0x000119F8 File Offset: 0x0000FBF8
		public void Awake()
		{
			Debug.Log("HighlightsFXSkinned Awake called.");
			this._skinnedRenderers = new HashSet<SkinnedMeshRenderer>();
			this._tempMeshFilters = new Dictionary<SkinnedMeshRenderer, MeshFilter>();
			this._highlightShader = Shader.Find("Hidden/Highlighted/StencilOpaque");
			bool flag = this._highlightShader != null;
			if (flag)
			{
				this._highlightMaterial = new Material(this._highlightShader);
				this._highlightMaterial.hideFlags = 61;
				Debug.Log("Highlight material created successfully.");
			}
			else
			{
				Debug.LogError("Failed to find shader 'Hidden/Highlighted/StencilOpaque'.");
			}
		}

		// Token: 0x06000347 RID: 839 RVA: 0x00011A94 File Offset: 0x0000FC94
		public void OnDestroy()
		{
			try
			{
				Debug.Log("HighlightsFXSkinned OnDestroy called.");
				bool flag = this._highlightMaterial != null;
				if (flag)
				{
					Object.Destroy(this._highlightMaterial);
				}
				this.CleanupAllMeshes();
				base.OnDestroy();
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Error during HighlightsFXSkinned OnDestroy: {0}", ex));
			}
		}

		// Token: 0x06000348 RID: 840 RVA: 0x00011B0C File Offset: 0x0000FD0C
		public void AddRenderer(SkinnedMeshRenderer renderer)
		{
			bool flag = renderer == null;
			if (!flag)
			{
				bool flag2 = !this._skinnedRenderers.Contains(renderer);
				if (flag2)
				{
					this._skinnedRenderers.Add(renderer);
					this.CreateTempMeshFilter(renderer);
					Debug.Log("Renderer added: " + renderer.name);
				}
			}
		}

		// Token: 0x06000349 RID: 841 RVA: 0x00011B6C File Offset: 0x0000FD6C
		public void RemoveRenderer(SkinnedMeshRenderer renderer)
		{
			bool flag = renderer == null;
			if (!flag)
			{
				bool flag2 = this._skinnedRenderers.Remove(renderer);
				if (flag2)
				{
					this.CleanupTempMeshFilter(renderer);
					Debug.Log("Renderer removed: " + renderer.name);
				}
			}
		}

		// Token: 0x0600034A RID: 842 RVA: 0x00011BBC File Offset: 0x0000FDBC
		private void CreateTempMeshFilter(SkinnedMeshRenderer skinnedRenderer)
		{
			bool flag = this._tempMeshFilters.ContainsKey(skinnedRenderer);
			if (!flag)
			{
				GameObject gameObject = new GameObject("TempMeshFilter_" + skinnedRenderer.name);
				gameObject.transform.SetParent(skinnedRenderer.transform);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
				Mesh mesh = new Mesh();
				mesh.MarkDynamic();
				skinnedRenderer.BakeMesh(mesh);
				meshFilter.sharedMesh = mesh;
				this._tempMeshFilters[skinnedRenderer] = meshFilter;
				Debug.Log("TempMeshFilter created for: " + skinnedRenderer.name);
			}
		}

		// Token: 0x0600034B RID: 843 RVA: 0x00011C88 File Offset: 0x0000FE88
		private void CleanupTempMeshFilter(SkinnedMeshRenderer renderer)
		{
			MeshFilter meshFilter;
			bool flag = this._tempMeshFilters.TryGetValue(renderer, ref meshFilter);
			if (flag)
			{
				bool flag2 = meshFilter != null;
				if (flag2)
				{
					bool flag3 = meshFilter.sharedMesh != null;
					if (flag3)
					{
						Object.Destroy(meshFilter.sharedMesh);
					}
					Object.Destroy(meshFilter.gameObject);
					Debug.Log("TempMeshFilter destroyed for: " + renderer.name);
				}
				this._tempMeshFilters.Remove(renderer);
			}
		}

		// Token: 0x0600034C RID: 844 RVA: 0x00011D08 File Offset: 0x0000FF08
		public void CleanupAllMeshes()
		{
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in this._skinnedRenderers)
			{
				this.CleanupTempMeshFilter(skinnedMeshRenderer);
			}
			this._skinnedRenderers.Clear();
			this._tempMeshFilters.Clear();
			Debug.Log("All temporary meshes cleaned up.");
		}

		// Token: 0x0600034D RID: 845 RVA: 0x00011D68 File Offset: 0x0000FF68
		public void UpdateMeshes()
		{
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in this._skinnedRenderers)
			{
				MeshFilter meshFilter;
				bool flag = skinnedMeshRenderer != null && this._tempMeshFilters.TryGetValue(skinnedMeshRenderer, ref meshFilter);
				if (flag)
				{
					bool flag2 = meshFilter != null && meshFilter.sharedMesh != null;
					if (flag2)
					{
						skinnedMeshRenderer.BakeMesh(meshFilter.sharedMesh);
						meshFilter.transform.position = skinnedMeshRenderer.transform.position;
						meshFilter.transform.rotation = skinnedMeshRenderer.transform.rotation;
						Debug.Log("Mesh updated for: " + skinnedMeshRenderer.name);
					}
				}
			}
		}

		// Token: 0x0600034E RID: 846 RVA: 0x00011E34 File Offset: 0x00010034
		public void Start()
		{
			try
			{
				Debug.Log("HighlightsFXSkinned Start called.");
				base.Start();
				this.Awake();
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Error during HighlightsFXSkinned Start: {0}", ex));
			}
		}

		// Token: 0x0600034F RID: 847 RVA: 0x000031ED File Offset: 0x000013ED
		public string ToString()
		{
			return "HighlightsFXSkinned";
		}

		// Token: 0x06000350 RID: 848 RVA: 0x00011E90 File Offset: 0x00010090
		public bool CheckResources()
		{
			bool flag = this._highlightMaterial != null && this._highlightShader != null;
			Debug.Log(string.Format("CheckResources: {0} (Material), {1} (Shader)", this._highlightMaterial != null, this._highlightShader != null));
			return flag;
		}

		// Token: 0x06000351 RID: 849 RVA: 0x00011EF8 File Offset: 0x000100F8
		protected bool CheckSupport()
		{
			bool flag = !SystemInfo.supportsImageEffects;
			bool flag2;
			if (flag)
			{
				base.enabled = false;
				Debug.LogWarning("Image effects not supported on this platform.");
				flag2 = false;
			}
			else
			{
				Debug.Log("Image effects are supported.");
				flag2 = true;
			}
			return flag2;
		}

		// Token: 0x06000352 RID: 850 RVA: 0x00011F44 File Offset: 0x00010144
		public void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			try
			{
				bool flag = !this.CheckResources() || this._skinnedRenderers.Count == 0;
				if (flag)
				{
					Graphics.Blit(source, destination);
					Debug.Log("CheckResources failed or no renderers to process. Blitting source to destination.");
				}
				else
				{
					this.UpdateMeshes();
					Graphics.Blit(source, destination, this._highlightMaterial);
					Debug.Log("Highlights rendered successfully.");
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Error during OnRenderImage: {0}", ex));
				Graphics.Blit(source, destination);
			}
		}

		// Token: 0x0400021B RID: 539
		private static HighlightsFXSkinned _instance;

		// Token: 0x0400021C RID: 540
		private HashSet<SkinnedMeshRenderer> _skinnedRenderers;

		// Token: 0x0400021D RID: 541
		private Dictionary<SkinnedMeshRenderer, MeshFilter> _tempMeshFilters;

		// Token: 0x0400021E RID: 542
		private Material _highlightMaterial;

		// Token: 0x0400021F RID: 543
		private Shader _highlightShader;
	}
}
