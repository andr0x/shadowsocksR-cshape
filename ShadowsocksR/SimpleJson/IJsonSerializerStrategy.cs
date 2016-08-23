using System;

namespace SimpleJson
{
	// Token: 0x02000057 RID: 87
	public interface IJsonSerializerStrategy
	{
		// Token: 0x06000364 RID: 868
		object DeserializeObject(object value, Type type);

		// Token: 0x06000363 RID: 867
		bool SerializeNonPrimitiveObject(object input, out object output);
	}
}
