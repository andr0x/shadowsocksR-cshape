using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Shadowsocks.Model;
using Shadowsocks.Util;

namespace Shadowsocks.Controller
{
	// Token: 0x0200003F RID: 63
	internal class HttpPortForwarder : Listener.Service
	{
		// Token: 0x0600023A RID: 570 RVA: 0x00015E82 File Offset: 0x00014082
		public HttpPortForwarder(int targetPort, Configuration config)
		{
			this._targetPort = targetPort;
			this._config = config;
		}

		// Token: 0x0600023B RID: 571 RVA: 0x00015E98 File Offset: 0x00014098
		public bool Handle(byte[] firstPacket, int length, Socket socket)
		{
			new HttpPortForwarder.Handler().Start(this._config, firstPacket, length, socket, this._targetPort);
			return true;
		}

		// Token: 0x040001BB RID: 443
		private Configuration _config;

		// Token: 0x040001BA RID: 442
		private int _targetPort;

		// Token: 0x020000A7 RID: 167
		private class Handler
		{
			// Token: 0x06000565 RID: 1381 RVA: 0x0002C3A0 File Offset: 0x0002A5A0
			private void CheckClose()
			{
				if (this._localShutdown && this._remoteShutdown)
				{
					this.Close();
				}
			}

			// Token: 0x06000566 RID: 1382 RVA: 0x0002C3B8 File Offset: 0x0002A5B8
			public void Close()
			{
				lock (this)
				{
					if (this._closed)
					{
						return;
					}
					this._closed = true;
				}
				if (this._local != null)
				{
					try
					{
						this._local.Shutdown(SocketShutdown.Both);
						this._local.Close();
					}
					catch (Exception arg_4A_0)
					{
						Logging.LogUsefulException(arg_4A_0);
					}
				}
				if (this._remote != null)
				{
					try
					{
						this._remote.Shutdown(SocketShutdown.Both);
						this._remote.Close();
					}
					catch (SocketException arg_72_0)
					{
						Logging.LogUsefulException(arg_72_0);
					}
				}
			}

			// Token: 0x0600055D RID: 1373 RVA: 0x0002C004 File Offset: 0x0002A204
			private void Connect()
			{
				try
				{
					IPAddress iPAddress;
					IPAddress.TryParse("127.0.0.1", out iPAddress);
					IPEndPoint remoteEP = new IPEndPoint(iPAddress, this._targetPort);
					this._remote = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
					this._remote.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, true);
					this._remote.BeginConnect(remoteEP, new AsyncCallback(this.ConnectCallback), null);
				}
				catch (Exception arg_57_0)
				{
					Logging.LogUsefulException(arg_57_0);
					this.Close();
				}
			}

			// Token: 0x0600055E RID: 1374 RVA: 0x0002C088 File Offset: 0x0002A288
			private void ConnectCallback(IAsyncResult ar)
			{
				if (this._closed)
				{
					return;
				}
				try
				{
					this._remote.EndConnect(ar);
					this.HandshakeReceive();
				}
				catch (Exception arg_1E_0)
				{
					Logging.LogUsefulException(arg_1E_0);
					this.Close();
				}
			}

			// Token: 0x0600055F RID: 1375 RVA: 0x0002C0D0 File Offset: 0x0002A2D0
			private void HandshakeReceive()
			{
				if (this._closed)
				{
					return;
				}
				try
				{
					this._remote.BeginSend(this._firstPacket, 0, this._firstPacketLength, SocketFlags.None, new AsyncCallback(this.StartPipe), null);
				}
				catch (Exception arg_33_0)
				{
					Logging.LogUsefulException(arg_33_0);
					this.Close();
				}
			}

			// Token: 0x0600055B RID: 1371 RVA: 0x0002BF58 File Offset: 0x0002A158
			private void HttpHandshakeAuthEndSend(IAsyncResult ar)
			{
				if (this._closed)
				{
					return;
				}
				try
				{
					this._local.EndSend(ar);
					this._local.BeginReceive(this.connetionRecvBuffer, 0, this._firstPacket.Length, SocketFlags.None, new AsyncCallback(this.HttpHandshakeRecv), null);
				}
				catch (Exception arg_42_0)
				{
					Logging.LogUsefulException(arg_42_0);
					this.Close();
				}
			}

