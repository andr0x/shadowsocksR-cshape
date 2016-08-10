using System;
using System.Collections.Generic;

namespace ZXing.Common.ReedSolomon
{
	// Token: 0x0200008F RID: 143
	public sealed class ReedSolomonEncoder
	{
		// Token: 0x06000522 RID: 1314 RVA: 0x0002A715 File Offset: 0x00028915
		public ReedSolomonEncoder(GenericGF field)
		{
			this.field = field;
			this.cachedGenerators = new List<GenericGFPoly>();
			this.cachedGenerators.Add(new GenericGFPoly(field, new int[]
			{
				1
			}));
		}

		// Token: 0x06000523 RID: 1315 RVA: 0x0002A74C File Offset: 0x0002894C
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

		// Token: 0x06000524 RID: 1316 RVA: 0x0002A7EC File Offset: 0x000289EC
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

		// Token: 0x0400037A RID: 890
		private readonly IList<GenericGFPoly> cachedGenerators;

		// Token: 0x04000379 RID: 889
		private readonly GenericGF field;
	}
}
