namespace Shadowsocks.View
{
	// Token: 0x02000005 RID: 5
	public partial class QRCodeForm : global::System.Windows.Forms.Form
	{
		// Token: 0x06000059 RID: 89 RVA: 0x0000717A File Offset: 0x0000537A
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x0000719C File Offset: 0x0000539C
		private void InitializeComponent()
		{
			this.pictureBox1 = new global::System.Windows.Forms.PictureBox();
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.pictureBox1.Location = new global::System.Drawing.Point(10, 10);
			this.pictureBox1.Margin = new global::System.Windows.Forms.Padding(0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new global::System.Drawing.Size(210, 210);
			this.pictureBox1.SizeMode = global::System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.None;
			this.AutoSize = true;
			base.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = global::System.Drawing.Color.White;
			base.ClientSize = new global::System.Drawing.Size(338, 274);
			base.Controls.Add(this.pictureBox1);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "QRCodeForm";
			base.Padding = new global::System.Windows.Forms.Padding(10);
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "QRCode";
			base.Load += new global::System.EventHandler(this.QRCodeForm_Load);
			((global::System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}

		// Token: 0x04000050 RID: 80
		private global::System.ComponentModel.IContainer components;

		// Token: 0x04000051 RID: 81
		private global::System.Windows.Forms.PictureBox pictureBox1;
	}
}
