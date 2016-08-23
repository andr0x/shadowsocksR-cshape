using System;
using System.Collections.Generic;

namespace ZXing.Common.ReedSolomon
{
	// Token: 0x02000091 RID: 145
	public sealed class ReedSolomonEncoder
	{
		// Token: 0x0600052B RID: 1323 RVA: 0x0002995D File Offset: 0x00027B5D
		public ReedSolomonEncoder(GenericGF field)
		{
			this.field = field;
			this.cachedGenerators = new List<GenericGFPoly>();
			this.cachedGenerators.Add(new GenericGFPoly(field, new int[]
			{
				1
			}));
		}

		// Token: 0x0600052C RID: 1324 RVA: 0x00029994 File Offset: 0x00027B94
		private GenericGFPoly buildGenerator(int degree)
		{
			if (degree >= this.cachedGenerators.Count)
			{
				GenericGFPoly genericGFPoly = this.cachedGenerators[this.cachedGenerators.Count - 1];
				for (int i = this.cachedGenerators.Count; i <= degree; i++)
				{
					GenericGFPoly genericGFPoly2 = genericGFPoly.multiply(new GenericGFPoly(this.field, new int[]
					{
						1,
						this.field.exp(i - 1 + this.field.GeneratorBase)
					}));
					this.cachedGenerators.Add(genericGFPoly2);
					genericGFPoly = genericGFPoly2;
				}
			}
			return this.cachedGenerators[degree];
		}

		// Token: 0x0600052D RID: 1325 RVA: 0x00029A34 File Offset: 0x00027C34
		public void encode(int[] toEncode, int ecBytes)
		{
			if (ecBytes == 0)
			{
				throw new ArgumentException("No error correction bytes");
			}
			int num = toEncode.Length - ecBytes;
			if (num <= 0)
			{
				throw new ArgumentException("No data bytes provided");
			}
			GenericGFPoly other = this.buildGenerator(ecBytes);
			int[] array = new int[num];
			Array.Copy(toEncode, 0, array, 0, num);
			int[] coefficients = new GenericGFPoly(this.field, array).multiplyByMonomial(ecBytes, 1).divide(other)[1].Coefficients;
			int num2 = ecBytes - coefficients.Length;
			for (int i = 0; i < num2; i++)
			{
				toEncode[num + i] = 0;
			}
			Array.Copy(coefficients, 0, toEncode, num + num2, coefficients.Length);
		}

		// Token: 0x04000377 RID: 887
		private readonly IList<GenericGFPoly> cachedGenerators;

		// Token: 0x04000376 RID: 886
		private readonly GenericGF field;
	}
}
