using System;
using System.Globalization;
using System.Text;
using ZXing.Common.Detector;

namespace ZXing
{
	// Token: 0x02000069 RID: 105
	public class ResultPoint
	{
		// Token: 0x060003CD RID: 973 RVA: 0x00007881 File Offset: 0x00005A81
		public ResultPoint()
		{
		}

		// Token: 0x060003CE RID: 974 RVA: 0x00020435 File Offset: 0x0001E635
		public ResultPoint(float x, float y)
		{
			this.x = x;
			this.y = y;
			this.bytesX = BitConverter.GetBytes(x);
			this.bytesY = BitConverter.GetBytes(y);
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x0002062C File Offset: 0x0001E82C
		private static float crossProductZ(ResultPoint pointA, ResultPoint pointB, ResultPoint pointC)
		{
			float num = pointB.x;
			float num2 = pointB.y;
			return (pointC.x - num) * (pointA.y - num2) - (pointC.y - num2) * (pointA.x - num);
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x0002060C File Offset: 0x0001E80C
		public static float distance(ResultPoint pattern1, ResultPoint pattern2)
		{
			return MathUtils.distance(pattern1.x, pattern1.y, pattern2.x, pattern2.y);
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x00020474 File Offset: 0x0001E674
		public override bool Equals(object other)
		{
			ResultPoint resultPoint = other as ResultPoint;
			return resultPoint != null && this.x == resultPoint.x && this.y == resultPoint.y;
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x000204AC File Offset: 0x0001E6AC
		public override int GetHashCode()
		{
			return 31 * (((int)this.bytesX[0] << 24) + ((int)this.bytesX[1] << 16) + ((int)this.bytesX[2] << 8) + (int)this.bytesX[3]) + ((int)this.bytesY[0] << 24) + ((int)this.bytesY[1] << 16) + ((int)this.bytesY[2] << 8) + (int)this.bytesY[3];
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x00020578 File Offset: 0x0001E778
		public static void orderBestPatterns(ResultPoint[] patterns)
		{
			float num = ResultPoint.distance(patterns[0], patterns[1]);
			float num2 = ResultPoint.distance(patterns[1], patterns[2]);
			float num3 = ResultPoint.distance(patterns[0], patterns[2]);
			ResultPoint resultPoint;
			ResultPoint resultPoint2;
			ResultPoint resultPoint3;
			if (num2 >= num && num2 >= num3)
			{
				resultPoint = patterns[0];
				resultPoint2 = patterns[1];
				resultPoint3 = patterns[2];
			}
			else if (num3 >= num2 && num3 >= num)
			{
				resultPoint = patterns[1];
				resultPoint2 = patterns[0];
				resultPoint3 = patterns[2];
			}
			else
			{
				resultPoint = patterns[2];
				resultPoint2 = patterns[0];
				resultPoint3 = patterns[1];
			}
			if (ResultPoint.crossProductZ(resultPoint2, resultPoint, resultPoint3) < 0f)
			{
				ResultPoint arg_77_0 = resultPoint2;
				resultPoint2 = resultPoint3;
				resultPoint3 = arg_77_0;
			}
			patterns[0] = resultPoint2;
			patterns[1] = resultPoint;
			patterns[2] = resultPoint3;
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x00020514 File Offset: 0x0001E714
		public override string ToString()
		{
			if (this.toString == null)
			{
				StringBuilder stringBuilder = new StringBuilder(25);
				stringBuilder.AppendFormat(CultureInfo.CurrentUICulture, "({0}, {1})", new object[]
				{
					this.x,
					this.y
				});
				this.toString = stringBuilder.ToString();
			}
			return this.toString;
		}

		// Token: 0x17000044 RID: 68
		public virtual float X
		{
			// Token: 0x060003CF RID: 975 RVA: 0x00020463 File Offset: 0x0001E663
			get
			{
				return this.x;
			}
		}

		// Token: 0x17000045 RID: 69
		public virtual float Y
		{
			// Token: 0x060003D0 RID: 976 RVA: 0x0002046B File Offset: 0x0001E66B
			get
			{
				return this.y;
			}
		}

		// Token: 0x040002CE RID: 718
		private readonly byte[] bytesX;

		// Token: 0x040002CF RID: 719
		private readonly byte[] bytesY;

		// Token: 0x040002D0 RID: 720
		private string toString;

		// Token: 0x040002CC RID: 716
		private readonly float x;

		// Token: 0x040002CD RID: 717
		private readonly float y;
	}
}
