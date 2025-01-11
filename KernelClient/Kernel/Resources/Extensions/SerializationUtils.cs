using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Il2CppSystem;
using Il2CppSystem.IO;
using Il2CppSystem.Runtime.Serialization.Formatters.Binary;

namespace Kernel.Resources.Extensions
{
	// Token: 0x02000045 RID: 69
	internal static class SerializationUtils
	{
		// Token: 0x060001A0 RID: 416 RVA: 0x0000B56C File Offset: 0x0000976C
		internal static byte[] ToByteArray(Object obj)
		{
			bool flag = obj == null;
			byte[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = memoryStream;
				binaryFormatter.Serialize(memoryStream2, obj);
				array = memoryStream.ToArray();
			}
			return array;
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x0000B5B0 File Offset: 0x000097B0
		internal static byte[] ToByteArray(object obj)
		{
			bool flag = obj == null;
			byte[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = memoryStream;
				binaryFormatter.Serialize(memoryStream2, obj);
				array = memoryStream.ToArray();
			}
			return array;
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x0000B5F0 File Offset: 0x000097F0
		internal static T FromByteArray<T>(byte[] data)
		{
			return (data == null) ? default(T) : ((T)((object)new BinaryFormatter().Deserialize(new MemoryStream(data))));
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x0000B628 File Offset: 0x00009828
		internal static T IL2CPPFromByteArray<T>(byte[] data)
		{
			bool flag = data == null;
			T t;
			if (flag)
			{
				t = default(T);
			}
			else
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				MemoryStream memoryStream = new MemoryStream(data);
				Object @object = binaryFormatter.Deserialize(memoryStream);
				t = (T)((object)@object);
			}
			return t;
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x0000B674 File Offset: 0x00009874
		internal static T FromIL2CPPToManaged<T>(Object obj)
		{
			return SerializationUtils.FromByteArray<T>(SerializationUtils.ToByteArray(obj));
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000B694 File Offset: 0x00009894
		internal static T FromManagedToIL2CPP<T>(object obj)
		{
			return SerializationUtils.IL2CPPFromByteArray<T>(SerializationUtils.ToByteArray(obj));
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x0000B6B4 File Offset: 0x000098B4
		internal static object[] FromIL2CPPArrayToManagedArray(Object[] obj)
		{
			object[] array = new object[obj.Length];
			for (int i = 0; i < obj.Length; i++)
			{
				array[i] = ((obj[i].GetIl2CppType().Attributes != 8192) ? obj[i] : SerializationUtils.FromIL2CPPToManaged<object>(obj[i]));
			}
			return array;
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x0000B708 File Offset: 0x00009908
		internal static Object[] FromManagedArrayToIL2CPPArray(object[] obj)
		{
			Object[] array = new Object[obj.Length];
			for (int i = 0; i < obj.Length; i++)
			{
				array[i] = (Object)((obj[i].GetType().Attributes != TypeAttributes.Serializable) ? obj[i] : SerializationUtils.FromManagedToIL2CPP<Object>(obj[i]));
			}
			return array;
		}
	}
}
