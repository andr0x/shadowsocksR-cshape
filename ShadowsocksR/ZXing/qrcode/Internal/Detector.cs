﻿using System;
using System.Collections.Generic;
using ZXing.Common;
using ZXing.Common.Detector;

namespace ZXing.QrCode.Internal
{
	// Token: 0x02000079 RID: 121
	public class Detector
	{
		// Token: 0x06000436 RID: 1078 RVA: 0x00024458 File Offset: 0x00022658
		public Detector(BitMatrix image)
		{
			this.image = image;
		}

		// Token: 0x0600043F RID: 1087 RVA: 0x00024754 File Offset: 0x00022954
		protected internal virtual float calculateModuleSize(ResultPoint topLeft, ResultPoint topRight, ResultPoint bottomLeft)
		{
			return (this.calculateModuleSizeOneWay(topLeft, topRight) + this.calculateModuleSizeOneWay(topLeft, bottomLeft)) / 2f;
		}

		// Token: 0x06000440 RID: 1088 RVA: 0x00024770 File Offset: 0x00022970
		private float calculateModuleSizeOneWay(ResultPoint pattern, ResultPoint otherPattern)
		{
			float num = this.sizeOfBlackWhiteBlackRunBothWays((int)pattern.X, (int)pattern.Y, (int)otherPattern.X, (int)otherPattern.Y);
			float num2 = this.sizeOfBlackWhiteBlackRunBothWays((int)otherPattern.X, (int)otherPattern.Y, (int)pattern.X, (int)pattern.Y);
			if (float.IsNaN(num))
			{
				return num2 / 7f;
			}
			if (float.IsNaN(num2))
			{
				return num / 7f;
			}
			return (num + num2) / 14f;
		}

		// Token: 0x0600043E RID: 1086 RVA: 0x000246EC File Offset: 0x000228EC
		private static bool computeDimension(ResultPoint topLeft, ResultPoint topRight, ResultPoint bottomLeft, float moduleSize, out int dimension)
		{
			int num = MathUtils.round(ResultPoint.distance(topLeft, topRight) / moduleSize);
			int num2 = MathUtils.round(ResultPoint.distance(topLeft, bottomLeft) / moduleSize);
			dimension = (num + num2 >> 1) + 7;
			switch (dimension & 3)
			{
			case 0:
				dimension++;
				break;
			case 2:
				dimension--;
				break;
			case 3:
				return true;
			}
			return true;
		}

		// Token: 0x0600043C RID: 1084 RVA: 0x00024634 File Offset: 0x00022834
		private static PerspectiveTransform createTransform(ResultPoint topLeft, ResultPoint topRight, ResultPoint bottomLeft, ResultPoint alignmentPattern, int dimension)
		{
			float num = (float)dimension - 3.5f;
			float x2p;
			float y2p;
			float x;
			float y;
			if (alignmentPattern != null)
			{
				x2p = alignmentPattern.X;
				y2p = alignmentPattern.Y;
				y = (x = num - 3f);
			}
			else
			{
				x2p = topRight.X - topLeft.X + bottomLeft.X;
				y2p = topRight.Y - topLeft.Y + bottomLeft.Y;
				y = (x = num);
			}
			return PerspectiveTransform.quadrilateralToQuadrilateral(3.5f, 3.5f, num, 3.5f, x, y, 3.5f, num, topLeft.X, topLeft.Y, topRight.X, topRight.Y, x2p, y2p, bottomLeft.X, bottomLeft.Y);
		}

		// Token: 0x06000439 RID: 1081 RVA: 0x00024477 File Offset: 0x00022677
		public virtual DetectorResult detect()
		{
			return this.detect(null);
		}

