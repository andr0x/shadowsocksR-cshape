using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Shadowsocks.Model;
using Shadowsocks.Util;

namespace Shadowsocks.Controller
{
	// Token: 0x02000047 RID: 71
	internal class ProxyAuthHandler
	{
		// Token: 0x0600025D RID: 605 RVA: 0x00017128 File Offset: 0x00015328
		public ProxyAuthHandler(Configuration config, ServerTransferTotal transfer, byte[] firstPacket, int length, Socket socket)
		{
			int port = ((IPEndPoint)socket.LocalEndPoint).Port;
			this._config = config;
			this._transfer = transfer;
			this._firstPacket = firstPacket;
			this._firstPacketLength = length;
			this._connection = socket;
			socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, true);
			if (this._config.GetPortMapCache().ContainsKey(port))
			{
				this.Connect();
				return;
			}
			this.HandshakeReceive();
		}

		// Token: 0x06000260 RID: 608 RVA: 0x0001722C File Offset: 0x0001542C
		private bool AuthConnection(Socket connection, string authUser, string authPass)
		{
			return (this._config.authUser ?? "").Length == 0 || (this._config.authUser == authUser && (this._config.authPass ?? "") == authPass);
		}

		// Token: 0x0600025F RID: 607 RVA: 0x0001721C File Offset: 0x0001541C
		private void Close()
		{
			this.CloseSocket(ref this._connection);
		}

		// Token: 0x0600025E RID: 606 RVA: 0x000171AC File Offset: 0x000153AC
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

		// Token: 0x0600026E RID: 622 RVA: 0x00017E14 File Offset: 0x00016014
		private void Connect()
		{
			int port = ((IPEndPoint)this._connection.LocalEndPoint).Port;
			Handler handler = new Handler();
			handler.getCurrentServer = ((string targetURI, bool usingRandom, bool forceRandom) => this._config.GetCurrentServer(targetURI, usingRandom, forceRandom));
			handler.keepCurrentServer = delegate(string targetURI, string id)
			{
				this._config.KeepCurrentServer(targetURI, id);
			};
			handler.connection = this._connection;
			handler.connectionUDP = this._connectionUDP;
			handler.cfg.reconnectTimesRemain = this._config.reconnectTimes;
			handler.cfg.forceRandom = this._config.random;
			handler.setServerTransferTotal(this._transfer);
			if (this._config.proxyEnable)
			{
				handler.cfg.proxyType = this._config.proxyType;
				handler.cfg.socks5RemoteHost = this._config.proxyHost;
				handler.cfg.socks5RemotePort = this._config.proxyPort;
				handler.cfg.socks5RemoteUsername = this._config.proxyAuthUser;
				handler.cfg.socks5RemotePassword = this._config.proxyAuthPass;
				handler.cfg.proxyUserAgent = this._config.proxyUserAgent;
			}
			handler.cfg.TTL = (double)this._config.TTL;
			handler.cfg.autoSwitchOff = this._config.autoBan;
			if (this._config.dns_server != null && this._config.dns_server.Length > 0)
			{
				handler.cfg.dns_servers = this._config.dns_server;
			}
			if (this._config.GetPortMapCache().ContainsKey(port))
			{
				PortMapConfigCache portMapConfigCache = this._config.GetPortMapCache()[port];
				if (portMapConfigCache.id == portMapConfigCache.server.id)
				{
					handler.select_server = portMapConfigCache.server;
					byte[] bytes = Encoding.UTF8.GetBytes(portMapConfigCache.server_addr);
					byte[] array = new byte[this._firstPacketLength + bytes.Length + 4];
					array[0] = 3;
					array[1] = (byte)bytes.Length;
					Array.Copy(bytes, 0, array, 2, bytes.Length);
					array[bytes.Length + 2] = (byte)(portMapConfigCache.server_port / 256);
					array[bytes.Length + 3] = (byte)(portMapConfigCache.server_port % 256);
					Array.Copy(this._firstPacket, 0, array, bytes.Length + 4, this._firstPacketLength);
					this._remoteHeaderSendBuffer = array;
				}
			}
			handler.Start(this._remoteHeaderSendBuffer, this._remoteHeaderSendBuffer.Length);
		}

