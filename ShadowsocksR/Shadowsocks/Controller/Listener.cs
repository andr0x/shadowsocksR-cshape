using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Timers;
using Shadowsocks.Model;
using Shadowsocks.Util;

namespace Shadowsocks.Controller
{
	// Token: 0x02000041 RID: 65
	public class Listener
	{
		// Token: 0x06000244 RID: 580 RVA: 0x000164CA File Offset: 0x000146CA
		public Listener(IList<Listener.Service> services)
		{
			this._services = services;
			this._stop = false;
		}

		// Token: 0x0600024C RID: 588 RVA: 0x00016A00 File Offset: 0x00014C00
		public void AcceptCallback(IAsyncResult ar)
		{
			if (this._stop)
			{
				return;
			}
			Socket socket = (Socket)ar.AsyncState;
			try
			{
				Socket socket2 = socket.EndAccept(ar);
				if ((this._authUser ?? "").Length == 0 && !Utils.isLAN(socket2))
				{
					socket2.Shutdown(SocketShutdown.Both);
					socket2.Close();
				}
				else
				{
					byte[] array = new byte[4096];
					object[] state = new object[]
					{
						socket2,
						array
					};
					int port = ((IPEndPoint)socket2.LocalEndPoint).Port;
					if (!this._config.GetPortMapCache().ContainsKey(port))
					{
						socket2.BeginReceive(array, 0, array.Length, SocketFlags.None, new AsyncCallback(this.ReceiveCallback), state);
					}
					else
					{
						using (IEnumerator<Listener.Service> enumerator = this._services.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								if (enumerator.Current.Handle(array, 0, socket2))
								{
									return;
								}
							}
						}
						socket2.Shutdown(SocketShutdown.Both);
						socket2.Close();
					}
				}
			}
			catch (ObjectDisposedException)
			{
			}
			catch (Exception arg_F4_0)
			{
				Console.WriteLine(arg_F4_0);
			}
			finally
			{
				try
				{
					socket.BeginAccept(new AsyncCallback(this.AcceptCallback), socket);
				}
				catch (ObjectDisposedException)
				{
				}
				catch (Exception arg_115_0)
				{
					Logging.LogUsefulException(arg_115_0);
					this.ResetTimeout(5.0, socket);
				}
			}
		}

		// Token: 0x06000246 RID: 582 RVA: 0x000164F4 File Offset: 0x000146F4
		private bool CheckIfPortInUse(int port)
		{
			try
			{
				IPEndPoint[] activeTcpListeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
				for (int i = 0; i < activeTcpListeners.Length; i++)
				{
					if (activeTcpListeners[i].Port == port)
					{
						return true;
					}
				}
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x06000245 RID: 581 RVA: 0x000164EB File Offset: 0x000146EB
		public IList<Listener.Service> GetServices()
		{
			return this._services;
		}

		// Token: 0x06000247 RID: 583 RVA: 0x00016544 File Offset: 0x00014744
		public bool isConfigChange(Configuration config)
		{
			try
			{
				if (this._shareOverLAN != config.shareOverLan || this._authUser != config.authUser || this._authPass != config.authPass || this._bypassWhiteList != config.bypassWhiteList || this._socket == null || ((IPEndPoint)this._socket.LocalEndPoint).Port != config.localPort)
				{
					return true;
				}
			}
			catch (Exception)
			{
			}
			return false;
		}

		// Token: 0x0600024D RID: 589 RVA: 0x00016B88 File Offset: 0x00014D88
		private void ReceiveCallback(IAsyncResult ar)
		{
			object[] expr_0B = (object[])ar.AsyncState;
			Socket socket = (Socket)expr_0B[0];
			byte[] firstPacket = (byte[])expr_0B[1];
			try
			{
				int length = socket.EndReceive(ar);
				using (IEnumerator<Listener.Service> enumerator = this._services.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Handle(firstPacket, length, socket))
						{
							return;
						}
					}
				}
				socket.Shutdown(SocketShutdown.Both);
				socket.Close();
			}
			catch (Exception arg_67_0)
			{
				Console.WriteLine(arg_67_0);
				socket.Shutdown(SocketShutdown.Both);
				socket.Close();
			}
		}