			// Token: 0x0600055A RID: 1370 RVA: 0x0002BEE0 File Offset: 0x0002A0E0
			private void HttpHandshakeRecv(IAsyncResult ar)
			{
				if (this._closed)
				{
					return;
				}
				try
				{
					int num = this._local.EndReceive(ar);
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
				catch (Exception arg_4E_0)
				{
					Logging.LogUsefulException(arg_4E_0);
					this.Close();
				}
			}

			// Token: 0x06000562 RID: 1378 RVA: 0x0002C244 File Offset: 0x0002A444
			private void PipeConnectionReceiveCallback(IAsyncResult ar)
			{
				if (this._closed)
				{
					return;
				}
				try
				{
					int num = this._local.EndReceive(ar);
					if (num > 0)
					{
						this._remote.BeginSend(this.connetionRecvBuffer, 0, num, SocketFlags.None, new AsyncCallback(this.PipeRemoteSendCallback), null);
					}
					else
					{
						this._remote.Shutdown(SocketShutdown.Send);
						this._remoteShutdown = true;
						this.CheckClose();
					}
				}
				catch (Exception arg_5A_0)
				{
					Logging.LogUsefulException(arg_5A_0);
					this.Close();
				}
			}

			// Token: 0x06000564 RID: 1380 RVA: 0x0002C334 File Offset: 0x0002A534
			private void PipeConnectionSendCallback(IAsyncResult ar)
			{
				if (this._closed)
				{
					return;
				}
				try
				{
					this._local.EndSend(ar);
					this._remote.BeginReceive(this.remoteRecvBuffer, 0, 16384, SocketFlags.None, new AsyncCallback(this.PipeRemoteReceiveCallback), null);
				}
				catch (Exception arg_3F_0)
				{
					Logging.LogUsefulException(arg_3F_0);
					this.Close();
				}
			}

			// Token: 0x06000561 RID: 1377 RVA: 0x0002C1C0 File Offset: 0x0002A3C0
			private void PipeRemoteReceiveCallback(IAsyncResult ar)
			{
				if (this._closed)
				{
					return;
				}
				try
				{
					int num = this._remote.EndReceive(ar);
					if (num > 0)
					{
						this._local.BeginSend(this.remoteRecvBuffer, 0, num, SocketFlags.None, new AsyncCallback(this.PipeConnectionSendCallback), null);
					}
					else
					{
						this._local.Shutdown(SocketShutdown.Send);
						this._localShutdown = true;
						this.CheckClose();
					}
				}
				catch (Exception arg_5A_0)
				{
					Logging.LogUsefulException(arg_5A_0);
					this.Close();
				}
			}

			// Token: 0x06000563 RID: 1379 RVA: 0x0002C2C8 File Offset: 0x0002A4C8
			private void PipeRemoteSendCallback(IAsyncResult ar)
			{
				if (this._closed)
				{
					return;
				}
				try
				{
					this._remote.EndSend(ar);
					this._local.BeginReceive(this.connetionRecvBuffer, 0, 16384, SocketFlags.None, new AsyncCallback(this.PipeConnectionReceiveCallback), null);
				}
				catch (Exception arg_3F_0)
				{
					Logging.LogUsefulException(arg_3F_0);
					this.Close();
				}
			}

			// Token: 0x06000559 RID: 1369 RVA: 0x0002BD70 File Offset: 0x00029F70
			private void RspHttpHandshakeReceive()
			{
				if (this.httpProxyState == null)
				{
					this.httpProxyState = new HttpPraser(true);
				}
				this.httpProxyState.httpAuthUser = this._config.authUser;
				this.httpProxyState.httpAuthPass = this._config.authPass;
				byte[] array = null;
				int num = this.httpProxyState.HandshakeReceive(this._firstPacket, this._firstPacketLength, ref array);
				if (num == 1)
				{
					this._local.BeginReceive(this.connetionRecvBuffer, 0, this._firstPacket.Length, SocketFlags.None, new AsyncCallback(this.HttpHandshakeRecv), null);
					return;
				}
				if (num == 2)
				{
					string s = this.httpProxyState.Http407();
					byte[] bytes = Encoding.UTF8.GetBytes(s);
					this._local.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, new AsyncCallback(this.HttpHandshakeAuthEndSend), null);
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
					this._local.BeginSend(bytes2, 0, bytes2.Length, SocketFlags.None, new AsyncCallback(this.StartConnect), null);
					return;
				}
				if (num == 500)
				{
					string s3 = this.httpProxyState.Http500();
					byte[] bytes3 = Encoding.UTF8.GetBytes(s3);
					this._local.BeginSend(bytes3, 0, bytes3.Length, SocketFlags.None, new AsyncCallback(this.HttpHandshakeAuthEndSend), null);
				}
			}

