using System;
using System.Collections.Generic;

namespace SimpleJson.Reflection
{
	// Token: 0x0200005E RID: 94
	public class SafeDictionary<TKey, TValue>
	{
		// Token: 0x0600038B RID: 907 RVA: 0x0001F884 File Offset: 0x0001DA84
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

		// Token: 0x0600038A RID: 906 RVA: 0x0001F875 File Offset: 0x0001DA75
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<TKey, TValue>>)this._dictionary).GetEnumerator();
		}

		// Token: 0x06000388 RID: 904 RVA: 0x0001F858 File Offset: 0x0001DA58
		public bool TryGetValue(TKey key, out TValue value)
		{
			return this._dictionary.TryGetValue(key, out value);
		}

		// Token: 0x1700002A RID: 42
		public TValue this[TKey key]
		{
			// Token: 0x06000389 RID: 905 RVA: 0x0001F867 File Offset: 0x0001DA67
			get
			{
				return this._dictionary[key];
			}
		}

		// Token: 0x04000278 RID: 632
		private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

		// Token: 0x04000277 RID: 631
		private readonly object _padlock = new object();
	}
}
