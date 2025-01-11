using System;
using System.Collections.Generic;
using UnityEngine;

namespace KernelClient.Modules.ESP
{
	// Token: 0x020000B6 RID: 182
	public class BatchLineRenderer : MonoBehaviour
	{
		// Token: 0x060004F9 RID: 1273 RVA: 0x0001C444 File Offset: 0x0001A644
		public void Initialize(Material mat, Color col)
		{
			this.material = mat;
			this.color = col;
			this.lineRenderer = base.gameObject.AddComponent<LineRenderer>();
			bool flag = this.lineRenderer == null;
			if (flag)
			{
				Debug.LogError("[BatchLineRenderer] Failed to add LineRenderer component.");
			}
			else
			{
				this.lineRenderer.material = this.material;
				this.lineRenderer.startColor = this.color;
				this.lineRenderer.endColor = this.color;
				this.lineRenderer.startWidth = 0.005f;
				this.lineRenderer.endWidth = 0.005f;
				this.lineRenderer.positionCount = 0;
				this.lineRenderer.useWorldSpace = true;
				this.lineRenderer.sortingOrder = 10;
				this.lineRenderer.shadowCastingMode = 0;
				this.lineRenderer.receiveShadows = false;
				Debug.Log(string.Format("[BatchLineRenderer] Initialized with Material: {0}, Color: {1}", this.material.name, this.color));
			}
		}

		// Token: 0x060004FA RID: 1274 RVA: 0x00003EEF File Offset: 0x000020EF
		public void AddLine(Vector3 start, Vector3 end)
		{
			this.positions.Add(start);
			this.positions.Add(end);
		}

		// Token: 0x060004FB RID: 1275 RVA: 0x00003F0C File Offset: 0x0000210C
		public void ClearLines()
		{
			this.positions.Clear();
		}

		// Token: 0x060004FC RID: 1276 RVA: 0x0001C55C File Offset: 0x0001A75C
		public void UpdateLines()
		{
			bool flag = this.positions.Count > 0;
			if (flag)
			{
				this.lineRenderer.positionCount = this.positions.Count;
				this.lineRenderer.SetPositions(this.positions.ToArray());
			}
			else
			{
				this.lineRenderer.positionCount = 0;
			}
		}

		// Token: 0x04000351 RID: 849
		private LineRenderer lineRenderer;

		// Token: 0x04000352 RID: 850
		private List<Vector3> positions = new List<Vector3>();

		// Token: 0x04000353 RID: 851
		private Material material;

		// Token: 0x04000354 RID: 852
		private Color color;
	}
}