		// Token: 0x0600024A RID: 586 RVA: 0x0001686C File Offset: 0x00014A6C
		private void ResetTimeout(double time, Socket socket)
		{
			if (time <= 0.0 && this.timer == null)
			{
				return;
			}
			object obj = this.timerLock;
			lock (obj)
			{
				if (time <= 0.0)
				{
					if (this.timer != null)
					{
						this.timer.Enabled = false;
						this.timer.Elapsed -= delegate(object sender, ElapsedEventArgs e)
						{
							this.timer_Elapsed(sender, e, socket);
						};
						this.timer.Dispose();
						this.timer = null;
					}
				}
				else if (this.timer == null)
				{
					this.timer = new Timer(time * 1000.0);
					this.timer.Elapsed += delegate(object sender, ElapsedEventArgs e)
					{
						this.timer_Elapsed(sender, e, socket);
					};
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

		// Token: 0x06000248 RID: 584 RVA: 0x000165D4 File Offset: 0x000147D4
		public void Start(Configuration config, int port)
		{
			this._config = config;
			this._shareOverLAN = config.shareOverLan;
			this._authUser = config.authUser;
			this._authPass = config.authPass;
			this._bypassWhiteList = config.bypassWhiteList;
			this._stop = false;
			int port2 = (port == 0) ? this._config.localPort : port;
			if (this.CheckIfPortInUse(port2))
			{
				throw new Exception(I18N.GetString("Port already in use"));
			}
			try
			{
				bool arg_8B_0 = true;
				this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				this._socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
				if (arg_8B_0)
				{
					try
					{
						this._socket_v6 = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
						this._socket_v6.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
						this._socket_v6.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
					}
					catch
					{
						this._socket_v6 = null;
					}
				}
				IPEndPoint localEP;
				IPEndPoint localEP2;
				if (this._shareOverLAN)
				{
					localEP = new IPEndPoint(IPAddress.Any, port2);
					localEP2 = new IPEndPoint(IPAddress.IPv6Any, port2);
				}
				else
				{
					localEP = new IPEndPoint(IPAddress.Loopback, port2);
					localEP2 = new IPEndPoint(IPAddress.IPv6Loopback, port2);
				}
				if (this._socket_v6 != null)
				{
					this._socket_v6.Bind(localEP2);
					this._socket_v6.Listen(1024);
				}
				try
				{
					this._socket.Bind(localEP);
					this._socket.Listen(1024);
				}
				catch (SocketException ex)
				{
					if (this._socket_v6 == null)
					{
						throw ex;
					}
					this._socket.Close();
					this._socket = this._socket_v6;
					this._socket_v6 = null;
				}
				Console.WriteLine("ShadowsocksR started");
				this._socket.BeginAccept(new AsyncCallback(this.AcceptCallback), this._socket);
				if (this._socket_v6 != null)
				{
					this._socket_v6.BeginAccept(new AsyncCallback(this.AcceptCallback), this._socket_v6);
				}
			}
			catch (SocketException)
			{
				this._socket.Close();
				if (this._socket_v6 != null)
				{
					this._socket_v6.Close();
				}
				throw;
			}
		}

		// Token: 0x06000249 RID: 585 RVA: 0x00016814 File Offset: 0x00014A14
		public void Stop()
		{
			this.ResetTimeout(0.0, null);
			this._stop = true;
			if (this._socket != null)
			{
				this._socket.Close();
				this._socket = null;
			}
			if (this._socket_v6 != null)
			{
				this._socket_v6.Close();
				this._socket_v6 = null;
			}
		}

		// Token: 0x0600024B RID: 587 RVA: 0x0001698C File Offset: 0x00014B8C
		private void timer_Elapsed(object sender, ElapsedEventArgs eventArgs, Socket socket)
		{
			if (this.timer == null)
			{
				return;
			}
			try
			{
				socket.BeginAccept(new AsyncCallback(this.AcceptCallback), socket);
				this.ResetTimeout(0.0, socket);
			}
			catch (ObjectDisposedException)
			{
			}
			catch (Exception arg_34_0)
			{
				Logging.LogUsefulException(arg_34_0);
				this.ResetTimeout(5.0, socket);
			}
		}

		// Token: 0x040001CA RID: 458
		protected Timer timer;

		// Token: 0x040001CB RID: 459
		protected object timerLock = new object();

		// Token: 0x040001C5 RID: 453
		private string _authPass;

		// Token: 0x040001C4 RID: 452
		private string _authUser;

		// Token: 0x040001C3 RID: 451
		private bool _bypassWhiteList;

		// Token: 0x040001C1 RID: 449
		private Configuration _config;

		// Token: 0x040001C9 RID: 457
		private IList<Listener.Service> _services;

		// Token: 0x040001C2 RID: 450
		private bool _shareOverLAN;

		// Token: 0x040001C6 RID: 454
		private Socket _socket;

		// Token: 0x040001C7 RID: 455
		private Socket _socket_v6;

		// Token: 0x040001C8 RID: 456
		private bool _stop;

		// Token: 0x020000AB RID: 171
		public interface Service
		{
			// Token: 0x06000571 RID: 1393
			bool Handle(byte[] firstPacket, int length, Socket socket);
		}
	}
}
