using System;

namespace ZXing.Common
{
	// Token: 0x02000087 RID: 135
	public sealed class DefaultGridSampler : GridSampler
	{
		// Token: 0x060004E4 RID: 1252 RVA: 0x00027CB0 File Offset: 0x00025EB0
		public override BitMatrix sampleGrid(BitMatrix image, int dimensionX, int dimensionY, PerspectiveTransform transform)
		{
			if (dimensionX <= 0 || dimensionY <= 0)
			{
				return null;
			}
			BitMatrix bitMatrix = new BitMatrix(dimensionX, dimensionY);
			float[] array = new float[dimensionX << 1];
			for (int i = 0; i < dimensionY; i++)
			{
				int num = array.Length;
				float num2 = (float)i + 0.5f;
				for (int j = 0; j < num; j += 2)
				{
					array[j] = (float)(j >> 1) + 0.5f;
					array[j + 1] = num2;
				}
				transform.transformPoints(array);
				if (!GridSampler.checkAndNudgePoints(image, array))
				{
					return null;
				}
				try
				{
					for (int k = 0; k < num; k += 2)
					{
						bitMatrix[k >> 1, i] = image[(int)array[k], (int)array[k + 1]];
					}
				}
				catch (IndexOutOfRangeException)
				{
					return null;
				}
			}
			return bitMatrix;
		}

		// Token: 0x060004E3 RID: 1251 RVA: 0x00027C70 File Offset: 0x00025E70
		public override BitMatrix sampleGrid(BitMatrix image, int dimensionX, int dimensionY, float p1ToX, float p1ToY, float p2ToX, float p2ToY, float p3ToX, float p3ToY, float p4ToX, float p4ToY, float p1FromX, float p1FromY, float p2FromX, float p2FromY, float p3FromX, float p3FromY, float p4FromX, float p4FromY)
		{
			PerspectiveTransform transform = PerspectiveTransform.quadrilateralToQuadrilateral(p1ToX, p1ToY, p2ToX, p2ToY, p3ToX, p3ToY, p4ToX, p4ToY, p1FromX, p1FromY, p2FromX, p2FromY, p3FromX, p3FromY, p4FromX, p4FromY);
			return this.sampleGrid(image, dimensionX, dimensionY, transform);
		}
	}
}
