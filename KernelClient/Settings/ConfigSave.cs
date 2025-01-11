using System;
using System.IO;
using System.Reflection;
using MelonLoader;

namespace KernelClient.Settings
{
	// Token: 0x02000068 RID: 104
	public class ConfigSave<T> where T : class, new()
	{
		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060002A5 RID: 677 RVA: 0x00002EAD File Offset: 0x000010AD
		private string FilePath { get; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060002A6 RID: 678 RVA: 0x00002EB5 File Offset: 0x000010B5
		// (set) Token: 0x060002A7 RID: 679 RVA: 0x00002EBD File Offset: 0x000010BD
		public T Config { get; private set; }

		// Token: 0x060002A8 RID: 680 RVA: 0x00002EC6 File Offset: 0x000010C6
		public ConfigSave(string path)
		{
			this.FilePath = path;
			this.CheckConfig();
			this.Config = JsonUtility.FromJson<T>(File.ReadAllText(this.FilePath));
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x0000F7B4 File Offset: 0x0000D9B4
		private void CheckConfig()
		{
			bool flag = !File.Exists(this.FilePath) || new FileInfo(this.FilePath).Length < 2L;
			if (flag)
			{
				T t = new T();
				string text = JsonUtility.ToJson<T>(t, true);
				File.WriteAllText(this.FilePath, text);
			}
		}

		// Token: 0x060002AA RID: 682 RVA: 0x0000F808 File Offset: 0x0000DA08
		private void UpdateConfig(object obj, FileSystemEventArgs args)
		{
			try
			{
				T t = JsonUtility.FromJson<T>(File.ReadAllText(this.FilePath));
				bool flag = t == null;
				if (!flag)
				{
					Type type = t.GetType();
					PropertyInfo[] array = ((type != null) ? type.GetProperties() : null);
					foreach (PropertyInfo propertyInfo in array)
					{
						PropertyInfo property = this.Config.GetType().GetProperty(propertyInfo.Name);
						bool flag2 = property != null && !object.Equals(propertyInfo.GetValue(t), property.GetValue(this.Config));
						if (flag2)
						{
							this.Config = t;
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error("Error updating config: " + ex.Message);
			}
		}

		// Token: 0x060002AB RID: 683 RVA: 0x0000F908 File Offset: 0x0000DB08
		public void Save()
		{
			string text = JsonUtility.ToJson<T>(this.Config, true);
			File.WriteAllText(this.FilePath, text);
		}
	}
}
