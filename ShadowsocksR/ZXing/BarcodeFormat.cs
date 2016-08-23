using System;

namespace ZXing
{
	// Token: 0x0200005F RID: 95
	[Flags]
	public enum BarcodeFormat
	{
		// Token: 0x0400027A RID: 634
		AZTEC = 1,
		// Token: 0x0400027B RID: 635
		CODABAR = 2,
		// Token: 0x0400027C RID: 636
		CODE_39 = 4,
		// Token: 0x0400027D RID: 637
		CODE_93 = 8,
		// Token: 0x0400027E RID: 638
		CODE_128 = 16,
		// Token: 0x0400027F RID: 639
		DATA_MATRIX = 32,
		// Token: 0x04000280 RID: 640
		EAN_8 = 64,
		// Token: 0x04000281 RID: 641
		EAN_13 = 128,
		// Token: 0x04000282 RID: 642
		ITF = 256,
		// Token: 0x04000283 RID: 643
		MAXICODE = 512,
		// Token: 0x04000284 RID: 644
		PDF_417 = 1024,
		// Token: 0x04000285 RID: 645
		QR_CODE = 2048,
		// Token: 0x04000286 RID: 646
		RSS_14 = 4096,
		// Token: 0x04000287 RID: 647
		RSS_EXPANDED = 8192,
		// Token: 0x04000288 RID: 648
		UPC_A = 16384,
		// Token: 0x04000289 RID: 649
		UPC_E = 32768,
		// Token: 0x0400028A RID: 650
		UPC_EAN_EXTENSION = 65536,
		// Token: 0x0400028B RID: 651
		MSI = 131072,
		// Token: 0x0400028C RID: 652
		PLESSEY = 262144,
		// Token: 0x0400028D RID: 653
		All_1D = 61918
	}
}
