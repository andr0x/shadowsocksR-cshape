using System;
using System.Collections.Generic;
using System.IO;
using SimpleJson;

namespace Shadowsocks.Model
{
	// Token: 0x0200002B RID: 43
	[Serializable]
	public class ServerTransferTotal
	{
		// Token: 0x06000189 RID: 393 RVA: 0x0001242C File Offset: 0x0001062C
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

		// Token: 0x06000188 RID: 392 RVA: 0x00012338 File Offset: 0x00010538
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

		// Token: 0x06000186 RID: 390 RVA: 0x0001229C File Offset: 0x0001049C
		public void Init()
		{
			this.saveCounter = 256;
			this.saveTime = DateTime.Now;
			if (this.servers == null)
			{
				this.servers = new Dictionary<string, object>();
			}
		}

		// Token: 0x06000185 RID: 389 RVA: 0x0001223C File Offset: 0x0001043C
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

		// Token: 0x06000187 RID: 391 RVA: 0x000122C8 File Offset: 0x000104C8
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

		// Token: 0x04000141 RID: 321
		private static string LOG_FILE = "transfer_log.json";

		// Token: 0x04000143 RID: 323
		private int saveCounter;

		// Token: 0x04000144 RID: 324
		private DateTime saveTime;

		// Token: 0x04000142 RID: 322
		public Dictionary<string, object> servers = new Dictionary<string, object>();

		// Token: 0x020000A4 RID: 164
		private class JsonSerializerStrategy : PocoJsonSerializerStrategy
		{
			// Token: 0x06000551 RID: 1361 RVA: 0x0002BC70 File Offset: 0x00029E70
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
