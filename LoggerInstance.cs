using System;
using System.IO;
using System.Text;

namespace KernelClient
{
	// Token: 0x0200004F RID: 79
	public static class LoggerInstance
	{
		// Token: 0x060001D7 RID: 471 RVA: 0x0000C2BC File Offset: 0x0000A4BC
		public static void Initialize(string logFileName)
		{
			string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BepInEx", "Logs", "VitalityMod");
			bool flag = !Directory.Exists(text);
			if (flag)
			{
				Directory.CreateDirectory(text);
			}
			LoggerInstance._logFilePath = Path.Combine(text, logFileName);
			LoggerInstance.LogInfo("LoggerInstance initialized.");
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x00002AEB File Offset: 0x00000CEB
		public static void LogInfo(string message)
		{
			LoggerInstance.Log("INFO", message, ConsoleColor.White);
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x00002AFC File Offset: 0x00000CFC
		public static void LogWarning(string message)
		{
			LoggerInstance.Log("WARN", message, ConsoleColor.Yellow);
		}

		// Token: 0x060001DA RID: 474 RVA: 0x00002B0D File Offset: 0x00000D0D
		public static void LogError(string message)
		{
			LoggerInstance.Log("ERROR", message, ConsoleColor.Red);
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000C318 File Offset: 0x0000A518
		private static void Log(string level, string message, ConsoleColor color)
		{
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
			string text2 = string.Concat(new string[] { "[", text, "] [", level, "] ", message });
			object @lock = LoggerInstance._lock;
			lock (@lock)
			{
				try
				{
					Console.ForegroundColor = color;
					Console.WriteLine(text2);
					Console.ResetColor();
					File.AppendAllText(LoggerInstance._logFilePath, text2 + Environment.NewLine, Encoding.UTF8);
				}
				catch (Exception ex)
				{
					Console.WriteLine("[LoggerInstance ERROR] Failed to log message: " + ex.Message);
				}
			}
		}

		// Token: 0x0400013A RID: 314
		private static readonly object _lock = new object();

		// Token: 0x0400013B RID: 315
		private static string _logFilePath;
	}
}
