using System;
using ZXing.Common;

namespace ZXing.QrCode.Internal
{
	// Token: 0x02000074 RID: 116
	public sealed class Version
	{
		// Token: 0x06000417 RID: 1047 RVA: 0x00022E24 File Offset: 0x00021024
		private Version(int versionNumber, int[] alignmentPatternCenters, params Version.ECBlocks[] ecBlocks)
		{
			this.versionNumber = versionNumber;
			this.alignmentPatternCenters = alignmentPatternCenters;
			this.ecBlocks = ecBlocks;
			int num = 0;
			int eCCodewordsPerBlock = ecBlocks[0].ECCodewordsPerBlock;
			Version.ECB[] eCBlocks = ecBlocks[0].getECBlocks();
			for (int i = 0; i < eCBlocks.Length; i++)
			{
				Version.ECB eCB = eCBlocks[i];
				num += eCB.Count * (eCB.DataCodewords + eCCodewordsPerBlock);
			}
			this.totalCodewords = num;
		}

		// Token: 0x06000420 RID: 1056 RVA: 0x00022F7C File Offset: 0x0002117C
		internal BitMatrix buildFunctionPattern()
		{
			int dimensionForVersion = this.DimensionForVersion;
			BitMatrix bitMatrix = new BitMatrix(dimensionForVersion);
			bitMatrix.setRegion(0, 0, 9, 9);
			bitMatrix.setRegion(dimensionForVersion - 8, 0, 8, 9);
			bitMatrix.setRegion(0, dimensionForVersion - 8, 9, 8);
			int num = this.alignmentPatternCenters.Length;
			for (int i = 0; i < num; i++)
			{
				int top = this.alignmentPatternCenters[i] - 2;
				for (int j = 0; j < num; j++)
				{
					if ((i != 0 || (j != 0 && j != num - 1)) && (i != num - 1 || j != 0))
					{
						bitMatrix.setRegion(this.alignmentPatternCenters[j] - 2, top, 5, 5);
					}
				}
			}
			bitMatrix.setRegion(6, 9, 1, dimensionForVersion - 17);
			bitMatrix.setRegion(9, 6, dimensionForVersion - 17, 1);
			if (this.versionNumber > 6)
			{
				bitMatrix.setRegion(dimensionForVersion - 11, 0, 3, 6);
				bitMatrix.setRegion(0, dimensionForVersion - 11, 6, 3);
			}
			return bitMatrix;
		}

