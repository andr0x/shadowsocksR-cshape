using System;
using System.Collections.Generic;

namespace ZXing
{
	// Token: 0x02000067 RID: 103
	public sealed class Result
	{
		// Token: 0x060003C7 RID: 967 RVA: 0x000202A8 File Offset: 0x0001E4A8
		public Result(string text, byte[] rawBytes, ResultPoint[] resultPoints, BarcodeFormat format) : this(text, rawBytes, resultPoints, format, DateTime.Now.Ticks)
		{
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x000202D0 File Offset: 0x0001E4D0
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

		// Token: 0x060003CB RID: 971 RVA: 0x000203B4 File Offset: 0x0001E5B4
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

		// Token: 0x060003CA RID: 970 RVA: 0x00020344 File Offset: 0x0001E544
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

		// Token: 0x060003C9 RID: 969 RVA: 0x00020320 File Offset: 0x0001E520
		public void putMetadata(ResultMetadataType type, object value)
		{
			if (this.ResultMetadata == null)
			{
				this.ResultMetadata = new Dictionary<ResultMetadataType, object>();
			}
			this.ResultMetadata[type] = value;
		}

		// Token: 0x060003CC RID: 972 RVA: 0x00020408 File Offset: 0x0001E608
		public override string ToString()
		{
			if (this.Text == null)
			{
				return "[" + this.RawBytes.Length + " bytes]";
			}
			return this.Text;
		}

		// Token: 0x17000041 RID: 65
		public BarcodeFormat BarcodeFormat
		{
			// Token: 0x060003C1 RID: 961 RVA: 0x00020275 File Offset: 0x0001E475
			get;
			// Token: 0x060003C2 RID: 962 RVA: 0x0002027D File Offset: 0x0001E47D
			private set;
		}

		// Token: 0x1700003F RID: 63
		public byte[] RawBytes
		{
			// Token: 0x060003BD RID: 957 RVA: 0x00020253 File Offset: 0x0001E453
			get;
			// Token: 0x060003BE RID: 958 RVA: 0x0002025B File Offset: 0x0001E45B
			private set;
		}

		// Token: 0x17000042 RID: 66
		public IDictionary<ResultMetadataType, object> ResultMetadata
		{
			// Token: 0x060003C3 RID: 963 RVA: 0x00020286 File Offset: 0x0001E486
			get;
			// Token: 0x060003C4 RID: 964 RVA: 0x0002028E File Offset: 0x0001E48E
			private set;
		}

		// Token: 0x17000040 RID: 64
		public ResultPoint[] ResultPoints
		{
			// Token: 0x060003BF RID: 959 RVA: 0x00020264 File Offset: 0x0001E464
			get;
			// Token: 0x060003C0 RID: 960 RVA: 0x0002026C File Offset: 0x0001E46C
			private set;
		}

		// Token: 0x1700003E RID: 62
		public string Text
		{
			// Token: 0x060003BB RID: 955 RVA: 0x00020242 File Offset: 0x0001E442
			get;
			// Token: 0x060003BC RID: 956 RVA: 0x0002024A File Offset: 0x0001E44A
			private set;
		}

		// Token: 0x17000043 RID: 67
		public long Timestamp
		{
			// Token: 0x060003C5 RID: 965 RVA: 0x00020297 File Offset: 0x0001E497
			get;
			// Token: 0x060003C6 RID: 966 RVA: 0x0002029F File Offset: 0x0001E49F
			private set;
		}
	}
}
