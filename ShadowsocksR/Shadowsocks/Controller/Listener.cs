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
	// Token: 0x02000043 RID: 67
	public class Listener
	{
		// Token: 0x06000259 RID: 601 RVA: 0x00016D56 File Offset: 0x00014F56
		public Listener(IList<Listener.Service> services)
		{
			this._services = services;
			this._stop = false;
		}

		// Token: 0x06000261 RID: 609 RVA: 0x0001727C File Offset: 0x0001547C
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

		// Token: 0x0600025B RID: 603 RVA: 0x00016D80 File Offset: 0x00014F80
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

		// Token: 0x0600025A RID: 602 RVA: 0x00016D77 File Offset: 0x00014F77
		public IList<Listener.Service> GetServices()
		{
			return this._services;
		}

		// Token: 0x0600025C RID: 604 RVA: 0x00016DD0 File Offset: 0x00014FD0
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

		// Token: 0x06000262 RID: 610 RVA: 0x00017404 File Offset: 0x00015604
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

		// Token: 0x0600025F RID: 607 RVA: 0x000170E8 File Offset: 0x000152E8
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

		// Token: 0x0600025D RID: 605 RVA: 0x00016E60 File Offset: 0x00015060
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

		// Token: 0x0600025E RID: 606 RVA: 0x00017090 File Offset: 0x00015290
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

		// Token: 0x06000260 RID: 608 RVA: 0x00017208 File Offset: 0x00015408
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

		// Token: 0x040001D4 RID: 468
		protected Timer timer;

		// Token: 0x040001D5 RID: 469
		protected object timerLock = new object();

		// Token: 0x040001CF RID: 463
		private string _authPass;

		// Token: 0x040001CE RID: 462
		private string _authUser;

		// Token: 0x040001CD RID: 461
		private bool _bypassWhiteList;

		// Token: 0x040001CB RID: 459
		private Configuration _config;

		// Token: 0x040001D3 RID: 467
		private IList<Listener.Service> _services;

		// Token: 0x040001CC RID: 460
		private bool _shareOverLAN;

		// Token: 0x040001D0 RID: 464
		private Socket _socket;

		// Token: 0x040001D1 RID: 465
		private Socket _socket_v6;

		// Token: 0x040001D2 RID: 466
		private bool _stop;

		// Token: 0x020000A9 RID: 169
		public interface Service
		{
			// Token: 0x06000568 RID: 1384
			bool Handle(byte[] firstPacket, int length, Socket socket);
		}
	}
}
