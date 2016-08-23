using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Shadowsocks.Encryption;
using Shadowsocks.Model;
using Shadowsocks.Obfs;

namespace Shadowsocks.Controller
{
	// Token: 0x02000049 RID: 73
	internal class ProxySocket
	{
		// Token: 0x06000272 RID: 626 RVA: 0x000180B4 File Offset: 0x000162B4
		public ProxySocket(AddressFamily af, SocketType type, ProtocolType protocol)
		{
			this._socket = new Socket(af, type, protocol);
		}

		// Token: 0x06000286 RID: 646 RVA: 0x000186A1 File Offset: 0x000168A1
		protected void AddRemoteUDPRecvBufferHeader(byte[] decryptBuffer, byte[] remoteSendBuffer, ref int bytesToSend)
		{
			Array.Copy(decryptBuffer, 0, remoteSendBuffer, 3, bytesToSend);
			remoteSendBuffer[0] = 0;
			remoteSendBuffer[1] = 0;
			remoteSendBuffer[2] = 0;
			bytesToSend += 3;
		}

		// Token: 0x06000279 RID: 633 RVA: 0x000181E8 File Offset: 0x000163E8
		public IAsyncResult BeginConnect(EndPoint ep, AsyncCallback callback, object state)
		{
			this._close = false;
			this._socketEndPoint = ep;
			return this._socket.BeginConnect(ep, callback, state);
		}

		// Token: 0x0600027F RID: 639 RVA: 0x00018320 File Offset: 0x00016520
		public IAsyncResult BeginReceive(byte[] buffer, int size, SocketFlags flags, AsyncCallback callback, object state)
		{
			CallbackState callbackState = new CallbackState();
			callbackState.buffer = buffer;
			callbackState.size = size;
			callbackState.state = state;
			return this._socket.BeginReceive(buffer, 0, size, flags, callback, callbackState);
		}

		// Token: 0x06000283 RID: 643 RVA: 0x000184F0 File Offset: 0x000166F0
		public IAsyncResult BeginReceiveFrom(byte[] buffer, int size, SocketFlags flags, ref EndPoint ep, AsyncCallback callback, object state)
		{
			CallbackState callbackState = new CallbackState();
			callbackState.buffer = buffer;
			callbackState.size = size;
			callbackState.state = state;
			return this._socket.BeginReceiveFrom(buffer, 0, size, flags, ref ep, callback, callbackState);
		}

		// Token: 0x06000281 RID: 641 RVA: 0x00018434 File Offset: 0x00016634
		public int BeginSend(byte[] buffer, int size, SocketFlags flags, AsyncCallback callback, object state)
		{
			CallbackState callbackState = new CallbackState();
			callbackState.size = size;
			callbackState.state = state;
			int datalength = 0;
			object encryptionLock = this._encryptionLock;
			int num2;
			byte[] buffer2;
			lock (encryptionLock)
			{
				int num;
				byte[] buf = this._protocol.ClientPreEncrypt(buffer, size, out num);
				byte[] array = new byte[num + 64];
				this._encryptor.Encrypt(buf, num, array, out datalength);
				buffer2 = this._obfs.ClientEncode(array, datalength, out num2);
			}
			this._socket.BeginSend(buffer2, 0, num2, SocketFlags.None, callback, callbackState);
			return num2;
		}

