using System;
using System.Collections.Generic;
using ZXing.Common;

namespace ZXing.QrCode.Internal
{
	// Token: 0x02000076 RID: 118
	internal sealed class AlignmentPatternFinder
	{
		// Token: 0x06000427 RID: 1063 RVA: 0x00024D4C File Offset: 0x00022F4C
		internal AlignmentPatternFinder(BitMatrix image, int startX, int startY, int width, int height, float moduleSize, ResultPointCallback resultPointCallback)
		{
			this.image = image;
			this.possibleCenters = new List<AlignmentPattern>(5);
			this.startX = startX;
			this.startY = startY;
			this.width = width;
			this.height = height;
			this.moduleSize = moduleSize;
			this.crossCheckStateCount = new int[3];
			this.resultPointCallback = resultPointCallback;
		}

		// Token: 0x06000429 RID: 1065 RVA: 0x00024F20 File Offset: 0x00023120
		private static float? centerFromEnd(int[] stateCount, int end)
		{
			float num = (float)(end - stateCount[2]) - (float)stateCount[1] / 2f;
			if (float.IsNaN(num))
			{
				return null;
			}
			return new float?(num);
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x00024F94 File Offset: 0x00023194
		private float? crossCheckVertical(int startI, int centerJ, int maxCount, int originalStateCountTotal)
		{
			int num = this.image.Height;
			int[] array = this.crossCheckStateCount;
			array[0] = 0;
			array[1] = 0;
			array[2] = 0;
			int num2 = startI;
			while (num2 >= 0 && this.image[centerJ, num2] && array[1] <= maxCount)
			{
				array[1]++;
				num2--;
			}
			if (num2 < 0 || array[1] > maxCount)
			{
				return null;
			}
			while (num2 >= 0 && !this.image[centerJ, num2] && array[0] <= maxCount)
			{
				array[0]++;
				num2--;
			}
			if (array[0] > maxCount)
			{
				return null;
			}
			num2 = startI + 1;
			while (num2 < num && this.image[centerJ, num2] && array[1] <= maxCount)
			{
				array[1]++;
				num2++;
			}
			if (num2 == num || array[1] > maxCount)
			{
				return null;
			}
			while (num2 < num && !this.image[centerJ, num2] && array[2] <= maxCount)
			{
				array[2]++;
				num2++;
			}
			if (array[2] > maxCount)
			{
				return null;
			}
			int num3 = array[0] + array[1] + array[2];
			if (5 * Math.Abs(num3 - originalStateCountTotal) >= 2 * originalStateCountTotal)
			{
				return null;
			}
			if (!this.foundPatternCross(array))
			{
				return null;
			}
			return AlignmentPatternFinder.centerFromEnd(array, num2);
		}

		// Token: 0x06000428 RID: 1064 RVA: 0x00024DAC File Offset: 0x00022FAC
		internal AlignmentPattern find()
		{
			int num = this.startX;
			int num2 = this.height;
			int num3 = num + this.width;
			int num4 = this.startY + (num2 >> 1);
			int[] array = new int[3];
			for (int i = 0; i < num2; i++)
			{
				int num5 = num4 + (((i & 1) == 0) ? (i + 1 >> 1) : (-(i + 1 >> 1)));
				array[0] = 0;
				array[1] = 0;
				array[2] = 0;
				int j = num;
				while (j < num3 && !this.image[j, num5])
				{
					j++;
				}
				int num6 = 0;
				while (j < num3)
				{
					if (this.image[j, num5])
					{
						if (num6 == 1)
						{
							array[num6]++;
						}
						else if (num6 == 2)
						{
							if (this.foundPatternCross(array))
							{
								AlignmentPattern alignmentPattern = this.handlePossibleCenter(array, num5, j);
								if (alignmentPattern != null)
								{
									return alignmentPattern;
								}
							}
							array[0] = array[2];
							array[1] = 1;
							array[2] = 0;
							num6 = 1;
						}
						else
						{
							array[++num6]++;
						}
					}
					else
					{
						if (num6 == 1)
						{
							num6++;
						}
						array[num6]++;
					}
					j++;
				}
				if (this.foundPatternCross(array))
				{
					AlignmentPattern alignmentPattern2 = this.handlePossibleCenter(array, num5, num3);
					if (alignmentPattern2 != null)
					{
						return alignmentPattern2;
					}
				}
			}
			if (this.possibleCenters.Count != 0)
			{
				return this.possibleCenters[0];
			}
			return null;
		}

		// Token: 0x0600042A RID: 1066 RVA: 0x00024F58 File Offset: 0x00023158
		private bool foundPatternCross(int[] stateCount)
		{
			float num = this.moduleSize / 2f;
			for (int i = 0; i < 3; i++)
			{
				if (Math.Abs(this.moduleSize - (float)stateCount[i]) >= num)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x00025100 File Offset: 0x00023300
		private AlignmentPattern handlePossibleCenter(int[] stateCount, int i, int j)
		{
			int originalStateCountTotal = stateCount[0] + stateCount[1] + stateCount[2];
			float? num = AlignmentPatternFinder.centerFromEnd(stateCount, j);
			if (!num.HasValue)
			{
				return null;
			}
			float? num2 = this.crossCheckVertical(i, (int)num.Value, 2 * stateCount[1], originalStateCountTotal);
			if (num2.HasValue)
			{
				float num3 = (float)(stateCount[0] + stateCount[1] + stateCount[2]) / 3f;
				foreach (AlignmentPattern current in this.possibleCenters)
				{
					if (current.aboutEquals(num3, num2.Value, num.Value))
					{
						AlignmentPattern result = current.combineEstimate(num2.Value, num.Value, num3);
						return result;
					}
				}
				AlignmentPattern alignmentPattern = new AlignmentPattern(num.Value, num2.Value, num3);
				this.possibleCenters.Add(alignmentPattern);
				if (this.resultPointCallback != null)
				{
					this.resultPointCallback(alignmentPattern);
				}
			}
			return null;
		}

		// Token: 0x04000309 RID: 777
		private readonly int[] crossCheckStateCount;

		// Token: 0x04000307 RID: 775
		private readonly int height;

		// Token: 0x04000302 RID: 770
		private readonly BitMatrix image;

		// Token: 0x04000308 RID: 776
		private readonly float moduleSize;

		// Token: 0x04000303 RID: 771
		private readonly IList<AlignmentPattern> possibleCenters;

		// Token: 0x0400030A RID: 778
		private readonly ResultPointCallback resultPointCallback;

		// Token: 0x04000304 RID: 772
		private readonly int startX;

		// Token: 0x04000305 RID: 773
		private readonly int startY;

		// Token: 0x04000306 RID: 774
		private readonly int width;
	}
}
