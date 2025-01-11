using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;

namespace KernelClient.Settings.Wrappers
{
	// Token: 0x0200006A RID: 106
	public class WavesEffectManager
	{
		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060002D7 RID: 727 RVA: 0x0000F99C File Offset: 0x0000DB9C
		public static WavesEffectManager Instance
		{
			get
			{
				bool flag = WavesEffectManager._instance == null;
				if (flag)
				{
					WavesEffectManager._instance = new WavesEffectManager();
				}
				return WavesEffectManager._instance;
			}
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x0000305A File Offset: 0x0000125A
		private WavesEffectManager()
		{
			MelonEvents.OnUpdate.Subscribe(new LemonAction(this.OnUpdate), 0, false);
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x0000F9CC File Offset: 0x0000DBCC
		~WavesEffectManager()
		{
			MelonEvents.OnUpdate.Unsubscribe(new LemonAction(this.OnUpdate));
		}

		// Token: 0x060002DA RID: 730 RVA: 0x0000FA0C File Offset: 0x0000DC0C
		public void RegisterWaveMesh(MeshFilter meshFilter, float amplitude, float frequency, float speed)
		{
			bool flag = meshFilter == null;
			if (flag)
			{
				MelonLogger.Error("WavesEffectManager: RegisterWaveMesh called with null MeshFilter.");
			}
			else
			{
				bool flag2 = this._activeWaveMeshes.Exists((WavesEffectManager.WaveMeshData wm) => wm.MeshFilter == meshFilter);
				if (flag2)
				{
					MelonLogger.Warning("WavesEffectManager: MeshFilter " + meshFilter.gameObject.name + " is already registered for wave effects.");
				}
				else
				{
					WavesEffectManager.WaveMeshData waveMeshData = new WavesEffectManager.WaveMeshData(meshFilter, amplitude, frequency, speed);
					this._activeWaveMeshes.Add(waveMeshData);
					MelonLogger.Msg("WavesEffectManager: Registered wave effect on " + meshFilter.gameObject.name + ".");
				}
			}
		}

		// Token: 0x060002DB RID: 731 RVA: 0x0000FAD0 File Offset: 0x0000DCD0
		public void UnregisterWaveMesh(MeshFilter meshFilter)
		{
			bool flag = meshFilter == null;
			if (flag)
			{
				MelonLogger.Error("WavesEffectManager: UnregisterWaveMesh called with null MeshFilter.");
			}
			else
			{
				int num = this._activeWaveMeshes.RemoveAll((WavesEffectManager.WaveMeshData wm) => wm.MeshFilter == meshFilter);
				bool flag2 = num > 0;
				if (flag2)
				{
					MelonLogger.Msg("WavesEffectManager: Unregistered wave effect from " + meshFilter.gameObject.name + ".");
				}
				else
				{
					MelonLogger.Warning("WavesEffectManager: MeshFilter " + meshFilter.gameObject.name + " was not registered for wave effects.");
				}
			}
		}

		// Token: 0x060002DC RID: 732 RVA: 0x0000FB7C File Offset: 0x0000DD7C
		private void OnUpdate()
		{
			foreach (WavesEffectManager.WaveMeshData waveMeshData in this._activeWaveMeshes)
			{
				bool flag = waveMeshData.MeshFilter == null || waveMeshData.MeshFilter.mesh == null;
				if (flag)
				{
					string text = "WavesEffectManager: MeshFilter ";
					MeshFilter meshFilter = waveMeshData.MeshFilter;
					MelonLogger.Warning(text + ((meshFilter != null) ? meshFilter.gameObject.name : null) + " is null or has no mesh.");
				}
				else
				{
					Vector3[] array = new Vector3[waveMeshData.OriginalVertices.Length];
					Array.Copy(waveMeshData.OriginalVertices, array, waveMeshData.OriginalVertices.Length);
					for (int i = 0; i < array.Length; i++)
					{
						Vector3 vector = array[i];
						vector.x += Mathf.Sin(Time.time * waveMeshData.Speed + vector.z * waveMeshData.Frequency + waveMeshData.TimeOffset) * waveMeshData.Amplitude;
						vector.y += Mathf.Sin(Time.time * waveMeshData.Speed + vector.x * waveMeshData.Frequency + waveMeshData.TimeOffset) * waveMeshData.Amplitude;
						vector.z += Mathf.Sin(Time.time * waveMeshData.Speed + vector.y * waveMeshData.Frequency + waveMeshData.TimeOffset) * waveMeshData.Amplitude;
						array[i] = vector;
					}
					waveMeshData.MeshFilter.mesh.vertices = array;
					waveMeshData.MeshFilter.mesh.RecalculateNormals();
				}
			}
		}

		// Token: 0x060002DD RID: 733 RVA: 0x00003088 File Offset: 0x00001288
		public void ClearAllWaveEffects()
		{
			this._activeWaveMeshes.Clear();
			MelonLogger.Msg("WavesEffectManager: Cleared all wave effects and restored original meshes.");
		}

		// Token: 0x040001B7 RID: 439
		private static WavesEffectManager _instance;

		// Token: 0x040001B8 RID: 440
		private readonly List<WavesEffectManager.WaveMeshData> _activeWaveMeshes = new List<WavesEffectManager.WaveMeshData>();

		// Token: 0x0200006B RID: 107
		private struct WaveMeshData
		{
			// Token: 0x060002DE RID: 734 RVA: 0x0000FD64 File Offset: 0x0000DF64
			public WaveMeshData(MeshFilter meshFilter, float amplitude, float frequency, float speed)
			{
				this.MeshFilter = meshFilter;
				this.OriginalVertices = meshFilter.mesh.vertices;
				this.Amplitude = amplitude;
				this.Frequency = frequency;
				this.Speed = speed;
				this.TimeOffset = Random.Range(0f, 6.2831855f);
			}

			// Token: 0x040001B9 RID: 441
			public MeshFilter MeshFilter;

			// Token: 0x040001BA RID: 442
			public Vector3[] OriginalVertices;

			// Token: 0x040001BB RID: 443
			public float Amplitude;

			// Token: 0x040001BC RID: 444
			public float Frequency;

			// Token: 0x040001BD RID: 445
			public float Speed;

			// Token: 0x040001BE RID: 446
			public float TimeOffset;
		}
	}
}