		// Token: 0x0600043A RID: 1082 RVA: 0x00024480 File Offset: 0x00022680
		public virtual DetectorResult detect(IDictionary<DecodeHintType, object> hints)
		{
			this.resultPointCallback = ((hints == null || !hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK)) ? null : ((ResultPointCallback)hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK]));
			FinderPatternInfo finderPatternInfo = new FinderPatternFinder(this.image, this.resultPointCallback).find(hints);
			if (finderPatternInfo == null)
			{
				return null;
			}
			return this.processFinderPatternInfo(finderPatternInfo);
		}

		// Token: 0x06000443 RID: 1091 RVA: 0x000249B4 File Offset: 0x00022BB4
		protected AlignmentPattern findAlignmentInRegion(float overallEstModuleSize, int estAlignmentX, int estAlignmentY, float allowanceFactor)
		{
			int num = (int)(allowanceFactor * overallEstModuleSize);
			int num2 = Math.Max(0, estAlignmentX - num);
			int num3 = Math.Min(this.image.Width - 1, estAlignmentX + num);
			if ((float)(num3 - num2) < overallEstModuleSize * 3f)
			{
				return null;
			}
			int num4 = Math.Max(0, estAlignmentY - num);
			int num5 = Math.Min(this.image.Height - 1, estAlignmentY + num);
			return new AlignmentPatternFinder(this.image, num2, num4, num3 - num2, num5 - num4, overallEstModuleSize, this.resultPointCallback).find();
		}

		// Token: 0x0600043B RID: 1083 RVA: 0x000244D4 File Offset: 0x000226D4
		protected internal virtual DetectorResult processFinderPatternInfo(FinderPatternInfo info)
		{
			FinderPattern topLeft = info.TopLeft;
			FinderPattern topRight = info.TopRight;
			FinderPattern bottomLeft = info.BottomLeft;
			float num = this.calculateModuleSize(topLeft, topRight, bottomLeft);
			if (num < 1f)
			{
				return null;
			}
			int dimension;
			if (!Detector.computeDimension(topLeft, topRight, bottomLeft, num, out dimension))
			{
				return null;
			}
			Version provisionalVersionForDimension = Version.getProvisionalVersionForDimension(dimension);
			if (provisionalVersionForDimension == null)
			{
				return null;
			}
			int num2 = provisionalVersionForDimension.DimensionForVersion - 7;
			AlignmentPattern alignmentPattern = null;
			if (provisionalVersionForDimension.AlignmentPatternCenters.Length != 0)
			{
				float num3 = topRight.X - topLeft.X + bottomLeft.X;
				float num4 = topRight.Y - topLeft.Y + bottomLeft.Y;
				float num5 = 1f - 3f / (float)num2;
				int estAlignmentX = (int)(topLeft.X + num5 * (num3 - topLeft.X));
				int estAlignmentY = (int)(topLeft.Y + num5 * (num4 - topLeft.Y));
				for (int i = 4; i <= 16; i <<= 1)
				{
					alignmentPattern = this.findAlignmentInRegion(num, estAlignmentX, estAlignmentY, (float)i);
					if (alignmentPattern != null)
					{
						break;
					}
				}
			}
			PerspectiveTransform transform = Detector.createTransform(topLeft, topRight, bottomLeft, alignmentPattern, dimension);
			BitMatrix bitMatrix = Detector.sampleGrid(this.image, transform, dimension);
			if (bitMatrix == null)
			{
				return null;
			}
			ResultPoint[] points;
			if (alignmentPattern == null)
			{
				points = new ResultPoint[]
				{
					bottomLeft,
					topLeft,
					topRight
				};
			}
			else
			{
				points = new ResultPoint[]
				{
					bottomLeft,
					topLeft,
					topRight,
					alignmentPattern
				};
			}
			return new DetectorResult(bitMatrix, points);
		}

		// Token: 0x0600043D RID: 1085 RVA: 0x000246DC File Offset: 0x000228DC
		private static BitMatrix sampleGrid(BitMatrix image, PerspectiveTransform transform, int dimension)
		{
			return GridSampler.Instance.sampleGrid(image, dimension, dimension, transform);
		}

		// Token: 0x06000442 RID: 1090 RVA: 0x000248C4 File Offset: 0x00022AC4
		private float sizeOfBlackWhiteBlackRun(int fromX, int fromY, int toX, int toY)
		{
			bool flag = Math.Abs(toY - fromY) > Math.Abs(toX - fromX);
			if (flag)
			{
				int arg_1B_0 = fromX;
				fromX = fromY;
				fromY = arg_1B_0;
				int arg_22_0 = toX;
				toX = toY;
				toY = arg_22_0;
			}
			int num = Math.Abs(toX - fromX);
			int num2 = Math.Abs(toY - fromY);
			int num3 = -num >> 1;
			int num4 = (fromX < toX) ? 1 : -1;
			int num5 = (fromY < toY) ? 1 : -1;
			int num6 = 0;
			int num7 = toX + num4;
			int num8 = fromX;
			int num9 = fromY;
			while (num8 != num7)
			{
				int x = flag ? num9 : num8;
				int y = flag ? num8 : num9;
				if (num6 == 1 == this.image[x, y])
				{
					if (num6 == 2)
					{
						return MathUtils.distance(num8, num9, fromX, fromY);
					}
					num6++;
				}
				num3 += num2;
				if (num3 > 0)
				{
					if (num9 == toY)
					{
						break;
					}
					num9 += num5;
					num3 -= num;
				}
				num8 += num4;
			}
			if (num6 == 2)
			{
				return MathUtils.distance(toX + num4, toY, fromX, fromY);
			}
			return float.NaN;
		}

		// Token: 0x06000441 RID: 1089 RVA: 0x000247EC File Offset: 0x000229EC
		private float sizeOfBlackWhiteBlackRunBothWays(int fromX, int fromY, int toX, int toY)
		{
			float arg_C2_0 = this.sizeOfBlackWhiteBlackRun(fromX, fromY, toX, toY);
			float num = 1f;
			int num2 = fromX - (toX - fromX);
			if (num2 < 0)
			{
				num = (float)fromX / (float)(fromX - num2);
				num2 = 0;
			}
			else if (num2 >= this.image.Width)
			{
				num = (float)(this.image.Width - 1 - fromX) / (float)(num2 - fromX);
				num2 = this.image.Width - 1;
			}
			int num3 = (int)((float)fromY - (float)(toY - fromY) * num);
			num = 1f;
			if (num3 < 0)
			{
				num = (float)fromY / (float)(fromY - num3);
				num3 = 0;
			}
			else if (num3 >= this.image.Height)
			{
				num = (float)(this.image.Height - 1 - fromY) / (float)(num3 - fromY);
				num3 = this.image.Height - 1;
			}
			num2 = (int)((float)fromX + (float)(num2 - fromX) * num);
			return arg_C2_0 + this.sizeOfBlackWhiteBlackRun(fromX, fromY, num2, num3) - 1f;
		}

		// Token: 0x17000053 RID: 83
		protected internal virtual BitMatrix Image
		{
			// Token: 0x06000437 RID: 1079 RVA: 0x00024467 File Offset: 0x00022667
			get
			{
				return this.image;
			}
		}

		// Token: 0x17000054 RID: 84
		protected internal virtual ResultPointCallback ResultPointCallback
		{
			// Token: 0x06000438 RID: 1080 RVA: 0x0002446F File Offset: 0x0002266F
			get
			{
				return this.resultPointCallback;
			}
		}

		// Token: 0x04000308 RID: 776
		private readonly BitMatrix image;

		// Token: 0x04000309 RID: 777
		private ResultPointCallback resultPointCallback;
	}
}
