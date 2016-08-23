using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Shadowsocks.Controller;
using Shadowsocks.Model;
using Shadowsocks.Properties;

namespace Shadowsocks.View
{
	// Token: 0x02000009 RID: 9
	public partial class ServerLogForm : Form
	{
		// Token: 0x0600006C RID: 108 RVA: 0x000079D0 File Offset: 0x00005BD0
		public ServerLogForm(ShadowsocksController controller)
		{
			this.controller = controller;
			base.Icon = Icon.FromHandle(Resources.ssw128.GetHicon());
			this.InitializeComponent();
			base.Width = 810;
			Configuration currentConfiguration = controller.GetCurrentConfiguration();
			if (currentConfiguration.configs.Count < 8)
			{
				base.Height = 300;
			}
			else if (currentConfiguration.configs.Count < 20)
			{
				base.Height = 300 + (currentConfiguration.configs.Count - 8) * 16;
			}
			else
			{
				base.Height = 500;
			}
			this.UpdateTexts();
			this.UpdateLog();
			this.contextMenu1 = new ContextMenu(new MenuItem[]
			{
				this.CreateMenuItem("Auto &size", new EventHandler(this.autosizeItem_Click)),
				this.topmostItem = this.CreateMenuItem("Always On &Top", new EventHandler(this.topmostItem_Click)),
				new MenuItem("-"),
				this.CreateMenuItem("Clear &MaxSpeed", new EventHandler(this.ClearMaxSpeed_Click)),
				this.clearItem = this.CreateMenuItem("&Clear", new EventHandler(this.ClearItem_Click))
			});
			this.ServerDataGrid.ContextMenu = this.contextMenu1;
			controller.ConfigChanged += new EventHandler(this.controller_ConfigChanged);
			int num = 0;
			for (int i = 0; i < this.ServerDataGrid.Columns.Count; i++)
			{
				if (this.ServerDataGrid.Columns[i].Visible)
				{
					num += this.ServerDataGrid.Columns[i].Width;
				}
			}
			base.Width = num + SystemInformation.VerticalScrollBarWidth + (base.Width - base.ClientSize.Width) + 1;
			this.ServerDataGrid.AutoResizeColumnHeadersHeight();
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00008DF4 File Offset: 0x00006FF4
		private void autosizeColumns()
		{
			for (int i = 0; i < this.ServerDataGrid.Columns.Count; i++)
			{
				string name = this.ServerDataGrid.Columns[i].Name;
				if ((name == "AvgLatency" || name == "AvgDownSpeed" || name == "MaxDownSpeed" || name == "AvgUpSpeed" || name == "MaxUpSpeed" || name == "Upload" || name == "Download" || name == "DownloadRaw" || name == "Group" || name == "Connecting" || name == "ErrorPercent" || name == "ConnectError" || name == "ConnectTimeout" || name == "Continuous" || name == "ConnectEmpty") && this.ServerDataGrid.Columns[i].Width > 2)
				{
					this.ServerDataGrid.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.AllCellsExceptHeader);
					if (name == "AvgLatency" || name == "Connecting" || name == "AvgDownSpeed" || name == "MaxDownSpeed" || name == "AvgUpSpeed" || name == "MaxUpSpeed")
					{
						this.ServerDataGrid.Columns[i].MinimumWidth = this.ServerDataGrid.Columns[i].Width;
					}
				}
			}
			int num = 0;
			for (int j = 0; j < this.ServerDataGrid.Columns.Count; j++)
			{
				if (this.ServerDataGrid.Columns[j].Visible)
				{
					num += this.ServerDataGrid.Columns[j].Width;
				}
			}
			base.Width = num + SystemInformation.VerticalScrollBarWidth + (base.Width - base.ClientSize.Width) + 1;
			this.ServerDataGrid.AutoResizeColumnHeadersHeight();
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00009030 File Offset: 0x00007230
		private void autosizeItem_Click(object sender, EventArgs e)
		{
			this.autosizeColumns();
		}

		// Token: 0x0600007F RID: 127 RVA: 0x000090C4 File Offset: 0x000072C4
		private void ClearItem_Click(object sender, EventArgs e)
		{
			using (List<Server>.Enumerator enumerator = this.controller.GetCurrentConfiguration().configs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					enumerator.Current.ServerSpeedLog().Clear();
				}
			}
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00009064 File Offset: 0x00007264
		private void ClearMaxSpeed_Click(object sender, EventArgs e)
		{
			using (List<Server>.Enumerator enumerator = this.controller.GetCurrentConfiguration().configs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					enumerator.Current.ServerSpeedLog().ClearMaxSpeed();
				}
			}
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00007EE5 File Offset: 0x000060E5
		private byte ColorMix(byte a, byte b, double alpha)
		{
			return (byte)((double)b * alpha + (double)a * (1.0 - alpha));
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00007EFC File Offset: 0x000060FC
		private Color ColorMix(Color a, Color b, double alpha)
		{
			return Color.FromArgb((int)this.ColorMix(a.R, b.R, alpha), (int)this.ColorMix(a.G, b.G, alpha), (int)this.ColorMix(a.B, b.B, alpha));
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00007CB3 File Offset: 0x00005EB3
		private void controller_ConfigChanged(object sender, EventArgs e)
		{
			this.UpdateTitle();
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00005D3C File Offset: 0x00003F3C
		private MenuItem CreateMenuItem(string text, EventHandler click)
		{
			return new MenuItem(I18N.GetString(text), click);
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00007CBC File Offset: 0x00005EBC
		private string FormatBytes(long bytes)
		{
			if (bytes >= 1114640907774197760L)
			{
				return ((double)bytes / 1.152921504606847E+18).ToString("F5") + "E";
			}
			if (bytes >= 1088516511498240L)
			{
				return ((double)bytes / 1125899906842624.0).ToString("F5") + "P";
			}
			if (bytes >= 1063004405760L)
			{
				return ((double)bytes / 1099511627776.0).ToString("F5") + "T";
			}
			if (bytes >= 1038090240L)
			{
				return ((double)bytes / 1073741824.0).ToString("F4") + "G";
			}
			if (bytes >= 104857600L)
			{
				return ((double)bytes / 1048576.0).ToString("F1") + "M";
			}
			if (bytes >= 10485760L)
			{
				return ((double)bytes / 1048576.0).ToString("F2") + "M";
			}
			if (bytes >= 1013760L)
			{
				return ((double)bytes / 1048576.0).ToString("F3") + "M";
			}
			if (bytes > 2048L)
			{
				return ((double)bytes / 1024.0).ToString("F1") + "K";
			}
			return bytes.ToString();
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00008000 File Offset: 0x00006200
		public void RefreshLog()
		{
			if (this.ServerSpeedLogList == null)
			{
				return;
			}
			Configuration currentConfiguration = this.controller.GetCurrentConfiguration();
			if (this.listOrder.Count > currentConfiguration.configs.Count)
			{
				this.listOrder.RemoveRange(currentConfiguration.configs.Count, this.listOrder.Count - currentConfiguration.configs.Count);
			}
			while (this.listOrder.Count < currentConfiguration.configs.Count)
			{
				this.listOrder.Add(0);
			}
			while (this.ServerDataGrid.Rows.Count < currentConfiguration.configs.Count)
			{
				this.ServerDataGrid.Rows.Add();
				int num = this.ServerDataGrid.Rows.Count - 1;
				this.ServerDataGrid[0, num].Value = num;
			}
			if (this.ServerDataGrid.Rows.Count > currentConfiguration.configs.Count)
			{
				for (int i = 0; i < this.ServerDataGrid.Rows.Count; i++)
				{
					if ((int)this.ServerDataGrid[0, i].Value >= currentConfiguration.configs.Count)
					{
						this.ServerDataGrid.Rows.RemoveAt(i);
						i--;
					}
				}
			}
			try
			{
				int num2 = (this.lastRefreshIndex >= this.ServerDataGrid.Rows.Count) ? 0 : this.lastRefreshIndex;
				int num3 = 0;
				while (num2 < this.ServerDataGrid.Rows.Count && num3 <= 200)
				{
					this.lastRefreshIndex = num2 + 1;
					int num4 = (int)this.ServerDataGrid[0, num2].Value;
					Server server = currentConfiguration.configs[num4];
					ServerSpeedLogShow serverSpeedLogShow = this.ServerSpeedLogList[num4];
					this.listOrder[num4] = num2;
					this.rowChange = false;
					for (int j = 0; j < this.ServerDataGrid.Columns.Count; j++)
					{
						DataGridViewCell dataGridViewCell = this.ServerDataGrid[j, num2];
						string name = this.ServerDataGrid.Columns[j].Name;
						if (name == "Server")
						{
							if (currentConfiguration.index == num4)
							{
								this.SetBackColor(dataGridViewCell, Color.Cyan);
							}
							else
							{
								this.SetBackColor(dataGridViewCell, Color.White);
							}
							this.SetCellText(dataGridViewCell, server.FriendlyName());
						}
						if (name == "Group")
						{
							this.SetCellText(dataGridViewCell, server.group);
						}
						if (name == "Enable")
						{
							if (server.isEnable())
							{
								this.SetBackColor(dataGridViewCell, Color.White);
							}
							else
							{
								this.SetBackColor(dataGridViewCell, Color.Red);
							}
						}
						else if (name == "TotalConnect")
						{
							this.SetCellText(dataGridViewCell, serverSpeedLogShow.totalConnectTimes);
						}
						else if (name == "Connecting")
						{
							long num5 = serverSpeedLogShow.totalConnectTimes - serverSpeedLogShow.totalDisconnectTimes;
							Color[] array = new Color[]
							{
								Color.White,
								Color.LightGreen,
								Color.Yellow,
								Color.Red,
								Color.Red
							};
							long[] array2 = new long[]
							{
								0L,
								16L,
								32L,
								64L,
								65536L
							};
							for (int k = 1; k < array.Length; k++)
							{
								if (num5 < array2[k])
								{
									this.SetBackColor(dataGridViewCell, this.ColorMix(array[k - 1], array[k], (double)(num5 - array2[k - 1]) / (double)(array2[k] - array2[k - 1])));
									break;
								}
							}
							this.SetCellText(dataGridViewCell, serverSpeedLogShow.totalConnectTimes - serverSpeedLogShow.totalDisconnectTimes);
						}
						else if (name == "AvgLatency")
						{
							if (serverSpeedLogShow.avgConnectTime >= 0L)
							{
								this.SetCellText(dataGridViewCell, serverSpeedLogShow.avgConnectTime);
							}
							else
							{
								this.SetCellText(dataGridViewCell, "-");
							}
						}
						else if (name == "AvgDownSpeed")
						{
							long avgDownloadBytes = serverSpeedLogShow.avgDownloadBytes;
							string newString = this.FormatBytes(avgDownloadBytes);
							Color[] array3 = new Color[]
							{
								Color.White,
								Color.LightGreen,
								Color.Yellow,
								Color.Pink,
								Color.Red,
								Color.Red
							};
							long[] array4 = new long[]
							{
								0L,
								65536L,
								524288L,
								4194304L,
								16777216L,
								1099511627776L
							};
							for (int l = 1; l < array3.Length; l++)
							{
								if (avgDownloadBytes < array4[l])
								{
									this.SetBackColor(dataGridViewCell, this.ColorMix(array3[l - 1], array3[l], (double)(avgDownloadBytes - array4[l - 1]) / (double)(array4[l] - array4[l - 1])));
									break;
								}
							}
							this.SetCellText(dataGridViewCell, newString);
						}
						else if (name == "MaxDownSpeed")
						{
							long maxDownloadBytes = serverSpeedLogShow.maxDownloadBytes;
							string newString2 = this.FormatBytes(maxDownloadBytes);
							Color[] array5 = new Color[]
							{
								Color.White,
								Color.LightGreen,
								Color.Yellow,
								Color.Pink,
								Color.Red,
								Color.Red
							};
							long[] array6 = new long[]
							{
								0L,
								65536L,
								524288L,
								4194304L,
								16777216L,
								1073741824L
							};
							for (int m = 1; m < array5.Length; m++)
							{
								if (maxDownloadBytes < array6[m])
								{
									this.SetBackColor(dataGridViewCell, this.ColorMix(array5[m - 1], array5[m], (double)(maxDownloadBytes - array6[m - 1]) / (double)(array6[m] - array6[m - 1])));
									break;
								}
							}
							this.SetCellText(dataGridViewCell, newString2);
						}
						else if (name == "AvgUpSpeed")
						{
							long avgUploadBytes = serverSpeedLogShow.avgUploadBytes;
							string newString3 = this.FormatBytes(avgUploadBytes);
							Color[] array7 = new Color[]
							{
								Color.White,
								Color.LightGreen,
								Color.Yellow,
								Color.Pink,
								Color.Red,
								Color.Red
							};
							long[] array8 = new long[]
							{
								0L,
								65536L,
								524288L,
								4194304L,
								16777216L,
								1099511627776L
							};
							for (int n = 1; n < array7.Length; n++)
							{
								if (avgUploadBytes < array8[n])
								{
									this.SetBackColor(dataGridViewCell, this.ColorMix(array7[n - 1], array7[n], (double)(avgUploadBytes - array8[n - 1]) / (double)(array8[n] - array8[n - 1])));
									break;
								}
							}
							this.SetCellText(dataGridViewCell, newString3);
						}
						else if (name == "MaxUpSpeed")
						{
							long maxUploadBytes = serverSpeedLogShow.maxUploadBytes;
							string newString4 = this.FormatBytes(maxUploadBytes);
							Color[] array9 = new Color[]
							{
								Color.White,
								Color.LightGreen,
								Color.Yellow,
								Color.Pink,
								Color.Red,
								Color.Red
							};
							long[] array10 = new long[]
							{
								0L,
								65536L,
								524288L,
								4194304L,
								16777216L,
								1073741824L
							};
							for (int num6 = 1; num6 < array9.Length; num6++)
							{
								if (maxUploadBytes < array10[num6])
								{
									this.SetBackColor(dataGridViewCell, this.ColorMix(array9[num6 - 1], array9[num6], (double)(maxUploadBytes - array10[num6 - 1]) / (double)(array10[num6] - array10[num6 - 1])));
									break;
								}
							}
							this.SetCellText(dataGridViewCell, newString4);
						}
						else if (name == "Upload")
						{
							string newString5 = this.FormatBytes(serverSpeedLogShow.totalUploadBytes);
							string text = serverSpeedLogShow.totalUploadBytes.ToString();
							if (dataGridViewCell.ToolTipText != text)
							{
								if (text == "0")
								{
									this.SetBackColor(dataGridViewCell, Color.FromArgb(244, 255, 244));
								}
								else
								{
									this.SetBackColor(dataGridViewCell, Color.LightGreen);
									dataGridViewCell.Tag = 8;
								}
							}
							else if (dataGridViewCell.Tag != null)
							{
								DataGridViewCell expr_84A = dataGridViewCell;
								expr_84A.Tag = (int)expr_84A.Tag - 1;
								if ((int)dataGridViewCell.Tag == 0)
								{
									this.SetBackColor(dataGridViewCell, Color.FromArgb(244, 255, 244));
								}
							}
							this.SetCellToolTipText(dataGridViewCell, text);
							this.SetCellText(dataGridViewCell, newString5);
						}
						else if (name == "Download")
						{
							string newString6 = this.FormatBytes(serverSpeedLogShow.totalDownloadBytes);
							string text2 = serverSpeedLogShow.totalDownloadBytes.ToString();
							if (dataGridViewCell.ToolTipText != text2)
							{
								if (text2 == "0")
								{
									this.SetBackColor(dataGridViewCell, Color.FromArgb(255, 240, 240));
								}
								else
								{
									this.SetBackColor(dataGridViewCell, Color.LightGreen);
									dataGridViewCell.Tag = 8;
								}
							}
							else if (dataGridViewCell.Tag != null)
							{
								DataGridViewCell expr_93A = dataGridViewCell;
								expr_93A.Tag = (int)expr_93A.Tag - 1;
								if ((int)dataGridViewCell.Tag == 0)
								{
									this.SetBackColor(dataGridViewCell, Color.FromArgb(255, 240, 240));
								}
							}
							this.SetCellToolTipText(dataGridViewCell, text2);
							this.SetCellText(dataGridViewCell, newString6);
						}
						else if (name == "DownloadRaw")
						{
							string newString7 = this.FormatBytes(serverSpeedLogShow.totalDownloadRawBytes);
							string text3 = serverSpeedLogShow.totalDownloadRawBytes.ToString();
							if (dataGridViewCell.ToolTipText != text3)
							{
								if (text3 == "0")
								{
									this.SetBackColor(dataGridViewCell, Color.FromArgb(255, 128, 128));
								}
								else
								{
									this.SetBackColor(dataGridViewCell, Color.LightGreen);
									dataGridViewCell.Tag = 8;
								}
							}
							else if (dataGridViewCell.Tag != null)
							{
								DataGridViewCell expr_A2A = dataGridViewCell;
								expr_A2A.Tag = (int)expr_A2A.Tag - 1;
								if ((int)dataGridViewCell.Tag == 0)
								{
									this.SetBackColor(dataGridViewCell, Color.FromArgb(240, 240, 255));
								}
							}
							this.SetCellToolTipText(dataGridViewCell, text3);
							this.SetCellText(dataGridViewCell, newString7);
						}
						else if (name == "ConnectError")
						{
							long num7 = serverSpeedLogShow.errorConnectTimes + serverSpeedLogShow.errorDecodeTimes;
							Color newColor = Color.FromArgb(255, (int)((byte)Math.Max(0.0, 255.0 - (double)num7 * 2.5)), (int)((byte)Math.Max(0.0, 255.0 - (double)num7 * 2.5)));
							this.SetBackColor(dataGridViewCell, newColor);
							this.SetCellText(dataGridViewCell, num7);
						}
						else if (name == "ConnectTimeout")
						{
							this.SetCellText(dataGridViewCell, serverSpeedLogShow.errorTimeoutTimes);
						}
						else if (name == "ConnectEmpty")
						{
							long errorEmptyTimes = serverSpeedLogShow.errorEmptyTimes;
							Color newColor2 = Color.FromArgb(255, (int)((byte)Math.Max(0L, 255L - errorEmptyTimes * 8L)), (int)((byte)Math.Max(0L, 255L - errorEmptyTimes * 8L)));
							this.SetBackColor(dataGridViewCell, newColor2);
							this.SetCellText(dataGridViewCell, errorEmptyTimes);
						}
						else if (name == "Continuous")
						{
							long errorContinurousTimes = serverSpeedLogShow.errorContinurousTimes;
							Color newColor3 = Color.FromArgb(255, (int)((byte)Math.Max(0L, 255L - errorContinurousTimes * 8L)), (int)((byte)Math.Max(0L, 255L - errorContinurousTimes * 8L)));
							this.SetBackColor(dataGridViewCell, newColor3);
							this.SetCellText(dataGridViewCell, errorContinurousTimes);
						}
						else if (name == "ErrorPercent")
						{
							if (serverSpeedLogShow.errorLogTimes + serverSpeedLogShow.totalConnectTimes - serverSpeedLogShow.totalDisconnectTimes > 0L)
							{
								double num8 = (double)(serverSpeedLogShow.errorConnectTimes + serverSpeedLogShow.errorTimeoutTimes + serverSpeedLogShow.errorDecodeTimes) * 100.0 / (double)(serverSpeedLogShow.errorLogTimes + serverSpeedLogShow.totalConnectTimes - serverSpeedLogShow.totalDisconnectTimes);
								this.SetBackColor(dataGridViewCell, Color.FromArgb(255, (int)((byte)(255.0 - num8 * 2.0)), (int)((byte)(255.0 - num8 * 2.0))));
								this.SetCellText(dataGridViewCell, num8.ToString("F0") + "%");
							}
							else
							{
								this.SetBackColor(dataGridViewCell, Color.White);
								this.SetCellText(dataGridViewCell, "-");
							}
						}
						if (name == "Server")
						{
							if (dataGridViewCell.Style.Alignment != DataGridViewContentAlignment.MiddleLeft)
							{
								dataGridViewCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
							}
						}
						else if (dataGridViewCell.Style.Alignment != DataGridViewContentAlignment.MiddleRight)
						{
							dataGridViewCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
						}
					}
					if (this.rowChange)
					{
						num3++;
					}
					num2++;
					num3++;
				}
			}
			catch
			{
			}
			if (this.ServerDataGrid.SortedColumn != null)
			{
				this.ServerDataGrid.Sort(this.ServerDataGrid.SortedColumn, (ListSortDirection)(this.ServerDataGrid.SortOrder - 1));
			}
		}

		// Token: 0x06000082 RID: 130 RVA: 0x000091EC File Offset: 0x000073EC
		private void ServerDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0)
			{
				int index = (int)this.ServerDataGrid[0, e.RowIndex].Value;
				if (this.ServerDataGrid.Columns[e.ColumnIndex].Name == "Server")
				{
					this.controller.GetCurrentConfiguration();
					this.controller.SelectServerIndex(index);
				}
				if (this.ServerDataGrid.Columns[e.ColumnIndex].Name == "Group")
				{
					Configuration currentConfiguration = this.controller.GetCurrentConfiguration();
					Server server = currentConfiguration.configs[index];
					string group = server.group;
					if (group != null && group.Length > 0)
					{
						bool flag = !server.enable;
						foreach (Server current in currentConfiguration.configs)
						{
							if (current.group == group && current.enable != flag)
							{
								current.setEnable(flag);
							}
						}
						this.controller.SelectServerIndex(currentConfiguration.index);
					}
				}
				if (this.ServerDataGrid.Columns[e.ColumnIndex].Name == "Enable")
				{
					Configuration currentConfiguration2 = this.controller.GetCurrentConfiguration();
					Server expr_16F = currentConfiguration2.configs[index];
					expr_16F.setEnable(!expr_16F.isEnable());
					this.controller.SelectServerIndex(currentConfiguration2.index);
				}
				this.ServerDataGrid[0, e.RowIndex].Selected = true;
			}
		}

		// Token: 0x06000083 RID: 131 RVA: 0x000093B0 File Offset: 0x000075B0
		private void ServerDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0)
			{
				int index = (int)this.ServerDataGrid[0, e.RowIndex].Value;
				if (this.ServerDataGrid.Columns[e.ColumnIndex].Name == "Server")
				{
					this.controller.ShowConfigForm();
				}
				if (this.ServerDataGrid.Columns[e.ColumnIndex].Name == "Connecting")
				{
					this.controller.GetCurrentConfiguration().configs[index].GetConnections().CloseAll();
				}
				if (this.ServerDataGrid.Columns[e.ColumnIndex].Name == "MaxDownSpeed" || this.ServerDataGrid.Columns[e.ColumnIndex].Name == "MaxUpSpeed")
				{
					this.controller.GetCurrentConfiguration().configs[index].ServerSpeedLog().ClearMaxSpeed();
				}
				if (this.ServerDataGrid.Columns[e.ColumnIndex].Name == "Upload" || this.ServerDataGrid.Columns[e.ColumnIndex].Name == "Download")
				{
					Configuration expr_169 = this.controller.GetCurrentConfiguration();
					expr_169.configs[index].ServerSpeedLog().Clear();
					expr_169.configs[index].setEnable(true);
				}
				if (this.ServerDataGrid.Columns[e.ColumnIndex].Name == "ConnectError" || this.ServerDataGrid.Columns[e.ColumnIndex].Name == "ConnectTimeout" || this.ServerDataGrid.Columns[e.ColumnIndex].Name == "ConnectEmpty" || this.ServerDataGrid.Columns[e.ColumnIndex].Name == "Continuous")
				{
					Configuration expr_237 = this.controller.GetCurrentConfiguration();
					expr_237.configs[index].ServerSpeedLog().ClearError();
					expr_237.configs[index].setEnable(true);
				}
				this.ServerDataGrid[0, e.RowIndex].Selected = true;
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00009C14 File Offset: 0x00007E14
		private void ServerDataGrid_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
		{
			int num = 0;
			for (int i = 0; i < this.ServerDataGrid.Columns.Count; i++)
			{
				if (this.ServerDataGrid.Columns[i].Visible)
				{
					num += this.ServerDataGrid.Columns[i].Width;
				}
			}
			base.Width = num + SystemInformation.VerticalScrollBarWidth + (base.Width - base.ClientSize.Width) + 1;
			this.ServerDataGrid.AutoResizeColumnHeadersHeight();
		}

		// Token: 0x06000081 RID: 129 RVA: 0x000091B9 File Offset: 0x000073B9
		private void ServerDataGrid_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				this.contextMenu1.Show(this.ServerDataGrid, new Point(e.X, e.Y));
			}
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000097AC File Offset: 0x000079AC
		private void ServerDataGrid_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
		{
			if (e.Column.Name == "Server" || e.Column.Name == "Group")
			{
				e.SortResult = string.Compare(Convert.ToString(e.CellValue1), Convert.ToString(e.CellValue2));
				e.Handled = true;
			}
			else if (e.Column.Name == "ID" || e.Column.Name == "TotalConnect" || e.Column.Name == "Connecting" || e.Column.Name == "ConnectError" || e.Column.Name == "ConnectTimeout" || e.Column.Name == "Continuous")
			{
				int num = Convert.ToInt32(e.CellValue1);
				int num2 = Convert.ToInt32(e.CellValue2);
				e.SortResult = ((num == num2) ? 0 : ((num < num2) ? -1 : 1));
			}
			else if (e.Column.Name == "ErrorPercent")
			{
				string text = Convert.ToString(e.CellValue1);
				string text2 = Convert.ToString(e.CellValue2);
				int num3 = (text.Length <= 1) ? 0 : Convert.ToInt32(Convert.ToDouble(text.Substring(0, text.Length - 1)) * 100.0);
				int num4 = (text2.Length <= 1) ? 0 : Convert.ToInt32(Convert.ToDouble(text2.Substring(0, text2.Length - 1)) * 100.0);
				e.SortResult = ((num3 == num4) ? 0 : ((num3 < num4) ? -1 : 1));
			}
			else if (e.Column.Name == "AvgLatency" || e.Column.Name == "AvgDownSpeed" || e.Column.Name == "MaxDownSpeed" || e.Column.Name == "AvgUpSpeed" || e.Column.Name == "MaxUpSpeed" || e.Column.Name == "Upload" || e.Column.Name == "Download" || e.Column.Name == "DownloadRaw")
			{
				string str = Convert.ToString(e.CellValue1);
				string str2 = Convert.ToString(e.CellValue2);
				long num5 = this.Str2Long(str);
				long num6 = this.Str2Long(str2);
				e.SortResult = ((num5 == num6) ? 0 : ((num5 < num6) ? -1 : 1));
			}
			if (e.SortResult == 0)
			{
				int num7 = this.listOrder[Convert.ToInt32(this.ServerDataGrid[0, e.RowIndex1].Value)];
				int num8 = this.listOrder[Convert.ToInt32(this.ServerDataGrid[0, e.RowIndex2].Value)];
				e.SortResult = ((num7 == num8) ? 0 : ((num7 < num8) ? -1 : 1));
				if (e.SortResult != 0 && this.ServerDataGrid.SortOrder == SortOrder.Descending)
				{
					e.SortResult = -e.SortResult;
				}
			}
			if (e.SortResult != 0)
			{
				e.Handled = true;
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00009C9F File Offset: 0x00007E9F
		private void ServerLogForm_Activated(object sender, EventArgs e)
		{
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00009634 File Offset: 0x00007834
		private void ServerLogForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.controller.ConfigChanged -= new EventHandler(this.controller_ConfigChanged);
			Thread thread = this.workerThread;
			this.workerThread = null;
			while (thread.IsAlive)
			{
				this.workerEvent.Set();
				Thread.Sleep(50);
			}
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00009B2F File Offset: 0x00007D2F
		private void ServerLogForm_Move(object sender, EventArgs e)
		{
			this.updatePause = 0;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00009B6C File Offset: 0x00007D6C
		private void ServerLogForm_ResizeEnd(object sender, EventArgs e)
		{
			this.updatePause = 0;
			int num = 0;
			for (int i = 0; i < this.ServerDataGrid.Columns.Count; i++)
			{
				if (this.ServerDataGrid.Columns[i].Visible)
				{
					num += this.ServerDataGrid.Columns[i].Width;
				}
			}
			num += SystemInformation.VerticalScrollBarWidth + (base.Width - base.ClientSize.Width) + 1;
			this.ServerDataGrid.Columns[2].Width += base.Width - num;
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00007E41 File Offset: 0x00006041
		public bool SetBackColor(DataGridViewCell cell, Color newColor)
		{
			if (cell.Style.BackColor != newColor)
			{
				cell.Style.BackColor = newColor;
				this.rowChange = true;
				return true;
			}
			return false;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00007E8D File Offset: 0x0000608D
		public bool SetCellText(DataGridViewCell cell, string newString)
		{
			if ((string)cell.Value != newString)
			{
				cell.Value = newString;
				this.rowChange = true;
				return true;
			}
			return false;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00007EB3 File Offset: 0x000060B3
		public bool SetCellText(DataGridViewCell cell, long newInteger)
		{
			if ((string)cell.Value != newInteger.ToString())
			{
				cell.Value = newInteger.ToString();
				this.rowChange = true;
				return true;
			}
			return false;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00007E6C File Offset: 0x0000606C
		public bool SetCellToolTipText(DataGridViewCell cell, string newString)
		{
			if (cell.ToolTipText != newString)
			{
				cell.ToolTipText = newString;
				this.rowChange = true;
				return true;
			}
			return false;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00009684 File Offset: 0x00007884
		private long Str2Long(string str)
		{
			if (str == "-")
			{
				return -1L;
			}
			if (str.LastIndexOf('K') > 0)
			{
				return (long)(Convert.ToDouble(str.Substring(0, str.LastIndexOf('K'))) * 1024.0);
			}
			if (str.LastIndexOf('M') > 0)
			{
				return (long)(Convert.ToDouble(str.Substring(0, str.LastIndexOf('M'))) * 1024.0 * 1024.0);
			}
			if (str.LastIndexOf('G') > 0)
			{
				return (long)(Convert.ToDouble(str.Substring(0, str.LastIndexOf('G'))) * 1024.0 * 1024.0 * 1024.0);
			}
			if (str.LastIndexOf('T') > 0)
			{
				return (long)(Convert.ToDouble(str.Substring(0, str.LastIndexOf('T'))) * 1024.0 * 1024.0 * 1024.0 * 1024.0);
			}
			long result;
			try
			{
				result = (long)Convert.ToDouble(str);
			}
			catch
			{
				result = -1L;
			}
			return result;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00009124 File Offset: 0x00007324
		private void timer_Tick(object sender, EventArgs e)
		{
			if (this.updatePause > 0)
			{
				this.updatePause--;
				return;
			}
			if (base.WindowState == FormWindowState.Minimized)
			{
				int num = this.pendingUpdate + 1;
				this.pendingUpdate = num;
				if (num < 40)
				{
					return;
				}
			}
			else
			{
				this.updateTick++;
			}
			this.pendingUpdate = 0;
			this.RefreshLog();
			this.UpdateLog();
			if (this.updateSize > 1)
			{
				this.updateSize--;
			}
			if (this.updateTick == 2 || this.updateSize == 1)
			{
				this.updateSize = 0;
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00009038 File Offset: 0x00007238
		private void topmostItem_Click(object sender, EventArgs e)
		{
			this.topmostItem.Checked = !this.topmostItem.Checked;
			base.TopMost = this.topmostItem.Checked;
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00007FC6 File Offset: 0x000061C6
		public void UpdateLog()
		{
			if (this.workerThread == null)
			{
				this.workerThread = new Thread(new ThreadStart(this.UpdateLogThread));
				this.workerThread.Start();
				return;
			}
			this.workerEvent.Set();
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00007F50 File Offset: 0x00006150
		public void UpdateLogThread()
		{
			while (this.workerThread != null)
			{
				Configuration currentConfiguration = this.controller.GetCurrentConfiguration();
				ServerSpeedLogShow[] array = new ServerSpeedLogShow[currentConfiguration.configs.Count];
				for (int i = 0; i < currentConfiguration.configs.Count; i++)
				{
					array[i] = currentConfiguration.configs[i].ServerSpeedLog().Translate();
				}
				this.ServerSpeedLogList = array;
				this.workerEvent.WaitOne();
			}
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00007C54 File Offset: 0x00005E54
		private void UpdateTexts()
		{
			this.UpdateTitle();
			for (int i = 0; i < this.ServerDataGrid.Columns.Count; i++)
			{
				this.ServerDataGrid.Columns[i].HeaderText = I18N.GetString(this.ServerDataGrid.Columns[i].HeaderText);
			}
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00007BC8 File Offset: 0x00005DC8
		private void UpdateTitle()
		{
			this.Text = string.Concat(new string[]
			{
				I18N.GetString("ServerLog"),
				"(",
				this.controller.GetCurrentConfiguration().shareOverLan ? "any" : "local",
				":",
				this.controller.GetCurrentConfiguration().localPort.ToString(),
				I18N.GetString(" Version"),
				"3.8.5.0 Beta)"
			});
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00009B38 File Offset: 0x00007D38
		protected override void WndProc(ref Message message)
		{
			int msg = message.Msg;
			if (msg == 532 || msg == 534)
			{
				this.updatePause = 2;
			}
			base.WndProc(ref message);
		}

		// Token: 0x04000069 RID: 105
		private MenuItem clearItem;

		// Token: 0x04000067 RID: 103
		private ContextMenu contextMenu1;

		// Token: 0x04000066 RID: 102
		private ShadowsocksController controller;

		// Token: 0x0400006B RID: 107
		private int lastRefreshIndex;

		// Token: 0x0400006A RID: 106
		private List<int> listOrder = new List<int>();

		// Token: 0x04000070 RID: 112
		private int pendingUpdate;

		// Token: 0x0400006C RID: 108
		private bool rowChange;

		// Token: 0x04000071 RID: 113
		private ServerSpeedLogShow[] ServerSpeedLogList;

		// Token: 0x04000068 RID: 104
		private MenuItem topmostItem;

		// Token: 0x0400006D RID: 109
		private int updatePause;

		// Token: 0x0400006F RID: 111
		private int updateSize;

		// Token: 0x0400006E RID: 110
		private int updateTick;

		// Token: 0x04000073 RID: 115
		private AutoResetEvent workerEvent = new AutoResetEvent(false);

		// Token: 0x04000072 RID: 114
		private Thread workerThread;
	}
}
