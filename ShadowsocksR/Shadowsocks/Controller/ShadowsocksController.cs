using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using Shadowsocks.Model;
using Shadowsocks.Util;

namespace Shadowsocks.Controller
{
	// Token: 0x02000052 RID: 82
	public class ShadowsocksController
	{
		// Token: 0x06000301 RID: 769 RVA: 0x0001CB44 File Offset: 0x0001AD44
		public ShadowsocksController()
		{
			this._config = Configuration.Load();
			this._transfer = ServerTransferTotal.Load();
			foreach (Server current in this._config.configs)
			{
				if (this._transfer.servers.ContainsKey(current.server))
				{
					ServerSpeedLog serverSpeedLog = new ServerSpeedLog(((ServerTrans)this._transfer.servers[current.server]).totalUploadBytes, ((ServerTrans)this._transfer.servers[current.server]).totalDownloadBytes);
					current.SetServerSpeedLog(serverSpeedLog);
				}
			}
		}

		// Token: 0x0600030C RID: 780 RVA: 0x0001D250 File Offset: 0x0001B450
		public bool AddServerBySSURL(string ssURL)
		{
			bool result;
			try
			{
				Server item = new Server(ssURL);
				this._config.configs.Add(item);
				this.SaveConfig(this._config);
				result = true;
			}
			catch (Exception arg_28_0)
			{
				Logging.LogUsefulException(arg_28_0);
				result = false;
			}
			return result;
		}

		// Token: 0x06000305 RID: 773 RVA: 0x0001CC54 File Offset: 0x0001AE54
		public Configuration GetConfiguration()
		{
			return Configuration.Load();
		}

		// Token: 0x06000306 RID: 774 RVA: 0x0001CC5B File Offset: 0x0001AE5B
		public Configuration GetCurrentConfiguration()
		{
			return this._config;
		}

		// Token: 0x06000304 RID: 772 RVA: 0x0001CC44 File Offset: 0x0001AE44
		public Server GetCurrentServer()
		{
			return this._config.GetCurrentServer(null, false, false);
		}

		// Token: 0x06000316 RID: 790 RVA: 0x0001D4B4 File Offset: 0x0001B6B4
		protected string GetObfsPartOfSSLink(Server server)
		{
			return string.Concat(new object[]
			{
				server.method,
				":",
				server.password,
				"@",
				server.server,
				":",
				server.server_port
			});
		}

		// Token: 0x06000317 RID: 791 RVA: 0x0001D510 File Offset: 0x0001B710
		public string GetSSLinkForCurrentServer()
		{
			Server currentServer = this.GetCurrentServer();
			string str = Utils.EncodeUrlSafeBase64(this.GetObfsPartOfSSLink(currentServer)).Replace("=", "");
			return "ss://" + str;
		}

		// Token: 0x06000318 RID: 792 RVA: 0x0001D54C File Offset: 0x0001B74C
		public string GetSSLinkForServer(Server server)
		{
			string str = Utils.EncodeUrlSafeBase64(this.GetObfsPartOfSSLink(server)).Replace("=", "");
			return "ss://" + str;
		}

		// Token: 0x06000319 RID: 793 RVA: 0x0001D580 File Offset: 0x0001B780
		public string GetSSRRemarksLinkForServer(Server server)
		{
			string arg_154_0 = string.Concat(new object[]
			{
				server.server,
				":",
				server.server_port,
				":",
				server.protocol,
				":",
				server.method,
				":",
				server.obfs,
				":",
				Utils.EncodeUrlSafeBase64(server.password).Replace("=", "")
			});
			string text = "obfsparam=" + Utils.EncodeUrlSafeBase64(server.obfsparam).Replace("=", "");
			if (server.remarks.Length > 0)
			{
				text = text + "&remarks=" + Utils.EncodeUrlSafeBase64(server.remarks).Replace("=", "");
			}
			if (server.group != null && server.group.Length > 0)
			{
				text = text + "&group=" + Utils.EncodeUrlSafeBase64(server.group).Replace("=", "");
			}
			if (server.udp_over_tcp)
			{
				text += "&uot=1";
			}
			if (server.server_udp_port > 0)
			{
				text = text + "&udpport=" + server.server_udp_port.ToString();
			}
			string str = Utils.EncodeUrlSafeBase64(arg_154_0 + "/?" + text).Replace("=", "");
			return "ssr://" + str;
		}

