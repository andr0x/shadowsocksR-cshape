using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using Shadowsocks.Model;

namespace Shadowsocks.Controller
{
	// Token: 0x02000053 RID: 83
	public class SystemProxy
	{
		// Token: 0x0600032B RID: 811 RVA: 0x0001DEB8 File Offset: 0x0001C0B8
		private static void CopyProxySettingFromLan()
		{
			RegistryKey registryKey = null;
			try
			{
				registryKey = SystemProxy.OpenUserRegKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Connections", true);
				object value = registryKey.GetValue("DefaultConnectionSettings");
				string[] valueNames = registryKey.GetValueNames();
				for (int i = 0; i < valueNames.Length; i++)
				{
					string text = valueNames[i];
					if (!text.Equals("DefaultConnectionSettings") && !text.Equals("LAN Connection") && !text.Equals("SavedLegacySettings"))
					{
						registryKey.SetValue(text, value);
					}
				}
				SystemProxy.NotifyIE();
			}
			catch (IOException arg_6E_0)
			{
				Logging.LogUsefulException(arg_6E_0);
			}
			finally
			{
				if (registryKey != null)
				{
					try
					{
						registryKey.Close();
					}
					catch (Exception arg_80_0)
					{
						Logging.LogUsefulException(arg_80_0);
					}
				}
			}
		}

		// Token: 0x0600032C RID: 812 RVA: 0x0001DF78 File Offset: 0x0001C178
		private static string GetTimestamp(DateTime value)
		{
			return value.ToString("yyyyMMddHHmmssffff");
		}

		// Token: 0x0600032D RID: 813 RVA: 0x0001DF88 File Offset: 0x0001C188
		private static void IEAutoDetectProxy(bool set)
		{
			RegistryKey registryKey = null;
			try
			{
				registryKey = SystemProxy.OpenUserRegKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Connections", true);
				byte[] array = (byte[])registryKey.GetValue("DefaultConnectionSettings");
				byte[] array2 = (byte[])registryKey.GetValue("SavedLegacySettings");
				if (array == null)
				{
					array = new byte[32];
					array[0] = 70;
					array[4] = 204;
					array[5] = 14;
					array[8] = 1;
				}
				if (set)
				{
					array[8] = Convert.ToByte((int)(array[8] & 8));
					array2[8] = Convert.ToByte((int)(array2[8] & 8));
				}
				else
				{
					array[8] = Convert.ToByte((int)array[8] & -9);
					array2[8] = Convert.ToByte((int)array2[8] & -9);
				}
				SystemProxy.RegistrySetValue(registryKey, "DefaultConnectionSettings", array);
				SystemProxy.RegistrySetValue(registryKey, "SavedLegacySettings", array2);
			}
			finally
			{
				if (registryKey != null)
				{
					try
					{
						registryKey.Close();
					}
					catch (Exception arg_B1_0)
					{
						Logging.LogUsefulException(arg_B1_0);
					}
				}
			}
		}

		// Token: 0x06000326 RID: 806
		[DllImport("wininet.dll")]
		public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

		// Token: 0x06000327 RID: 807 RVA: 0x0001DCA7 File Offset: 0x0001BEA7
		public static void NotifyIE()
		{
			SystemProxy._settingsReturn = SystemProxy.InternetSetOption(IntPtr.Zero, 39, IntPtr.Zero, 0);
			SystemProxy._refreshReturn = SystemProxy.InternetSetOption(IntPtr.Zero, 37, IntPtr.Zero, 0);
		}

		// Token: 0x06000329 RID: 809 RVA: 0x0001DD08 File Offset: 0x0001BF08
		public static RegistryKey OpenUserRegKey(string name, bool writable)
		{
			return RegistryKey.OpenRemoteBaseKey(RegistryHive.CurrentUser, "").OpenSubKey(name, writable);
		}

		// Token: 0x06000328 RID: 808 RVA: 0x0001DCD8 File Offset: 0x0001BED8
		public static void RegistrySetValue(RegistryKey registry, string name, object value)
		{
			try
			{
				registry.SetValue(name, value);
			}
			catch (Exception arg_0A_0)
			{
				Logging.LogUsefulException(arg_0A_0);
			}
		}

		// Token: 0x0600032A RID: 810 RVA: 0x0001DD20 File Offset: 0x0001BF20
		public static void Update(Configuration config, bool forceDisable)
		{
			bool global = config.global;
			bool flag = config.enabled;
			if (forceDisable)
			{
				flag = false;
			}
			RegistryKey registryKey = null;
			try
			{
				registryKey = SystemProxy.OpenUserRegKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
				if (flag)
				{
					if (global)
					{
						SystemProxy.RegistrySetValue(registryKey, "ProxyEnable", 1);
						SystemProxy.RegistrySetValue(registryKey, "ProxyServer", "127.0.0.1:" + config.localPort.ToString());
						SystemProxy.RegistrySetValue(registryKey, "AutoConfigURL", "");
					}
					else
					{
						string value = "http://127.0.0.1:" + config.localPort.ToString() + "/pac?t=" + SystemProxy.GetTimestamp(DateTime.Now);
						SystemProxy.RegistrySetValue(registryKey, "ProxyEnable", 0);
						registryKey.GetValue("ProxyServer");
						SystemProxy.RegistrySetValue(registryKey, "ProxyServer", "");
						SystemProxy.RegistrySetValue(registryKey, "AutoConfigURL", value);
					}
				}
				else
				{
					SystemProxy.RegistrySetValue(registryKey, "ProxyEnable", 0);
					SystemProxy.RegistrySetValue(registryKey, "ProxyServer", "");
					SystemProxy.RegistrySetValue(registryKey, "AutoConfigURL", "");
				}
				SystemProxy.IEAutoDetectProxy(false);
				SystemProxy.NotifyIE();
				SystemProxy.CopyProxySettingFromLan();
			}
			catch (Exception arg_113_0)
			{
				Logging.LogUsefulException(arg_113_0);
				MessageBox.Show(I18N.GetString("Failed to update registry"));
			}
			finally
			{
				if (registryKey != null)
				{
					try
					{
						registryKey.Close();
					}
					catch (Exception arg_135_0)
					{
						Logging.LogUsefulException(arg_135_0);
					}
				}
			}
		}

		// Token: 0x0400025F RID: 607
		public const int INTERNET_OPTION_REFRESH = 37;

		// Token: 0x0400025E RID: 606
		public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;

		// Token: 0x04000261 RID: 609
		private static bool _refreshReturn;

		// Token: 0x04000260 RID: 608
		private static bool _settingsReturn;
	}
}
