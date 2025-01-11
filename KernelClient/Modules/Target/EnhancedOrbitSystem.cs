using System;
using System.Collections;
using System.Collections.Generic;
using MelonLoader;
using ReMod.Core.UI.QuickMenu;
using ReMod.Core.VRChat;
using UnityEngine;
using VRC;
using VRC.SDKBase;
using VRCApi;

namespace KernelClient.Modules.Target
{
	// Token: 0x02000091 RID: 145
	internal class EnhancedOrbitSystem : KernelModule
	{
		// Token: 0x060003F0 RID: 1008 RVA: 0x00015AD8 File Offset: 0x00013CD8
		public override void OnUiManagerInit()
		{
			this.InitializeConfigs();
			IButtonPage targetMenu = MenuSetup._uiManager.TargetMenu;
			ReCategoryPage reCategoryPage = targetMenu.AddCategoryPage("Orbit Controls", "", null, "#ffffff");
			ReMenuCategory reMenuCategory = reCategoryPage.AddCategory("Basic Patterns");
			ReMenuCategory reMenuCategory2 = reCategoryPage.AddCategory("Complex Patterns");
			ReMenuCategory reMenuCategory3 = reCategoryPage.AddCategory("Special Patterns");
			ReMenuCategory reMenuCategory4 = reCategoryPage.AddCategory("Stationary");
			ReMenuCategory reMenuCategory5 = reCategoryPage.AddCategory("Axis Settings");
			ReMenuCategory reMenuCategory6 = reCategoryPage.AddCategory("Settings");
			ReMenuCategory reMenuCategory7 = reCategoryPage.AddCategory("Effects");
			ReMenuCategory reMenuCategory8 = reCategoryPage.AddCategory("Utilities");
			ReMenuCategory reMenuCategory9 = reCategoryPage.AddCategory("Chaos Mode");
			this.AddPatternToggle(reMenuCategory, EnhancedOrbitSystem.OrbitMode.Circle);
			this.AddPatternToggle(reMenuCategory, EnhancedOrbitSystem.OrbitMode.Square);
			this.AddPatternToggle(reMenuCategory, EnhancedOrbitSystem.OrbitMode.Triangle);
			this.AddPatternToggle(reMenuCategory, EnhancedOrbitSystem.OrbitMode.Line);
			this.AddPatternToggle(reMenuCategory2, EnhancedOrbitSystem.OrbitMode.Spiral);
			this.AddPatternToggle(reMenuCategory2, EnhancedOrbitSystem.OrbitMode.Pentagon);
			this.AddPatternToggle(reMenuCategory2, EnhancedOrbitSystem.OrbitMode.Hexagon);
			this.AddPatternToggle(reMenuCategory2, EnhancedOrbitSystem.OrbitMode.Octagon);
			this.AddPatternToggle(reMenuCategory2, EnhancedOrbitSystem.OrbitMode.Infinity);
			this.AddPatternToggle(reMenuCategory2, EnhancedOrbitSystem.OrbitMode.DNA);
			this.AddPatternToggle(reMenuCategory2, EnhancedOrbitSystem.OrbitMode.Wave);
			this.AddPatternToggle(reMenuCategory2, EnhancedOrbitSystem.OrbitMode.Vortex);
			this.AddPatternToggle(reMenuCategory2, EnhancedOrbitSystem.OrbitMode.Atom);
			this.AddPatternToggle(reMenuCategory3, EnhancedOrbitSystem.OrbitMode.Heart);
			this.AddPatternToggle(reMenuCategory3, EnhancedOrbitSystem.OrbitMode.Star);
			this.AddPatternToggle(reMenuCategory3, EnhancedOrbitSystem.OrbitMode.Swas);
			this.AddPatternToggle(reMenuCategory4, EnhancedOrbitSystem.OrbitMode.SinglePoint);
			this.AddPatternToggle(reMenuCategory4, EnhancedOrbitSystem.OrbitMode.StaticCluster);
			using (IEnumerator enumerator = Enum.GetValues(typeof(EnhancedOrbitSystem.OrbitAxis)).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					EnhancedOrbitSystem.OrbitAxis axis = (EnhancedOrbitSystem.OrbitAxis)enumerator.Current;
					reMenuCategory5.AddToggle(string.Format("Axis {0}", axis), string.Format("Set orbit axis to {0}", axis), delegate(bool state)
					{
						bool flag = state && MenuEx.QMSelectedUserLocal.field_Private_IUser_0 != null;
						if (flag)
						{
							this._target = PlayerExtensions.GetPlayer(MenuEx.QMSelectedUserLocal.field_Private_IUser_0.Method_Public_Abstract_Virtual_New_get_String_0());
							this._currentAxis = axis;
						}
					}, this._currentAxis == axis);
				}
			}
			ReMenuSliderCategory reMenuSliderCategory = reMenuCategory6.AddCategoryPage("Basic Settings", "", null, "#ffffff").AddSliderCategory("Basic Orbit Controls");
			reMenuSliderCategory.AddSlider("Speed", "Adjust orbit speed", delegate(float val)
			{
				this._configs[this._currentMode].Speed = val;
			}, 1f, 0.1f, 10f, "#ffffff");
			reMenuSliderCategory.AddSlider("Radius", "Adjust orbit radius", delegate(float val)
			{
				this._configs[this._currentMode].Radius = val;
			}, 2f, 0.5f, 10f, "#ffffff");
			reMenuSliderCategory.AddSlider("Height", "Adjust orbit height", delegate(float val)
			{
				this._configs[this._currentMode].Height = val;
			}, 1f, -5f, 5f, "#ffffff");
			reMenuSliderCategory.AddSlider("Scale", "Adjust overall scale", delegate(float val)
			{
				this._configs[this._currentMode].Scale = val;
			}, 1f, 0.1f, 5f, "#ffffff");
			ReMenuSliderCategory reMenuSliderCategory2 = reMenuCategory6.AddCategoryPage("Wave Settings", "", null, "#ffffff").AddSliderCategory("Wave Controls");
			reMenuSliderCategory2.AddSlider("Wave Amplitude", "Adjust wave height", delegate(float val)
			{
				this._configs[this._currentMode].WaveAmplitude = val;
			}, 0.5f, 0f, 2f, "#ffffff");
			reMenuSliderCategory2.AddSlider("Wave Frequency", "Adjust wave frequency", delegate(float val)
			{
				this._configs[this._currentMode].WaveFrequency = val;
			}, 1f, 0.1f, 5f, "#ffffff");
			reMenuSliderCategory2.AddSlider("Rotation Speed", "Adjust rotation speed", delegate(float val)
			{
				this._configs[this._currentMode].RotationSpeed = val;
			}, 45f, 0f, 360f, "#ffffff");
			ReMenuSliderCategory reMenuSliderCategory3 = reMenuCategory6.AddCategoryPage("Swas Settings", "", null, "#ffffff").AddSliderCategory("Swas Controls");
			reMenuSliderCategory3.AddSlider("Swas Size", "Adjust swas pattern size", delegate(float val)
			{
				this._configs[EnhancedOrbitSystem.OrbitMode.Swas].SwasSize = val;
				this._swasSize = val;
			}, 45f, 5f, 100f, "#ffffff");
			reMenuSliderCategory3.AddSlider("Swas Spread", "Adjust swas spread", delegate(float val)
			{
				this._configs[EnhancedOrbitSystem.OrbitMode.Swas].SwasSpread = val;
			}, 1f, 0.5f, 3f, "#ffffff");
			reMenuSliderCategory3.AddSlider("Swas Height", "Adjust swas height", delegate(float val)
			{
				this._configs[EnhancedOrbitSystem.OrbitMode.Swas].SwasHeight = val;
			}, 2f, 0f, 5f, "#ffffff");
			reMenuSliderCategory3.AddSlider("Swas Rotation", "Adjust swas rotation speed", delegate(float val)
			{
				this._configs[EnhancedOrbitSystem.OrbitMode.Swas].SwasRotationSpeed = val;
			}, 1f, 0.1f, 5f, "#ffffff");
			reMenuCategory7.AddToggle("Reverse", "Reverse orbit direction", delegate(bool val)
			{
				this._configs[this._currentMode].Reverse = val;
			}, false);
			reMenuCategory7.AddToggle("Pulse", "Enable size pulsing effect", delegate(bool val)
			{
				this._configs[this._currentMode].Pulse = val;
			}, false);
			reMenuCategory7.AddToggle("Rainbow", "Enable rainbow color effect", delegate(bool val)
			{
				this._configs[this._currentMode].Rainbow = val;
			}, false);
			reMenuCategory7.AddToggle("Spin", "Enable item spinning", delegate(bool val)
			{
				this._configs[this._currentMode].Spin = val;
			}, false);
			reMenuCategory8.AddButton("Reset All", "Reset all settings to defaults", new Action(this.ResetAllSettings), null, "#ffffff");
			reMenuCategory8.AddButton("Randomize", "Randomize current pattern settings", new Action(this.RandomizeCurrentPattern), null, "#ffffff");
			reMenuCategory9.AddToggle("Chaos Mode", "Enable chaos mode (objects bounce between players)", delegate(bool val)
			{
				this._chaosMode = val;
				if (val)
				{
					MelonLogger.Msg("Chaos mode enabled!");
				}
				else
				{
					MelonLogger.Msg("Chaos mode disabled!");
				}
			}, false);
			ToastNotif.Toast("Orbit System Initialized", "All orbit controls ready!", EmbeddedResourceLoader.LoadEmbeddedSprite("KernelClient.assets.IOI.png"), 5f);
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x00016084 File Offset: 0x00014284
		private void AddPatternToggle(ReMenuCategory category, EnhancedOrbitSystem.OrbitMode mode)
		{
			this._modeToggles[mode] = category.AddToggle(mode.ToString(), string.Format("Enable {0} orbit pattern", mode), delegate(bool state)
			{
				bool flag = MenuEx.QMSelectedUserLocal.field_Private_IUser_0 != null;
				if (flag)
				{
					this._target = PlayerExtensions.GetPlayer(MenuEx.QMSelectedUserLocal.field_Private_IUser_0.Method_Public_Abstract_Virtual_New_get_String_0());
					this.OnModeToggle(mode, state);
				}
			}, false);
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x000160F4 File Offset: 0x000142F4
		private void InitializeConfigs()
		{
			this._configs = new Dictionary<EnhancedOrbitSystem.OrbitMode, EnhancedOrbitSystem.OrbitConfig>();
			this._modeToggles = new Dictionary<EnhancedOrbitSystem.OrbitMode, ReMenuToggle>();
			foreach (object obj in Enum.GetValues(typeof(EnhancedOrbitSystem.OrbitMode)))
			{
				EnhancedOrbitSystem.OrbitMode orbitMode = (EnhancedOrbitSystem.OrbitMode)obj;
				bool flag = orbitMode > EnhancedOrbitSystem.OrbitMode.None;
				if (flag)
				{
					this._configs[orbitMode] = new EnhancedOrbitSystem.OrbitConfig();
				}
			}
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x00016188 File Offset: 0x00014388
		private void OnModeToggle(EnhancedOrbitSystem.OrbitMode mode, bool state)
		{
			bool flag = MenuEx.QMSelectedUserLocal.field_Private_IUser_0 == null;
			if (!flag)
			{
				if (state)
				{
					foreach (KeyValuePair<EnhancedOrbitSystem.OrbitMode, ReMenuToggle> keyValuePair in this._modeToggles)
					{
						bool flag2 = keyValuePair.Key != mode && keyValuePair.Value.Value;
						if (flag2)
						{
							keyValuePair.Value.Toggle(new bool?(false), true);
						}
					}
					this._target = PlayerExtensions.GetPlayer(MenuEx.QMSelectedUserLocal.field_Private_IUser_0.Method_Public_Abstract_Virtual_New_get_String_0());
					this._currentMode = mode;
					this._isOrbiting = true;
					this._swasEnabled = mode == EnhancedOrbitSystem.OrbitMode.Swas;
					bool flag3 = this._pickups == null || this._pickups.Length == 0;
					if (flag3)
					{
						this._pickups = Object.FindObjectsOfType<VRC_Pickup>();
					}
					bool flag4 = this._orbitCenter == null;
					if (flag4)
					{
						this._orbitCenter = new GameObject("OrbitCenter");
					}
				}
				else
				{
					bool flag5 = this._currentMode == mode;
					if (flag5)
					{
						this._isOrbiting = false;
						this._swasEnabled = false;
						this._currentMode = EnhancedOrbitSystem.OrbitMode.None;
					}
				}
			}
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x000162DC File Offset: 0x000144DC
		public override void OnUpdate()
		{
			bool flag = !this._isOrbiting || this._target == null;
			if (!flag)
			{
				try
				{
					bool swasEnabled = this._swasEnabled;
					if (swasEnabled)
					{
						this.HandleSwas();
					}
					else
					{
						this.UpdateOrbitCenter();
						this.ApplyOrbitPattern();
					}
					bool chaosMode = this._chaosMode;
					if (chaosMode)
					{
						this.ChaosUpdate();
					}
					this._time += Time.deltaTime;
				}
				catch (Exception ex)
				{
					MelonLogger.Error("Orbit error: " + ex.Message);
					this.DisableAll();
				}
			}
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x00016388 File Offset: 0x00014588
		private void ChaosUpdate()
		{
			bool flag = Time.frameCount % 120 == 0;
			if (flag)
			{
				List<Player> list = new List<Player>(PlayerManager.Method_Public_Static_get_PlayerManager_0().field_Private_List_1_Player_0.ToArray());
				bool flag2 = list.Count > 1 && this._pickups.Length != 0;
				if (flag2)
				{
					int num = this._rng.Next(this._pickups.Length);
					VRC_Pickup vrc_Pickup = this._pickups[num];
					int num2 = this._rng.Next(list.Count);
					Player player = list[num2];
					Networking.SetOwner(player.field_Private_VRCPlayerApi_0, vrc_Pickup.gameObject);
					Vector3 vector;
					vector..ctor(Random.Range(-1f, 1f), Random.Range(0.5f, 1.5f), Random.Range(-1f, 1f));
					vrc_Pickup.transform.position = player._vrcplayer.transform.position + vector;
				}
			}
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x00016488 File Offset: 0x00014688
		private void HandleSwas()
		{
			try
			{
				bool flag = this._target != null;
				if (flag)
				{
					Vector3 bonePosition = this._target.field_Private_VRCPlayerApi_0.GetBonePosition(10);
					this._setLocation = new Vector3(bonePosition.x, bonePosition.y + this._configs[EnhancedOrbitSystem.OrbitMode.Swas].SwasHeight, bonePosition.z);
				}
				bool flag2 = this._rotateState >= 360f;
				if (flag2)
				{
					this._rotateState = Time.deltaTime;
				}
				else
				{
					this._rotateState += Time.deltaTime;
				}
				bool flag3 = this._hasTakenOwner >= 90f;
				if (flag3)
				{
					this._hasTakenOwner = 0f;
					for (int i = 0; i < this._pickups.Length; i++)
					{
						Networking.SetOwner(Player.Method_Internal_Static_get_Player_0().field_Private_VRCPlayerApi_0, this._pickups[i].gameObject);
					}
				}
				else
				{
					this._hasTakenOwner += 1f;
				}
				float num = (float)Convert.ToInt16(this._pickups.Length / 8);
				float num2 = (float)this._pickups.Length / this._swasSize;
				for (int j = 0; j < this._pickups.Length; j++)
				{
					VRC_Pickup vrc_Pickup = this._pickups[j];
					float num3 = (float)(j % 8);
					float num4 = (float)(j / 8);
					switch ((int)num3)
					{
					case 0:
						vrc_Pickup.transform.position = this._setLocation + new Vector3(0f, num2 * (num4 / num), 0f);
						break;
					case 1:
						vrc_Pickup.transform.position = this._setLocation + new Vector3(0f, -num2 * (num4 / num), 0f);
						break;
					case 2:
						vrc_Pickup.transform.position = this._setLocation + new Vector3(-Mathfrep.Cos(this._rotateState) * num2 * (num4 / num), 0f, Mathfrep.Sin(this._rotateState) * num2 * (num4 / num));
						break;
					case 3:
						vrc_Pickup.transform.position = this._setLocation + new Vector3(-Mathfrep.Cos(this._rotateState + this._setMultiplier) * num2 * (num4 / num), 0f, Mathfrep.Sin(this._rotateState + this._setMultiplier) * num2 * (num4 / num));
						break;
					case 4:
						vrc_Pickup.transform.position = this._setLocation + new Vector3(-Mathfrep.Cos(this._rotateState + this._setMultiplier) * num2, num2 * (num4 / num), Mathfrep.Sin(this._rotateState + this._setMultiplier) * num2);
						break;
					case 5:
						vrc_Pickup.transform.position = this._setLocation + new Vector3(-Mathfrep.Cos(this._rotateState) * num2, -num2 * (num4 / num), Mathfrep.Sin(this._rotateState) * num2);
						break;
					case 6:
						vrc_Pickup.transform.position = this._setLocation + new Vector3(-Mathfrep.Cos(this._rotateState + this._setMultiplier) * num2 * (num4 / num), -num2, Mathfrep.Sin(this._rotateState + this._setMultiplier) * (num2 * (num4 / num)));
						break;
					case 7:
						vrc_Pickup.transform.position = this._setLocation + new Vector3(-Mathfrep.Cos(this._rotateState) * num2 * (num4 / num), num2, Mathfrep.Sin(this._rotateState) * num2 * (num4 / num));
						break;
					}
					vrc_Pickup.GetComponent<Rigidbody>().velocity = Vector3.zero;
					vrc_Pickup.transform.rotation = Quaternion.Euler(0f, this._rotateState * -90f, 0f);
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Swas error: " + ex.Message);
				this.DisableAll();
			}
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x000168E8 File Offset: 0x00014AE8
		private void UpdateOrbitCenter()
		{
			bool flag = this._orbitCenter != null && this._target != null;
			if (flag)
			{
				this._orbitCenter.transform.position = this._target._vrcplayer.transform.position + new Vector3(0f, this._configs[this._currentMode].Height, 0f);
			}
		}

		// Token: 0x060003F8 RID: 1016 RVA: 0x0001696C File Offset: 0x00014B6C
		private void ApplyOrbitPattern()
		{
			EnhancedOrbitSystem.OrbitConfig orbitConfig = this._configs[this._currentMode];
			float num = this._time * (orbitConfig.Reverse ? (-orbitConfig.Speed) : orbitConfig.Speed);
			switch (this._currentMode)
			{
			case EnhancedOrbitSystem.OrbitMode.Circle:
				this.ApplyCirclePattern(orbitConfig, num);
				break;
			case EnhancedOrbitSystem.OrbitMode.Square:
				this.ApplySquarePattern(orbitConfig, num);
				break;
			case EnhancedOrbitSystem.OrbitMode.Triangle:
				this.ApplyPolygonPattern(3, orbitConfig, num);
				break;
			case EnhancedOrbitSystem.OrbitMode.Line:
				this.ApplyLinePattern(orbitConfig, num);
				break;
			case EnhancedOrbitSystem.OrbitMode.Spiral:
				this.ApplySpiralPattern(orbitConfig, num);
				break;
			case EnhancedOrbitSystem.OrbitMode.Pentagon:
				this.ApplyPolygonPattern(5, orbitConfig, num);
				break;
			case EnhancedOrbitSystem.OrbitMode.Hexagon:
				this.ApplyPolygonPattern(6, orbitConfig, num);
				break;
			case EnhancedOrbitSystem.OrbitMode.Octagon:
				this.ApplyPolygonPattern(8, orbitConfig, num);
				break;
			case EnhancedOrbitSystem.OrbitMode.Infinity:
				this.ApplyInfinityPattern(orbitConfig, num);
				break;
			case EnhancedOrbitSystem.OrbitMode.DNA:
				this.ApplyDNAPattern(orbitConfig, num);
				break;
			case EnhancedOrbitSystem.OrbitMode.Wave:
				this.ApplyWavePattern(orbitConfig, num);
				break;
			case EnhancedOrbitSystem.OrbitMode.Vortex:
				this.ApplyVortexPattern(orbitConfig, num);
				break;
			case EnhancedOrbitSystem.OrbitMode.Atom:
				this.ApplyAtomPattern(orbitConfig, num);
				break;
			case EnhancedOrbitSystem.OrbitMode.Heart:
				this.ApplyHeartPattern(orbitConfig, num);
				break;
			case EnhancedOrbitSystem.OrbitMode.Star:
				this.ApplyStarPattern(orbitConfig, num);
				break;
			case EnhancedOrbitSystem.OrbitMode.SinglePoint:
				this.ApplySinglePointPattern(orbitConfig, num);
				break;
			case EnhancedOrbitSystem.OrbitMode.StaticCluster:
				this.ApplyStaticClusterPattern(orbitConfig, num);
				break;
			}
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x00016AD8 File Offset: 0x00014CD8
		private void ApplySinglePointPattern(EnhancedOrbitSystem.OrbitConfig config, float time)
		{
			Vector3 position = this._orbitCenter.transform.position;
			for (int i = 0; i < this._pickups.Length; i++)
			{
				this.SetPickupTransform(this._pickups[i], position, 0f, config);
			}
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x00016B28 File Offset: 0x00014D28
		private void ApplyStaticClusterPattern(EnhancedOrbitSystem.OrbitConfig config, float time)
		{
			Vector3 position = this._orbitCenter.transform.position;
			float num = config.Radius * 0.5f;
			for (int i = 0; i < this._pickups.Length; i++)
			{
				float num2 = (float)i / (float)this._pickups.Length;
				float num3 = num2 * 3.1415927f * 2f;
				float num4 = num * (0.5f + 0.5f * Mathf.PerlinNoise((float)i * 0.1f, time * 0.1f));
				float num5 = Mathfrep.Cos(num3) * num4;
				float num6 = Mathfrep.Sin(num3) * num4;
				Vector3 vector = position + new Vector3(num5, 0f, num6);
				this.SetPickupTransform(this._pickups[i], vector, num3 * 57.295776f, config);
			}
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x00016C00 File Offset: 0x00014E00
		private void ApplyLinePattern(EnhancedOrbitSystem.OrbitConfig config, float time)
		{
			Vector3 position = this._orbitCenter.transform.position;
			float num = config.Radius * 2f;
			for (int i = 0; i < this._pickups.Length; i++)
			{
				float num2 = (float)i / (float)(this._pickups.Length - 1);
				float num3 = Mathfrep.Lerp(-num / 2f, num / 2f, num2);
				Vector3 vector = position + new Vector3(num3, config.Height, 0f);
				this.SetPickupTransform(this._pickups[i], vector, num2 * 360f, config);
			}
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x00016CA0 File Offset: 0x00014EA0
		private void ApplyCirclePattern(EnhancedOrbitSystem.OrbitConfig config, float time)
		{
			for (int i = 0; i < this._pickups.Length; i++)
			{
				float num = 360f / (float)this._pickups.Length * (float)i + time * 90f;
				float num2 = (config.Pulse ? (config.Radius * (1f + Mathfrep.Sin(time * 2f) * 0.2f)) : config.Radius);
				Vector3 vector = this.CalculateOrbitPosition(num, num2, config);
				this.SetPickupTransform(this._pickups[i], vector, num, config);
			}
		}

		// Token: 0x060003FD RID: 1021 RVA: 0x00016D30 File Offset: 0x00014F30
		private void ApplySpiralPattern(EnhancedOrbitSystem.OrbitConfig config, float time)
		{
			for (int i = 0; i < this._pickups.Length; i++)
			{
				float num = (float)i / (float)this._pickups.Length;
				float num2 = num * 720f + time * 90f;
				float num3 = config.Radius * (1f + num);
				Vector3 vector = this.CalculateOrbitPosition(num2, num3, config);
				vector.y += num * config.Height;
				this.SetPickupTransform(this._pickups[i], vector, num2, config);
			}
		}

		// Token: 0x060003FE RID: 1022 RVA: 0x000034D6 File Offset: 0x000016D6
		private void ApplySquarePattern(EnhancedOrbitSystem.OrbitConfig config, float time)
		{
			this.ApplyPolygonPath(4, config, time);
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x000034E3 File Offset: 0x000016E3
		private void ApplyPolygonPattern(int sides, EnhancedOrbitSystem.OrbitConfig config, float time)
		{
			this.ApplyPolygonPath(sides, config, time);
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x00016DB8 File Offset: 0x00014FB8
		private void ApplyPolygonPath(int sides, EnhancedOrbitSystem.OrbitConfig config, float time)
		{
			for (int i = 0; i < this._pickups.Length; i++)
			{
				float num = ((float)i / (float)this._pickups.Length + time * 0.1f) % 1f;
				float num2 = num * (float)sides % 1f;
				int num3 = (int)(num * (float)sides);
				float num4 = 360f / (float)sides;
				float num5 = (float)num3 * num4 * 0.017453292f;
				float num6 = (float)((num3 + 1) % sides) * num4 * 0.017453292f;
				Vector3 vector;
				vector..ctor(Mathfrep.Cos(num5) * config.Radius, config.Height, Mathfrep.Sin(num5) * config.Radius);
				Vector3 vector2;
				vector2..ctor(Mathfrep.Cos(num6) * config.Radius, config.Height, Mathfrep.Sin(num6) * config.Radius);
				Vector3 vector3 = Vector3.Lerp(vector, vector2, num2);
				this.SetPickupTransform(this._pickups[i], this._orbitCenter.transform.position + vector3, num * 360f, config);
			}
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x00016EC8 File Offset: 0x000150C8
		private void ApplyInfinityPattern(EnhancedOrbitSystem.OrbitConfig config, float time)
		{
			for (int i = 0; i < this._pickups.Length; i++)
			{
				float num = (float)i / (float)this._pickups.Length + time * 0.1f;
				float num2 = num * 3.1415927f * 2f;
				float num3 = Mathfrep.Sin(num2) * config.Radius;
				float num4 = Mathfrep.Sin(num2 * 2f) * config.Radius * 0.5f;
				Vector3 vector = this._orbitCenter.transform.position + new Vector3(num3, config.Height, num4);
				this.SetPickupTransform(this._pickups[i], vector, num * 360f, config);
			}
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x00016F84 File Offset: 0x00015184
		private void ApplyDNAPattern(EnhancedOrbitSystem.OrbitConfig config, float time)
		{
			for (int i = 0; i < this._pickups.Length; i++)
			{
				float num = (float)i / (float)this._pickups.Length;
				float num2 = num * 720f + time * 90f;
				float num3 = Mathfrep.Cos(num2 * 0.017453292f) * config.Radius;
				float num4 = num * config.Height * 2f - config.Height;
				float num5 = Mathfrep.Sin(num2 * 0.017453292f) * config.Radius;
				float num6 = Mathfrep.Sin(num2 * 0.017453292f * config.WaveFrequency) * config.WaveAmplitude;
				Vector3 vector;
				vector..ctor(num3 + num6, num4, num5 + num6);
				this.SetPickupTransform(this._pickups[i], this._orbitCenter.transform.position + vector, num2, config);
			}
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x00017068 File Offset: 0x00015268
		private void ApplyWavePattern(EnhancedOrbitSystem.OrbitConfig config, float time)
		{
			for (int i = 0; i < this._pickups.Length; i++)
			{
				float num = (float)i / (float)this._pickups.Length;
				float num2 = (num - 0.5f) * config.Radius * 2f;
				float num3 = Mathfrep.Sin((num * config.WaveFrequency + time) * 3.1415927f * 2f) * config.WaveAmplitude;
				Vector3 vector = this._orbitCenter.transform.position + new Vector3(num2, num3 + config.Height, 0f);
				this.SetPickupTransform(this._pickups[i], vector, num * 360f, config);
			}
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x00017120 File Offset: 0x00015320
		private void ApplyVortexPattern(EnhancedOrbitSystem.OrbitConfig config, float time)
		{
			for (int i = 0; i < this._pickups.Length; i++)
			{
				float num = (float)i / (float)this._pickups.Length;
				float num2 = num * 720f + time * 90f;
				float num3 = config.Radius * (1f - num * 0.5f);
				Vector3 vector = this.CalculateOrbitPosition(num2, num3, config);
				vector.y += num * config.Height;
				this.SetPickupTransform(this._pickups[i], vector, num2, config);
			}
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x000171AC File Offset: 0x000153AC
		private void ApplyAtomPattern(EnhancedOrbitSystem.OrbitConfig config, float time)
		{
			int num = 3;
			for (int i = 0; i < this._pickups.Length; i++)
			{
				int num2 = i % num;
				float num3 = (float)(i / num) / (float)(this._pickups.Length / num);
				Quaternion quaternion = Quaternion.Euler((float)num2 * 120f, 0f, 0f);
				float num4 = num3 * 360f + time * 90f;
				Vector3 vector;
				vector..ctor(Mathfrep.Cos(num4 * 0.017453292f) * config.Radius, 0f, Mathfrep.Sin(num4 * 0.017453292f) * config.Radius);
				Vector3 vector2 = this._orbitCenter.transform.position + quaternion * vector;
				this.SetPickupTransform(this._pickups[i], vector2, num4, config);
			}
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x00017284 File Offset: 0x00015484
		private void ApplyHeartPattern(EnhancedOrbitSystem.OrbitConfig config, float time)
		{
			for (int i = 0; i < this._pickups.Length; i++)
			{
				float num = (float)i / (float)this._pickups.Length + time * 0.1f;
				float num2 = num * 3.1415927f * 2f;
				float num3 = config.Radius * 0.2f;
				float num4 = num3 * 16f * Mathfrep.Pow(Mathfrep.Sin(num2), 3f);
				float num5 = num3 * (13f * Mathfrep.Cos(num2) - 5f * Mathfrep.Cos(2f * num2) - 2f * Mathfrep.Cos(3f * num2) - Mathfrep.Cos(4f * num2));
				bool pulse = config.Pulse;
				if (pulse)
				{
					float num6 = 1f + Mathfrep.Sin(time * 2f) * 0.2f;
					num4 *= num6;
					num5 *= num6;
				}
				Vector3 vector;
				vector..ctor(num4, config.Height, num5);
				float num7 = config.WaveAmplitude * Mathfrep.Sin(time * config.WaveFrequency + num2);
				vector += new Vector3(Mathfrep.Sin(num2) * num7, 0f, Mathfrep.Cos(num2) * num7);
				this.SetPickupTransform(this._pickups[i], this._orbitCenter.transform.position + vector, num * 360f, config);
			}
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x000173F4 File Offset: 0x000155F4
		private void ApplyStarPattern(EnhancedOrbitSystem.OrbitConfig config, float time)
		{
			int num = Mathf.Max(5, Mathfrep.RoundToInt(config.SpreadFactor));
			float num2 = config.Radius * 0.4f;
			for (int i = 0; i < this._pickups.Length; i++)
			{
				float num3 = (float)i / (float)this._pickups.Length + time * 0.1f;
				float num4 = num3 * 3.1415927f * 2f;
				float num5 = num4 % 6.2831855f;
				float num6 = 6.2831855f / (float)(num * 2);
				float num7 = num5 / num6;
				float num8 = num7 % 1f;
				float num9 = (((int)num7 % 2 == 0) ? Mathfrep.Lerp(config.Radius, num2, num8) : Mathfrep.Lerp(num2, config.Radius, num8));
				num9 += num3 * config.WaveAmplitude * Mathfrep.Sin(time * config.WaveFrequency);
				float num10 = Mathfrep.Cos(num4) * num9;
				float num11 = Mathfrep.Sin(num4) * num9;
				float num12 = time * config.RotationSpeed * 0.017453292f;
				float num13 = num10 * Mathfrep.Cos(num12) - num11 * Mathfrep.Sin(num12);
				float num14 = num10 * Mathfrep.Sin(num12) + num11 * Mathfrep.Cos(num12);
				Vector3 vector;
				vector..ctor(num13, config.Height, num14);
				bool pulse = config.Pulse;
				if (pulse)
				{
					float num15 = 1f + Mathfrep.Sin(time * 2f + num5) * 0.2f;
					vector *= num15;
				}
				this.SetPickupTransform(this._pickups[i], this._orbitCenter.transform.position + vector, num3 * 360f, config);
			}
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x000175A4 File Offset: 0x000157A4
		private Vector3 CalculateOrbitPosition(float angle, float radius, EnhancedOrbitSystem.OrbitConfig config)
		{
			float num = angle * 0.017453292f;
			Vector3 zero = Vector3.zero;
			switch (this._currentAxis)
			{
			case EnhancedOrbitSystem.OrbitAxis.XY:
				zero..ctor(Mathfrep.Cos(num) * radius, Mathfrep.Sin(num) * radius + config.Height, 0f);
				break;
			case EnhancedOrbitSystem.OrbitAxis.XZ:
				zero..ctor(Mathfrep.Cos(num) * radius, config.Height, Mathfrep.Sin(num) * radius);
				break;
			case EnhancedOrbitSystem.OrbitAxis.YZ:
				zero..ctor(0f, Mathfrep.Cos(num) * radius + config.Height, Mathfrep.Sin(num) * radius);
				break;
			case EnhancedOrbitSystem.OrbitAxis.ThreeD:
				zero..ctor(Mathfrep.Cos(num) * radius, Mathfrep.Sin(num * 0.5f) * radius + config.Height, Mathfrep.Sin(num) * radius);
				break;
			}
			return this._orbitCenter.transform.position + zero;
		}

		// Token: 0x06000409 RID: 1033 RVA: 0x00017698 File Offset: 0x00015898
		private void SetPickupTransform(VRC_Pickup pickup, Vector3 position, float angle, EnhancedOrbitSystem.OrbitConfig config)
		{
			bool flag = Networking.GetOwner(pickup.gameObject) != Networking.LocalPlayer;
			if (flag)
			{
				Networking.SetOwner(Networking.LocalPlayer, pickup.gameObject);
			}
			pickup.transform.position = position;
			bool spin = config.Spin;
			if (spin)
			{
				pickup.transform.rotation = Quaternion.Euler(0f, angle * config.RotationSpeed, 0f);
			}
			else
			{
				pickup.transform.rotation = Quaternion.identity;
			}
			pickup.GetComponent<Rigidbody>().velocity = Vector3.zero;
			bool rainbow = config.Rainbow;
			if (rainbow)
			{
				Renderer component = pickup.GetComponent<Renderer>();
				bool flag2 = component != null;
				if (flag2)
				{
					float num = (Time.time * 0.1f + angle / 360f) % 1f;
					component.material.color = Color.HSVToRGB(num, 1f, 1f);
				}
			}
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x00017794 File Offset: 0x00015994
		private void DisableAll()
		{
			this._isOrbiting = false;
			this._swasEnabled = false;
			this._currentMode = EnhancedOrbitSystem.OrbitMode.None;
			foreach (ReMenuToggle reMenuToggle in this._modeToggles.Values)
			{
				reMenuToggle.Toggle(new bool?(false), true);
			}
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x00017810 File Offset: 0x00015A10
		private void ResetAllSettings()
		{
			foreach (EnhancedOrbitSystem.OrbitConfig orbitConfig in this._configs.Values)
			{
				orbitConfig.Speed = 1f;
				orbitConfig.Radius = 2f;
				orbitConfig.Height = 1f;
				orbitConfig.Scale = 1f;
				orbitConfig.WaveAmplitude = 0.5f;
				orbitConfig.WaveFrequency = 1f;
				orbitConfig.RotationSpeed = 45f;
				orbitConfig.Reverse = false;
				orbitConfig.Pulse = false;
				orbitConfig.Rainbow = false;
				orbitConfig.Spin = false;
			}
			MelonLogger.Msg("All orbit settings have been reset.");
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x000178DC File Offset: 0x00015ADC
		private void RandomizeCurrentPattern()
		{
			bool flag = this._currentMode == EnhancedOrbitSystem.OrbitMode.None;
			if (!flag)
			{
				EnhancedOrbitSystem.OrbitConfig orbitConfig = this._configs[this._currentMode];
				orbitConfig.Speed = Random.Range(0.5f, 3f);
				orbitConfig.Radius = Random.Range(1f, 5f);
				orbitConfig.Height = Random.Range(-2f, 2f);
				orbitConfig.Scale = Random.Range(0.5f, 2f);
				orbitConfig.WaveAmplitude = Random.Range(0.2f, 1f);
				orbitConfig.WaveFrequency = Random.Range(0.5f, 3f);
				orbitConfig.RotationSpeed = Random.Range(15f, 90f);
				MelonLogger.Msg("Current pattern randomized.");
			}
		}

		// Token: 0x0600040D RID: 1037 RVA: 0x000179AC File Offset: 0x00015BAC
		public override void OnPlayerJoined(Player player)
		{
			bool flag = player == Player.Method_Internal_Static_get_Player_0();
			if (flag)
			{
				this.DisableAll();
				this._pickups = Object.FindObjectsOfType<VRC_Pickup>();
			}
		}

		// Token: 0x0400028C RID: 652
		private Player _target;

		// Token: 0x0400028D RID: 653
		private VRC_Pickup[] _pickups;

		// Token: 0x0400028E RID: 654
		private GameObject _orbitCenter;

		// Token: 0x0400028F RID: 655
		private EnhancedOrbitSystem.OrbitMode _currentMode = EnhancedOrbitSystem.OrbitMode.None;

		// Token: 0x04000290 RID: 656
		private EnhancedOrbitSystem.OrbitAxis _currentAxis = EnhancedOrbitSystem.OrbitAxis.XZ;

		// Token: 0x04000291 RID: 657
		private bool _isOrbiting;

		// Token: 0x04000292 RID: 658
		private float _time;

		// Token: 0x04000293 RID: 659
		private bool _chaosMode;

		// Token: 0x04000294 RID: 660
		private Vector3 _setLocation;

		// Token: 0x04000295 RID: 661
		private float _swasSize = 45f;

		// Token: 0x04000296 RID: 662
		private float _hasTakenOwner;

		// Token: 0x04000297 RID: 663
		private float _rotateState;

		// Token: 0x04000298 RID: 664
		private float _setMultiplier = 160f;

		// Token: 0x04000299 RID: 665
		private bool _swasEnabled;

		// Token: 0x0400029A RID: 666
		private Dictionary<EnhancedOrbitSystem.OrbitMode, EnhancedOrbitSystem.OrbitConfig> _configs;

		// Token: 0x0400029B RID: 667
		private Dictionary<EnhancedOrbitSystem.OrbitMode, ReMenuToggle> _modeToggles;

		// Token: 0x0400029C RID: 668
		private Random _rng = new Random();

		// Token: 0x02000092 RID: 146
		private enum OrbitMode
		{
			// Token: 0x0400029E RID: 670
			None,
			// Token: 0x0400029F RID: 671
			Circle,
			// Token: 0x040002A0 RID: 672
			Square,
			// Token: 0x040002A1 RID: 673
			Triangle,
			// Token: 0x040002A2 RID: 674
			Line,
			// Token: 0x040002A3 RID: 675
			Spiral,
			// Token: 0x040002A4 RID: 676
			Pentagon,
			// Token: 0x040002A5 RID: 677
			Hexagon,
			// Token: 0x040002A6 RID: 678
			Octagon,
			// Token: 0x040002A7 RID: 679
			Infinity,
			// Token: 0x040002A8 RID: 680
			DNA,
			// Token: 0x040002A9 RID: 681
			Wave,
			// Token: 0x040002AA RID: 682
			Vortex,
			// Token: 0x040002AB RID: 683
			Atom,
			// Token: 0x040002AC RID: 684
			Heart,
			// Token: 0x040002AD RID: 685
			Star,
			// Token: 0x040002AE RID: 686
			Swas,
			// Token: 0x040002AF RID: 687
			SinglePoint,
			// Token: 0x040002B0 RID: 688
			StaticCluster,
			// Token: 0x040002B1 RID: 689
			Custom
		}

		// Token: 0x02000093 RID: 147
		private enum OrbitAxis
		{
			// Token: 0x040002B3 RID: 691
			XY,
			// Token: 0x040002B4 RID: 692
			XZ,
			// Token: 0x040002B5 RID: 693
			YZ,
			// Token: 0x040002B6 RID: 694
			ThreeD
		}

		// Token: 0x02000094 RID: 148
		private class OrbitConfig
		{
			// Token: 0x040002B7 RID: 695
			public float Speed = 1f;

			// Token: 0x040002B8 RID: 696
			public float Radius = 2f;

			// Token: 0x040002B9 RID: 697
			public float Height = 1f;

			// Token: 0x040002BA RID: 698
			public float Scale = 1f;

			// Token: 0x040002BB RID: 699
			public float SecondaryRadius = 1f;

			// Token: 0x040002BC RID: 700
			public float WaveAmplitude = 0.5f;

			// Token: 0x040002BD RID: 701
			public float WaveFrequency = 1f;

			// Token: 0x040002BE RID: 702
			public float RotationSpeed = 45f;

			// Token: 0x040002BF RID: 703
			public float PhaseOffset = 0f;

			// Token: 0x040002C0 RID: 704
			public float SpreadFactor = 1f;

			// Token: 0x040002C1 RID: 705
			public bool Reverse = false;

			// Token: 0x040002C2 RID: 706
			public bool Pulse = false;

			// Token: 0x040002C3 RID: 707
			public bool Rainbow = false;

			// Token: 0x040002C4 RID: 708
			public bool Spin = false;

			// Token: 0x040002C5 RID: 709
			public float SwasSize = 45f;

			// Token: 0x040002C6 RID: 710
			public float SwasHeight = 2f;

			// Token: 0x040002C7 RID: 711
			public float SwasRotationSpeed = 1f;

			// Token: 0x040002C8 RID: 712
			public float SwasSpread = 1f;
		}
	}
}
