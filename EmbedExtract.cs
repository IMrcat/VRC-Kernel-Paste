using System;
using System.IO;
using System.Reflection;
using MelonLoader;

// Token: 0x02000009 RID: 9
public class EmbedExtract
{
	// Token: 0x06000020 RID: 32 RVA: 0x000047F4 File Offset: 0x000029F4
	public static bool ExtractResource(string resourceName, string outputPath)
	{
		bool flag = File.Exists(outputPath);
		bool flag2;
		if (flag)
		{
			MelonLogger.Msg("File " + outputPath + " already exists. Extraction aborted.");
			flag2 = false;
		}
		else
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(resourceName))
			{
				bool flag3 = manifestResourceStream == null;
				if (flag3)
				{
					MelonLogger.Msg("Resource " + resourceName + " not found.");
					return false;
				}
				string directoryName = Path.GetDirectoryName(outputPath);
				bool flag4 = !Directory.Exists(directoryName);
				if (flag4)
				{
					Directory.CreateDirectory(directoryName);
				}
				using (FileStream fileStream = new FileStream(outputPath, FileMode.Create))
				{
					manifestResourceStream.CopyTo(fileStream);
				}
			}
			MelonLogger.Msg(string.Concat(new string[] { "Resource ", resourceName, " extracted to ", outputPath, "." }));
			flag2 = true;
		}
		return flag2;
	}
}
