using System;
using System.Collections.Generic;

namespace ZXing.Common
{
	// Token: 0x02000084 RID: 132
	public sealed class DecoderResult
	{
		// Token: 0x060004D8 RID: 1240 RVA: 0x000289CA File Offset: 0x00026BCA
		public DecoderResult(byte[] rawBytes, string text, IList<byte[]> byteSegments, string ecLevel) : this(rawBytes, text, byteSegments, ecLevel, -1, -1)
		{
		}

		// Token: 0x060004D9 RID: 1241 RVA: 0x000289DC File Offset: 0x00026BDC
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

		// Token: 0x17000070 RID: 112
		public IList<byte[]> ByteSegments
		{
			// Token: 0x060004C9 RID: 1225 RVA: 0x0002893A File Offset: 0x00026B3A
			get;
			// Token: 0x060004CA RID: 1226 RVA: 0x00028942 File Offset: 0x00026B42
			private set;
		}

		// Token: 0x17000071 RID: 113
		public string ECLevel
		{
			// Token: 0x060004CB RID: 1227 RVA: 0x0002894B File Offset: 0x00026B4B
			get;
			// Token: 0x060004CC RID: 1228 RVA: 0x00028953 File Offset: 0x00026B53
			private set;
		}

		// Token: 0x17000075 RID: 117
		public int Erasures
		{
			// Token: 0x060004D2 RID: 1234 RVA: 0x00028997 File Offset: 0x00026B97
			get;
			// Token: 0x060004D3 RID: 1235 RVA: 0x0002899F File Offset: 0x00026B9F
			set;
		}

		// Token: 0x17000073 RID: 115
		public int ErrorsCorrected
		{
			// Token: 0x060004CE RID: 1230 RVA: 0x00028975 File Offset: 0x00026B75
			get;
			// Token: 0x060004CF RID: 1231 RVA: 0x0002897D File Offset: 0x00026B7D
			set;
		}

		// Token: 0x17000077 RID: 119
		public object Other
		{
			// Token: 0x060004D6 RID: 1238 RVA: 0x000289B9 File Offset: 0x00026BB9
			get;
			// Token: 0x060004D7 RID: 1239 RVA: 0x000289C1 File Offset: 0x00026BC1
			set;
		}

		// Token: 0x1700006E RID: 110
		public byte[] RawBytes
		{
			// Token: 0x060004C5 RID: 1221 RVA: 0x00028918 File Offset: 0x00026B18
			get;
			// Token: 0x060004C6 RID: 1222 RVA: 0x00028920 File Offset: 0x00026B20
			private set;
		}

		// Token: 0x17000072 RID: 114
		public bool StructuredAppend
		{
			// Token: 0x060004CD RID: 1229 RVA: 0x0002895C File Offset: 0x00026B5C
			get
			{
				return this.StructuredAppendParity >= 0 && this.StructuredAppendSequenceNumber >= 0;
			}
		}

		// Token: 0x17000076 RID: 118
		public int StructuredAppendParity
		{
			// Token: 0x060004D4 RID: 1236 RVA: 0x000289A8 File Offset: 0x00026BA8
			get;
			// Token: 0x060004D5 RID: 1237 RVA: 0x000289B0 File Offset: 0x00026BB0
			private set;
		}

		// Token: 0x17000074 RID: 116
		public int StructuredAppendSequenceNumber
		{
			// Token: 0x060004D0 RID: 1232 RVA: 0x00028986 File Offset: 0x00026B86
			get;
			// Token: 0x060004D1 RID: 1233 RVA: 0x0002898E File Offset: 0x00026B8E
			private set;
		}

		// Token: 0x1700006F RID: 111
		public string Text
		{
			// Token: 0x060004C7 RID: 1223 RVA: 0x00028929 File Offset: 0x00026B29
			get;
			// Token: 0x060004C8 RID: 1224 RVA: 0x00028931 File Offset: 0x00026B31
			private set;
		}
	}
}
