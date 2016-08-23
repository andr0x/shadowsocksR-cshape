using System;
using System.Collections.Generic;
using System.IO;
using SimpleJson;

namespace Shadowsocks.Model
{
	// Token: 0x02000029 RID: 41
	[Serializable]
	public class ServerTransferTotal
	{
		// Token: 0x06000175 RID: 373 RVA: 0x00011A9C File Offset: 0x0000FC9C
		public void AddDownload(string server, long size)
		{
			Dictionary<string, object> obj = this.servers;
			lock (obj)
			{
				if (!this.servers.ContainsKey(server))
				{
					this.servers.Add(server, new ServerTrans());
				}
				((ServerTrans)this.servers[server]).totalDownloadBytes += size;
			}
			int num = this.saveCounter - 1;
			this.saveCounter = num;
			if (num <= 0)
			{
				this.saveCounter = 256;
				if ((DateTime.Now - this.saveTime).TotalMinutes > 10.0)
				{
					obj = this.servers;
					lock (obj)
					{
						ServerTransferTotal.Save(this);
						this.saveTime = DateTime.Now;
					}
				}
			}
		}

		// Token: 0x06000174 RID: 372 RVA: 0x000119A8 File Offset: 0x0000FBA8
		public void AddUpload(string server, long size)
		{
			Dictionary<string, object> obj = this.servers;
			lock (obj)
			{
				if (!this.servers.ContainsKey(server))
				{
					this.servers.Add(server, new ServerTrans());
				}
				((ServerTrans)this.servers[server]).totalUploadBytes += size;
			}
			int num = this.saveCounter - 1;
			this.saveCounter = num;
			if (num <= 0)
			{
				this.saveCounter = 256;
				if ((DateTime.Now - this.saveTime).TotalMinutes > 10.0)
				{
					obj = this.servers;
					lock (obj)
					{
						ServerTransferTotal.Save(this);
						this.saveTime = DateTime.Now;
					}
				}
			}
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0001190C File Offset: 0x0000FB0C
		public void Init()
		{
			this.saveCounter = 256;
			this.saveTime = DateTime.Now;
			if (this.servers == null)
			{
				this.servers = new Dictionary<string, object>();
			}
		}

		// Token: 0x06000171 RID: 369 RVA: 0x000118AC File Offset: 0x0000FAAC
		public static ServerTransferTotal Load()
		{
			ServerTransferTotal result;
			try
			{
				string json = File.ReadAllText(ServerTransferTotal.LOG_FILE);
				ServerTransferTotal expr_10 = new ServerTransferTotal();
				expr_10.servers = SimpleJson.SimpleJson.DeserializeObject<Dictionary<string, object>>(json, new ServerTransferTotal.JsonSerializerStrategy());
				expr_10.Init();
				result = expr_10;
			}
			catch (Exception ex)
			{
				if (!(ex is FileNotFoundException))
				{
					Console.WriteLine(ex);
				}
				result = new ServerTransferTotal();
			}
			return result;
		}

		// Token: 0x06000173 RID: 371 RVA: 0x00011938 File Offset: 0x0000FB38
		public static void Save(ServerTransferTotal config)
		{
			try
			{
				using (StreamWriter streamWriter = new StreamWriter(File.Open(ServerTransferTotal.LOG_FILE, FileMode.Create)))
				{
					string value = SimpleJson.SimpleJson.SerializeObject(config.servers);
					streamWriter.Write(value);
					streamWriter.Flush();
				}
			}
			catch (IOException value2)
			{
				Console.Error.WriteLine(value2);
			}
		}

		// Token: 0x04000136 RID: 310
		private static string LOG_FILE = "transfer_log.json";

		// Token: 0x04000138 RID: 312
		private int saveCounter;

		// Token: 0x04000139 RID: 313
		private DateTime saveTime;

		// Token: 0x04000137 RID: 311
		public Dictionary<string, object> servers = new Dictionary<string, object>();

		// Token: 0x020000A6 RID: 166
		private class JsonSerializerStrategy : PocoJsonSerializerStrategy
		{
			// Token: 0x0600055A RID: 1370 RVA: 0x0002AEB8 File Offset: 0x000290B8
			public override object DeserializeObject(object value, Type type)
			{
				if (type == typeof(long) && value.GetType() == typeof(string))
				{
					return long.Parse(value.ToString());
				}
				if (type == typeof(object))
				{
					return base.DeserializeObject(value, typeof(ServerTrans));
				}
				return base.DeserializeObject(value, type);
			}
		}
	}
}
