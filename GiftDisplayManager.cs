using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
using ReMod.Core.UI.QuickMenu;
using UnhollowerBaseLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KernelClient
{
	// Token: 0x02000047 RID: 71
	public class GiftDisplayManager : KernelModule
	{
		// Token: 0x060001AE RID: 430 RVA: 0x0000B874 File Offset: 0x00009A74
		public override void OnUiManagerInit()
		{
			IButtonPage qmmenu = MenuSetup._uiManager.QMMenu;
			qmmenu.AddToggle("Gift Claim", "Toggle gift Claim", new Action<bool>(this.ToggleGiftDisplay), this._isEnabled, "#ffffff");
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0000B8B8 File Offset: 0x00009AB8
		private void StartCoroutines()
		{
			bool flag = this._scanCoroutine == null;
			if (flag)
			{
				this._scanCoroutine = MelonCoroutines.Start(this.ScanForGiftsCoroutine());
			}
			bool flag2 = this._processCoroutine == null;
			if (flag2)
			{
				this._processCoroutine = MelonCoroutines.Start(this.ProcessGiftQueueCoroutine());
			}
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0000B904 File Offset: 0x00009B04
		private void StopCoroutines()
		{
			bool flag = this._scanCoroutine != null;
			if (flag)
			{
				MelonCoroutines.Stop(this._scanCoroutine);
				this._scanCoroutine = null;
			}
			bool flag2 = this._processCoroutine != null;
			if (flag2)
			{
				MelonCoroutines.Stop(this._processCoroutine);
				this._processCoroutine = null;
			}
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000299B File Offset: 0x00000B9B
		private IEnumerator ScanForGiftsCoroutine()
		{
			WaitForSeconds waitInterval = new WaitForSeconds(0.5f);
			for (;;)
			{
				bool isEnabled = this._isEnabled;
				if (isEnabled)
				{
					this._currentScene = SceneManager.GetActiveScene();
					Il2CppReferenceArray<GameObject> rootObjects = this._currentScene.GetRootGameObjects();
					foreach (GameObject root in rootObjects)
					{
						bool flag = root == null;
						if (!flag)
						{
							List<GameObject> gifts = (from t in root.GetComponentsInChildren<Transform>(true)
								where t != null && t.gameObject != null && t.name == "Gift Display Prefab(Clone)" && !this._claimedGifts.Contains(t.gameObject.GetInstanceID().ToString())
								select t.gameObject).ToList<GameObject>();
							foreach (GameObject gift in gifts)
							{
								bool flag2 = !this._giftQueue.Contains(gift);
								if (flag2)
								{
									this._giftQueue.Enqueue(gift);
									MelonLogger.Msg(string.Format("New gift found! Queue size: {0}", this._giftQueue.Count));
								}
								gift = null;
							}
							List<GameObject>.Enumerator enumerator2 = default(List<GameObject>.Enumerator);
							yield return null;
							gifts = null;
							root = null;
						}
					}
					IEnumerator<GameObject> enumerator = null;
					rootObjects = null;
				}
				yield return waitInterval;
			}
			yield break;
			yield break;
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x000029AA File Offset: 0x00000BAA
		private IEnumerator ProcessGiftQueueCoroutine()
		{
			WaitForSeconds waitTime = new WaitForSeconds(0.5f);
			for (;;)
			{
				bool flag = this._isEnabled && this._giftQueue.Count > 0 && Time.time - this._lastClaimTime >= 0.5f;
				if (flag)
				{
					this.ProcessNextGift();
				}
				yield return waitTime;
			}
			yield break;
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000B958 File Offset: 0x00009B58
		private void ProcessNextGift()
		{
			bool flag = this._giftQueue.Count == 0;
			if (!flag)
			{
				GameObject gameObject = this._giftQueue.Peek();
				bool flag2 = gameObject == null || gameObject.IsDestroyed();
				if (flag2)
				{
					this._giftQueue.Dequeue();
					MelonLogger.Warning("Removed invalid gift from queue");
				}
				else
				{
					try
					{
						VRCPlayer field_Internal_Static_VRCPlayer_ = VRCPlayer.field_Internal_Static_VRCPlayer_0;
						bool flag3 = field_Internal_Static_VRCPlayer_ != null;
						if (flag3)
						{
							field_Internal_Static_VRCPlayer_.transform.position = gameObject.transform.position;
							this._claimedGifts.Add(gameObject.GetInstanceID().ToString());
							this._giftQueue.Dequeue();
							this._lastClaimTime = Time.time;
							ToastNotif.Toast(string.Format("Gift claimed ({0} total)", this._claimedGifts.Count), null, null, 5f);
							MelonLogger.Msg(string.Format("Successfully claimed gift. Remaining in queue: {0}", this._giftQueue.Count));
						}
					}
					catch (Exception ex)
					{
						MelonLogger.Error("Error processing gift: " + ex.Message);
						this._giftQueue.Dequeue();
					}
				}
			}
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x0000BAA4 File Offset: 0x00009CA4
		private void ToggleGiftDisplay(bool state)
		{
			this._isEnabled = state;
			if (state)
			{
				this._activeGiftCache.Clear();
				this.StartCoroutines();
				MelonLogger.Msg("Gift Display Manager enabled");
			}
			else
			{
				this.StopCoroutines();
				this._giftQueue.Clear();
				this._activeGiftCache.Clear();
				MelonLogger.Msg("Gift Display Manager disabled");
			}
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x000029B9 File Offset: 0x00000BB9
		public void ClearClaimedGiftsHistory()
		{
			this._claimedGifts.Clear();
			ToastNotif.Toast("Claimed gifts history cleared", null, null, 5f);
			MelonLogger.Msg("Cleared claimed gifts history");
		}

		// Token: 0x0400011B RID: 283
		private bool _isEnabled = true;

		// Token: 0x0400011C RID: 284
		private HashSet<string> _claimedGifts = new HashSet<string>();

		// Token: 0x0400011D RID: 285
		private Queue<GameObject> _giftQueue = new Queue<GameObject>();

		// Token: 0x0400011E RID: 286
		private float _lastClaimTime = 0f;

		// Token: 0x0400011F RID: 287
		private const float CLAIM_COOLDOWN = 0.5f;

		// Token: 0x04000120 RID: 288
		private const float SCAN_INTERVAL = 0.5f;

		// Token: 0x04000121 RID: 289
		private object _scanCoroutine;

		// Token: 0x04000122 RID: 290
		private object _processCoroutine;

		// Token: 0x04000123 RID: 291
		private readonly Dictionary<string, List<GameObject>> _activeGiftCache = new Dictionary<string, List<GameObject>>();

		// Token: 0x04000124 RID: 292
		private Scene _currentScene;
	}
}
