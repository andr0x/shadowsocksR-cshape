using System;

namespace ZXing.Common
{
	// Token: 0x0200008C RID: 140
	public sealed class PerspectiveTransform
	{
		// Token: 0x06000501 RID: 1281 RVA: 0x0002859C File Offset: 0x0002679C
		private PerspectiveTransform(float a11, float a21, float a31, float a12, float a22, float a32, float a13, float a23, float a33)
		{
			this.a11 = a11;
			this.a12 = a12;
			this.a13 = a13;
			this.a21 = a21;
			this.a22 = a22;
			this.a23 = a23;
			this.a31 = a31;
			this.a32 = a32;
			this.a33 = a33;
		}

		// Token: 0x06000507 RID: 1287 RVA: 0x00028838 File Offset: 0x00026A38
		internal PerspectiveTransform buildAdjoint()
		{
			return new PerspectiveTransform(this.a22 * this.a33 - this.a23 * this.a32, this.a23 * this.a31 - this.a21 * this.a33, this.a21 * this.a32 - this.a22 * this.a31, this.a13 * this.a32 - this.a12 * this.a33, this.a11 * this.a33 - this.a13 * this.a31, this.a12 * this.a31 - this.a11 * this.a32, this.a12 * this.a23 - this.a13 * this.a22, this.a13 * this.a21 - this.a11 * this.a23, this.a11 * this.a22 - this.a12 * this.a21);
		}

		// Token: 0x06000502 RID: 1282 RVA: 0x000285F4 File Offset: 0x000267F4
		public static PerspectiveTransform quadrilateralToQuadrilateral(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3, float x0p, float y0p, float x1p, float y1p, float x2p, float y2p, float x3p, float y3p)
		{
			PerspectiveTransform other = PerspectiveTransform.quadrilateralToSquare(x0, y0, x1, y1, x2, y2, x3, y3);
			return PerspectiveTransform.squareToQuadrilateral(x0p, y0p, x1p, y1p, x2p, y2p, x3p, y3p).times(other);
		}

		// Token: 0x06000506 RID: 1286 RVA: 0x0002881E File Offset: 0x00026A1E
		public static PerspectiveTransform quadrilateralToSquare(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3)
		{
			return PerspectiveTransform.squareToQuadrilateral(x0, y0, x1, y1, x2, y2, x3, y3).buildAdjoint();
		}

		// Token: 0x06000505 RID: 1285 RVA: 0x0002875C File Offset: 0x0002695C
		public static PerspectiveTransform squareToQuadrilateral(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3)
		{
			float num = x0 - x1 + x2 - x3;
			float num2 = y0 - y1 + y2 - y3;
			if (num == 0f && num2 == 0f)
			{
				return new PerspectiveTransform(x1 - x0, x2 - x1, x0, y1 - y0, y2 - y1, y0, 0f, 0f, 1f);
			}
			float arg_5F_0 = x1 - x2;
			float num3 = x3 - x2;
			float num4 = y1 - y2;
			float num5 = y3 - y2;
			float num6 = arg_5F_0 * num5 - num3 * num4;
			float num7 = (num * num5 - num3 * num2) / num6;
			float num8 = (arg_5F_0 * num2 - num * num4) / num6;
			return new PerspectiveTransform(x1 - x0 + num7 * x1, x3 - x0 + num8 * x3, x0, y1 - y0 + num7 * y1, y3 - y0 + num8 * y3, y0, num7, num8, 1f);
		}

		// Token: 0x06000508 RID: 1288 RVA: 0x00028940 File Offset: 0x00026B40
		internal PerspectiveTransform times(PerspectiveTransform other)
		{
			return new PerspectiveTransform(this.a11 * other.a11 + this.a21 * other.a12 + this.a31 * other.a13, this.a11 * other.a21 + this.a21 * other.a22 + this.a31 * other.a23, this.a11 * other.a31 + this.a21 * other.a32 + this.a31 * other.a33, this.a12 * other.a11 + this.a22 * other.a12 + this.a32 * other.a13, this.a12 * other.a21 + this.a22 * other.a22 + this.a32 * other.a23, this.a12 * other.a31 + this.a22 * other.a32 + this.a32 * other.a33, this.a13 * other.a11 + this.a23 * other.a12 + this.a33 * other.a13, this.a13 * other.a21 + this.a23 * other.a22 + this.a33 * other.a23, this.a13 * other.a31 + this.a23 * other.a32 + this.a33 * other.a33);
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x00028630 File Offset: 0x00026830
		public void transformPoints(float[] points)
		{
			int num = points.Length;
			float num2 = this.a11;
			float num3 = this.a12;
			float num4 = this.a13;
			float num5 = this.a21;
			float num6 = this.a22;
			float num7 = this.a23;
			float num8 = this.a31;
			float num9 = this.a32;
			float num10 = this.a33;
			for (int i = 0; i < num; i += 2)
			{
				float num11 = points[i];
				float num12 = points[i + 1];
				float num13 = num4 * num11 + num7 * num12 + num10;
				points[i] = (num2 * num11 + num5 * num12 + num8) / num13;
				points[i + 1] = (num3 * num11 + num6 * num12 + num9) / num13;
			}
		}

		// Token: 0x06000504 RID: 1284 RVA: 0x000286E0 File Offset: 0x000268E0
		public void transformPoints(float[] xValues, float[] yValues)
		{
			int num = xValues.Length;
			for (int i = 0; i < num; i++)
			{
				float num2 = xValues[i];
				float num3 = yValues[i];
				float num4 = this.a13 * num2 + this.a23 * num3 + this.a33;
				xValues[i] = (this.a11 * num2 + this.a21 * num3 + this.a31) / num4;
				yValues[i] = (this.a12 * num2 + this.a22 * num3 + this.a32) / num4;
			}
		}

		// Token: 0x04000352 RID: 850
		private float a11;

		// Token: 0x04000353 RID: 851
		private float a12;

		// Token: 0x04000354 RID: 852
		private float a13;

		// Token: 0x04000355 RID: 853
		private float a21;

		// Token: 0x04000356 RID: 854
		private float a22;

		// Token: 0x04000357 RID: 855
		private float a23;

		// Token: 0x04000358 RID: 856
		private float a31;

		// Token: 0x04000359 RID: 857
		private float a32;

		// Token: 0x0400035A RID: 858
		private float a33;
	}
}
