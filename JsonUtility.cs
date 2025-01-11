using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using MelonLoader;

// Token: 0x02000011 RID: 17
public static class JsonUtility
{
	// Token: 0x06000049 RID: 73 RVA: 0x00005498 File Offset: 0x00003698
	public static string ToJson<T>(T obj, bool prettyPrint = false)
	{
		StringBuilder stringBuilder = new StringBuilder();
		JsonUtility.SerializeObject(obj, stringBuilder, prettyPrint, 0);
		return stringBuilder.ToString();
	}

	// Token: 0x0600004A RID: 74 RVA: 0x000054C8 File Offset: 0x000036C8
	public static T FromJson<T>(string json) where T : new()
	{
		T t = new T();
		JsonUtility.DeserializeObject(json, t);
		return t;
	}

	// Token: 0x0600004B RID: 75 RVA: 0x000054F0 File Offset: 0x000036F0
	private static void SerializeObject(object obj, StringBuilder sb, bool prettyPrint, int indentLevel)
	{
		bool flag = obj == null;
		if (flag)
		{
			sb.Append("null");
		}
		else
		{
			Type type = obj.GetType();
			bool flag2 = JsonUtility.IsSimple(type);
			if (flag2)
			{
				JsonUtility.SerializeSimple(obj, sb);
			}
			else
			{
				bool flag3 = typeof(IEnumerable<object>).IsAssignableFrom(type);
				if (flag3)
				{
					JsonUtility.SerializeList((IEnumerable<object>)obj, sb, prettyPrint, indentLevel);
				}
				else
				{
					sb.Append("{");
					if (prettyPrint)
					{
						sb.AppendLine();
					}
					bool flag4 = true;
					foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
					{
						bool flag5 = !flag4;
						if (flag5)
						{
							sb.Append(",");
							if (prettyPrint)
							{
								sb.AppendLine();
							}
						}
						flag4 = false;
						if (prettyPrint)
						{
							sb.Append(new string(' ', (indentLevel + 1) * 4));
						}
						sb.Append("\"" + fieldInfo.Name + "\":");
						if (prettyPrint)
						{
							sb.Append(" ");
						}
						JsonUtility.SerializeObject(fieldInfo.GetValue(obj), sb, prettyPrint, indentLevel + 1);
					}
					if (prettyPrint)
					{
						sb.AppendLine();
						sb.Append(new string(' ', indentLevel * 4));
					}
					sb.Append("}");
				}
			}
		}
	}

	// Token: 0x0600004C RID: 76 RVA: 0x00005664 File Offset: 0x00003864
	private static bool IsSimple(Type type)
	{
		return type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || type == typeof(DateTime);
	}

	// Token: 0x0600004D RID: 77 RVA: 0x000056B8 File Offset: 0x000038B8
	private static void SerializeSimple(object obj, StringBuilder sb)
	{
		bool flag = obj is string || obj is char;
		if (flag)
		{
			sb.Append("\"" + JsonUtility.EscapeString(obj.ToString()) + "\"");
		}
		else
		{
			bool flag2 = obj is bool;
			if (flag2)
			{
				sb.Append(((bool)obj) ? "true" : "false");
			}
			else
			{
				DateTime dateTime;
				bool flag3;
				if (obj is DateTime)
				{
					dateTime = (DateTime)obj;
					flag3 = true;
				}
				else
				{
					flag3 = false;
				}
				bool flag4 = flag3;
				if (flag4)
				{
					sb.Append("\"" + dateTime.ToString("o") + "\"");
				}
				else
				{
					sb.Append(Convert.ToString(obj, CultureInfo.InvariantCulture));
				}
			}
		}
	}

	// Token: 0x0600004E RID: 78 RVA: 0x00005780 File Offset: 0x00003980
	private static void SerializeList(IEnumerable<object> list, StringBuilder sb, bool prettyPrint, int indentLevel)
	{
		sb.Append("[");
		if (prettyPrint)
		{
			sb.AppendLine();
		}
		bool flag = true;
		foreach (object obj in list)
		{
			bool flag2 = !flag;
			if (flag2)
			{
				sb.Append(",");
				if (prettyPrint)
				{
					sb.AppendLine();
				}
			}
			flag = false;
			if (prettyPrint)
			{
				sb.Append(new string(' ', (indentLevel + 1) * 4));
			}
			JsonUtility.SerializeObject(obj, sb, prettyPrint, indentLevel + 1);
		}
		if (prettyPrint)
		{
			sb.AppendLine();
			sb.Append(new string(' ', indentLevel * 4));
		}
		sb.Append("]");
	}