		// Token: 0x06000265 RID: 613 RVA: 0x00017630 File Offset: 0x00015830
		private void HandshakeAuthReceiveCallback(IAsyncResult ar)
		{
			try
			{
				if (this._connection.EndReceive(ar) >= 3)
				{
					byte b = this._connetionRecvBuffer[1];
					byte count = this._connetionRecvBuffer[(int)(b + 2)];
					byte[] expr_29 = new byte[2];
					expr_29[0] = 1;
					byte[] array = expr_29;
					string @string = Encoding.UTF8.GetString(this._connetionRecvBuffer, 2, (int)b);
					string string2 = Encoding.UTF8.GetString(this._connetionRecvBuffer, (int)(b + 3), (int)count);
					if (this.AuthConnection(this._connection, @string, string2))
					{
						this._connection.BeginSend(array, 0, array.Length, SocketFlags.None, new AsyncCallback(this.HandshakeSendCallback), null);
					}
				}
				else
				{
					Console.WriteLine("failed to recv data in HandshakeAuthReceiveCallback");
					this.Close();
				}
			}
			catch (Exception arg_9B_0)
			{
				Logging.LogUsefulException(arg_9B_0);
				this.Close();
			}
		}

		// Token: 0x06000264 RID: 612 RVA: 0x000175D0 File Offset: 0x000157D0
		private void HandshakeAuthSendCallback(IAsyncResult ar)
		{
			try
			{
				this._connection.EndSend(ar);
				this._connection.BeginReceive(this._connetionRecvBuffer, 0, 1024, SocketFlags.None, new AsyncCallback(this.HandshakeAuthReceiveCallback), null);
			}
			catch (Exception arg_35_0)
			{
				Logging.LogUsefulException(arg_35_0);
				this.Close();
			}
		}

