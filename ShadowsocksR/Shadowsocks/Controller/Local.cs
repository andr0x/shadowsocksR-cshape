using System;
using System.Net;
using System.Net.Sockets;
using Shadowsocks.Model;

namespace Shadowsocks.Controller
{
	// Token: 0x0200004F RID: 79
	internal class Local : Listener.Service
	{
		// Token: 0x060002C0 RID: 704 RVA: 0x0001A23C File Offset: 0x0001843C
		public Local(Configuration config, ServerTransferTotal transfer)
		{
			this._config = config;
			this._transfer = transfer;
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x0001A252 File Offset: 0x00018452
		protected bool Accept(byte[] firstPacket, int length)
		{
			return length >= 2 && (firstPacket[0] == 5 || firstPacket[0] == 4);
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x0001A26C File Offset: 0x0001846C
		public bool Handle(byte[] firstPacket, int length, Socket socket)
		{
			int port = ((IPEndPoint)socket.LocalEndPoint).Port;
			if (!this._config.GetPortMapCache().ContainsKey(port) && !this.Accept(firstPacket, length))
			{
				return false;
			}
			new ProxyAuthHandler(this._config, this._transfer, firstPacket, length, socket);
			return true;
		}

		// Token: 0x04000212 RID: 530
		private Configuration _config;

		// Token: 0x04000213 RID: 531
		private ServerTransferTotal _transfer;
	}
}
