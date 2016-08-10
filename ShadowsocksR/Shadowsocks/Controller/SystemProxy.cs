using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using Shadowsocks.Model;

namespace Shadowsocks.Controller
{
	// Token: 0x02000051 RID: 81
	public class SystemProxy
	{
		// Token: 0x06000322 RID: 802 RVA: 0x0001EC68 File Offset: 0x0001CE68
		private static void CopyProxySettingFromLan()
		{
			RegistryKey registryKey = null;
			try
			{
				registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Connections", true);
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
			catch (IOException arg_73_0)
			{
				Logging.LogUsefulException(arg_73_0);
			}
			finally
			{
				if (registryKey != null)
				{
					try
					{
						registryKey.Close();
					}
					catch (Exception arg_85_0)
					{
						Logging.LogUsefulException(arg_85_0);
					}
				}
			}
		}

		// Token: 0x0600031F RID: 799 RVA: 0x0001E9D8 File Offset: 0x0001CBD8
		public static int GetFIPS()
		{
			RegistryKey registryKey = null;
			try
			{
				registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Lsa\\FipsAlgorithmPolicy", false);
				return (int)registryKey.GetValue("Enabled");
			}
			catch (Exception arg_26_0)
			{
				Logging.LogUsefulException(arg_26_0);
			}
			finally
			{
				if (registryKey != null)
				{
					try
					{
						registryKey.Close();
					}
					catch (Exception arg_38_0)
					{
						Logging.LogUsefulException(arg_38_0);
					}
				}
			}
			return -1;
		}

		// Token: 0x06000323 RID: 803 RVA: 0x0001ED2C File Offset: 0x0001CF2C
		private static string GetTimestamp(DateTime value)
		{
			return value.ToString("yyyyMMddHHmmssffff");
		}

		// Token: 0x06000324 RID: 804 RVA: 0x0001ED3C File Offset: 0x0001CF3C
		private static void IEAutoDetectProxy(bool set)
		{
			RegistryKey registryKey = null;
			try
			{
				registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Connections", true);
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
					catch (Exception arg_B6_0)
					{
						Logging.LogUsefulException(arg_B6_0);
					}
				}
			}
		}

		// Token: 0x0600031C RID: 796
		[DllImport("wininet.dll")]
		public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

		// Token: 0x0600031D RID: 797 RVA: 0x0001E977 File Offset: 0x0001CB77
		public static void NotifyIE()
		{
			SystemProxy._settingsReturn = SystemProxy.InternetSetOption(IntPtr.Zero, 39, IntPtr.Zero, 0);
			SystemProxy._refreshReturn = SystemProxy.InternetSetOption(IntPtr.Zero, 37, IntPtr.Zero, 0);
		}

		// Token: 0x0600031E RID: 798 RVA: 0x0001E9A8 File Offset: 0x0001CBA8
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

		// Token: 0x06000320 RID: 800 RVA: 0x0001EA50 File Offset: 0x0001CC50
		public static bool SetFIPS(int val)
		{
			RegistryKey registryKey = null;
			try
			{
				registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Lsa\\FipsAlgorithmPolicy", true);
				SystemProxy.RegistrySetValue(registryKey, "Enabled", val);
				return true;
			}
			catch (Exception arg_28_0)
			{
				Logging.LogUsefulException(arg_28_0);
			}
			finally
			{
				if (registryKey != null)
				{
					try
					{
						registryKey.Close();
					}
					catch (Exception arg_3A_0)
					{
						Logging.LogUsefulException(arg_3A_0);
					}
				}
			}
			return false;
		}

		// Token: 0x06000321 RID: 801 RVA: 0x0001EACC File Offset: 0x0001CCCC
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
				registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
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
			catch (Exception arg_118_0)
			{
				Logging.LogUsefulException(arg_118_0);
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
					catch (Exception arg_13A_0)
					{
						Logging.LogUsefulException(arg_13A_0);
					}
				}
			}
		}

		// Token: 0x04000262 RID: 610
		public const int INTERNET_OPTION_REFRESH = 37;

		// Token: 0x04000261 RID: 609
		public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;

		// Token: 0x04000264 RID: 612
		private static bool _refreshReturn;

		// Token: 0x04000263 RID: 611
		private static bool _settingsReturn;
	}
}
