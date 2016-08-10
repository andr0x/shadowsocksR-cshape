using System;

namespace ZXing.Common.ReedSolomon
{
	// Token: 0x0200008E RID: 142
	public sealed class ReedSolomonDecoder
	{
		// Token: 0x0600051D RID: 1309 RVA: 0x0002A367 File Offset: 0x00028567
		public ReedSolomonDecoder(GenericGF field)
		{
			this.field = field;
		}

		// Token: 0x0600051E RID: 1310 RVA: 0x0002A378 File Offset: 0x00028578
		public bool decode(int[] received, int twoS)
		{
			GenericGFPoly genericGFPoly = new GenericGFPoly(this.field, received);
			int[] array = new int[twoS];
			bool flag = true;
			for (int i = 0; i < twoS; i++)
			{
				int num = genericGFPoly.evaluateAt(this.field.exp(i + this.field.GeneratorBase));
				int[] expr_3D = array;
				expr_3D[expr_3D.Length - 1 - i] = num;
				if (num != 0)
				{
					flag = false;
				}
			}
			if (flag)
			{
				return true;
			}
			GenericGFPoly b = new GenericGFPoly(this.field, array);
			GenericGFPoly[] array2 = this.runEuclideanAlgorithm(this.field.buildMonomial(twoS, 1), b, twoS);
			if (array2 == null)
			{
				return false;
			}
			GenericGFPoly errorLocator = array2[0];
			int[] array3 = this.findErrorLocations(errorLocator);
			if (array3 == null)
			{
				return false;
			}
			GenericGFPoly errorEvaluator = array2[1];
			int[] array4 = this.findErrorMagnitudes(errorEvaluator, array3);
			for (int j = 0; j < array3.Length; j++)
			{
				int num2 = received.Length - 1 - this.field.log(array3[j]);
				if (num2 < 0)
				{
					return false;
				}
				received[num2] = GenericGF.addOrSubtract(received[num2], array4[j]);
			}
			return true;
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x0002A5DC File Offset: 0x000287DC
		private int[] findErrorLocations(GenericGFPoly errorLocator)
		{
			int degree = errorLocator.Degree;
			if (degree == 1)
			{
				return new int[]
				{
					errorLocator.getCoefficient(1)
				};
			}
			int[] array = new int[degree];
			int num = 0;
			int num2 = 1;
			while (num2 < this.field.Size && num < degree)
			{
				if (errorLocator.evaluateAt(num2) == 0)
				{
					array[num] = this.field.inverse(num2);
					num++;
				}
				num2++;
			}
			if (num != degree)
			{
				return null;
			}
			return array;
		}

		// Token: 0x06000521 RID: 1313 RVA: 0x0002A64C File Offset: 0x0002884C
		private int[] findErrorMagnitudes(GenericGFPoly errorEvaluator, int[] errorLocations)
		{
			int num = errorLocations.Length;
			int[] array = new int[num];
			for (int i = 0; i < num; i++)
			{
				int num2 = this.field.inverse(errorLocations[i]);
				int a = 1;
				for (int j = 0; j < num; j++)
				{
					if (i != j)
					{
						int num3 = this.field.multiply(errorLocations[j], num2);
						int b = ((num3 & 1) == 0) ? (num3 | 1) : (num3 & -2);
						a = this.field.multiply(a, b);
					}
				}
				array[i] = this.field.multiply(errorEvaluator.evaluateAt(num2), this.field.inverse(a));
				if (this.field.GeneratorBase != 0)
				{
					array[i] = this.field.multiply(array[i], num2);
				}
			}
			return array;
		}

		// Token: 0x0600051F RID: 1311 RVA: 0x0002A47C File Offset: 0x0002867C
		internal GenericGFPoly[] runEuclideanAlgorithm(GenericGFPoly a, GenericGFPoly b, int R)
		{
			if (a.Degree < b.Degree)
			{
				GenericGFPoly arg_12_0 = a;
				a = b;
				b = arg_12_0;
			}
			GenericGFPoly genericGFPoly = a;
			GenericGFPoly genericGFPoly2 = b;
			GenericGFPoly genericGFPoly3 = this.field.Zero;
			GenericGFPoly genericGFPoly4 = this.field.One;
			while (genericGFPoly2.Degree >= R / 2)
			{
				GenericGFPoly genericGFPoly5 = genericGFPoly;
				GenericGFPoly other = genericGFPoly3;
				genericGFPoly = genericGFPoly2;
				genericGFPoly3 = genericGFPoly4;
				if (genericGFPoly.isZero)
				{
					return null;
				}
				genericGFPoly2 = genericGFPoly5;
				GenericGFPoly genericGFPoly6 = this.field.Zero;
				GenericGFPoly expr_5A = genericGFPoly;
				int coefficient = expr_5A.getCoefficient(expr_5A.Degree);
				int b2 = this.field.inverse(coefficient);
				while (genericGFPoly2.Degree >= genericGFPoly.Degree && !genericGFPoly2.isZero)
				{
					int degree = genericGFPoly2.Degree - genericGFPoly.Degree;
					GenericGF arg_9B_0 = this.field;
					GenericGFPoly expr_8E = genericGFPoly2;
					int coefficient2 = arg_9B_0.multiply(expr_8E.getCoefficient(expr_8E.Degree), b2);
					genericGFPoly6 = genericGFPoly6.addOrSubtract(this.field.buildMonomial(degree, coefficient2));
					genericGFPoly2 = genericGFPoly2.addOrSubtract(genericGFPoly.multiplyByMonomial(degree, coefficient2));
				}
				genericGFPoly4 = genericGFPoly6.multiply(genericGFPoly3).addOrSubtract(other);
				if (genericGFPoly2.Degree >= genericGFPoly.Degree)
				{
					return null;
				}
			}
			int coefficient3 = genericGFPoly4.getCoefficient(0);
			if (coefficient3 == 0)
			{
				return null;
			}
			int scalar = this.field.inverse(coefficient3);
			GenericGFPoly genericGFPoly7 = genericGFPoly4.multiply(scalar);
			GenericGFPoly genericGFPoly8 = genericGFPoly2.multiply(scalar);
			return new GenericGFPoly[]
			{
				genericGFPoly7,
				genericGFPoly8
			};
		}

		// Token: 0x04000378 RID: 888
		private readonly GenericGF field;
	}
}
