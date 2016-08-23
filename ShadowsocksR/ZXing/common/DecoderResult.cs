using System;
using System.Collections.Generic;

namespace ZXing.Common
{
	// Token: 0x02000086 RID: 134
	public sealed class DecoderResult
	{
		// Token: 0x060004E1 RID: 1249 RVA: 0x00027C12 File Offset: 0x00025E12
		public DecoderResult(byte[] rawBytes, string text, IList<byte[]> byteSegments, string ecLevel) : this(rawBytes, text, byteSegments, ecLevel, -1, -1)
		{
		}

		// Token: 0x060004E2 RID: 1250 RVA: 0x00027C24 File Offset: 0x00025E24
		public DecoderResult(byte[] rawBytes, string text, IList<byte[]> byteSegments, string ecLevel, int saSequence, int saParity)
		{
			if (rawBytes == null && text == null)
			{
				throw new ArgumentException();
			}
			this.RawBytes = rawBytes;
			this.Text = text;
			this.ByteSegments = byteSegments;
			this.ECLevel = ecLevel;
			this.StructuredAppendParity = saParity;
			this.StructuredAppendSequenceNumber = saSequence;
		}

		// Token: 0x17000073 RID: 115
		public IList<byte[]> ByteSegments
		{
			// Token: 0x060004D2 RID: 1234 RVA: 0x00027B82 File Offset: 0x00025D82
			get;
			// Token: 0x060004D3 RID: 1235 RVA: 0x00027B8A File Offset: 0x00025D8A
			private set;
		}

		// Token: 0x17000074 RID: 116
		public string ECLevel
		{
			// Token: 0x060004D4 RID: 1236 RVA: 0x00027B93 File Offset: 0x00025D93
			get;
			// Token: 0x060004D5 RID: 1237 RVA: 0x00027B9B File Offset: 0x00025D9B
			private set;
		}

		// Token: 0x17000078 RID: 120
		public int Erasures
		{
			// Token: 0x060004DB RID: 1243 RVA: 0x00027BDF File Offset: 0x00025DDF
			get;
			// Token: 0x060004DC RID: 1244 RVA: 0x00027BE7 File Offset: 0x00025DE7
			set;
		}

		// Token: 0x17000076 RID: 118
		public int ErrorsCorrected
		{
			// Token: 0x060004D7 RID: 1239 RVA: 0x00027BBD File Offset: 0x00025DBD
			get;
			// Token: 0x060004D8 RID: 1240 RVA: 0x00027BC5 File Offset: 0x00025DC5
			set;
		}

		// Token: 0x1700007A RID: 122
		public object Other
		{
			// Token: 0x060004DF RID: 1247 RVA: 0x00027C01 File Offset: 0x00025E01
			get;
			// Token: 0x060004E0 RID: 1248 RVA: 0x00027C09 File Offset: 0x00025E09
			set;
		}

		// Token: 0x17000071 RID: 113
		public byte[] RawBytes
		{
			// Token: 0x060004CE RID: 1230 RVA: 0x00027B60 File Offset: 0x00025D60
			get;
			// Token: 0x060004CF RID: 1231 RVA: 0x00027B68 File Offset: 0x00025D68
			private set;
		}

		// Token: 0x17000075 RID: 117
		public bool StructuredAppend
		{
			// Token: 0x060004D6 RID: 1238 RVA: 0x00027BA4 File Offset: 0x00025DA4
			get
			{
				return this.StructuredAppendParity >= 0 && this.StructuredAppendSequenceNumber >= 0;
			}
		}

		// Token: 0x17000079 RID: 121
		public int StructuredAppendParity
		{
			// Token: 0x060004DD RID: 1245 RVA: 0x00027BF0 File Offset: 0x00025DF0
			get;
			// Token: 0x060004DE RID: 1246 RVA: 0x00027BF8 File Offset: 0x00025DF8
			private set;
		}

		// Token: 0x17000077 RID: 119
		public int StructuredAppendSequenceNumber
		{
			// Token: 0x060004D9 RID: 1241 RVA: 0x00027BCE File Offset: 0x00025DCE
			get;
			// Token: 0x060004DA RID: 1242 RVA: 0x00027BD6 File Offset: 0x00025DD6
			private set;
		}

		// Token: 0x17000072 RID: 114
		public string Text
		{
			// Token: 0x060004D0 RID: 1232 RVA: 0x00027B71 File Offset: 0x00025D71
			get;
			// Token: 0x060004D1 RID: 1233 RVA: 0x00027B79 File Offset: 0x00025D79
			private set;
		}
	}
}
