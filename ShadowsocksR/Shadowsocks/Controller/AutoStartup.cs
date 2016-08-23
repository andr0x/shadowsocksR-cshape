using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Shadowsocks.Controller
{
	// Token: 0x0200003A RID: 58
	internal class AutoStartup
	{
		// Token: 0x0600020F RID: 527 RVA: 0x00014CC4 File Offset: 0x00012EC4
		public static bool Check()
		{
			RegistryKey registryKey = null;
			bool result;
			try
			{
				string arg_07_0 = Application.ExecutablePath;
				registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");
				string[] valueNames = registryKey.GetValueNames();
				registryKey.Close();
				string[] array = valueNames;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Equals(AutoStartup.Key))
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			catch (Exception arg_4E_0)
			{
				Logging.LogUsefulException(arg_4E_0);
				result = false;
			}
			finally
			{
				if (registryKey != null)
				{
					try
					{
						registryKey.Close();
					}
					catch (Exception arg_63_0)
					{
						Logging.LogUsefulException(arg_63_0);
					}
				}
			}
			return result;
		}

		// Token: 0x0600020E RID: 526 RVA: 0x00014C30 File Offset: 0x00012E30
		public static bool Set(bool enabled)
		{
			RegistryKey registryKey = null;
			bool result;
			try
			{
				string executablePath = Application.ExecutablePath;
				registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
				if (enabled)
				{
					registryKey.SetValue(AutoStartup.Key, executablePath);
				}
				else
				{
					registryKey.DeleteValue(AutoStartup.Key);
				}
				registryKey.Close();
				result = true;
			}
			catch (Exception arg_3F_0)
			{
				Logging.LogUsefulException(arg_3F_0);
				result = false;
			}
			finally
			{
				if (registryKey != null)
				{
					try
					{
						registryKey.Close();
					}
					catch (Exception arg_53_0)
					{
						Logging.LogUsefulException(arg_53_0);
					}
				}
			}
			return result;
		}

		// Token: 0x040001A2 RID: 418
		private static string Key = "ShadowsocksR_" + Application.StartupPath.GetHashCode();
	}
}
