using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SimpleJson
{
	// Token: 0x02000054 RID: 84
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class JsonArray : List<object>
	{
		// Token: 0x0600032F RID: 815 RVA: 0x0001E06C File Offset: 0x0001C26C
		public JsonArray()
		{
		}

		// Token: 0x06000330 RID: 816 RVA: 0x0001E074 File Offset: 0x0001C274
		public JsonArray(int capacity) : base(capacity)
		{
		}

		// Token: 0x06000331 RID: 817 RVA: 0x0001E07D File Offset: 0x0001C27D
		public override string ToString()
		{
			return SimpleJson.SerializeObject(this) ?? string.Empty;
		}
	}
}
