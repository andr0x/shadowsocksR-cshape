using System;

namespace SimpleJson
{
	// Token: 0x02000055 RID: 85
	public interface IJsonSerializerStrategy
	{
		// Token: 0x0600035B RID: 859
		object DeserializeObject(object value, Type type);

		// Token: 0x0600035A RID: 858
		bool SerializeNonPrimitiveObject(object input, out object output);
	}
}
