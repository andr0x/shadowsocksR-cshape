using System;
using Shadowsocks.Obfs;

namespace Shadowsocks.Controller
{
	// Token: 0x0200004E RID: 78
	internal class ProtocolResponseDetector
	{
		// Token: 0x060002A5 RID: 677 RVA: 0x00018D68 File Offset: 0x00016F68
		public ProtocolResponseDetector()
		{
			this.Pass = false;
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x00019008 File Offset: 0x00017208
		protected void Finish()
		{
			this.send_buffer = null;
			this.recv_buffer = null;
			this.protocol = ProtocolResponseDetector.Protocol.UNKONWN;
			this.Pass = true;
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x00018F1C File Offset: 0x0001711C
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

		// Token: 0x060002A6 RID: 678 RVA: 0x00018D90 File Offset: 0x00016F90
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

		// Token: 0x1700001D RID: 29
		public bool Pass
		{
			// Token: 0x060002A3 RID: 675 RVA: 0x00018D57 File Offset: 0x00016F57
			get;
			// Token: 0x060002A4 RID: 676 RVA: 0x00018D5F File Offset: 0x00016F5F
			set;
		}

		// Token: 0x04000201 RID: 513
		protected ProtocolResponseDetector.Protocol protocol;

		// Token: 0x04000203 RID: 515
		protected byte[] recv_buffer = new byte[0];

		// Token: 0x04000202 RID: 514
		protected byte[] send_buffer = new byte[0];

		// Token: 0x020000AD RID: 173
		public enum Protocol
		{
			// Token: 0x0400045D RID: 1117
			UNKONWN = -1,
			// Token: 0x0400045E RID: 1118
			NOTBEGIN,
			// Token: 0x0400045F RID: 1119
			HTTP,
			// Token: 0x04000460 RID: 1120
			TLS,
			// Token: 0x04000461 RID: 1121
			SOCKS4 = 4,
			// Token: 0x04000462 RID: 1122
			SOCKS5
		}
	}
}
