using System;
using System.Reflection;

namespace SimpleJson.Reflection
{
	// Token: 0x0200005D RID: 93
	public class CacheResolver
	{
		// Token: 0x06000380 RID: 896 RVA: 0x0001F69E File Offset: 0x0001D89E
		public CacheResolver(MemberMapLoader memberMapLoader)
		{
			this._memberMapLoader = memberMapLoader;
		}

		// Token: 0x06000383 RID: 899 RVA: 0x0001F775 File Offset: 0x0001D975
		private static GetHandler CreateGetHandler(FieldInfo fieldInfo)
		{
			return (object instance) => fieldInfo.GetValue(instance);
		}

		// Token: 0x06000385 RID: 901 RVA: 0x0001F7D4 File Offset: 0x0001D9D4
		private static GetHandler CreateGetHandler(PropertyInfo propertyInfo)
		{
			MethodInfo getMethodInfo = propertyInfo.GetGetMethod(true);
			if (getMethodInfo == null)
			{
				return null;
			}
			return (object instance) => getMethodInfo.Invoke(instance, Type.EmptyTypes);
		}

		// Token: 0x06000384 RID: 900 RVA: 0x0001F790 File Offset: 0x0001D990
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

		// Token: 0x06000386 RID: 902 RVA: 0x0001F810 File Offset: 0x0001DA10
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

		// Token: 0x06000381 RID: 897 RVA: 0x0001F6B8 File Offset: 0x0001D8B8
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

		// Token: 0x06000382 RID: 898 RVA: 0x0001F718 File Offset: 0x0001D918
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

		// Token: 0x04000276 RID: 630
		private static readonly SafeDictionary<Type, CacheResolver.CtorDelegate> ConstructorCache = new SafeDictionary<Type, CacheResolver.CtorDelegate>();

		// Token: 0x04000274 RID: 628
		private readonly MemberMapLoader _memberMapLoader;

		// Token: 0x04000275 RID: 629
		private readonly SafeDictionary<Type, SafeDictionary<string, CacheResolver.MemberMap>> _memberMapsCache = new SafeDictionary<Type, SafeDictionary<string, CacheResolver.MemberMap>>();

		// Token: 0x020000B5 RID: 181
		// Token: 0x06000590 RID: 1424
		private delegate object CtorDelegate();

		// Token: 0x020000B6 RID: 182
		public sealed class MemberMap
		{
			// Token: 0x06000593 RID: 1427 RVA: 0x0002BBA7 File Offset: 0x00029DA7
			public MemberMap(PropertyInfo propertyInfo)
			{
				this.MemberInfo = propertyInfo;
				this.Type = propertyInfo.PropertyType;
				this.Getter = CacheResolver.CreateGetHandler(propertyInfo);
				this.Setter = CacheResolver.CreateSetHandler(propertyInfo);
			}

			// Token: 0x06000594 RID: 1428 RVA: 0x0002BBDA File Offset: 0x00029DDA
			public MemberMap(FieldInfo fieldInfo)
			{
				this.MemberInfo = fieldInfo;
				this.Type = fieldInfo.FieldType;
				this.Getter = CacheResolver.CreateGetHandler(fieldInfo);
				this.Setter = CacheResolver.CreateSetHandler(fieldInfo);
			}

			// Token: 0x04000469 RID: 1129
			public readonly GetHandler Getter;

			// Token: 0x04000467 RID: 1127
			public readonly MemberInfo MemberInfo;

			// Token: 0x0400046A RID: 1130
			public readonly SetHandler Setter;

			// Token: 0x04000468 RID: 1128
			public readonly Type Type;
		}
	}
}
