﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace SimpleJson
{
	// Token: 0x02000056 RID: 86
	public class SimpleJson
	{
		// Token: 0x06000347 RID: 839 RVA: 0x0001E298 File Offset: 0x0001C498
		public static object DeserializeObject(string json)
		{
			object result;
			if (SimpleJson.TryDeserializeObject(json, out result))
			{
				return result;
			}
			throw new SerializationException("Invalid JSON string");
		}

		// Token: 0x0600034C RID: 844 RVA: 0x0001E34F File Offset: 0x0001C54F
		public static T DeserializeObject<T>(string json)
		{
			return (T)((object)SimpleJson.DeserializeObject(json, typeof(T), null));
		}

		// Token: 0x0600034A RID: 842 RVA: 0x0001E32D File Offset: 0x0001C52D
		public static object DeserializeObject(string json, Type type)
		{
			return SimpleJson.DeserializeObject(json, type, null);
		}

		// Token: 0x0600034B RID: 843 RVA: 0x0001E337 File Offset: 0x0001C537
		public static T DeserializeObject<T>(string json, IJsonSerializerStrategy jsonSerializerStrategy)
		{
			return (T)((object)SimpleJson.DeserializeObject(json, typeof(T), jsonSerializerStrategy));
		}

		// Token: 0x06000349 RID: 841 RVA: 0x0001E2EC File Offset: 0x0001C4EC
		public static object DeserializeObject(string json, Type type, IJsonSerializerStrategy jsonSerializerStrategy)
		{
			object obj = SimpleJson.DeserializeObject(json);
			if (!(type == null) && (obj == null || !obj.GetType().IsAssignableFrom(type)))
			{
				return (jsonSerializerStrategy ?? SimpleJson.CurrentJsonSerializerStrategy).DeserializeObject(obj, type);
			}
			return obj;
		}

		// Token: 0x06000355 RID: 853 RVA: 0x0001E80A File Offset: 0x0001CA0A
		protected static void EatWhitespace(char[] json, ref int index)
		{
			while (index < json.Length && " \t\n\r\b\f".IndexOf(json[index]) != -1)
			{
				index++;
			}
		}

		// Token: 0x06000358 RID: 856 RVA: 0x0001E9B8 File Offset: 0x0001CBB8
		protected static string GetIndentString(int indent)
		{
			string text = "";
			for (int i = 0; i < indent; i++)
			{
				text += "\t";
			}
			return text;
		}

		// Token: 0x06000354 RID: 852 RVA: 0x0001E7DC File Offset: 0x0001C9DC
		protected static int GetLastIndexOfNumber(char[] json, int index)
		{
			int num = index;
			while (num < json.Length && "0123456789+-.eE".IndexOf(json[num]) != -1)
			{
				num++;
			}
			return num - 1;
		}

		// Token: 0x0600035E RID: 862 RVA: 0x0001EE54 File Offset: 0x0001D054
		protected static bool IsNumeric(object value)
		{
			return value is sbyte || value is byte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong || value is float || value is double || value is decimal;
		}

		// Token: 0x06000356 RID: 854 RVA: 0x0001E82C File Offset: 0x0001CA2C
		protected static int LookAhead(char[] json, int index)
		{
			int num = index;
			return SimpleJson.NextToken(json, ref num);
		}

		// Token: 0x06000357 RID: 855 RVA: 0x0001E844 File Offset: 0x0001CA44
		protected static int NextToken(char[] json, ref int index)
		{
			SimpleJson.EatWhitespace(json, ref index);
			if (index == json.Length)
			{
				return 0;
			}
			char c = json[index];
			index++;
			if (c <= '[')
			{
				switch (c)
				{
				case '"':
					return 7;
				case '#':
				case '$':
				case '%':
				case '&':
				case '\'':
				case '(':
				case ')':
				case '*':
				case '+':
				case '.':
				case '/':
					break;
				case ',':
					return 6;
				case '-':
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					return 8;
				case ':':
					return 5;
				default:
					if (c == '[')
					{
						return 3;
					}
					break;
				}
			}
			else
			{
				if (c == ']')
				{
					return 4;
				}
				if (c == '{')
				{
					return 1;
				}
				if (c == '}')
				{
					return 2;
				}
			}
			index--;
			int num = json.Length - index;
			if (num >= 5 && json[index] == 'f' && json[index + 1] == 'a' && json[index + 2] == 'l' && json[index + 3] == 's' && json[index + 4] == 'e')
			{
				index += 5;
				return 10;
			}
			if (num >= 4 && json[index] == 't' && json[index + 1] == 'r' && json[index + 2] == 'u' && json[index + 3] == 'e')
			{
				index += 4;
				return 9;
			}
			if (num >= 4 && json[index] == 'n' && json[index + 1] == 'u' && json[index + 2] == 'l' && json[index + 3] == 'l')
			{
				index += 4;
				return 11;
			}
			return 0;
		}

		// Token: 0x06000350 RID: 848 RVA: 0x0001E430 File Offset: 0x0001C630
		protected static JsonArray ParseArray(char[] json, ref int index, ref bool success)
		{
			JsonArray jsonArray = new JsonArray();
			SimpleJson.NextToken(json, ref index);
			bool flag = false;
			while (!flag)
			{
				int num = SimpleJson.LookAhead(json, index);
				if (num == 0)
				{
					success = false;
					return null;
				}
				if (num == 6)
				{
					SimpleJson.NextToken(json, ref index);
				}
				else
				{
					if (num == 4)
					{
						SimpleJson.NextToken(json, ref index);
						break;
					}
					object item = SimpleJson.ParseValue(json, ref index, ref success);
					if (!success)
					{
						return null;
					}
					jsonArray.Add(item);
				}
			}
			return jsonArray;
		}

		// Token: 0x06000353 RID: 851 RVA: 0x0001E740 File Offset: 0x0001C940
		protected static object ParseNumber(char[] json, ref int index, ref bool success)
		{
			SimpleJson.EatWhitespace(json, ref index);
			int lastIndexOfNumber = SimpleJson.GetLastIndexOfNumber(json, index);
			int length = lastIndexOfNumber - index + 1;
			string text = new string(json, index, length);
			object result;
			if (text.IndexOf(".", StringComparison.OrdinalIgnoreCase) != -1 || text.IndexOf("e", StringComparison.OrdinalIgnoreCase) != -1)
			{
				double num;
				success = double.TryParse(new string(json, index, length), NumberStyles.Any, CultureInfo.InvariantCulture, out num);
				result = num;
			}
			else
			{
				long num2;
				success = long.TryParse(new string(json, index, length), NumberStyles.Any, CultureInfo.InvariantCulture, out num2);
				result = num2;
			}
			index = lastIndexOfNumber + 1;
			return result;
		}

		// Token: 0x0600034F RID: 847 RVA: 0x0001E3A0 File Offset: 0x0001C5A0
		protected static IDictionary<string, object> ParseObject(char[] json, ref int index, ref bool success)
		{
			IDictionary<string, object> dictionary = new JsonObject();
			SimpleJson.NextToken(json, ref index);
			bool flag = false;
			while (!flag)
			{
				int num = SimpleJson.LookAhead(json, index);
				if (num == 0)
				{
					success = false;
					return null;
				}
				if (num == 6)
				{
					SimpleJson.NextToken(json, ref index);
				}
				else
				{
					if (num == 2)
					{
						SimpleJson.NextToken(json, ref index);
						return dictionary;
					}
					string key = SimpleJson.ParseString(json, ref index, ref success);
					if (!success)
					{
						success = false;
						return null;
					}
					num = SimpleJson.NextToken(json, ref index);
					if (num != 5)
					{
						success = false;
						return null;
					}
					object value = SimpleJson.ParseValue(json, ref index, ref success);
					if (!success)
					{
						success = false;
						return null;
					}
					dictionary[key] = value;
				}
			}
			return dictionary;
		}

		// Token: 0x06000352 RID: 850 RVA: 0x0001E538 File Offset: 0x0001C738
		protected static string ParseString(char[] json, ref int index, ref bool success)
		{
			StringBuilder stringBuilder = new StringBuilder(2000);
			SimpleJson.EatWhitespace(json, ref index);
			int num = index;
			index = num + 1;
			char c = json[num];
			bool flag = false;
			while (!flag && index != json.Length)
			{
				num = index;
				index = num + 1;
				c = json[num];
				if (c == '"')
				{
					flag = true;
					break;
				}
				if (c == '\\')
				{
					if (index == json.Length)
					{
						break;
					}
					num = index;
					index = num + 1;
					c = json[num];
					if (c == '"')
					{
						stringBuilder.Append('"');
					}
					else if (c == '\\')
					{
						stringBuilder.Append('\\');
					}
					else if (c == '/')
					{
						stringBuilder.Append('/');
					}
					else if (c == 'b')
					{
						stringBuilder.Append('\b');
					}
					else if (c == 'f')
					{
						stringBuilder.Append('\f');
					}
					else if (c == 'n')
					{
						stringBuilder.Append('\n');
					}
					else if (c == 'r')
					{
						stringBuilder.Append('\r');
					}
					else if (c == 't')
					{
						stringBuilder.Append('\t');
					}
					else if (c == 'u')
					{
						if (json.Length - index < 4)
						{
							break;
						}
						uint num2;
						if (!(success = uint.TryParse(new string(json, index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num2)))
						{
							return "";
						}
						if (55296u <= num2 && num2 <= 56319u)
						{
							index += 4;
							uint num3;
							if (json.Length - index < 6 || !(new string(json, index, 2) == "\\u") || !uint.TryParse(new string(json, index + 2, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num3) || 56320u > num3 || num3 > 57343u)
							{
								success = false;
								return "";
							}
							stringBuilder.Append((char)num2);
							stringBuilder.Append((char)num3);
							index += 6;
						}
						else
						{
							stringBuilder.Append(char.ConvertFromUtf32((int)num2));
							index += 4;
						}
					}
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			if (!flag)
			{
				success = false;
				return null;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000351 RID: 849 RVA: 0x0001E498 File Offset: 0x0001C698
		protected static object ParseValue(char[] json, ref int index, ref bool success)
		{
			switch (SimpleJson.LookAhead(json, index))
			{
			case 1:
				return SimpleJson.ParseObject(json, ref index, ref success);
			case 3:
				return SimpleJson.ParseArray(json, ref index, ref success);
			case 7:
				return SimpleJson.ParseString(json, ref index, ref success);
			case 8:
				return SimpleJson.ParseNumber(json, ref index, ref success);
			case 9:
				SimpleJson.NextToken(json, ref index);
				return true;
			case 10:
				SimpleJson.NextToken(json, ref index);
				return false;
			case 11:
				SimpleJson.NextToken(json, ref index);
				return null;
			}
			success = false;
			return null;
		}

		// Token: 0x0600035B RID: 859 RVA: 0x0001EBB4 File Offset: 0x0001CDB4
		protected static bool SerializeArray(IJsonSerializerStrategy jsonSerializerStrategy, IEnumerable anArray, StringBuilder builder, int indent)
		{
			builder.Append("[\r\n");
			bool flag = true;
			foreach (object current in anArray)
			{
				if (!flag)
				{
					builder.Append(",\r\n");
				}
				builder.Append(SimpleJson.GetIndentString(indent));
				if (!SimpleJson.SerializeValue(jsonSerializerStrategy, current, builder, indent + 1))
				{
					return false;
				}
				flag = false;
			}
			builder.Append("\r\n");
			builder.Append(SimpleJson.GetIndentString(indent - 1));
			builder.Append("]");
			return true;
		}

		// Token: 0x0600035D RID: 861 RVA: 0x0001ED3C File Offset: 0x0001CF3C
		protected static bool SerializeNumber(object number, StringBuilder builder)
		{
			if (number is long)
			{
				builder.Append(((long)number).ToString(CultureInfo.InvariantCulture));
			}
			else if (number is ulong)
			{
				builder.Append(((ulong)number).ToString(CultureInfo.InvariantCulture));
			}
			else if (number is int)
			{
				builder.Append(((int)number).ToString(CultureInfo.InvariantCulture));
			}
			else if (number is uint)
			{
				builder.Append(((uint)number).ToString(CultureInfo.InvariantCulture));
			}
			else if (number is decimal)
			{
				builder.Append(((decimal)number).ToString(CultureInfo.InvariantCulture));
			}
			else if (number is float)
			{
				builder.Append(((float)number).ToString(CultureInfo.InvariantCulture));
			}
			else
			{
				builder.Append(Convert.ToDouble(number, CultureInfo.InvariantCulture).ToString("r", CultureInfo.InvariantCulture));
			}
			return true;
		}

		// Token: 0x0600034E RID: 846 RVA: 0x0001E393 File Offset: 0x0001C593
		public static string SerializeObject(object json)
		{
			return SimpleJson.SerializeObject(json, SimpleJson.CurrentJsonSerializerStrategy);
		}

		// Token: 0x0600034D RID: 845 RVA: 0x0001E368 File Offset: 0x0001C568
		public static string SerializeObject(object json, IJsonSerializerStrategy jsonSerializerStrategy)
		{
			StringBuilder stringBuilder = new StringBuilder(2000);
			if (!SimpleJson.SerializeValue(jsonSerializerStrategy, json, stringBuilder, 1))
			{
				return null;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600035A RID: 858 RVA: 0x0001EAD8 File Offset: 0x0001CCD8
		protected static bool SerializeObject(IJsonSerializerStrategy jsonSerializerStrategy, IEnumerable keys, IEnumerable values, StringBuilder builder, int indent)
		{
			builder.Append("{\r\n");
			IEnumerator enumerator = keys.GetEnumerator();
			IEnumerator enumerator2 = values.GetEnumerator();
			bool flag = true;
			while (enumerator.MoveNext() && enumerator2.MoveNext())
			{
				object current = enumerator.Current;
				object current2 = enumerator2.Current;
				if (!flag)
				{
					builder.Append(",\r\n");
				}
				if (current is string)
				{
					builder.Append(SimpleJson.GetIndentString(indent));
					SimpleJson.SerializeString((string)current, builder);
				}
				else if (!SimpleJson.SerializeValue(jsonSerializerStrategy, current2, builder, indent + 1))
				{
					return false;
				}
				builder.Append(" : ");
				if (!SimpleJson.SerializeValue(jsonSerializerStrategy, current2, builder, indent + 1))
				{
					return false;
				}
				flag = false;
			}
			builder.Append("\r\n");
			builder.Append(SimpleJson.GetIndentString(indent - 1));
			builder.Append("}");
			return true;
		}

		// Token: 0x0600035C RID: 860 RVA: 0x0001EC68 File Offset: 0x0001CE68
		protected static bool SerializeString(string aString, StringBuilder builder)
		{
			builder.Append("\"");
			char[] array = aString.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				char c = array[i];
				if (c == '"')
				{
					builder.Append("\\\"");
				}
				else if (c == '\\')
				{
					builder.Append("\\\\");
				}
				else if (c == '\b')
				{
					builder.Append("\\b");
				}
				else if (c == '\f')
				{
					builder.Append("\\f");
				}
				else if (c == '\n')
				{
					builder.Append("\\n");
				}
				else if (c == '\r')
				{
					builder.Append("\\r");
				}
				else if (c == '\t')
				{
					builder.Append("\\t");
				}
				else
				{
					builder.Append(c);
				}
			}
			builder.Append("\"");
			return true;
		}

		// Token: 0x06000359 RID: 857 RVA: 0x0001E9E4 File Offset: 0x0001CBE4
		protected static bool SerializeValue(IJsonSerializerStrategy jsonSerializerStrategy, object value, StringBuilder builder, int indent = 1)
		{
			bool flag = true;
			if (value is string)
			{
				flag = SimpleJson.SerializeString((string)value, builder);
			}
			else if (value is IDictionary<string, object>)
			{
				IDictionary<string, object> dictionary = (IDictionary<string, object>)value;
				flag = SimpleJson.SerializeObject(jsonSerializerStrategy, dictionary.Keys, dictionary.Values, builder, indent);
			}
			else if (value is IDictionary<string, string>)
			{
				IDictionary<string, string> dictionary2 = (IDictionary<string, string>)value;
				flag = SimpleJson.SerializeObject(jsonSerializerStrategy, dictionary2.Keys, dictionary2.Values, builder, indent);
			}
			else if (value is IEnumerable)
			{
				flag = SimpleJson.SerializeArray(jsonSerializerStrategy, (IEnumerable)value, builder, indent);
			}
			else if (SimpleJson.IsNumeric(value))
			{
				flag = SimpleJson.SerializeNumber(value, builder);
			}
			else if (value is bool)
			{
				builder.Append(((bool)value) ? "true" : "false");
			}
			else if (value == null)
			{
				builder.Append("null");
			}
			else
			{
				object value2;
				flag = jsonSerializerStrategy.SerializeNonPrimitiveObject(value, out value2);
				if (flag)
				{
					SimpleJson.SerializeValue(jsonSerializerStrategy, value2, builder, indent);
				}
			}
			return flag;
		}

		// Token: 0x06000348 RID: 840 RVA: 0x0001E2BC File Offset: 0x0001C4BC
		public static bool TryDeserializeObject(string json, out object @object)
		{
			bool result = true;
			if (json != null)
			{
				char[] json2 = json.ToCharArray();
				int num = 0;
				@object = SimpleJson.ParseValue(json2, ref num, ref result);
			}
			else
			{
				@object = null;
			}
			return result;
		}

		// Token: 0x17000028 RID: 40
		public static IJsonSerializerStrategy CurrentJsonSerializerStrategy
		{
			// Token: 0x0600035F RID: 863 RVA: 0x0001EED0 File Offset: 0x0001D0D0
			get
			{
				IJsonSerializerStrategy arg_14_0;
				if ((arg_14_0 = SimpleJson.currentJsonSerializerStrategy) == null)
				{
					arg_14_0 = (SimpleJson.currentJsonSerializerStrategy = SimpleJson.PocoJsonSerializerStrategy);
				}
				return arg_14_0;
			}
			// Token: 0x06000360 RID: 864 RVA: 0x0001EEE6 File Offset: 0x0001D0E6
			set
			{
				SimpleJson.currentJsonSerializerStrategy = value;
			}
		}

		// Token: 0x17000029 RID: 41
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static PocoJsonSerializerStrategy PocoJsonSerializerStrategy
		{
			// Token: 0x06000361 RID: 865 RVA: 0x0001EEEE File Offset: 0x0001D0EE
			get
			{
				PocoJsonSerializerStrategy arg_14_0;
				if ((arg_14_0 = SimpleJson.pocoJsonSerializerStrategy) == null)
				{
					arg_14_0 = (SimpleJson.pocoJsonSerializerStrategy = new PocoJsonSerializerStrategy());
				}
				return arg_14_0;
			}
		}

		// Token: 0x0400026F RID: 623
		private const int BUILDER_CAPACITY = 2000;

		// Token: 0x04000270 RID: 624
		private static IJsonSerializerStrategy currentJsonSerializerStrategy;

		// Token: 0x04000271 RID: 625
		private static PocoJsonSerializerStrategy pocoJsonSerializerStrategy;

		// Token: 0x04000268 RID: 616
		private const int TOKEN_COLON = 5;

		// Token: 0x04000269 RID: 617
		private const int TOKEN_COMMA = 6;

		// Token: 0x04000265 RID: 613
		private const int TOKEN_CURLY_CLOSE = 2;

		// Token: 0x04000264 RID: 612
		private const int TOKEN_CURLY_OPEN = 1;

		// Token: 0x0400026D RID: 621
		private const int TOKEN_FALSE = 10;

		// Token: 0x04000263 RID: 611
		private const int TOKEN_NONE = 0;

		// Token: 0x0400026E RID: 622
		private const int TOKEN_NULL = 11;

		// Token: 0x0400026B RID: 619
		private const int TOKEN_NUMBER = 8;

		// Token: 0x04000267 RID: 615
		private const int TOKEN_SQUARED_CLOSE = 4;

		// Token: 0x04000266 RID: 614
		private const int TOKEN_SQUARED_OPEN = 3;

		// Token: 0x0400026A RID: 618
		private const int TOKEN_STRING = 7;

		// Token: 0x0400026C RID: 620
		private const int TOKEN_TRUE = 9;
	}
}
