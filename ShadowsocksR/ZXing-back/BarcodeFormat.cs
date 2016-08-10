using System;

namespace ZXing
{
	// Token: 0x0200005D RID: 93
	[Flags]
	public enum BarcodeFormat
	{
		// Token: 0x0400027D RID: 637
		AZTEC = 1,
		// Token: 0x0400027E RID: 638
		CODABAR = 2,
		// Token: 0x0400027F RID: 639
		CODE_39 = 4,
		// Token: 0x04000280 RID: 640
		CODE_93 = 8,
		// Token: 0x04000281 RID: 641
		CODE_128 = 16,
		// Token: 0x04000282 RID: 642
		DATA_MATRIX = 32,
		// Token: 0x04000283 RID: 643
		EAN_8 = 64,
		// Token: 0x04000284 RID: 644
		EAN_13 = 128,
		// Token: 0x04000285 RID: 645
		ITF = 256,
		// Token: 0x04000286 RID: 646
		MAXICODE = 512,
		// Token: 0x04000287 RID: 647
		PDF_417 = 1024,
		// Token: 0x04000288 RID: 648
		QR_CODE = 2048,
		// Token: 0x04000289 RID: 649
		RSS_14 = 4096,
		// Token: 0x0400028A RID: 650
		RSS_EXPANDED = 8192,
		// Token: 0x0400028B RID: 651
		UPC_A = 16384,
		// Token: 0x0400028C RID: 652
		UPC_E = 32768,
		// Token: 0x0400028D RID: 653
		UPC_EAN_EXTENSION = 65536,
		// Token: 0x0400028E RID: 654
		MSI = 131072,
		// Token: 0x0400028F RID: 655
		PLESSEY = 262144,
		// Token: 0x04000290 RID: 656
		All_1D = 61918
	}
}
