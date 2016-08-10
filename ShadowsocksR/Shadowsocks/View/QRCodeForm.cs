using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Shadowsocks.Controller;
using Shadowsocks.Properties;
using ZXing.QrCode.Internal;

namespace Shadowsocks.View
{
	// Token: 0x02000005 RID: 5
	public partial class QRCodeForm : Form
	{
		// Token: 0x06000056 RID: 86 RVA: 0x00006EE3 File Offset: 0x000050E3
		public QRCodeForm(string code)
		{
			this.code = code;
			this.InitializeComponent();
			base.Icon = Icon.FromHandle(Resources.ssw128.GetHicon());
			this.Text = I18N.GetString("QRCode");
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00006F20 File Offset: 0x00005120
		private void GenQR(string ssconfig)
		{
			ByteMatrix matrix = Encoder.encode(ssconfig,ErrorCorrectionLevel.M).Matrix;
			int num = Math.Max(this.pictureBox1.Height / matrix.Height, 1);
			Bitmap image = new Bitmap(matrix.Width * num, matrix.Height * num);
			using (Graphics graphics = Graphics.FromImage(image))
			{
				graphics.Clear(Color.White);
				using (Brush brush = new SolidBrush(Color.Black))
				{
					for (int i = 0; i < matrix.Width; i++)
					{
						for (int j = 0; j < matrix.Height; j++)
						{
							if (matrix[i, j] != 0)
							{
								Graphics arg_81_0 = graphics;
								Brush arg_81_1 = brush;
								int arg_81_2 = num * i;
								int arg_81_3 = num * j;
								int expr_80 = num;
								arg_81_0.FillRectangle(arg_81_1, arg_81_2, arg_81_3, expr_80, expr_80);
							}
						}
					}
				}
			}
			this.pictureBox1.Image = image;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00007014 File Offset: 0x00005214
		private void QRCodeForm_Load(object sender, EventArgs e)
		{
			this.GenQR(this.code);
		}

		// Token: 0x0400004F RID: 79
		private string code;
	}
}
