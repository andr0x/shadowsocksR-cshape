using System;
using System.Collections.Generic;
using ZXing.Common;

namespace ZXing.QrCode.Internal
{
	// Token: 0x0200007B RID: 123
	public class FinderPatternFinder
	{
		// Token: 0x0600044A RID: 1098 RVA: 0x00024B29 File Offset: 0x00022D29
		public FinderPatternFinder(BitMatrix image) : this(image, null)
		{
		}

		// Token: 0x0600044B RID: 1099 RVA: 0x00024B33 File Offset: 0x00022D33
		public FinderPatternFinder(BitMatrix image, ResultPointCallback resultPointCallback)
		{
			this.image = image;
			this.possibleCenters = new List<FinderPattern>();
			this.crossCheckStateCount = new int[5];
			this.resultPointCallback = resultPointCallback;
		}

		// Token: 0x0600044F RID: 1103 RVA: 0x00024D94 File Offset: 0x00022F94
		private static float? centerFromEnd(int[] stateCount, int end)
		{
			float num = (float)(end - stateCount[4] - stateCount[3]) - (float)stateCount[2] / 2f;
			if (float.IsNaN(num))
			{
				return null;
			}
			return new float?(num);
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x00024E94 File Offset: 0x00023094
		private bool crossCheckDiagonal(int startI, int centerJ, int maxCount, int originalStateCountTotal)
		{
			int height = this.image.Height;
			int width = this.image.Width;
			int[] array = this.CrossCheckStateCount;
			int num = 0;
			while (startI - num >= 0 && this.image[centerJ - num, startI - num])
			{
				array[2]++;
				num++;
			}
			if (startI - num < 0 || centerJ - num < 0)
			{
				return false;
			}
			while (startI - num >= 0 && centerJ - num >= 0 && !this.image[centerJ - num, startI - num] && array[1] <= maxCount)
			{
				array[1]++;
				num++;
			}
			if (startI - num < 0 || centerJ - num < 0 || array[1] > maxCount)
			{
				return false;
			}
			while (startI - num >= 0 && centerJ - num >= 0 && this.image[centerJ - num, startI - num] && array[0] <= maxCount)
			{
				array[0]++;
				num++;
			}
			if (array[0] > maxCount)
			{
				return false;
			}
			num = 1;
			while (startI + num < height && centerJ + num < width && this.image[centerJ + num, startI + num])
			{
				array[2]++;
				num++;
			}
			if (startI + num >= height || centerJ + num >= width)
			{
				return false;
			}
			while (startI + num < height && centerJ + num < width && !this.image[centerJ + num, startI + num] && array[3] < maxCount)
			{
				array[3]++;
				num++;
			}
			if (startI + num >= height || centerJ + num >= width || array[3] >= maxCount)
			{
				return false;
			}
			while (startI + num < height && centerJ + num < width && this.image[centerJ + num, startI + num] && array[4] < maxCount)
			{
				array[4]++;
				num++;
			}
			return array[4] < maxCount && Math.Abs(array[0] + array[1] + array[2] + array[3] + array[4] - originalStateCountTotal) < 2 * originalStateCountTotal && FinderPatternFinder.foundPatternCross(array);
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x0002523C File Offset: 0x0002343C
		private float? crossCheckHorizontal(int startJ, int centerI, int maxCount, int originalStateCountTotal)
		{
			int width = this.image.Width;
			int[] array = this.CrossCheckStateCount;
			int num = startJ;
			while (num >= 0 && this.image[num, centerI])
			{
				array[2]++;
				num--;
			}
			if (num < 0)
			{
				return null;
			}
			while (num >= 0 && !this.image[num, centerI] && array[1] <= maxCount)
			{
				array[1]++;
				num--;
			}
			if (num < 0 || array[1] > maxCount)
			{
				return null;
			}
			while (num >= 0 && this.image[num, centerI] && array[0] <= maxCount)
			{
				array[0]++;
				num--;
			}
			if (array[0] > maxCount)
			{
				return null;
			}
			num = startJ + 1;
			while (num < width && this.image[num, centerI])
			{
				array[2]++;
				num++;
			}
			if (num == width)
			{
				return null;
			}
			while (num < width && !this.image[num, centerI] && array[3] < maxCount)
			{
				array[3]++;
				num++;
			}
			if (num == width || array[3] >= maxCount)
			{
				return null;
			}
			while (num < width && this.image[num, centerI] && array[4] < maxCount)
			{
				array[4]++;
				num++;
			}
			if (array[4] >= maxCount)
			{
				return null;
			}
			int num2 = array[0] + array[1] + array[2] + array[3] + array[4];
			if (5 * Math.Abs(num2 - originalStateCountTotal) >= originalStateCountTotal)
			{
				return null;
			}
			if (!FinderPatternFinder.foundPatternCross(array))
			{
				return null;
			}
			return FinderPatternFinder.centerFromEnd(array, num);
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x00025074 File Offset: 0x00023274
		private float? crossCheckVertical(int startI, int centerJ, int maxCount, int originalStateCountTotal)
		{
			int height = this.image.Height;
			int[] array = this.CrossCheckStateCount;
			int num = startI;
			while (num >= 0 && this.image[centerJ, num])
			{
				array[2]++;
				num--;
			}
			if (num < 0)
			{
				return null;
			}
			while (num >= 0 && !this.image[centerJ, num] && array[1] <= maxCount)
			{
				array[1]++;
				num--;
			}
			if (num < 0 || array[1] > maxCount)
			{
				return null;
			}
			while (num >= 0 && this.image[centerJ, num] && array[0] <= maxCount)
			{
				array[0]++;
				num--;
			}
			if (array[0] > maxCount)
			{
				return null;
			}
			num = startI + 1;
			while (num < height && this.image[centerJ, num])
			{
				array[2]++;
				num++;
			}
			if (num == height)
			{
				return null;
			}
			while (num < height && !this.image[centerJ, num] && array[3] < maxCount)
			{
				array[3]++;
				num++;
			}
			if (num == height || array[3] >= maxCount)
			{
				return null;
			}
			while (num < height && this.image[centerJ, num] && array[4] < maxCount)
			{
				array[4]++;
				num++;
			}
			if (array[4] >= maxCount)
			{
				return null;
			}
			int num2 = array[0] + array[1] + array[2] + array[3] + array[4];
			if (5 * Math.Abs(num2 - originalStateCountTotal) >= 2 * originalStateCountTotal)
			{
				return null;
			}
			if (!FinderPatternFinder.foundPatternCross(array))
			{
				return null;
			}
			return FinderPatternFinder.centerFromEnd(array, num);
		}

		// Token: 0x0600044E RID: 1102 RVA: 0x00024B70 File Offset: 0x00022D70
		internal virtual FinderPatternInfo find(IDictionary<DecodeHintType, object> hints)
		{
			bool flag = hints != null && hints.ContainsKey(DecodeHintType.TRY_HARDER);
			bool pureBarcode = hints != null && hints.ContainsKey(DecodeHintType.PURE_BARCODE);
			int height = this.image.Height;
			int width = this.image.Width;
			int num = 3 * height / 228;
			if (num < 3 | flag)
			{
				num = 3;
			}
			bool flag2 = false;
			int[] array = new int[5];
			int num2 = num - 1;
			while (num2 < height && !flag2)
			{
				array[0] = 0;
				array[1] = 0;
				array[2] = 0;
				array[3] = 0;
				array[4] = 0;
				int num3 = 0;
				for (int i = 0; i < width; i++)
				{
					if (this.image[i, num2])
					{
						if ((num3 & 1) == 1)
						{
							num3++;
						}
						array[num3]++;
					}
					else if ((num3 & 1) == 0)
					{
						if (num3 == 4)
						{
							if (FinderPatternFinder.foundPatternCross(array))
							{
								if (this.handlePossibleCenter(array, num2, i, pureBarcode))
								{
									num = 2;
									if (this.hasSkipped)
									{
										flag2 = this.haveMultiplyConfirmedCenters();
									}
									else
									{
										int num4 = this.findRowSkip();
										if (num4 > array[2])
										{
											num2 += num4 - array[2] - num;
											i = width - 1;
										}
									}
									num3 = 0;
									array[0] = 0;
									array[1] = 0;
									array[2] = 0;
									array[3] = 0;
									array[4] = 0;
								}
								else
								{
									array[0] = array[2];
									array[1] = array[3];
									array[2] = array[4];
									array[3] = 1;
									array[4] = 0;
									num3 = 3;
								}
							}
							else
							{
								array[0] = array[2];
								array[1] = array[3];
								array[2] = array[4];
								array[3] = 1;
								array[4] = 0;
								num3 = 3;
							}
						}
						else
						{
							array[++num3]++;
						}
					}
					else
					{
						array[num3]++;
					}
				}
				if (FinderPatternFinder.foundPatternCross(array) && this.handlePossibleCenter(array, num2, width, pureBarcode))
				{
					num = array[0];
					if (this.hasSkipped)
					{
						flag2 = this.haveMultiplyConfirmedCenters();
					}
				}
				num2 += num;
			}
			FinderPattern[] array2 = this.selectBestPatterns();
			if (array2 == null)
			{
				return null;
			}
			ResultPoint.orderBestPatterns(array2);
			return new FinderPatternInfo(array2);
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x00025564 File Offset: 0x00023764
		private int findRowSkip()
		{
			if (this.possibleCenters.Count <= 1)
			{
				return 0;
			}
			ResultPoint resultPoint = null;
			foreach (FinderPattern current in this.possibleCenters)
			{
				if (current.Count >= 2)
				{
					if (resultPoint != null)
					{
						this.hasSkipped = true;
						return (int)(Math.Abs(resultPoint.X - current.X) - Math.Abs(resultPoint.Y - current.Y)) / 2;
					}
					resultPoint = current;
				}
			}
			return 0;
		}

		// Token: 0x06000450 RID: 1104 RVA: 0x00024DD0 File Offset: 0x00022FD0
		protected internal static bool foundPatternCross(int[] stateCount)
		{
			int num = 0;
			for (int i = 0; i < 5; i++)
			{
				int num2 = stateCount[i];
				if (num2 == 0)
				{
					return false;
				}
				num += num2;
			}
			if (num < 7)
			{
				return false;
			}
			int num3 = (num << 8) / 7;
			int num4 = num3 / 2;
			return Math.Abs(num3 - (stateCount[0] << 8)) < num4 && Math.Abs(num3 - (stateCount[1] << 8)) < num4 && Math.Abs(3 * num3 - (stateCount[2] << 8)) < 3 * num4 && Math.Abs(num3 - (stateCount[3] << 8)) < num4 && Math.Abs(num3 - (stateCount[4] << 8)) < num4;
		}

		// Token: 0x06000455 RID: 1109 RVA: 0x00025404 File Offset: 0x00023604
		protected bool handlePossibleCenter(int[] stateCount, int i, int j, bool pureBarcode)
		{
			int num = stateCount[0] + stateCount[1] + stateCount[2] + stateCount[3] + stateCount[4];
			float? num2 = FinderPatternFinder.centerFromEnd(stateCount, j);
			if (!num2.HasValue)
			{
				return false;
			}
			float? num3 = this.crossCheckVertical(i, (int)num2.Value, stateCount[2], num);
			if (num3.HasValue)
			{
				num2 = this.crossCheckHorizontal((int)num2.Value, (int)num3.Value, stateCount[2], num);
				if (num2.HasValue && (!pureBarcode || this.crossCheckDiagonal((int)num3.Value, (int)num2.Value, stateCount[2], num)))
				{
					float num4 = (float)num / 7f;
					bool flag = false;
					for (int k = 0; k < this.possibleCenters.Count; k++)
					{
						FinderPattern finderPattern = this.possibleCenters[k];
						if (finderPattern.aboutEquals(num4, num3.Value, num2.Value))
						{
							this.possibleCenters.RemoveAt(k);
							this.possibleCenters.Insert(k, finderPattern.combineEstimate(num3.Value, num2.Value, num4));
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						FinderPattern finderPattern2 = new FinderPattern(num2.Value, num3.Value, num4);
						this.possibleCenters.Add(finderPattern2);
						if (this.resultPointCallback != null)
						{
							this.resultPointCallback(finderPattern2);
						}
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x00025608 File Offset: 0x00023808
		private bool haveMultiplyConfirmedCenters()
		{
			int num = 0;
			float num2 = 0f;
			int count = this.possibleCenters.Count;
			foreach (FinderPattern current in this.possibleCenters)
			{
				if (current.Count >= 2)
				{
					num++;
					num2 += current.EstimatedModuleSize;
				}
			}
			if (num < 3)
			{
				return false;
			}
			float num3 = num2 / (float)count;
			float num4 = 0f;
			for (int i = 0; i < count; i++)
			{
				FinderPattern finderPattern = this.possibleCenters[i];
				num4 += Math.Abs(finderPattern.EstimatedModuleSize - num3);
			}
			return num4 <= 0.05f * num2;
		}

		// Token: 0x06000458 RID: 1112 RVA: 0x000256D4 File Offset: 0x000238D4
		private FinderPattern[] selectBestPatterns()
		{
			int count = this.possibleCenters.Count;
			if (count < 3)
			{
				return null;
			}
			if (count > 3)
			{
				float num = 0f;
				float num2 = 0f;
				using (List<FinderPattern>.Enumerator enumerator = this.possibleCenters.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						float estimatedModuleSize = enumerator.Current.EstimatedModuleSize;
						num += estimatedModuleSize;
						float arg_4C_0 = num2;
						float expr_4A = estimatedModuleSize;
						num2 = arg_4C_0 + expr_4A * expr_4A;
					}
				}
				float num3 = num / (float)count;
				double arg_73_0 = (double)(num2 / (float)count);
				float expr_71 = num3;
				float val = (float)Math.Sqrt(arg_73_0 - (double)(expr_71 * expr_71));
				this.possibleCenters.Sort(new FinderPatternFinder.FurthestFromAverageComparator(num3));
				float num4 = Math.Max(0.2f * num3, val);
				int num5 = 0;
				while (num5 < this.possibleCenters.Count && this.possibleCenters.Count > 3)
				{
					if (Math.Abs(this.possibleCenters[num5].EstimatedModuleSize - num3) > num4)
					{
						this.possibleCenters.RemoveAt(num5);
						num5--;
					}
					num5++;
				}
			}
			if (this.possibleCenters.Count > 3)
			{
				float num6 = 0f;
				foreach (FinderPattern current in this.possibleCenters)
				{
					num6 += current.EstimatedModuleSize;
				}
				float f = num6 / (float)this.possibleCenters.Count;
				this.possibleCenters.Sort(new FinderPatternFinder.CenterComparator(f));
				this.possibleCenters = this.possibleCenters.GetRange(0, 3);
			}
			return new FinderPattern[]
			{
				this.possibleCenters[0],
				this.possibleCenters[1],
				this.possibleCenters[2]
			};
		}

		// Token: 0x17000059 RID: 89
		private int[] CrossCheckStateCount
		{
			// Token: 0x06000451 RID: 1105 RVA: 0x00024E5C File Offset: 0x0002305C
			get
			{
				this.crossCheckStateCount[0] = 0;
				this.crossCheckStateCount[1] = 0;
				this.crossCheckStateCount[2] = 0;
				this.crossCheckStateCount[3] = 0;
				this.crossCheckStateCount[4] = 0;
				return this.crossCheckStateCount;
			}
		}

		// Token: 0x17000057 RID: 87
		protected internal virtual BitMatrix Image
		{
			// Token: 0x0600044C RID: 1100 RVA: 0x00024B60 File Offset: 0x00022D60
			get
			{
				return this.image;
			}
		}

		// Token: 0x17000058 RID: 88
		protected internal virtual List<FinderPattern> PossibleCenters
		{
			// Token: 0x0600044D RID: 1101 RVA: 0x00024B68 File Offset: 0x00022D68
			get
			{
				return this.possibleCenters;
			}
		}

		// Token: 0x0400030C RID: 780
		private const int CENTER_QUORUM = 2;

		// Token: 0x04000313 RID: 787
		private readonly int[] crossCheckStateCount;

		// Token: 0x04000312 RID: 786
		private bool hasSkipped;

		// Token: 0x04000310 RID: 784
		private readonly BitMatrix image;

		// Token: 0x0400030F RID: 783
		private const int INTEGER_MATH_SHIFT = 8;

		// Token: 0x0400030E RID: 782
		protected internal const int MAX_MODULES = 57;

		// Token: 0x0400030D RID: 781
		protected internal const int MIN_SKIP = 3;

		// Token: 0x04000311 RID: 785
		private List<FinderPattern> possibleCenters;

		// Token: 0x04000314 RID: 788
		private readonly ResultPointCallback resultPointCallback;

		// Token: 0x020000C7 RID: 199
		private sealed class CenterComparator : IComparer<FinderPattern>
		{
			// Token: 0x060005B9 RID: 1465 RVA: 0x0002BDCC File Offset: 0x00029FCC
			public CenterComparator(float f)
			{
				this.average = f;
			}

			// Token: 0x060005BA RID: 1466 RVA: 0x0002BDDC File Offset: 0x00029FDC
			public int Compare(FinderPattern x, FinderPattern y)
			{
				if (y.Count != x.Count)
				{
					return y.Count - x.Count;
				}
				float num = Math.Abs(y.EstimatedModuleSize - this.average);
				float num2 = Math.Abs(x.EstimatedModuleSize - this.average);
				if (num < num2)
				{
					return 1;
				}
				if (num != num2)
				{
					return -1;
				}
				return 0;
			}

			// Token: 0x04000475 RID: 1141
			private readonly float average;
		}

		// Token: 0x020000C6 RID: 198
		private sealed class FurthestFromAverageComparator : IComparer<FinderPattern>
		{
			// Token: 0x060005B7 RID: 1463 RVA: 0x0002BD7B File Offset: 0x00029F7B
			public FurthestFromAverageComparator(float f)
			{
				this.average = f;
			}

			// Token: 0x060005B8 RID: 1464 RVA: 0x0002BD8C File Offset: 0x00029F8C
			public int Compare(FinderPattern x, FinderPattern y)
			{
				float num = Math.Abs(y.EstimatedModuleSize - this.average);
				float num2 = Math.Abs(x.EstimatedModuleSize - this.average);
				if (num < num2)
				{
					return -1;
				}
				if (num != num2)
				{
					return 1;
				}
				return 0;
			}

			// Token: 0x04000474 RID: 1140
			private readonly float average;
		}
	}
}