		// Token: 0x06000307 RID: 775 RVA: 0x0001CC64 File Offset: 0x0001AE64
		public List<Server> MergeConfiguration(Configuration mergeConfig, List<Server> servers)
		{
			List<Server> list = new List<Server>();
			if (servers != null)
			{
				for (int i = 0; i < servers.Count; i++)
				{
					for (int j = 0; j < mergeConfig.configs.Count; j++)
					{
						if (mergeConfig.configs[j].server == servers[i].server && mergeConfig.configs[j].server_port == servers[i].server_port && mergeConfig.configs[j].server_udp_port == servers[i].server_udp_port && mergeConfig.configs[j].method == servers[i].method && mergeConfig.configs[j].protocol == servers[i].protocol && mergeConfig.configs[j].obfs == servers[i].obfs && mergeConfig.configs[j].password == servers[i].password && mergeConfig.configs[j].udp_over_tcp == servers[i].udp_over_tcp)
						{
							servers[i].CopyServer(mergeConfig.configs[j]);
							break;
						}
					}
				}
			}
			for (int k = 0; k < mergeConfig.configs.Count; k++)
			{
				int num = 0;
				while (num < servers.Count && (!(mergeConfig.configs[k].server == servers[num].server) || mergeConfig.configs[k].server_port != servers[num].server_port || mergeConfig.configs[k].server_udp_port != servers[num].server_udp_port || !(mergeConfig.configs[k].method == servers[num].method) || !(mergeConfig.configs[k].protocol == servers[num].protocol) || !(mergeConfig.configs[k].obfs == servers[num].obfs) || !(mergeConfig.configs[k].password == servers[num].password) || mergeConfig.configs[k].udp_over_tcp != servers[num].udp_over_tcp))
				{
					num++;
				}
				if (num == servers.Count)
				{
					list.Add(mergeConfig.configs[k]);
				}
			}
			return list;
		}

		// Token: 0x06000308 RID: 776 RVA: 0x0001CF64 File Offset: 0x0001B164
		public Configuration MergeGetConfiguration(Configuration mergeConfig)
		{
			Configuration configuration = Configuration.Load();
			if (mergeConfig != null)
			{
				this.MergeConfiguration(mergeConfig, configuration.configs);
			}
			return configuration;
		}

		// Token: 0x06000320 RID: 800 RVA: 0x0001DC14 File Offset: 0x0001BE14
		private void pacServer_PACFileChanged(object sender, EventArgs e)
		{
			this.UpdateSystemProxy();
		}

		// Token: 0x06000321 RID: 801 RVA: 0x0001DC1C File Offset: 0x0001BE1C
		private void pacServer_PACUpdateCompleted(object sender, GFWListUpdater.ResultEventArgs e)
		{
			if (this.UpdatePACFromGFWListCompleted != null)
			{
				this.UpdatePACFromGFWListCompleted(sender, e);
			}
		}

		// Token: 0x06000322 RID: 802 RVA: 0x0001DC33 File Offset: 0x0001BE33
		private void pacServer_PACUpdateError(object sender, ErrorEventArgs e)
		{
			if (this.UpdatePACFromGFWListError != null)
			{
				this.UpdatePACFromGFWListError(sender, e);
			}
		}

		// Token: 0x06000324 RID: 804 RVA: 0x0001DC7A File Offset: 0x0001BE7A
		private void ReleaseMemory()
		{
			while (true)
			{
				Utils.ReleaseMemory();
				Thread.Sleep(30000);
			}
		}

