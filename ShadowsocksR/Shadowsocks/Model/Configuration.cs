using System;
using System.Collections.Generic;
using System.IO;
using Shadowsocks.Controller;
using SimpleJson;

namespace Shadowsocks.Model
{
	// Token: 0x02000029 RID: 41
	[Serializable]
	public class Configuration
	{
		// Token: 0x06000177 RID: 375 RVA: 0x00011E0C File Offset: 0x0001000C
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

		// Token: 0x0600017D RID: 381 RVA: 0x000121A9 File Offset: 0x000103A9
		private static void Assert(bool condition)
		{
			if (!condition)
			{
				throw new Exception(I18N.GetString("assertion failure"));
			}
		}

		// Token: 0x0600017F RID: 383 RVA: 0x000121DC File Offset: 0x000103DC
		private static void CheckPassword(string password)
		{
			if (string.IsNullOrEmpty(password))
			{
				throw new ArgumentException(I18N.GetString("Password can not be blank"));
			}
		}

		// Token: 0x0600017E RID: 382 RVA: 0x000121BE File Offset: 0x000103BE
		public static void CheckPort(int port)
		{
			if (port <= 0 || port > 65535)
			{
				throw new ArgumentException(I18N.GetString("Port out of range"));
			}
		}

		// Token: 0x06000176 RID: 374 RVA: 0x00011DD4 File Offset: 0x0000FFD4
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

		// Token: 0x06000180 RID: 384 RVA: 0x000121F6 File Offset: 0x000103F6
		private static void CheckServer(string server)
		{
			if (string.IsNullOrEmpty(server))
			{
				throw new ArgumentException(I18N.GetString("Server IP can not be blank"));
			}
		}

		// Token: 0x06000174 RID: 372 RVA: 0x00011C54 File Offset: 0x0000FE54
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

		// Token: 0x06000173 RID: 371 RVA: 0x000118CC File Offset: 0x0000FACC
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

		// Token: 0x0600017B RID: 379 RVA: 0x00012190 File Offset: 0x00010390
		public static Server GetDefaultServer()
		{
			return new Server();
		}

		// Token: 0x0600017C RID: 380 RVA: 0x00012197 File Offset: 0x00010397
		public static Server GetErrorServer()
		{
			return new Server
			{
				server = "invalid"
			};
		}

		// Token: 0x06000175 RID: 373 RVA: 0x00011DCC File Offset: 0x0000FFCC
		public Dictionary<int, PortMapConfigCache> GetPortMapCache()
		{
			return this.portMapCache;
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00011800 File Offset: 0x0000FA00
		public bool KeepCurrentServer(string targetAddr)
		{
			if (this.sameHostForSameTarget && targetAddr != null)
			{
				ServerSelectStrategy obj = this.serverStrategy;
				lock (obj)
				{
					if (this.uri2time.ContainsKey(targetAddr))
					{
						UriVisitTime uriVisitTime = this.uri2time[targetAddr];
						if (uriVisitTime.index < this.configs.Count && this.configs[uriVisitTime.index].enable)
						{
							this.time2uri.Remove(uriVisitTime);
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

		// Token: 0x06000178 RID: 376 RVA: 0x00011EA4 File Offset: 0x000100A4
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

		// Token: 0x0600017A RID: 378 RVA: 0x00012158 File Offset: 0x00010358
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

		// Token: 0x06000179 RID: 377 RVA: 0x000120B0 File Offset: 0x000102B0
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

		// Token: 0x04000133 RID: 307
		public string authPass;

		// Token: 0x04000132 RID: 306
		public string authUser;

		// Token: 0x04000134 RID: 308
		public bool autoBan;

		// Token: 0x04000125 RID: 293
		public bool bypassWhiteList;

		// Token: 0x0400011E RID: 286
		public List<Server> configs;

		// Token: 0x0400013E RID: 318
		private static string CONFIG_FILE = "gui-config.json";

		// Token: 0x04000137 RID: 311
		public string dns_server;

		// Token: 0x04000122 RID: 290
		public bool enabled;

		// Token: 0x04000121 RID: 289
		public bool global;

		// Token: 0x0400011F RID: 287
		public int index;

		// Token: 0x04000124 RID: 292
		public bool isDefault;

		// Token: 0x04000136 RID: 310
		public int keepVisitTime;

		// Token: 0x04000126 RID: 294
		public int localPort;

		// Token: 0x0400012B RID: 299
		public bool pacDirectGoProxy;

		// Token: 0x04000139 RID: 313
		public Dictionary<string, object> portMap = new Dictionary<string, object>();

		// Token: 0x0400013D RID: 317
		private Dictionary<int, PortMapConfigCache> portMapCache = new Dictionary<int, PortMapConfigCache>();

		// Token: 0x04000130 RID: 304
		public string proxyAuthPass;

		// Token: 0x0400012F RID: 303
		public string proxyAuthUser;

		// Token: 0x0400012A RID: 298
		public bool proxyEnable;

		// Token: 0x0400012D RID: 301
		public string proxyHost;

		// Token: 0x0400012E RID: 302
		public int proxyPort;

		// Token: 0x0400012C RID: 300
		public int proxyType;

		// Token: 0x04000131 RID: 305
		public string proxyUserAgent;

		// Token: 0x04000120 RID: 288
		public bool random;

		// Token: 0x04000128 RID: 296
		public int randomAlgorithm;

		// Token: 0x04000127 RID: 295
		public int reconnectTimes;

		// Token: 0x04000135 RID: 309
		public bool sameHostForSameTarget;

		// Token: 0x0400013A RID: 314
		private ServerSelectStrategy serverStrategy = new ServerSelectStrategy();

		// Token: 0x04000123 RID: 291
		public bool shareOverLan;

		// Token: 0x0400013C RID: 316
		private SortedDictionary<UriVisitTime, string> time2uri = new SortedDictionary<UriVisitTime, string>();

		// Token: 0x04000138 RID: 312
		public Dictionary<string, string> token = new Dictionary<string, string>();

		// Token: 0x04000129 RID: 297
		public int TTL;

		// Token: 0x0400013B RID: 315
		private Dictionary<string, UriVisitTime> uri2time = new Dictionary<string, UriVisitTime>();

		// Token: 0x020000A3 RID: 163
		private class JsonSerializerStrategy : PocoJsonSerializerStrategy
		{
			// Token: 0x0600054F RID: 1359 RVA: 0x0002BBCC File Offset: 0x00029DCC
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
