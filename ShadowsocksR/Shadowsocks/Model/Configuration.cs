using System;
using System.Collections.Generic;
using System.IO;
using Shadowsocks.Controller;
using SimpleJson;

namespace Shadowsocks.Model
{
	// Token: 0x02000027 RID: 39
	[Serializable]
	public class Configuration
	{
		// Token: 0x06000163 RID: 355 RVA: 0x0001147C File Offset: 0x0000F67C
		public Configuration()
		{
			this.index = 0;
			this.isDefault = true;
			this.localPort = 1080;
			this.reconnectTimes = 3;
			this.keepVisitTime = 180;
			this.configs = new List<Server>
			{
				Configuration.GetDefaultServer()
			};
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00011819 File Offset: 0x0000FA19
		private static void Assert(bool condition)
		{
			if (!condition)
			{
				throw new Exception(I18N.GetString("assertion failure"));
			}
		}

		// Token: 0x0600016B RID: 363 RVA: 0x0001184C File Offset: 0x0000FA4C
		private static void CheckPassword(string password)
		{
			if (string.IsNullOrEmpty(password))
			{
				throw new ArgumentException(I18N.GetString("Password can not be blank"));
			}
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0001182E File Offset: 0x0000FA2E
		public static void CheckPort(int port)
		{
			if (port <= 0 || port > 65535)
			{
				throw new ArgumentException(I18N.GetString("Port out of range"));
			}
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00011444 File Offset: 0x0000F644
		public static void CheckServer(Server server)
		{
			Configuration.CheckPort(server.server_port);
			if (server.server_udp_port != 0)
			{
				Configuration.CheckPort(server.server_udp_port);
			}
			Configuration.CheckPassword(server.password);
			Configuration.CheckServer(server.server);
		}

		// Token: 0x0600016C RID: 364 RVA: 0x00011866 File Offset: 0x0000FA66
		private static void CheckServer(string server)
		{
			if (string.IsNullOrEmpty(server))
			{
				throw new ArgumentException(I18N.GetString("Server IP can not be blank"));
			}
		}

		// Token: 0x06000160 RID: 352 RVA: 0x000112C4 File Offset: 0x0000F4C4
		public void FlushPortMapCache()
		{
			this.portMapCache = new Dictionary<int, PortMapConfigCache>();
			Dictionary<string, Server> dictionary = new Dictionary<string, Server>();
			foreach (Server current in this.configs)
			{
				dictionary[current.id] = current;
			}
			foreach (KeyValuePair<string, object> current2 in this.portMap)
			{
				int key = 0;
				PortMapConfig portMapConfig = (PortMapConfig)current2.Value;
				if (portMapConfig.enable && dictionary.ContainsKey(portMapConfig.id))
				{
					try
					{
						key = int.Parse(current2.Key);
					}
					catch (FormatException)
					{
						continue;
					}
					this.portMapCache[key] = new PortMapConfigCache();
					this.portMapCache[key].id = portMapConfig.id;
					this.portMapCache[key].server = dictionary[portMapConfig.id];
					this.portMapCache[key].server_addr = portMapConfig.server_addr;
					this.portMapCache[key].server_port = portMapConfig.server_port;
				}
			}
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00010F3C File Offset: 0x0000F13C
		public Server GetCurrentServer(string targetAddr = null, bool usingRandom = false, bool forceRandom = false)
		{
			ServerSelectStrategy obj = this.serverStrategy;
			Server result;
			lock (obj)
			{
				using (SortedDictionary<UriVisitTime, string>.Enumerator enumerator = this.time2uri.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						KeyValuePair<UriVisitTime, string> current = enumerator.Current;
						if ((DateTime.Now - current.Key.visitTime).TotalSeconds >= (double)this.keepVisitTime)
						{
							this.uri2time.Remove(current.Value);
							this.time2uri.Remove(current.Key);
						}
					}
				}
				if (this.sameHostForSameTarget && !forceRandom && targetAddr != null && this.uri2time.ContainsKey(targetAddr))
				{
					UriVisitTime uriVisitTime = this.uri2time[targetAddr];
					if (uriVisitTime.index < this.configs.Count && this.configs[uriVisitTime.index].enable)
					{
						this.time2uri.Remove(uriVisitTime);
						uriVisitTime.visitTime = DateTime.Now;
						this.uri2time[targetAddr] = uriVisitTime;
						this.time2uri[uriVisitTime] = targetAddr;
						result = this.configs[uriVisitTime.index];
						return result;
					}
				}
				if (forceRandom)
				{
					int num = this.serverStrategy.Select(this.configs, this.index, this.randomAlgorithm, true);
					if (num == -1)
					{
						result = Configuration.GetErrorServer();
					}
					else
					{
						result = this.configs[num];
					}
				}
				else if (usingRandom && this.random)
				{
					int num2 = this.serverStrategy.Select(this.configs, this.index, this.randomAlgorithm, false);
					if (num2 == -1)
					{
						result = Configuration.GetErrorServer();
					}
					else
					{
						if (targetAddr != null)
						{
							UriVisitTime uriVisitTime2 = new UriVisitTime();
							uriVisitTime2.uri = targetAddr;
							uriVisitTime2.index = num2;
							uriVisitTime2.visitTime = DateTime.Now;
							if (this.uri2time.ContainsKey(targetAddr))
							{
								this.time2uri.Remove(this.uri2time[targetAddr]);
							}
							this.uri2time[targetAddr] = uriVisitTime2;
							this.time2uri[uriVisitTime2] = targetAddr;
						}
						result = this.configs[num2];
					}
				}
				else if (this.index >= 0 && this.index < this.configs.Count)
				{
					int num3 = this.index;
					if (usingRandom)
					{
						int num4 = 0;
						while (num4 < this.configs.Count && !this.configs[num3].isEnable())
						{
							num3 = (num3 + 1) % this.configs.Count;
							num4++;
						}
					}
					if (targetAddr != null)
					{
						UriVisitTime uriVisitTime3 = new UriVisitTime();
						uriVisitTime3.uri = targetAddr;
						uriVisitTime3.index = num3;
						uriVisitTime3.visitTime = DateTime.Now;
						if (this.uri2time.ContainsKey(targetAddr))
						{
							this.time2uri.Remove(this.uri2time[targetAddr]);
						}
						this.uri2time[targetAddr] = uriVisitTime3;
						this.time2uri[uriVisitTime3] = targetAddr;
					}
					result = this.configs[num3];
				}
				else
				{
					result = Configuration.GetErrorServer();
				}
			}
			return result;
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00011800 File Offset: 0x0000FA00
		public static Server GetDefaultServer()
		{
			return new Server();
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00011807 File Offset: 0x0000FA07
		public static Server GetErrorServer()
		{
			return new Server
			{
				server = "invalid"
			};
		}

		// Token: 0x06000161 RID: 353 RVA: 0x0001143C File Offset: 0x0000F63C
		public Dictionary<int, PortMapConfigCache> GetPortMapCache()
		{
			return this.portMapCache;
		}

		// Token: 0x0600015E RID: 350 RVA: 0x00010E3C File Offset: 0x0000F03C
		public bool KeepCurrentServer(string targetAddr, string id)
		{
			if (this.sameHostForSameTarget && targetAddr != null)
			{
				ServerSelectStrategy obj = this.serverStrategy;
				lock (obj)
				{
					if (this.uri2time.ContainsKey(targetAddr))
					{
						UriVisitTime uriVisitTime = this.uri2time[targetAddr];
						int num = -1;
						for (int i = 0; i < this.configs.Count; i++)
						{
							if (this.configs[i].id == id)
							{
								num = i;
								break;
							}
						}
						if (num >= 0 && this.configs[num].enable)
						{
							this.time2uri.Remove(uriVisitTime);
							uriVisitTime.index = num;
							uriVisitTime.visitTime = DateTime.Now;
							this.uri2time[targetAddr] = uriVisitTime;
							this.time2uri[uriVisitTime] = targetAddr;
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00011514 File Offset: 0x0000F714
		public static Configuration Load()
		{
			Configuration result;
			try
			{
				Configuration configuration = SimpleJson.SimpleJson.DeserializeObject<Configuration>(File.ReadAllText(Configuration.CONFIG_FILE), new Configuration.JsonSerializerStrategy());
				configuration.isDefault = false;
				if (configuration.localPort == 0)
				{
					configuration.localPort = 1080;
				}
				if (configuration.keepVisitTime == 0)
				{
					configuration.keepVisitTime = 180;
				}
				if (configuration.portMap == null)
				{
					configuration.portMap = new Dictionary<string, object>();
				}
				if (configuration.token == null)
				{
					configuration.token = new Dictionary<string, string>();
				}
				int num = 0;
				foreach (Server current in configuration.configs)
				{
					string remarks = current.remarks;
					if (remarks.Length != 0 && current.remarks[remarks.Length - 1] == '=')
					{
						current.remarks_base64 = remarks;
						if (current.remarks_base64 == current.remarks)
						{
							current.remarks = remarks;
							num = 0;
							break;
						}
						num++;
						current.remarks = remarks;
					}
				}
				if (num > 0)
				{
					foreach (Server current2 in configuration.configs)
					{
						string remarks2 = current2.remarks;
						if (remarks2.Length != 0)
						{
							current2.remarks_base64 = remarks2;
						}
					}
				}
				result = configuration;
			}
			catch (Exception ex)
			{
				if (!(ex is FileNotFoundException))
				{
					Console.WriteLine(ex);
				}
				result = new Configuration
				{
					index = 0,
					isDefault = true,
					localPort = 1080,
					reconnectTimes = 3,
					keepVisitTime = 180,
					configs = new List<Server>
					{
						Configuration.GetDefaultServer()
					},
					portMap = new Dictionary<string, object>()
				};
			}
			return result;
		}

		// Token: 0x06000166 RID: 358 RVA: 0x000117C8 File Offset: 0x0000F9C8
		public Configuration Load(string config_str)
		{
			try
			{
				Configuration expr_0B = SimpleJson.SimpleJson.DeserializeObject<Configuration>(config_str, new Configuration.JsonSerializerStrategy());
				expr_0B.isDefault = false;
				return expr_0B;
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00011720 File Offset: 0x0000F920
		public static void Save(Configuration config)
		{
			if (config.index >= config.configs.Count)
			{
				config.index = config.configs.Count - 1;
			}
			if (config.index < 0)
			{
				config.index = 0;
			}
			config.isDefault = false;
			try
			{
				using (StreamWriter streamWriter = new StreamWriter(File.Open(Configuration.CONFIG_FILE, FileMode.Create)))
				{
					string value = SimpleJson.SimpleJson.SerializeObject(config);
					streamWriter.Write(value);
					streamWriter.Flush();
				}
			}
			catch (IOException value2)
			{
				Console.Error.WriteLine(value2);
			}
		}

		// Token: 0x04000128 RID: 296
		public string authPass;

		// Token: 0x04000127 RID: 295
		public string authUser;

		// Token: 0x04000129 RID: 297
		public bool autoBan;

		// Token: 0x0400011A RID: 282
		public bool bypassWhiteList;

		// Token: 0x04000113 RID: 275
		public List<Server> configs;

		// Token: 0x04000133 RID: 307
		private static string CONFIG_FILE = "gui-config.json";

		// Token: 0x0400012C RID: 300
		public string dns_server;

		// Token: 0x04000117 RID: 279
		public bool enabled;

		// Token: 0x04000116 RID: 278
		public bool global;

		// Token: 0x04000114 RID: 276
		public int index;

		// Token: 0x04000119 RID: 281
		public bool isDefault;

		// Token: 0x0400012B RID: 299
		public int keepVisitTime;

		// Token: 0x0400011B RID: 283
		public int localPort;

		// Token: 0x04000120 RID: 288
		public bool pacDirectGoProxy;

		// Token: 0x0400012E RID: 302
		public Dictionary<string, object> portMap = new Dictionary<string, object>();

		// Token: 0x04000132 RID: 306
		private Dictionary<int, PortMapConfigCache> portMapCache = new Dictionary<int, PortMapConfigCache>();

		// Token: 0x04000125 RID: 293
		public string proxyAuthPass;

		// Token: 0x04000124 RID: 292
		public string proxyAuthUser;

		// Token: 0x0400011F RID: 287
		public bool proxyEnable;

		// Token: 0x04000122 RID: 290
		public string proxyHost;

		// Token: 0x04000123 RID: 291
		public int proxyPort;

		// Token: 0x04000121 RID: 289
		public int proxyType;

		// Token: 0x04000126 RID: 294
		public string proxyUserAgent;

		// Token: 0x04000115 RID: 277
		public bool random;

		// Token: 0x0400011D RID: 285
		public int randomAlgorithm;

		// Token: 0x0400011C RID: 284
		public int reconnectTimes;

		// Token: 0x0400012A RID: 298
		public bool sameHostForSameTarget;

		// Token: 0x0400012F RID: 303
		private ServerSelectStrategy serverStrategy = new ServerSelectStrategy();

		// Token: 0x04000118 RID: 280
		public bool shareOverLan;

		// Token: 0x04000131 RID: 305
		private SortedDictionary<UriVisitTime, string> time2uri = new SortedDictionary<UriVisitTime, string>();

		// Token: 0x0400012D RID: 301
		public Dictionary<string, string> token = new Dictionary<string, string>();

		// Token: 0x0400011E RID: 286
		public int TTL;

		// Token: 0x04000130 RID: 304
		private Dictionary<string, UriVisitTime> uri2time = new Dictionary<string, UriVisitTime>();

		// Token: 0x020000A5 RID: 165
		private class JsonSerializerStrategy : PocoJsonSerializerStrategy
		{
			// Token: 0x06000558 RID: 1368 RVA: 0x0002AE14 File Offset: 0x00029014
			public override object DeserializeObject(object value, Type type)
			{
				if (type == typeof(int) && value.GetType() == typeof(string))
				{
					return int.Parse(value.ToString());
				}
				if (type == typeof(object) && value.GetType() == typeof(JsonObject) && ((JsonObject)value).Count == PortMapConfig.MemberCount)
				{
					return base.DeserializeObject(value, typeof(PortMapConfig));
				}
				return base.DeserializeObject(value, type);
			}
		}
	}
}
