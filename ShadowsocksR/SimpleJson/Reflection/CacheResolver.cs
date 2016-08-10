using System;
using System.Reflection;

namespace SimpleJson.Reflection
{
	// Token: 0x0200005B RID: 91
	public class CacheResolver
	{
		// Token: 0x06000377 RID: 887 RVA: 0x00020456 File Offset: 0x0001E656
		public CacheResolver(MemberMapLoader memberMapLoader)
		{
			this._memberMapLoader = memberMapLoader;
		}

		// Token: 0x0600037A RID: 890 RVA: 0x0002052D File Offset: 0x0001E72D
		private static GetHandler CreateGetHandler(FieldInfo fieldInfo)
		{
			return (object instance) => fieldInfo.GetValue(instance);
		}

		// Token: 0x0600037C RID: 892 RVA: 0x0002058C File Offset: 0x0001E78C
		private static GetHandler CreateGetHandler(PropertyInfo propertyInfo)
		{
			MethodInfo getMethodInfo = propertyInfo.GetGetMethod(true);
			if (getMethodInfo == null)
			{
				return null;
			}
			return (object instance) => getMethodInfo.Invoke(instance, Type.EmptyTypes);
		}

		// Token: 0x0600037B RID: 891 RVA: 0x00020548 File Offset: 0x0001E748
		private static SetHandler CreateSetHandler(FieldInfo fieldInfo)
		{
			if (fieldInfo.IsInitOnly || fieldInfo.IsLiteral)
			{
				return null;
			}
			return delegate(object instance, object value)
			{
				fieldInfo.SetValue(instance, value);
			};
		}

		// Token: 0x0600037D RID: 893 RVA: 0x000205C8 File Offset: 0x0001E7C8
		private static SetHandler CreateSetHandler(PropertyInfo propertyInfo)
		{
			MethodInfo setMethodInfo = propertyInfo.GetSetMethod(true);
			if (setMethodInfo == null)
			{
				return null;
			}
			return delegate(object instance, object value)
			{
				setMethodInfo.Invoke(instance, new object[]
				{
					value
				});
			};
		}

		// Token: 0x06000378 RID: 888 RVA: 0x00020470 File Offset: 0x0001E670
		public static object GetNewInstance(Type type)
		{
			CacheResolver.CtorDelegate ctorDelegate;
			if (CacheResolver.ConstructorCache.TryGetValue(type, out ctorDelegate))
			{
				return ctorDelegate();
			}
			ConstructorInfo constructorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
			ctorDelegate = (() => constructorInfo.Invoke(null));
			CacheResolver.ConstructorCache.Add(type, ctorDelegate);
			return ctorDelegate();
		}

		// Token: 0x06000379 RID: 889 RVA: 0x000204D0 File Offset: 0x0001E6D0
		public SafeDictionary<string, CacheResolver.MemberMap> LoadMaps(Type type)
		{
			if (type == null || type == typeof(object))
			{
				return null;
			}
			SafeDictionary<string, CacheResolver.MemberMap> safeDictionary;
			if (this._memberMapsCache.TryGetValue(type, out safeDictionary))
			{
				return safeDictionary;
			}
			safeDictionary = new SafeDictionary<string, CacheResolver.MemberMap>();
			this._memberMapLoader(type, safeDictionary);
			this._memberMapsCache.Add(type, safeDictionary);
			return safeDictionary;
		}

		// Token: 0x04000279 RID: 633
		private static readonly SafeDictionary<Type, CacheResolver.CtorDelegate> ConstructorCache = new SafeDictionary<Type, CacheResolver.CtorDelegate>();

		// Token: 0x04000277 RID: 631
		private readonly MemberMapLoader _memberMapLoader;

		// Token: 0x04000278 RID: 632
		private readonly SafeDictionary<Type, SafeDictionary<string, CacheResolver.MemberMap>> _memberMapsCache = new SafeDictionary<Type, SafeDictionary<string, CacheResolver.MemberMap>>();

		// Token: 0x020000B4 RID: 180
		// Token: 0x0600058B RID: 1419
		private delegate object CtorDelegate();

		// Token: 0x020000B5 RID: 181
		public sealed class MemberMap
		{
			// Token: 0x0600058E RID: 1422 RVA: 0x0002C95F File Offset: 0x0002AB5F
			public MemberMap(PropertyInfo propertyInfo)
			{
				this.MemberInfo = propertyInfo;
				this.Type = propertyInfo.PropertyType;
				this.Getter = CacheResolver.CreateGetHandler(propertyInfo);
				this.Setter = CacheResolver.CreateSetHandler(propertyInfo);
			}

			// Token: 0x0600058F RID: 1423 RVA: 0x0002C992 File Offset: 0x0002AB92
			public MemberMap(FieldInfo fieldInfo)
			{
				this.MemberInfo = fieldInfo;
				this.Type = fieldInfo.FieldType;
				this.Getter = CacheResolver.CreateGetHandler(fieldInfo);
				this.Setter = CacheResolver.CreateSetHandler(fieldInfo);
			}

			// Token: 0x0400046C RID: 1132
			public readonly GetHandler Getter;

			// Token: 0x0400046A RID: 1130
			public readonly MemberInfo MemberInfo;

			// Token: 0x0400046D RID: 1133
			public readonly SetHandler Setter;

			// Token: 0x0400046B RID: 1131
			public readonly Type Type;
		}
	}
}
