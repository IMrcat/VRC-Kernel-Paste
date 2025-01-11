using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

// Token: 0x0200000C RID: 12
public static class GUIUtility
{
	// Token: 0x06000026 RID: 38
	[DllImport("user32.dll")]
	private static extern bool OpenClipboard(IntPtr hWndNewOwner);

	// Token: 0x06000027 RID: 39
	[DllImport("user32.dll")]
	private static extern bool CloseClipboard();

	// Token: 0x06000028 RID: 40
	[DllImport("user32.dll")]
	private static extern bool EmptyClipboard();

	// Token: 0x06000029 RID: 41
	[DllImport("user32.dll")]
	private static extern IntPtr GetClipboardData(uint uFormat);

	// Token: 0x0600002A RID: 42
	[DllImport("user32.dll")]
	private static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

	// Token: 0x0600002B RID: 43
	[DllImport("kernel32.dll")]
	private static extern IntPtr GlobalLock(IntPtr hMem);

	// Token: 0x0600002C RID: 44
	[DllImport("kernel32.dll")]
	private static extern bool GlobalUnlock(IntPtr hMem);

	// Token: 0x0600002D RID: 45
	[DllImport("kernel32.dll")]
	private static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

	// Token: 0x0600002E RID: 46
	[DllImport("kernel32.dll")]
	private static extern IntPtr GlobalFree(IntPtr hMem);

	// Token: 0x0600002F RID: 47
	[DllImport("kernel32.dll")]
	private static extern UIntPtr GlobalSize(IntPtr hMem);

	// Token: 0x17000007 RID: 7
	// (get) Token: 0x06000030 RID: 48 RVA: 0x000049B8 File Offset: 0x00002BB8
	// (set) Token: 0x06000031 RID: 49 RVA: 0x00004A60 File Offset: 0x00002C60
	public static string systemCopyBuffer
	{
		get
		{
			bool flag = !GUIUtility.OpenClipboard(IntPtr.Zero);
			string text;
			if (flag)
			{
				text = string.Empty;
			}
			else
			{
				try
				{
					IntPtr clipboardData = GUIUtility.GetClipboardData(13U);
					bool flag2 = clipboardData == IntPtr.Zero;
					if (flag2)
					{
						text = string.Empty;
					}
					else
					{
						IntPtr intPtr = GUIUtility.GlobalLock(clipboardData);
						bool flag3 = intPtr == IntPtr.Zero;
						if (flag3)
						{
							text = string.Empty;
						}
						else
						{
							try
							{
								text = Marshal.PtrToStringUni(intPtr);
							}
							finally
							{
								GUIUtility.GlobalUnlock(clipboardData);
							}
						}
					}
				}
				finally
				{
					GUIUtility.CloseClipboard();
				}
			}
			return text;
		}
		set
		{
			bool flag = string.IsNullOrEmpty(value);
			if (!flag)
			{
				bool flag2 = !GUIUtility.OpenClipboard(IntPtr.Zero);
				if (!flag2)
				{
					try
					{
						GUIUtility.EmptyClipboard();
						IntPtr intPtr = IntPtr.Zero;
						try
						{
							int num = (value.Length + 1) * 2;
							intPtr = GUIUtility.GlobalAlloc(66U, (UIntPtr)((ulong)((long)num)));
							bool flag3 = intPtr == IntPtr.Zero;
							if (!flag3)
							{
								IntPtr intPtr2 = GUIUtility.GlobalLock(intPtr);
								bool flag4 = intPtr2 == IntPtr.Zero;
								if (!flag4)
								{
									try
									{
										Marshal.Copy(value.ToCharArray(), 0, intPtr2, value.Length);
									}
									finally
									{
										GUIUtility.GlobalUnlock(intPtr);
									}
									bool flag5 = GUIUtility.SetClipboardData(13U, intPtr) == IntPtr.Zero;
									if (flag5)
									{
										throw new Win32Exception(Marshal.GetLastWin32Error());
									}
									intPtr = IntPtr.Zero;
								}
							}
						}
						finally
						{
							bool flag6 = intPtr != IntPtr.Zero;
							if (flag6)
							{
								GUIUtility.GlobalFree(intPtr);
							}
						}
					}
					finally
					{
						GUIUtility.CloseClipboard();
					}
				}
			}
		}
	}

	// Token: 0x04000052 RID: 82
	private const uint CF_UNICODETEXT = 13U;

	// Token: 0x04000053 RID: 83
	private const uint GMEM_MOVEABLE = 2U;

	// Token: 0x04000054 RID: 84
	private const uint GMEM_ZEROINIT = 64U;
}
