using System;
using System.Net;
using System.Net.Sockets;

namespace Shadowsocks.Controller
{
	// Token: 0x02000045 RID: 69
	internal class PortForwarder : Listener.Service
	{
		// Token: 0x0600025A RID: 602 RVA: 0x00017102 File Offset: 0x00015302
		public PortForwarder(int targetPort)
		{
			this._targetPort = targetPort;
		}

		// Token: 0x0600025B RID: 603 RVA: 0x00017111 File Offset: 0x00015311
		public bool Handle(byte[] firstPacket, int length, Socket socket)
		{
			new PortForwarder.Handler().Start(firstPacket, length, socket, this._targetPort);
			return true;
		}

		// Token: 0x040001D6 RID: 470
		private int _targetPort;

		// Token: 0x020000AD RID: 173
		private class Handler
		{
			// Token: 0x0600057D RID: 1405 RVA: 0x0002BA9C File Offset: 0x00029C9C
			private void CheckClose()
			{
				if (this._localShutdown && this._remoteShutdown)
				{
					this.Close();
				}
			}

			// Token: 0x0600057E RID: 1406 RVA: 0x0002BAB4 File Offset: 0x00029CB4
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

			// Token: 0x06000576 RID: 1398 RVA: 0x0002B784 File Offset: 0x00029984
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

			// Token: 0x06000577 RID: 1399 RVA: 0x0002B7CC File Offset: 0x000299CC
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

			// Token: 0x0600057A RID: 1402 RVA: 0x0002B940 File Offset: 0x00029B40
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

			// Token: 0x0600057C RID: 1404 RVA: 0x0002BA30 File Offset: 0x00029C30
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

			// Token: 0x06000579 RID: 1401 RVA: 0x0002B8BC File Offset: 0x00029ABC
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

			// Token: 0x0600057B RID: 1403 RVA: 0x0002B9C4 File Offset: 0x00029BC4
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

			// Token: 0x06000575 RID: 1397 RVA: 0x0002B6F0 File Offset: 0x000298F0
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

			// Token: 0x06000578 RID: 1400 RVA: 0x0002B82C File Offset: 0x00029A2C
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

			// Token: 0x04000458 RID: 1112
			private byte[] connetionRecvBuffer = new byte[16384];

			// Token: 0x04000456 RID: 1110
			public const int RecvSize = 16384;

			// Token: 0x04000457 RID: 1111
			private byte[] remoteRecvBuffer = new byte[16384];

			// Token: 0x04000453 RID: 1107
			private bool _closed;

			// Token: 0x0400044F RID: 1103
			private byte[] _firstPacket;

			// Token: 0x04000450 RID: 1104
			private int _firstPacketLength;

			// Token: 0x04000451 RID: 1105
			private Socket _local;

			// Token: 0x04000454 RID: 1108
			private bool _localShutdown;

			// Token: 0x04000452 RID: 1106
			private Socket _remote;

			// Token: 0x04000455 RID: 1109
			private bool _remoteShutdown;
		}
	}
}
