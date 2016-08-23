using System;
using Shadowsocks.Obfs;

namespace Shadowsocks.Controller
{
	// Token: 0x0200004B RID: 75
	internal class ProtocolResponseDetector
	{
		// Token: 0x0600029C RID: 668 RVA: 0x00019200 File Offset: 0x00017400
		public ProtocolResponseDetector()
		{
			this.Pass = false;
		}

		// Token: 0x0600029F RID: 671 RVA: 0x000194A0 File Offset: 0x000176A0
		protected void Finish()
		{
			this.send_buffer = null;
			this.recv_buffer = null;
			this.protocol = ProtocolResponseDetector.Protocol.UNKONWN;
			this.Pass = true;
		}

		// Token: 0x0600029E RID: 670 RVA: 0x000193B4 File Offset: 0x000175B4
		public int OnRecv(byte[] recv_data, int length)
		{
			if (this.protocol == ProtocolResponseDetector.Protocol.UNKONWN || this.protocol == ProtocolResponseDetector.Protocol.NOTBEGIN)
			{
				return 0;
			}
			Array.Resize<byte>(ref this.recv_buffer, this.recv_buffer.Length + length);
			Array.Copy(recv_data, 0, this.recv_buffer, this.recv_buffer.Length - length, length);
			if (this.recv_buffer.Length < 2)
			{
				return 0;
			}
			if (this.protocol == ProtocolResponseDetector.Protocol.HTTP && this.recv_buffer.Length > 4)
			{
				if (this.recv_buffer[0] == 72 && this.recv_buffer[1] == 84 && this.recv_buffer[2] == 84 && this.recv_buffer[3] == 80)
				{
					this.Finish();
					return 0;
				}
				this.protocol = ProtocolResponseDetector.Protocol.UNKONWN;
				return 1;
			}
			else
			{
				if (this.protocol != ProtocolResponseDetector.Protocol.TLS || this.recv_buffer.Length <= 4)
				{
					return 0;
				}
				if (this.recv_buffer[0] == 22 && this.recv_buffer[1] == 3)
				{
					this.Finish();
					return 0;
				}
				this.protocol = ProtocolResponseDetector.Protocol.UNKONWN;
				return 2;
			}
		}

		// Token: 0x0600029D RID: 669 RVA: 0x00019228 File Offset: 0x00017428
		public void OnSend(byte[] send_data, int length)
		{
			if (this.protocol != ProtocolResponseDetector.Protocol.NOTBEGIN)
			{
				return;
			}
			Array.Resize<byte>(ref this.send_buffer, this.send_buffer.Length + length);
			Array.Copy(send_data, 0, this.send_buffer, this.send_buffer.Length - length, length);
			if (this.send_buffer.Length < 2)
			{
				return;
			}
			int headSize = ObfsBase.GetHeadSize(this.send_buffer, this.send_buffer.Length);
			if (this.send_buffer.Length - headSize < 0)
			{
				return;
			}
			byte[] array = new byte[this.send_buffer.Length - headSize];
			Array.Copy(this.send_buffer, headSize, array, 0, array.Length);
			if (array.Length < 2)
			{
				return;
			}
			if (array.Length > 8)
			{
				if (array[0] == 22 && array[1] == 3 && array[2] >= 0 && array[2] <= 3)
				{
					this.protocol = ProtocolResponseDetector.Protocol.TLS;
					return;
				}
				if ((array[0] == 71 && array[1] == 69 && array[2] == 84 && array[3] == 32) || (array[0] == 80 && array[1] == 85 && array[2] == 84 && array[3] == 32) || (array[0] == 72 && array[1] == 69 && array[2] == 65 && array[3] == 68 && array[4] == 32) || (array[0] == 80 && array[1] == 79 && array[2] == 83 && array[3] == 84 && array[4] == 32) || (array[0] == 67 && array[1] == 79 && array[2] == 78 && array[3] == 78 && array[4] == 69 && array[5] == 67 && array[6] == 84 && array[7] == 32))
				{
					this.protocol = ProtocolResponseDetector.Protocol.HTTP;
					return;
				}
			}
			else
			{
				this.protocol = ProtocolResponseDetector.Protocol.UNKONWN;
			}
		}

		// Token: 0x1700001F RID: 31
		public bool Pass
		{
			// Token: 0x0600029A RID: 666 RVA: 0x000191EF File Offset: 0x000173EF
			get;
			// Token: 0x0600029B RID: 667 RVA: 0x000191F7 File Offset: 0x000173F7
			set;
		}

		// Token: 0x040001FB RID: 507
		protected ProtocolResponseDetector.Protocol protocol;

		// Token: 0x040001FD RID: 509
		protected byte[] recv_buffer = new byte[0];

		// Token: 0x040001FC RID: 508
		protected byte[] send_buffer = new byte[0];

		// Token: 0x020000AE RID: 174
		public enum Protocol
		{
			// Token: 0x0400045A RID: 1114
			UNKONWN = -1,
			// Token: 0x0400045B RID: 1115
			NOTBEGIN,
			// Token: 0x0400045C RID: 1116
			HTTP,
			// Token: 0x0400045D RID: 1117
			TLS,
			// Token: 0x0400045E RID: 1118
			SOCKS4 = 4,
			// Token: 0x0400045F RID: 1119
			SOCKS5
		}
	}
}