		// Token: 0x06000288 RID: 648 RVA: 0x000187B4 File Offset: 0x000169B4
		public int BeginSendTo(byte[] buffer, int size, SocketFlags flags, EndPoint ep, AsyncCallback callback, object state)
		{
			CallbackState callbackState = new CallbackState();
			callbackState.buffer = buffer;
			callbackState.size = size;
			callbackState.state = state;
			byte[] array = new byte[65536];
			int num = 3;
			int num2 = size - num;
			byte[] array2 = new byte[num2];
			Array.Copy(buffer, num, array2, 0, num2);
			object encryptionLock = this._encryptionLock;
			int num3;
			lock (encryptionLock)
			{
				this._encryptor.ResetEncrypt();
				this._protocol.SetServerInfoIV(this._encryptor.getIV());
				int length;
				byte[] buf = this._protocol.ClientUdpPreEncrypt(array2, num2, out length);
				this._encryptor.Encrypt(buf, length, array, out num3);
			}
			if (this._proxy)
			{
				string proxy_server = this._proxy_server;
				int proxy_udp_port = this._proxy_udp_port;
				IPAddress iPAddress;
				if (!IPAddress.TryParse(proxy_server, out iPAddress))
				{
					array2 = new byte[num + 1 + 1 + proxy_server.Length + 2 + num3];
					Array.Copy(array, 0, array2, num + 1 + 1 + proxy_server.Length + 2, num3);
					array2[0] = 0;
					array2[1] = 0;
					array2[2] = 0;
					array2[3] = 3;
					array2[4] = (byte)proxy_server.Length;
					for (int i = 0; i < proxy_server.Length; i++)
					{
						array2[5 + i] = (byte)proxy_server[i];
					}
					array2[5 + proxy_server.Length] = (byte)(proxy_udp_port / 256);
					array2[5 + proxy_server.Length + 1] = (byte)(proxy_udp_port % 256);
				}
				else
				{
					byte[] addressBytes = iPAddress.GetAddressBytes();
					array2 = new byte[num + 1 + addressBytes.Length + 2 + num3];
					Array.Copy(array, 0, array2, num + 1 + addressBytes.Length + 2, num3);
					array2[0] = 0;
					array2[1] = 0;
					array2[2] = 0;
                    array2[3] = (byte)((iPAddress.AddressFamily == AddressFamily.InterNetworkV6) ? 4 : 1);
					for (int j = 0; j < addressBytes.Length; j++)
					{
						array2[4 + j] = addressBytes[j];
					}
					array2[4 + addressBytes.Length] = (byte)(proxy_udp_port / 256);
					array2[4 + addressBytes.Length + 1] = (byte)(proxy_udp_port % 256);
				}
				num3 = array2.Length;
				Array.Copy(array2, array, num3);
			}
			this._socket.BeginSendTo(array, 0, num3, flags, ep, callback, callbackState);
			return num3;
		}

		// Token: 0x06000278 RID: 632 RVA: 0x00018134 File Offset: 0x00016334
		public void Close()
		{
			this._socket.Close();
			if (this._protocol != null)
			{
				this._protocol.Dispose();
				this._protocol = null;
			}
			if (this._obfs != null)
			{
				this._obfs.Dispose();
				this._obfs = null;
			}
			object encryptionLock = this._encryptionLock;
			lock (encryptionLock)
			{
				object decryptionLock = this._decryptionLock;
				lock (decryptionLock)
				{
					if (this._encryptor != null)
					{
						this._encryptor.Dispose();
					}
				}
			}
		}