		// Token: 0x0600031D RID: 797 RVA: 0x0001D758 File Offset: 0x0001B958
		protected void Reload()
		{
			if (this._port_map_listener != null)
			{
				using (List<Listener>.Enumerator enumerator = this._port_map_listener.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						enumerator.Current.Stop();
					}
				}
				this._port_map_listener = null;
			}
			this._config = this.MergeGetConfiguration(this._config);
			this._config.FlushPortMapCache();
			if (this.polipoRunner == null)
			{
				this.polipoRunner = new HttpProxyRunner();
			}
			if (this._pacServer == null)
			{
				this._pacServer = new PACServer();
				this._pacServer.PACFileChanged += new EventHandler(this.pacServer_PACFileChanged);
			}
			this._pacServer.UpdateConfiguration(this._config);
			if (this.gfwListUpdater == null)
			{
				this.gfwListUpdater = new GFWListUpdater();
				this.gfwListUpdater.UpdateCompleted += new EventHandler<GFWListUpdater.ResultEventArgs>(this.pacServer_PACUpdateCompleted);
				this.gfwListUpdater.Error += new ErrorEventHandler(this.pacServer_PACUpdateError);
			}
			bool flag = this.firstRun;
			for (int i = 1; i <= 5; i++)
			{
				flag = false;
				try
				{
					if (this._listener != null && !this._listener.isConfigChange(this._config))
					{
						Local value = new Local(this._config, this._transfer);
						this._listener.GetServices()[0] = value;
						if (this.polipoRunner.HasExited())
						{
							this.polipoRunner.Stop();
							this.polipoRunner.Start(this._config);
							this._listener.GetServices()[3] = new HttpPortForwarder(this.polipoRunner.RunningPort, this._config);
						}
					}
					else
					{
						if (this._listener != null)
						{
							this._listener.Stop();
							this._listener = null;
						}
						this.polipoRunner.Stop();
						this.polipoRunner.Start(this._config);
						Local item = new Local(this._config, this._transfer);
						this._listener = new Listener(new List<Listener.Service>
						{
							item,
							this._pacServer,
							new APIServer(this, this._config),
							new HttpPortForwarder(this.polipoRunner.RunningPort, this._config)
						});
						this._listener.Start(this._config, 0);
					}
					break;
				}
				catch (Exception ex)
				{
					if (ex is SocketException && ((SocketException)ex).SocketErrorCode == SocketError.AccessDenied)
					{
						ex = new Exception(I18N.GetString("Port already in use") + string.Format(" {0}", this._config.localPort), ex);
					}
					Logging.LogUsefulException(ex);
					if (!flag)
					{
						this.ReportError(ex);
						break;
					}
					Thread.Sleep(1000 * i * i);
					if (this._listener != null)
					{
						this._listener.Stop();
						this._listener = null;
					}
				}
			}
			this._port_map_listener = new List<Listener>();
			foreach (KeyValuePair<int, PortMapConfigCache> current in this._config.GetPortMapCache())
			{
				try
				{
					Local item2 = new Local(this._config, this._transfer);
					Listener listener = new Listener(new List<Listener.Service>
					{
						item2
					});
					listener.Start(this._config, current.Key);
					this._port_map_listener.Add(listener);
				}
				catch (Exception ex2)
				{
					if (ex2 is SocketException && ((SocketException)ex2).SocketErrorCode == SocketError.AccessDenied)
					{
						ex2 = new Exception(I18N.GetString("Port already in use") + string.Format(" {0}", current.Key), ex2);
					}
					Logging.LogUsefulException(ex2);
					this.ReportError(ex2);
				}
			}
			if (this.ConfigChanged != null)
			{
				this.ConfigChanged(this, new EventArgs());
			}
			this.UpdateSystemProxy();
			Utils.ReleaseMemory();
		}

		// Token: 0x06000303 RID: 771 RVA: 0x0001CC28 File Offset: 0x0001AE28
		protected void ReportError(Exception e)
		{
			if (this.Errored != null)
			{
				this.Errored(this, new ErrorEventArgs(e));
			}
		}

		// Token: 0x0600031E RID: 798 RVA: 0x0001DBC8 File Offset: 0x0001BDC8
		protected void SaveConfig(Configuration newConfig)
		{
			Configuration.Save(newConfig);
			this.Reload();
		}

