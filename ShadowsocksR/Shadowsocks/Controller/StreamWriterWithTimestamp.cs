using System;
using System.IO;

namespace Shadowsocks.Controller
{
	// Token: 0x02000046 RID: 70
	public class StreamWriterWithTimestamp : StreamWriter
	{
		// Token: 0x0600026B RID: 619 RVA: 0x0001791C File Offset: 0x00015B1C
		public StreamWriterWithTimestamp(Stream stream) : base(stream)
		{
		}

		// Token: 0x0600026C RID: 620 RVA: 0x00017928 File Offset: 0x00015B28
		private string GetTimestamp()
		{
			return "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ";
		}

		// Token: 0x0600026E RID: 622 RVA: 0x0001796A File Offset: 0x00015B6A
		public override void Write(string value)
		{
			base.Write(this.GetTimestamp() + value);
		}

		// Token: 0x0600026D RID: 621 RVA: 0x00017956 File Offset: 0x00015B56
		public override void WriteLine(string value)
		{
			base.WriteLine(this.GetTimestamp() + value);
		}
	}
}