		// Token: 0x0600028F RID: 655 RVA: 0x00018E0C File Offset: 0x0001700C
		public bool ConnectHttpProxyServer(string strRemoteHost, int iRemotePort, string socks5RemoteUsername, string socks5RemotePassword, string proxyUserAgent)
		{
			this._proxy = true;
			IPAddress iPAddress;
			IPAddress.TryParse(strRemoteHost, out iPAddress);
			if (iPAddress != null)
			{
				strRemoteHost = iPAddress.ToString();
			}
			string text = ((strRemoteHost.IndexOf(':') >= 0) ? ("[" + strRemoteHost + "]") : strRemoteHost) + ":" + iRemotePort.ToString();
			string str = Convert.ToBase64String(Encoding.UTF8.GetBytes(socks5RemoteUsername + ":" + socks5RemotePassword));
			string text2 = string.Concat(new string[]
			{
				"CONNECT ",
				text,
				" HTTP/1.0\r\nHost: ",
				text,
				"\r\n"
			});
			if (proxyUserAgent != null && proxyUserAgent.Length > 0)
			{
				text2 = text2 + "User-Agent: " + proxyUserAgent + "\r\n";
			}
			text2 += "Proxy-Connection: Keep-Alive\r\n";
			if (socks5RemoteUsername.Length > 0)
			{
				text2 = text2 + "Proxy-Authorization: Basic " + str + "\r\n";
			}
			text2 += "\r\n";
			byte[] bytes = Encoding.UTF8.GetBytes(text2);
			Socket arg_101_0 = this._socket;
			byte[] expr_FD = bytes;
			arg_101_0.Send(expr_FD, expr_FD.Length, SocketFlags.None);
			byte[] array = new byte[1024];
			Socket arg_11F_0 = this._socket;
			byte[] expr_11B = array;
			int num = arg_11F_0.Receive(expr_11B, expr_11B.Length, SocketFlags.None);
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

		// Token: 0x0600028C RID: 652 RVA: 0x00018A24 File Offset: 0x00016C24
		public bool ConnectSocks5ProxyServer(string strRemoteHost, int iRemotePort, bool udp, string socks5RemoteUsername, string socks5RemotePassword)
		{
			int errorCode = 10054;
			this._proxy = true;
			byte[] array = new byte[10];
			array[0] = 5;
			array[1] = 2;
			array[2] = 0;
			array[3] = 2;
			this._socket.Send(array, 4, SocketFlags.None);
			byte[] array2 = new byte[32];
			Socket arg_47_0 = this._socket;
			byte[] expr_43 = array2;
			if (arg_47_0.Receive(expr_43, expr_43.Length, SocketFlags.None) < 2)
			{
				throw new SocketException(errorCode);
			}
			if (array2[0] != 5 || (array2[1] != 0 && array2[1] != 2))
			{
				throw new SocketException(errorCode);
			}
			if (array2[1] != 0)
			{
				if (array2[1] != 2)
				{
					return false;
				}
				if (socks5RemoteUsername.Length == 0)
				{
					throw new SocketException(errorCode);
				}
				array = new byte[socks5RemoteUsername.Length + socks5RemotePassword.Length + 3];
				array[0] = 1;
				array[1] = (byte)socks5RemoteUsername.Length;
				for (int i = 0; i < socks5RemoteUsername.Length; i++)
				{
					array[2 + i] = (byte)socks5RemoteUsername[i];
				}
				array[socks5RemoteUsername.Length + 2] = (byte)socks5RemotePassword.Length;
				for (int j = 0; j < socks5RemotePassword.Length; j++)
				{
					array[socks5RemoteUsername.Length + 3 + j] = (byte)socks5RemotePassword[j];
				}
				Socket arg_121_0 = this._socket;
				byte[] expr_11D = array;
				arg_121_0.Send(expr_11D, expr_11D.Length, SocketFlags.None);
				Socket arg_132_0 = this._socket;
				byte[] expr_12E = array2;
				arg_132_0.Receive(expr_12E, expr_12E.Length, SocketFlags.None);
				if (array2[0] != 1 || array2[1] != 0)
				{
					throw new SocketException(10061);
				}
			}
			if (!udp)
			{
				List<byte> list = new List<byte>();
				list.Add(5);
				list.Add(1);
				list.Add(0);
				IPAddress iPAddress;
				IPAddress.TryParse(strRemoteHost, out iPAddress);
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
				this._socket.Send(list.ToArray(), list.Count, SocketFlags.None);
				Socket arg_262_0 = this._socket;
				byte[] expr_25E = array2;
				if (arg_262_0.Receive(expr_25E, expr_25E.Length, SocketFlags.None) < 2 || array2[0] != 5 || array2[1] != 0)
				{
					throw new SocketException(errorCode);
				}
				return true;
			}
			else
			{
				List<byte> list2 = new List<byte>();
				list2.Add(5);
				list2.Add(3);
				list2.Add(0);
				IPAddress iPAddress2 = ((IPEndPoint)this._socketEndPoint).Address;
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
				this._socket.Send(list2.ToArray(), list2.Count, SocketFlags.None);
				Socket arg_344_0 = this._socket;
				byte[] expr_340 = array2;
				arg_344_0.Receive(expr_340, expr_340.Length, SocketFlags.None);
				if (array2[0] != 5 || array2[1] != 0)
				{
					throw new SocketException(errorCode);
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
				this._remoteUDPEndPoint = new IPEndPoint(iPAddress2, port);
				return true;
			}
		}

		// Token: 0x0600027A RID: 634 RVA: 0x00018206 File Offset: 0x00016406
		public void EndConnect(IAsyncResult ar)
		{
			this._socket.EndConnect(ar);
		}

		// Token: 0x06000280 RID: 640 RVA: 0x0001835C File Offset: 0x0001655C
		public int EndReceive(IAsyncResult ar, out bool sendback)
		{
			int num = this._socket.EndReceive(ar);
			sendback = false;
			if (num > 0)
			{
				CallbackState callbackState = (CallbackState)ar.AsyncState;
				callbackState.size = num;
				object decryptionLock = this._decryptionLock;
				lock (decryptionLock)
				{
					int datalength = 0;
					int num2;
					byte[] array = this._obfs.ClientDecode(callbackState.buffer, num, out num2, out sendback);
					byte[] array2 = new byte[array.Length];
					if (num2 > 0)
					{
						this._encryptor.Decrypt(array, num2, array2, out datalength);
						int num3;
						array2 = this._protocol.ClientPostDecrypt(array2, datalength, out num3);
						Array.Copy(array2, 0, callbackState.buffer, 0, num3);
						return num3;
					}
				}
				return 0;
			}
			this._close = true;
			return num;
		}

		// Token: 0x06000287 RID: 647 RVA: 0x000186C0 File Offset: 0x000168C0
		public int EndReceiveFrom(IAsyncResult ar, ref EndPoint ep)
		{
			int num = this._socket.EndReceiveFrom(ar, ref ep);
			if (num <= 0)
			{
				this._close = true;
				return num;
			}
			CallbackState callbackState = (CallbackState)ar.AsyncState;
			callbackState.size = num;
			if (!this.RemoveRemoteUDPRecvBufferHeader(callbackState.buffer, ref num))
			{
				return 0;
			}
			byte[] array = new byte[65536];
			object decryptionLock = this._decryptionLock;
			int num2;
			byte[] sourceArray;
			lock (decryptionLock)
			{
				byte[] array2 = new byte[65536];
				this._encryptor.ResetDecrypt();
				int datalength;
				this._encryptor.Decrypt(callbackState.buffer, num, array2, out datalength);
				array2 = ProxySocket.ParseUDPHeader(array2, ref datalength);
				this.AddRemoteUDPRecvBufferHeader(array2, array, ref datalength);
				sourceArray = this._protocol.ClientUdpPostDecrypt(array, datalength, out num2);
			}
			Array.Copy(sourceArray, 0, callbackState.buffer, 0, num2);
			return num2;
		}

		// Token: 0x06000282 RID: 642 RVA: 0x000184E0 File Offset: 0x000166E0
		public int EndSend(IAsyncResult ar)
		{
			return this._socket.EndSend(ar);
		}

		// Token: 0x06000289 RID: 649 RVA: 0x000189FC File Offset: 0x00016BFC
		public int EndSendTo(IAsyncResult ar)
		{
			return this._socket.EndSendTo(ar);
		}

		// Token: 0x0600028A RID: 650 RVA: 0x00018A0A File Offset: 0x00016C0A
		public int GetAsyncResultSize(IAsyncResult ar)
		{
			return ((CallbackState)ar.AsyncState).size;
		}

		// Token: 0x0600028B RID: 651 RVA: 0x00018A1C File Offset: 0x00016C1C
		public IPEndPoint GetProxyUdpEndPoint()
		{
			return this._remoteUDPEndPoint;
		}

		// Token: 0x06000273 RID: 627 RVA: 0x000180E0 File Offset: 0x000162E0
		public Socket GetSocket()
		{
			return this._socket;
		}

		// Token: 0x06000285 RID: 645 RVA: 0x000185D0 File Offset: 0x000167D0
		protected static byte[] ParseUDPHeader(byte[] buffer, ref int len)
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

		// Token: 0x06000284 RID: 644 RVA: 0x00018530 File Offset: 0x00016730
		private bool RemoveRemoteUDPRecvBufferHeader(byte[] remoteRecvBuffer, ref int bytesRead)
		{
			if (this._proxy)
			{
				if (bytesRead < 7)
				{
					return false;
				}
				if (remoteRecvBuffer[3] == 1)
				{
					int num = 10;
					bytesRead -= num;
					byte arg_26_0 = remoteRecvBuffer[num - 2];
					byte arg_2C_0 = remoteRecvBuffer[num - 1];
					Array.Copy(remoteRecvBuffer, num, remoteRecvBuffer, 0, bytesRead);
				}
				else if (remoteRecvBuffer[3] == 4)
				{
					int num2 = 22;
					bytesRead -= num2;
					byte arg_4E_0 = remoteRecvBuffer[num2 - 2];
					byte arg_54_0 = remoteRecvBuffer[num2 - 1];
					Array.Copy(remoteRecvBuffer, num2, remoteRecvBuffer, 0, bytesRead);
				}
				else
				{
					if (remoteRecvBuffer[3] != 3)
					{
						return false;
					}
					int num3 = (int)(5 + remoteRecvBuffer[4] + 2);
					bytesRead -= num3;
					byte arg_7B_0 = remoteRecvBuffer[num3 - 2];
					byte arg_81_0 = remoteRecvBuffer[num3 - 1];
					Array.Copy(remoteRecvBuffer, num3, remoteRecvBuffer, 0, bytesRead);
				}
			}
			return true;
		}

		// Token: 0x0600027B RID: 635 RVA: 0x00018214 File Offset: 0x00016414
		public void SetEncryptor(IEncryptor encryptor)
		{
			this._encryptor = encryptor;
		}

		// Token: 0x0600027D RID: 637 RVA: 0x00018226 File Offset: 0x00016426
		public void SetObfs(IObfs obfs)
		{
			this._obfs = obfs;
		}

		// Token: 0x0600027E RID: 638 RVA: 0x00018230 File Offset: 0x00016430
		public void SetObfsPlugin(Server server, int head_len)
		{
			lock (server)
			{
				if (server.getProtocolData() == null)
				{
					server.setProtocolData(this._protocol.InitData());
				}
				if (server.getObfsData() == null)
				{
					server.setObfsData(this._obfs.InitData());
				}
			}
			int tcp_mss = 1460;
			string host = server.server;
			host = this._proxy_server;
			this._protocol.SetServerInfo(new ServerInfo(host, server.server_port, "", server.getProtocolData(), this._encryptor.getIV(), this._encryptor.getKey(), head_len, tcp_mss));
			this._obfs.SetServerInfo(new ServerInfo(host, server.server_port, server.obfsparam, server.getObfsData(), this._encryptor.getIV(), this._encryptor.getKey(), head_len, tcp_mss));
		}

		// Token: 0x0600027C RID: 636 RVA: 0x0001821D File Offset: 0x0001641D
		public void SetProtocol(IObfs protocol)
		{
			this._protocol = protocol;
		}

		// Token: 0x0600028D RID: 653 RVA: 0x00018DF9 File Offset: 0x00016FF9
		public void SetTcpServer(string server, int port)
		{
			this._proxy_server = server;
			this._proxy_udp_port = port;
		}

		// Token: 0x0600028E RID: 654 RVA: 0x00018DF9 File Offset: 0x00016FF9
		public void SetUdpServer(string server, int port)
		{
			this._proxy_server = server;
			this._proxy_udp_port = port;
		}

		// Token: 0x06000277 RID: 631 RVA: 0x00018125 File Offset: 0x00016325
		public void Shutdown(SocketShutdown how)
		{
			this._socket.Shutdown(how);
		}

		// Token: 0x1700001E RID: 30
		public AddressFamily AddressFamily
		{
			// Token: 0x06000276 RID: 630 RVA: 0x00018118 File Offset: 0x00016318
			get
			{
				return this._socket.AddressFamily;
			}
		}

		// Token: 0x1700001D RID: 29
		public bool CanSendKeepAlive
		{
			// Token: 0x06000275 RID: 629 RVA: 0x000180F0 File Offset: 0x000162F0
			get
			{
				Dictionary<string, int> expr_05 = new Dictionary<string, int>();
				expr_05["auth_sha1_v2"] = 1;
				return expr_05.ContainsKey(this._protocol.Name());
			}
		}

		// Token: 0x1700001C RID: 28
		public bool IsClose
		{
			// Token: 0x06000274 RID: 628 RVA: 0x000180E8 File Offset: 0x000162E8
			get
			{
				return this._close;
			}
		}

		// Token: 0x040001F0 RID: 496
		protected bool _close;

		// Token: 0x040001EA RID: 490
		protected object _decryptionLock = new object();

		// Token: 0x040001E9 RID: 489
		protected object _encryptionLock = new object();

		// Token: 0x040001E8 RID: 488
		protected IEncryptor _encryptor;

		// Token: 0x040001EC RID: 492
		public IObfs _obfs;

		// Token: 0x040001EB RID: 491
		public IObfs _protocol;

		// Token: 0x040001ED RID: 493
		protected bool _proxy;

		// Token: 0x040001EE RID: 494
		protected string _proxy_server;

		// Token: 0x040001EF RID: 495
		protected int _proxy_udp_port;

		// Token: 0x040001E7 RID: 487
		protected IPEndPoint _remoteUDPEndPoint;

		// Token: 0x040001E5 RID: 485
		protected Socket _socket;

		// Token: 0x040001E6 RID: 486
		protected EndPoint _socketEndPoint;
	}
}
