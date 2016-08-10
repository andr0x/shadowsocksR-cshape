using System;
using System.Net;
using System.Net.Sockets;

namespace Shadowsocks.Controller
{
	// Token: 0x02000047 RID: 71
	internal class PortForwarder : Listener.Service
	{
		// Token: 0x0600026F RID: 623 RVA: 0x0001797E File Offset: 0x00015B7E
		public PortForwarder(int targetPort)
		{
			this._targetPort = targetPort;
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0001798D File Offset: 0x00015B8D
		public bool Handle(byte[] firstPacket, int length, Socket socket)
		{
			new PortForwarder.Handler().Start(firstPacket, length, socket, this._targetPort);
			return true;
		}

		// Token: 0x040001E0 RID: 480
		private int _targetPort;

		// Token: 0x020000AB RID: 171
		private class Handler
		{
			// Token: 0x06000574 RID: 1396 RVA: 0x0002C854 File Offset: 0x0002AA54
			private void CheckClose()
			{
				if (this._localShutdown && this._remoteShutdown)
				{
					this.Close();
				}
			}

			// Token: 0x06000575 RID: 1397 RVA: 0x0002C86C File Offset: 0x0002AA6C
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

			// Token: 0x0600056D RID: 1389 RVA: 0x0002C53C File Offset: 0x0002A73C
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

			// Token: 0x0600056E RID: 1390 RVA: 0x0002C584 File Offset: 0x0002A784
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

			// Token: 0x06000571 RID: 1393 RVA: 0x0002C6F8 File Offset: 0x0002A8F8
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

			// Token: 0x06000573 RID: 1395 RVA: 0x0002C7E8 File Offset: 0x0002A9E8
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

			// Token: 0x06000570 RID: 1392 RVA: 0x0002C674 File Offset: 0x0002A874
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

			// Token: 0x06000572 RID: 1394 RVA: 0x0002C77C File Offset: 0x0002A97C
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

			// Token: 0x0600056C RID: 1388 RVA: 0x0002C4A8 File Offset: 0x0002A6A8
			public void Start(byte[] firstPacket, int length, Socket socket, int targetPort)
			{
				this._firstPacket = firstPacket;
				this._firstPacketLength = length;
				this._local = socket;
				try
				{
					IPAddress iPAddress;
					IPAddress.TryParse("127.0.0.1", out iPAddress);
					IPEndPoint remoteEP = new IPEndPoint(iPAddress, targetPort);
					this._remote = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
					this._remote.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, true);
					this._remote.BeginConnect(remoteEP, new AsyncCallback(this.ConnectCallback), null);
				}
				catch (Exception arg_68_0)
				{
					Logging.LogUsefulException(arg_68_0);
					this.Close();
				}
			}

			// Token: 0x0600056F RID: 1391 RVA: 0x0002C5E4 File Offset: 0x0002A7E4
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

			// Token: 0x0400045B RID: 1115
			private byte[] connetionRecvBuffer = new byte[16384];

			// Token: 0x04000459 RID: 1113
			public const int RecvSize = 16384;

			// Token: 0x0400045A RID: 1114
			private byte[] remoteRecvBuffer = new byte[16384];

			// Token: 0x04000456 RID: 1110
			private bool _closed;

			// Token: 0x04000452 RID: 1106
			private byte[] _firstPacket;

			// Token: 0x04000453 RID: 1107
			private int _firstPacketLength;

			// Token: 0x04000454 RID: 1108
			private Socket _local;

			// Token: 0x04000457 RID: 1111
			private bool _localShutdown;

			// Token: 0x04000455 RID: 1109
			private Socket _remote;

			// Token: 0x04000458 RID: 1112
			private bool _remoteShutdown;
		}
	}
}
