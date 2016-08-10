using System;
using System.Text;

namespace ZXing.Common.ReedSolomon
{
	// Token: 0x0200008D RID: 141
	internal sealed class GenericGFPoly
	{
		// Token: 0x06000511 RID: 1297 RVA: 0x00029E30 File Offset: 0x00028030
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

		// Token: 0x06000517 RID: 1303 RVA: 0x00029F70 File Offset: 0x00028170
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

		// Token: 0x0600051B RID: 1307 RVA: 0x0002A1A8 File Offset: 0x000283A8
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

		// Token: 0x06000516 RID: 1302 RVA: 0x00029EEC File Offset: 0x000280EC
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

		// Token: 0x06000515 RID: 1301 RVA: 0x00029ED5 File Offset: 0x000280D5
		internal int getCoefficient(int degree)
		{
			return this.coefficients[this.coefficients.Length - 1 - degree];
		}

		// Token: 0x06000518 RID: 1304 RVA: 0x0002A018 File Offset: 0x00028218
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

		// Token: 0x06000519 RID: 1305 RVA: 0x0002A0DC File Offset: 0x000282DC
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

		// Token: 0x0600051A RID: 1306 RVA: 0x0002A140 File Offset: 0x00028340
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

		// Token: 0x0600051C RID: 1308 RVA: 0x0002A28C File Offset: 0x0002848C
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

		// Token: 0x17000081 RID: 129
		internal int[] Coefficients
		{
			// Token: 0x06000512 RID: 1298 RVA: 0x00029EB4 File Offset: 0x000280B4
			get
			{
				return this.coefficients;
			}
		}

		// Token: 0x17000082 RID: 130
		internal int Degree
		{
			// Token: 0x06000513 RID: 1299 RVA: 0x00029EBC File Offset: 0x000280BC
			get
			{
				return this.coefficients.Length - 1;
			}
		}

		// Token: 0x17000083 RID: 131
		internal bool isZero
		{
			// Token: 0x06000514 RID: 1300 RVA: 0x00029EC8 File Offset: 0x000280C8
			get
			{
				return this.coefficients[0] == 0;
			}
		}

		// Token: 0x04000377 RID: 887
		private readonly int[] coefficients;

		// Token: 0x04000376 RID: 886
		private readonly GenericGF field;
	}
}
