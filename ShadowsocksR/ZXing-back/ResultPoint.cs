using System;
using System.Globalization;
using System.Text;
using ZXing.Common.Detector;

namespace ZXing
{
	// Token: 0x02000067 RID: 103
	public class ResultPoint
	{
		// Token: 0x060003C4 RID: 964 RVA: 0x00007729 File Offset: 0x00005929
		public ResultPoint()
		{
		}

		// Token: 0x060003C5 RID: 965 RVA: 0x000211ED File Offset: 0x0001F3ED
		public ResultPoint(float x, float y)
		{
			this.x = x;
			this.y = y;
			this.bytesX = BitConverter.GetBytes(x);
			this.bytesY = BitConverter.GetBytes(y);
		}

		// Token: 0x060003CD RID: 973 RVA: 0x000213E4 File Offset: 0x0001F5E4
		private static float crossProductZ(ResultPoint pointA, ResultPoint pointB, ResultPoint pointC)
		{
			float num = pointB.x;
			float num2 = pointB.y;
			return (pointC.x - num) * (pointA.y - num2) - (pointC.y - num2) * (pointA.x - num);
		}

		// Token: 0x060003CC RID: 972 RVA: 0x000213C4 File Offset: 0x0001F5C4
		public static float distance(ResultPoint pattern1, ResultPoint pattern2)
		{
			return MathUtils.distance(pattern1.x, pattern1.y, pattern2.x, pattern2.y);
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x0002122C File Offset: 0x0001F42C
		public override bool Equals(object other)
		{
			ResultPoint resultPoint = other as ResultPoint;
			return resultPoint != null && this.x == resultPoint.x && this.y == resultPoint.y;
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x00021264 File Offset: 0x0001F464
		public override int GetHashCode()
		{
			return 31 * (((int)this.bytesX[0] << 24) + ((int)this.bytesX[1] << 16) + ((int)this.bytesX[2] << 8) + (int)this.bytesX[3]) + ((int)this.bytesY[0] << 24) + ((int)this.bytesY[1] << 16) + ((int)this.bytesY[2] << 8) + (int)this.bytesY[3];
		}

		// Token: 0x060003CB RID: 971 RVA: 0x00021330 File Offset: 0x0001F530
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

		// Token: 0x060003CA RID: 970 RVA: 0x000212CC File Offset: 0x0001F4CC
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

		// Token: 0x17000041 RID: 65
		public virtual float X
		{
			// Token: 0x060003C6 RID: 966 RVA: 0x0002121B File Offset: 0x0001F41B
			get
			{
				return this.x;
			}
		}

		// Token: 0x17000042 RID: 66
		public virtual float Y
		{
			// Token: 0x060003C7 RID: 967 RVA: 0x00021223 File Offset: 0x0001F423
			get
			{
				return this.y;
			}
		}

		// Token: 0x040002D1 RID: 721
		private readonly byte[] bytesX;

		// Token: 0x040002D2 RID: 722
		private readonly byte[] bytesY;

		// Token: 0x040002D3 RID: 723
		private string toString;

		// Token: 0x040002CF RID: 719
		private readonly float x;

		// Token: 0x040002D0 RID: 720
		private readonly float y;
	}
}
