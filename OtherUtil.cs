using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Il2CppSystem;
using KernelClient.API;
using MelonLoader;
using ReMod.Core.VRChat;
using UnhollowerRuntimeLib.XrefScans;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using VRC.DataModel;
using VRC.Localization;
using VRC.UI.Elements.Controls;
using VRC.UI.Elements.Menus;

// Token: 0x02000036 RID: 54
public static class OtherUtil
{
	// Token: 0x06000132 RID: 306 RVA: 0x00009768 File Offset: 0x00007968
	public static bool XRefScanForMethod(MethodBase methodBase, string methodName = null, string reflectedType = null)
	{
		bool flag = false;
		foreach (XrefInstance xrefInstance in XrefScanner.XrefScan(methodBase))
		{
			bool flag2 = xrefInstance.Type != 1;
			if (!flag2)
			{
				MethodBase methodBase2 = xrefInstance.TryResolve();
				bool flag3 = !(methodBase2 == null);
				if (flag3)
				{
					Type reflectedType2 = methodBase2.ReflectedType;
					Console.WriteLine((reflectedType2 != null) ? reflectedType2.Name : null);
					bool flag4 = !string.IsNullOrEmpty(methodName);
					if (flag4)
					{
						flag = methodBase2.Name != null && methodBase2.Name.IndexOf(methodName, StringComparison.OrdinalIgnoreCase) >= 0;
					}
					bool flag5 = !string.IsNullOrEmpty(reflectedType);
					if (flag5)
					{
						Type reflectedType3 = methodBase2.ReflectedType;
						flag = ((reflectedType3 != null) ? reflectedType3.Name : null) != null && methodBase2.ReflectedType.Name.IndexOf(reflectedType, StringComparison.OrdinalIgnoreCase) >= 0;
					}
					bool flag6 = flag;
					if (flag6)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06000133 RID: 307 RVA: 0x00009890 File Offset: 0x00007A90
	public static string ToAgeString(DateTime dob)
	{
		DateTime today = DateTime.Today;
		int num = today.Month - dob.Month;
		int num2 = today.Year - dob.Year;
		bool flag = today.Day < dob.Day;
		if (flag)
		{
			num--;
		}
		bool flag2 = num < 0;
		if (flag2)
		{
			num2--;
			num += 12;
		}
		int days = (today - dob.AddMonths(num2 * 12 + num)).Days;
		return string.Format("{0} Year{1}, {2} Month{3}, {4} Day{5}", new object[]
		{
			num2,
			(num2 == 1) ? "" : "s",
			num,
			(num == 1) ? "" : "s",
			days,
			(days == 1) ? "" : "s"
		});
	}

	// Token: 0x06000134 RID: 308 RVA: 0x000026CF File Offset: 0x000008CF
	private static void CloseInputPopupWrapper(string _)
	{
		OtherUtil.CloseInputPopup();
	}

	// Token: 0x06000135 RID: 309 RVA: 0x0000997C File Offset: 0x00007B7C
	public static void InputPopup(string title, string placeholder, string okButton, Action<string> realTimeString, Action<string> endString, Action onClose, bool multiLine = true, int charLimit = 0, bool world = false)
	{
		bool flag = OtherUtil._container == null;
		if (flag)
		{
			OtherUtil._container = new GameObject("KernelInputterIguess");
			Object.DontDestroyOnLoad(OtherUtil._container);
		}
		bool flag2 = OtherUtil._keyboardComponent == null;
		if (flag2)
		{
			OtherUtil._keyboardComponent = OtherUtil._container.AddComponent<VRCInputField>();
		}
		bool flag3 = endString == null;
		if (flag3)
		{
			Action<string> action = delegate
			{
				OtherUtil.CloseInputPopup();
			};
			endString = action;
		}
		try
		{
			KeyboardData keyboardData = new KeyboardData();
			KeyboardData keyboardData2 = keyboardData.Method_Public_KeyboardData_LocalizableString_LocalizableString_String_LocalizableString_LocalizableString_0(LocalizableStringExtensions.Localize(title, null, null, null), LocalizableStringExtensions.Localize(placeholder, null, null, null), "1", LocalizableStringExtensions.Localize(okButton, null, null, null), LocalizableStringExtensions.Localize("", null, null, null));
			KeyboardData keyboardData3 = keyboardData2.Method_Public_KeyboardData_Action_1_String_Action_1_String_Action_Boolean_PDM_0(realTimeString, endString, onClose, true);
			KeyboardData keyboardData4 = keyboardData3.Method_Public_KeyboardData_InputPopupType_Boolean_PDM_0(0, true);
			KeyboardData keyboardData5 = keyboardData4.Method_Public_KeyboardData_InputType_ContentType_Int32_Boolean_Boolean_InterfacePublicAbstractBoStVoAc1VoAcSt1BoUnique_PDM_0(0, 0, charLimit, multiLine, false, null);
			keyboardData5._isWorldKeyboard = world;
			OtherUtil._keyboardComponent._keyboardData = keyboardData5;
			OtherUtil._keyboardComponent.Method_Private_Void_0();
		}
		catch (Exception ex)
		{
			MelonLogger.Error(string.Format("{0}\n{1}", ex, ex.Source));
		}
	}

	// Token: 0x06000136 RID: 310 RVA: 0x00009AD0 File Offset: 0x00007CD0
	public static MainMenuSelectedUser GetMainMenuSelectedUser()
	{
		bool flag = OtherUtil._mainMenuSelectedUser == null;
		if (flag)
		{
			OtherUtil._mainMenuSelectedUser = MenuEx.MMenuParent.transform.Find("Menu_UserDetail").GetComponent<MainMenuSelectedUser>();
		}
		return OtherUtil._mainMenuSelectedUser;
	}

	// Token: 0x06000137 RID: 311 RVA: 0x000026D8 File Offset: 0x000008D8
	public static IEnumerator LoadAudio(AudioSource audioSource, string path)
	{
		UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, 20);
		yield return www.SendWebRequest();
		bool flag = www.result != 2 && www.result != 3;
		if (flag)
		{
			audioSource.clip = DownloadHandlerAudioClip.GetContent(www);
		}
		www.Dispose();
		yield break;
	}

	// Token: 0x06000138 RID: 312 RVA: 0x00009B18 File Offset: 0x00007D18
	public static void CloseInputPopup()
	{
		bool flag = OtherUtil._closeKeyboardObject == null;
		if (flag)
		{
			OtherUtil._closeKeyboardObject = APIUtils.SocialMenuInstance.transform.Find("Container/MMParent/Modal_MM_Keyboard/MenuPanel/Header_Modal_H1_Centered/RightItemContainer/Button_Close").gameObject;
		}
		bool flag2 = !(OtherUtil._closeKeyboardObject == null);
		if (flag2)
		{
			Button component = OtherUtil._closeKeyboardObject.GetComponent<Button>();
			bool flag3 = component != null;
			if (flag3)
			{
				component.onClick.Invoke();
			}
		}
	}

	// Token: 0x040000D4 RID: 212
	private static MainMenuSelectedUser _mainMenuSelectedUser;

	// Token: 0x040000D5 RID: 213
	private static VRCInputField _keyboardComponent;

	// Token: 0x040000D6 RID: 214
	private static GameObject _closeKeyboardObject;

	// Token: 0x040000D7 RID: 215
	private static GameObject _container;

	// Token: 0x040000D8 RID: 216
	public static List<string> totalHudLogs = new List<string>();

	// Token: 0x040000D9 RID: 217
	public static List<string> processStrings = new List<string>();
}
