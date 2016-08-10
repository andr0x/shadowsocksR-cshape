using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Shadowsocks.Encryption;
using Shadowsocks.Model;

namespace Shadowsocks.Controller
{
	// Token: 0x0200004D RID: 77
	internal class Local : Listener.Service
	{
		// Token: 0x0600029C RID: 668 RVA: 0x00018990 File Offset: 0x00016B90
		public Local(Configuration config, ServerTransferTotal transfer)
		{
			this._config = config;
			this._transfer = transfer;
		}

		// Token: 0x0600029D RID: 669 RVA: 0x000189A6 File Offset: 0x00016BA6
		protected bool Accept(byte[] firstPacket, int length)
		{
			return length >= 2 && (firstPacket[0] == 5 || firstPacket[0] == 4);
		}

		// Token: 0x0600029E RID: 670 RVA: 0x000189C0 File Offset: 0x00016BC0
		private bool AuthConnection(Socket connection, string authUser, string authPass)
		{
			return (this._config.authUser ?? "").Length == 0 || (this._config.authUser == authUser && (this._config.authPass ?? "") == authPass);
		}

		// Token: 0x0600029F RID: 671 RVA: 0x00018A1C File Offset: 0x00016C1C
		public bool Handle(byte[] firstPacket, int length, Socket socket)
		{
			int port = ((IPEndPoint)socket.LocalEndPoint).Port;
			if (!this._config.GetPortMapCache().ContainsKey(port) && !this.Accept(firstPacket, length))
			{
				return false;
			}
			socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, true);
			Handler handler = new Handler();
			handler.getCurrentServer = ((string targetURI, bool usingRandom, bool forceRandom) => this._config.GetCurrentServer(targetURI, usingRandom, forceRandom));
			handler.keepCurrentServer = delegate(string targetURI)
			{
				this._config.KeepCurrentServer(targetURI);
			};
			handler.connection = socket;
			handler.reconnectTimesRemain = this._config.reconnectTimes;
			handler.forceRandom = this._config.random;
			handler.setServerTransferTotal(this._transfer);
			if (this._config.proxyEnable)
			{
				handler.proxyType = this._config.proxyType;
				handler.socks5RemoteHost = this._config.proxyHost;
				handler.socks5RemotePort = this._config.proxyPort;
				handler.socks5RemoteUsername = this._config.proxyAuthUser;
				handler.socks5RemotePassword = this._config.proxyAuthPass;
				handler.proxyUserAgent = this._config.proxyUserAgent;
			}
			handler.TTL = (double)this._config.TTL;
			handler.autoSwitchOff = this._config.autoBan;
			if (this._config.authUser != null && this._config.authUser.Length > 0)
			{
				handler.authConnection = new Handler.AuthConnection(this.AuthConnection);
				handler.authUser = (this._config.authUser ?? "");
				handler.authPass = (this._config.authPass ?? "");
			}
			if (this._config.dns_server != null && this._config.dns_server.Length > 0)
			{
				handler.dns_servers = this._config.dns_server;
			}
			if (!Local.EncryptorLoaded)
			{
				try
				{
					handler.server = this._config.GetCurrentServer(null, true, false);
					EncryptorFactory.GetEncryptor(handler.server.method, handler.server.password).Dispose();
				}
				catch
				{
				}
				finally
				{
					Local.EncryptorLoaded = true;
					handler.server = null;
				}
			}
			if (this._config.GetPortMapCache().ContainsKey(port))
			{
				PortMapConfigCache portMapConfigCache = this._config.GetPortMapCache()[port];
				if (portMapConfigCache.id == portMapConfigCache.server.id)
				{
					handler.select_server = portMapConfigCache.server;
					handler.command = 1;
					byte[] bytes = Encoding.UTF8.GetBytes(portMapConfigCache.server_addr);
					byte[] array = new byte[length + bytes.Length + 4];
					array[0] = 3;
					array[1] = (byte)bytes.Length;
					Array.Copy(bytes, 0, array, 2, bytes.Length);
					array[bytes.Length + 2] = (byte)(portMapConfigCache.server_port / 256);
					array[bytes.Length + 3] = (byte)(portMapConfigCache.server_port % 256);
					Array.Copy(firstPacket, 0, array, bytes.Length + 4, length);
					Handler arg_2E2_0 = handler;
					byte[] expr_2DE = array;
					arg_2E2_0.Start(expr_2DE, expr_2DE.Length, true);
					return true;
				}
			}
			handler.Start(firstPacket, length, false);
			return true;
		}

		// Token: 0x04000200 RID: 512
		private static bool EncryptorLoaded;

		// Token: 0x040001FE RID: 510
		private Configuration _config;

		// Token: 0x040001FF RID: 511
		private ServerTransferTotal _transfer;
	}
}