		// Token: 0x06000422 RID: 1058 RVA: 0x00023068 File Offset: 0x00021268
		private static Version[] buildVersions()
		{
			return new Version[]
			{
				new Version(1, new int[0], new Version.ECBlocks[]
				{
					new Version.ECBlocks(7, new Version.ECB[]
					{
						new Version.ECB(1, 19)
					}),
					new Version.ECBlocks(10, new Version.ECB[]
					{
						new Version.ECB(1, 16)
					}),
					new Version.ECBlocks(13, new Version.ECB[]
					{
						new Version.ECB(1, 13)
					}),
					new Version.ECBlocks(17, new Version.ECB[]
					{
						new Version.ECB(1, 9)
					})
				}),
				new Version(2, new int[]
				{
					6,
					18
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(10, new Version.ECB[]
					{
						new Version.ECB(1, 34)
					}),
					new Version.ECBlocks(16, new Version.ECB[]
					{
						new Version.ECB(1, 28)
					}),
					new Version.ECBlocks(22, new Version.ECB[]
					{
						new Version.ECB(1, 22)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(1, 16)
					})
				}),
				new Version(3, new int[]
				{
					6,
					22
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(15, new Version.ECB[]
					{
						new Version.ECB(1, 55)
					}),
					new Version.ECBlocks(26, new Version.ECB[]
					{
						new Version.ECB(1, 44)
					}),
					new Version.ECBlocks(18, new Version.ECB[]
					{
						new Version.ECB(2, 17)
					}),
					new Version.ECBlocks(22, new Version.ECB[]
					{
						new Version.ECB(2, 13)
					})
				}),
				new Version(4, new int[]
				{
					6,
					26
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(20, new Version.ECB[]
					{
						new Version.ECB(1, 80)
					}),
					new Version.ECBlocks(18, new Version.ECB[]
					{
						new Version.ECB(2, 32)
					}),
					new Version.ECBlocks(26, new Version.ECB[]
					{
						new Version.ECB(2, 24)
					}),
					new Version.ECBlocks(16, new Version.ECB[]
					{
						new Version.ECB(4, 9)
					})
				}),
				new Version(5, new int[]
				{
					6,
					30
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(26, new Version.ECB[]
					{
						new Version.ECB(1, 108)
					}),
					new Version.ECBlocks(24, new Version.ECB[]
					{
						new Version.ECB(2, 43)
					}),
					new Version.ECBlocks(18, new Version.ECB[]
					{
						new Version.ECB(2, 15),
						new Version.ECB(2, 16)
					}),
					new Version.ECBlocks(22, new Version.ECB[]
					{
						new Version.ECB(2, 11),
						new Version.ECB(2, 12)
					})
				}),
				new Version(6, new int[]
				{
					6,
					34
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(18, new Version.ECB[]
					{
						new Version.ECB(2, 68)
					}),
					new Version.ECBlocks(16, new Version.ECB[]
					{
						new Version.ECB(4, 27)
					}),
					new Version.ECBlocks(24, new Version.ECB[]
					{
						new Version.ECB(4, 19)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(4, 15)
					})
				}),
				new Version(7, new int[]
				{
					6,
					22,
					38
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(20, new Version.ECB[]
					{
						new Version.ECB(2, 78)
					}),
					new Version.ECBlocks(18, new Version.ECB[]
					{
						new Version.ECB(4, 31)
					}),
					new Version.ECBlocks(18, new Version.ECB[]
					{
						new Version.ECB(2, 14),
						new Version.ECB(4, 15)
					}),
					new Version.ECBlocks(26, new Version.ECB[]
					{
						new Version.ECB(4, 13),
						new Version.ECB(1, 14)
					})
				}),
				new Version(8, new int[]
				{
					6,
					24,
					42
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(24, new Version.ECB[]
					{
						new Version.ECB(2, 97)
					}),
					new Version.ECBlocks(22, new Version.ECB[]
					{
						new Version.ECB(2, 38),
						new Version.ECB(2, 39)
					}),
					new Version.ECBlocks(22, new Version.ECB[]
					{
						new Version.ECB(4, 18),
						new Version.ECB(2, 19)
					}),
					new Version.ECBlocks(26, new Version.ECB[]
					{
						new Version.ECB(4, 14),
						new Version.ECB(2, 15)
					})
				}),
				new Version(9, new int[]
				{
					6,
					26,
					46
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(2, 116)
					}),
					new Version.ECBlocks(22, new Version.ECB[]
					{
						new Version.ECB(3, 36),
						new Version.ECB(2, 37)
					}),
					new Version.ECBlocks(20, new Version.ECB[]
					{
						new Version.ECB(4, 16),
						new Version.ECB(4, 17)
					}),
					new Version.ECBlocks(24, new Version.ECB[]
					{
						new Version.ECB(4, 12),
						new Version.ECB(4, 13)
					})
				}),
				new Version(10, new int[]
				{
					6,
					28,
					50
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(18, new Version.ECB[]
					{
						new Version.ECB(2, 68),
						new Version.ECB(2, 69)
					}),
					new Version.ECBlocks(26, new Version.ECB[]
					{
						new Version.ECB(4, 43),
						new Version.ECB(1, 44)
					}),
					new Version.ECBlocks(24, new Version.ECB[]
					{
						new Version.ECB(6, 19),
						new Version.ECB(2, 20)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(6, 15),
						new Version.ECB(2, 16)
					})
				}),
				new Version(11, new int[]
				{
					6,
					30,
					54
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(20, new Version.ECB[]
					{
						new Version.ECB(4, 81)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(1, 50),
						new Version.ECB(4, 51)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(4, 22),
						new Version.ECB(4, 23)
					}),
					new Version.ECBlocks(24, new Version.ECB[]
					{
						new Version.ECB(3, 12),
						new Version.ECB(8, 13)
					})
				}),
				new Version(12, new int[]
				{
					6,
					32,
					58
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(24, new Version.ECB[]
					{
						new Version.ECB(2, 92),
						new Version.ECB(2, 93)
					}),
					new Version.ECBlocks(22, new Version.ECB[]
					{
						new Version.ECB(6, 36),
						new Version.ECB(2, 37)
					}),
					new Version.ECBlocks(26, new Version.ECB[]
					{
						new Version.ECB(4, 20),
						new Version.ECB(6, 21)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(7, 14),
						new Version.ECB(4, 15)
					})
				}),
				new Version(13, new int[]
				{
					6,
					34,
					62
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(26, new Version.ECB[]
					{
						new Version.ECB(4, 107)
					}),
					new Version.ECBlocks(22, new Version.ECB[]
					{
						new Version.ECB(8, 37),
						new Version.ECB(1, 38)
					}),
					new Version.ECBlocks(24, new Version.ECB[]
					{
						new Version.ECB(8, 20),
						new Version.ECB(4, 21)
					}),
					new Version.ECBlocks(22, new Version.ECB[]
					{
						new Version.ECB(12, 11),
						new Version.ECB(4, 12)
					})
				}),
				new Version(14, new int[]
				{
					6,
					26,
					46,
					66
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(3, 115),
						new Version.ECB(1, 116)
					}),
					new Version.ECBlocks(24, new Version.ECB[]
					{
						new Version.ECB(4, 40),
						new Version.ECB(5, 41)
					}),
					new Version.ECBlocks(20, new Version.ECB[]
					{
						new Version.ECB(11, 16),
						new Version.ECB(5, 17)
					}),
					new Version.ECBlocks(24, new Version.ECB[]
					{
						new Version.ECB(11, 12),
						new Version.ECB(5, 13)
					})
				}),
				new Version(15, new int[]
				{
					6,
					26,
					48,
					70
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(22, new Version.ECB[]
					{
						new Version.ECB(5, 87),
						new Version.ECB(1, 88)
					}),
					new Version.ECBlocks(24, new Version.ECB[]
					{
						new Version.ECB(5, 41),
						new Version.ECB(5, 42)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(5, 24),
						new Version.ECB(7, 25)
					}),
					new Version.ECBlocks(24, new Version.ECB[]
					{
						new Version.ECB(11, 12),
						new Version.ECB(7, 13)
					})
				}),
				new Version(16, new int[]
				{
					6,
					26,
					50,
					74
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(24, new Version.ECB[]
					{
						new Version.ECB(5, 98),
						new Version.ECB(1, 99)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(7, 45),
						new Version.ECB(3, 46)
					}),
					new Version.ECBlocks(24, new Version.ECB[]
					{
						new Version.ECB(15, 19),
						new Version.ECB(2, 20)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(3, 15),
						new Version.ECB(13, 16)
					})
				}),
				new Version(17, new int[]
				{
					6,
					30,
					54,
					78
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(1, 107),
						new Version.ECB(5, 108)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(10, 46),
						new Version.ECB(1, 47)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(1, 22),
						new Version.ECB(15, 23)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(2, 14),
						new Version.ECB(17, 15)
					})
				}),
				new Version(18, new int[]
				{
					6,
					30,
					56,
					82
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(5, 120),
						new Version.ECB(1, 121)
					}),
					new Version.ECBlocks(26, new Version.ECB[]
					{
						new Version.ECB(9, 43),
						new Version.ECB(4, 44)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(17, 22),
						new Version.ECB(1, 23)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(2, 14),
						new Version.ECB(19, 15)
					})
				}),
				new Version(19, new int[]
				{
					6,
					30,
					58,
					86
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(3, 113),
						new Version.ECB(4, 114)
					}),
					new Version.ECBlocks(26, new Version.ECB[]
					{
						new Version.ECB(3, 44),
						new Version.ECB(11, 45)
					}),
					new Version.ECBlocks(26, new Version.ECB[]
					{
						new Version.ECB(17, 21),
						new Version.ECB(4, 22)
					}),
					new Version.ECBlocks(26, new Version.ECB[]
					{
						new Version.ECB(9, 13),
						new Version.ECB(16, 14)
					})
				}),
				new Version(20, new int[]
				{
					6,
					34,
					62,
					90
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(3, 107),
						new Version.ECB(5, 108)
					}),
					new Version.ECBlocks(26, new Version.ECB[]
					{
						new Version.ECB(3, 41),
						new Version.ECB(13, 42)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(15, 24),
						new Version.ECB(5, 25)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(15, 15),
						new Version.ECB(10, 16)
					})
				}),
				new Version(21, new int[]
				{
					6,
					28,
					50,
					72,
					94
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(4, 116),
						new Version.ECB(4, 117)
					}),
					new Version.ECBlocks(26, new Version.ECB[]
					{
						new Version.ECB(17, 42)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(17, 22),
						new Version.ECB(6, 23)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(19, 16),
						new Version.ECB(6, 17)
					})
				}),
				new Version(22, new int[]
				{
					6,
					26,
					50,
					74,
					98
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(2, 111),
						new Version.ECB(7, 112)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(17, 46)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(7, 24),
						new Version.ECB(16, 25)
					}),
					new Version.ECBlocks(24, new Version.ECB[]
					{
						new Version.ECB(34, 13)
					})
				}),
				new Version(23, new int[]
				{
					6,
					30,
					54,
					74,
					102
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(4, 121),
						new Version.ECB(5, 122)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(4, 47),
						new Version.ECB(14, 48)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(11, 24),
						new Version.ECB(14, 25)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(16, 15),
						new Version.ECB(14, 16)
					})
				}),
				new Version(24, new int[]
				{
					6,
					28,
					54,
					80,
					106
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(6, 117),
						new Version.ECB(4, 118)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(6, 45),
						new Version.ECB(14, 46)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(11, 24),
						new Version.ECB(16, 25)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(30, 16),
						new Version.ECB(2, 17)
					})
				}),
				new Version(25, new int[]
				{
					6,
					32,
					58,
					84,
					110
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(26, new Version.ECB[]
					{
						new Version.ECB(8, 106),
						new Version.ECB(4, 107)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(8, 47),
						new Version.ECB(13, 48)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(7, 24),
						new Version.ECB(22, 25)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(22, 15),
						new Version.ECB(13, 16)
					})
				}),
				new Version(26, new int[]
				{
					6,
					30,
					58,
					86,
					114
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(10, 114),
						new Version.ECB(2, 115)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(19, 46),
						new Version.ECB(4, 47)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(28, 22),
						new Version.ECB(6, 23)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(33, 16),
						new Version.ECB(4, 17)
					})
				}),
				new Version(27, new int[]
				{
					6,
					34,
					62,
					90,
					118
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(8, 122),
						new Version.ECB(4, 123)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(22, 45),
						new Version.ECB(3, 46)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(8, 23),
						new Version.ECB(26, 24)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(12, 15),
						new Version.ECB(28, 16)
					})
				}),
				new Version(28, new int[]
				{
					6,
					26,
					50,
					74,
					98,
					122
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(3, 117),
						new Version.ECB(10, 118)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(3, 45),
						new Version.ECB(23, 46)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(4, 24),
						new Version.ECB(31, 25)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(11, 15),
						new Version.ECB(31, 16)
					})
				}),
				new Version(29, new int[]
				{
					6,
					30,
					54,
					78,
					102,
					126
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(7, 116),
						new Version.ECB(7, 117)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(21, 45),
						new Version.ECB(7, 46)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(1, 23),
						new Version.ECB(37, 24)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(19, 15),
						new Version.ECB(26, 16)
					})
				}),
				new Version(30, new int[]
				{
					6,
					26,
					52,
					78,
					104,
					130
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(5, 115),
						new Version.ECB(10, 116)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(19, 47),
						new Version.ECB(10, 48)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(15, 24),
						new Version.ECB(25, 25)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(23, 15),
						new Version.ECB(25, 16)
					})
				}),
				new Version(31, new int[]
				{
					6,
					30,
					56,
					82,
					108,
					134
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(13, 115),
						new Version.ECB(3, 116)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(2, 46),
						new Version.ECB(29, 47)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(42, 24),
						new Version.ECB(1, 25)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(23, 15),
						new Version.ECB(28, 16)
					})
				}),
				new Version(32, new int[]
				{
					6,
					34,
					60,
					86,
					112,
					138
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(17, 115)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(10, 46),
						new Version.ECB(23, 47)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(10, 24),
						new Version.ECB(35, 25)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(19, 15),
						new Version.ECB(35, 16)
					})
				}),
				new Version(33, new int[]
				{
					6,
					30,
					58,
					86,
					114,
					142
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(17, 115),
						new Version.ECB(1, 116)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(14, 46),
						new Version.ECB(21, 47)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(29, 24),
						new Version.ECB(19, 25)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(11, 15),
						new Version.ECB(46, 16)
					})
				}),
				new Version(34, new int[]
				{
					6,
					34,
					62,
					90,
					118,
					146
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(13, 115),
						new Version.ECB(6, 116)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(14, 46),
						new Version.ECB(23, 47)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(44, 24),
						new Version.ECB(7, 25)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(59, 16),
						new Version.ECB(1, 17)
					})
				}),
				new Version(35, new int[]
				{
					6,
					30,
					54,
					78,
					102,
					126,
					150
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(12, 121),
						new Version.ECB(7, 122)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(12, 47),
						new Version.ECB(26, 48)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(39, 24),
						new Version.ECB(14, 25)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(22, 15),
						new Version.ECB(41, 16)
					})
				}),
				new Version(36, new int[]
				{
					6,
					24,
					50,
					76,
					102,
					128,
					154
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(6, 121),
						new Version.ECB(14, 122)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(6, 47),
						new Version.ECB(34, 48)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(46, 24),
						new Version.ECB(10, 25)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(2, 15),
						new Version.ECB(64, 16)
					})
				}),
				new Version(37, new int[]
				{
					6,
					28,
					54,
					80,
					106,
					132,
					158
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(17, 122),
						new Version.ECB(4, 123)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(29, 46),
						new Version.ECB(14, 47)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(49, 24),
						new Version.ECB(10, 25)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(24, 15),
						new Version.ECB(46, 16)
					})
				}),
				new Version(38, new int[]
				{
					6,
					32,
					58,
					84,
					110,
					136,
					162
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(4, 122),
						new Version.ECB(18, 123)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(13, 46),
						new Version.ECB(32, 47)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(48, 24),
						new Version.ECB(14, 25)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(42, 15),
						new Version.ECB(32, 16)
					})
				}),
				new Version(39, new int[]
				{
					6,
					26,
					54,
					82,
					110,
					138,
					166
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(20, 117),
						new Version.ECB(4, 118)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(40, 47),
						new Version.ECB(7, 48)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(43, 24),
						new Version.ECB(22, 25)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(10, 15),
						new Version.ECB(67, 16)
					})
				}),
				new Version(40, new int[]
				{
					6,
					30,
					58,
					86,
					114,
					142,
					170
				}, new Version.ECBlocks[]
				{
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(19, 118),
						new Version.ECB(6, 119)
					}),
					new Version.ECBlocks(28, new Version.ECB[]
					{
						new Version.ECB(18, 47),
						new Version.ECB(31, 48)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(34, 24),
						new Version.ECB(34, 25)
					}),
					new Version.ECBlocks(30, new Version.ECB[]
					{
						new Version.ECB(20, 15),
						new Version.ECB(61, 16)
					})
				})
			};
		}

		// Token: 0x0600041F RID: 1055 RVA: 0x00022F1C File Offset: 0x0002111C
		internal static Version decodeVersionInformation(int versionBits)
		{
			int num = 2147483647;
			int num2 = 0;
			for (int i = 0; i < Version.VERSION_DECODE_INFO.Length; i++)
			{
				int num3 = Version.VERSION_DECODE_INFO[i];
				if (num3 == versionBits)
				{
					return Version.getVersionForNumber(i + 7);
				}
				int num4 = FormatInformation.numBitsDiffering(versionBits, num3);
				if (num4 < num)
				{
					num2 = i + 7;
					num = num4;
				}
			}
			if (num <= 3)
			{
				return Version.getVersionForNumber(num2);
			}
			return null;
		}

		// Token: 0x0600041C RID: 1052 RVA: 0x00022EB3 File Offset: 0x000210B3
		public Version.ECBlocks getECBlocksForLevel(ErrorCorrectionLevel ecLevel)
		{
			return this.ecBlocks[ecLevel.ordinal()];
		}

		// Token: 0x0600041D RID: 1053 RVA: 0x00022EC4 File Offset: 0x000210C4
		public static Version getProvisionalVersionForDimension(int dimension)
		{
			if (dimension % 4 != 1)
			{
				return null;
			}
			Version result;
			try
			{
				result = Version.getVersionForNumber(dimension - 17 >> 2);
			}
			catch (ArgumentException)
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600041E RID: 1054 RVA: 0x00022F00 File Offset: 0x00021100
		public static Version getVersionForNumber(int versionNumber)
		{
			if (versionNumber < 1 || versionNumber > 40)
			{
				throw new ArgumentException();
			}
			return Version.VERSIONS[versionNumber - 1];
		}

		// Token: 0x06000421 RID: 1057 RVA: 0x0002305B File Offset: 0x0002125B
		public override string ToString()
		{
			return Convert.ToString(this.versionNumber);
		}

		// Token: 0x1700004D RID: 77
		public int[] AlignmentPatternCenters
		{
			// Token: 0x06000419 RID: 1049 RVA: 0x00022E96 File Offset: 0x00021096
			get
			{
				return this.alignmentPatternCenters;
			}
		}

		// Token: 0x1700004F RID: 79
		public int DimensionForVersion
		{
			// Token: 0x0600041B RID: 1051 RVA: 0x00022EA6 File Offset: 0x000210A6
			get
			{
				return 17 + 4 * this.versionNumber;
			}
		}

		// Token: 0x1700004E RID: 78
		public int TotalCodewords
		{
			// Token: 0x0600041A RID: 1050 RVA: 0x00022E9E File Offset: 0x0002109E
			get
			{
				return this.totalCodewords;
			}
		}

		// Token: 0x1700004C RID: 76
		public int VersionNumber
		{
			// Token: 0x06000418 RID: 1048 RVA: 0x00022E8E File Offset: 0x0002108E
			get
			{
				return this.versionNumber;
			}
		}

		// Token: 0x040002FE RID: 766
		private readonly int[] alignmentPatternCenters;

		// Token: 0x040002FF RID: 767
		private readonly Version.ECBlocks[] ecBlocks;

		// Token: 0x04000300 RID: 768
		private readonly int totalCodewords;

		// Token: 0x040002FD RID: 765
		private readonly int versionNumber;

		// Token: 0x040002FC RID: 764
		private static readonly Version[] VERSIONS = Version.buildVersions();

		// Token: 0x040002FB RID: 763
		private static readonly int[] VERSION_DECODE_INFO = new int[]
		{
			31892,
			34236,
			39577,
			42195,
			48118,
			51042,
			55367,
			58893,
			63784,
			68472,
			70749,
			76311,
			79154,
			84390,
			87683,
			92361,
			96236,
			102084,
			102881,
			110507,
			110734,
			117786,
			119615,
			126325,
			127568,
			133589,
			136944,
			141498,
			145311,
			150283,
			152622,
			158308,
			161089,
			167017
		};

		// Token: 0x020000C4 RID: 196
		public sealed class ECB
		{
			// Token: 0x060005AF RID: 1455 RVA: 0x0002CB0D File Offset: 0x0002AD0D
			internal ECB(int count, int dataCodewords)
			{
				this.count = count;
				this.dataCodewords = dataCodewords;
			}

			// Token: 0x17000093 RID: 147
			public int Count
			{
				// Token: 0x060005B0 RID: 1456 RVA: 0x0002CB23 File Offset: 0x0002AD23
				get
				{
					return this.count;
				}
			}

			// Token: 0x17000094 RID: 148
			public int DataCodewords
			{
				// Token: 0x060005B1 RID: 1457 RVA: 0x0002CB2B File Offset: 0x0002AD2B
				get
				{
					return this.dataCodewords;
				}
			}

			// Token: 0x04000475 RID: 1141
			private readonly int count;

			// Token: 0x04000476 RID: 1142
			private readonly int dataCodewords;
		}

		// Token: 0x020000C3 RID: 195
		public sealed class ECBlocks
		{
			// Token: 0x060005AA RID: 1450 RVA: 0x0002CAA3 File Offset: 0x0002ACA3
			internal ECBlocks(int ecCodewordsPerBlock, params Version.ECB[] ecBlocks)
			{
				this.ecCodewordsPerBlock = ecCodewordsPerBlock;
				this.ecBlocks = ecBlocks;
			}

			// Token: 0x060005AE RID: 1454 RVA: 0x0002CB05 File Offset: 0x0002AD05
			public Version.ECB[] getECBlocks()
			{
				return this.ecBlocks;
			}

			// Token: 0x17000090 RID: 144
			public int ECCodewordsPerBlock
			{
				// Token: 0x060005AB RID: 1451 RVA: 0x0002CAB9 File Offset: 0x0002ACB9
				get
				{
					return this.ecCodewordsPerBlock;
				}
			}

			// Token: 0x17000091 RID: 145
			public int NumBlocks
			{
				// Token: 0x060005AC RID: 1452 RVA: 0x0002CAC4 File Offset: 0x0002ACC4
				get
				{
					int num = 0;
					Version.ECB[] array = this.ecBlocks;
					for (int i = 0; i < array.Length; i++)
					{
						Version.ECB eCB = array[i];
						num += eCB.Count;
					}
					return num;
				}
			}

			// Token: 0x17000092 RID: 146
			public int TotalECCodewords
			{
				// Token: 0x060005AD RID: 1453 RVA: 0x0002CAF6 File Offset: 0x0002ACF6
				get
				{
					return this.ecCodewordsPerBlock * this.NumBlocks;
				}
			}

			// Token: 0x04000474 RID: 1140
			private readonly Version.ECB[] ecBlocks;

			// Token: 0x04000473 RID: 1139
			private readonly int ecCodewordsPerBlock;
		}
	}
}