	// Token: 0x0600004F RID: 79 RVA: 0x00005860 File Offset: 0x00003A60
	private static string EscapeString(string str)
	{
		return str.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n")
			.Replace("\r", "\\r")
			.Replace("\t", "\\t");
	}

	// Token: 0x06000050 RID: 80 RVA: 0x000058C0 File Offset: 0x00003AC0
	private static void DeserializeObject(string json, object obj)
	{
		int i = 0;
		JsonUtility.SkipWhitespace(json, ref i);
		bool flag = json[i] != '{';
		if (flag)
		{
			throw new Exception("Invalid JSON format. Expected '{' at the beginning.");
		}
		i++;
		while (i < json.Length)
		{
			JsonUtility.SkipWhitespace(json, ref i);
			bool flag2 = json[i] == '}';
			if (flag2)
			{
				i++;
				break;
			}
			string text = JsonUtility.ParseString(json, ref i);
			JsonUtility.SkipWhitespace(json, ref i);
			bool flag3 = json[i] != ':';
			if (flag3)
			{
				throw new Exception("Invalid JSON format. Expected ':' after key.");
			}
			i++;
			JsonUtility.SkipWhitespace(json, ref i);
			object obj2 = JsonUtility.ParseValue(json, ref i);
			JsonUtility.SetFieldValue(obj, text, obj2);
			JsonUtility.SkipWhitespace(json, ref i);
			bool flag4 = json[i] == ',';
			if (flag4)
			{
				i++;
			}
			else
			{
				bool flag5 = json[i] == '}';
				if (flag5)
				{
					i++;
					break;
				}
				throw new Exception("Invalid JSON format. Expected ',' or '}' after value.");
			}
		}
	}

	// Token: 0x06000051 RID: 81 RVA: 0x000059D0 File Offset: 0x00003BD0
	private static object ParseValue(string json, ref int index)
	{
		JsonUtility.SkipWhitespace(json, ref index);
		bool flag = json[index] == '"';
		object obj;
		if (flag)
		{
			obj = JsonUtility.ParseString(json, ref index);
		}
		else
		{
			bool flag2 = json[index] == '{';
			if (flag2)
			{
				throw new NotImplementedException("Nested objects are not supported.");
			}
			bool flag3 = json[index] == '[';
			if (flag3)
			{
				obj = JsonUtility.ParseList(json, ref index);
			}
			else
			{
				bool flag4 = char.IsDigit(json[index]) || json[index] == '-';
				if (flag4)
				{
					obj = JsonUtility.ParseNumber(json, ref index);
				}
				else
				{
					bool flag5 = json.Substring(index).StartsWith("true");
					if (flag5)
					{
						index += 4;
						obj = true;
					}
					else
					{
						bool flag6 = json.Substring(index).StartsWith("false");
						if (flag6)
						{
							index += 5;
							obj = false;
						}
						else
						{
							bool flag7 = json.Substring(index).StartsWith("null");
							if (!flag7)
							{
								throw new Exception(string.Format("Invalid JSON value at position {0}.", index));
							}
							index += 4;
							obj = null;
						}
					}
				}
			}
		}
		return obj;
	}

