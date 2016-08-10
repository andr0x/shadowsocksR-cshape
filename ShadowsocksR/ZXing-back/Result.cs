using System;
using System.Collections.Generic;

namespace ZXing
{
	// Token: 0x02000065 RID: 101
	public sealed class Result
	{
		// Token: 0x060003BE RID: 958 RVA: 0x00021060 File Offset: 0x0001F260
		public Result(string text, byte[] rawBytes, ResultPoint[] resultPoints, BarcodeFormat format) : this(text, rawBytes, resultPoints, format, DateTime.Now.Ticks)
		{
		}

		// Token: 0x060003BF RID: 959 RVA: 0x00021088 File Offset: 0x0001F288
		public Result(string text, byte[] rawBytes, ResultPoint[] resultPoints, BarcodeFormat format, long timestamp)
		{
			if (text == null && rawBytes == null)
			{
				throw new ArgumentException("Text and bytes are null");
			}
			this.Text = text;
			this.RawBytes = rawBytes;
			this.ResultPoints = resultPoints;
			this.BarcodeFormat = format;
			this.ResultMetadata = null;
			this.Timestamp = timestamp;
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x0002116C File Offset: 0x0001F36C
		public void addResultPoints(ResultPoint[] newPoints)
		{
			ResultPoint[] resultPoints = this.ResultPoints;
			if (resultPoints == null)
			{
				this.ResultPoints = newPoints;
				return;
			}
			if (newPoints != null && newPoints.Length != 0)
			{
				ResultPoint[] array = new ResultPoint[resultPoints.Length + newPoints.Length];
				Array.Copy(resultPoints, 0, array, 0, resultPoints.Length);
				Array.Copy(newPoints, 0, array, resultPoints.Length, newPoints.Length);
				this.ResultPoints = array;
			}
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x000210FC File Offset: 0x0001F2FC
		public void putAllMetadata(IDictionary<ResultMetadataType, object> metadata)
		{
			if (metadata != null)
			{
				if (this.ResultMetadata == null)
				{
					this.ResultMetadata = metadata;
					return;
				}
				foreach (KeyValuePair<ResultMetadataType, object> current in metadata)
				{
					this.ResultMetadata[current.Key] = current.Value;
				}
			}
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x000210D8 File Offset: 0x0001F2D8
		public void putMetadata(ResultMetadataType type, object value)
		{
			if (this.ResultMetadata == null)
			{
				this.ResultMetadata = new Dictionary<ResultMetadataType, object>();
			}
			this.ResultMetadata[type] = value;
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x000211C0 File Offset: 0x0001F3C0
		public override string ToString()
		{
			if (this.Text == null)
			{
				return "[" + this.RawBytes.Length + " bytes]";
			}
			return this.Text;
		}

		// Token: 0x1700003E RID: 62
		public BarcodeFormat BarcodeFormat
		{
			// Token: 0x060003B8 RID: 952 RVA: 0x0002102D File Offset: 0x0001F22D
			get;
			// Token: 0x060003B9 RID: 953 RVA: 0x00021035 File Offset: 0x0001F235
			private set;
		}

		// Token: 0x1700003C RID: 60
		public byte[] RawBytes
		{
			// Token: 0x060003B4 RID: 948 RVA: 0x0002100B File Offset: 0x0001F20B
			get;
			// Token: 0x060003B5 RID: 949 RVA: 0x00021013 File Offset: 0x0001F213
			private set;
		}

		// Token: 0x1700003F RID: 63
		public IDictionary<ResultMetadataType, object> ResultMetadata
		{
			// Token: 0x060003BA RID: 954 RVA: 0x0002103E File Offset: 0x0001F23E
			get;
			// Token: 0x060003BB RID: 955 RVA: 0x00021046 File Offset: 0x0001F246
			private set;
		}

		// Token: 0x1700003D RID: 61
		public ResultPoint[] ResultPoints
		{
			// Token: 0x060003B6 RID: 950 RVA: 0x0002101C File Offset: 0x0001F21C
			get;
			// Token: 0x060003B7 RID: 951 RVA: 0x00021024 File Offset: 0x0001F224
			private set;
		}

		// Token: 0x1700003B RID: 59
		public string Text
		{
			// Token: 0x060003B2 RID: 946 RVA: 0x00020FFA File Offset: 0x0001F1FA
			get;
			// Token: 0x060003B3 RID: 947 RVA: 0x00021002 File Offset: 0x0001F202
			private set;
		}

		// Token: 0x17000040 RID: 64
		public long Timestamp
		{
			// Token: 0x060003BC RID: 956 RVA: 0x0002104F File Offset: 0x0001F24F
			get;
			// Token: 0x060003BD RID: 957 RVA: 0x00021057 File Offset: 0x0001F257
			private set;
		}
	}
}
