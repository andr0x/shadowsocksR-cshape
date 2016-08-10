using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SimpleJson
{
	// Token: 0x02000052 RID: 82
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class JsonArray : List<object>
	{
		// Token: 0x06000326 RID: 806 RVA: 0x0001EE24 File Offset: 0x0001D024
		public JsonArray()
		{
		}

		// Token: 0x06000327 RID: 807 RVA: 0x0001EE2C File Offset: 0x0001D02C
		public JsonArray(int capacity) : base(capacity)
		{
		}

		// Token: 0x06000328 RID: 808 RVA: 0x0001EE35 File Offset: 0x0001D035
		public override string ToString()
		{
			return SimpleJson.SerializeObject(this) ?? string.Empty;
		}
	}
}