			// Token: 0x06000558 RID: 1368 RVA: 0x0002BCF4 File Offset: 0x00029EF4
			public void Start(Configuration config, byte[] firstPacket, int length, Socket socket, int targetPort)
			{
				this._firstPacket = firstPacket;
				this._firstPacketLength = length;
				this._local = socket;
				this._targetPort = targetPort;
				this._config = config;
				if ((this._config.authUser ?? "").Length == 0 || Utils.isMatchSubNet(((IPEndPoint)this._local.RemoteEndPoint).Address, "127.0.0.0/8"))
				{
					this.Connect();
					return;
				}
				this.RspHttpHandshakeReceive();
			}

			// Token: 0x0600055C RID: 1372 RVA: 0x0002BFC4 File Offset: 0x0002A1C4
			private void StartConnect(IAsyncResult ar)
			{
				try
				{
					this._local.EndSend(ar);
					this.Connect();
				}
				catch (Exception arg_15_0)
				{
					Logging.LogUsefulException(arg_15_0);
					this.Close();
				}
			}

			// Token: 0x06000560 RID: 1376 RVA: 0x0002C130 File Offset: 0x0002A330
			private void StartPipe(IAsyncResult ar)
			{
				if (this._closed)
				{
					return;
				}
				try
				{
					this._remote.EndSend(ar);
					this._remote.BeginReceive(this.remoteRecvBuffer, 0, 16384, SocketFlags.None, new AsyncCallback(this.PipeRemoteReceiveCallback), null);
					this._local.BeginReceive(this.connetionRecvBuffer, 0, 16384, SocketFlags.None, new AsyncCallback(this.PipeConnectionReceiveCallback), null);
				}
				catch (Exception arg_65_0)
				{
					Logging.LogUsefulException(arg_65_0);
					this.Close();
				}
			}

			// Token: 0x0400044B RID: 1099
			private byte[] connetionRecvBuffer = new byte[16384];

			// Token: 0x04000448 RID: 1096
			private HttpPraser httpProxyState;

			// Token: 0x04000449 RID: 1097
			public const int RecvSize = 16384;

			// Token: 0x0400044A RID: 1098
			private byte[] remoteRecvBuffer = new byte[16384];

			// Token: 0x04000444 RID: 1092
			private bool _closed;

			// Token: 0x04000447 RID: 1095
			private Configuration _config;

			// Token: 0x0400043F RID: 1087
			private byte[] _firstPacket;

			// Token: 0x04000440 RID: 1088
			private int _firstPacketLength;

			// Token: 0x04000442 RID: 1090
			private Socket _local;

			// Token: 0x04000445 RID: 1093
			private bool _localShutdown;

			// Token: 0x04000443 RID: 1091
			private Socket _remote;

			// Token: 0x04000446 RID: 1094
			private bool _remoteShutdown;

			// Token: 0x04000441 RID: 1089
			private int _targetPort;
		}
	}
}
