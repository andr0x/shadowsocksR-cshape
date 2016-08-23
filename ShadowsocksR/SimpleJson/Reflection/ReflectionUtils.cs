using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace SimpleJson.Reflection
{
	// Token: 0x02000059 RID: 89
	public class ReflectionUtils
	{
		// Token: 0x0600036D RID: 877 RVA: 0x0001F58D File Offset: 0x0001D78D
		public static Attribute GetAttribute(MemberInfo info, Type type)
		{
			if (info == null || type == null || !Attribute.IsDefined(info, type))
			{
				return null;
			}
			return Attribute.GetCustomAttribute(info, type);
		}

		// Token: 0x0600036E RID: 878 RVA: 0x0001F5B3 File Offset: 0x0001D7B3
		public static Attribute GetAttribute(Type objectType, Type attributeType)
		{
			if (objectType == null || attributeType == null || !Attribute.IsDefined(objectType, attributeType))
			{
				return null;
			}
			return Attribute.GetCustomAttribute(objectType, attributeType);
		}

		// Token: 0x06000371 RID: 881 RVA: 0x0001F665 File Offset: 0x0001D865
		public static bool IsNullableType(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		// Token: 0x06000370 RID: 880 RVA: 0x0001F630 File Offset: 0x0001D830
		public static bool IsTypeDictionary(Type type)
		{
			return typeof(IDictionary).IsAssignableFrom(type) || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<, >));
		}

		// Token: 0x0600036F RID: 879 RVA: 0x0001F5DC File Offset: 0x0001D7DC
		public static bool IsTypeGenericeCollectionInterface(Type type)
		{
			if (!type.IsGenericType)
			{
				return false;
			}
			Type genericTypeDefinition = type.GetGenericTypeDefinition();
			return genericTypeDefinition == typeof(IList<>) || genericTypeDefinition == typeof(ICollection<>) || genericTypeDefinition == typeof(IEnumerable<>);
		}

		// Token: 0x06000372 RID: 882 RVA: 0x0001F686 File Offset: 0x0001D886
		public static object ToNullableType(object obj, Type nullableType)
		{
			if (obj != null)
			{
				return Convert.ChangeType(obj, Nullable.GetUnderlyingType(nullableType), CultureInfo.InvariantCulture);
			}
			return null;
		}
	}
}
