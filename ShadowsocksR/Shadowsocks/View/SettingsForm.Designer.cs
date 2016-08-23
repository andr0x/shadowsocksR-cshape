namespace Shadowsocks.View
{
	// Token: 0x0200000A RID: 10
	public partial class SettingsForm : global::System.Windows.Forms.Form
	{
		// Token: 0x06000098 RID: 152 RVA: 0x0000AF55 File Offset: 0x00009155
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x0000AF74 File Offset: 0x00009174
		private void InitializeComponent()
		{
			this.tableLayoutPanel1 = new global::System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel2 = new global::System.Windows.Forms.TableLayoutPanel();
			this.LabelRandom = new global::System.Windows.Forms.Label();
			this.RandomComboBox = new global::System.Windows.Forms.ComboBox();
			this.CheckAutoBan = new global::System.Windows.Forms.CheckBox();
			this.checkRandom = new global::System.Windows.Forms.CheckBox();
			this.checkAutoStartup = new global::System.Windows.Forms.CheckBox();
			this.Socks5ProxyGroup = new global::System.Windows.Forms.GroupBox();
			this.tableLayoutPanel9 = new global::System.Windows.Forms.TableLayoutPanel();
			this.LabelS5Password = new global::System.Windows.Forms.Label();
			this.LabelS5Username = new global::System.Windows.Forms.Label();
			this.TextS5Pass = new global::System.Windows.Forms.TextBox();
			this.LabelS5Port = new global::System.Windows.Forms.Label();
			this.TextS5User = new global::System.Windows.Forms.TextBox();
			this.LabelS5Server = new global::System.Windows.Forms.Label();
			this.TextS5Port = new global::System.Windows.Forms.TextBox();
			this.TextS5Server = new global::System.Windows.Forms.TextBox();
			this.comboProxyType = new global::System.Windows.Forms.ComboBox();
			this.CheckSockProxy = new global::System.Windows.Forms.CheckBox();
			this.checkBoxPacProxy = new global::System.Windows.Forms.CheckBox();
			this.label1 = new global::System.Windows.Forms.Label();
			this.TextUserAgent = new global::System.Windows.Forms.TextBox();
			this.ListenGroup = new global::System.Windows.Forms.GroupBox();
			this.tableLayoutPanel4 = new global::System.Windows.Forms.TableLayoutPanel();
			this.TextAuthPass = new global::System.Windows.Forms.TextBox();
			this.LabelAuthPass = new global::System.Windows.Forms.Label();
			this.TextAuthUser = new global::System.Windows.Forms.TextBox();
			this.LabelAuthUser = new global::System.Windows.Forms.Label();
			this.checkShareOverLan = new global::System.Windows.Forms.CheckBox();
			this.ProxyPortTextBox = new global::System.Windows.Forms.TextBox();
			this.ProxyPortLabel = new global::System.Windows.Forms.Label();
			this.tableLayoutPanel10 = new global::System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel3 = new global::System.Windows.Forms.TableLayoutPanel();
			this.MyCancelButton = new global::System.Windows.Forms.Button();
			this.OKButton = new global::System.Windows.Forms.Button();
			this.tableLayoutPanel5 = new global::System.Windows.Forms.TableLayoutPanel();
			this.ReconnectLabel = new global::System.Windows.Forms.Label();
			this.ReconnectText = new global::System.Windows.Forms.TextBox();
			this.TTLLabel = new global::System.Windows.Forms.Label();
			this.TTLText = new global::System.Windows.Forms.TextBox();
			this.label2 = new global::System.Windows.Forms.Label();
			this.DNSText = new global::System.Windows.Forms.TextBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.Socks5ProxyGroup.SuspendLayout();
			this.tableLayoutPanel9.SuspendLayout();
			this.ListenGroup.SuspendLayout();
			this.tableLayoutPanel4.SuspendLayout();
			this.tableLayoutPanel10.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.tableLayoutPanel5.SuspendLayout();
			base.SuspendLayout();
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.Socks5ProxyGroup, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.ListenGroup, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel10, 1, 2);
			this.tableLayoutPanel1.Location = new global::System.Drawing.Point(15, 16);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new global::System.Drawing.Size(570, 409);
			this.tableLayoutPanel1.TabIndex = 0;
			this.tableLayoutPanel2.AutoSize = true;
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Percent, 100f));
			this.tableLayoutPanel2.Controls.Add(this.LabelRandom, 0, 2);
			this.tableLayoutPanel2.Controls.Add(this.RandomComboBox, 1, 2);
			this.tableLayoutPanel2.Controls.Add(this.CheckAutoBan, 1, 3);
			this.tableLayoutPanel2.Controls.Add(this.checkRandom, 1, 1);
			this.tableLayoutPanel2.Controls.Add(this.checkAutoStartup, 1, 0);
			this.tableLayoutPanel2.Location = new global::System.Drawing.Point(356, 3);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 4;
			this.tableLayoutPanel2.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new global::System.Drawing.Size(211, 104);
			this.tableLayoutPanel2.TabIndex = 21;
			this.LabelRandom.AutoSize = true;
			this.LabelRandom.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.LabelRandom.Location = new global::System.Drawing.Point(3, 50);
			this.LabelRandom.Name = "LabelRandom";
			this.LabelRandom.Size = new global::System.Drawing.Size(52, 29);
			this.LabelRandom.TabIndex = 12;
			this.LabelRandom.Text = "Balance";
			this.LabelRandom.TextAlign = global::System.Drawing.ContentAlignment.MiddleRight;
			this.RandomComboBox.Anchor = global::System.Windows.Forms.AnchorStyles.None;
			this.RandomComboBox.DropDownStyle = global::System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.RandomComboBox.FormattingEnabled = true;
			this.RandomComboBox.Items.AddRange(new object[]
			{
				"Order",
				"Random",
				"LowLatency",
				"LowException",
				"SelectedFirst",
				"Timer",
				"SeletedGroup"
			});
			this.RandomComboBox.Location = new global::System.Drawing.Point(61, 53);
			this.RandomComboBox.Name = "RandomComboBox";
			this.RandomComboBox.Size = new global::System.Drawing.Size(147, 23);
			this.RandomComboBox.TabIndex = 13;
			this.CheckAutoBan.AutoSize = true;
			this.CheckAutoBan.Location = new global::System.Drawing.Point(61, 82);
			this.CheckAutoBan.Name = "CheckAutoBan";
			this.CheckAutoBan.Size = new global::System.Drawing.Size(72, 19);
			this.CheckAutoBan.TabIndex = 18;
			this.CheckAutoBan.Text = "AutoBan";
			this.CheckAutoBan.UseVisualStyleBackColor = true;
			this.checkRandom.AutoSize = true;
			this.checkRandom.Location = new global::System.Drawing.Point(61, 28);
			this.checkRandom.Name = "checkRandom";
			this.checkRandom.Size = new global::System.Drawing.Size(112, 19);
			this.checkRandom.TabIndex = 19;
			this.checkRandom.Text = "Enable balance";
			this.checkRandom.UseVisualStyleBackColor = true;
			this.checkAutoStartup.AutoSize = true;
			this.checkAutoStartup.Location = new global::System.Drawing.Point(61, 3);
			this.checkAutoStartup.Name = "checkAutoStartup";
			this.checkAutoStartup.Size = new global::System.Drawing.Size(96, 19);
			this.checkAutoStartup.TabIndex = 20;
			this.checkAutoStartup.Text = "Start on Boot";
			this.checkAutoStartup.UseVisualStyleBackColor = true;
			this.Socks5ProxyGroup.AutoSize = true;
			this.Socks5ProxyGroup.Controls.Add(this.tableLayoutPanel9);
			this.Socks5ProxyGroup.Location = new global::System.Drawing.Point(14, 0);
			this.Socks5ProxyGroup.Margin = new global::System.Windows.Forms.Padding(14, 0, 0, 0);
			this.Socks5ProxyGroup.Name = "Socks5ProxyGroup";
			this.tableLayoutPanel1.SetRowSpan(this.Socks5ProxyGroup, 2);
			this.Socks5ProxyGroup.Size = new global::System.Drawing.Size(339, 241);
			this.Socks5ProxyGroup.TabIndex = 19;
			this.Socks5ProxyGroup.TabStop = false;
			this.Socks5ProxyGroup.Text = "Remote proxy";
			this.tableLayoutPanel9.AutoSize = true;
			this.tableLayoutPanel9.ColumnCount = 2;
			this.tableLayoutPanel9.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel9.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel9.Controls.Add(this.LabelS5Password, 0, 5);
			this.tableLayoutPanel9.Controls.Add(this.LabelS5Username, 0, 4);
			this.tableLayoutPanel9.Controls.Add(this.TextS5Pass, 1, 5);
			this.tableLayoutPanel9.Controls.Add(this.LabelS5Port, 0, 3);
			this.tableLayoutPanel9.Controls.Add(this.TextS5User, 1, 4);
			this.tableLayoutPanel9.Controls.Add(this.LabelS5Server, 0, 2);
			this.tableLayoutPanel9.Controls.Add(this.TextS5Port, 1, 3);
			this.tableLayoutPanel9.Controls.Add(this.TextS5Server, 1, 2);
			this.tableLayoutPanel9.Controls.Add(this.comboProxyType, 1, 1);
			this.tableLayoutPanel9.Controls.Add(this.CheckSockProxy, 0, 0);
			this.tableLayoutPanel9.Controls.Add(this.checkBoxPacProxy, 1, 0);
			this.tableLayoutPanel9.Controls.Add(this.label1, 0, 6);
			this.tableLayoutPanel9.Controls.Add(this.TextUserAgent, 1, 6);
			this.tableLayoutPanel9.Location = new global::System.Drawing.Point(11, 32);
			this.tableLayoutPanel9.Name = "tableLayoutPanel9";
			this.tableLayoutPanel9.RowCount = 7;
			this.tableLayoutPanel9.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel9.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel9.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel9.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel9.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel9.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel9.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel9.Size = new global::System.Drawing.Size(322, 189);
			this.tableLayoutPanel9.TabIndex = 0;
			this.LabelS5Password.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.LabelS5Password.AutoSize = true;
			this.LabelS5Password.Location = new global::System.Drawing.Point(14, 141);
			this.LabelS5Password.Name = "LabelS5Password";
			this.LabelS5Password.Size = new global::System.Drawing.Size(63, 15);
			this.LabelS5Password.TabIndex = 5;
			this.LabelS5Password.Text = "Password";
			this.LabelS5Username.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.LabelS5Username.AutoSize = true;
			this.LabelS5Username.Location = new global::System.Drawing.Point(11, 114);
			this.LabelS5Username.Name = "LabelS5Username";
			this.LabelS5Username.Size = new global::System.Drawing.Size(66, 15);
			this.LabelS5Username.TabIndex = 4;
			this.LabelS5Username.Text = "Username";
			this.TextS5Pass.ImeMode = global::System.Windows.Forms.ImeMode.Off;
			this.TextS5Pass.Location = new global::System.Drawing.Point(83, 138);
			this.TextS5Pass.Name = "TextS5Pass";
			this.TextS5Pass.Size = new global::System.Drawing.Size(236, 21);
			this.TextS5Pass.TabIndex = 7;
			this.LabelS5Port.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.LabelS5Port.AutoSize = true;
			this.LabelS5Port.Location = new global::System.Drawing.Point(48, 87);
			this.LabelS5Port.Name = "LabelS5Port";
			this.LabelS5Port.Size = new global::System.Drawing.Size(29, 15);
			this.LabelS5Port.TabIndex = 1;
			this.LabelS5Port.Text = "Port";
			this.TextS5User.ImeMode = global::System.Windows.Forms.ImeMode.Off;
			this.TextS5User.Location = new global::System.Drawing.Point(83, 111);
			this.TextS5User.Name = "TextS5User";
			this.TextS5User.Size = new global::System.Drawing.Size(236, 21);
			this.TextS5User.TabIndex = 6;
			this.LabelS5Server.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.LabelS5Server.AutoSize = true;
			this.LabelS5Server.Location = new global::System.Drawing.Point(21, 60);
			this.LabelS5Server.Name = "LabelS5Server";
			this.LabelS5Server.Size = new global::System.Drawing.Size(56, 15);
			this.LabelS5Server.TabIndex = 0;
			this.LabelS5Server.Text = "Server IP";
			this.TextS5Port.ImeMode = global::System.Windows.Forms.ImeMode.Off;
			this.TextS5Port.Location = new global::System.Drawing.Point(83, 84);
			this.TextS5Port.Name = "TextS5Port";
			this.TextS5Port.Size = new global::System.Drawing.Size(236, 21);
			this.TextS5Port.TabIndex = 3;
			this.TextS5Server.ImeMode = global::System.Windows.Forms.ImeMode.Off;
			this.TextS5Server.Location = new global::System.Drawing.Point(83, 57);
			this.TextS5Server.Name = "TextS5Server";
			this.TextS5Server.Size = new global::System.Drawing.Size(236, 21);
			this.TextS5Server.TabIndex = 2;
			this.comboProxyType.DropDownStyle = global::System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProxyType.FormattingEnabled = true;
			this.comboProxyType.Items.AddRange(new object[]
			{
				"Socks5(support UDP)",
				"Http tunnel",
				"TCP Port tunnel"
			});
			this.comboProxyType.Location = new global::System.Drawing.Point(83, 28);
			this.comboProxyType.Name = "comboProxyType";
			this.comboProxyType.Size = new global::System.Drawing.Size(236, 23);
			this.comboProxyType.TabIndex = 9;
			this.CheckSockProxy.AutoSize = true;
			this.CheckSockProxy.Location = new global::System.Drawing.Point(3, 3);
			this.CheckSockProxy.Name = "CheckSockProxy";
			this.CheckSockProxy.Size = new global::System.Drawing.Size(74, 19);
			this.CheckSockProxy.TabIndex = 8;
			this.CheckSockProxy.Text = "Proxy On";
			this.CheckSockProxy.UseVisualStyleBackColor = true;
			this.checkBoxPacProxy.AutoSize = true;
			this.checkBoxPacProxy.Location = new global::System.Drawing.Point(83, 3);
			this.checkBoxPacProxy.Name = "checkBoxPacProxy";
			this.checkBoxPacProxy.Size = new global::System.Drawing.Size(179, 19);
			this.checkBoxPacProxy.TabIndex = 10;
			this.checkBoxPacProxy.Text = "PAC \"direct\" return this proxy";
			this.checkBoxPacProxy.UseVisualStyleBackColor = true;
			this.label1.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.label1.AutoSize = true;
			this.label1.Location = new global::System.Drawing.Point(12, 168);
			this.label1.Name = "label1";
			this.label1.Size = new global::System.Drawing.Size(65, 15);
			this.label1.TabIndex = 5;
			this.label1.Text = "UserAgent";
			this.TextUserAgent.ImeMode = global::System.Windows.Forms.ImeMode.Off;
			this.TextUserAgent.Location = new global::System.Drawing.Point(83, 165);
			this.TextUserAgent.Name = "TextUserAgent";
			this.TextUserAgent.Size = new global::System.Drawing.Size(236, 21);
			this.TextUserAgent.TabIndex = 7;
			this.ListenGroup.AutoSize = true;
			this.ListenGroup.Controls.Add(this.tableLayoutPanel4);
			this.ListenGroup.Location = new global::System.Drawing.Point(14, 241);
			this.ListenGroup.Margin = new global::System.Windows.Forms.Padding(14, 0, 0, 0);
			this.ListenGroup.Name = "ListenGroup";
			this.ListenGroup.Size = new global::System.Drawing.Size(325, 168);
			this.ListenGroup.TabIndex = 22;
			this.ListenGroup.TabStop = false;
			this.ListenGroup.Text = "Local proxy";
			this.tableLayoutPanel4.AutoSize = true;
			this.tableLayoutPanel4.ColumnCount = 2;
			this.tableLayoutPanel4.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel4.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel4.Controls.Add(this.TextAuthPass, 1, 3);
			this.tableLayoutPanel4.Controls.Add(this.LabelAuthPass, 0, 3);
			this.tableLayoutPanel4.Controls.Add(this.TextAuthUser, 1, 2);
			this.tableLayoutPanel4.Controls.Add(this.LabelAuthUser, 0, 2);
			this.tableLayoutPanel4.Controls.Add(this.checkShareOverLan, 1, 0);
			this.tableLayoutPanel4.Controls.Add(this.ProxyPortTextBox, 1, 1);
			this.tableLayoutPanel4.Controls.Add(this.ProxyPortLabel, 0, 1);
			this.tableLayoutPanel4.Location = new global::System.Drawing.Point(5, 32);
			this.tableLayoutPanel4.Name = "tableLayoutPanel4";
			this.tableLayoutPanel4.RowCount = 4;
			this.tableLayoutPanel4.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.Size = new global::System.Drawing.Size(314, 116);
			this.tableLayoutPanel4.TabIndex = 0;
			this.TextAuthPass.ImeMode = global::System.Windows.Forms.ImeMode.Off;
			this.TextAuthPass.Location = new global::System.Drawing.Point(75, 82);
			this.TextAuthPass.Name = "TextAuthPass";
			this.TextAuthPass.Size = new global::System.Drawing.Size(236, 21);
			this.TextAuthPass.TabIndex = 9;
			this.LabelAuthPass.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.LabelAuthPass.AutoSize = true;
			this.LabelAuthPass.Location = new global::System.Drawing.Point(6, 90);
			this.LabelAuthPass.Name = "LabelAuthPass";
			this.LabelAuthPass.Size = new global::System.Drawing.Size(63, 15);
			this.LabelAuthPass.TabIndex = 8;
			this.LabelAuthPass.Text = "Password";
			this.TextAuthUser.ImeMode = global::System.Windows.Forms.ImeMode.Off;
			this.TextAuthUser.Location = new global::System.Drawing.Point(75, 55);
			this.TextAuthUser.Name = "TextAuthUser";
			this.TextAuthUser.Size = new global::System.Drawing.Size(236, 21);
			this.TextAuthUser.TabIndex = 7;
			this.LabelAuthUser.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.LabelAuthUser.AutoSize = true;
			this.LabelAuthUser.Location = new global::System.Drawing.Point(3, 58);
			this.LabelAuthUser.Name = "LabelAuthUser";
			this.LabelAuthUser.Size = new global::System.Drawing.Size(66, 15);
			this.LabelAuthUser.TabIndex = 5;
			this.LabelAuthUser.Text = "Username";
			this.checkShareOverLan.AutoSize = true;
			this.checkShareOverLan.Location = new global::System.Drawing.Point(75, 3);
			this.checkShareOverLan.Name = "checkShareOverLan";
			this.checkShareOverLan.Size = new global::System.Drawing.Size(151, 19);
			this.checkShareOverLan.TabIndex = 5;
			this.checkShareOverLan.Text = "Allow Clients from LAN";
			this.checkShareOverLan.UseVisualStyleBackColor = true;
			this.ProxyPortTextBox.ImeMode = global::System.Windows.Forms.ImeMode.Off;
			this.ProxyPortTextBox.Location = new global::System.Drawing.Point(75, 28);
			this.ProxyPortTextBox.MaxLength = 10;
			this.ProxyPortTextBox.Name = "ProxyPortTextBox";
			this.ProxyPortTextBox.Size = new global::System.Drawing.Size(236, 21);
			this.ProxyPortTextBox.TabIndex = 4;
			this.ProxyPortTextBox.WordWrap = false;
			this.ProxyPortLabel.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.ProxyPortLabel.AutoSize = true;
			this.ProxyPortLabel.Location = new global::System.Drawing.Point(40, 31);
			this.ProxyPortLabel.Name = "ProxyPortLabel";
			this.ProxyPortLabel.Size = new global::System.Drawing.Size(29, 15);
			this.ProxyPortLabel.TabIndex = 3;
			this.ProxyPortLabel.Text = "Port";
			this.tableLayoutPanel10.Anchor = global::System.Windows.Forms.AnchorStyles.None;
			this.tableLayoutPanel10.AutoSize = true;
			this.tableLayoutPanel10.ColumnCount = 1;
			this.tableLayoutPanel10.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Percent, 100f));
			this.tableLayoutPanel10.Controls.Add(this.tableLayoutPanel3, 0, 2);
			this.tableLayoutPanel10.Controls.Add(this.tableLayoutPanel5, 0, 1);
			this.tableLayoutPanel10.Location = new global::System.Drawing.Point(365, 257);
			this.tableLayoutPanel10.Name = "tableLayoutPanel10";
			this.tableLayoutPanel10.RowCount = 3;
			this.tableLayoutPanel10.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel10.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel10.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel10.Size = new global::System.Drawing.Size(193, 135);
			this.tableLayoutPanel10.TabIndex = 20;
			this.tableLayoutPanel3.Anchor = global::System.Windows.Forms.AnchorStyles.Bottom;
			this.tableLayoutPanel3.AutoSize = true;
			this.tableLayoutPanel3.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.ColumnCount = 2;
			this.tableLayoutPanel3.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Absolute, 23f));
			this.tableLayoutPanel3.Controls.Add(this.MyCancelButton, 1, 0);
			this.tableLayoutPanel3.Controls.Add(this.OKButton, 0, 0);
			this.tableLayoutPanel3.Location = new global::System.Drawing.Point(6, 90);
			this.tableLayoutPanel3.Margin = new global::System.Windows.Forms.Padding(3, 3, 0, 3);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 1;
			this.tableLayoutPanel3.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.Size = new global::System.Drawing.Size(183, 42);
			this.tableLayoutPanel3.TabIndex = 14;
			this.MyCancelButton.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.MyCancelButton.Dock = global::System.Windows.Forms.DockStyle.Right;
			this.MyCancelButton.Location = new global::System.Drawing.Point(96, 3);
			this.MyCancelButton.Margin = new global::System.Windows.Forms.Padding(3, 3, 0, 0);
			this.MyCancelButton.Name = "MyCancelButton";
			this.MyCancelButton.Size = new global::System.Drawing.Size(87, 39);
			this.MyCancelButton.TabIndex = 9;
			this.MyCancelButton.Text = "Cancel";
			this.MyCancelButton.UseVisualStyleBackColor = true;
			this.MyCancelButton.Click += new global::System.EventHandler(this.CancelButton_Click);
			this.OKButton.DialogResult = global::System.Windows.Forms.DialogResult.OK;
			this.OKButton.Dock = global::System.Windows.Forms.DockStyle.Right;
			this.OKButton.Location = new global::System.Drawing.Point(3, 3);
			this.OKButton.Margin = new global::System.Windows.Forms.Padding(3, 3, 3, 0);
			this.OKButton.Name = "OKButton";
			this.OKButton.Size = new global::System.Drawing.Size(87, 39);
			this.OKButton.TabIndex = 8;
			this.OKButton.Text = "OK";
			this.OKButton.UseVisualStyleBackColor = true;
			this.OKButton.Click += new global::System.EventHandler(this.OKButton_Click);
			this.tableLayoutPanel5.Anchor = global::System.Windows.Forms.AnchorStyles.Bottom;
			this.tableLayoutPanel5.AutoSize = true;
			this.tableLayoutPanel5.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel5.ColumnCount = 2;
			this.tableLayoutPanel5.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel5.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel5.Controls.Add(this.ReconnectLabel, 0, 3);
			this.tableLayoutPanel5.Controls.Add(this.ReconnectText, 1, 3);
			this.tableLayoutPanel5.Controls.Add(this.TTLLabel, 0, 4);
			this.tableLayoutPanel5.Controls.Add(this.TTLText, 1, 4);
			this.tableLayoutPanel5.Controls.Add(this.label2, 0, 0);
			this.tableLayoutPanel5.Controls.Add(this.DNSText, 1, 0);
			this.tableLayoutPanel5.Location = new global::System.Drawing.Point(0, 0);
			this.tableLayoutPanel5.Margin = new global::System.Windows.Forms.Padding(0);
			this.tableLayoutPanel5.Name = "tableLayoutPanel5";
			this.tableLayoutPanel5.Padding = new global::System.Windows.Forms.Padding(3);
			this.tableLayoutPanel5.RowCount = 5;
			this.tableLayoutPanel5.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel5.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel5.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel5.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel5.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel5.Size = new global::System.Drawing.Size(193, 87);
			this.tableLayoutPanel5.TabIndex = 9;
			this.ReconnectLabel.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.ReconnectLabel.AutoSize = true;
			this.ReconnectLabel.Location = new global::System.Drawing.Point(6, 36);
			this.ReconnectLabel.Name = "ReconnectLabel";
			this.ReconnectLabel.Size = new global::System.Drawing.Size(66, 15);
			this.ReconnectLabel.TabIndex = 3;
			this.ReconnectLabel.Text = "Reconnect";
			this.ReconnectText.ImeMode = global::System.Windows.Forms.ImeMode.Off;
			this.ReconnectText.Location = new global::System.Drawing.Point(78, 33);
			this.ReconnectText.MaxLength = 10;
			this.ReconnectText.Name = "ReconnectText";
			this.ReconnectText.Size = new global::System.Drawing.Size(109, 21);
			this.ReconnectText.TabIndex = 4;
			this.ReconnectText.WordWrap = false;
			this.TTLLabel.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.TTLLabel.AutoSize = true;
			this.TTLLabel.Location = new global::System.Drawing.Point(44, 63);
			this.TTLLabel.Name = "TTLLabel";
			this.TTLLabel.Size = new global::System.Drawing.Size(28, 15);
			this.TTLLabel.TabIndex = 3;
			this.TTLLabel.Text = "TTL";
			this.TTLText.ImeMode = global::System.Windows.Forms.ImeMode.Off;
			this.TTLText.Location = new global::System.Drawing.Point(78, 60);
			this.TTLText.MaxLength = 10;
			this.TTLText.Name = "TTLText";
			this.TTLText.Size = new global::System.Drawing.Size(109, 21);
			this.TTLText.TabIndex = 4;
			this.TTLText.WordWrap = false;
			this.label2.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.label2.AutoSize = true;
			this.label2.Location = new global::System.Drawing.Point(39, 9);
			this.label2.Name = "label2";
			this.label2.Size = new global::System.Drawing.Size(33, 15);
			this.label2.TabIndex = 3;
			this.label2.Text = "DNS";
			this.DNSText.ImeMode = global::System.Windows.Forms.ImeMode.Off;
			this.DNSText.Location = new global::System.Drawing.Point(78, 6);
			this.DNSText.MaxLength = 0;
			this.DNSText.Name = "DNSText";
			this.DNSText.Size = new global::System.Drawing.Size(109, 21);
			this.DNSText.TabIndex = 4;
			this.DNSText.WordWrap = false;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(96f, 96f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Dpi;
			this.AutoSize = true;
			base.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			base.ClientSize = new global::System.Drawing.Size(728, 573);
			base.Controls.Add(this.tableLayoutPanel1);
			this.Font = new global::System.Drawing.Font("Arial", 9f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 0);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "SettingsForm";
			base.Padding = new global::System.Windows.Forms.Padding(12, 13, 12, 13);
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SettingsForm";
			base.FormClosed += new global::System.Windows.Forms.FormClosedEventHandler(this.SettingsForm_FormClosed);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.Socks5ProxyGroup.ResumeLayout(false);
			this.Socks5ProxyGroup.PerformLayout();
			this.tableLayoutPanel9.ResumeLayout(false);
			this.tableLayoutPanel9.PerformLayout();
			this.ListenGroup.ResumeLayout(false);
			this.ListenGroup.PerformLayout();
			this.tableLayoutPanel4.ResumeLayout(false);
			this.tableLayoutPanel4.PerformLayout();
			this.tableLayoutPanel10.ResumeLayout(false);
			this.tableLayoutPanel10.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel5.ResumeLayout(false);
			this.tableLayoutPanel5.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x040000A8 RID: 168
		private global::System.Windows.Forms.CheckBox CheckAutoBan;

		// Token: 0x040000AA RID: 170
		private global::System.Windows.Forms.CheckBox checkAutoStartup;

		// Token: 0x040000B3 RID: 179
		private global::System.Windows.Forms.CheckBox checkBoxPacProxy;

		// Token: 0x040000A9 RID: 169
		private global::System.Windows.Forms.CheckBox checkRandom;

		// Token: 0x040000AB RID: 171
		private global::System.Windows.Forms.CheckBox checkShareOverLan;

		// Token: 0x04000099 RID: 153
		private global::System.Windows.Forms.CheckBox CheckSockProxy;

		// Token: 0x040000AC RID: 172
		private global::System.Windows.Forms.ComboBox comboProxyType;

		// Token: 0x0400008D RID: 141
		private global::System.ComponentModel.IContainer components;

		// Token: 0x040000B7 RID: 183
		private global::System.Windows.Forms.TextBox DNSText;

		// Token: 0x040000B4 RID: 180
		private global::System.Windows.Forms.Label label1;

		// Token: 0x040000B6 RID: 182
		private global::System.Windows.Forms.Label label2;

		// Token: 0x040000B0 RID: 176
		private global::System.Windows.Forms.Label LabelAuthPass;

		// Token: 0x040000B2 RID: 178
		private global::System.Windows.Forms.Label LabelAuthUser;

		// Token: 0x040000A6 RID: 166
		private global::System.Windows.Forms.Label LabelRandom;

		// Token: 0x04000093 RID: 147
		private global::System.Windows.Forms.Label LabelS5Password;

		// Token: 0x04000095 RID: 149
		private global::System.Windows.Forms.Label LabelS5Port;

		// Token: 0x04000094 RID: 148
		private global::System.Windows.Forms.Label LabelS5Server;

		// Token: 0x04000098 RID: 152
		private global::System.Windows.Forms.Label LabelS5Username;

		// Token: 0x040000AD RID: 173
		private global::System.Windows.Forms.GroupBox ListenGroup;

		// Token: 0x0400009C RID: 156
		private global::System.Windows.Forms.Button MyCancelButton;

		// Token: 0x0400009D RID: 157
		private global::System.Windows.Forms.Button OKButton;

		// Token: 0x040000A0 RID: 160
		private global::System.Windows.Forms.Label ProxyPortLabel;

		// Token: 0x0400009F RID: 159
		private global::System.Windows.Forms.TextBox ProxyPortTextBox;

		// Token: 0x040000A7 RID: 167
		private global::System.Windows.Forms.ComboBox RandomComboBox;

		// Token: 0x040000A1 RID: 161
		private global::System.Windows.Forms.Label ReconnectLabel;

		// Token: 0x040000A2 RID: 162
		private global::System.Windows.Forms.TextBox ReconnectText;

		// Token: 0x0400008F RID: 143
		private global::System.Windows.Forms.GroupBox Socks5ProxyGroup;

		// Token: 0x0400008E RID: 142
		private global::System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

		// Token: 0x0400009A RID: 154
		private global::System.Windows.Forms.TableLayoutPanel tableLayoutPanel10;

		// Token: 0x040000A5 RID: 165
		private global::System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;

		// Token: 0x0400009B RID: 155
		private global::System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;

		// Token: 0x040000AE RID: 174
		private global::System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;

		// Token: 0x0400009E RID: 158
		private global::System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;

		// Token: 0x04000090 RID: 144
		private global::System.Windows.Forms.TableLayoutPanel tableLayoutPanel9;

		// Token: 0x040000AF RID: 175
		private global::System.Windows.Forms.TextBox TextAuthPass;

		// Token: 0x040000B1 RID: 177
		private global::System.Windows.Forms.TextBox TextAuthUser;

		// Token: 0x04000091 RID: 145
		private global::System.Windows.Forms.TextBox TextS5Pass;

		// Token: 0x04000097 RID: 151
		private global::System.Windows.Forms.TextBox TextS5Port;

		// Token: 0x04000096 RID: 150
		private global::System.Windows.Forms.TextBox TextS5Server;

		// Token: 0x04000092 RID: 146
		private global::System.Windows.Forms.TextBox TextS5User;

		// Token: 0x040000B5 RID: 181
		private global::System.Windows.Forms.TextBox TextUserAgent;

		// Token: 0x040000A3 RID: 163
		private global::System.Windows.Forms.Label TTLLabel;

		// Token: 0x040000A4 RID: 164
		private global::System.Windows.Forms.TextBox TTLText;
	}
}