		// Token: 0x06000309 RID: 777 RVA: 0x0001CF8C File Offset: 0x0001B18C
		public void SaveServers(List<Server> servers, int localPort)
		{
			List<Server> arg_31_0 = this.MergeConfiguration(this._config, servers);
			this._config.configs = servers;
			this._config.localPort = localPort;
			this.SaveConfig(this._config);
			using (List<Server>.Enumerator enumerator = arg_31_0.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					enumerator.Current.GetConnections().CloseAll();
				}
			}
		}

		// Token: 0x0600030A RID: 778 RVA: 0x0001D00C File Offset: 0x0001B20C
		public bool SaveServersConfig(string config)
		{
			Configuration configuration = this._config.Load(config);
			if (configuration != null)
			{
				this.SaveServersConfig(configuration);
				return true;
			}
			return false;
		}

		// Token: 0x0600030B RID: 779 RVA: 0x0001D034 File Offset: 0x0001B234
		public void SaveServersConfig(Configuration config)
		{
			List<Server> arg_1BB_0 = this.MergeConfiguration(this._config, config.configs);
			this._config.configs = config.configs;
			this._config.index = config.index;
			this._config.random = config.random;
			this._config.global = config.global;
			this._config.enabled = config.enabled;
			this._config.shareOverLan = config.shareOverLan;
			this._config.bypassWhiteList = config.bypassWhiteList;
			this._config.localPort = config.localPort;
			this._config.reconnectTimes = config.reconnectTimes;
			this._config.randomAlgorithm = config.randomAlgorithm;
			this._config.TTL = config.TTL;
			this._config.dns_server = config.dns_server;
			this._config.proxyEnable = config.proxyEnable;
			this._config.pacDirectGoProxy = config.pacDirectGoProxy;
			this._config.proxyType = config.proxyType;
			this._config.proxyHost = config.proxyHost;
			this._config.proxyPort = config.proxyPort;
			this._config.proxyAuthUser = config.proxyAuthUser;
			this._config.proxyAuthPass = config.proxyAuthPass;
			this._config.proxyUserAgent = config.proxyUserAgent;
			this._config.authUser = config.authUser;
			this._config.authPass = config.authPass;
			this._config.autoBan = config.autoBan;
			this._config.sameHostForSameTarget = config.sameHostForSameTarget;
			this._config.keepVisitTime = config.keepVisitTime;
			using (List<Server>.Enumerator enumerator = arg_1BB_0.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					enumerator.Current.GetConnections().CloseAll();
				}
			}
			this.SelectServerIndex(this._config.index);
		}

		// Token: 0x06000312 RID: 786 RVA: 0x0001D366 File Offset: 0x0001B566
		public void SelectServerIndex(int index)
		{
			this._config.index = index;
			this.SaveConfig(this._config);
		}

		// Token: 0x06000325 RID: 805 RVA: 0x0001DC8C File Offset: 0x0001BE8C
		public void ShowConfigForm()
		{
			if (this.ShowConfigFormEvent != null)
			{
				this.ShowConfigFormEvent(this, new EventArgs());
			}
		}

		// Token: 0x06000302 RID: 770 RVA: 0x0001CC20 File Offset: 0x0001AE20
		public void Start()
		{
			this.Reload();
		}

		// Token: 0x06000323 RID: 803 RVA: 0x0001DC4A File Offset: 0x0001BE4A
		private void StartReleasingMemory()
		{
			this._ramThread = new Thread(new ThreadStart(this.ReleaseMemory));
			this._ramThread.IsBackground = true;
			this._ramThread.Start();
		}

		// Token: 0x06000313 RID: 787 RVA: 0x0001D380 File Offset: 0x0001B580
		public void Stop()
		{
			if (this.stopped)
			{
				return;
			}
			this.stopped = true;
			if (this._port_map_listener != null)
			{
				using (List<Listener>.Enumerator enumerator = this._port_map_listener.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						enumerator.Current.Stop();
					}
				}
				this._port_map_listener = null;
			}
			if (this._listener != null)
			{
				this._listener.Stop();
			}
			if (this.polipoRunner != null)
			{
				this.polipoRunner.Stop();
			}
			if (this._config.enabled)
			{
				SystemProxy.Update(this._config, true);
			}
			ServerTransferTotal.Save(this._transfer);
		}

		// Token: 0x0600030F RID: 783 RVA: 0x0001D312 File Offset: 0x0001B512
		public void ToggleBypass(bool bypass)
		{
			this._config.bypassWhiteList = bypass;
			this.UpdateSystemProxy();
			this.SaveConfig(this._config);
		}

		// Token: 0x0600030D RID: 781 RVA: 0x0001D2A0 File Offset: 0x0001B4A0
		public void ToggleEnable(bool enabled)
		{
			this._config.enabled = enabled;
			this.UpdateSystemProxy();
			this.SaveConfig(this._config);
			if (this.EnableStatusChanged != null)
			{
				this.EnableStatusChanged(this, new EventArgs());
			}
		}

		// Token: 0x0600030E RID: 782 RVA: 0x0001D2D9 File Offset: 0x0001B4D9
		public void ToggleGlobal(bool global)
		{
			this._config.global = global;
			this.UpdateSystemProxy();
			this.SaveConfig(this._config);
			if (this.EnableGlobalChanged != null)
			{
				this.EnableGlobalChanged(this, new EventArgs());
			}
		}

		// Token: 0x06000311 RID: 785 RVA: 0x0001D34C File Offset: 0x0001B54C
		public void ToggleSameHostForSameTargetRandom(bool enabled)
		{
			this._config.sameHostForSameTarget = enabled;
			this.SaveConfig(this._config);
		}

		// Token: 0x06000310 RID: 784 RVA: 0x0001D332 File Offset: 0x0001B532
		public void ToggleSelectRandom(bool enabled)
		{
			this._config.random = enabled;
			this.SaveConfig(this._config);
		}

		// Token: 0x06000314 RID: 788 RVA: 0x0001D43C File Offset: 0x0001B63C
		public void TouchPACFile()
		{
			string path = this._pacServer.TouchPACFile();
			if (this.PACFileReadyToOpen != null)
			{
				this.PACFileReadyToOpen(this, new ShadowsocksController.PathEventArgs
				{
					Path = path
				});
			}
		}

		// Token: 0x06000315 RID: 789 RVA: 0x0001D478 File Offset: 0x0001B678
		public void TouchUserRuleFile()
		{
			string path = this._pacServer.TouchUserRuleFile();
			if (this.UserRuleFileReadyToOpen != null)
			{
				this.UserRuleFileReadyToOpen(this, new ShadowsocksController.PathEventArgs
				{
					Path = path
				});
			}
		}

		// Token: 0x0600031C RID: 796 RVA: 0x0001D73D File Offset: 0x0001B93D
		public void UpdateBypassListFromDefault()
		{
			if (this.gfwListUpdater != null)
			{
				this.gfwListUpdater.UpdateBypassListFromDefault(this._config);
			}
		}

		// Token: 0x0600031A RID: 794 RVA: 0x0001D706 File Offset: 0x0001B906
		public void UpdatePACFromGFWList()
		{
			if (this.gfwListUpdater != null)
			{
				this.gfwListUpdater.UpdatePACFromGFWList(this._config);
			}
		}

		// Token: 0x0600031B RID: 795 RVA: 0x0001D721 File Offset: 0x0001B921
		public void UpdatePACFromOnlinePac(string url)
		{
			if (this.gfwListUpdater != null)
			{
				this.gfwListUpdater.UpdatePACFromGFWList(this._config, url);
			}
		}

		// Token: 0x0600031F RID: 799 RVA: 0x0001DBD6 File Offset: 0x0001BDD6
		private void UpdateSystemProxy()
		{
			if (this._config.enabled)
			{
				SystemProxy.Update(this._config, false);
				this._systemProxyIsDirty = true;
				return;
			}
			if (this._systemProxyIsDirty)
			{
				SystemProxy.Update(this._config, false);
				this._systemProxyIsDirty = false;
			}
		}

		// Token: 0x14000006 RID: 6
		// Token: 0x060002EF RID: 751 RVA: 0x0001C754 File Offset: 0x0001A954
		// Token: 0x060002F0 RID: 752 RVA: 0x0001C78C File Offset: 0x0001A98C
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event EventHandler ConfigChanged;

		// Token: 0x14000008 RID: 8
		// Token: 0x060002F3 RID: 755 RVA: 0x0001C834 File Offset: 0x0001AA34
		// Token: 0x060002F4 RID: 756 RVA: 0x0001C86C File Offset: 0x0001AA6C
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event EventHandler EnableGlobalChanged;

		// Token: 0x14000007 RID: 7
		// Token: 0x060002F1 RID: 753 RVA: 0x0001C7C4 File Offset: 0x0001A9C4
		// Token: 0x060002F2 RID: 754 RVA: 0x0001C7FC File Offset: 0x0001A9FC
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event EventHandler EnableStatusChanged;

		// Token: 0x1400000E RID: 14
		// Token: 0x060002FF RID: 767 RVA: 0x0001CAD4 File Offset: 0x0001ACD4
		// Token: 0x06000300 RID: 768 RVA: 0x0001CB0C File Offset: 0x0001AD0C
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event ErrorEventHandler Errored;

		// Token: 0x1400000A RID: 10
		// Token: 0x060002F7 RID: 759 RVA: 0x0001C914 File Offset: 0x0001AB14
		// Token: 0x060002F8 RID: 760 RVA: 0x0001C94C File Offset: 0x0001AB4C
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event EventHandler<ShadowsocksController.PathEventArgs> PACFileReadyToOpen;

		// Token: 0x14000009 RID: 9
		// Token: 0x060002F5 RID: 757 RVA: 0x0001C8A4 File Offset: 0x0001AAA4
		// Token: 0x060002F6 RID: 758 RVA: 0x0001C8DC File Offset: 0x0001AADC
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event EventHandler ShowConfigFormEvent;

		// Token: 0x1400000C RID: 12
		// Token: 0x060002FB RID: 763 RVA: 0x0001C9F4 File Offset: 0x0001ABF4
		// Token: 0x060002FC RID: 764 RVA: 0x0001CA2C File Offset: 0x0001AC2C
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event EventHandler<GFWListUpdater.ResultEventArgs> UpdatePACFromGFWListCompleted;

		// Token: 0x1400000D RID: 13
		// Token: 0x060002FD RID: 765 RVA: 0x0001CA64 File Offset: 0x0001AC64
		// Token: 0x060002FE RID: 766 RVA: 0x0001CA9C File Offset: 0x0001AC9C
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event ErrorEventHandler UpdatePACFromGFWListError;

		// Token: 0x1400000B RID: 11
		// Token: 0x060002F9 RID: 761 RVA: 0x0001C984 File Offset: 0x0001AB84
		// Token: 0x060002FA RID: 762 RVA: 0x0001C9BC File Offset: 0x0001ABBC
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event EventHandler<ShadowsocksController.PathEventArgs> UserRuleFileReadyToOpen;

		// Token: 0x04000253 RID: 595
		private bool firstRun = true;

		// Token: 0x04000251 RID: 593
		private GFWListUpdater gfwListUpdater;

		// Token: 0x04000250 RID: 592
		private HttpProxyRunner polipoRunner;

		// Token: 0x04000252 RID: 594
		private bool stopped;

		// Token: 0x0400024E RID: 590
		private Configuration _config;

		// Token: 0x0400024B RID: 587
		private Listener _listener;

		// Token: 0x0400024D RID: 589
		private PACServer _pacServer;

		// Token: 0x0400024C RID: 588
		private List<Listener> _port_map_listener;

		// Token: 0x0400024A RID: 586
		private Thread _ramThread;

		// Token: 0x04000254 RID: 596
		private bool _systemProxyIsDirty;

		// Token: 0x0400024F RID: 591
		private ServerTransferTotal _transfer;

		// Token: 0x020000B4 RID: 180
		public class PathEventArgs : EventArgs
		{
			// Token: 0x04000466 RID: 1126
			public string Path;
		}
	}
}
