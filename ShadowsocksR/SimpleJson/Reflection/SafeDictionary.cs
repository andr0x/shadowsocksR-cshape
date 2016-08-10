using System;
using System.Collections.Generic;

namespace SimpleJson.Reflection
{
	// Token: 0x0200005C RID: 92
	public class SafeDictionary<TKey, TValue>
	{
		// Token: 0x06000382 RID: 898 RVA: 0x0002063C File Offset: 0x0001E83C
		public void Add(TKey key, TValue value)
		{
			object padlock = this._padlock;
			lock (padlock)
			{
				if (!this._dictionary.ContainsKey(key))
				{
					this._dictionary.Add(key, value);
				}
			}
		}

		// Token: 0x06000381 RID: 897 RVA: 0x0002062D File Offset: 0x0001E82D
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<TKey, TValue>>)this._dictionary).GetEnumerator();
		}

		// Token: 0x0600037F RID: 895 RVA: 0x00020610 File Offset: 0x0001E810
		public bool TryGetValue(TKey key, out TValue value)
		{
			return this._dictionary.TryGetValue(key, out value);
		}

		// Token: 0x17000027 RID: 39
		public TValue this[TKey key]
		{
			// Token: 0x06000380 RID: 896 RVA: 0x0002061F File Offset: 0x0001E81F
			get
			{
				return this._dictionary[key];
			}
		}

		// Token: 0x0400027B RID: 635
		private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

		// Token: 0x0400027A RID: 634
		private readonly object _padlock = new object();
	}
}
