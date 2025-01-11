using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace KernelClient
{
	// Token: 0x02000056 RID: 86
	public class SimpleJson
	{
		// Token: 0x0600021D RID: 541 RVA: 0x0000D4A8 File Offset: 0x0000B6A8
		public static string Serialize(object obj)
		{
			bool flag = obj == null;
			string text;
			if (flag)
			{
				text = "null";
			}
			else
			{
				string text2 = obj as string;
				bool flag2 = text2 != null;
				if (flag2)
				{
					text = "\"" + SimpleJson.EscapeString(text2) + "\"";
				}
				else
				{
					bool flag3;
					bool flag4;
					if (obj is bool)
					{
						flag3 = (bool)obj;
						flag4 = true;
					}
					else
					{
						flag4 = false;
					}
					bool flag5 = flag4;
					if (flag5)
					{
						text = (flag3 ? "true" : "false");
					}
					else
					{
						bool flag6 = obj is int || obj is long || obj is double || obj is float || obj is decimal;
						if (flag6)
						{
							text = Convert.ToString(obj, CultureInfo.InvariantCulture);
						}
						else
						{
							Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
							bool flag7 = dictionary != null;
							if (flag7)
							{
								List<string> list = new List<string>();
								foreach (KeyValuePair<string, object> keyValuePair in dictionary)
								{
									list.Add("\"" + SimpleJson.EscapeString(keyValuePair.Key) + "\":" + SimpleJson.Serialize(keyValuePair.Value));
								}
								text = "{" + string.Join(",", list) + "}";
							}
							else
							{
								List<object> list2 = obj as List<object>;
								bool flag8 = list2 != null;
								if (!flag8)
								{
									throw new NotSupportedException(string.Format("Type {0} is not supported for serialization.", obj.GetType()));
								}
								List<string> list3 = new List<string>();
								foreach (object obj2 in list2)
								{
									list3.Add(SimpleJson.Serialize(obj2));
								}
								text = "[" + string.Join(",", list3) + "]";
							}
						}
					}
				}
			}
			return text;
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0000D6B8 File Offset: 0x0000B8B8
		public static object Deserialize(string json)
		{
			int num = 0;
			return SimpleJson.ParseValue(json, ref num);
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000D6D4 File Offset: 0x0000B8D4
		private static object ParseValue(string json, ref int index)
		{
			SimpleJson.SkipWhitespace(json, ref index);
			bool flag = index >= json.Length;
			if (flag)
			{
				throw new Exception("Unexpected end of JSON input.");
			}
			char c = json[index];
			char c2 = c;
			char c3 = c2;
			object obj;
			if (c3 != '"')
			{
				if (c3 != '[')
				{
					if (c3 != '{')
					{
						bool flag2 = char.IsDigit(c) || c == '-';
						if (flag2)
						{
							obj = SimpleJson.ParseNumber(json, ref index);
						}
						else
						{
							bool flag3 = json.Substring(index).StartsWith("true");
							if (flag3)
							{
								index += 4;
								obj = true;
							}
							else
							{
								bool flag4 = json.Substring(index).StartsWith("false");
								if (flag4)
								{
									index += 5;
									obj = false;
								}
								else
								{
									bool flag5 = json.Substring(index).StartsWith("null");
									if (!flag5)
									{
										throw new Exception(string.Format("Unexpected character '{0}' at position {1}.", c, index));
									}
									index += 4;
									obj = null;
								}
							}
						}
					}
					else
					{
						obj = SimpleJson.ParseObject(json, ref index);
					}
				}
				else
				{
					obj = SimpleJson.ParseArray(json, ref index);
				}
			}
			else
			{
				obj = SimpleJson.ParseString(json, ref index);
			}
			return obj;
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000D814 File Offset: 0x0000BA14
		private static Dictionary<string, object> ParseObject(string json, ref int index)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			index++;
			for (;;)
			{
				SimpleJson.SkipWhitespace(json, ref index);
				bool flag = index >= json.Length;
				if (flag)
				{
					break;
				}
				bool flag2 = json[index] == '}';
				if (flag2)
				{
					goto Block_2;
				}
				string text = SimpleJson.ParseString(json, ref index).ToString();
				SimpleJson.SkipWhitespace(json, ref index);
				bool flag3 = json[index] != ':';
				if (flag3)
				{
					goto Block_3;
				}
				index++;
				object obj = SimpleJson.ParseValue(json, ref index);
				dictionary[text] = obj;
				SimpleJson.SkipWhitespace(json, ref index);
				bool flag4 = json[index] == ',';
				if (!flag4)
				{
					goto IL_00D2;
				}
				index++;
			}
			throw new Exception("Unexpected end of JSON input while parsing object.");
			Block_2:
			index++;
			return dictionary;
			Block_3:
			throw new Exception(string.Format("Expected ':' after key in object at position {0}.", index));
			IL_00D2:
			bool flag5 = json[index] == '}';
			if (!flag5)
			{
				throw new Exception(string.Format("Expected ',' or '}}' in object at position {0}.", index));
			}
			index++;
			return dictionary;
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0000D934 File Offset: 0x0000BB34
		private static List<object> ParseArray(string json, ref int index)
		{
			List<object> list = new List<object>();
			index++;
			for (;;)
			{
				SimpleJson.SkipWhitespace(json, ref index);
				bool flag = index >= json.Length;
				if (flag)
				{
					break;
				}
				bool flag2 = json[index] == ']';
				if (flag2)
				{
					goto Block_2;
				}
				object obj = SimpleJson.ParseValue(json, ref index);
				list.Add(obj);
				SimpleJson.SkipWhitespace(json, ref index);
				bool flag3 = json[index] == ',';
				if (!flag3)
				{
					goto IL_0084;
				}
				index++;
			}
			throw new Exception("Unexpected end of JSON input while parsing array.");
			Block_2:
			index++;
			return list;
			IL_0084:
			bool flag4 = json[index] == ']';
			if (!flag4)
			{
				throw new Exception(string.Format("Expected ',' or ']' in array at position {0}.", index));
			}
			index++;
			return list;
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0000DA08 File Offset: 0x0000BC08
		private static string ParseString(string json, ref int index)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (index++; index < json.Length; index++)
			{
				char c = json[index];
				char c2 = c;
				char c3 = c2;
				if (c3 == '"')
				{
					index++;
					return stringBuilder.ToString();
				}
				if (c3 != '\\')
				{
					stringBuilder.Append(c);
				}
				else
				{
					index++;
					bool flag = index >= json.Length;
					if (flag)
					{
						throw new Exception("Unexpected end of JSON input in escape sequence.");
					}
					char c4 = json[index];
					char c5 = c4;
					char c6 = c5;
					if (c6 <= '\\')
					{
						if (c6 != '"')
						{
							if (c6 != '/')
							{
								if (c6 != '\\')
								{
									goto IL_01BD;
								}
								stringBuilder.Append('\\');
							}
							else
							{
								stringBuilder.Append('/');
							}
						}
						else
						{
							stringBuilder.Append('"');
						}
					}
					else if (c6 <= 'f')
					{
						if (c6 != 'b')
						{
							if (c6 != 'f')
							{
								goto IL_01BD;
							}
							stringBuilder.Append('\f');
						}
						else
						{
							stringBuilder.Append('\b');
						}
					}
					else if (c6 != 'n')
					{
						switch (c6)
						{
						case 'r':
							stringBuilder.Append('\r');
							break;
						case 's':
							goto IL_01BD;
						case 't':
							stringBuilder.Append('\t');
							break;
						case 'u':
						{
							bool flag2 = index + 4 >= json.Length;
							if (flag2)
							{
								throw new Exception("Unexpected end of JSON input in unicode escape.");
							}
							string text = json.Substring(index + 1, 4);
							int num;
							bool flag3 = !int.TryParse(text, NumberStyles.HexNumber, null, out num);
							if (flag3)
							{
								throw new Exception("Invalid unicode escape '\\u" + text + "'.");
							}
							stringBuilder.Append((char)num);
							index += 4;
							break;
						}
						default:
							goto IL_01BD;
						}
					}
					else
					{
						stringBuilder.Append('\n');
					}
					goto IL_01E0;
					IL_01BD:
					throw new Exception(string.Format("Invalid escape character '\\{0}'.", c4));
				}
				IL_01E0:;
			}
			throw new Exception("Unexpected end of JSON input while parsing string.");
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0000DC1C File Offset: 0x0000BE1C
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
			bool flag3 = index < json.Length && (json[index] == 'e' || json[index] == 'E');
			if (flag3)
			{
				index++;
				bool flag4 = json[index] == '+' || json[index] == '-';
				if (flag4)
				{
					index++;
				}
				while (index < json.Length && char.IsDigit(json[index]))
				{
					index++;
				}
			}
			string text = json.Substring(num, index - num);
			double num2;
			bool flag5 = double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out num2);
			if (flag5)
			{
				return num2;
			}
			throw new Exception("Invalid number format: '" + text + "'");
		}

		// Token: 0x06000224 RID: 548 RVA: 0x00005E60 File Offset: 0x00004060
		private static void SkipWhitespace(string json, ref int index)
		{
			while (index < json.Length && char.IsWhiteSpace(json[index]))
			{
				index++;
			}
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0000DD8C File Offset: 0x0000BF8C
		private static string EscapeString(string str)
		{
			return str.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\b", "\\b")
				.Replace("\f", "\\f")
				.Replace("\n", "\\n")
				.Replace("\r", "\\r")
				.Replace("\t", "\\t");
		}
	}
}
