using System;
using System.Diagnostics;
using System.Reflection;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine;
using VRC;

namespace KernelClient
{
	// Token: 0x02000058 RID: 88
	public static class ObjectHandler
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x0600022A RID: 554 RVA: 0x0000DE70 File Offset: 0x0000C070
		// (remove) Token: 0x0600022B RID: 555 RVA: 0x0000DEA4 File Offset: 0x0000C0A4
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<GameObject> OnObjectInstantiated;

		// Token: 0x0600022C RID: 556 RVA: 0x0000DED8 File Offset: 0x0000C0D8
		static ObjectHandler()
		{
			try
			{
				foreach (FieldInfo fieldInfo in typeof(NetworkManager).GetFields())
				{
					bool flag = fieldInfo.FieldType.ToString().Contains("VRCEventDelegate");
					if (flag)
					{
						object value = fieldInfo.GetValue(NetworkManager.field_Internal_Static_NetworkManager_0);
						Type type = value.GetType();
						bool flag2 = type.ToString().Contains("Player_GameObject");
						if (flag2)
						{
							FieldInfo fieldInfo2 = type.GetFields()[0];
							object value2 = fieldInfo2.GetValue(value);
							bool flag3 = value2 != null;
							if (flag3)
							{
								MethodInfo method = value2.GetType().GetMethod("Add");
								Action<Player, GameObject> action = new Action<Player, GameObject>(ObjectHandler.OnObjectInstantiatedEvent);
								if (method != null)
								{
									method.Invoke(value2, new object[] { action });
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("Failed to initialize ObjectHandler: {0}", ex));
			}
		}

		// Token: 0x0600022D RID: 557 RVA: 0x0000DFF4 File Offset: 0x0000C1F4
		private static void OnObjectInstantiatedEvent(Player player, GameObject obj)
		{
			try
			{
				bool flag = obj == null;
				if (!flag)
				{
					int instanceID = obj.GetInstanceID();
					bool flag2 = ObjectHandler._processedObjects.Contains(instanceID);
					if (!flag2)
					{
						ObjectHandler._processedObjects.Add(instanceID);
						Action<GameObject> onObjectInstantiated = ObjectHandler.OnObjectInstantiated;
						if (onObjectInstantiated != null)
						{
							onObjectInstantiated(obj);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("Error in OnObjectInstantiatedEvent: {0}", ex));
			}
		}

		// Token: 0x0600022E RID: 558 RVA: 0x0000E070 File Offset: 0x0000C270
		public static void ForceCleanup()
		{
			try
			{
				ObjectHandler._processedObjects.Clear();
			}
			catch (Exception ex)
			{
				MelonLogger.Error(string.Format("Error in ForceCleanup: {0}", ex));
			}
		}

		// Token: 0x0600022F RID: 559 RVA: 0x0000E0B4 File Offset: 0x0000C2B4
		public static void ProcessObject(GameObject obj)
		{
			bool flag = obj != null;
			if (flag)
			{
				ObjectHandler.OnObjectInstantiatedEvent(Player.Method_Internal_Static_get_Player_0(), obj);
			}
		}

		// Token: 0x06000230 RID: 560 RVA: 0x0000E0DC File Offset: 0x0000C2DC
		public static bool HasBeenProcessed(GameObject obj)
		{
			return obj != null && ObjectHandler._processedObjects.Contains(obj.GetInstanceID());
		}

		// Token: 0x06000231 RID: 561 RVA: 0x00002C98 File Offset: 0x00000E98
		public static void Reset()
		{
			ObjectHandler._processedObjects.Clear();
		}

		// Token: 0x06000232 RID: 562 RVA: 0x00002CA6 File Offset: 0x00000EA6
		public static void OnWorldLoad()
		{
			ObjectHandler.Reset();
		}

		// Token: 0x0400015D RID: 349
		private static readonly HashSet<int> _processedObjects = new HashSet<int>();
	}
}
