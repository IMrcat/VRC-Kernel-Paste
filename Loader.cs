using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using KernelClient.API;
using KernelClient.Core.Mono;
using MelonLoader;
using ReMod.Core.Unity;
using ReMod.Core.VRChat;
using TMPro;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using VRC.Core;
using VRC.UI.Core;
using VRC.UI.Elements.Controls;

// Token: 0x02000020 RID: 32
public class Loader
{
	// Token: 0x1700000A RID: 10
	// (get) Token: 0x060000D0 RID: 208 RVA: 0x00002472 File Offset: 0x00000672
	public static Loader Instance
	{
		get
		{
			Loader loader;
			if ((loader = Loader._instance) == null)
			{
				loader = (Loader._instance = new Loader());
			}
			return loader;
		}
	}

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x060000D1 RID: 209 RVA: 0x00002488 File Offset: 0x00000688
	// (set) Token: 0x060000D2 RID: 210 RVA: 0x0000248F File Offset: 0x0000068F
	public static bool IsBot { get; set; }

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x060000D3 RID: 211 RVA: 0x00002497 File Offset: 0x00000697
	// (set) Token: 0x060000D4 RID: 212 RVA: 0x0000249E File Offset: 0x0000069E
	public static string NumberBot { get; set; } = "0";

	// Token: 0x1700000D RID: 13
	// (get) Token: 0x060000D5 RID: 213 RVA: 0x000024A6 File Offset: 0x000006A6
	// (set) Token: 0x060000D6 RID: 214 RVA: 0x000024AD File Offset: 0x000006AD
	public static string ProfileNumber { get; set; } = "0";

