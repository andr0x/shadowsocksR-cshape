using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace SimpleJson
{
	// Token: 0x02000055 RID: 85
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class JsonObject : IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		// Token: 0x0600033C RID: 828 RVA: 0x0001E189 File Offset: 0x0001C389
		public void Add(KeyValuePair<string, object> item)
		{
			this._members.Add(item.Key, item.Value);
		}

		// Token: 0x06000334 RID: 820 RVA: 0x0001E118 File Offset: 0x0001C318
		public void Add(string key, object value)
		{
			this._members.Add(key, value);
		}

		// Token: 0x0600033D RID: 829 RVA: 0x0001E1A4 File Offset: 0x0001C3A4
		public void Clear()
		{
			this._members.Clear();
		}

		// Token: 0x0600033E RID: 830 RVA: 0x0001E1B1 File Offset: 0x0001C3B1
		public bool Contains(KeyValuePair<string, object> item)
		{
			return this._members.ContainsKey(item.Key) && this._members[item.Key] == item.Value;
		}

		// Token: 0x06000335 RID: 821 RVA: 0x0001E127 File Offset: 0x0001C327
		public bool ContainsKey(string key)
		{
			return this._members.ContainsKey(key);
		}

		// Token: 0x0600033F RID: 831 RVA: 0x0001E1E4 File Offset: 0x0001C3E4
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

		// Token: 0x06000333 RID: 819 RVA: 0x0001E09C File Offset: 0x0001C29C
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

		// Token: 0x06000343 RID: 835 RVA: 0x0001E268 File Offset: 0x0001C468
		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return this._members.GetEnumerator();
		}

		// Token: 0x06000337 RID: 823 RVA: 0x0001E142 File Offset: 0x0001C342
		public bool Remove(string key)
		{
			return this._members.Remove(key);
		}

		// Token: 0x06000342 RID: 834 RVA: 0x0001E254 File Offset: 0x0001C454
		public bool Remove(KeyValuePair<string, object> item)
		{
			return this._members.Remove(item.Key);
		}

		// Token: 0x06000344 RID: 836 RVA: 0x0001E268 File Offset: 0x0001C468
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._members.GetEnumerator();
		}

		// Token: 0x06000345 RID: 837 RVA: 0x0001E27A File Offset: 0x0001C47A
		public override string ToString()
		{
			return SimpleJson.SerializeObject(this);
		}

		// Token: 0x06000338 RID: 824 RVA: 0x0001E150 File Offset: 0x0001C350
		public bool TryGetValue(string key, out object value)
		{
			return this._members.TryGetValue(key, out value);
		}

		// Token: 0x17000026 RID: 38
		public int Count
		{
			// Token: 0x06000340 RID: 832 RVA: 0x0001E244 File Offset: 0x0001C444
			get
			{
				return this._members.Count;
			}
		}

		// Token: 0x17000027 RID: 39
		public bool IsReadOnly
		{
			// Token: 0x06000341 RID: 833 RVA: 0x0001E251 File Offset: 0x0001C451
			get
			{
				return false;
			}
		}

		// Token: 0x17000022 RID: 34
		public object this[int index]
		{
			// Token: 0x06000332 RID: 818 RVA: 0x0001E08E File Offset: 0x0001C28E
			get
			{
				return JsonObject.GetAtIndex(this._members, index);
			}
		}

		// Token: 0x17000025 RID: 37
		public object this[string key]
		{
			// Token: 0x0600033A RID: 826 RVA: 0x0001E16C File Offset: 0x0001C36C
			get
			{
				return this._members[key];
			}
			// Token: 0x0600033B RID: 827 RVA: 0x0001E17A File Offset: 0x0001C37A
			set
			{
				this._members[key] = value;
			}
		}

		// Token: 0x17000023 RID: 35
		public ICollection<string> Keys
		{
			// Token: 0x06000336 RID: 822 RVA: 0x0001E135 File Offset: 0x0001C335
			get
			{
				return this._members.Keys;
			}
		}

		// Token: 0x17000024 RID: 36
		public ICollection<object> Values
		{
			// Token: 0x06000339 RID: 825 RVA: 0x0001E15F File Offset: 0x0001C35F
			get
			{
				return this._members.Values;
			}
		}

		// Token: 0x04000262 RID: 610
		private readonly Dictionary<string, object> _members = new Dictionary<string, object>();
	}
}