		// Token: 0x06000261 RID: 609 RVA: 0x00017288 File Offset: 0x00015488
		private void HandshakeReceive()
		{
			try
			{
				if (this._firstPacketLength > 1)
				{
					if ((this._config.authUser == null || this._config.authUser.Length <= 0 || Utils.isMatchSubNet(((IPEndPoint)this._connection.RemoteEndPoint).Address, "127.0.0.0/8")) && this._firstPacket[0] == 4 && this._firstPacketLength >= 9)
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
			catch (Exception arg_96_0)
			{
				Logging.LogUsefulException(arg_96_0);
				this.Close();
			}
		}

		// Token: 0x06000267 RID: 615 RVA: 0x00017758 File Offset: 0x00015958
		private void HandshakeReceive2Callback(IAsyncResult ar)
		{
			try
			{
				int num = this._connection.EndReceive(ar);
				if (num >= 3)
				{
					this.command = this._connetionRecvBuffer[1];
					if (num > 3)
					{
						this._remoteHeaderSendBuffer = new byte[num - 3];
						Array.Copy(this._connetionRecvBuffer, 3, this._remoteHeaderSendBuffer, 0, this._remoteHeaderSendBuffer.Length);
					}
					else
					{
						this._remoteHeaderSendBuffer = null;
					}
					if (this.command == 3)
					{
						if (num >= 5)
						{
							if (this._remoteHeaderSendBuffer == null)
							{
								this._remoteHeaderSendBuffer = new byte[num - 3];
								Array.Copy(this._connetionRecvBuffer, 3, this._remoteHeaderSendBuffer, 0, this._remoteHeaderSendBuffer.Length);
							}
							else
							{
								Array.Resize<byte>(ref this._remoteHeaderSendBuffer, this._remoteHeaderSendBuffer.Length + num - 3);
								Array.Copy(this._connetionRecvBuffer, 3, this._remoteHeaderSendBuffer, this._remoteHeaderSendBuffer.Length - num, num);
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
			catch (Exception arg_104_0)
			{
				Logging.LogUsefulException(arg_104_0);
				this.Close();
			}
		}

		// Token: 0x06000266 RID: 614 RVA: 0x000176F8 File Offset: 0x000158F8
		private void HandshakeSendCallback(IAsyncResult ar)
		{
			try
			{
				this._connection.EndSend(ar);
				this._connection.BeginReceive(this._connetionRecvBuffer, 0, 1024, SocketFlags.None, new AsyncCallback(this.HandshakeReceive2Callback), null);
			}
			catch (Exception arg_35_0)
			{
				Logging.LogUsefulException(arg_35_0);
				this.Close();
			}
		}

		// Token: 0x0600026B RID: 619 RVA: 0x00017D00 File Offset: 0x00015F00
		private void HttpHandshakeAuthEndSend(IAsyncResult ar)
		{
			try
			{
				this._connection.EndSend(ar);
				this._connection.BeginReceive(this._connetionRecvBuffer, 0, this._firstPacket.Length, SocketFlags.None, new AsyncCallback(this.HttpHandshakeRecv), null);
			}
			catch (Exception arg_38_0)
			{
				Logging.LogUsefulException(arg_38_0);
				this.Close();
			}
		}

		// Token: 0x0600026C RID: 620 RVA: 0x00017D64 File Offset: 0x00015F64
		private void HttpHandshakeRecv(IAsyncResult ar)
		{
			try
			{
				int num = this._connection.EndReceive(ar);
				if (num > 0)
				{
					Array.Copy(this._connetionRecvBuffer, this._firstPacket, num);
					this._firstPacketLength = num;
					this.RspHttpHandshakeReceive();
				}
				else
				{
					Console.WriteLine("failed to recv data in HttpHandshakeRecv");
					this.Close();
				}
			}
			catch (Exception arg_44_0)
			{
				Logging.LogUsefulException(arg_44_0);
				this.Close();
			}
		}

		// Token: 0x0600026A RID: 618 RVA: 0x00017B40 File Offset: 0x00015D40
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
			if (Utils.isMatchSubNet(((IPEndPoint)this._connection.RemoteEndPoint).Address, "127.0.0.0/8"))
			{
				this.httpProxyState.httpAuthUser = "";
				this.httpProxyState.httpAuthPass = "";
			}
			else
			{
				this.httpProxyState.httpAuthUser = this._config.authUser;
				this.httpProxyState.httpAuthPass = this._config.authPass;
			}
			int num = this.httpProxyState.HandshakeReceive(this._firstPacket, this._firstPacketLength, ref this._remoteHeaderSendBuffer);
			if (num == 1)
			{
				this._connection.BeginReceive(this._connetionRecvBuffer, 0, this._firstPacket.Length, SocketFlags.None, new AsyncCallback(this.HttpHandshakeRecv), null);
				return;
			}
			if (num == 2)
			{
				string s = this.httpProxyState.Http407();
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				this._connection.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, new AsyncCallback(this.HttpHandshakeAuthEndSend), null);
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
				this._connection.BeginSend(bytes2, 0, bytes2.Length, SocketFlags.None, new AsyncCallback(this.StartConnect), null);
				return;
			}
			if (num == 500)
			{
				string s3 = this.httpProxyState.Http500();
				byte[] bytes3 = Encoding.UTF8.GetBytes(s3);
				this._connection.BeginSend(bytes3, 0, bytes3.Length, SocketFlags.None, new AsyncCallback(this.HttpHandshakeAuthEndSend), null);
			}
		}

		// Token: 0x06000262 RID: 610 RVA: 0x00017348 File Offset: 0x00015548
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
				this._remoteHeaderSendBuffer = new byte[2 + range2.Count + 2];
				this._remoteHeaderSendBuffer[0] = 3;
				this._remoteHeaderSendBuffer[1] = (byte)range2.Count;
				Array.Copy(range2.ToArray(), 0, this._remoteHeaderSendBuffer, 2, range2.Count);
				this._remoteHeaderSendBuffer[2 + range2.Count] = range[2];
				this._remoteHeaderSendBuffer[2 + range2.Count + 1] = range[3];
			}
			else
			{
				for (int k = 0; k < 4; k++)
				{
					range.Add(this._firstPacket[4 + k]);
				}
				this._remoteHeaderSendBuffer = new byte[7];
				this._remoteHeaderSendBuffer[0] = 1;
				Array.Copy(range.ToArray(), 4, this._remoteHeaderSendBuffer, 1, 4);
				this._remoteHeaderSendBuffer[5] = range[2];
				this._remoteHeaderSendBuffer[6] = range[3];
			}
			this.command = 1;
			this._connection.BeginSend(range.ToArray(), 0, range.Count, SocketFlags.None, new AsyncCallback(this.StartConnect), null);
		}

		// Token: 0x06000263 RID: 611 RVA: 0x00017510 File Offset: 0x00015710
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
			if (this._config.authUser != null && this._config.authUser.Length > 0 && !Utils.isMatchSubNet(((IPEndPoint)this._connection.RemoteEndPoint).Address, "127.0.0.0/8"))
			{
				array[1] = 2;
				this._connection.BeginSend(array, 0, array.Length, SocketFlags.None, new AsyncCallback(this.HandshakeAuthSendCallback), null);
				return;
			}
			this._connection.BeginSend(array, 0, array.Length, SocketFlags.None, new AsyncCallback(this.HandshakeSendCallback), null);
		}

		// Token: 0x06000269 RID: 617 RVA: 0x00017AC4 File Offset: 0x00015CC4
		private void RspSocks5TCPHeader()
		{
			if (this._connection.AddressFamily == AddressFamily.InterNetwork)
			{
				byte[] expr_15 = new byte[10];
				expr_15[0] = 5;
				expr_15[3] = 1;
				byte[] array = expr_15;
				this._connection.BeginSend(array, 0, array.Length, SocketFlags.None, new AsyncCallback(this.StartConnect), null);
				return;
			}
			byte[] expr_45 = new byte[22];
			expr_45[0] = 5;
			expr_45[3] = 4;
			byte[] array2 = expr_45;
			this._connection.BeginSend(array2, 0, array2.Length, SocketFlags.None, new AsyncCallback(this.StartConnect), null);
		}

		// Token: 0x06000268 RID: 616 RVA: 0x00017894 File Offset: 0x00015A94
		private void RspSocks5UDPHeader(int bytesRead)
		{
			bool flag = this._connection.AddressFamily == AddressFamily.InterNetworkV6;
			int num = 0;
			if (bytesRead >= 9)
			{
				flag = (this._remoteHeaderSendBuffer[0] == 4);
				if (!flag)
				{
					num = (int)this._remoteHeaderSendBuffer[5] * 256 + (int)this._remoteHeaderSendBuffer[6];
				}
				else
				{
					num = (int)this._remoteHeaderSendBuffer[17] * 256 + (int)this._remoteHeaderSendBuffer[18];
				}
			}
			if (!flag)
			{
				this._remoteHeaderSendBuffer = new byte[7];
				this._remoteHeaderSendBuffer[0] = 9;
				this._remoteHeaderSendBuffer[5] = (byte)(num / 256);
				this._remoteHeaderSendBuffer[6] = (byte)(num % 256);
			}
			else
			{
				this._remoteHeaderSendBuffer = new byte[19];
				this._remoteHeaderSendBuffer[0] = 12;
				this._remoteHeaderSendBuffer[17] = (byte)(num / 256);
				this._remoteHeaderSendBuffer[18] = (byte)(num % 256);
			}
			int i = 0;
			IPAddress iPAddress = flag ? IPAddress.IPv6Any : IPAddress.Any;
			this._connectionUDP = new Socket(iPAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
			while (i < 65536)
			{
				try
				{
					this._connectionUDP.Bind(new IPEndPoint(iPAddress, i));
					break;
				}
				catch (Exception)
				{
				}
				i++;
			}
			i = ((IPEndPoint)this._connectionUDP.LocalEndPoint).Port;
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
				Array.Copy(((IPEndPoint)this._connection.LocalEndPoint).Address.GetAddressBytes(), 0, array, 4, 4);
				this._connection.BeginSend(array, 0, array.Length, SocketFlags.None, new AsyncCallback(this.StartConnect), null);
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
			Array.Copy(((IPEndPoint)this._connection.LocalEndPoint).Address.GetAddressBytes(), 0, array2, 4, 16);
			this._connection.BeginSend(array2, 0, array2.Length, SocketFlags.None, new AsyncCallback(this.StartConnect), null);
		}

		// Token: 0x0600026D RID: 621 RVA: 0x00017DD4 File Offset: 0x00015FD4
		private void StartConnect(IAsyncResult ar)
		{
			try
			{
				this._connection.EndSend(ar);
				this.Connect();
			}
			catch (Exception arg_15_0)
			{
				Logging.LogUsefulException(arg_15_0);
				this.Close();
			}
		}

		// Token: 0x040001DF RID: 479
		public byte command;

		// Token: 0x040001E1 RID: 481
		protected HttpPraser httpProxyState;

		// Token: 0x040001DD RID: 477
		protected const int RECV_SIZE = 16384;

		// Token: 0x040001D7 RID: 471
		private Configuration _config;

		// Token: 0x040001DB RID: 475
		private Socket _connection;

		// Token: 0x040001DC RID: 476
		private Socket _connectionUDP;

		// Token: 0x040001DE RID: 478
		protected byte[] _connetionRecvBuffer = new byte[32768];

		// Token: 0x040001D9 RID: 473
		private byte[] _firstPacket;

		// Token: 0x040001DA RID: 474
		private int _firstPacketLength;

		// Token: 0x040001E0 RID: 480
		protected byte[] _remoteHeaderSendBuffer;

		// Token: 0x040001D8 RID: 472
		private ServerTransferTotal _transfer;
	}
}
