using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Shadowsocks.Model
{
	// Token: 0x02000023 RID: 35
	public class Connections
	{
		// Token: 0x0600014C RID: 332 RVA: 0x00010780 File Offset: 0x0000E980
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

		// Token: 0x0600014E RID: 334 RVA: 0x00010874 File Offset: 0x0000EA74
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

		// Token: 0x0600014D RID: 333 RVA: 0x000107F0 File Offset: 0x0000E9F0
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
			// Token: 0x0600014F RID: 335 RVA: 0x00010900 File Offset: 0x0000EB00
			get
			{
				return this.sockets.Count;
			}
		}

		// Token: 0x040000FA RID: 250
		private Dictionary<Socket, int> sockets = new Dictionary<Socket, int>();
	}
}
