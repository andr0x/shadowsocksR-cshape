﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using SimpleJson.Reflection;

namespace SimpleJson
{
	// Token: 0x02000058 RID: 88
	public class PocoJsonSerializerStrategy : IJsonSerializerStrategy
	{
		// Token: 0x06000365 RID: 869 RVA: 0x0001EF04 File Offset: 0x0001D104
		public PocoJsonSerializerStrategy()
		{
			this.CacheResolver = new CacheResolver(new MemberMapLoader(this.BuildMap));
		}

		// Token: 0x06000366 RID: 870 RVA: 0x0001EF24 File Offset: 0x0001D124
		protected virtual void BuildMap(Type type, SafeDictionary<string, CacheResolver.MemberMap> memberMaps)
		{
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			for (int i = 0; i < properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				memberMaps.Add(propertyInfo.Name, new CacheResolver.MemberMap(propertyInfo));
			}
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				memberMaps.Add(fieldInfo.Name, new CacheResolver.MemberMap(fieldInfo));
			}
		}

		// Token: 0x06000368 RID: 872 RVA: 0x0001EFA4 File Offset: 0x0001D1A4
		public virtual object DeserializeObject(object value, Type type)
		{
			object obj = null;
			if (value is string)
			{
				string text = value as string;
				if (!string.IsNullOrEmpty(text))
				{
					if (type == typeof(DateTime) || (ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(DateTime)))
					{
						obj = DateTime.ParseExact(text, PocoJsonSerializerStrategy.Iso8601Format, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
					}
					else if (type == typeof(Guid) || (ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(Guid)))
					{
						obj = new Guid(text);
					}
					else
					{
						obj = text;
					}
				}
				else if (type == typeof(Guid))
				{
					obj = default(Guid);
				}
				else if (ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(Guid))
				{
					obj = null;
				}
				else
				{
					obj = text;
				}
			}
			else if (value is bool)
			{
				obj = value;
			}
			else if (value == null)
			{
				obj = null;
			}
			else if ((value is long && type == typeof(long)) || (value is double && type == typeof(double)))
			{
				obj = value;
			}
			else
			{
				if ((!(value is double) || !(type != typeof(double))) && (!(value is long) || !(type != typeof(long))))
				{
					if (value is IDictionary<string, object>)
					{
						IDictionary<string, object> dictionary = (IDictionary<string, object>)value;
						if (ReflectionUtils.IsTypeDictionary(type))
						{
							Type type2 = type.GetGenericArguments()[0];
							Type type3 = type.GetGenericArguments()[1];
							IDictionary dictionary2 = (IDictionary)CacheResolver.GetNewInstance(typeof(Dictionary<, >).MakeGenericType(new Type[]
							{
								type2,
								type3
							}));
							foreach (KeyValuePair<string, object> current in dictionary)
							{
								dictionary2.Add(current.Key, this.DeserializeObject(current.Value, type3));
							}
							obj = dictionary2;
							return obj;
						}
						obj = CacheResolver.GetNewInstance(type);
						SafeDictionary<string, CacheResolver.MemberMap> safeDictionary = this.CacheResolver.LoadMaps(type);
						if (safeDictionary == null)
						{
							obj = value;
							return obj;
						}
						using (IEnumerator<KeyValuePair<string, CacheResolver.MemberMap>> enumerator2 = safeDictionary.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								KeyValuePair<string, CacheResolver.MemberMap> current2 = enumerator2.Current;
								CacheResolver.MemberMap value2 = current2.Value;
								if (value2.Setter != null)
								{
									string key = current2.Key;
									if (dictionary.ContainsKey(key))
									{
										object value3 = this.DeserializeObject(dictionary[key], value2.Type);
										value2.Setter(obj, value3);
									}
								}
							}
							return obj;
						}
					}
					if (value is IList<object>)
					{
						IList<object> list = (IList<object>)value;
						IList list2 = null;
						if (type.IsArray)
						{
							list2 = (IList)Activator.CreateInstance(type, new object[]
							{
								list.Count
							});
							int num = 0;
							using (IEnumerator<object> enumerator3 = list.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									object current3 = enumerator3.Current;
									list2[num++] = this.DeserializeObject(current3, type.GetElementType());
								}
								goto IL_410;
							}
						}
						if (ReflectionUtils.IsTypeGenericeCollectionInterface(type) || typeof(IList).IsAssignableFrom(type))
						{
							Type type4 = type.GetGenericArguments()[0];
							list2 = (IList)CacheResolver.GetNewInstance(typeof(List<>).MakeGenericType(new Type[]
							{
								type4
							}));
							foreach (object current4 in list)
							{
								list2.Add(this.DeserializeObject(current4, type4));
							}
						}
						IL_410:
						obj = list2;
					}
					return obj;
				}
				obj = (typeof(IConvertible).IsAssignableFrom(type) ? Convert.ChangeType(value, type, CultureInfo.InvariantCulture) : value);
			}
			if (ReflectionUtils.IsNullableType(type))
			{
				return ReflectionUtils.ToNullableType(obj, type);
			}
			if (obj == null && type == typeof(Guid))
			{
				return default(Guid);
			}
			return obj;
		}

		// Token: 0x06000369 RID: 873 RVA: 0x0001F430 File Offset: 0x0001D630
		protected virtual object SerializeEnum(Enum p)
		{
			return Convert.ToDouble(p, CultureInfo.InvariantCulture);
		}

		// Token: 0x06000367 RID: 871 RVA: 0x0001EF8E File Offset: 0x0001D18E
		public virtual bool SerializeNonPrimitiveObject(object input, out object output)
		{
			return this.TrySerializeKnownTypes(input, out output) || this.TrySerializeUnknownTypes(input, out output);
		}

		// Token: 0x0600036A RID: 874 RVA: 0x0001F444 File Offset: 0x0001D644
		protected virtual bool TrySerializeKnownTypes(object input, out object output)
		{
			bool result = true;
			if (input is DateTime)
			{
				output = ((DateTime)input).ToUniversalTime().ToString(PocoJsonSerializerStrategy.Iso8601Format[0], CultureInfo.InvariantCulture);
			}
			else if (input is Guid)
			{
				output = ((Guid)input).ToString("D");
			}
			else if (input is Uri)
			{
				output = input.ToString();
			}
			else if (input is Enum)
			{
				output = this.SerializeEnum((Enum)input);
			}
			else
			{
				result = false;
				output = null;
			}
			return result;
		}

		// Token: 0x0600036B RID: 875 RVA: 0x0001F4D0 File Offset: 0x0001D6D0
		protected virtual bool TrySerializeUnknownTypes(object input, out object output)
		{
			output = null;
			Type type = input.GetType();
			if (type.FullName == null)
			{
				return false;
			}
			IDictionary<string, object> dictionary = new JsonObject();
			foreach (KeyValuePair<string, CacheResolver.MemberMap> current in this.CacheResolver.LoadMaps(type))
			{
				if (current.Value.Getter != null)
				{
					dictionary.Add(current.Key, current.Value.Getter(input));
				}
			}
			output = dictionary;
			return true;
		}

		// Token: 0x04000272 RID: 626
		internal CacheResolver CacheResolver;

		// Token: 0x04000273 RID: 627
		private static readonly string[] Iso8601Format = new string[]
		{
			"yyyy-MM-dd\\THH:mm:ss.FFFFFFF\\Z",
			"yyyy-MM-dd\\THH:mm:ss\\Z",
			"yyyy-MM-dd\\THH:mm:ssK"
		};
	}
}
