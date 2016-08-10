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
using Shadowsocks.Util;

namespace Shadowsocks.Controller
{
	// Token: 0x0200004F RID: 79
	internal class Handler
	{
		// Token: 0x060002D9 RID: 729 RVA: 0x0001C771 File Offset: 0x0001A971
		private void AddRemoteUDPRecvBufferHeader(byte[] decryptBuffer, ref int bytesToSend)
		{
			Array.Copy(decryptBuffer, 0, this.remoteSendBuffer, 3, bytesToSend);
			this.remoteSendBuffer[0] = 0;
			this.remoteSendBuffer[1] = 0;
			this.remoteSendBuffer[2] = 0;
			bytesToSend += 3;
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x000197B0 File Offset: 0x000179B0
		private void BeginConnect(IPAddress ipAddress, int serverPort)
		{
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, serverPort);
			this.remoteTCPEndPoint = remoteEP;
			if (this.server.server_udp_port == 0)
			{
				new IPEndPoint(ipAddress, serverPort);
				this.remoteUDPEndPoint = remoteEP;
			}
			else
			{
				IPEndPoint iPEndPoint = new IPEndPoint(ipAddress, this.server.server_udp_port);
				this.remoteUDPEndPoint = iPEndPoint;
			}
			if (this.socks5RemotePort != 0 || this.connectionUDP == null || (this.connectionUDP != null && this.server.udp_over_tcp))
			{
				this.remote = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				this.remote.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, true);
				int num = 4;
				uint arg_AC_0 = 1u;
				uint value = 60000u;
				uint value2 = 5000u;
				byte[] array = new byte[num * 3];
				Array.Copy(BitConverter.GetBytes(arg_AC_0), 0, array, 0, num);
				Array arg_C6_0 = BitConverter.GetBytes(value);
				int arg_C6_1 = 0;
				Array arg_C6_2 = array;
				int expr_C5 = num;
				Array.Copy(arg_C6_0, arg_C6_1, arg_C6_2, expr_C5, expr_C5);
				Array.Copy(BitConverter.GetBytes(value2), 0, array, num * 2, num);
				this.remote.IOControl(unchecked ((IOControlCode)( unchecked ((ulong)-1744830460))), array, null);
			}
			if (this.connectionUDP != null && !this.server.udp_over_tcp)
			{
				try
				{
					this.remoteUDP = new Socket(ipAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
					this.remoteUDP.Bind(new IPEndPoint((ipAddress.AddressFamily == AddressFamily.InterNetworkV6) ? IPAddress.IPv6Any : IPAddress.Any, 0));
				}
				catch (SocketException)
				{
					this.remoteUDP = null;
				}
			}
			this.ResetTimeout(this.TTL);
			if (this.socks5RemotePort != 0 || this.connectionUDP == null || this.server.udp_over_tcp)
			{
				this.speedTester.BeginConnect();
				IAsyncResult asyncResult = this.remote.BeginConnect(remoteEP, new AsyncCallback(this.ConnectCallback), new CallbackStatus());
				double num2 = this.TTL;
				if (num2 <= 0.0)
				{
					num2 = 40.0;
				}
				else if (num2 <= 10.0)
				{
					num2 = 10.0;
				}
				if (this.reconnectTimesRemain + this.reconnectTimes > 0 || this.TTL > 0.0)
				{
					if (!asyncResult.AsyncWaitHandle.WaitOne((int)(num2 * 250.0), true))
					{
						((CallbackStatus)asyncResult.AsyncState).SetIfEqu(-1, 0);
						if (((CallbackStatus)asyncResult.AsyncState).Status == -1)
						{
							this.lastErrCode = 8;
							this.server.ServerSpeedLog().AddTimeoutTimes();
							this.CloseSocket(ref this.remote);
							this.Close();
							return;
						}
					}
					else if (((CallbackStatus)asyncResult.AsyncState).Status == -1)
					{
						this.lastErrCode = 1;
						this.server.ServerSpeedLog().AddErrorTimes();
						this.CloseSocket(ref this.remote);
						this.Close();
					}
				}
				return;
			}
			Handler.ConnectState connectState = this.State;
			if (connectState == Handler.ConnectState.CONNECTING)
			{
				this.StartPipe();
				return;
			}
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x00019C38 File Offset: 0x00017E38
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
			int num2 = 0;
			while (num2 < 10 && ((this.arTCPConnection != null && this.arTCPConnection.IsCompleted) || (this.arTCPRemote != null && this.arTCPRemote.IsCompleted) || (this.arUDPConnection != null && this.arUDPConnection.IsCompleted) || (this.arUDPRemote != null && this.arUDPRemote.IsCompleted)))
			{
				Thread.Sleep(10 * (num2 + 1) * (num2 + 1));
				num2++;
			}
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
			this.keepCurrentServer(this.targetHost);
			this.ResetTimeout(0.0);
			try
			{
				bool arg_1BB_0 = this.TryReconnect();
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
				this.authConnection = null;
				if (!arg_1BB_0)
				{
					this.CloseSocket(ref this.connection);
					this.CloseSocket(ref this.connectionUDP);
				}
				else
				{
					this.connection = null;
					this.connectionUDP = null;
				}
				if (this.obfs != null)
				{
					this.obfs.Dispose();
					this.obfs = null;
				}
				if (this.protocol != null)
				{
					this.protocol.Dispose();
					this.protocol = null;
				}
				object obj = this.encryptionLock;
				lock (obj)
				{
					object obj2 = this.decryptionLock;
					lock (obj2)
					{
						if (this.encryptor != null)
						{
							this.encryptor.Dispose();
						}
						if (this.encryptorUDP != null)
						{
							this.encryptorUDP.Dispose();
						}
					}
				}
			}
			catch (Exception arg_283_0)
			{
				Logging.LogUsefulException(arg_283_0);
			}
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x00019BCC File Offset: 0x00017DCC
		private void CloseSocket(ref Socket sock)
		{
			lock (this)
			{
				if (sock != null)
				{
					Socket socket = sock;
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

		// Token: 0x060002C7 RID: 711 RVA: 0x0001B360 File Offset: 0x00019560
		private void Connect()
		{
			this.remote = null;
			this.remoteUDP = null;
			this.arTCPConnection = null;
			this.arTCPRemote = null;
			if (this.select_server == null)
			{
				if (this.targetHost == null)
				{
					this.targetHost = this.GetQueryString();
					this.server = this.getCurrentServer(this.targetHost, true, false);
				}
				else
				{
					this.server = this.getCurrentServer(this.targetHost, true, this.forceRandom);
				}
			}
			else
			{
				this.server = this.select_server;
			}
			this.speedTester.server = this.server.server;
			if (this.socks5RemotePort > 0 && this.server.udp_over_tcp)
			{
				this.command = 1;
			}
			this.ResetTimeout(this.TTL);
			if (this.fouce_local_dns_query && this.targetHost != null)
			{
				IPAddress iPAddress = this.QueryDns(this.targetHost, this.dns_servers);
				if (iPAddress != null)
				{
					this.server.DnsBuffer().UpdateDns(this.targetHost, iPAddress);
					this.targetHost = iPAddress.ToString();
					this.ResetTimeout(this.TTL);
				}
			}
			lock (this)
			{
				this.server.ServerSpeedLog().AddConnectTimes();
				if (this.State == Handler.ConnectState.HANDSHAKE)
				{
					this.State = Handler.ConnectState.CONNECTING;
				}
				try
				{
					this.encryptor = EncryptorFactory.GetEncryptor(this.server.method, this.server.password);
					this.encryptorUDP = EncryptorFactory.GetEncryptor(this.server.method, this.server.password);
					this.server.GetConnections().AddRef(this.connection);
				}
				catch
				{
				}
			}
			this.protocol = ObfsFactory.GetObfs(this.server.protocol);
			this.obfs = ObfsFactory.GetObfs(this.server.obfs);
			string text = this.server.server;
			int server_port = this.server.server_port;
			if (this.socks5RemotePort > 0)
			{
				text = this.socks5RemoteHost;
				server_port = this.socks5RemotePort;
			}
			IPAddress iPAddress2;
			if (!IPAddress.TryParse(text, out iPAddress2))
			{
				if (this.server.DnsBuffer().isExpired(text))
				{
					bool flag2 = false;
					if (!flag2)
					{
						iPAddress2 = this.QueryDns(text, this.dns_servers);
						if (iPAddress2 != null)
						{
							this.server.DnsBuffer().UpdateDns(text, iPAddress2);
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
			this.BeginConnect(iPAddress2, server_port);
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x0001B678 File Offset: 0x00019878
		private void ConnectCallback(IAsyncResult ar)
		{
			if (ar != null && ar.AsyncState != null)
			{
				((CallbackStatus)ar.AsyncState).SetIfEqu(1, 0);
			}
			if (ar != null && ar.AsyncState != null && ((CallbackStatus)ar.AsyncState).Status != 1)
			{
				this.lastErrCode = 1;
				this.server.ServerSpeedLog().AddErrorTimes();
				return;
			}
			try
			{
				this.remote.EndConnect(ar);
				if (this.socks5RemotePort > 0)
				{
					this.remoteTCPEndPoint = null;
					if (!this.ConnectProxyServer(this.server.server, this.server.server_port, this.remote, 10054))
					{
						throw new SocketException(10054);
					}
				}
				this.speedTester.EndConnect();
				Handler.ConnectState connectState = this.State;
				if (connectState == Handler.ConnectState.CONNECTING)
				{
					this.StartPipe();
				}
			}
			catch (Exception e)
			{
				this.LogSocketException(e);
				string remarks;
				string serverUrl = this.getServerUrl(out remarks);
				if (!Logging.LogSocketException(remarks, serverUrl, e))
				{
					Logging.LogUsefulException(e);
				}
				this.Close();
			}
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x00019F34 File Offset: 0x00018134
		private bool ConnectHttpProxyServer(string strRemoteHost, int iRemotePort, Socket sProxyServer, int socketErrorCode)
		{
			bool flag = true;
			IPAddress iPAddress;
			if (!IPAddress.TryParse(strRemoteHost, out iPAddress) && !flag)
			{
				if (this.server.DnsTargetBuffer().isExpired(strRemoteHost))
				{
					try
					{
						iPAddress = Dns.GetHostEntry(strRemoteHost).AddressList[0];
						this.server.DnsTargetBuffer().UpdateDns(strRemoteHost, iPAddress);
						goto IL_58;
					}
					catch (Exception)
					{
						goto IL_58;
					}
				}
				iPAddress = this.server.DnsTargetBuffer().ip;
			}
			IL_58:
			if (iPAddress != null)
			{
				strRemoteHost = iPAddress.ToString();
			}
			string text = ((strRemoteHost.IndexOf(':') >= 0) ? ("[" + strRemoteHost + "]") : strRemoteHost) + ":" + iRemotePort.ToString();
			string str = Convert.ToBase64String(Encoding.UTF8.GetBytes(this.socks5RemoteUsername + ":" + this.socks5RemotePassword));
			string text2 = string.Concat(new string[]
			{
				"CONNECT ",
				text,
				" HTTP/1.0\r\nHost: ",
				text,
				"\r\n"
			});
			if (this.proxyUserAgent != null && this.proxyUserAgent.Length > 0)
			{
				text2 = text2 + "User-Agent: " + this.proxyUserAgent + "\r\n";
			}
			text2 += "Proxy-Connection: Keep-Alive\r\n";
			if (this.socks5RemoteUsername.Length > 0)
			{
				text2 = text2 + "Proxy-Authorization: Basic " + str + "\r\n";
			}
			text2 += "\r\n";
			byte[] bytes = Encoding.UTF8.GetBytes(text2);
			byte[] expr_164 = bytes;
			sProxyServer.Send(expr_164, expr_164.Length, SocketFlags.None);
			byte[] array = new byte[1024];
			byte[] expr_17D = array;
			int num = sProxyServer.Receive(expr_17D, expr_17D.Length, SocketFlags.None);
			if (num > 13)
			{
				string[] array2 = Encoding.UTF8.GetString(array, 0, num).Split(new char[]
				{
					' '
				});
				if (array2.Length > 1 && array2[1] == "200")
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x0001C0E4 File Offset: 0x0001A2E4
		private void ConnectionSend(byte[] buffer, int bytesToSend)
		{
			if (this.connectionUDP == null)
			{
				this.connection.BeginSend(buffer, 0, bytesToSend, SocketFlags.None, new AsyncCallback(this.PipeConnectionSendCallback), null);
				return;
			}
			this.connectionUDP.BeginSendTo(buffer, 0, bytesToSend, SocketFlags.None, this.connectionUDPEndPoint, new AsyncCallback(this.PipeConnectionUDPSendCallback), null);
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x0001A55C File Offset: 0x0001875C
		private bool ConnectProxyServer(string strRemoteHost, int iRemotePort, Socket sProxyServer, int socketErrorCode)
		{
			if (this.proxyType == 0)
			{
				return this.ConnectSocks5ProxyServer(strRemoteHost, iRemotePort, sProxyServer, socketErrorCode);
			}
			return this.proxyType != 1 || this.ConnectHttpProxyServer(strRemoteHost, iRemotePort, sProxyServer, socketErrorCode);
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x0001A11C File Offset: 0x0001831C
		private bool ConnectSocks5ProxyServer(string strRemoteHost, int iRemotePort, Socket sProxyServer, int socketErrorCode)
		{
			byte[] array = new byte[10];
			array[0] = 5;
			array[1] = 2;
			array[2] = 0;
			array[3] = 2;
			sProxyServer.Send(array, 4, SocketFlags.None);
			byte[] array2 = new byte[32];
			byte[] expr_2C = array2;
			if (sProxyServer.Receive(expr_2C, expr_2C.Length, SocketFlags.None) < 2)
			{
				throw new SocketException(socketErrorCode);
			}
			if (array2[0] != 5 || (array2[1] != 0 && array2[1] != 2))
			{
				throw new SocketException(socketErrorCode);
			}
			if (array2[1] != 0)
			{
				if (array2[1] != 2)
				{
					return false;
				}
				if (this.socks5RemoteUsername.Length == 0)
				{
					throw new SocketException(socketErrorCode);
				}
				array = new byte[this.socks5RemoteUsername.Length + this.socks5RemotePassword.Length + 3];
				array[0] = 1;
				array[1] = (byte)this.socks5RemoteUsername.Length;
				for (int i = 0; i < this.socks5RemoteUsername.Length; i++)
				{
					array[2 + i] = (byte)this.socks5RemoteUsername[i];
				}
				array[this.socks5RemoteUsername.Length + 2] = (byte)this.socks5RemotePassword.Length;
				for (int j = 0; j < this.socks5RemotePassword.Length; j++)
				{
					array[this.socks5RemoteUsername.Length + 3 + j] = (byte)this.socks5RemotePassword[j];
				}
				byte[] expr_12A = array;
				sProxyServer.Send(expr_12A, expr_12A.Length, SocketFlags.None);
				byte[] expr_136 = array2;
				sProxyServer.Receive(expr_136, expr_136.Length, SocketFlags.None);
				if (array2[0] != 1 || array2[1] != 0)
				{
					throw new SocketException(10061);
				}
			}
			if (this.command == 1)
			{
				List<byte> list = new List<byte>();
				list.Add(5);
				list.Add(1);
				list.Add(0);
				bool flag = false;
				IPAddress iPAddress;
				if (!IPAddress.TryParse(strRemoteHost, out iPAddress) && !flag)
				{
					if (this.server.DnsTargetBuffer().isExpired(strRemoteHost))
					{
						try
						{
							iPAddress = Dns.GetHostEntry(strRemoteHost).AddressList[0];
							this.server.DnsTargetBuffer().UpdateDns(strRemoteHost, iPAddress);
							goto IL_1E0;
						}
						catch (Exception)
						{
							goto IL_1E0;
						}
					}
					iPAddress = this.server.DnsTargetBuffer().ip;
				}
				IL_1E0:
				if (iPAddress == null)
				{
					list.Add(3);
					list.Add((byte)strRemoteHost.Length);
					for (int k = 0; k < strRemoteHost.Length; k++)
					{
						list.Add((byte)strRemoteHost[k]);
					}
				}
				else
				{
					byte[] addressBytes = iPAddress.GetAddressBytes();
					if (addressBytes.GetLength(0) > 4)
					{
						list.Add(4);
						for (int l = 0; l < 16; l++)
						{
							list.Add(addressBytes[l]);
						}
					}
					else
					{
						list.Add(1);
						for (int m = 0; m < 4; m++)
						{
							list.Add(addressBytes[m]);
						}
					}
				}
				list.Add((byte)(iRemotePort / 256));
				list.Add((byte)(iRemotePort % 256));
				sProxyServer.Send(list.ToArray(), list.Count, SocketFlags.None);
				byte[] expr_2B6 = array2;
				if (sProxyServer.Receive(expr_2B6, expr_2B6.Length, SocketFlags.None) < 2 || array2[0] != 5 || array2[1] != 0)
				{
					throw new SocketException(socketErrorCode);
				}
				return true;
			}
			else
			{
				if (this.command != 3)
				{
					return false;
				}
				List<byte> list2 = new List<byte>();
				list2.Add(5);
				list2.Add(3);
				list2.Add(0);
				IPAddress iPAddress2 = this.remoteUDPEndPoint.Address;
				byte[] addressBytes2 = iPAddress2.GetAddressBytes();
				if (addressBytes2.GetLength(0) > 4)
				{
					list2.Add(4);
					for (int n = 0; n < 16; n++)
					{
						list2.Add(addressBytes2[n]);
					}
				}
				else
				{
					list2.Add(1);
					for (int num = 0; num < 4; num++)
					{
						list2.Add(addressBytes2[num]);
					}
				}
				list2.Add(0);
				list2.Add(0);
				sProxyServer.Send(list2.ToArray(), list2.Count, SocketFlags.None);
				byte[] expr_396 = array2;
				sProxyServer.Receive(expr_396, expr_396.Length, SocketFlags.None);
				if (array2[0] != 5 || array2[1] != 0)
				{
					throw new SocketException(socketErrorCode);
				}
				byte[] array3;
				int port;
				if (array2[0] != 4)
				{
					array3 = new byte[4];
					Array.Copy(array2, 4, array3, 0, 4);
					port = (int)array2[8] * 256 + (int)array2[9];
				}
				else
				{
					array3 = new byte[16];
					Array.Copy(array2, 4, array3, 0, 16);
					port = (int)array2[20] * 256 + (int)array2[21];
				}
				iPAddress2 = new IPAddress(array3);
				this.remoteUDPEndPoint = new IPEndPoint(iPAddress2, port);
				return true;
			}
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x00019A94 File Offset: 0x00017C94
		private void DnsCallback(IAsyncResult ar)
		{
			((CallbackStatus)ar.AsyncState).SetIfEqu(1, 0);
			if (((CallbackStatus)ar.AsyncState).Status != 1)
			{
				return;
			}
			try
			{
				IPAddress iPAddress = Dns.EndGetHostEntry(ar).AddressList[0];
				int server_port = this.server.server_port;
				if (this.socks5RemotePort > 0)
				{
					this.server.DnsBuffer().UpdateDns(this.socks5RemoteHost, iPAddress);
					server_port = this.socks5RemotePort;
				}
				else
				{
					this.server.DnsBuffer().UpdateDns(this.server.server, iPAddress);
				}
				this.BeginConnect(iPAddress, server_port);
			}
			catch (Exception e)
			{
				this.LogSocketException(e);
				string remarks;
				string serverUrl = this.getServerUrl(out remarks);
				if (!Logging.LogSocketException(remarks, serverUrl, e))
				{
					Logging.LogUsefulException(e);
				}
				this.Close();
			}
		}

		// Token: 0x060002CB RID: 715 RVA: 0x0001B85C File Offset: 0x00019A5C
		private void doConnectionTCPRecv()
		{
			if (this.connection != null && this.connectionTCPIdle)
			{
				this.connectionTCPIdle = false;
				this.arTCPConnection = this.connection.BeginReceive(this.connetionRecvBuffer, 0, 16384, SocketFlags.None, new AsyncCallback(this.PipeConnectionReceiveCallback), null);
			}
		}

		// Token: 0x060002CD RID: 717 RVA: 0x0001B8CC File Offset: 0x00019ACC
		private void doConnectionUDPRecv()
		{
			if (this.connectionUDP != null && this.connectionUDPIdle)
			{
				EndPoint endPoint = new IPEndPoint((this.connectionUDP.AddressFamily == AddressFamily.InterNetworkV6) ? IPAddress.IPv6Any : IPAddress.Any, 0);
				this.connectionUDPIdle = false;
				this.arUDPConnection = this.connectionUDP.BeginReceiveFrom(this.connetionRecvBuffer, 0, 65536, SocketFlags.None, ref endPoint, new AsyncCallback(this.PipeConnectionUDPReceiveCallback), null);
			}
		}

		// Token: 0x060002CF RID: 719 RVA: 0x0001B974 File Offset: 0x00019B74
		private void doRemoteTCPRecv()
		{
			if (this.remote != null && this.remoteTCPIdle)
			{
				this.remoteTCPIdle = false;
				this.arTCPRemote = this.remote.BeginReceive(this.remoteRecvBuffer, 0, 16384, SocketFlags.None, new AsyncCallback(this.PipeRemoteReceiveCallback), null);
			}
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x0001B9E4 File Offset: 0x00019BE4
		private void doRemoteUDPRecv()
		{
			if (this.remoteUDP != null && this.remoteUDPIdle)
			{
				EndPoint endPoint = new IPEndPoint((this.remoteUDP.AddressFamily == AddressFamily.InterNetworkV6) ? IPAddress.IPv6Any : IPAddress.Any, 0);
				this.remoteUDPIdle = false;
				this.arUDPRemote = this.remoteUDP.BeginReceiveFrom(this.remoteRecvBuffer, 0, 65536, SocketFlags.None, ref endPoint, new AsyncCallback(this.PipeRemoteUDPReceiveCallback), null);
			}
		}

		// Token: 0x060002CC RID: 716 RVA: 0x0001B8AB File Offset: 0x00019AAB
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

		// Token: 0x060002CE RID: 718 RVA: 0x0001B93F File Offset: 0x00019B3F
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

		// Token: 0x060002D0 RID: 720 RVA: 0x0001B9C3 File Offset: 0x00019BC3
		private int endRemoteTCPRecv(IAsyncResult ar)
		{
			if (this.remote != null)
			{
				int arg_1B_0 = this.remote.EndReceive(ar);
				this.remoteTCPIdle = true;
				return arg_1B_0;
			}
			return 0;
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x0001BA57 File Offset: 0x00019C57
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

		// Token: 0x060002D4 RID: 724 RVA: 0x0001BD5C File Offset: 0x00019F5C
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

		// Token: 0x060002AD RID: 685 RVA: 0x00019280 File Offset: 0x00017480
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

		// Token: 0x060002C1 RID: 705 RVA: 0x0001AC28 File Offset: 0x00018E28
		private void HandshakeAuthReceiveCallback(IAsyncResult ar)
		{
			if (this.closed)
			{
				return;
			}
			try
			{
				if (this.connection.EndReceive(ar) >= 3)
				{
					byte b = this.connetionRecvBuffer[1];
					byte count = this.connetionRecvBuffer[(int)(b + 2)];
					byte[] expr_33 = new byte[2];
					expr_33[0] = 1;
					byte[] array = expr_33;
					string @string = Encoding.UTF8.GetString(this.connetionRecvBuffer, 2, (int)b);
					string string2 = Encoding.UTF8.GetString(this.connetionRecvBuffer, (int)(b + 3), (int)count);
					if (this.authConnection(this.connection, @string, string2))
					{
						this.connection.BeginSend(array, 0, array.Length, SocketFlags.None, new AsyncCallback(this.HandshakeSendCallback), null);
					}
				}
				else
				{
					Console.WriteLine("failed to recv data in HandshakeAuthReceiveCallback");
					this.Close();
				}
			}
			catch (Exception e)
			{
				this.LogSocketException(e);
				string remarks;
				string serverUrl = this.getServerUrl(out remarks);
				if (!Logging.LogSocketException(remarks, serverUrl, e))
				{
					Logging.LogUsefulException(e);
				}
				this.Close();
			}
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x0001ABA0 File Offset: 0x00018DA0
		private void HandshakeAuthSendCallback(IAsyncResult ar)
		{
			if (this.closed)
			{
				return;
			}
			try
			{
				this.connection.EndSend(ar);
				this.connection.BeginReceive(this.connetionRecvBuffer, 0, 1024, SocketFlags.None, new AsyncCallback(this.HandshakeAuthReceiveCallback), null);
			}
			catch (Exception e)
			{
				this.LogSocketException(e);
				string remarks;
				string serverUrl = this.getServerUrl(out remarks);
				if (!Logging.LogSocketException(remarks, serverUrl, e))
				{
					Logging.LogUsefulException(e);
				}
				this.Close();
			}
		}

		// Token: 0x060002BF RID: 703 RVA: 0x0001AAD4 File Offset: 0x00018CD4
		private void HandshakeReceive()
		{
			if (this.closed)
			{
				return;
			}
			try
			{
				if (this._firstPacketLength > 1)
				{
					if ((this.authConnection == null || Utils.isMatchSubNet(((IPEndPoint)this.connection.RemoteEndPoint).Address, "127.0.0.0/8")) && this._firstPacket[0] == 4 && this._firstPacketLength >= 9)
					{
						this.RspSocks4aHandshakeReceive();
					}
					else if (this._firstPacket[0] == 5 && this._firstPacketLength >= 2)
					{
						this.RspSocks5HandshakeReceive();
					}
					else
					{
						this.RspHttpHandshakeReceive();
					}
				}
				else
				{
					this.Close();
				}
			}
			catch (Exception e)
			{
				this.LogSocketException(e);
				string remarks;
				string serverUrl = this.getServerUrl(out remarks);
				if (!Logging.LogSocketException(remarks, serverUrl, e))
				{
					Logging.LogUsefulException(e);
				}
				this.Close();
			}
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x0001B058 File Offset: 0x00019258
		private void HandshakeReceive2Callback(IAsyncResult ar)
		{
			if (this.closed)
			{
				return;
			}
			try
			{
				int num = this.connection.EndReceive(ar);
				if (num >= 3)
				{
					this.command = this.connetionRecvBuffer[1];
					if (num > 3)
					{
						this.remoteHeaderSendBuffer = new byte[num - 3];
						Array.Copy(this.connetionRecvBuffer, 3, this.remoteHeaderSendBuffer, 0, this.remoteHeaderSendBuffer.Length);
					}
					else
					{
						this.remoteHeaderSendBuffer = null;
					}
					if (this.command == 3)
					{
						if (num >= 5)
						{
							if (this.remoteHeaderSendBuffer == null)
							{
								this.remoteHeaderSendBuffer = new byte[num - 3];
								Array.Copy(this.connetionRecvBuffer, 3, this.remoteHeaderSendBuffer, 0, this.remoteHeaderSendBuffer.Length);
							}
							else
							{
								Array.Resize<byte>(ref this.remoteHeaderSendBuffer, this.remoteHeaderSendBuffer.Length + num - 3);
								Array.Copy(this.connetionRecvBuffer, 3, this.remoteHeaderSendBuffer, this.remoteHeaderSendBuffer.Length - num, num);
							}
							this.RspSocks5UDPHeader(num);
						}
						else
						{
							Console.WriteLine("failed to recv data in HandshakeReceive2Callback");
							this.Close();
						}
					}
					else
					{
						this.RspSocks5TCPHeader();
					}
				}
				else
				{
					Console.WriteLine("failed to recv data in HandshakeReceive2Callback");
					this.Close();
				}
			}
			catch (Exception e)
			{
				this.LogSocketException(e);
				string remarks;
				string serverUrl = this.getServerUrl(out remarks);
				if (!Logging.LogSocketException(remarks, serverUrl, e))
				{
					Logging.LogUsefulException(e);
				}
				this.Close();
			}
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x0001AD20 File Offset: 0x00018F20
		private void HandshakeSendCallback(IAsyncResult ar)
		{
			if (this.closed)
			{
				return;
			}
			try
			{
				this.connection.EndSend(ar);
				this.connection.BeginReceive(this.connetionRecvBuffer, 0, 1024, SocketFlags.None, new AsyncCallback(this.HandshakeReceive2Callback), null);
			}
			catch (Exception e)
			{
				this.LogSocketException(e);
				string remarks;
				string serverUrl = this.getServerUrl(out remarks);
				if (!Logging.LogSocketException(remarks, serverUrl, e))
				{
					Logging.LogUsefulException(e);
				}
				this.Close();
			}
		}

		// Token: 0x060002BB RID: 699 RVA: 0x0001A740 File Offset: 0x00018940
		private void HttpHandshakeAuthEndSend(IAsyncResult ar)
		{
			if (this.closed)
			{
				return;
			}
			try
			{
				this.connection.EndSend(ar);
				this.connection.BeginReceive(this.connetionRecvBuffer, 0, this._firstPacket.Length, SocketFlags.None, new AsyncCallback(this.HttpHandshakeRecv), null);
			}
			catch (Exception e)
			{
				this.LogSocketException(e);
				string remarks;
				string serverUrl = this.getServerUrl(out remarks);
				if (!Logging.LogSocketException(remarks, serverUrl, e))
				{
					Logging.LogUsefulException(e);
				}
				this.Close();
			}
		}

		// Token: 0x060002BC RID: 700 RVA: 0x0001A7CC File Offset: 0x000189CC
		private void HttpHandshakeRecv(IAsyncResult ar)
		{
			if (this.closed)
			{
				return;
			}
			try
			{
				int num = this.connection.EndReceive(ar);
				if (num > 0)
				{
					Array.Copy(this.connetionRecvBuffer, this._firstPacket, num);
					this._firstPacketLength = num;
					this.RspHttpHandshakeReceive();
				}
				else
				{
					Console.WriteLine("failed to recv data in HttpHandshakeRecv");
					this.Close();
				}
			}
			catch (Exception e)
			{
				this.LogSocketException(e);
				string remarks;
				string serverUrl = this.getServerUrl(out remarks);
				if (!Logging.LogSocketException(remarks, serverUrl, e))
				{
					Logging.LogUsefulException(e);
				}
				this.Close();
			}
		}

		// Token: 0x060002AF RID: 687 RVA: 0x000192C0 File Offset: 0x000174C0
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
					if (server.ServerSpeedLog().ErrorEncryptTimes >= 2L && server.ServerSpeedLog().ErrorContinurousTimes >= 5L && this.autoSwitchOff)
					{
						server.setEnable(false);
					}
				}
				return 16;
			}
			if (e is ProtocolException)
			{
				ProtocolException arg_72_0 = (ProtocolException)e;
				if (this.lastErrCode == 0 && server != null)
				{
					this.lastErrCode = 16;
					server.ServerSpeedLog().AddErrorDecodeTimes();
					if (server.ServerSpeedLog().ErrorEncryptTimes >= 2L && server.ServerSpeedLog().ErrorContinurousTimes >= 5L && this.autoSwitchOff)
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
							if (server.ServerSpeedLog().ErrorConnectTimes >= 3L && server.ServerSpeedLog().ErrorContinurousTimes >= 5L && this.autoSwitchOff)
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
							if (server.ServerSpeedLog().ErrorConnectTimes >= 3L && this.autoSwitchOff)
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
							if (server.ServerSpeedLog().ErrorConnectTimes >= 3L && server.ServerSpeedLog().ErrorContinurousTimes >= 5L && this.autoSwitchOff)
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
							if (server.ServerSpeedLog().ErrorConnectTimes >= 3L && server.ServerSpeedLog().ErrorContinurousTimes >= 5L && this.autoSwitchOff)
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

		// Token: 0x060002CA RID: 714 RVA: 0x0001B788 File Offset: 0x00019988
		public static byte[] ParseUDPHeader(byte[] buffer, ref int len)
		{
			if (buffer.Length == 0)
			{
				return buffer;
			}
			if (buffer[0] == 129)
			{
				len--;
				byte[] array = new byte[len];
				Array.Copy(buffer, 1, array, 0, len);
				return array;
			}
			if (buffer[0] == 128 && len >= 2)
			{
				int num = (int)buffer[1];
				if (num + 2 < len)
				{
					len = len - num - 2;
					byte[] array2 = new byte[len];
					Array.Copy(buffer, num + 2, array2, 0, len);
					return array2;
				}
			}
			if (buffer[0] == 130 && len >= 3)
			{
				int num2 = ((int)buffer[1] << 8) + (int)buffer[2];
				if (num2 + 3 < len)
				{
					len = len - num2 - 3;
					byte[] array3 = new byte[len];
					Array.Copy(buffer, num2 + 3, array3, 0, len);
					return array3;
				}
			}
			if (len < buffer.Length)
			{
				byte[] array4 = new byte[len];
				Array.Copy(buffer, array4, len);
				return array4;
			}
			return buffer;
		}

		// Token: 0x060002DD RID: 733 RVA: 0x0001CD58 File Offset: 0x0001AF58
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
							this.ResetTimeout(this.TTL);
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
							if (this.obfs != null && this.obfs.getSentLength() > 0L)
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
								this.ResetTimeout(this.TTL);
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
				string remarks;
				string serverUrl = this.getServerUrl(out remarks);
				if (!Logging.LogSocketException(remarks, serverUrl, e))
				{
					Logging.LogUsefulException(e);
				}
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

		// Token: 0x060002E2 RID: 738 RVA: 0x0001D230 File Offset: 0x0001B430
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
				this.LogSocketException(e);
				string remarks;
				string serverUrl = this.getServerUrl(out remarks);
				if (!Logging.LogSocketException(remarks, serverUrl, e))
				{
					Logging.LogUsefulException(e);
				}
				this.Close();
			}
		}

		// Token: 0x060002DE RID: 734 RVA: 0x0001CF14 File Offset: 0x0001B114
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
						this.ResetTimeout(this.TTL);
					}
					else
					{
						flag = true;
					}
				}
			}
			catch (Exception e)
			{
				string remarks;
				string serverUrl = this.getServerUrl(out remarks);
				if (!Logging.LogSocketException(remarks, serverUrl, e))
				{
					Logging.LogUsefulException(e);
				}
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

		// Token: 0x060002E3 RID: 739 RVA: 0x0001D29C File Offset: 0x0001B49C
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
				this.LogSocketException(e);
				string remarks;
				string serverUrl = this.getServerUrl(out remarks);
				if (!Logging.LogSocketException(remarks, serverUrl, e))
				{
					Logging.LogUsefulException(e);
				}
				this.Close();
			}
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x0001C13C File Offset: 0x0001A33C
		private void PipeRemoteReceiveCallback(IAsyncResult ar)
		{
			Interlocked.Increment(ref this.remoteRecvCount);
			bool flag = false;
			try
			{
				if (!this.closed)
				{
					int num = this.endRemoteTCPRecv(ar);
					if (num > 0)
					{
						int num2 = 0;
						int num3;
						bool flag2;
						byte[] array = this.obfs.ClientDecode(this.remoteRecvBuffer, num, out num3, out flag2);
						byte[] array2 = new byte[array.Length];
						if (flag2)
						{
							this.RemoteSend(this.connetionRecvBuffer, 0);
							this.doRemoteTCPRecv();
						}
						if (num3 > 0)
						{
							object obj = this.decryptionLock;
							lock (obj)
							{
								this.encryptor.Decrypt(array, num3, array2, out num2);
								int num4;
								array2 = this.protocol.ClientPostDecrypt(array2, num2, out num4);
								num2 = num4;
							}
						}
						if (this.speedTester.BeginDownload())
						{
							DateTime arg_CE_0 = this.speedTester.timeBeginDownload;
							DateTime arg_DA_0 = this.speedTester.timeBeginUpload;
							int num5 = (int)(this.speedTester.timeBeginDownload - this.speedTester.timeBeginUpload).TotalMilliseconds;
							if (num5 >= 0)
							{
								this.server.ServerSpeedLog().AddConnectTime(num5);
							}
						}
						this.ResetTimeout(this.TTL);
						if (num2 == 0)
						{
							this.server.ServerSpeedLog().AddDownloadBytes((long)num);
							this.speedTester.AddDownloadSize(num);
							this.doRemoteTCPRecv();
						}
						else
						{
							if (this.connectionUDP == null)
							{
								Logging.LogBin(LogLevel.Debug, "remote recv", array2, num2);
							}
							else
							{
								Logging.LogBin(LogLevel.Debug, "udp remote recv", array2, num2);
							}
							if (this.connectionUDP == null)
							{
								if (this.detector.OnRecv(array2, num2) > 0)
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
								this.connection.BeginSend(array2, 0, num2, SocketFlags.None, new AsyncCallback(this.PipeConnectionSendCallback), null);
								this.server.ServerSpeedLog().AddDownloadRawBytes((long)num2);
								this.speedTester.AddRecvSize(num2);
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
									Array.Copy(array2, 0, this.remoteUDPRecvBuffer, this.remoteUDPRecvBufferLength, num2);
									this.remoteUDPRecvBufferLength += num2;
									while (this.remoteUDPRecvBufferLength > 6)
									{
										int num6 = ((int)this.remoteUDPRecvBuffer[0] << 8) + (int)this.remoteUDPRecvBuffer[1];
										if (num6 >= 65280)
										{
											num6 = ((int)this.remoteUDPRecvBuffer[1] << 8) + (int)this.remoteUDPRecvBuffer[2] + 65280;
										}
										if (num6 > this.remoteUDPRecvBufferLength)
										{
											break;
										}
										if (num6 >= 65280)
										{
											byte[] array3 = new byte[num6 - 1];
											Array.Copy(this.remoteUDPRecvBuffer, 1, array3, 0, num6 - 1);
											this.remoteUDPRecvBufferLength -= num6;
											Array.Copy(this.remoteUDPRecvBuffer, num6, this.remoteUDPRecvBuffer, 0, this.remoteUDPRecvBufferLength);
											array3[0] = 0;
											array3[1] = 0;
											list.Add(array3);
										}
										else
										{
											byte[] array4 = new byte[num6];
											Array.Copy(this.remoteUDPRecvBuffer, array4, num6);
											this.remoteUDPRecvBufferLength -= num6;
											Array.Copy(this.remoteUDPRecvBuffer, num6, this.remoteUDPRecvBuffer, 0, this.remoteUDPRecvBufferLength);
											array4[0] = 0;
											array4[1] = 0;
											list.Add(array4);
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
										this.server.ServerSpeedLog().AddDownloadRawBytes((long)current.Length);
										this.speedTester.AddRecvSize(current.Length);
										this.connectionUDP.BeginSendTo(current, 0, current.Length, SocketFlags.None, this.connectionUDPEndPoint, new AsyncCallback(this.PipeConnectionUDPSendCallback), null);
									}
								}
							}
							this.server.ServerSpeedLog().AddDownloadBytes((long)num);
							this.speedTester.AddDownloadSize(num);
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
				this.LogSocketException(e);
				string remarks;
				string serverUrl = this.getServerUrl(out remarks);
				if (!Logging.LogSocketException(remarks, serverUrl, e))
				{
					Logging.LogUsefulException(e);
				}
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

		// Token: 0x060002DF RID: 735 RVA: 0x0001D0A8 File Offset: 0x0001B2A8
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
						this.keepCurrentServer(this.targetHost);
					}
					this.lastKeepTime = DateTime.Now;
				}
			}
			catch (Exception e)
			{
				this.LogSocketException(e);
				string remarks;
				string serverUrl = this.getServerUrl(out remarks);
				if (!Logging.LogSocketException(remarks, serverUrl, e))
				{
					Logging.LogUsefulException(e);
				}
				this.Close();
			}
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x0001D1D0 File Offset: 0x0001B3D0
		private void PipeRemoteTDPSendCallback(IAsyncResult ar)
		{
			if (this.closed)
			{
				return;
			}
			try
			{
				this.doConnectionTCPRecv();
				this.doConnectionUDPRecv();
			}
			catch (Exception e)
			{
				this.LogSocketException(e);
				string remarks;
				string serverUrl = this.getServerUrl(out remarks);
				if (!Logging.LogSocketException(remarks, serverUrl, e))
				{
					Logging.LogUsefulException(e);
				}
				this.Close();
			}
		}

		// Token: 0x060002DA RID: 730 RVA: 0x0001C7A4 File Offset: 0x0001A9A4
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
					if (num > 0)
					{
						this.server.ServerSpeedLog().AddDownloadBytes((long)num);
						this.speedTester.AddDownloadSize(num);
						if (this.RemoveRemoteUDPRecvBufferHeader(ref num))
						{
							object obj = this.decryptionLock;
							int num2;
							lock (obj)
							{
								byte[] array = new byte[32768];
								this.encryptorUDP.ResetDecrypt();
								this.encryptorUDP.Decrypt(this.remoteRecvBuffer, num, array, out num2);
								array = Handler.ParseUDPHeader(array, ref num2);
								this.AddRemoteUDPRecvBufferHeader(array, ref num2);
							}
							if (this.connectionUDP == null)
							{
								Logging.LogBin(LogLevel.Debug, "remote recv", this.remoteSendBuffer, num2);
							}
							else
							{
								Logging.LogBin(LogLevel.Debug, "udp remote recv", this.remoteSendBuffer, num2);
							}
							int num3;
							byte[] buffer = this.protocol.ClientUdpPostDecrypt(this.remoteSendBuffer, num2, out num3);
							if (this.connectionUDP == null)
							{
								this.connection.BeginSend(buffer, 0, num3, SocketFlags.None, new AsyncCallback(this.PipeConnectionSendCallback), null);
							}
							else
							{
								this.connectionUDP.BeginSendTo(this.remoteSendBuffer, 0, num3, SocketFlags.None, this.connectionUDPEndPoint, new AsyncCallback(this.PipeConnectionUDPSendCallback), null);
							}
							this.speedTester.AddRecvSize(num3);
							this.server.ServerSpeedLog().AddDownloadRawBytes((long)num3);
							this.ResetTimeout(this.TTL);
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
				this.LogSocketException(e);
				string remarks;
				string serverUrl = this.getServerUrl(out remarks);
				if (!Logging.LogSocketException(remarks, serverUrl, e))
				{
					Logging.LogUsefulException(e);
				}
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

		// Token: 0x060002E0 RID: 736 RVA: 0x0001D164 File Offset: 0x0001B364
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
				this.LogSocketException(e);
				string remarks;
				string serverUrl = this.getServerUrl(out remarks);
				if (!Logging.LogSocketException(remarks, serverUrl, e))
				{
					Logging.LogUsefulException(e);
				}
				this.Close();
			}
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x0001B1BC File Offset: 0x000193BC
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

		// Token: 0x060002B0 RID: 688 RVA: 0x00019554 File Offset: 0x00017754
		public bool ReConnect()
		{
			Handler handler = new Handler();
			handler.getCurrentServer = this.getCurrentServer;
			handler.keepCurrentServer = this.keepCurrentServer;
			handler.authConnection = this.authConnection;
			handler.select_server = this.select_server;
			handler.connection = this.connection;
			handler.connectionUDP = this.connectionUDP;
			handler.reconnectTimesRemain = this.reconnectTimesRemain - 1;
			handler.reconnectTimes = this.reconnectTimes + 1;
			handler.forceRandom = this.forceRandom;
			handler.targetHost = this.targetHost;
			handler.command = this.command;
			handler.speedTester.transfer = this.speedTester.transfer;
			handler.proxyType = this.proxyType;
			handler.socks5RemoteHost = this.socks5RemoteHost;
			handler.socks5RemotePort = this.socks5RemotePort;
			handler.socks5RemoteUsername = this.socks5RemoteUsername;
			handler.socks5RemotePassword = this.socks5RemotePassword;
			handler.TTL = this.TTL;
			handler.autoSwitchOff = this.autoSwitchOff;
			handler.authConnection = this.authConnection;
			handler.authUser = this.authUser;
			handler.authPass = this.authPass;
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
			Handler arg_1CC_0 = handler;
			byte[] expr_1C8 = array;
			arg_1CC_0.Start(expr_1C8, expr_1C8.Length, true);
			return true;
		}

		// Token: 0x060002DB RID: 731 RVA: 0x0001C9E8 File Offset: 0x0001ABE8
		private void RemoteSend(byte[] bytes, int length)
		{
			if (this.remote == null)
			{
				return;
			}
			int datalength = 0;
			Logging.LogBin(LogLevel.Debug, "remote send", bytes, length);
			object obj = this.encryptionLock;
			lock (obj)
			{
				if (this.closed)
				{
					return;
				}
				int length2;
				byte[] buf = this.protocol.ClientPreEncrypt(bytes, length, out length2);
				this.encryptor.Encrypt(buf, length2, this.connetionSendBuffer, out datalength);
			}
			int num;
			byte[] buffer = this.obfs.ClientEncode(this.connetionSendBuffer, datalength, out num);
			if (this.remote != null && num > 0)
			{
				this.remote.BeginSend(buffer, 0, num, SocketFlags.None, new AsyncCallback(this.PipeRemoteSendCallback), null);
				this.server.ServerSpeedLog().AddUploadBytes((long)num);
				this.speedTester.AddUploadSize(num);
			}
		}

		// Token: 0x060002DC RID: 732 RVA: 0x0001CAD0 File Offset: 0x0001ACD0
		private void RemoteSendto(byte[] bytes, int length)
		{
			int num = 3;
			length -= num;
			byte[] array = new byte[length];
			Array.Copy(bytes, num, array, 0, length);
			object obj = this.encryptionLock;
			int num2;
			lock (obj)
			{
				if (this.closed)
				{
					return;
				}
				this.encryptorUDP.ResetEncrypt();
				this.protocol.SetServerInfoIV(this.encryptorUDP.getIV());
				int length2;
				byte[] buf = this.protocol.ClientUdpPreEncrypt(array, length, out length2);
				this.encryptorUDP.Encrypt(buf, length2, this.connetionSendBuffer, out num2);
			}
			if (this.socks5RemotePort > 0)
			{
				string text = this.server.server;
				int num3 = (this.server.server_udp_port == 0) ? this.server.server_port : this.server.server_udp_port;
				IPAddress iPAddress;
				if (!IPAddress.TryParse(text, out iPAddress))
				{
					array = new byte[num + 1 + 1 + text.Length + 2 + num2];
					Array.Copy(this.connetionSendBuffer, 0, array, num + 1 + 1 + text.Length + 2, num2);
					array[0] = 0;
					array[1] = 0;
					array[2] = 0;
					array[3] = 3;
					array[4] = (byte)text.Length;
					for (int i = 0; i < text.Length; i++)
					{
						array[5 + i] = (byte)text[i];
					}
					array[5 + text.Length] = (byte)(num3 / 256);
					array[5 + text.Length + 1] = (byte)(num3 % 256);
				}
				else
				{
					byte[] addressBytes = iPAddress.GetAddressBytes();
					array = new byte[num + 1 + addressBytes.Length + 2 + num2];
					Array.Copy(this.connetionSendBuffer, 0, array, num + 1 + addressBytes.Length + 2, num2);
					array[0] = 0;
					array[1] = 0;
					array[2] = 0;
                    array[3] = ((iPAddress.AddressFamily == AddressFamily.InterNetworkV6) ? (byte)4 : (byte)1);
					for (int j = 0; j < addressBytes.Length; j++)
					{
						array[4 + j] = addressBytes[j];
					}
					array[4 + addressBytes.Length] = (byte)(num3 / 256);
					array[4 + addressBytes.Length + 1] = (byte)(num3 % 256);
				}
				num2 = array.Length;
				Array.Copy(array, this.connetionSendBuffer, num2);
			}
			this.remoteUDP.BeginSendTo(this.connetionSendBuffer, 0, num2, SocketFlags.None, this.remoteUDPEndPoint, new AsyncCallback(this.PipeRemoteUDPSendCallback), null);
			this.server.ServerSpeedLog().AddUploadBytes((long)num2);
			this.speedTester.AddUploadSize(num2);
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x0001C64C File Offset: 0x0001A84C
		private bool RemoveRemoteUDPRecvBufferHeader(ref int bytesRead)
		{
			if (this.socks5RemotePort > 0)
			{
				if (bytesRead < 7)
				{
					return false;
				}
				int num2;
				if (this.remoteRecvBuffer[3] == 1)
				{
					int num = 10;
					bytesRead -= num;
					num2 = (int)this.remoteRecvBuffer[num - 2] * 256 + (int)this.remoteRecvBuffer[num - 1];
					Array.Copy(this.remoteRecvBuffer, num, this.remoteRecvBuffer, 0, bytesRead);
				}
				else if (this.remoteRecvBuffer[3] == 4)
				{
					int num3 = 22;
					bytesRead -= num3;
					num2 = (int)this.remoteRecvBuffer[num3 - 2] * 256 + (int)this.remoteRecvBuffer[num3 - 1];
					Array.Copy(this.remoteRecvBuffer, num3, this.remoteRecvBuffer, 0, bytesRead);
				}
				else
				{
					if (this.remoteRecvBuffer[3] != 3)
					{
						return false;
					}
					int num4 = (int)(5 + this.remoteRecvBuffer[4] + 2);
					bytesRead -= num4;
					num2 = (int)this.remoteRecvBuffer[num4 - 2] * 256 + (int)this.remoteRecvBuffer[num4 - 1];
					Array.Copy(this.remoteRecvBuffer, num4, this.remoteRecvBuffer, 0, bytesRead);
				}
				if (num2 != this.server.server_port && num2 != this.server.server_udp_port)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060002AB RID: 683 RVA: 0x0001906C File Offset: 0x0001726C
		private void ResetTimeout(double time)
		{
			if (time <= 0.0 && this.timer == null)
			{
				return;
			}
			this.try_keep_alive = 0;
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
				else if (this.timer == null)
				{
					this.timer = new System.Timers.Timer(time * 1000.0);
					this.timer.Elapsed += new ElapsedEventHandler(this.timer_Elapsed);
					this.timer.Start();
				}
				else
				{
					this.timer.Interval = time * 1000.0;
					this.timer.Stop();
					this.timer.Start();
				}
			}
		}

		// Token: 0x060002BA RID: 698 RVA: 0x0001A588 File Offset: 0x00018788
		private void RspHttpHandshakeReceive()
		{
			this.command = 1;
			if (this.httpProxyState == null)
			{
				this.httpProxyState = new HttpPraser(false);
			}
			else
			{
				this.command = 1;
			}
			if (Utils.isMatchSubNet(((IPEndPoint)this.connection.RemoteEndPoint).Address, "127.0.0.0/8"))
			{
				this.httpProxyState.httpAuthUser = "";
				this.httpProxyState.httpAuthPass = "";
			}
			else
			{
				this.httpProxyState.httpAuthUser = this.authUser;
				this.httpProxyState.httpAuthPass = this.authPass;
			}
			int num = this.httpProxyState.HandshakeReceive(this._firstPacket, this._firstPacketLength, ref this.remoteHeaderSendBuffer);
			if (num == 1)
			{
				this.connection.BeginReceive(this.connetionRecvBuffer, 0, this._firstPacket.Length, SocketFlags.None, new AsyncCallback(this.HttpHandshakeRecv), null);
				return;
			}
			if (num == 2)
			{
				string s = this.httpProxyState.Http407();
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				this.connection.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, new AsyncCallback(this.HttpHandshakeAuthEndSend), null);
				return;
			}
			if (num == 3)
			{
				this.Connect();
				return;
			}
			if (num == 4)
			{
				this.Connect();
				return;
			}
			if (num == 0)
			{
				string s2 = this.httpProxyState.Http200();
				byte[] bytes2 = Encoding.UTF8.GetBytes(s2);
				this.connection.BeginSend(bytes2, 0, bytes2.Length, SocketFlags.None, new AsyncCallback(this.StartConnect), null);
				return;
			}
			if (num == 500)
			{
				string s3 = this.httpProxyState.Http500();
				byte[] bytes3 = Encoding.UTF8.GetBytes(s3);
				this.connection.BeginSend(bytes3, 0, bytes3.Length, SocketFlags.None, new AsyncCallback(this.HttpHandshakeAuthEndSend), null);
			}
		}

		// Token: 0x060002BD RID: 701 RVA: 0x0001A864 File Offset: 0x00018A64
		private void RspSocks4aHandshakeReceive()
		{
			List<byte> list = new List<byte>();
			for (int i = 0; i < this._firstPacketLength; i++)
			{
				list.Add(this._firstPacket[i]);
			}
			List<byte> range = list.GetRange(0, 4);
			range[0] = 0;
			range[1] = 90;
			if (this._firstPacket[4] == 0 && this._firstPacket[5] == 0 && this._firstPacket[6] == 0 && this._firstPacket[7] == 1)
			{
				for (int j = 0; j < 4; j++)
				{
					range.Add(0);
				}
				int num = list.IndexOf(0, 8);
				List<byte> range2 = list.GetRange(num + 1, list.Count - num - 2);
				this.remoteHeaderSendBuffer = new byte[2 + range2.Count + 2];
				this.remoteHeaderSendBuffer[0] = 3;
				this.remoteHeaderSendBuffer[1] = (byte)range2.Count;
				Array.Copy(range2.ToArray(), 0, this.remoteHeaderSendBuffer, 2, range2.Count);
				this.remoteHeaderSendBuffer[2 + range2.Count] = range[2];
				this.remoteHeaderSendBuffer[2 + range2.Count + 1] = range[3];
			}
			else
			{
				for (int k = 0; k < 4; k++)
				{
					range.Add(this._firstPacket[4 + k]);
				}
				this.remoteHeaderSendBuffer = new byte[7];
				this.remoteHeaderSendBuffer[0] = 1;
				Array.Copy(range.ToArray(), 4, this.remoteHeaderSendBuffer, 1, 4);
				this.remoteHeaderSendBuffer[5] = range[2];
				this.remoteHeaderSendBuffer[6] = range[3];
			}
			this.command = 1;
			this.connection.BeginSend(range.ToArray(), 0, range.Count, SocketFlags.None, new AsyncCallback(this.StartConnect), null);
		}

		// Token: 0x060002BE RID: 702 RVA: 0x0001AA2C File Offset: 0x00018C2C
		private void RspSocks5HandshakeReceive()
		{
			byte[] expr_06 = new byte[2];
			expr_06[0] = 5;
			byte[] array = expr_06;
			if (this._firstPacket[0] != 5)
			{
				array = new byte[]
				{
					0,
					91
				};
				Console.WriteLine("socks 4/5 protocol error");
			}
			if (this.authConnection != null && !Utils.isMatchSubNet(((IPEndPoint)this.connection.RemoteEndPoint).Address, "127.0.0.0/8"))
			{
				array[1] = 2;
				this.connection.BeginSend(array, 0, array.Length, SocketFlags.None, new AsyncCallback(this.HandshakeAuthSendCallback), null);
				return;
			}
			this.connection.BeginSend(array, 0, array.Length, SocketFlags.None, new AsyncCallback(this.HandshakeSendCallback), null);
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x0001AFDC File Offset: 0x000191DC
		private void RspSocks5TCPHeader()
		{
			if (this.connection.AddressFamily == AddressFamily.InterNetwork)
			{
				byte[] expr_15 = new byte[10];
				expr_15[0] = 5;
				expr_15[3] = 1;
				byte[] array = expr_15;
				this.connection.BeginSend(array, 0, array.Length, SocketFlags.None, new AsyncCallback(this.StartConnect), null);
				return;
			}
			byte[] expr_45 = new byte[22];
			expr_45[0] = 5;
			expr_45[3] = 4;
			byte[] array2 = expr_45;
			this.connection.BeginSend(array2, 0, array2.Length, SocketFlags.None, new AsyncCallback(this.StartConnect), null);
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x0001ADA8 File Offset: 0x00018FA8
		private void RspSocks5UDPHeader(int bytesRead)
		{
			bool flag = this.connection.AddressFamily == AddressFamily.InterNetworkV6;
			int num = 0;
			if (bytesRead >= 9)
			{
				flag = (this.remoteHeaderSendBuffer[0] == 4);
				if (!flag)
				{
					num = (int)this.remoteHeaderSendBuffer[5] * 256 + (int)this.remoteHeaderSendBuffer[6];
				}
				else
				{
					num = (int)this.remoteHeaderSendBuffer[17] * 256 + (int)this.remoteHeaderSendBuffer[18];
				}
			}
			if (!flag)
			{
				this.remoteHeaderSendBuffer = new byte[7];
				this.remoteHeaderSendBuffer[0] = 9;
				this.remoteHeaderSendBuffer[5] = (byte)(num / 256);
				this.remoteHeaderSendBuffer[6] = (byte)(num % 256);
			}
			else
			{
				this.remoteHeaderSendBuffer = new byte[19];
				this.remoteHeaderSendBuffer[0] = 12;
				this.remoteHeaderSendBuffer[17] = (byte)(num / 256);
				this.remoteHeaderSendBuffer[18] = (byte)(num % 256);
			}
			this.connectionUDPEndPoint = null;
			int i = 0;
			IPAddress iPAddress = flag ? IPAddress.IPv6Any : IPAddress.Any;
			this.connectionUDP = new Socket(iPAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
			while (i < 65536)
			{
				try
				{
					this.connectionUDP.Bind(new IPEndPoint(iPAddress, i));
					break;
				}
				catch (Exception)
				{
				}
				i++;
			}
			i = ((IPEndPoint)this.connectionUDP.LocalEndPoint).Port;
			if (!flag)
			{
				byte[] array = new byte[]
				{
					5,
					0,
					0,
					1,
					0,
					0,
					0,
					0,
					(byte)(i / 256),
					(byte)(i % 256)
				};
				Array.Copy(((IPEndPoint)this.connection.LocalEndPoint).Address.GetAddressBytes(), 0, array, 4, 4);
				this.connection.BeginSend(array, 0, array.Length, SocketFlags.None, new AsyncCallback(this.StartConnect), null);
				return;
			}
			byte[] array2 = new byte[]
			{
				5,
				0,
				0,
				4,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				(byte)(i / 256),
				(byte)(i % 256)
			};
			Array.Copy(((IPEndPoint)this.connection.LocalEndPoint).Address.GetAddressBytes(), 0, array2, 4, 16);
			this.connection.BeginSend(array2, 0, array2.Length, SocketFlags.None, new AsyncCallback(this.StartConnect), null);
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x0001BA78 File Offset: 0x00019C78
		private void SetObfsPlugin()
		{
			object obj = this.obfsLock;
			lock (obj)
			{
				if (this.server.getProtocolData() == null)
				{
					this.server.setProtocolData(this.protocol.InitData());
				}
				if (this.server.getObfsData() == null)
				{
					this.server.setObfsData(this.obfs.InitData());
				}
			}
			int tcp_mss = 1460;
			int headSize;
			if (this.connectionSendBufferList != null && this.connectionSendBufferList.Count > 0)
			{
				headSize = ObfsBase.GetHeadSize(this.connectionSendBufferList[0], 30);
			}
			else
			{
				headSize = ObfsBase.GetHeadSize(this.remoteHeaderSendBuffer, 30);
			}
			if (this.remoteTCPEndPoint != null)
			{
				try
				{
					this.protocol.SetServerInfo(new ServerInfo(this.remoteTCPEndPoint.Address.ToString(), this.server.server_port, "", this.server.getProtocolData(), this.encryptor.getIV(), this.encryptor.getKey(), headSize, tcp_mss));
				}
				catch (Exception)
				{
					this.protocol.SetServerInfo(new ServerInfo(this.server.server, this.server.server_port, "", this.server.getProtocolData(), this.encryptor.getIV(), this.encryptor.getKey(), headSize, tcp_mss));
				}
				try
				{
					this.obfs.SetServerInfo(new ServerInfo(this.remoteTCPEndPoint.Address.ToString(), this.server.server_port, this.server.obfsparam, this.server.getObfsData(), this.encryptor.getIV(), this.encryptor.getKey(), headSize, tcp_mss));
					return;
				}
				catch (Exception)
				{
					this.obfs.SetServerInfo(new ServerInfo(this.server.server, this.server.server_port, this.server.obfsparam, this.server.getObfsData(), this.encryptor.getIV(), this.encryptor.getKey(), headSize, tcp_mss));
					return;
				}
			}
			this.protocol.SetServerInfo(new ServerInfo(this.server.server, this.server.server_port, "", this.server.getProtocolData(), this.encryptor.getIV(), this.encryptor.getKey(), headSize, tcp_mss));
			this.obfs.SetServerInfo(new ServerInfo(this.server.server, this.server.server_port, this.server.obfsparam, this.server.getObfsData(), this.encryptor.getIV(), this.encryptor.getKey(), headSize, tcp_mss));
		}

		// Token: 0x060002AE RID: 686 RVA: 0x000192B2 File Offset: 0x000174B2
		public void setServerTransferTotal(ServerTransferTotal transfer)
		{
			this.speedTester.transfer = transfer;
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x00019750 File Offset: 0x00017950
		public void Start(byte[] firstPacket, int length, bool tunnel)
		{
			this._firstPacket = firstPacket;
			this._firstPacketLength = length;
			if (this.socks5RemotePort > 0)
			{
				this.autoSwitchOff = false;
			}
			this.ResetTimeout(this.TTL);
			if (this.State == Handler.ConnectState.READY)
			{
				this.State = Handler.ConnectState.HANDSHAKE;
				if (tunnel)
				{
					this.remoteHeaderSendBuffer = firstPacket;
					this.Connect();
					return;
				}
				this.HandshakeReceive();
			}
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x0001B61C File Offset: 0x0001981C
		private void StartConnect(IAsyncResult ar)
		{
			try
			{
				this.connection.EndSend(ar);
				this.Connect();
			}
			catch (Exception e)
			{
				this.LogSocketException(e);
				string remarks;
				string serverUrl = this.getServerUrl(out remarks);
				if (!Logging.LogSocketException(remarks, serverUrl, e))
				{
					Logging.LogUsefulException(e);
				}
				this.Close();
			}
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x0001BE30 File Offset: 0x0001A030
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
				this.ResetTimeout(this.TTL);
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
								byte[] expr_80 = current;
								this.RemoteSend(expr_80, expr_80.Length);
							}
							goto IL_21F;
						}
					}
					if (this.httpProxyState != null)
					{
						if (this.remoteHeaderSendBuffer != null)
						{
							this.detector.OnSend(this.remoteHeaderSendBuffer, this.remoteHeaderSendBuffer.Length);
							byte[] array = new byte[this.remoteHeaderSendBuffer.Length];
							Array.Copy(this.remoteHeaderSendBuffer, array, array.Length);
							this.connectionSendBufferList.Add(array);
							this.RemoteSend(this.remoteHeaderSendBuffer, this.remoteHeaderSendBuffer.Length);
						}
						this.remoteHeaderSendBuffer = null;
						this.httpProxyState = null;
					}
					else if (this.remoteHeaderSendBuffer != null)
					{
						this.detector.OnSend(this.remoteHeaderSendBuffer, this.remoteHeaderSendBuffer.Length);
						byte[] array2 = new byte[this.remoteHeaderSendBuffer.Length];
						Array.Copy(this.remoteHeaderSendBuffer, array2, array2.Length);
						this.connectionSendBufferList.Add(array2);
						this.RemoteSend(this.remoteHeaderSendBuffer, this.remoteHeaderSendBuffer.Length);
						this.remoteHeaderSendBuffer = null;
					}
				}
				else
				{
					this.connetionRecvBuffer = new byte[131072];
					this.connetionSendBuffer = new byte[133120];
					this.remoteRecvBuffer = new byte[131072];
					this.remoteSendBuffer = new byte[133120];
					if (!this.server.udp_over_tcp && this.remoteUDP != null)
					{
						if (this.socks5RemotePort == 0)
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
				IL_21F:
				this.State = Handler.ConnectState.CONNECTED;
				this.doRemoteTCPRecv();
				this.doRemoteUDPRecv();
				this.doConnectionTCPRecv();
				this.doConnectionUDPRecv();
			}
			catch (Exception e)
			{
				this.LogSocketException(e);
				string remarks;
				string serverUrl = this.getServerUrl(out remarks);
				if (!Logging.LogSocketException(remarks, serverUrl, e))
				{
					Logging.LogUsefulException(e);
				}
				this.Close();
			}
		}

		// Token: 0x060002AC RID: 684 RVA: 0x00019180 File Offset: 0x00017380
		private void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (this.closed)
			{
				return;
			}
			bool flag = false;
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			dictionary["auth_sha1_v2"] = 1;
			try
			{
				if (this.try_keep_alive <= 0 && this.State == Handler.ConnectState.CONNECTED && dictionary.ContainsKey(this.protocol.Name()))
				{
					this.try_keep_alive++;
					this.RemoteSend(null, -1);
				}
				else if (this.connection != null)
				{
					if (this.remote != null && this.reconnectTimesRemain > 0 && this.connectionSendBufferList != null && (this.State == Handler.ConnectState.CONNECTED || this.State == Handler.ConnectState.CONNECTING))
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

		// Token: 0x060002B4 RID: 692 RVA: 0x00019B70 File Offset: 0x00017D70
		public bool TryReconnect()
		{
			if (this.State == Handler.ConnectState.CONNECTING)
			{
				if (this.reconnectTimesRemain > 0)
				{
					return this.ReConnect();
				}
			}
			else if (this.State == Handler.ConnectState.CONNECTED && this.obfs.getSentLength() == 0L && this.connectionSendBufferList != null && this.reconnectTimesRemain > 0)
			{
				this.State = Handler.ConnectState.CONNECTING;
				return this.ReConnect();
			}
			return false;
		}

		// Token: 0x1700001E RID: 30
		private Handler.ConnectState State
		{
			// Token: 0x060002A9 RID: 681 RVA: 0x00019026 File Offset: 0x00017226
			get
			{
				return this.state;
			}
			// Token: 0x060002AA RID: 682 RVA: 0x00019030 File Offset: 0x00017230
			set
			{
				lock (this)
				{
					this.state = value;
				}
			}
		}

		// Token: 0x04000243 RID: 579
		protected IAsyncResult arTCPConnection;

		// Token: 0x04000244 RID: 580
		protected IAsyncResult arTCPRemote;

		// Token: 0x04000245 RID: 581
		protected IAsyncResult arUDPConnection;

		// Token: 0x04000246 RID: 582
		protected IAsyncResult arUDPRemote;

		// Token: 0x04000216 RID: 534
		public Handler.AuthConnection authConnection;

		// Token: 0x0400022F RID: 559
		public string authPass;

		// Token: 0x0400022E RID: 558
		public string authUser;

		// Token: 0x04000217 RID: 535
		public bool autoSwitchOff = true;

		// Token: 0x0400022A RID: 554
		protected const int AutoSwitchOffErrorTimes = 5;

		// Token: 0x04000229 RID: 553
		protected const int BufferSize = 17408;

		// Token: 0x04000238 RID: 568
		protected bool closed;

		// Token: 0x04000225 RID: 549
		public byte command;

		// Token: 0x0400020D RID: 525
		public Socket connection;

		// Token: 0x04000242 RID: 578
		protected int connectionRecvCount;

		// Token: 0x04000233 RID: 563
		protected List<byte[]> connectionSendBufferList = new List<byte[]>();

		// Token: 0x0400023D RID: 573
		protected bool connectionTCPIdle;

		// Token: 0x0400020E RID: 526
		public Socket connectionUDP;

		// Token: 0x0400020F RID: 527
		protected IPEndPoint connectionUDPEndPoint;

		// Token: 0x0400023E RID: 574
		protected bool connectionUDPIdle;

		// Token: 0x04000231 RID: 561
		protected byte[] connetionRecvBuffer = new byte[32768];

		// Token: 0x04000232 RID: 562
		protected byte[] connetionSendBuffer = new byte[34816];

		// Token: 0x0400023A RID: 570
		protected object decryptionLock = new object();

		// Token: 0x0400021F RID: 543
		protected ProtocolResponseDetector detector = new ProtocolResponseDetector();

		// Token: 0x04000224 RID: 548
		protected DnsQuery dns;

		// Token: 0x0400020B RID: 523
		public string dns_servers;

		// Token: 0x04000239 RID: 569
		protected object encryptionLock = new object();

		// Token: 0x0400021B RID: 539
		protected IEncryptor encryptor;

		// Token: 0x0400021C RID: 540
		protected IEncryptor encryptorUDP;

		// Token: 0x0400021A RID: 538
		public bool forceRandom;

		// Token: 0x0400020C RID: 524
		public bool fouce_local_dns_query;

		// Token: 0x04000205 RID: 517
		public Handler.GetCurrentServer getCurrentServer;

		// Token: 0x04000230 RID: 560
		protected HttpPraser httpProxyState;

		// Token: 0x04000206 RID: 518
		public Handler.KeepCurrentServer keepCurrentServer;

		// Token: 0x04000248 RID: 584
		protected int lastErrCode;

		// Token: 0x04000235 RID: 565
		protected DateTime lastKeepTime;

		// Token: 0x0400021E RID: 542
		public IObfs obfs;

		// Token: 0x0400023B RID: 571
		protected object obfsLock = new object();

		// Token: 0x0400021D RID: 541
		public IObfs protocol;

		// Token: 0x04000210 RID: 528
		public int proxyType;

		// Token: 0x04000215 RID: 533
		public string proxyUserAgent;

		// Token: 0x04000249 RID: 585
		protected Random random = new Random();

		// Token: 0x04000219 RID: 537
		protected int reconnectTimes;

		// Token: 0x04000218 RID: 536
		public int reconnectTimesRemain;

		// Token: 0x04000228 RID: 552
		protected const int RecvSize = 16384;

		// Token: 0x0400023C RID: 572
		protected object recvUDPoverTCPLock = new object();

		// Token: 0x04000220 RID: 544
		protected Socket remote;

		// Token: 0x0400022D RID: 557
		protected byte[] remoteHeaderSendBuffer;

		// Token: 0x0400022B RID: 555
		protected byte[] remoteRecvBuffer = new byte[32768];

		// Token: 0x04000241 RID: 577
		protected int remoteRecvCount;

		// Token: 0x0400022C RID: 556
		protected byte[] remoteSendBuffer = new byte[34816];

		// Token: 0x04000222 RID: 546
		protected IPEndPoint remoteTCPEndPoint;

		// Token: 0x0400023F RID: 575
		protected bool remoteTCPIdle;

		// Token: 0x04000221 RID: 545
		protected Socket remoteUDP;

		// Token: 0x04000223 RID: 547
		protected IPEndPoint remoteUDPEndPoint;

		// Token: 0x04000240 RID: 576
		protected bool remoteUDPIdle;

		// Token: 0x04000236 RID: 566
		protected byte[] remoteUDPRecvBuffer = new byte[32768];

		// Token: 0x04000237 RID: 567
		protected int remoteUDPRecvBufferLength;

		// Token: 0x04000208 RID: 520
		public Server select_server;

		// Token: 0x04000207 RID: 519
		public Server server;

		// Token: 0x04000211 RID: 529
		public string socks5RemoteHost;

		// Token: 0x04000214 RID: 532
		public string socks5RemotePassword;

		// Token: 0x04000212 RID: 530
		public int socks5RemotePort;

		// Token: 0x04000213 RID: 531
		public string socks5RemoteUsername;

		// Token: 0x04000247 RID: 583
		protected SpeedTester speedTester = new SpeedTester();

		// Token: 0x0400024C RID: 588
		private Handler.ConnectState state;

		// Token: 0x04000234 RID: 564
		protected string targetHost;

		// Token: 0x0400024A RID: 586
		protected System.Timers.Timer timer;

		// Token: 0x0400024B RID: 587
		protected object timerLock = new object();

		// Token: 0x0400020A RID: 522
		public int try_keep_alive;

		// Token: 0x04000209 RID: 521
		public double TTL;

		// Token: 0x04000226 RID: 550
		protected byte[] _firstPacket;

		// Token: 0x04000227 RID: 551
		protected int _firstPacketLength;

		// Token: 0x020000B1 RID: 177
		// Token: 0x06000586 RID: 1414
		public delegate bool AuthConnection(Socket connection, string authUser, string authPass);

		// Token: 0x020000B2 RID: 178
		private enum ConnectState
		{
			// Token: 0x04000464 RID: 1124
			END = -1,
			// Token: 0x04000465 RID: 1125
			READY,
			// Token: 0x04000466 RID: 1126
			HANDSHAKE,
			// Token: 0x04000467 RID: 1127
			CONNECTING,
			// Token: 0x04000468 RID: 1128
			CONNECTED
		}

		// Token: 0x020000AF RID: 175
		// Token: 0x0600057E RID: 1406
		public delegate Server GetCurrentServer(string targetURI = null, bool usingRandom = false, bool forceRandom = false);

		// Token: 0x020000AE RID: 174
		// Token: 0x0600057A RID: 1402
		private delegate IPHostEntry GetHostEntryHandler(string ip);

		// Token: 0x020000B0 RID: 176
		// Token: 0x06000582 RID: 1410
		public delegate void KeepCurrentServer(string targetURI);
	}
}
