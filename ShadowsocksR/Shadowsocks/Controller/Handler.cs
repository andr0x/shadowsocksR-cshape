using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using OpenDNS;
using Shadowsocks.Encryption;
using Shadowsocks.Model;
using Shadowsocks.Obfs;

namespace Shadowsocks.Controller
{
	// Token: 0x02000051 RID: 81
	internal class Handler
	{
		// Token: 0x060002CC RID: 716 RVA: 0x0001A9A8 File Offset: 0x00018BA8
		private void BeginConnect(IPAddress ipAddress, int serverPort)
		{
			IPEndPoint ep = new IPEndPoint(ipAddress, serverPort);
			if (this.server.server_udp_port == 0)
			{
				new IPEndPoint(ipAddress, serverPort);
				this.remoteUDPEndPoint = ep;
			}
			else
			{
				IPEndPoint iPEndPoint = new IPEndPoint(ipAddress, this.server.server_udp_port);
				this.remoteUDPEndPoint = iPEndPoint;
			}
			if (this.cfg.socks5RemotePort != 0 || this.connectionUDP == null || (this.connectionUDP != null && this.server.udp_over_tcp))
			{
				this.remote = new ProxySocket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				this.remote.GetSocket().SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, true);
				try
				{
					this.remote.SetEncryptor(EncryptorFactory.GetEncryptor(this.server.method, this.server.password));
				}
				catch
				{
				}
				this.remote.SetProtocol(ObfsFactory.GetObfs(this.server.protocol));
				this.remote.SetObfs(ObfsFactory.GetObfs(this.server.obfs));
			}
			if (this.connectionUDP != null && !this.server.udp_over_tcp)
			{
				try
				{
					this.remoteUDP = new ProxySocket(ipAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
					this.remoteUDP.GetSocket().Bind(new IPEndPoint((ipAddress.AddressFamily == AddressFamily.InterNetworkV6) ? IPAddress.IPv6Any : IPAddress.Any, 0));
					this.remoteUDP.SetProtocol(ObfsFactory.GetObfs(this.server.protocol));
					this.remoteUDP.SetObfs(ObfsFactory.GetObfs(this.server.obfs));
				}
				catch (SocketException)
				{
					this.remoteUDP = null;
				}
			}
			this.ResetTimeout(this.cfg.TTL);
			if (this.cfg.socks5RemotePort == 0 && this.connectionUDP != null && !this.server.udp_over_tcp)
			{
				if (this.State == Handler.ConnectState.CONNECTING)
				{
					this.StartPipe();
					return;
				}
			}
			else
			{
				this.speedTester.BeginConnect();
				IAsyncResult asyncResult = this.remote.BeginConnect(ep, new AsyncCallback(this.ConnectCallback), new CallbackStatus());
				double num = this.cfg.TTL;
				if (num <= 0.0)
				{
					num = 120.0;
				}
				else if (num <= 10.0)
				{
					num = 10.0;
				}
				if ((this.cfg.reconnectTimesRemain + this.cfg.reconnectTimes > 0 || this.cfg.TTL > 0.0) && !asyncResult.AsyncWaitHandle.WaitOne((int)(num * 250.0), true))
				{
					((CallbackStatus)asyncResult.AsyncState).SetIfEqu(-1, 0);
					if (((CallbackStatus)asyncResult.AsyncState).Status == -1)
					{
						if (this.lastErrCode == 0)
						{
							this.lastErrCode = 8;
							this.server.ServerSpeedLog().AddTimeoutTimes();
						}
						this.CloseSocket(ref this.remote);
						this.Close();
					}
				}
			}
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x0001ADBC File Offset: 0x00018FBC
		public void Close()
		{
			lock (this)
			{
				if (this.closed)
				{
					return;
				}
				this.closed = true;
			}
			int num = 0;
			while (num < 10 && (this.remoteRecvCount > 0 || this.connectionRecvCount > 0))
			{
				Thread.Sleep(10 * (num + 1) * (num + 1));
				num++;
			}
			this.CloseSocket(ref this.remote);
			this.CloseSocket(ref this.remoteUDP);
			if (this.lastErrCode == 0 && this.server != null)
			{
				if (this.speedTester.sizeRecv == 0L && this.speedTester.sizeUpload > 0L)
				{
					this.server.ServerSpeedLog().AddErrorEmptyTimes();
				}
				else
				{
					this.server.ServerSpeedLog().AddNoErrorTimes();
				}
			}
			this.keepCurrentServer(this.cfg.targetHost, this.server.id);
			this.ResetTimeout(0.0);
			try
			{
				bool arg_154_0 = this.TryReconnect();
				if (this.State != Handler.ConnectState.END)
				{
					if (this.State != Handler.ConnectState.READY && this.State != Handler.ConnectState.HANDSHAKE && this.server != null)
					{
						this.server.ServerSpeedLog().AddDisconnectTimes();
						this.server.GetConnections().DecRef(this.connection);
					}
					this.State = Handler.ConnectState.END;
				}
				this.getCurrentServer = null;
				this.keepCurrentServer = null;
				if (!arg_154_0)
				{
					this.CloseSocket(ref this.connection);
					this.CloseSocket(ref this.connectionUDP);
				}
				else
				{
					this.connection = null;
					this.connectionUDP = null;
				}
			}
			catch (Exception arg_180_0)
			{
				Logging.LogUsefulException(arg_180_0);
			}
		}

		// Token: 0x060002CE RID: 718 RVA: 0x0001ACDC File Offset: 0x00018EDC
		private void CloseSocket(ref Socket sock)
		{
			lock (this)
			{
				if (sock != null)
				{
					Socket socket = sock;
					sock = null;
					try
					{
						socket.Shutdown(SocketShutdown.Both);
					}
					catch
					{
					}
					try
					{
						socket.Close();
					}
					catch
					{
					}
				}
			}
		}

		// Token: 0x060002CF RID: 719 RVA: 0x0001AD4C File Offset: 0x00018F4C
		private void CloseSocket(ref ProxySocket sock)
		{
			lock (this)
			{
				if (sock != null)
				{
					ProxySocket proxySocket = sock;
					sock = null;
					try
					{
						proxySocket.Shutdown(SocketShutdown.Both);
					}
					catch
					{
					}
					try
					{
						proxySocket.Close();
					}
					catch
					{
					}
				}
			}
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x0001B238 File Offset: 0x00019438
		private void Connect()
		{
			this.remote = null;
			this.remoteUDP = null;
			if (this.select_server == null)
			{
				if (this.cfg.targetHost == null)
				{
					this.cfg.targetHost = this.GetQueryString();
					this.server = this.getCurrentServer(this.cfg.targetHost, true, false);
				}
				else
				{
					this.server = this.getCurrentServer(this.cfg.targetHost, true, this.cfg.forceRandom);
				}
			}
			else
			{
				this.server = this.select_server;
			}
			this.speedTester.server = this.server.server;
			this.ResetTimeout(this.cfg.TTL);
			if (this.cfg.fouce_local_dns_query && this.cfg.targetHost != null)
			{
				IPAddress iPAddress = this.QueryDns(this.cfg.targetHost, this.cfg.dns_servers);
				if (iPAddress != null)
				{
					this.server.DnsBuffer().UpdateDns(this.cfg.targetHost, iPAddress);
					this.cfg.targetHost = iPAddress.ToString();
					this.ResetTimeout(this.cfg.TTL);
				}
			}
			lock (this)
			{
				this.server.ServerSpeedLog().AddConnectTimes();
				if (this.State == Handler.ConnectState.HANDSHAKE)
				{
					this.State = Handler.ConnectState.CONNECTING;
				}
				this.server.GetConnections().AddRef(this.connection);
			}
			string socks5RemoteHost = this.server.server;
			int serverPort = this.server.server_port;
			if (this.cfg.socks5RemotePort > 0)
			{
				socks5RemoteHost = this.cfg.socks5RemoteHost;
				serverPort = this.cfg.socks5RemotePort;
			}
			IPAddress iPAddress2;
			if (!IPAddress.TryParse(socks5RemoteHost, out iPAddress2))
			{
				if (this.server.DnsBuffer().isExpired(socks5RemoteHost))
				{
					bool flag2 = false;
					if (!flag2)
					{
						iPAddress2 = this.QueryDns(socks5RemoteHost, this.cfg.dns_servers);
						if (iPAddress2 != null)
						{
							this.server.DnsBuffer().UpdateDns(socks5RemoteHost, iPAddress2);
							flag2 = true;
						}
					}
					if (!flag2)
					{
						this.lastErrCode = 8;
						this.server.ServerSpeedLog().AddTimeoutTimes();
						this.Close();
						return;
					}
				}
				else
				{
					iPAddress2 = this.server.DnsBuffer().ip;
				}
			}
			this.BeginConnect(iPAddress2, serverPort);
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x0001B4A0 File Offset: 0x000196A0
		private void ConnectCallback(IAsyncResult ar)
		{
			if (ar != null && ar.AsyncState != null)
			{
				((CallbackStatus)ar.AsyncState).SetIfEqu(1, 0);
				if (((CallbackStatus)ar.AsyncState).Status != 1)
				{
					return;
				}
			}
			try
			{
				this.remote.EndConnect(ar);
				if (this.cfg.socks5RemotePort > 0 && !this.ConnectProxyServer(this.server.server, this.server.server_port))
				{
					throw new SocketException(10054);
				}
				this.speedTester.EndConnect();
				if (this.State == Handler.ConnectState.CONNECTING)
				{
					this.StartPipe();
				}
			}
			catch (Exception e)
			{
				this.LogExceptionAndClose(e);
			}
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x0001BAE4 File Offset: 0x00019CE4
		private void ConnectionSend(byte[] buffer, int bytesToSend)
		{
			if (this.connectionUDP == null)
			{
				this.connection.BeginSend(buffer, 0, bytesToSend, SocketFlags.None, new AsyncCallback(this.PipeConnectionSendCallback), null);
				return;
			}
			this.connectionUDP.BeginSendTo(buffer, 0, bytesToSend, SocketFlags.None, this.connectionUDPEndPoint, new AsyncCallback(this.PipeConnectionUDPSendCallback), null);
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x0001AF6C File Offset: 0x0001916C
		private bool ConnectProxyServer(string strRemoteHost, int iRemotePort)
		{
			if (this.cfg.proxyType == 0)
			{
				bool arg_B9_0 = this.remote.ConnectSocks5ProxyServer(strRemoteHost, iRemotePort, this.connectionUDP != null && !this.server.udp_over_tcp, this.cfg.socks5RemoteUsername, this.cfg.socks5RemotePassword);
				this.remoteUDPEndPoint = this.remote.GetProxyUdpEndPoint();
				this.remote.SetTcpServer(this.server.server, this.server.server_port);
				this.remote.SetUdpServer(this.server.server, (this.server.server_udp_port == 0) ? this.server.server_port : this.server.server_udp_port);
				return arg_B9_0;
			}
			if (this.cfg.proxyType == 1)
			{
				bool arg_117_0 = this.remote.ConnectHttpProxyServer(strRemoteHost, iRemotePort, this.cfg.socks5RemoteUsername, this.cfg.socks5RemotePassword, this.cfg.proxyUserAgent);
				this.remote.SetTcpServer(this.server.server, this.server.server_port);
				return arg_117_0;
			}
			return true;
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x0001B558 File Offset: 0x00019758
		private void doConnectionTCPRecv()
		{
			if (this.connection != null && this.connectionTCPIdle)
			{
				this.connectionTCPIdle = false;
				this.connection.BeginReceive(this.connetionRecvBuffer, 0, 16384, SocketFlags.None, new AsyncCallback(this.PipeConnectionReceiveCallback), null);
			}
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x0001B5B8 File Offset: 0x000197B8
		private void doConnectionUDPRecv()
		{
			if (this.connectionUDP != null && this.connectionUDPIdle)
			{
				EndPoint endPoint = new IPEndPoint((this.connectionUDP.AddressFamily == AddressFamily.InterNetworkV6) ? IPAddress.IPv6Any : IPAddress.Any, 0);
				this.connectionUDPIdle = false;
				this.connectionUDP.BeginReceiveFrom(this.connetionRecvBuffer, 0, 65536, SocketFlags.None, ref endPoint, new AsyncCallback(this.PipeConnectionUDPReceiveCallback), null);
			}
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x0001B65B File Offset: 0x0001985B
		private void doRemoteTCPRecv()
		{
			if (this.remote != null && this.remoteTCPIdle)
			{
				this.remoteTCPIdle = false;
				this.remote.BeginReceive(this.remoteRecvBuffer, 16384, SocketFlags.None, new AsyncCallback(this.PipeRemoteReceiveCallback), null);
			}
		}

		// Token: 0x060002DB RID: 731 RVA: 0x0001B6D8 File Offset: 0x000198D8
		private void doRemoteUDPRecv()
		{
			if (this.remoteUDP != null && this.remoteUDPIdle)
			{
				EndPoint endPoint = new IPEndPoint((this.remoteUDP.AddressFamily == AddressFamily.InterNetworkV6) ? IPAddress.IPv6Any : IPAddress.Any, 0);
				this.remoteUDPIdle = false;
				this.remoteUDP.BeginReceiveFrom(this.remoteRecvBuffer, 65536, SocketFlags.None, ref endPoint, new AsyncCallback(this.PipeRemoteUDPReceiveCallback), null);
			}
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x0001B597 File Offset: 0x00019797
		private int endConnectionTCPRecv(IAsyncResult ar)
		{
			if (this.connection != null)
			{
				int arg_1B_0 = this.connection.EndReceive(ar);
				this.connectionTCPIdle = true;
				return arg_1B_0;
			}
			return 0;
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x0001B626 File Offset: 0x00019826
		private int endConnectionUDPRecv(IAsyncResult ar, ref EndPoint endPoint)
		{
			if (this.connectionUDP != null)
			{
				int arg_31_0 = this.connectionUDP.EndReceiveFrom(ar, ref endPoint);
				if (this.connectionUDPEndPoint == null)
				{
					this.connectionUDPEndPoint = (IPEndPoint)endPoint;
				}
				this.connectionUDPIdle = true;
				return arg_31_0;
			}
			return 0;
		}

		// Token: 0x060002DA RID: 730 RVA: 0x0001B69C File Offset: 0x0001989C
		private int endRemoteTCPRecv(IAsyncResult ar)
		{
			if (this.remote != null)
			{
				bool flag;
				int arg_2D_0 = this.remote.EndReceive(ar, out flag);
				this.remoteTCPIdle = true;
				if (flag)
				{
					this.RemoteSend(new byte[0], 0);
				}
				return arg_2D_0;
			}
			return 0;
		}

		// Token: 0x060002DC RID: 732 RVA: 0x0001B745 File Offset: 0x00019945
		private int endRemoteUDPRecv(IAsyncResult ar, ref EndPoint endPoint)
		{
			if (this.remoteUDP != null)
			{
				int arg_1C_0 = this.remoteUDP.EndReceiveFrom(ar, ref endPoint);
				this.remoteUDPIdle = true;
				return arg_1C_0;
			}
			return 0;
		}

		// Token: 0x060002DE RID: 734 RVA: 0x0001B7E4 File Offset: 0x000199E4
		private string GetQueryString()
		{
			if (this.remoteHeaderSendBuffer == null)
			{
				return null;
			}
			if (this.remoteHeaderSendBuffer[0] == 1)
			{
				if (this.remoteHeaderSendBuffer.Length > 4)
				{
					byte[] array = new byte[4];
					Array.Copy(this.remoteHeaderSendBuffer, 1, array, 0, 4);
					return new IPAddress(array).ToString();
				}
				return null;
			}
			else if (this.remoteHeaderSendBuffer[0] == 4)
			{
				if (this.remoteHeaderSendBuffer.Length > 16)
				{
					byte[] array2 = new byte[16];
					Array.Copy(this.remoteHeaderSendBuffer, 1, array2, 0, 16);
					return new IPAddress(array2).ToString();
				}
				return null;
			}
			else
			{
				if (this.remoteHeaderSendBuffer[0] == 3 && this.remoteHeaderSendBuffer.Length > 1 && this.remoteHeaderSendBuffer.Length > (int)(this.remoteHeaderSendBuffer[1] + 1))
				{
					return Encoding.UTF8.GetString(this.remoteHeaderSendBuffer, 2, (int)this.remoteHeaderSendBuffer[1]);
				}
				return null;
			}
		}

		// Token: 0x060002EB RID: 747 RVA: 0x0001C630 File Offset: 0x0001A830
		protected string getServerUrl(out string remarks)
		{
			Server server = this.server;
			if (server == null)
			{
				remarks = "";
				return "";
			}
			remarks = server.remarks;
			return server.server;
		}

		// Token: 0x060002EC RID: 748 RVA: 0x0001C664 File Offset: 0x0001A864
		private void LogException(Exception e)
		{
			this.LogSocketException(e);
			string remarks;
			string serverUrl = this.getServerUrl(out remarks);
			if (!Logging.LogSocketException(remarks, serverUrl, e))
			{
				Logging.LogUsefulException(e);
			}
		}

		// Token: 0x060002ED RID: 749 RVA: 0x0001C692 File Offset: 0x0001A892
		private void LogExceptionAndClose(Exception e)
		{
			this.LogException(e);
			this.Close();
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x0001A530 File Offset: 0x00018730
		public int LogSocketException(Exception e)
		{
			Server server = this.server;
			if (e is ObfsException)
			{
				ObfsException arg_15_0 = (ObfsException)e;
				if (this.lastErrCode == 0 && server != null)
				{
					this.lastErrCode = 16;
					server.ServerSpeedLog().AddErrorDecodeTimes();
					if (server.ServerSpeedLog().ErrorEncryptTimes >= 2L && server.ServerSpeedLog().ErrorContinurousTimes >= 5L && this.cfg.autoSwitchOff)
					{
						server.setEnable(false);
					}
				}
				return 16;
			}
			if (e is ProtocolException)
			{
				ProtocolException arg_77_0 = (ProtocolException)e;
				if (this.lastErrCode == 0 && server != null)
				{
					this.lastErrCode = 16;
					server.ServerSpeedLog().AddErrorDecodeTimes();
					if (server.ServerSpeedLog().ErrorEncryptTimes >= 2L && server.ServerSpeedLog().ErrorContinurousTimes >= 5L && this.cfg.autoSwitchOff)
					{
						server.setEnable(false);
					}
				}
				return 16;
			}
			if (e is SocketException)
			{
				SocketException ex = (SocketException)e;
				if (ex.SocketErrorCode != SocketError.ConnectionAborted)
				{
					if (ex.ErrorCode == 11004)
					{
						if (this.lastErrCode == 0 && server != null)
						{
							this.lastErrCode = 1;
							server.ServerSpeedLog().AddErrorTimes();
							if (server.ServerSpeedLog().ErrorConnectTimes >= 3L && server.ServerSpeedLog().ErrorContinurousTimes >= 5L && this.cfg.autoSwitchOff)
							{
								server.setEnable(false);
							}
						}
						return 1;
					}
					if (ex.SocketErrorCode == SocketError.HostNotFound)
					{
						if (this.lastErrCode == 0 && server != null)
						{
							this.lastErrCode = 2;
							server.ServerSpeedLog().AddErrorTimes();
							if (server.ServerSpeedLog().ErrorConnectTimes >= 3L && this.cfg.autoSwitchOff)
							{
								server.setEnable(false);
							}
						}
						return 2;
					}
					if (ex.SocketErrorCode == SocketError.ConnectionRefused)
					{
						if (this.lastErrCode == 0 && server != null)
						{
							this.lastErrCode = 1;
							server.ServerSpeedLog().AddErrorTimes();
							if (server.ServerSpeedLog().ErrorConnectTimes >= 3L && server.ServerSpeedLog().ErrorContinurousTimes >= 5L && this.cfg.autoSwitchOff)
							{
								server.setEnable(false);
							}
						}
						return 2;
					}
					if (ex.SocketErrorCode == SocketError.NetworkUnreachable)
					{
						if (this.lastErrCode == 0 && server != null)
						{
							this.lastErrCode = 3;
							server.ServerSpeedLog().AddErrorTimes();
							if (server.ServerSpeedLog().ErrorConnectTimes >= 3L && server.ServerSpeedLog().ErrorContinurousTimes >= 5L && this.cfg.autoSwitchOff)
							{
								server.setEnable(false);
							}
						}
						return 3;
					}
					if (ex.SocketErrorCode == SocketError.TimedOut)
					{
						if (this.lastErrCode == 0 && server != null)
						{
							this.lastErrCode = 8;
							server.ServerSpeedLog().AddTimeoutTimes();
						}
						return 8;
					}
					if (this.lastErrCode == 0)
					{
						this.lastErrCode = -1;
						if (server != null)
						{
							server.ServerSpeedLog().AddNoErrorTimes();
						}
					}
					return 0;
				}
			}
			return 0;
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x0001C170 File Offset: 0x0001A370
		private void PipeConnectionReceiveCallback(IAsyncResult ar)
		{
			Interlocked.Increment(ref this.connectionRecvCount);
			bool flag = false;
			try
			{
				if (!this.closed)
				{
					int num = this.endConnectionTCPRecv(ar);
					if (num > 0)
					{
						if (this.connectionUDP != null)
						{
							this.doConnectionTCPRecv();
							this.ResetTimeout(this.cfg.TTL);
						}
						else
						{
							if (this.State == Handler.ConnectState.CONNECTED)
							{
								if (this.remoteHeaderSendBuffer != null)
								{
									Array.Copy(this.connetionRecvBuffer, 0, this.connetionRecvBuffer, this.remoteHeaderSendBuffer.Length, num);
									Array.Copy(this.remoteHeaderSendBuffer, 0, this.connetionRecvBuffer, 0, this.remoteHeaderSendBuffer.Length);
									num += this.remoteHeaderSendBuffer.Length;
									this.remoteHeaderSendBuffer = null;
								}
								else
								{
									Logging.LogBin(LogLevel.Debug, "remote send", this.connetionRecvBuffer, num);
								}
							}
							if (this.speedTester.sizeRecv > 0L)
							{
								this.connectionSendBufferList = null;
							}
							else if (this.connectionSendBufferList != null)
							{
								this.detector.OnSend(this.connetionRecvBuffer, num);
								byte[] array = new byte[num];
								Array.Copy(this.connetionRecvBuffer, array, array.Length);
								this.connectionSendBufferList.Add(array);
							}
							if (!this.closed && this.State == Handler.ConnectState.CONNECTED)
							{
								this.ResetTimeout(this.cfg.TTL);
								this.RemoteSend(this.connetionRecvBuffer, num);
							}
						}
					}
					else
					{
						flag = true;
					}
				}
			}
			catch (Exception e)
			{
				this.LogException(e);
				flag = true;
			}
			finally
			{
				Interlocked.Decrement(ref this.connectionRecvCount);
				if (flag)
				{
					this.Close();
				}
			}
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x0001C598 File Offset: 0x0001A798
		private void PipeConnectionSendCallback(IAsyncResult ar)
		{
			if (this.closed)
			{
				return;
			}
			try
			{
				this.connection.EndSend(ar);
				this.doRemoteTCPRecv();
				this.doRemoteUDPRecv();
			}
			catch (Exception e)
			{
				this.LogExceptionAndClose(e);
			}
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x0001C31C File Offset: 0x0001A51C
		private void PipeConnectionUDPReceiveCallback(IAsyncResult ar)
		{
			bool flag = false;
			Interlocked.Increment(ref this.connectionRecvCount);
			try
			{
				if (!this.closed)
				{
					EndPoint endPoint = new IPEndPoint((this.connectionUDP.AddressFamily == AddressFamily.InterNetworkV6) ? IPAddress.IPv6Any : IPAddress.Any, 0);
					int num = this.endConnectionUDPRecv(ar, ref endPoint);
					if (num > 0)
					{
						byte[] array = new byte[num];
						Array.Copy(this.connetionRecvBuffer, array, num);
						Logging.LogBin(LogLevel.Debug, "udp remote send", this.connetionRecvBuffer, num);
						if (!this.server.udp_over_tcp && this.remoteUDP != null)
						{
							this.RemoteSendto(array, num);
						}
						else if (array[0] == 0 && array[1] == 0)
						{
							if (num >= 65280)
							{
								byte[] array2 = new byte[num + 1];
								Array.Copy(this.connetionRecvBuffer, 0, array2, 1, num);
								array2[0] = 255;
								array2[1] = (byte)(num - 65280 + 1 >> 8);
								array2[2] = (byte)(num - 65280 + 1);
								this.RemoteSend(array2, num + 1);
							}
							else
							{
								array[0] = (byte)(num >> 8);
								array[1] = (byte)num;
								this.RemoteSend(array, num);
							}
						}
						this.ResetTimeout(this.cfg.TTL);
					}
					else
					{
						flag = true;
					}
				}
			}
			catch (Exception e)
			{
				this.LogException(e);
				flag = true;
			}
			finally
			{
				Interlocked.Decrement(ref this.connectionRecvCount);
				if (flag)
				{
					this.Close();
				}
			}
		}

		// Token: 0x060002EA RID: 746 RVA: 0x0001C5E4 File Offset: 0x0001A7E4
		private void PipeConnectionUDPSendCallback(IAsyncResult ar)
		{
			if (this.closed)
			{
				return;
			}
			try
			{
				this.connectionUDP.EndSendTo(ar);
				this.doRemoteTCPRecv();
				this.doRemoteUDPRecv();
			}
			catch (Exception e)
			{
				this.LogExceptionAndClose(e);
			}
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x0001BB3C File Offset: 0x00019D3C
		private void PipeRemoteReceiveCallback(IAsyncResult ar)
		{
			Interlocked.Increment(ref this.remoteRecvCount);
			bool flag = false;
			try
			{
				if (!this.closed)
				{
					int num = this.endRemoteTCPRecv(ar);
					if (this.remote.IsClose)
					{
						flag = true;
					}
					else
					{
						int asyncResultSize = this.remote.GetAsyncResultSize(ar);
						this.server.ServerSpeedLog().AddDownloadBytes((long)asyncResultSize);
						this.speedTester.AddDownloadSize(asyncResultSize);
						this.ResetTimeout(this.cfg.TTL);
						if (num <= 0)
						{
							this.doRemoteTCPRecv();
						}
						else
						{
							int num2 = num;
							if (this.speedTester.BeginDownload())
							{
								DateTime arg_9F_0 = this.speedTester.timeBeginDownload;
								DateTime arg_AB_0 = this.speedTester.timeBeginUpload;
								int num3 = (int)(this.speedTester.timeBeginDownload - this.speedTester.timeBeginUpload).TotalMilliseconds;
								if (num3 >= 0)
								{
									this.server.ServerSpeedLog().AddConnectTime(num3);
								}
							}
							this.server.ServerSpeedLog().AddDownloadRawBytes((long)num);
							this.speedTester.AddRecvSize(num);
							Array.Copy(this.remoteRecvBuffer, this.remoteSendBuffer, num);
							if (this.connectionUDP == null)
							{
								if (this.detector.OnRecv(this.remoteSendBuffer, num2) > 0)
								{
									this.server.ServerSpeedLog().AddErrorTimes();
								}
								if (this.detector.Pass)
								{
									this.server.ServerSpeedLog().ResetErrorDecodeTimes();
								}
								else
								{
									this.server.ServerSpeedLog().ResetEmptyTimes();
								}
								this.connection.BeginSend(this.remoteSendBuffer, 0, num2, SocketFlags.None, new AsyncCallback(this.PipeConnectionSendCallback), null);
							}
							else
							{
								List<byte[]> list = new List<byte[]>();
								object obj = this.recvUDPoverTCPLock;
								lock (obj)
								{
									if (num2 + this.remoteUDPRecvBufferLength > this.remoteUDPRecvBuffer.Length)
									{
										Array.Resize<byte>(ref this.remoteUDPRecvBuffer, num2 + this.remoteUDPRecvBufferLength);
									}
									Array.Copy(this.remoteSendBuffer, 0, this.remoteUDPRecvBuffer, this.remoteUDPRecvBufferLength, num2);
									this.remoteUDPRecvBufferLength += num2;
									while (this.remoteUDPRecvBufferLength > 6)
									{
										int num4 = ((int)this.remoteUDPRecvBuffer[0] << 8) + (int)this.remoteUDPRecvBuffer[1];
										if (num4 >= 65280)
										{
											num4 = ((int)this.remoteUDPRecvBuffer[1] << 8) + (int)this.remoteUDPRecvBuffer[2] + 65280;
										}
										if (num4 > this.remoteUDPRecvBufferLength)
										{
											break;
										}
										if (num4 >= 65280)
										{
											byte[] array = new byte[num4 - 1];
											Array.Copy(this.remoteUDPRecvBuffer, 1, array, 0, num4 - 1);
											this.remoteUDPRecvBufferLength -= num4;
											Array.Copy(this.remoteUDPRecvBuffer, num4, this.remoteUDPRecvBuffer, 0, this.remoteUDPRecvBufferLength);
											array[0] = 0;
											array[1] = 0;
											list.Add(array);
										}
										else
										{
											byte[] array2 = new byte[num4];
											Array.Copy(this.remoteUDPRecvBuffer, array2, num4);
											this.remoteUDPRecvBufferLength -= num4;
											Array.Copy(this.remoteUDPRecvBuffer, num4, this.remoteUDPRecvBuffer, 0, this.remoteUDPRecvBufferLength);
											array2[0] = 0;
											array2[1] = 0;
											list.Add(array2);
										}
									}
								}
								if (list.Count == 0)
								{
									this.doRemoteTCPRecv();
								}
								else
								{
									foreach (byte[] current in list)
									{
										this.connectionUDP.BeginSendTo(current, 0, current.Length, SocketFlags.None, this.connectionUDPEndPoint, new AsyncCallback(this.PipeConnectionUDPSendCallback), null);
									}
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				this.LogException(e);
				flag = true;
			}
			finally
			{
				Interlocked.Decrement(ref this.remoteRecvCount);
				if (flag)
				{
					this.Close();
				}
			}
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x0001C4A0 File Offset: 0x0001A6A0
		private void PipeRemoteSendCallback(IAsyncResult ar)
		{
			if (this.closed)
			{
				return;
			}
			try
			{
				this.remote.EndSend(ar);
				this.doConnectionTCPRecv();
				this.doConnectionUDPRecv();
				DateTime arg_29_0 = this.lastKeepTime;
				if ((DateTime.Now - this.lastKeepTime).TotalSeconds > 5.0)
				{
					if (this.keepCurrentServer != null)
					{
						this.keepCurrentServer(this.cfg.targetHost, this.server.id);
					}
					this.lastKeepTime = DateTime.Now;
				}
			}
			catch (Exception e)
			{
				this.LogExceptionAndClose(e);
			}
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x0001BF5C File Offset: 0x0001A15C
		private void PipeRemoteUDPReceiveCallback(IAsyncResult ar)
		{
			bool flag = false;
			Interlocked.Decrement(ref this.remoteRecvCount);
			try
			{
				if (!this.closed)
				{
					EndPoint endPoint = new IPEndPoint((this.remoteUDP.AddressFamily == AddressFamily.InterNetworkV6) ? IPAddress.IPv6Any : IPAddress.Any, 0);
					int num = this.endRemoteUDPRecv(ar, ref endPoint);
					if (this.remote.IsClose)
					{
						flag = true;
					}
					else
					{
						int asyncResultSize = this.remoteUDP.GetAsyncResultSize(ar);
						this.server.ServerSpeedLog().AddDownloadBytes((long)asyncResultSize);
						this.speedTester.AddDownloadSize(asyncResultSize);
						this.ResetTimeout(this.cfg.TTL);
						if (num <= 0)
						{
							this.doRemoteUDPRecv();
						}
						else
						{
							if (this.connectionUDP == null)
							{
								this.connection.BeginSend(this.remoteSendBuffer, 0, num, SocketFlags.None, new AsyncCallback(this.PipeConnectionSendCallback), null);
							}
							else
							{
								this.connectionUDP.BeginSendTo(this.remoteSendBuffer, 0, num, SocketFlags.None, this.connectionUDPEndPoint, new AsyncCallback(this.PipeConnectionUDPSendCallback), null);
							}
							this.speedTester.AddRecvSize(num);
							this.server.ServerSpeedLog().AddDownloadRawBytes((long)num);
						}
					}
				}
			}
			catch (Exception e)
			{
				this.LogException(e);
				flag = true;
			}
			finally
			{
				Interlocked.Decrement(ref this.remoteRecvCount);
				if (flag)
				{
					this.Close();
				}
			}
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x0001C54C File Offset: 0x0001A74C
		private void PipeRemoteUDPSendCallback(IAsyncResult ar)
		{
			if (this.closed)
			{
				return;
			}
			try
			{
				this.remoteUDP.EndSendTo(ar);
				this.doConnectionTCPRecv();
				this.doConnectionUDPRecv();
			}
			catch (Exception e)
			{
				this.LogExceptionAndClose(e);
			}
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x0001B094 File Offset: 0x00019294
		private IPAddress QueryDns(string host, string dns_servers)
		{
			IPAddress result;
			if (!IPAddress.TryParse(host, out result) && this.server.DnsBuffer().isExpired(host))
			{
				if (dns_servers != null)
				{
					Types[] array = new Types[]
					{
						Types.A,
						Types.AAAA
					};
					string[] array2 = dns_servers.Split(new char[]
					{
						','
					});
					for (int i = 0; i < array.Length; i++)
					{
						this.dns = new DnsQuery(host, array[i]);
						this.dns.RecursionDesired = true;
						string[] array3 = array2;
						for (int j = 0; j < array3.Length; j++)
						{
							string value = array3[j];
							this.dns.Servers.Add(value);
						}
						if (this.dns.Send())
						{
							int count = this.dns.Response.Answers.Count;
							if (count > 0)
							{
								for (int k = 0; k < count; k++)
								{
									if (((ResourceRecord)this.dns.Response.Answers[k]).Type == array[i])
									{
										return ((Address)this.dns.Response.Answers[k]).IP;
									}
								}
							}
						}
					}
				}
				try
				{
					Handler.GetHostEntryHandler getHostEntryHandler = new Handler.GetHostEntryHandler(Dns.GetHostEntry);
					IAsyncResult asyncResult = getHostEntryHandler.BeginInvoke(host, null, null);
					if (asyncResult.AsyncWaitHandle.WaitOne(5, false))
					{
						IPAddress[] addressList = getHostEntryHandler.EndInvoke(asyncResult).AddressList;
						int j = 0;
						if (j < addressList.Length)
						{
							return addressList[j];
						}
					}
				}
				catch
				{
				}
				return result;
			}
			return result;
		}

		// Token: 0x060002CA RID: 714 RVA: 0x0001A7E0 File Offset: 0x000189E0
		public bool ReConnect()
		{
			Handler handler = new Handler();
			handler.getCurrentServer = this.getCurrentServer;
			handler.keepCurrentServer = this.keepCurrentServer;
			handler.select_server = this.select_server;
			handler.connection = this.connection;
			handler.connectionUDP = this.connectionUDP;
			handler.cfg = this.cfg;
			handler.cfg.reconnectTimesRemain = this.cfg.reconnectTimesRemain - 1;
			handler.cfg.reconnectTimes = this.cfg.reconnectTimes + 1;
			handler.speedTester.transfer = this.speedTester.transfer;
			int num = 0;
			byte[] array = this.remoteHeaderSendBuffer;
			if (this.connectionSendBufferList != null && this.connectionSendBufferList.Count > 0)
			{
				foreach (byte[] current in this.connectionSendBufferList)
				{
					num += current.Length;
				}
				array = new byte[num];
				num = 0;
				foreach (byte[] current2 in this.connectionSendBufferList)
				{
					Buffer.BlockCopy(current2, 0, array, num, current2.Length);
					num += current2.Length;
				}
			}
			Handler arg_143_0 = handler;
			byte[] expr_140 = array;
			arg_143_0.Start(expr_140, expr_140.Length);
			return true;
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x0001C0D8 File Offset: 0x0001A2D8
		private void RemoteSend(byte[] bytes, int length)
		{
			int num = this.remote.BeginSend(bytes, length, SocketFlags.None, new AsyncCallback(this.PipeRemoteSendCallback), null);
			this.server.ServerSpeedLog().AddUploadBytes((long)num);
			this.speedTester.AddUploadSize(num);
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x0001C120 File Offset: 0x0001A320
		private void RemoteSendto(byte[] bytes, int length)
		{
			int num = this.remoteUDP.BeginSendTo(bytes, length, SocketFlags.None, this.remoteUDPEndPoint, new AsyncCallback(this.PipeRemoteUDPSendCallback), null);
			this.server.ServerSpeedLog().AddUploadBytes((long)num);
			this.speedTester.AddUploadSize(num);
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x0001A314 File Offset: 0x00018514
		private void ResetTimeout(double time)
		{
			if (time <= 0.0 && this.timer == null)
			{
				return;
			}
			this.cfg.try_keep_alive = 0;
			object obj = this.timerLock;
			lock (obj)
			{
				if (time <= 0.0)
				{
					if (this.timer != null)
					{
						this.timer.Enabled = false;
						this.timer.Elapsed -= new ElapsedEventHandler(this.timer_Elapsed);
						this.timer.Dispose();
						this.timer = null;
					}
				}
				else
				{
					if (this.timer == null)
					{
						this.timer = new System.Timers.Timer(time * 1000.0);
						this.timer.Elapsed += new ElapsedEventHandler(this.timer_Elapsed);
					}
					else
					{
						this.timer.Interval = time * 1000.0;
						this.timer.Stop();
					}
					this.timer.Start();
				}
			}
		}

		// Token: 0x060002DD RID: 733 RVA: 0x0001B768 File Offset: 0x00019968
		private void SetObfsPlugin()
		{
			int headSize;
			if (this.connectionSendBufferList != null && this.connectionSendBufferList.Count > 0)
			{
				headSize = ObfsBase.GetHeadSize(this.connectionSendBufferList[0], 30);
			}
			else
			{
				headSize = ObfsBase.GetHeadSize(this.remoteHeaderSendBuffer, 30);
			}
			ProxySocket expr_43 = this.remote;
			if (expr_43 != null)
			{
				expr_43.SetObfsPlugin(this.server, headSize);
			}
			ProxySocket expr_5B = this.remoteUDP;
			if (expr_5B == null)
			{
				return;
			}
			expr_5B.SetObfsPlugin(this.server, headSize);
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x0001A520 File Offset: 0x00018720
		public void setServerTransferTotal(ServerTransferTotal transfer)
		{
			this.speedTester.transfer = transfer;
		}

		// Token: 0x060002CB RID: 715 RVA: 0x0001A954 File Offset: 0x00018B54
		public void Start(byte[] firstPacket, int length)
		{
			if (this.cfg.socks5RemotePort > 0)
			{
				this.cfg.autoSwitchOff = false;
			}
			this.ResetTimeout(this.cfg.TTL);
			if (this.State == Handler.ConnectState.READY)
			{
				this.State = Handler.ConnectState.HANDSHAKE;
				this.remoteHeaderSendBuffer = firstPacket;
				this.Connect();
			}
		}

		// Token: 0x060002DF RID: 735 RVA: 0x0001B8B8 File Offset: 0x00019AB8
		private void StartPipe()
		{
			try
			{
				this.connectionTCPIdle = true;
				this.connectionUDPIdle = true;
				this.remoteTCPIdle = true;
				this.remoteUDPIdle = true;
				this.closed = false;
				this.remoteUDPRecvBufferLength = 0;
				this.SetObfsPlugin();
				this.ResetTimeout(this.cfg.TTL);
				this.speedTester.BeginUpload();
				if (this.connectionUDP == null)
				{
					if (this.connectionSendBufferList != null && this.connectionSendBufferList.Count > 0)
					{
						using (List<byte[]>.Enumerator enumerator = this.connectionSendBufferList.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								byte[] current = enumerator.Current;
								byte[] expr_85 = current;
								this.RemoteSend(expr_85, expr_85.Length);
							}
							goto IL_10B;
						}
					}
					if (this.remoteHeaderSendBuffer != null)
					{
						this.detector.OnSend(this.remoteHeaderSendBuffer, this.remoteHeaderSendBuffer.Length);
						byte[] array = new byte[this.remoteHeaderSendBuffer.Length];
						Array.Copy(this.remoteHeaderSendBuffer, array, array.Length);
						this.connectionSendBufferList.Add(array);
						this.RemoteSend(this.remoteHeaderSendBuffer, this.remoteHeaderSendBuffer.Length);
						this.remoteHeaderSendBuffer = null;
					}
					IL_10B:
					this.remote.GetSocket().SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, false);
				}
				else
				{
					this.connetionRecvBuffer = new byte[131072];
					this.connetionSendBuffer = new byte[133120];
					this.remoteRecvBuffer = new byte[131072];
					this.remoteSendBuffer = new byte[133120];
					if (!this.server.udp_over_tcp && this.remoteUDP != null)
					{
						if (this.cfg.socks5RemotePort == 0)
						{
							this.CloseSocket(ref this.remote);
						}
						this.remoteHeaderSendBuffer = null;
					}
					else if (this.remoteHeaderSendBuffer != null)
					{
						this.RemoteSend(this.remoteHeaderSendBuffer, this.remoteHeaderSendBuffer.Length);
						this.remoteHeaderSendBuffer = null;
					}
				}
				this.State = Handler.ConnectState.CONNECTED;
				this.doRemoteTCPRecv();
				this.doRemoteUDPRecv();
				this.doConnectionTCPRecv();
				this.doConnectionUDPRecv();
			}
			catch (Exception e)
			{
				this.LogExceptionAndClose(e);
			}
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x0001A420 File Offset: 0x00018620
		private void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (this.closed)
			{
				return;
			}
			bool flag = false;
			try
			{
				if (this.cfg.try_keep_alive <= 0 && this.State == Handler.ConnectState.CONNECTED && this.remote != null && this.remote.CanSendKeepAlive)
				{
					this.cfg.try_keep_alive++;
					this.RemoteSend(null, -1);
				}
				else if (this.connection != null)
				{
					if (this.remote != null && this.cfg.reconnectTimesRemain > 0 && this.connectionSendBufferList != null && (this.State == Handler.ConnectState.CONNECTED || this.State == Handler.ConnectState.CONNECTING))
					{
						this.remote.Shutdown(SocketShutdown.Both);
					}
					else
					{
						Server server = this.server;
						if (server != null && this.connectionSendBufferList != null && this.lastErrCode == 0)
						{
							this.lastErrCode = 8;
							server.ServerSpeedLog().AddTimeoutTimes();
						}
						this.connection.Shutdown(SocketShutdown.Both);
					}
				}
			}
			catch (Exception)
			{
			}
			if (flag)
			{
				this.Close();
			}
		}

		// Token: 0x060002CD RID: 717 RVA: 0x0001ACB0 File Offset: 0x00018EB0
		public bool TryReconnect()
		{
			if (this.cfg.reconnectTimesRemain > 0)
			{
				if (this.State == Handler.ConnectState.CONNECTING)
				{
					return this.ReConnect();
				}
				Handler.ConnectState arg_26_0 = this.State;
			}
			return false;
		}

		// Token: 0x17000021 RID: 33
		private Handler.ConnectState State
		{
			// Token: 0x060002C4 RID: 708 RVA: 0x0001A2CE File Offset: 0x000184CE
			get
			{
				return this.state;
			}
			// Token: 0x060002C5 RID: 709 RVA: 0x0001A2D8 File Offset: 0x000184D8
			set
			{
				lock (this)
				{
					this.state = value;
				}
			}
		}

		// Token: 0x04000232 RID: 562
		protected const int AutoSwitchOffErrorTimes = 5;

		// Token: 0x04000231 RID: 561
		protected const int BufferSize = 17408;

		// Token: 0x04000227 RID: 551
		public HandlerConfig cfg = new HandlerConfig();

		// Token: 0x0400023C RID: 572
		protected bool closed;

		// Token: 0x04000228 RID: 552
		public Socket connection;

		// Token: 0x04000243 RID: 579
		protected int connectionRecvCount;

		// Token: 0x04000238 RID: 568
		protected List<byte[]> connectionSendBufferList = new List<byte[]>();

		// Token: 0x0400023E RID: 574
		protected bool connectionTCPIdle;

		// Token: 0x04000229 RID: 553
		public Socket connectionUDP;

		// Token: 0x0400022A RID: 554
		protected IPEndPoint connectionUDPEndPoint;

		// Token: 0x0400023F RID: 575
		protected bool connectionUDPIdle;

		// Token: 0x04000236 RID: 566
		protected byte[] connetionRecvBuffer = new byte[32768];

		// Token: 0x04000237 RID: 567
		protected byte[] connetionSendBuffer = new byte[34816];

		// Token: 0x0400022B RID: 555
		protected ProtocolResponseDetector detector = new ProtocolResponseDetector();

		// Token: 0x0400022F RID: 559
		protected DnsQuery dns;

		// Token: 0x04000223 RID: 547
		public Handler.GetCurrentServer getCurrentServer;

		// Token: 0x04000224 RID: 548
		public Handler.KeepCurrentServer keepCurrentServer;

		// Token: 0x04000245 RID: 581
		protected int lastErrCode;

		// Token: 0x04000239 RID: 569
		protected DateTime lastKeepTime;

		// Token: 0x04000246 RID: 582
		protected Random random = new Random();

		// Token: 0x04000230 RID: 560
		protected const int RecvSize = 16384;

		// Token: 0x0400023D RID: 573
		protected object recvUDPoverTCPLock = new object();

		// Token: 0x0400022C RID: 556
		protected ProxySocket remote;

		// Token: 0x04000235 RID: 565
		protected byte[] remoteHeaderSendBuffer;

		// Token: 0x04000233 RID: 563
		protected byte[] remoteRecvBuffer = new byte[32768];

		// Token: 0x04000242 RID: 578
		protected int remoteRecvCount;

		// Token: 0x04000234 RID: 564
		protected byte[] remoteSendBuffer = new byte[34816];

		// Token: 0x04000240 RID: 576
		protected bool remoteTCPIdle;

		// Token: 0x0400022D RID: 557
		protected ProxySocket remoteUDP;

		// Token: 0x0400022E RID: 558
		protected IPEndPoint remoteUDPEndPoint;

		// Token: 0x04000241 RID: 577
		protected bool remoteUDPIdle;

		// Token: 0x0400023A RID: 570
		protected byte[] remoteUDPRecvBuffer = new byte[32768];

		// Token: 0x0400023B RID: 571
		protected int remoteUDPRecvBufferLength;

		// Token: 0x04000226 RID: 550
		public Server select_server;

		// Token: 0x04000225 RID: 549
		public Server server;

		// Token: 0x04000244 RID: 580
		protected SpeedTester speedTester = new SpeedTester();

		// Token: 0x04000249 RID: 585
		private Handler.ConnectState state;

		// Token: 0x04000247 RID: 583
		protected System.Timers.Timer timer;

		// Token: 0x04000248 RID: 584
		protected object timerLock = new object();

		// Token: 0x020000B3 RID: 179
		private enum ConnectState
		{
			// Token: 0x04000461 RID: 1121
			END = -1,
			// Token: 0x04000462 RID: 1122
			READY,
			// Token: 0x04000463 RID: 1123
			HANDSHAKE,
			// Token: 0x04000464 RID: 1124
			CONNECTING,
			// Token: 0x04000465 RID: 1125
			CONNECTED
		}

		// Token: 0x020000B1 RID: 177
		// Token: 0x06000587 RID: 1415
		public delegate Server GetCurrentServer(string targetURI = null, bool usingRandom = false, bool forceRandom = false);

		// Token: 0x020000B0 RID: 176
		// Token: 0x06000583 RID: 1411
		private delegate IPHostEntry GetHostEntryHandler(string ip);

		// Token: 0x020000B2 RID: 178
		// Token: 0x0600058B RID: 1419
		public delegate void KeepCurrentServer(string targetURI, string id);
	}
}