	// Token: 0x06000052 RID: 82 RVA: 0x00005B04 File Offset: 0x00003D04
	private static string ParseString(string json, ref int index)
	{
		bool flag = json[index] != '"';
		if (flag)
		{
			throw new Exception("Invalid JSON format. Expected '\"' at the beginning of string.");
		}
		index++;
		StringBuilder stringBuilder = new StringBuilder();
		while (index < json.Length)
		{
			bool flag2 = json[index] == '"';
			if (flag2)
			{
				index++;
				break;
			}
			bool flag3 = json[index] == '\\';
			if (flag3)
			{
				index++;
				bool flag4 = index >= json.Length;
				if (flag4)
				{
					throw new Exception("Invalid JSON format. Incomplete escape sequence.");
				}
				char c = json[index];
				char c2 = c;
				char c3 = c2;
				if (c3 <= 'b')
				{
					if (c3 <= '/')
					{
						if (c3 != '"')
						{
							if (c3 != '/')
							{
								goto IL_0145;
							}
							stringBuilder.Append('/');
						}
						else
						{
							stringBuilder.Append('"');
						}
					}
					else if (c3 != '\\')
					{
						if (c3 != 'b')
						{
							goto IL_0145;
						}
						stringBuilder.Append('\b');
					}
					else
					{
						stringBuilder.Append('\\');
					}
				}
				else if (c3 <= 'n')
				{
					if (c3 != 'f')
					{
						if (c3 != 'n')
						{
							goto IL_0145;
						}
						stringBuilder.Append('\n');
					}
					else
					{
						stringBuilder.Append('\f');
					}
				}
				else if (c3 != 'r')
				{
					if (c3 != 't')
					{
						goto IL_0145;
					}
					stringBuilder.Append('\t');
				}
				else
				{
					stringBuilder.Append('\r');
				}
				goto IL_0170;
				IL_0145:
				throw new Exception(string.Format("Invalid escape character '\\{0}' in JSON string.", c));
			}
			else
			{
				stringBuilder.Append(json[index]);
			}
			IL_0170:
			index++;
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06000053 RID: 83 RVA: 0x00005CA8 File Offset: 0x00003EA8
	private static List<object> ParseList(string json, ref int index)
	{
		List<object> list = new List<object>();
		bool flag = json[index] != '[';
		if (flag)
		{
			throw new Exception("Invalid JSON format. Expected '[' at the beginning of list.");
		}
		index++;
		while (index < json.Length)
		{
			JsonUtility.SkipWhitespace(json, ref index);
			bool flag2 = json[index] == ']';
			if (flag2)
			{
				index++;
				break;
			}
			object obj = JsonUtility.ParseValue(json, ref index);
			list.Add(obj);
			JsonUtility.SkipWhitespace(json, ref index);
			bool flag3 = json[index] == ',';
			if (flag3)
			{
				index++;
			}
			else
			{
				bool flag4 = json[index] == ']';
				if (flag4)
				{
					index++;
					break;
				}
				throw new Exception("Invalid JSON format. Expected ',' or ']' after list item.");
			}
		}
		return list;
	}

	// Token: 0x06000054 RID: 84 RVA: 0x00005D7C File Offset: 0x00003F7C
	private static double ParseNumber(string json, ref int index)
	{
		int num = index;
		bool flag = json[index] == '-';
		if (flag)
		{
			index++;
		}
		while (index < json.Length && char.IsDigit(json[index]))
		{
			index++;
		}
		bool flag2 = index < json.Length && json[index] == '.';
		if (flag2)
		{
			index++;
			while (index < json.Length && char.IsDigit(json[index]))
			{
				index++;
			}
		}
		string text = json.Substring(num, index - num);
		double num2;
		bool flag3 = double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out num2);
		if (flag3)
		{
			return num2;
		}
		throw new Exception("Invalid number format: " + text);
	}

	// Token: 0x06000055 RID: 85 RVA: 0x00005E60 File Offset: 0x00004060
	private static void SkipWhitespace(string json, ref int index)
	{
		while (index < json.Length && char.IsWhiteSpace(json[index]))
		{
			index++;
		}
	}

	// Token: 0x06000056 RID: 86 RVA: 0x00005E98 File Offset: 0x00004098
	private static void SetFieldValue(object obj, string fieldName, object value)
	{
		Type type = obj.GetType();
		FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
		bool flag = field != null;
		if (flag)
		{
			bool flag2 = value == null;
			if (flag2)
			{
				field.SetValue(obj, null);
			}
			else
			{
				Type fieldType = field.FieldType;
				bool flag3 = fieldType == typeof(int);
				if (flag3)
				{
					field.SetValue(obj, Convert.ToInt32(value));
				}
				else
				{
					bool flag4 = fieldType == typeof(float);
					if (flag4)
					{
						field.SetValue(obj, Convert.ToSingle(value));
					}
					else
					{
						bool flag5 = fieldType == typeof(double);
						if (flag5)
						{
							field.SetValue(obj, Convert.ToDouble(value));
						}
						else
						{
							bool flag6 = fieldType == typeof(bool);
							if (flag6)
							{
								field.SetValue(obj, Convert.ToBoolean(value));
							}
							else
							{
								bool flag7 = fieldType == typeof(string);
								if (flag7)
								{
									field.SetValue(obj, value.ToString());
								}
								else
								{
									bool flag8 = fieldType == typeof(DateTime);
									if (flag8)
									{
										field.SetValue(obj, DateTime.Parse(value.ToString()));
									}
									else
									{
										bool flag9 = fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>);
										if (flag9)
										{
											Type type2 = fieldType.GetGenericArguments()[0];
											List<object> list = (List<object>)value;
											IList<object> list2 = (IList<object>)Activator.CreateInstance(fieldType);
											foreach (object obj2 in list)
											{
												list2.Add(Convert.ChangeType(obj2, type2));
											}
											field.SetValue(obj, list2);
										}
										else
										{
											field.SetValue(obj, Convert.ChangeType(value, fieldType));
										}
									}
								}
							}
						}
					}
				}
			}
		}
		else
		{
			MelonLogger.Warning(string.Concat(new string[] { "Field '", fieldName, "' not found on type '", type.FullName, "'." }));
		}
	}
}
