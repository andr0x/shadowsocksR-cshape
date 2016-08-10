using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace SimpleJson
{
	// Token: 0x02000053 RID: 83
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class JsonObject : IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		// Token: 0x06000333 RID: 819 RVA: 0x0001EF41 File Offset: 0x0001D141
		public void Add(KeyValuePair<string, object> item)
		{
			this._members.Add(item.Key, item.Value);
		}

		// Token: 0x0600032B RID: 811 RVA: 0x0001EED0 File Offset: 0x0001D0D0
		public void Add(string key, object value)
		{
			this._members.Add(key, value);
		}

		// Token: 0x06000334 RID: 820 RVA: 0x0001EF5C File Offset: 0x0001D15C
		public void Clear()
		{
			this._members.Clear();
		}

		// Token: 0x06000335 RID: 821 RVA: 0x0001EF69 File Offset: 0x0001D169
		public bool Contains(KeyValuePair<string, object> item)
		{
			return this._members.ContainsKey(item.Key) && this._members[item.Key] == item.Value;
		}

		// Token: 0x0600032C RID: 812 RVA: 0x0001EEDF File Offset: 0x0001D0DF
		public bool ContainsKey(string key)
		{
			return this._members.ContainsKey(key);
		}

		// Token: 0x06000336 RID: 822 RVA: 0x0001EF9C File Offset: 0x0001D19C
		public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			int num = this.Count;
			foreach (KeyValuePair<string, object> current in this)
			{
				array[arrayIndex++] = current;
				if (--num <= 0)
				{
					break;
				}
			}
		}

		// Token: 0x0600032A RID: 810 RVA: 0x0001EE54 File Offset: 0x0001D054
		internal static object GetAtIndex(IDictionary<string, object> obj, int index)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (index >= obj.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			int num = 0;
			foreach (KeyValuePair<string, object> current in obj)
			{
				if (num++ == index)
				{
					return current.Value;
				}
			}
			return null;
		}

		// Token: 0x0600033A RID: 826 RVA: 0x0001F020 File Offset: 0x0001D220
		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return this._members.GetEnumerator();
		}

		// Token: 0x0600032E RID: 814 RVA: 0x0001EEFA File Offset: 0x0001D0FA
		public bool Remove(string key)
		{
			return this._members.Remove(key);
		}

		// Token: 0x06000339 RID: 825 RVA: 0x0001F00C File Offset: 0x0001D20C
		public bool Remove(KeyValuePair<string, object> item)
		{
			return this._members.Remove(item.Key);
		}

		// Token: 0x0600033B RID: 827 RVA: 0x0001F020 File Offset: 0x0001D220
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._members.GetEnumerator();
		}

		// Token: 0x0600033C RID: 828 RVA: 0x0001F032 File Offset: 0x0001D232
		public override string ToString()
		{
			return SimpleJson.SerializeObject(this);
		}

		// Token: 0x0600032F RID: 815 RVA: 0x0001EF08 File Offset: 0x0001D108
		public bool TryGetValue(string key, out object value)
		{
			return this._members.TryGetValue(key, out value);
		}

		// Token: 0x17000023 RID: 35
		public int Count
		{
			// Token: 0x06000337 RID: 823 RVA: 0x0001EFFC File Offset: 0x0001D1FC
			get
			{
				return this._members.Count;
			}
		}

		// Token: 0x17000024 RID: 36
		public bool IsReadOnly
		{
			// Token: 0x06000338 RID: 824 RVA: 0x0001F009 File Offset: 0x0001D209
			get
			{
				return false;
			}
		}

		// Token: 0x1700001F RID: 31
		public object this[int index]
		{
			// Token: 0x06000329 RID: 809 RVA: 0x0001EE46 File Offset: 0x0001D046
			get
			{
				return JsonObject.GetAtIndex(this._members, index);
			}
		}

		// Token: 0x17000022 RID: 34
		public object this[string key]
		{
			// Token: 0x06000331 RID: 817 RVA: 0x0001EF24 File Offset: 0x0001D124
			get
			{
				return this._members[key];
			}
			// Token: 0x06000332 RID: 818 RVA: 0x0001EF32 File Offset: 0x0001D132
			set
			{
				this._members[key] = value;
			}
		}

		// Token: 0x17000020 RID: 32
		public ICollection<string> Keys
		{
			// Token: 0x0600032D RID: 813 RVA: 0x0001EEED File Offset: 0x0001D0ED
			get
			{
				return this._members.Keys;
			}
		}

		// Token: 0x17000021 RID: 33
		public ICollection<object> Values
		{
			// Token: 0x06000330 RID: 816 RVA: 0x0001EF17 File Offset: 0x0001D117
			get
			{
				return this._members.Values;
			}
		}

		// Token: 0x04000265 RID: 613
		private readonly Dictionary<string, object> _members = new Dictionary<string, object>();
	}
}
