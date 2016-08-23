using System;
using System.Text;

namespace ZXing.QrCode.Internal
{
	// Token: 0x02000082 RID: 130
	public sealed class QRCode
	{
		// Token: 0x06000498 RID: 1176 RVA: 0x000271AF File Offset: 0x000253AF
		public QRCode()
		{
			this.MaskPattern = -1;
		}

		// Token: 0x060004A4 RID: 1188 RVA: 0x000272F8 File Offset: 0x000254F8
		public static bool isValidMaskPattern(int maskPattern)
		{
			return maskPattern >= 0 && maskPattern < QRCode.NUM_MASK_PATTERNS;
		}

		// Token: 0x060004A3 RID: 1187 RVA: 0x00027214 File Offset: 0x00025414
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(200);
			stringBuilder.Append("<<\n");
			stringBuilder.Append(" mode: ");
			stringBuilder.Append(this.Mode);
			stringBuilder.Append("\n ecLevel: ");
			stringBuilder.Append(this.ECLevel);
			stringBuilder.Append("\n version: ");
			if (this.Version == null)
			{
				stringBuilder.Append("null");
			}
			else
			{
				stringBuilder.Append(this.Version);
			}
			stringBuilder.Append("\n maskPattern: ");
			stringBuilder.Append(this.MaskPattern);
			if (this.Matrix == null)
			{
				stringBuilder.Append("\n matrix: null\n");
			}
			else
			{
				stringBuilder.Append("\n matrix:\n");
				stringBuilder.Append(this.Matrix.ToString());
			}
			stringBuilder.Append(">>\n");
			return stringBuilder.ToString();
		}

		// Token: 0x17000064 RID: 100
		public ErrorCorrectionLevel ECLevel
		{
			// Token: 0x0600049B RID: 1179 RVA: 0x000271CF File Offset: 0x000253CF
			get;
			// Token: 0x0600049C RID: 1180 RVA: 0x000271D7 File Offset: 0x000253D7
			set;
		}

		// Token: 0x17000066 RID: 102
		public int MaskPattern
		{
			// Token: 0x0600049F RID: 1183 RVA: 0x000271F1 File Offset: 0x000253F1
			get;
			// Token: 0x060004A0 RID: 1184 RVA: 0x000271F9 File Offset: 0x000253F9
			set;
		}

		// Token: 0x17000067 RID: 103
		public ByteMatrix Matrix
		{
			// Token: 0x060004A1 RID: 1185 RVA: 0x00027202 File Offset: 0x00025402
			get;
			// Token: 0x060004A2 RID: 1186 RVA: 0x0002720A File Offset: 0x0002540A
			set;
		}

		// Token: 0x17000063 RID: 99
		public Mode Mode
		{
			// Token: 0x06000499 RID: 1177 RVA: 0x000271BE File Offset: 0x000253BE
			get;
			// Token: 0x0600049A RID: 1178 RVA: 0x000271C6 File Offset: 0x000253C6
			set;
		}

		// Token: 0x17000065 RID: 101
		public Version Version
		{
			// Token: 0x0600049D RID: 1181 RVA: 0x000271E0 File Offset: 0x000253E0
			get;
			// Token: 0x0600049E RID: 1182 RVA: 0x000271E8 File Offset: 0x000253E8
			set;
		}

		// Token: 0x0400032A RID: 810
		public static int NUM_MASK_PATTERNS = 8;
	}
}
