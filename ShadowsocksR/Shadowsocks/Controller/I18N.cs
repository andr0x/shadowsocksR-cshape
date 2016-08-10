using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Shadowsocks.Properties;

namespace Shadowsocks.Controller
{
	// Token: 0x02000042 RID: 66
	public class I18N
	{
		// Token: 0x06000256 RID: 598 RVA: 0x00016CA4 File Offset: 0x00014EA4
		protected static Dictionary<string, string> Strings;
		static I18N()
		{
			I18N.Strings = new Dictionary<string, string>();
			if (CultureInfo.CurrentCulture.IetfLanguageTag.ToLowerInvariant().StartsWith("zh"))
			{
				string[] array = Regex.Split(Resources.cn, "\r\n|\r|\n");
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					if (!text.StartsWith("#"))
					{
						string[] array2 = Regex.Split(text, "=");
						if (array2.Length == 2)
						{
							string value = Regex.Replace(array2[1], "\\\\n", "\r\n");
							I18N.Strings[array2[0]] = value;
						}
					}
				}
			}
		}

		// Token: 0x06000257 RID: 599 RVA: 0x00016D3A File Offset: 0x00014F3A
		public static string GetString(string key)
		{
			if (I18N.Strings.ContainsKey(key))
			{
				return I18N.Strings[key];
			}
			return key;
		}

		// Token: 0x040001CA RID: 458
	}
}
