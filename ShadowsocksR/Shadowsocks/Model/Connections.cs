using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Shadowsocks.Model
{
	// Token: 0x02000021 RID: 33
	public class Connections
	{
		// Token: 0x06000138 RID: 312 RVA: 0x0000FD3C File Offset: 0x0000DF3C
		public bool AddRef(Socket socket)
		{
			bool result;
			lock (this)
			{
				if (this.sockets.ContainsKey(socket))
				{
					Dictionary<Socket, int> dictionary = this.sockets;
					dictionary[socket]++;
				}
				else
				{
					this.sockets[socket] = 1;
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600013A RID: 314 RVA: 0x0000FE30 File Offset: 0x0000E030
		public void CloseAll()
		{
			Socket[] array;
			lock (this)
			{
				array = new Socket[this.sockets.Count];
				this.sockets.Keys.CopyTo(array, 0);
			}
			Socket[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Socket socket = array2[i];
				try
				{
					socket.Shutdown(SocketShutdown.Both);
				}
				catch
				{
				}
			}
		}

		// Token: 0x06000139 RID: 313 RVA: 0x0000FDAC File Offset: 0x0000DFAC
		public bool DecRef(Socket socket)
		{
			bool result;
			lock (this)
			{
				if (this.sockets.ContainsKey(socket))
				{
					Dictionary<Socket, int> dictionary = this.sockets;
					dictionary[socket]--;
					if (this.sockets[socket] == 0)
					{
						this.sockets.Remove(socket);
					}
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x17000010 RID: 16
		public int Count
		{
			// Token: 0x0600013B RID: 315 RVA: 0x0000FEBC File Offset: 0x0000E0BC
			get
			{
				return this.sockets.Count;
			}
		}

		// Token: 0x040000EF RID: 239
		private Dictionary<Socket, int> sockets = new Dictionary<Socket, int>();
	}
}
