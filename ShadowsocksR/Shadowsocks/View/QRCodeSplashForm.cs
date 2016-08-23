using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Shadowsocks.View
{
	// Token: 0x02000006 RID: 6
	public class QRCodeSplashForm : PerPixelAlphaForm
	{
		// Token: 0x0600005B RID: 91 RVA: 0x000072E4 File Offset: 0x000054E4
		public QRCodeSplashForm()
		{
			base.Load += new EventHandler(this.QRCodeSplashForm_Load);
			base.AutoScaleMode = AutoScaleMode.None;
			this.BackColor = Color.White;
			base.ClientSize = new Size(1, 1);
			base.ControlBox = false;
			base.FormBorderStyle = FormBorderStyle.None;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "QRCodeSplashForm";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = SizeGripStyle.Hide;
			base.StartPosition = FormStartPosition.Manual;
			base.TopMost = true;
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00007374 File Offset: 0x00005574
		private void QRCodeSplashForm_Load(object sender, EventArgs e)
		{
			base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.BackColor = Color.Transparent;
			this.flashStep = 0;
			this.x = 0;
			this.y = 0;
			this.w = base.Width;
			this.h = base.Height;
			this.sw = Stopwatch.StartNew();
			this.timer = new Timer();
            QRCodeSplashForm.ANIMATION_STEPS = (int)(QRCodeSplashForm.ANIMATION_TIME * QRCodeSplashForm.FPS);
			this.timer.Interval = (int)(QRCodeSplashForm.ANIMATION_TIME * 1000.0 / (double)QRCodeSplashForm.ANIMATION_STEPS);
			this.timer.Tick += new EventHandler(this.timer_Tick);
			this.timer.Start();
			this.bitmap = new Bitmap(base.Width, base.Height, PixelFormat.Format32bppArgb);
			this.g = Graphics.FromImage(this.bitmap);
			this.pen = new Pen(Color.Red, 3f);
			this.brush = new SolidBrush(Color.FromArgb(30, Color.Red));
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00007494 File Offset: 0x00005694
		private void timer_Tick(object sender, EventArgs e)
		{
			double num = (double)this.sw.ElapsedMilliseconds / 1000.0 / QRCodeSplashForm.ANIMATION_TIME;
			if (num < 1.0)
			{
				num = 1.0 - Math.Pow(1.0 - num, 4.0);
				this.x = (int)((double)this.TargetRect.X * num);
				this.y = (int)((double)this.TargetRect.Y * num);
				this.w = (int)((double)this.TargetRect.Width * num + (double)base.Size.Width * (1.0 - num));
				this.h = (int)((double)this.TargetRect.Height * num + (double)base.Size.Height * (1.0 - num));
				this.pen.Color = Color.FromArgb((int)(255.0 * num), Color.Red);
				this.brush.Color = Color.FromArgb((int)(30.0 * num), Color.Red);
				this.g.Clear(Color.Transparent);
				this.g.FillRectangle(this.brush, this.x, this.y, this.w, this.h);
				this.g.DrawRectangle(this.pen, this.x, this.y, this.w, this.h);
				base.SetBitmap(this.bitmap);
				return;
			}
			if (this.flashStep == 0)
			{
				this.timer.Interval = 100;
				this.g.Clear(Color.Transparent);
				base.SetBitmap(this.bitmap);
			}
			else if (this.flashStep == 1)
			{
				this.timer.Interval = 50;
				this.g.FillRectangle(this.brush, this.x, this.y, this.w, this.h);
				this.g.DrawRectangle(this.pen, this.x, this.y, this.w, this.h);
				base.SetBitmap(this.bitmap);
			}
			else if (this.flashStep == 1)
			{
				this.g.Clear(Color.Transparent);
				base.SetBitmap(this.bitmap);
			}
			else if (this.flashStep == 2)
			{
				this.g.FillRectangle(this.brush, this.x, this.y, this.w, this.h);
				this.g.DrawRectangle(this.pen, this.x, this.y, this.w, this.h);
				base.SetBitmap(this.bitmap);
			}
			else if (this.flashStep == 3)
			{
				this.g.Clear(Color.Transparent);
				base.SetBitmap(this.bitmap);
			}
			else if (this.flashStep == 4)
			{
				this.g.FillRectangle(this.brush, this.x, this.y, this.w, this.h);
				this.g.DrawRectangle(this.pen, this.x, this.y, this.w, this.h);
				base.SetBitmap(this.bitmap);
			}
			else
			{
				this.sw.Stop();
				this.timer.Stop();
				this.pen.Dispose();
				this.brush.Dispose();
				this.bitmap.Dispose();
				base.Close();
			}
			this.flashStep++;
		}

		// Token: 0x17000001 RID: 1
		protected override CreateParams CreateParams
		{
			// Token: 0x0600005D RID: 93 RVA: 0x00007478 File Offset: 0x00005678
			get
			{
				CreateParams expr_06 = base.CreateParams;
				expr_06.ExStyle |= 524288;
				return expr_06;
			}
		}

		// Token: 0x04000057 RID: 87
		private static int ANIMATION_STEPS = (int)(QRCodeSplashForm.ANIMATION_TIME * QRCodeSplashForm.FPS);

		// Token: 0x04000056 RID: 86
		private static double ANIMATION_TIME = 0.5;

		// Token: 0x0400005D RID: 93
		private Bitmap bitmap;

		// Token: 0x04000060 RID: 96
		private SolidBrush brush;

		// Token: 0x04000054 RID: 84
		private int flashStep;

		// Token: 0x04000055 RID: 85
		private static double FPS = 66.666666666666671;

		// Token: 0x0400005E RID: 94
		private Graphics g;

		// Token: 0x0400005C RID: 92
		private int h;

		// Token: 0x0400005F RID: 95
		private Pen pen;

		// Token: 0x04000058 RID: 88
		private Stopwatch sw;

		// Token: 0x04000052 RID: 82
		public Rectangle TargetRect;

		// Token: 0x04000053 RID: 83
		private Timer timer;

		// Token: 0x0400005B RID: 91
		private int w;

		// Token: 0x04000059 RID: 89
		private int x;

		// Token: 0x0400005A RID: 90
		private int y;
	}
}
