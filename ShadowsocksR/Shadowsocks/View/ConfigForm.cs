using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Shadowsocks.Controller;
using Shadowsocks.Encryption;
using Shadowsocks.Model;
using Shadowsocks.Obfs;
using Shadowsocks.Properties;
using ZXing.QrCode.Internal;

namespace Shadowsocks.View
{
	// Token: 0x02000003 RID: 3
	public partial class ConfigForm : Form
	{
		// Token: 0x06000002 RID: 2 RVA: 0x0000210C File Offset: 0x0000030C
		public ConfigForm(ShadowsocksController controller, UpdateChecker updateChecker, int focusIndex)
		{
			this.Font = SystemFonts.MessageBoxFont;
			this.InitializeComponent();
			base.Icon = Icon.FromHandle(Resources.ssw128.GetHicon());
			this.controller = controller;
			this.updateChecker = updateChecker;
			if (updateChecker.LatestVersionURL == null)
			{
				this.LinkUpdate.Visible = false;
			}
			foreach (string current in EncryptorFactory.GetEncryptor())
			{
				this.EncryptionSelect.Items.Add(current);
			}
			this.UpdateTexts();
			controller.ConfigChanged += new EventHandler(this.controller_ConfigChanged);
			this.LoadCurrentConfiguration();
			if (this._modifiedConfiguration.index >= 0 && this._modifiedConfiguration.index < this._modifiedConfiguration.configs.Count)
			{
				this._oldSelectedID = this._modifiedConfiguration.configs[this._modifiedConfiguration.index].id;
			}
			if (focusIndex == -1)
			{
				focusIndex = this._modifiedConfiguration.configs.Count - 1;
			}
			if (focusIndex >= 0 && focusIndex < this._modifiedConfiguration.configs.Count)
			{
				this.SetServerListSelectedIndex(focusIndex);
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002E90 File Offset: 0x00001090
		private void AddButton_Click(object sender, EventArgs e)
		{
			if (this.SaveOldSelectedServer() == -1)
			{
				return;
			}
			Server defaultServer = Configuration.GetDefaultServer();
			this._modifiedConfiguration.configs.Add(defaultServer);
			this.LoadConfiguration(this._modifiedConfiguration);
			this.ServersListBox.SelectedIndex = this._modifiedConfiguration.configs.Count - 1;
			this._oldSelectedIndex = this.ServersListBox.SelectedIndex;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00003051 File Offset: 0x00001251
		private void CancelButton_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000032A8 File Offset: 0x000014A8
		private void checkAdvSetting_CheckedChanged(object sender, EventArgs e)
		{
			if (this.checkAdvSetting.Checked)
			{
				this.labelUDPPort.Visible = true;
				this.textUDPPort.Visible = true;
				this.UDPoverTCPLabel.Visible = true;
				this.CheckUDPoverUDP.Visible = true;
				return;
			}
			this.labelUDPPort.Visible = false;
			this.textUDPPort.Visible = false;
			this.UDPoverTCPLabel.Visible = false;
			this.CheckUDPoverUDP.Visible = false;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00003298 File Offset: 0x00001498
		private void checkSSRLink_CheckedChanged(object sender, EventArgs e)
		{
			this.SaveOldSelectedServer();
			this.LoadSelectedServer();
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00003067 File Offset: 0x00001267
		private void ConfigForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.controller.ConfigChanged -= new EventHandler(this.controller_ConfigChanged);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00003059 File Offset: 0x00001259
		private void ConfigForm_Shown(object sender, EventArgs e)
		{
			this.IPTextBox.Focus();
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000025D9 File Offset: 0x000007D9
		private void controller_ConfigChanged(object sender, EventArgs e)
		{
			this.LoadCurrentConfiguration();
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002EF8 File Offset: 0x000010F8
		private void DeleteButton_Click(object sender, EventArgs e)
		{
			this._oldSelectedIndex = this.ServersListBox.SelectedIndex;
			if (this._oldSelectedIndex >= 0 && this._oldSelectedIndex < this._modifiedConfiguration.configs.Count)
			{
				this._modifiedConfiguration.configs.RemoveAt(this._oldSelectedIndex);
			}
			if (this._oldSelectedIndex >= this._modifiedConfiguration.configs.Count)
			{
				this._oldSelectedIndex = this._modifiedConfiguration.configs.Count - 1;
			}
			this.ServersListBox.SelectedIndex = this._oldSelectedIndex;
			this.LoadConfiguration(this._modifiedConfiguration);
			this.SetServerListSelectedIndex(this._oldSelectedIndex);
			this.LoadSelectedServer();
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00003114 File Offset: 0x00001314
		private void DownButton_Click(object sender, EventArgs e)
		{
			this._oldSelectedIndex = this.ServersListBox.SelectedIndex;
			int oldSelectedIndex = this._oldSelectedIndex;
			this.SaveOldSelectedServer();
			if (this._oldSelectedIndex >= 0 && this._oldSelectedIndex < this._modifiedConfiguration.configs.Count - 1)
			{
				this._modifiedConfiguration.configs.Reverse(oldSelectedIndex, 2);
				this._oldSelectedIndex = oldSelectedIndex + 1;
				this.ServersListBox.SelectedIndex = oldSelectedIndex + 1;
				this.LoadConfiguration(this._modifiedConfiguration);
				this.ServersListBox.SelectedIndex = this._oldSelectedIndex;
				this.LoadSelectedServer();
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000028C4 File Offset: 0x00000AC4
		private void GenQR(string ssconfig)
		{
			ByteMatrix matrix = Encoder.encode(ssconfig, ErrorCorrectionLevel.M).Matrix;
			int num = Math.Max(260 / matrix.Height, 1);
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
								Graphics arg_7B_0 = graphics;
								Brush arg_7B_1 = brush;
								int arg_7B_2 = num * i;
								int arg_7B_3 = num * j;
								int expr_7A = num;
								arg_7B_0.FillRectangle(arg_7B_1, arg_7B_2, arg_7B_3, expr_7A, expr_7A);
							}
						}
					}
				}
			}
			this.PictureQRcode.Image = image;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000031E3 File Offset: 0x000013E3
		private void LinkUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(this.updateChecker.LatestVersionURL);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002BC4 File Offset: 0x00000DC4
		private void LoadConfiguration(Configuration configuration)
		{
			if (this.ServersListBox.Items.Count != this._modifiedConfiguration.configs.Count)
			{
				this.ServersListBox.Items.Clear();
				using (List<Server>.Enumerator enumerator = this._modifiedConfiguration.configs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Server current = enumerator.Current;
						if (current.group != null && current.group.Length > 0)
						{
							this.ServersListBox.Items.Add(current.group + " - " + current.FriendlyName());
						}
						else
						{
							this.ServersListBox.Items.Add("      " + current.FriendlyName());
						}
					}
					return;
				}
			}
			for (int i = 0; i < this._modifiedConfiguration.configs.Count; i++)
			{
				if (this._modifiedConfiguration.configs[i].group != null && this._modifiedConfiguration.configs[i].group.Length > 0)
				{
					this.ServersListBox.Items[i] = this._modifiedConfiguration.configs[i].group + " - " + this._modifiedConfiguration.configs[i].FriendlyName();
				}
				else
				{
					this.ServersListBox.Items[i] = "      " + this._modifiedConfiguration.configs[i].FriendlyName();
				}
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002DE6 File Offset: 0x00000FE6
		private void LoadCurrentConfiguration()
		{
			this._modifiedConfiguration = this.controller.GetConfiguration();
			this.LoadConfiguration(this._modifiedConfiguration);
			this.SetServerListSelectedIndex(this._modifiedConfiguration.index);
			this.LoadSelectedServer();
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000029B4 File Offset: 0x00000BB4
		private void LoadSelectedServer()
		{
			if (this.ServersListBox.SelectedIndex >= 0 && this.ServersListBox.SelectedIndex < this._modifiedConfiguration.configs.Count)
			{
				Server server = this._modifiedConfiguration.configs[this.ServersListBox.SelectedIndex];
				this.IPTextBox.Text = server.server;
				this.ServerPortTextBox.Text = server.server_port.ToString();
				this.textUDPPort.Text = server.server_udp_port.ToString();
				this.PasswordTextBox.Text = server.password;
				this.EncryptionSelect.Text = (server.method ?? "aes-256-cfb");
				if (server.protocol == null || server.protocol.Length == 0)
				{
					this.TCPProtocolComboBox.Text = "origin";
				}
				else
				{
					this.TCPProtocolComboBox.Text = (server.protocol ?? "origin");
				}
				this.ObfsCombo.Text = (server.obfs ?? "plain");
				this.textObfsParam.Text = server.obfsparam;
				this.RemarksTextBox.Text = server.remarks;
				this.TextGroup.Text = server.group;
				this.CheckUDPoverUDP.Checked = server.udp_over_tcp;
				this._SelectedID = server.id;
				this.ServerGroupBox.Visible = true;
				if (this.checkSSRLink.Checked)
				{
					this.TextLink.Text = this.controller.GetSSRRemarksLinkForServer(server);
				}
				else
				{
					this.TextLink.Text = this.controller.GetSSLinkForServer(server);
				}
				if (this.CheckTCPoverUDP.Checked || this.CheckUDPoverUDP.Checked || server.server_udp_port != 0)
				{
					this.checkAdvSetting.Checked = true;
				}
				this.PasswordLabel.Checked = false;
				this.GenQR(this.TextLink.Text);
				return;
			}
			this.ServerGroupBox.Visible = false;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00003220 File Offset: 0x00001420
		private void ObfsCombo_TextChanged(object sender, EventArgs e)
		{
			try
			{
				if (((ObfsBase)ObfsFactory.GetObfs(this.ObfsCombo.Text)).GetObfs()[this.ObfsCombo.Text][2] > 0)
				{
					this.textObfsParam.Enabled = true;
				}
				else
				{
					this.textObfsParam.Enabled = false;
				}
			}
			catch
			{
				this.textObfsParam.Enabled = true;
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002FAC File Offset: 0x000011AC
		private void OKButton_Click(object sender, EventArgs e)
		{
			if (this.SaveOldSelectedServer() == -1)
			{
				return;
			}
			if (this._modifiedConfiguration.configs.Count == 0)
			{
				MessageBox.Show(I18N.GetString("Please add at least one server"));
				return;
			}
			if (this._oldSelectedID != null)
			{
				for (int i = 0; i < this._modifiedConfiguration.configs.Count; i++)
				{
					if (this._modifiedConfiguration.configs[i].id == this._oldSelectedID)
					{
						this._modifiedConfiguration.index = i;
						break;
					}
				}
			}
			this.controller.SaveServersConfig(this._modifiedConfiguration);
			base.Close();
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000031F6 File Offset: 0x000013F6
		private void PasswordLabel_CheckedChanged(object sender, EventArgs e)
		{
			if (this.PasswordLabel.Checked)
			{
				this.PasswordTextBox.UseSystemPasswordChar = false;
				return;
			}
			this.PasswordTextBox.UseSystemPasswordChar = true;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002604 File Offset: 0x00000804
		private int SaveOldSelectedServer()
		{
			try
			{
				int result;
				if (this._oldSelectedIndex == -1 || this._oldSelectedIndex >= this._modifiedConfiguration.configs.Count)
				{
					result = 0;
					return result;
				}
				Server server = new Server
				{
					server = this.IPTextBox.Text.Trim(),
					server_port = int.Parse(this.ServerPortTextBox.Text),
					server_udp_port = int.Parse(this.textUDPPort.Text),
					password = this.PasswordTextBox.Text,
					method = this.EncryptionSelect.Text,
					obfs = this.ObfsCombo.Text,
					obfsparam = this.textObfsParam.Text,
					remarks = this.RemarksTextBox.Text,
					group = this.TextGroup.Text.Trim(),
					udp_over_tcp = this.CheckUDPoverUDP.Checked,
					protocol = this.TCPProtocolComboBox.Text,
					id = this._SelectedID
				};
				Configuration.CheckServer(server);
				int num = 0;
				if (this._modifiedConfiguration.configs[this._oldSelectedIndex].server != server.server || this._modifiedConfiguration.configs[this._oldSelectedIndex].server_port != server.server_port || this._modifiedConfiguration.configs[this._oldSelectedIndex].remarks_base64 != server.remarks_base64 || this._modifiedConfiguration.configs[this._oldSelectedIndex].group != server.group)
				{
					num = 1;
				}
				Server server2 = this._modifiedConfiguration.configs[this._oldSelectedIndex];
				if (server2.server == server.server && server2.server_port == server.server_port && server2.password == server.password && server2.method == server.method && server2.obfs == server.obfs && server2.obfsparam == server.obfsparam)
				{
					server.setObfsData(server2.getObfsData());
				}
				this._modifiedConfiguration.configs[this._oldSelectedIndex] = server;
				result = num;
				return result;
			}
			catch (FormatException)
			{
				MessageBox.Show(I18N.GetString("Illegal port number format"));
			}
			catch (Exception arg_26C_0)
			{
				MessageBox.Show(arg_26C_0.Message);
			}
			return -1;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002E1C File Offset: 0x0000101C
		private void ServersListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this._oldSelectedIndex == this.ServersListBox.SelectedIndex || this.ServersListBox.SelectedIndex == -1)
			{
				return;
			}
			int num = this.SaveOldSelectedServer();
			if (num == -1)
			{
				this.ServersListBox.SelectedIndex = this._oldSelectedIndex;
				return;
			}
			if (num == 1)
			{
				this.LoadConfiguration(this._modifiedConfiguration);
			}
			this.LoadSelectedServer();
			this._oldSelectedIndex = this.ServersListBox.SelectedIndex;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002D80 File Offset: 0x00000F80
		private void SetServerListSelectedIndex(int index)
		{
			int oldSelectedIndex = this._oldSelectedIndex;
			int num = Math.Min(index + 5, this.ServersListBox.Items.Count - 1);
			if (num != index)
			{
				this._oldSelectedIndex = num;
				this.ServersListBox.SelectedIndex = num;
				this._oldSelectedIndex = oldSelectedIndex;
				this.ServersListBox.SelectedIndex = index;
				return;
			}
			this.ServersListBox.SelectedIndex = index;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000025E1 File Offset: 0x000007E1
		private void ShowWindow()
		{
			base.Opacity = 1.0;
			base.Show();
			this.IPTextBox.Focus();
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000031AF File Offset: 0x000013AF
		private void TextBox_Enter(object sender, EventArgs e)
		{
			this.SaveOldSelectedServer();
			this.LoadSelectedServer();
			((TextBox)sender).SelectAll();
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000031C9 File Offset: 0x000013C9
		private void TextBox_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				((TextBox)sender).SelectAll();
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00003080 File Offset: 0x00001280
		private void UpButton_Click(object sender, EventArgs e)
		{
			this._oldSelectedIndex = this.ServersListBox.SelectedIndex;
			int oldSelectedIndex = this._oldSelectedIndex;
			this.SaveOldSelectedServer();
			if (oldSelectedIndex > 0 && oldSelectedIndex < this._modifiedConfiguration.configs.Count)
			{
				this._modifiedConfiguration.configs.Reverse(oldSelectedIndex - 1, 2);
				this._oldSelectedIndex = oldSelectedIndex - 1;
				this.ServersListBox.SelectedIndex = oldSelectedIndex - 1;
				this.LoadConfiguration(this._modifiedConfiguration);
				this.ServersListBox.SelectedIndex = this._oldSelectedIndex;
				this.LoadSelectedServer();
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002264 File Offset: 0x00000464
		private void UpdateTexts()
		{
			this.Text = string.Concat(new string[]
			{
				I18N.GetString("Edit Servers"),
				"(",
				this.controller.GetCurrentConfiguration().shareOverLan ? "any" : "local",
				":",
				this.controller.GetCurrentConfiguration().localPort.ToString(),
				I18N.GetString(" Version"),
				"3.8.4.2 blue)"
			});
			this.AddButton.Text = I18N.GetString("&Add");
			this.DeleteButton.Text = I18N.GetString("&Delete");
			this.UpButton.Text = I18N.GetString("Up");
			this.DownButton.Text = I18N.GetString("Down");
			this.IPLabel.Text = I18N.GetString("Server IP");
			this.ServerPortLabel.Text = I18N.GetString("Server Port");
			this.labelUDPPort.Text = I18N.GetString("UDP Port");
			this.PasswordLabel.Text = I18N.GetString("Password");
			this.EncryptionLabel.Text = I18N.GetString("Encryption");
			this.labelRemarks.Text = I18N.GetString("Remarks");
			this.checkAdvSetting.Text = I18N.GetString(this.checkAdvSetting.Text);
			this.TCPoverUDPLabel.Text = I18N.GetString(this.TCPoverUDPLabel.Text);
			this.UDPoverTCPLabel.Text = I18N.GetString(this.UDPoverTCPLabel.Text);
			this.TCPProtocolLabel.Text = I18N.GetString(this.TCPProtocolLabel.Text);
			this.labelObfs.Text = I18N.GetString(this.labelObfs.Text);
			this.labelObfsParam.Text = I18N.GetString(this.labelObfsParam.Text);
			this.ObfsUDPLabel.Text = I18N.GetString(this.ObfsUDPLabel.Text);
			this.LabelNote.Text = I18N.GetString(this.LabelNote.Text);
			this.CheckTCPoverUDP.Text = I18N.GetString(this.CheckTCPoverUDP.Text);
			this.CheckUDPoverUDP.Text = I18N.GetString(this.CheckUDPoverUDP.Text);
			this.CheckObfsUDP.Text = I18N.GetString(this.CheckObfsUDP.Text);
			this.checkSSRLink.Text = I18N.GetString(this.checkSSRLink.Text);
			for (int i = 0; i < this.TCPProtocolComboBox.Items.Count; i++)
			{
				this.TCPProtocolComboBox.Items[i] = I18N.GetString(this.TCPProtocolComboBox.Items[i].ToString());
			}
			this.ServerGroupBox.Text = I18N.GetString("Server");
			this.OKButton.Text = I18N.GetString("OK");
			this.MyCancelButton.Text = I18N.GetString("Cancel");
			this.LinkUpdate.MaximumSize = new Size(this.ServersListBox.Width, this.ServersListBox.Height);
			this.LinkUpdate.Text = string.Format(I18N.GetString("New version {0} {1} available"), "ShadowsocksR", this.updateChecker.LatestVersionNumber);
		}

		// Token: 0x04000001 RID: 1
		private ShadowsocksController controller;

		// Token: 0x04000002 RID: 2
		private UpdateChecker updateChecker;

		// Token: 0x04000003 RID: 3
		private Configuration _modifiedConfiguration;

		// Token: 0x04000005 RID: 5
		private string _oldSelectedID;

		// Token: 0x04000004 RID: 4
		private int _oldSelectedIndex = -1;

		// Token: 0x04000006 RID: 6
		private string _SelectedID;
	}
}
