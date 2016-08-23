using System;
using System.Text;

namespace ZXing.Common.ReedSolomon
{
	// Token: 0x0200008F RID: 143
	internal sealed class GenericGFPoly
	{
		// Token: 0x0600051A RID: 1306 RVA: 0x00029078 File Offset: 0x00027278
		internal GenericGFPoly(GenericGF field, int[] coefficients)
		{
			if (coefficients.Length == 0)
			{
				throw new ArgumentException();
			}
			this.field = field;
			int num = coefficients.Length;
			if (num <= 1 || coefficients[0] != 0)
			{
				this.coefficients = coefficients;
				return;
			}
			int num2 = 1;
			while (num2 < num && coefficients[num2] == 0)
			{
				num2++;
			}
			if (num2 == num)
			{
				this.coefficients = field.Zero.coefficients;
				return;
			}
			this.coefficients = new int[num - num2];
			Array.Copy(coefficients, num2, this.coefficients, 0, this.coefficients.Length);
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x000291B8 File Offset: 0x000273B8
		internal GenericGFPoly addOrSubtract(GenericGFPoly other)
		{
			if (!this.field.Equals(other.field))
			{
				throw new ArgumentException("GenericGFPolys do not have same GenericGF field");
			}
			if (this.isZero)
			{
				return other;
			}
			if (other.isZero)
			{
				return this;
			}
			int[] array = this.coefficients;
			int[] array2 = other.coefficients;
			if (array.Length > array2.Length)
			{
				int[] arg_4B_0 = array;
				array = array2;
				array2 = arg_4B_0;
			}
			int[] array3 = new int[array2.Length];
			int num = array2.Length - array.Length;
			Array.Copy(array2, 0, array3, 0, num);
			for (int i = num; i < array2.Length; i++)
			{
				array3[i] = GenericGF.addOrSubtract(array[i - num], array2[i]);
			}
			return new GenericGFPoly(this.field, array3);
		}

		// Token: 0x06000524 RID: 1316 RVA: 0x000293F0 File Offset: 0x000275F0
		internal GenericGFPoly[] divide(GenericGFPoly other)
		{
			if (!this.field.Equals(other.field))
			{
				throw new ArgumentException("GenericGFPolys do not have same GenericGF field");
			}
			if (other.isZero)
			{
				throw new ArgumentException("Divide by 0");
			}
			GenericGFPoly genericGFPoly = this.field.Zero;
			GenericGFPoly genericGFPoly2 = this;
			int coefficient = other.getCoefficient(other.Degree);
			int b = this.field.inverse(coefficient);
			while (genericGFPoly2.Degree >= other.Degree && !genericGFPoly2.isZero)
			{
				int degree = genericGFPoly2.Degree - other.Degree;
				GenericGF arg_7D_0 = this.field;
				GenericGFPoly expr_71 = genericGFPoly2;
				int coefficient2 = arg_7D_0.multiply(expr_71.getCoefficient(expr_71.Degree), b);
				GenericGFPoly other2 = other.multiplyByMonomial(degree, coefficient2);
				GenericGFPoly other3 = this.field.buildMonomial(degree, coefficient2);
				genericGFPoly = genericGFPoly.addOrSubtract(other3);
				genericGFPoly2 = genericGFPoly2.addOrSubtract(other2);
			}
			return new GenericGFPoly[]
			{
				genericGFPoly,
				genericGFPoly2
			};
		}

		// Token: 0x0600051F RID: 1311 RVA: 0x00029134 File Offset: 0x00027334
		internal int evaluateAt(int a)
		{
			int num = 0;
			if (a == 0)
			{
				return this.getCoefficient(0);
			}
			int num2 = this.coefficients.Length;
			if (a == 1)
			{
				int[] array = this.coefficients;
				for (int i = 0; i < array.Length; i++)
				{
					int b = array[i];
					num = GenericGF.addOrSubtract(num, b);
				}
				return num;
			}
			num = this.coefficients[0];
			for (int j = 1; j < num2; j++)
			{
				num = GenericGF.addOrSubtract(this.field.multiply(a, num), this.coefficients[j]);
			}
			return num;
		}

		// Token: 0x0600051E RID: 1310 RVA: 0x0002911D File Offset: 0x0002731D
		internal int getCoefficient(int degree)
		{
			return this.coefficients[this.coefficients.Length - 1 - degree];
		}

		// Token: 0x06000521 RID: 1313 RVA: 0x00029260 File Offset: 0x00027460
		internal GenericGFPoly multiply(GenericGFPoly other)
		{
			if (!this.field.Equals(other.field))
			{
				throw new ArgumentException("GenericGFPolys do not have same GenericGF field");
			}
			if (this.isZero || other.isZero)
			{
				return this.field.Zero;
			}
			int[] array = this.coefficients;
			int num = array.Length;
			int[] array2 = other.coefficients;
			int num2 = array2.Length;
			int[] array3 = new int[num + num2 - 1];
			for (int i = 0; i < num; i++)
			{
				int a = array[i];
				for (int j = 0; j < num2; j++)
				{
					array3[i + j] = GenericGF.addOrSubtract(array3[i + j], this.field.multiply(a, array2[j]));
				}
			}
			return new GenericGFPoly(this.field, array3);
		}

		// Token: 0x06000522 RID: 1314 RVA: 0x00029324 File Offset: 0x00027524
		internal GenericGFPoly multiply(int scalar)
		{
			if (scalar == 0)
			{
				return this.field.Zero;
			}
			if (scalar == 1)
			{
				return this;
			}
			int num = this.coefficients.Length;
			int[] array = new int[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = this.field.multiply(this.coefficients[i], scalar);
			}
			return new GenericGFPoly(this.field, array);
		}

		// Token: 0x06000523 RID: 1315 RVA: 0x00029388 File Offset: 0x00027588
		internal GenericGFPoly multiplyByMonomial(int degree, int coefficient)
		{
			if (degree < 0)
			{
				throw new ArgumentException();
			}
			if (coefficient == 0)
			{
				return this.field.Zero;
			}
			int num = this.coefficients.Length;
			int[] array = new int[num + degree];
			for (int i = 0; i < num; i++)
			{
				array[i] = this.field.multiply(this.coefficients[i], coefficient);
			}
			return new GenericGFPoly(this.field, array);
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x000294D4 File Offset: 0x000276D4
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(8 * this.Degree);
			for (int i = this.Degree; i >= 0; i--)
			{
				int num = this.getCoefficient(i);
				if (num != 0)
				{
					if (num < 0)
					{
						stringBuilder.Append(" - ");
						num = -num;
					}
					else if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(" + ");
					}
					if (i == 0 || num != 1)
					{
						int num2 = this.field.log(num);
						if (num2 == 0)
						{
							stringBuilder.Append('1');
						}
						else if (num2 == 1)
						{
							stringBuilder.Append('a');
						}
						else
						{
							stringBuilder.Append("a^");
							stringBuilder.Append(num2);
						}
					}
					if (i != 0)
					{
						if (i == 1)
						{
							stringBuilder.Append('x');
						}
						else
						{
							stringBuilder.Append("x^");
							stringBuilder.Append(i);
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x17000084 RID: 132
		internal int[] Coefficients
		{
			// Token: 0x0600051B RID: 1307 RVA: 0x000290FC File Offset: 0x000272FC
			get
			{
				return this.coefficients;
			}
		}

		// Token: 0x17000085 RID: 133
		internal int Degree
		{
			// Token: 0x0600051C RID: 1308 RVA: 0x00029104 File Offset: 0x00027304
			get
			{
				return this.coefficients.Length - 1;
			}
		}

		// Token: 0x17000086 RID: 134
		internal bool isZero
		{
			// Token: 0x0600051D RID: 1309 RVA: 0x00029110 File Offset: 0x00027310
			get
			{
				return this.coefficients[0] == 0;
			}
		}

		// Token: 0x04000374 RID: 884
		private readonly int[] coefficients;

		// Token: 0x04000373 RID: 883
		private readonly GenericGF field;
	}
}
