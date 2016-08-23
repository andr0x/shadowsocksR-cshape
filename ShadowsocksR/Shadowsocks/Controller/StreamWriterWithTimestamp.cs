using System;
using System.IO;

namespace Shadowsocks.Controller
{
	// Token: 0x02000044 RID: 68
	public class StreamWriterWithTimestamp : StreamWriter
	{
		// Token: 0x06000256 RID: 598 RVA: 0x000170A0 File Offset: 0x000152A0
		public StreamWriterWithTimestamp(Stream stream) : base(stream)
		{
		}

		// Token: 0x06000257 RID: 599 RVA: 0x000170AC File Offset: 0x000152AC
		private string GetTimestamp()
		{
			return "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ";
		}

		// Token: 0x06000259 RID: 601 RVA: 0x000170EE File Offset: 0x000152EE
		public override void Write(string value)
		{
			base.Write(this.GetTimestamp() + value);
		}

		// Token: 0x06000258 RID: 600 RVA: 0x000170DA File Offset: 0x000152DA
		public override void WriteLine(string value)
		{
			base.WriteLine(this.GetTimestamp() + value);
		}
	}
}
