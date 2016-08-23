namespace Shadowsocks.View
{
	// Token: 0x02000009 RID: 9
	public partial class ServerLogForm : global::System.Windows.Forms.Form
	{
		// Token: 0x0600008C RID: 140 RVA: 0x00009CA1 File Offset: 0x00007EA1
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00009CC0 File Offset: 0x00007EC0
		private void InitializeComponent()
		{
			this.components = new global::System.ComponentModel.Container();
			this.ServerDataGrid = new global::System.Windows.Forms.DataGridView();
			this.timer = new global::System.Windows.Forms.Timer(this.components);
			this.tableLayoutPanel1 = new global::System.Windows.Forms.TableLayoutPanel();
			this.ID = new global::System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Group = new global::System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Server = new global::System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Enable = new global::System.Windows.Forms.DataGridViewTextBoxColumn();
			this.TotalConnect = new global::System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Connecting = new global::System.Windows.Forms.DataGridViewTextBoxColumn();
			this.AvgLatency = new global::System.Windows.Forms.DataGridViewTextBoxColumn();
			this.AvgDownSpeed = new global::System.Windows.Forms.DataGridViewTextBoxColumn();
			this.MaxDownSpeed = new global::System.Windows.Forms.DataGridViewTextBoxColumn();
			this.AvgUpSpeed = new global::System.Windows.Forms.DataGridViewTextBoxColumn();
			this.MaxUpSpeed = new global::System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Download = new global::System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Upload = new global::System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DownloadRaw = new global::System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ErrorPercent = new global::System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ConnectError = new global::System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ConnectTimeout = new global::System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ConnectEmpty = new global::System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Continuous = new global::System.Windows.Forms.DataGridViewTextBoxColumn();
			((global::System.ComponentModel.ISupportInitialize)this.ServerDataGrid).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			base.SuspendLayout();
			this.ServerDataGrid.AllowUserToAddRows = false;
			this.ServerDataGrid.AllowUserToDeleteRows = false;
			this.ServerDataGrid.AllowUserToResizeRows = false;
			this.ServerDataGrid.BorderStyle = global::System.Windows.Forms.BorderStyle.None;
			this.ServerDataGrid.ColumnHeadersHeight = 46;
			this.ServerDataGrid.Columns.AddRange(new global::System.Windows.Forms.DataGridViewColumn[]
			{
				this.ID,
				this.Group,
				this.Server,
				this.Enable,
				this.TotalConnect,
				this.Connecting,
				this.AvgLatency,
				this.AvgDownSpeed,
				this.MaxDownSpeed,
				this.AvgUpSpeed,
				this.MaxUpSpeed,
				this.Download,
				this.Upload,
				this.DownloadRaw,
				this.ErrorPercent,
				this.ConnectError,
				this.ConnectTimeout,
				this.ConnectEmpty,
				this.Continuous
			});
			this.ServerDataGrid.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.ServerDataGrid.Location = new global::System.Drawing.Point(0, 0);
			this.ServerDataGrid.Margin = new global::System.Windows.Forms.Padding(0);
			this.ServerDataGrid.MinimumSize = new global::System.Drawing.Size(1, 1);
			this.ServerDataGrid.MultiSelect = false;
			this.ServerDataGrid.Name = "ServerDataGrid";
			this.ServerDataGrid.ReadOnly = true;
			this.ServerDataGrid.RowHeadersVisible = false;
			this.ServerDataGrid.RowTemplate.Height = 23;
			this.ServerDataGrid.Size = new global::System.Drawing.Size(128, 48);
			this.ServerDataGrid.TabIndex = 0;
			this.ServerDataGrid.CellClick += new global::System.Windows.Forms.DataGridViewCellEventHandler(this.ServerDataGrid_CellClick);
			this.ServerDataGrid.CellDoubleClick += new global::System.Windows.Forms.DataGridViewCellEventHandler(this.ServerDataGrid_CellDoubleClick);
			this.ServerDataGrid.ColumnWidthChanged += new global::System.Windows.Forms.DataGridViewColumnEventHandler(this.ServerDataGrid_ColumnWidthChanged);
			this.ServerDataGrid.SortCompare += new global::System.Windows.Forms.DataGridViewSortCompareEventHandler(this.ServerDataGrid_SortCompare);
			this.ServerDataGrid.MouseUp += new global::System.Windows.Forms.MouseEventHandler(this.ServerDataGrid_MouseUp);
			this.timer.Enabled = true;
			this.timer.Interval = 250;
			this.timer.Tick += new global::System.EventHandler(this.timer_Tick);
			this.tableLayoutPanel1.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.ServerDataGrid, 0, 0);
			this.tableLayoutPanel1.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.GrowStyle = global::System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
			this.tableLayoutPanel1.Location = new global::System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new global::System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new global::System.Drawing.Size(128, 48);
			this.tableLayoutPanel1.TabIndex = 1;
			this.ID.HeaderText = "ID";
			this.ID.MinimumWidth = 2;
			this.ID.Name = "ID";
			this.ID.ReadOnly = true;
			this.ID.Width = 28;
			this.Group.HeaderText = "Group";
			this.Group.Name = "Group";
			this.Group.ReadOnly = true;
			this.Group.Width = 48;
			this.Server.HeaderText = "Server";
			this.Server.MinimumWidth = 2;
			this.Server.Name = "Server";
			this.Server.ReadOnly = true;
			this.Enable.HeaderText = "Enable";
			this.Enable.MinimumWidth = 8;
			this.Enable.Name = "Enable";
			this.Enable.ReadOnly = true;
			this.Enable.Resizable = global::System.Windows.Forms.DataGridViewTriState.True;
			this.Enable.SortMode = global::System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.Enable.Width = 24;
			this.TotalConnect.HeaderText = "Total Connect";
			this.TotalConnect.MinimumWidth = 2;
			this.TotalConnect.Name = "TotalConnect";
			this.TotalConnect.ReadOnly = true;
			this.TotalConnect.Visible = false;
			this.TotalConnect.Width = 48;
			this.Connecting.HeaderText = "Connecting";
			this.Connecting.MinimumWidth = 16;
			this.Connecting.Name = "Connecting";
			this.Connecting.ReadOnly = true;
			this.Connecting.Width = 28;
			this.AvgLatency.HeaderText = "Latency";
			this.AvgLatency.MinimumWidth = 36;
			this.AvgLatency.Name = "AvgLatency";
			this.AvgLatency.ReadOnly = true;
			this.AvgLatency.Width = 48;
			this.AvgDownSpeed.HeaderText = "Avg DSpeed";
			this.AvgDownSpeed.MinimumWidth = 60;
			this.AvgDownSpeed.Name = "AvgDownSpeed";
			this.AvgDownSpeed.ReadOnly = true;
			this.AvgDownSpeed.Width = 60;
			this.MaxDownSpeed.HeaderText = "Max DSpeed";
			this.MaxDownSpeed.MinimumWidth = 2;
			this.MaxDownSpeed.Name = "MaxDownSpeed";
			this.MaxDownSpeed.ReadOnly = true;
			this.MaxDownSpeed.Width = 60;
			this.AvgUpSpeed.HeaderText = "Avg UpSpeed";
			this.AvgUpSpeed.MinimumWidth = 60;
			this.AvgUpSpeed.Name = "AvgUpSpeed";
			this.AvgUpSpeed.ReadOnly = true;
			this.AvgUpSpeed.Width = 60;
			this.MaxUpSpeed.HeaderText = "Max UpSpeed";
			this.MaxUpSpeed.MinimumWidth = 2;
			this.MaxUpSpeed.Name = "MaxUpSpeed";
			this.MaxUpSpeed.ReadOnly = true;
			this.MaxUpSpeed.Width = 60;
			this.Download.HeaderText = "Dload";
			this.Download.MinimumWidth = 2;
			this.Download.Name = "Download";
			this.Download.ReadOnly = true;
			this.Download.Width = 72;
			this.Upload.HeaderText = "Upload";
			this.Upload.MinimumWidth = 2;
			this.Upload.Name = "Upload";
			this.Upload.ReadOnly = true;
			this.Upload.Width = 72;
			this.DownloadRaw.HeaderText = "DloadRaw";
			this.DownloadRaw.MinimumWidth = 2;
			this.DownloadRaw.Name = "DownloadRaw";
			this.DownloadRaw.ReadOnly = true;
			this.DownloadRaw.Width = 60;
			this.ErrorPercent.HeaderText = "Error Percent";
			this.ErrorPercent.MinimumWidth = 2;
			this.ErrorPercent.Name = "ErrorPercent";
			this.ErrorPercent.ReadOnly = true;
			this.ErrorPercent.SortMode = global::System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.ErrorPercent.Visible = false;
			this.ErrorPercent.Width = 48;
			this.ConnectError.HeaderText = "Error";
			this.ConnectError.MinimumWidth = 2;
			this.ConnectError.Name = "ConnectError";
			this.ConnectError.ReadOnly = true;
			this.ConnectError.SortMode = global::System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.ConnectError.Width = 28;
			this.ConnectTimeout.HeaderText = "Timeout";
			this.ConnectTimeout.MinimumWidth = 2;
			this.ConnectTimeout.Name = "ConnectTimeout";
			this.ConnectTimeout.ReadOnly = true;
			this.ConnectTimeout.SortMode = global::System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.ConnectTimeout.Width = 28;
			this.ConnectEmpty.HeaderText = "Empty Response";
			this.ConnectEmpty.MinimumWidth = 2;
			this.ConnectEmpty.Name = "ConnectEmpty";
			this.ConnectEmpty.ReadOnly = true;
			this.ConnectEmpty.SortMode = global::System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.ConnectEmpty.Width = 28;
			this.Continuous.HeaderText = "Continuous";
			this.Continuous.Name = "Continuous";
			this.Continuous.ReadOnly = true;
			this.Continuous.Visible = false;
			this.Continuous.Width = 28;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(96f, 96f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Dpi;
			base.ClientSize = new global::System.Drawing.Size(128, 48);
			base.Controls.Add(this.tableLayoutPanel1);
			this.Font = new global::System.Drawing.Font("新宋体", 9f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 0);
			base.Margin = new global::System.Windows.Forms.Padding(3, 2, 3, 2);
			base.Name = "ServerLogForm";
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ServerLog";
			base.Activated += new global::System.EventHandler(this.ServerLogForm_Activated);
			base.FormClosed += new global::System.Windows.Forms.FormClosedEventHandler(this.ServerLogForm_FormClosed);
			base.ResizeEnd += new global::System.EventHandler(this.ServerLogForm_ResizeEnd);
			base.Move += new global::System.EventHandler(this.ServerLogForm_Move);
			((global::System.ComponentModel.ISupportInitialize)this.ServerDataGrid).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		// Token: 0x0400007F RID: 127
		private global::System.Windows.Forms.DataGridViewTextBoxColumn AvgDownSpeed;

		// Token: 0x0400007E RID: 126
		private global::System.Windows.Forms.DataGridViewTextBoxColumn AvgLatency;

		// Token: 0x04000081 RID: 129
		private global::System.Windows.Forms.DataGridViewTextBoxColumn AvgUpSpeed;

		// Token: 0x04000074 RID: 116
		private global::System.ComponentModel.IContainer components;

		// Token: 0x04000089 RID: 137
		private global::System.Windows.Forms.DataGridViewTextBoxColumn ConnectEmpty;

		// Token: 0x04000087 RID: 135
		private global::System.Windows.Forms.DataGridViewTextBoxColumn ConnectError;

		// Token: 0x0400007D RID: 125
		private global::System.Windows.Forms.DataGridViewTextBoxColumn Connecting;

		// Token: 0x04000088 RID: 136
		private global::System.Windows.Forms.DataGridViewTextBoxColumn ConnectTimeout;

		// Token: 0x0400008A RID: 138
		private global::System.Windows.Forms.DataGridViewTextBoxColumn Continuous;

		// Token: 0x04000083 RID: 131
		private global::System.Windows.Forms.DataGridViewTextBoxColumn Download;

		// Token: 0x04000085 RID: 133
		private global::System.Windows.Forms.DataGridViewTextBoxColumn DownloadRaw;

		// Token: 0x0400007B RID: 123
		private global::System.Windows.Forms.DataGridViewTextBoxColumn Enable;

		// Token: 0x04000086 RID: 134
		private global::System.Windows.Forms.DataGridViewTextBoxColumn ErrorPercent;

		// Token: 0x04000079 RID: 121
		private global::System.Windows.Forms.DataGridViewTextBoxColumn Group;

		// Token: 0x04000078 RID: 120
		private global::System.Windows.Forms.DataGridViewTextBoxColumn ID;

		// Token: 0x04000080 RID: 128
		private global::System.Windows.Forms.DataGridViewTextBoxColumn MaxDownSpeed;

		// Token: 0x04000082 RID: 130
		private global::System.Windows.Forms.DataGridViewTextBoxColumn MaxUpSpeed;

		// Token: 0x0400007A RID: 122
		private global::System.Windows.Forms.DataGridViewTextBoxColumn Server;

		// Token: 0x04000075 RID: 117
		private global::System.Windows.Forms.DataGridView ServerDataGrid;

		// Token: 0x04000077 RID: 119
		private global::System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

		// Token: 0x04000076 RID: 118
		private global::System.Windows.Forms.Timer timer;

		// Token: 0x0400007C RID: 124
		private global::System.Windows.Forms.DataGridViewTextBoxColumn TotalConnect;

		// Token: 0x04000084 RID: 132
		private global::System.Windows.Forms.DataGridViewTextBoxColumn Upload;
	}
}
