using System;
using System.Text;

namespace ZXing.QrCode.Internal
{
	// Token: 0x02000080 RID: 128
	public sealed class QRCode
	{
		// Token: 0x0600048F RID: 1167 RVA: 0x00027F67 File Offset: 0x00026167
		public QRCode()
		{
			this.MaskPattern = -1;
		}

		// Token: 0x0600049B RID: 1179 RVA: 0x000280B0 File Offset: 0x000262B0
		public static bool isValidMaskPattern(int maskPattern)
		{
			return maskPattern >= 0 && maskPattern < QRCode.NUM_MASK_PATTERNS;
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x00027FCC File Offset: 0x000261CC
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

		// Token: 0x17000061 RID: 97
		public ErrorCorrectionLevel ECLevel
		{
			// Token: 0x06000492 RID: 1170 RVA: 0x00027F87 File Offset: 0x00026187
			get;
			// Token: 0x06000493 RID: 1171 RVA: 0x00027F8F File Offset: 0x0002618F
			set;
		}

		// Token: 0x17000063 RID: 99
		public int MaskPattern
		{
			// Token: 0x06000496 RID: 1174 RVA: 0x00027FA9 File Offset: 0x000261A9
			get;
			// Token: 0x06000497 RID: 1175 RVA: 0x00027FB1 File Offset: 0x000261B1
			set;
		}

		// Token: 0x17000064 RID: 100
		public ByteMatrix Matrix
		{
			// Token: 0x06000498 RID: 1176 RVA: 0x00027FBA File Offset: 0x000261BA
			get;
			// Token: 0x06000499 RID: 1177 RVA: 0x00027FC2 File Offset: 0x000261C2
			set;
		}

		// Token: 0x17000060 RID: 96
		public Mode Mode
		{
			// Token: 0x06000490 RID: 1168 RVA: 0x00027F76 File Offset: 0x00026176
			get;
			// Token: 0x06000491 RID: 1169 RVA: 0x00027F7E File Offset: 0x0002617E
			set;
		}

		// Token: 0x17000062 RID: 98
		public Version Version
		{
			// Token: 0x06000494 RID: 1172 RVA: 0x00027F98 File Offset: 0x00026198
			get;
			// Token: 0x06000495 RID: 1173 RVA: 0x00027FA0 File Offset: 0x000261A0
			set;
		}

		// Token: 0x0400032D RID: 813
		public static int NUM_MASK_PATTERNS = 8;
	}
}