	// Token: 0x060000D7 RID: 215 RVA: 0x00008090 File Offset: 0x00006290
	public static void Start()
	{
		MelonLogger.Msg("[KernelClient] Starting Loader...");
		Loader.RegisterIl2CppTypes();
		Loader.InitializeModules();
		Loader.OnApplicationStart();
		MelonCoroutines.Start(Loader.WaitForUser());
		MelonCoroutines.Start(Loader.WaitForUi());
		MelonCoroutines.Start(Loader.FiveSecTimer());
		Loader.LogStartupMessage();
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x000080E4 File Offset: 0x000062E4
	private static void OnApplicationStart()
	{
		MelonLogger.Msg("[KernelClient] Triggering OnApplicationStart for all modules...");
		using (List<KernelModule>.Enumerator enumerator = Loader.Modules.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KernelModule module = enumerator.Current;
				Loader.ExecuteWithLogging(delegate
				{
					module.OnApplicationStart();
				}, "Module " + module.GetType().Name + " OnApplicationStart");
			}
		}
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x000024B5 File Offset: 0x000006B5
	private static void RegisterIl2CppTypes()
	{
		MelonLogger.Msg("[KernelClient] Registering Il2Cpp types...");
		Loader.ExecuteWithLogging(delegate
		{
			ClassInjector.RegisterTypeInIl2Cpp<CustomNameplate>();
			ClassInjector.RegisterTypeInIl2Cpp<CustomNameplateAccountAge>();
			ClassInjector.RegisterTypeInIl2Cpp<EnableDisableListener>();
		}, "Il2Cpp Type Registration");
	}

	// Token: 0x060000DA RID: 218 RVA: 0x0000817C File Offset: 0x0000637C
	private static void InitializeModules()
	{
		MelonLogger.Msg("[KernelClient] Initializing Modules...");
		try
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			var orderedEnumerable = from t in (from t in executingAssembly.GetTypes()
					where !t.IsAbstract && t.BaseType == typeof(KernelModule) && !t.IsDefined(typeof(ComponentDisabled), false)
					select t).Select(delegate(Type t)
				{
					ComponentPriority customAttribute = t.GetCustomAttribute<ComponentPriority>();
					return new
					{
						Type = t,
						Priority = ((customAttribute != null) ? customAttribute.Priority : 0)
					};
				})
				orderby t.Priority
				select t;
			using (var enumerator = orderedEnumerable.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					<>f__AnonymousType0<Type, int> moduleInfo = enumerator.Current;
					Loader.ExecuteWithLogging(delegate
					{
						MelonLogger.Msg("[KernelClient] Initializing module: " + moduleInfo.Type.Name);
						ConstructorInfo constructor = moduleInfo.Type.GetConstructor(Type.EmptyTypes);
						bool flag2 = constructor == null;
						if (flag2)
						{
							MelonLogger.Warning("[KernelClient] No parameterless constructor found for module " + moduleInfo.Type.Name + ". Skipping initialization.");
						}
						else
						{
							KernelModule kernelModule = (KernelModule)Activator.CreateInstance(moduleInfo.Type);
							bool flag3 = kernelModule == null;
							if (flag3)
							{
								MelonLogger.Error("[KernelClient] Failed to create instance of module " + moduleInfo.Type.Name + ".");
							}
							else
							{
								Loader.Modules.Add(kernelModule);
								MelonLogger.Msg("[KernelClient] Module " + moduleInfo.Type.Name + " loaded successfully.");
							}
						}
					}, "Module " + moduleInfo.Type.Name + " Initialization");
				}
			}
			MelonLogger.Msg(string.Format("[KernelClient] Initialized {0} modules successfully.", Loader.Modules.Count), new object[] { ConsoleColor.Green });
		}
		catch (ReflectionTypeLoadException ex)
		{
			MelonLogger.Error("[KernelClient] Reflection Type Load Exception: " + ex.Message);
			foreach (Exception ex2 in ex.LoaderExceptions)
			{
				bool flag = ex2 != null;
				if (flag)
				{
					MelonLogger.Error(string.Concat(new string[]
					{
						"- ",
						ex2.GetType().Name,
						": ",
						ex2.Message,
						"\nStackTrace: ",
						ex2.StackTrace
					}));
				}
			}
		}
		catch (Exception ex3)
		{
			MelonLogger.Error(string.Format("[KernelClient] Critical Module Initialization Error: {0}: {1}\nSource: {2}\nTargetSite: {3}\nStackTrace: {4}", new object[]
			{
				ex3.GetType().FullName,
				ex3.Message,
				ex3.Source,
				ex3.TargetSite,
				ex3.StackTrace
			}));
		}
	}

	// Token: 0x060000DB RID: 219 RVA: 0x000024ED File Offset: 0x000006ED
	public static IEnumerator FiveSecTimer()
	{
		for (;;)
		{
			yield return new WaitForSeconds(5f);
		}
		yield break;
	}

	// Token: 0x060000DC RID: 220 RVA: 0x000024F5 File Offset: 0x000006F5
	private static IEnumerator WaitForUser()
	{
		MelonLogger.Msg("[KernelClient] Waiting for user to log in...");
		while (APIUser.CurrentUser == null)
		{
			yield return null;
		}
		MelonLogger.Msg("[KernelClient] User Logged in successfully.");
		yield break;
	}

	// Token: 0x060000DD RID: 221 RVA: 0x000024FD File Offset: 0x000006FD
	private static IEnumerator WaitForUi()
	{
		MelonLogger.Msg("[KernelClient] Waiting for UI Manager...");
		while (VRCUiManager.field_Private_Static_VRCUiManager_0 == null)
		{
			yield return null;
		}
		while (UIManager.Method_Public_Static_get_UIManager_PDM_0() == null)
		{
			yield return null;
		}
		MelonCoroutines.Start(Loader.LoadAudioAsync());
		MelonLogger.Msg("[KernelClient] Waiting for QMParent scene...");
		while (!Loader.IsSceneLoaded("QMParent"))
		{
			yield return null;
		}
		MelonLogger.Msg("[KernelClient] QMParent scene loaded.");
		MelonLogger.Msg("[KernelClient] Waiting for MMParent scene...");
		while (!Loader.IsSceneLoaded("MMParent"))
		{
			yield return null;
		}
		MelonLogger.Msg("[KernelClient] MMParent scene loaded.");
		MelonLogger.Msg("[KernelClient] Waiting for ActionMenuController...");
		while (ActionMenuController.Method_Public_Static_get_ActionMenuController_PDM_0() == null)
		{
			yield return null;
		}
		ReModPatches.Patch();
		MenuSetup.OnUiManagerInit();
		MelonLogger.Msg("[KernelClient] UI Initialization Complete.");
		yield break;
	}

	// Token: 0x060000DE RID: 222 RVA: 0x000083E4 File Offset: 0x000065E4
	private static bool IsSceneLoaded(string objectName)
	{
		return Loader.FindDontDestroyOnLoadObject(objectName) != null;
	}

	// Token: 0x060000DF RID: 223 RVA: 0x00002505 File Offset: 0x00000705
	private static IEnumerator LoadAudioAsync()
	{
		string audioPath = Path.Combine(Environment.CurrentDirectory, "KernelClient/Loading.wav");
		bool flag = !File.Exists(audioPath);
		if (flag)
		{
			EmbedExtract.ExtractResource("KernelClient.assets.loading.wav", audioPath);
		}
		UnityWebRequest request = UnityWebRequest.Get("file://" + audioPath);
		yield return request.SendWebRequest();
		bool flag2 = request.isNetworkError || request.isHttpError;
		if (flag2)
		{
			MelonLogger.Error("Failed to load audio: " + request.error);
			yield break;
		}
		Loader._audioClip = WebRequestWWW.InternalCreateAudioClipUsingDH(request.downloadHandler, request.url, false, false, 20);
		Loader.SetupAudioSources();
		yield break;
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x00008404 File Offset: 0x00006604
	private static void SetupAudioSources()
	{
		Transform transform = APIUtils.UserInterface.transform.Find("LoadingBackground_TealGradient_Music/LoadingSound");
		bool flag = transform != null;
		if (flag)
		{
			Loader._loadingScreenAudio = transform.GetComponent<AudioSource>();
			Loader._loadingMusicController = transform.GetComponent<VRCUiPageLoadingMusicController>();
			bool flag2 = Loader._loadingMusicController != null;
			if (flag2)
			{
				Object.Destroy(Loader._loadingMusicController);
			}
			bool flag3 = Loader._loadingScreenAudio != null;
			if (flag3)
			{
				Loader._loadingScreenAudio.clip = Loader._audioClip;
				Loader._loadingScreenAudio.playOnAwake = true;
				Loader._loadingScreenAudio.Play();
			}
		}
		Transform transform2 = APIUtils.UserInterface.transform.Find("MenuContent/Popups/LoadingPopup/LoadingSound");
		bool flag4 = transform2 != null;
		if (flag4)
		{
			Loader._popupAudio = transform2.GetComponent<AudioSource>();
			Loader._popupMusicController = transform2.GetComponent<VRCUiPageLoadingMusicController>();
			bool flag5 = Loader._popupMusicController != null;
			if (flag5)
			{
				Object.Destroy(Loader._popupMusicController);
			}
			bool flag6 = Loader._popupAudio != null;
			if (flag6)
			{
				Loader._popupAudio.clip = Loader._audioClip;
				Loader._popupAudio.playOnAwake = true;
				Loader._popupAudio.Play();
			}
		}
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x00008530 File Offset: 0x00006730
	private static void LogStartupMessage()
	{
		MelonLogger.Msg("");
		MelonLogger.Msg(ConsoleColor.DarkMagenta, "------------------------------------------------------------------------");
		MelonLogger.Msg(ConsoleColor.Blue, "Kernel Client 1.0.1 by the Kernel team.");
		MelonLogger.Msg(ConsoleColor.DarkCyan, "Data (The Owner), Bluethefox, Vida?, Kuz, Lili, Spiral, BOSHI");
		MelonLogger.Msg(ConsoleColor.DarkMagenta, "------------------------------------------------------------------------");
		MelonLogger.Msg("");
	}

	// Token: 0x060000E2 RID: 226 RVA: 0x00008588 File Offset: 0x00006788
	public static void OnUpdate()
	{
		using (List<KernelModule>.Enumerator enumerator = Loader.Modules.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KernelModule module2 = enumerator.Current;
				Loader.ExecuteWithLogging(delegate
				{
					module2.OnUpdate();
				}, "Module " + module2.GetType().Name + " OnUpdate");
			}
		}
		using (List<KernelModule>.Enumerator enumerator2 = Loader.Modules.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				KernelModule module = enumerator2.Current;
				Loader.ExecuteWithLogging(delegate
				{
					module.OnRenderObject();
				}, "Module " + module.GetType().Name + " OnRenderObject");
			}
		}
	}

	// Token: 0x060000E3 RID: 227 RVA: 0x00008690 File Offset: 0x00006890
	public static void OnSceneWasLoaded(int buildIndex, string sceneName)
	{
		MelonLogger.Msg("[KernelClient] Scene loaded: " + sceneName);
		Loader.ProcessScene(sceneName);
		bool flag = buildIndex != -1;
		if (!flag)
		{
			using (List<KernelModule>.Enumerator enumerator = Loader.Modules.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KernelModule module = enumerator.Current;
					Loader.ExecuteWithLogging(delegate
					{
						module.OnSceneWasLoaded(buildIndex, sceneName);
					}, "Module " + module.GetType().Name + " OnSceneWasLoaded");
				}
			}
		}
	}

	// Token: 0x060000E4 RID: 228 RVA: 0x0000876C File Offset: 0x0000696C
	private static void ProcessScene(string sceneName)
	{
		MelonLogger.Msg("[KernelClient] Processing scene: " + sceneName);
		Loader.ApplyCustomizations();
		if (!(sceneName == "ui"))
		{
			if (!(sceneName == "Application2"))
			{
				if (!(sceneName == "GameCore"))
				{
					MelonLogger.Msg("[KernelClient] No specific processing defined for scene: " + sceneName);
				}
				else
				{
					Loader.ProcessGameCoreScene();
				}
			}
			else
			{
				Loader.ProcessApplicationScene();
			}
		}
		else
		{
			Loader.ProcessUIScene();
		}
	}

	// Token: 0x060000E5 RID: 229 RVA: 0x000087EC File Offset: 0x000069EC
	private static void ApplyCustomizations()
	{
		MelonLogger.Msg("[KernelClient] Applying customizations to GameObjects...");
		bool flag = Loader._skyCubeTexture == null;
		if (flag)
		{
			Loader.ExecuteWithLogging(delegate
			{
				Loader._skyCubeTexture = EmbeddedResourceLoader.LoadEmbeddedTexture("KernelClient.assets.Gradient_DarkTeal_REPL.png");
				bool flag3 = Loader._skyCubeTexture != null;
				if (flag3)
				{
					MelonLogger.Msg("[KernelClient] SkyCube Texture loaded successfully.");
				}
				else
				{
					MelonLogger.Warning("[KernelClient] Failed to load SkyCube Texture.");
				}
			}, "Load SkyCube Texture");
		}
		bool flag2 = Loader._backgroundTexture == null;
		if (flag2)
		{
			Loader.ExecuteWithLogging(delegate
			{
				Loader._backgroundTexture = EmbeddedResourceLoader.LoadEmbeddedTexture("KernelClient.assets.Nightsky.png");
				bool flag4 = Loader._backgroundTexture != null;
				if (flag4)
				{
					MelonLogger.Msg("[KernelClient] Background Texture loaded successfully.");
				}
				else
				{
					MelonLogger.Warning("[KernelClient] Failed to load Background Texture.");
				}
			}, "Load Background Texture");
		}
		Loader.CustomizeGameObject("SkyCube_Baked", new Action<GameObject>(Loader.SkyCubeCustomization), true);
		Loader.CustomizeGameObject("Lighter", new Action<GameObject>(Loader.LighterCustomization), true);
		Loader.CustomizeGameObject("Button", new Action<GameObject>(Loader.ButtonCustomization), true);
		Loader.CustomizeGameObject("Page_DevTools", new Action<GameObject>(Loader.DevToolsCustomization), true);
		Loader.CustomizeGameObject("Background", new Action<GameObject>(Loader.BackgroundCustomization), true);
	}

	// Token: 0x060000E6 RID: 230 RVA: 0x000088F4 File Offset: 0x00006AF4
	private static void SkyCubeCustomization(GameObject obj)
	{
		bool flag = obj == null;
		if (flag)
		{
			MelonLogger.Warning("[KernelClient] SkyCube_Baked GameObject is null.");
		}
		else
		{
			MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
			bool flag2 = meshRenderer != null && Loader._skyCubeTexture != null;
			if (flag2)
			{
				Loader.ExecuteWithLogging(delegate
				{
					meshRenderer.material.SetTexture("_Tex", Loader._skyCubeTexture);
					meshRenderer.material.SetColor("_Tint", Loader._primaryColor);
					MelonLogger.Msg("[KernelClient] SkyCube_Baked customized successfully.");
				}, "Customize SkyCube_Baked");
			}
			else
			{
				MelonLogger.Warning("[KernelClient] SkyCube_Baked MeshRenderer or Texture is missing.");
			}
		}
	}

	// Token: 0x060000E7 RID: 231 RVA: 0x00008978 File Offset: 0x00006B78
	private static void LighterCustomization(GameObject obj)
	{
		bool flag = obj == null;
		if (flag)
		{
			MelonLogger.Warning("[KernelClient] Lighter GameObject is null.");
		}
		else
		{
			Image image = obj.GetComponent<Image>();
			bool flag2 = image != null;
			if (flag2)
			{
				Loader.ExecuteWithLogging(delegate
				{
					image.color = Loader._primaryColor;
					MelonLogger.Msg("[KernelClient] Lighter customized successfully.");
				}, "Customize Lighter");
			}
			else
			{
				MelonLogger.Warning("[KernelClient] Lighter Image component is missing.");
			}
		}
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x000089EC File Offset: 0x00006BEC
	private static void ButtonCustomization(GameObject obj)
	{
		bool flag = obj == null;
		if (flag)
		{
			MelonLogger.Warning("[KernelClient] Button GameObject is null.");
		}
		else
		{
			Button button = obj.GetComponent<Button>();
			bool flag2 = button != null;
			if (flag2)
			{
				Loader.ExecuteWithLogging(delegate
				{
					ColorBlock colors = button.colors;
					colors.normalColor = Loader._primaryColor;
					colors.highlightedColor = Loader._highlightColor;
					colors.pressedColor = Loader._pressedColor;
					button.colors = colors;
					MelonLogger.Msg("[KernelClient] Button customized successfully.");
				}, "Customize Button");
			}
			else
			{
				MelonLogger.Warning("[KernelClient] Button component is missing.");
			}
		}
	}

	// Token: 0x060000E9 RID: 233 RVA: 0x00008A60 File Offset: 0x00006C60
	private static void TextTitleCustomization(GameObject obj)
	{
		bool flag = obj == null;
		if (flag)
		{
			MelonLogger.Warning("[KernelClient] Text_Title GameObject is null.");
		}
		else
		{
			TextMeshProUGUIEx textComponent = obj.GetComponent<TextMeshProUGUIEx>();
			bool flag2 = textComponent != null;
			if (flag2)
			{
				Loader.ExecuteWithLogging(delegate
				{
					bool flag3 = textComponent.GetParsedText() == "Launch Pad";
					if (flag3)
					{
						textComponent.richText = true;
						Object.Destroy(textComponent);
						MelonLogger.Msg("[KernelClient] Text_Title customized and TextMeshProUGUIEx destroyed.");
					}
					else
					{
						MelonLogger.Msg("[KernelClient] Text_Title does not match 'Launch Pad'. No customization applied.");
					}
				}, "Customize Text_Title");
			}
			else
			{
				MelonLogger.Warning("[KernelClient] TextMeshProUGUIEx component is missing on Text_Title.");
			}
		}
	}

	// Token: 0x060000EA RID: 234 RVA: 0x00008AD4 File Offset: 0x00006CD4
	private static void BannersCustomization(GameObject obj)
	{
		bool flag = obj == null;
		if (flag)
		{
			MelonLogger.Warning("[KernelClient] Carousel_Banners GameObject is null.");
		}
		else
		{
			Loader.ExecuteWithLogging(delegate
			{
				obj.SetActive(false);
				MelonLogger.Msg("[KernelClient] Carousel_Banners deactivated.");
			}, "Customize Carousel_Banners");
		}
	}

	// Token: 0x060000EB RID: 235 RVA: 0x00008B24 File Offset: 0x00006D24
	private static void DevToolsCustomization(GameObject obj)
	{
		bool flag = obj == null;
		if (flag)
		{
			MelonLogger.Warning("[KernelClient] Page_DevTools GameObject is null.");
		}
		else
		{
			Loader.ExecuteWithLogging(delegate
			{
				obj.SetActive(true);
				MelonLogger.Msg("[KernelClient] Page_DevTools activated.");
			}, "Customize Page_DevTools");
		}
	}

	// Token: 0x060000EC RID: 236 RVA: 0x00008B74 File Offset: 0x00006D74
	private static void BackgroundCustomization(GameObject obj)
	{
		bool flag = obj == null;
		if (flag)
		{
			MelonLogger.Warning("[KernelClient] Background GameObject is null.");
		}
		else
		{
			Image image = obj.GetComponent<Image>();
			bool flag2 = image != null && Loader._backgroundTexture != null;
			if (flag2)
			{
				Loader.ExecuteWithLogging(delegate
				{
					image.sprite = Sprite.Create(Loader._backgroundTexture, new Rect(0f, 0f, (float)Loader._backgroundTexture.width, (float)Loader._backgroundTexture.height), new Vector2(0f, 0f), 1f);
					image.color = Color.white;
					MelonLogger.Msg("[KernelClient] Background customized successfully.");
				}, "Customize Background");
			}
			else
			{
				MelonLogger.Warning("[KernelClient] Background Image component or Texture is missing.");
			}
		}
	}

	// Token: 0x060000ED RID: 237 RVA: 0x00008BF8 File Offset: 0x00006DF8
	private static void CustomizeGameObject(string objectName, Action<GameObject> customization, bool dondestroy = true)
	{
		if (dondestroy)
		{
			GameObject obj = Loader.FindDontDestroyOnLoadObject(objectName);
			bool flag = obj != null;
			if (flag)
			{
				Loader.ExecuteWithLogging(delegate
				{
					customization(obj);
				}, "Customize " + objectName);
			}
			else
			{
				MelonLogger.Warning("[KernelClient] GameObject '" + objectName + "' not found for customization.");
			}
		}
	}

	// Token: 0x060000EE RID: 238 RVA: 0x00008C80 File Offset: 0x00006E80
	private static GameObject FindDontDestroyOnLoadObject(string objectName)
	{
		GameObject[] array = Object.FindObjectsOfType<GameObject>();
		foreach (GameObject gameObject in array)
		{
			bool flag = gameObject.name == objectName;
			if (flag)
			{
				return gameObject;
			}
		}
		return null;
	}

	// Token: 0x060000EF RID: 239 RVA: 0x0000250D File Offset: 0x0000070D
	private static void ProcessUIScene()
	{
		MelonLogger.Msg("[KernelClient] Processing UI Scene...");
	}

	// Token: 0x060000F0 RID: 240 RVA: 0x0000251B File Offset: 0x0000071B
	private static void ProcessApplicationScene()
	{
		MelonLogger.Msg("[KernelClient] Processing Application Scene...");
	}

	// Token: 0x060000F1 RID: 241 RVA: 0x00002529 File Offset: 0x00000729
	private static void ProcessGameCoreScene()
	{
		MelonLogger.Msg("[KernelClient] Processing GameCore Scene...");
	}

	// Token: 0x060000F2 RID: 242 RVA: 0x00008CD0 File Offset: 0x00006ED0
	private static void ExecuteWithLogging(Action action, string actionDescription)
	{
		try
		{
			action();
		}
		catch (Exception ex)
		{
			MelonLogger.Error(string.Concat(new string[]
			{
				"[KernelClient] Error during ",
				actionDescription,
				": ",
				ex.GetType().Name,
				": ",
				ex.Message,
				"\n",
				ex.StackTrace
			}));
		}
	}

	// Token: 0x04000099 RID: 153
	private const string CLIENT_NAME = "Kernel Client";

	// Token: 0x0400009A RID: 154
	private const string CLIENT_VERSION = "1.0.1";

	// Token: 0x0400009B RID: 155
	private const string LOADING_AUDIO_RESOURCE = "KernelClient.assets.loading.wav";

	// Token: 0x0400009C RID: 156
	private static Loader _instance;

	// Token: 0x0400009D RID: 157
	public static readonly List<KernelModule> Modules = new List<KernelModule>();

	// Token: 0x0400009E RID: 158
	private static AudioClip _audioClip;

	// Token: 0x0400009F RID: 159
	private static AudioSource _loadingScreenAudio;

	// Token: 0x040000A0 RID: 160
	private static AudioSource _popupAudio;

	// Token: 0x040000A1 RID: 161
	private static VRCUiPageLoadingMusicController _loadingMusicController;

	// Token: 0x040000A2 RID: 162
	private static VRCUiPageLoadingMusicController _popupMusicController;

	// Token: 0x040000A3 RID: 163
	public static TextMeshProUGUI DebugText;

	// Token: 0x040000A4 RID: 164
	public static MenuStateController MenuStateController;

	// Token: 0x040000A5 RID: 165
	internal static Queue<Action> Queue = new Queue<Action>();

	// Token: 0x040000A6 RID: 166
	private static Texture2D _skyCubeTexture;

	// Token: 0x040000A7 RID: 167
	private static Texture2D _backgroundTexture;

	// Token: 0x040000A8 RID: 168
	private static Color _primaryColor = new Color(0.447f, 0f, 0.733f, 1f);

	// Token: 0x040000A9 RID: 169
	private static Color _highlightColor = new Color(0.547f, 0f, 0.933f, 1f);

	// Token: 0x040000AA RID: 170
	private static Color _pressedColor = new Color(0.247f, 0f, 0.333f, 1f);

	// Token: 0x040000AE RID: 174
	private const string LOADING_AUDIO_PATH = "KernelClient/Loading.wav";
}
