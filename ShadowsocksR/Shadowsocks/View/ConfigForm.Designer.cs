namespace Shadowsocks.View
{
	// Token: 0x02000003 RID: 3
	public partial class ConfigForm : global::System.Windows.Forms.Form
	{
		// Token: 0x0600001C RID: 28 RVA: 0x00003323 File Offset: 0x00001523
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00003344 File Offset: 0x00001544
		private void InitializeComponent()
		{
			this.tableLayoutPanel1 = new global::System.Windows.Forms.TableLayoutPanel();
			this.ObfsCombo = new global::System.Windows.Forms.ComboBox();
			this.labelObfs = new global::System.Windows.Forms.Label();
			this.IPLabel = new global::System.Windows.Forms.Label();
			this.ServerPortLabel = new global::System.Windows.Forms.Label();
			this.IPTextBox = new global::System.Windows.Forms.TextBox();
			this.ServerPortTextBox = new global::System.Windows.Forms.TextBox();
			this.PasswordTextBox = new global::System.Windows.Forms.TextBox();
			this.EncryptionLabel = new global::System.Windows.Forms.Label();
			this.EncryptionSelect = new global::System.Windows.Forms.ComboBox();
			this.TextLink = new global::System.Windows.Forms.TextBox();
			this.RemarksTextBox = new global::System.Windows.Forms.TextBox();
			this.ObfsUDPLabel = new global::System.Windows.Forms.Label();
			this.CheckObfsUDP = new global::System.Windows.Forms.CheckBox();
			this.TCPProtocolLabel = new global::System.Windows.Forms.Label();
			this.UDPoverTCPLabel = new global::System.Windows.Forms.Label();
			this.CheckUDPoverUDP = new global::System.Windows.Forms.CheckBox();
			this.LabelNote = new global::System.Windows.Forms.Label();
			this.PasswordLabel = new global::System.Windows.Forms.CheckBox();
			this.TCPoverUDPLabel = new global::System.Windows.Forms.Label();
			this.CheckTCPoverUDP = new global::System.Windows.Forms.CheckBox();
			this.TCPProtocolComboBox = new global::System.Windows.Forms.ComboBox();
			this.labelObfsParam = new global::System.Windows.Forms.Label();
			this.textObfsParam = new global::System.Windows.Forms.TextBox();
			this.labelGroup = new global::System.Windows.Forms.Label();
			this.TextGroup = new global::System.Windows.Forms.TextBox();
			this.checkAdvSetting = new global::System.Windows.Forms.CheckBox();
			this.labelUDPPort = new global::System.Windows.Forms.Label();
			this.textUDPPort = new global::System.Windows.Forms.TextBox();
			this.checkSSRLink = new global::System.Windows.Forms.CheckBox();
			this.labelRemarks = new global::System.Windows.Forms.Label();
			this.panel2 = new global::System.Windows.Forms.Panel();
			this.DeleteButton = new global::System.Windows.Forms.Button();
			this.AddButton = new global::System.Windows.Forms.Button();
			this.ServerGroupBox = new global::System.Windows.Forms.GroupBox();
			this.PictureQRcode = new global::System.Windows.Forms.PictureBox();
			this.ServersListBox = new global::System.Windows.Forms.ListBox();
			this.tableLayoutPanel2 = new global::System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel7 = new global::System.Windows.Forms.TableLayoutPanel();
			this.LinkUpdate = new global::System.Windows.Forms.LinkLabel();
			this.tableLayoutPanel4 = new global::System.Windows.Forms.TableLayoutPanel();
			this.DownButton = new global::System.Windows.Forms.Button();
			this.UpButton = new global::System.Windows.Forms.Button();
			this.tableLayoutPanel5 = new global::System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel3 = new global::System.Windows.Forms.TableLayoutPanel();
			this.MyCancelButton = new global::System.Windows.Forms.Button();
			this.OKButton = new global::System.Windows.Forms.Button();
			this.tableLayoutPanel1.SuspendLayout();
			this.ServerGroupBox.SuspendLayout();
			((global::System.ComponentModel.ISupportInitialize)this.PictureQRcode).BeginInit();
			this.tableLayoutPanel2.SuspendLayout();
			this.tableLayoutPanel7.SuspendLayout();
			this.tableLayoutPanel4.SuspendLayout();
			this.tableLayoutPanel5.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			base.SuspendLayout();
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.ObfsCombo, 1, 5);
			this.tableLayoutPanel1.Controls.Add(this.labelObfs, 0, 5);
			this.tableLayoutPanel1.Controls.Add(this.IPLabel, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.ServerPortLabel, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.IPTextBox, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.ServerPortTextBox, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.PasswordTextBox, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.EncryptionLabel, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.EncryptionSelect, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this.TextLink, 1, 9);
			this.tableLayoutPanel1.Controls.Add(this.RemarksTextBox, 1, 7);
			this.tableLayoutPanel1.Controls.Add(this.ObfsUDPLabel, 0, 14);
			this.tableLayoutPanel1.Controls.Add(this.CheckObfsUDP, 1, 14);
			this.tableLayoutPanel1.Controls.Add(this.TCPProtocolLabel, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this.UDPoverTCPLabel, 0, 13);
			this.tableLayoutPanel1.Controls.Add(this.CheckUDPoverUDP, 1, 13);
			this.tableLayoutPanel1.Controls.Add(this.LabelNote, 1, 10);
			this.tableLayoutPanel1.Controls.Add(this.PasswordLabel, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.TCPoverUDPLabel, 0, 12);
			this.tableLayoutPanel1.Controls.Add(this.CheckTCPoverUDP, 1, 12);
			this.tableLayoutPanel1.Controls.Add(this.TCPProtocolComboBox, 1, 4);
			this.tableLayoutPanel1.Controls.Add(this.labelObfsParam, 0, 6);
			this.tableLayoutPanel1.Controls.Add(this.textObfsParam, 1, 6);
			this.tableLayoutPanel1.Controls.Add(this.labelGroup, 0, 8);
			this.tableLayoutPanel1.Controls.Add(this.TextGroup, 1, 8);
			this.tableLayoutPanel1.Controls.Add(this.checkAdvSetting, 0, 10);
			this.tableLayoutPanel1.Controls.Add(this.labelUDPPort, 0, 11);
			this.tableLayoutPanel1.Controls.Add(this.textUDPPort, 1, 11);
			this.tableLayoutPanel1.Controls.Add(this.checkSSRLink, 0, 9);
			this.tableLayoutPanel1.Controls.Add(this.labelRemarks, 0, 7);
			this.tableLayoutPanel1.Location = new global::System.Drawing.Point(8, 32);
			this.tableLayoutPanel1.Margin = new global::System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.Padding = new global::System.Windows.Forms.Padding(3);
			this.tableLayoutPanel1.RowCount = 15;
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new global::System.Drawing.Size(321, 409);
			this.tableLayoutPanel1.TabIndex = 0;
			this.ObfsCombo.Anchor = (global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.ObfsCombo.DropDownStyle = global::System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ObfsCombo.FormattingEnabled = true;
			this.ObfsCombo.Items.AddRange(new object[]
			{
				"plain",
				"http_simple",
				"http_post",
				"random_head",
				"tls1.0_session_auth",
				"tls1.2_ticket_auth"
			});
			this.ObfsCombo.Location = new global::System.Drawing.Point(100, 145);
			this.ObfsCombo.Name = "ObfsCombo";
			this.ObfsCombo.Size = new global::System.Drawing.Size(215, 23);
			this.ObfsCombo.TabIndex = 34;
			this.ObfsCombo.TextChanged += new global::System.EventHandler(this.ObfsCombo_TextChanged);
			this.labelObfs.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.labelObfs.AutoSize = true;
			this.labelObfs.Location = new global::System.Drawing.Point(61, 149);
			this.labelObfs.Name = "labelObfs";
			this.labelObfs.Size = new global::System.Drawing.Size(33, 15);
			this.labelObfs.TabIndex = 33;
			this.labelObfs.Text = "Obfs";
			this.IPLabel.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.IPLabel.AutoSize = true;
			this.IPLabel.Location = new global::System.Drawing.Point(38, 9);
			this.IPLabel.Name = "IPLabel";
			this.IPLabel.Size = new global::System.Drawing.Size(56, 15);
			this.IPLabel.TabIndex = 0;
			this.IPLabel.Text = "Server IP";
			this.ServerPortLabel.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.ServerPortLabel.AutoSize = true;
			this.ServerPortLabel.Location = new global::System.Drawing.Point(27, 36);
			this.ServerPortLabel.Name = "ServerPortLabel";
			this.ServerPortLabel.Size = new global::System.Drawing.Size(67, 15);
			this.ServerPortLabel.TabIndex = 1;
			this.ServerPortLabel.Text = "Server Port";
			this.IPTextBox.Anchor = (global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.IPTextBox.Location = new global::System.Drawing.Point(100, 6);
			this.IPTextBox.MaxLength = 512;
			this.IPTextBox.Name = "IPTextBox";
			this.IPTextBox.Size = new global::System.Drawing.Size(215, 21);
			this.IPTextBox.TabIndex = 0;
			this.IPTextBox.WordWrap = false;
			this.ServerPortTextBox.Anchor = (global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.ServerPortTextBox.Location = new global::System.Drawing.Point(100, 33);
			this.ServerPortTextBox.MaxLength = 10;
			this.ServerPortTextBox.Name = "ServerPortTextBox";
			this.ServerPortTextBox.Size = new global::System.Drawing.Size(215, 21);
			this.ServerPortTextBox.TabIndex = 1;
			this.ServerPortTextBox.WordWrap = false;
			this.PasswordTextBox.Anchor = (global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.PasswordTextBox.Location = new global::System.Drawing.Point(100, 60);
			this.PasswordTextBox.MaxLength = 256;
			this.PasswordTextBox.Name = "PasswordTextBox";
			this.PasswordTextBox.Size = new global::System.Drawing.Size(215, 21);
			this.PasswordTextBox.TabIndex = 2;
			this.PasswordTextBox.UseSystemPasswordChar = true;
			this.PasswordTextBox.WordWrap = false;
			this.EncryptionLabel.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.EncryptionLabel.AutoSize = true;
			this.EncryptionLabel.Location = new global::System.Drawing.Point(30, 91);
			this.EncryptionLabel.Name = "EncryptionLabel";
			this.EncryptionLabel.Size = new global::System.Drawing.Size(64, 15);
			this.EncryptionLabel.TabIndex = 8;
			this.EncryptionLabel.Text = "Encryption";
			this.EncryptionSelect.Anchor = (global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.EncryptionSelect.DropDownStyle = global::System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.EncryptionSelect.FormattingEnabled = true;
			this.EncryptionSelect.ImeMode = global::System.Windows.Forms.ImeMode.NoControl;
			this.EncryptionSelect.ItemHeight = 15;
			this.EncryptionSelect.Location = new global::System.Drawing.Point(100, 87);
			this.EncryptionSelect.Name = "EncryptionSelect";
			this.EncryptionSelect.Size = new global::System.Drawing.Size(215, 23);
			this.EncryptionSelect.TabIndex = 3;
			this.TextLink.Anchor = (global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.TextLink.Location = new global::System.Drawing.Point(100, 255);
			this.TextLink.MaxLength = 32;
			this.TextLink.Name = "TextLink";
			this.TextLink.Size = new global::System.Drawing.Size(215, 21);
			this.TextLink.TabIndex = 12;
			this.TextLink.WordWrap = false;
			this.TextLink.Enter += new global::System.EventHandler(this.TextBox_Enter);
			this.TextLink.MouseUp += new global::System.Windows.Forms.MouseEventHandler(this.TextBox_MouseUp);
			this.RemarksTextBox.Anchor = (global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.RemarksTextBox.Location = new global::System.Drawing.Point(100, 201);
			this.RemarksTextBox.MaxLength = 32;
			this.RemarksTextBox.Name = "RemarksTextBox";
			this.RemarksTextBox.Size = new global::System.Drawing.Size(215, 21);
			this.RemarksTextBox.TabIndex = 10;
			this.RemarksTextBox.WordWrap = false;
			this.ObfsUDPLabel.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.ObfsUDPLabel.AutoSize = true;
			this.ObfsUDPLabel.Location = new global::System.Drawing.Point(32, 386);
			this.ObfsUDPLabel.Margin = new global::System.Windows.Forms.Padding(3, 5, 3, 5);
			this.ObfsUDPLabel.Name = "ObfsUDPLabel";
			this.ObfsUDPLabel.Size = new global::System.Drawing.Size(62, 15);
			this.ObfsUDPLabel.TabIndex = 25;
			this.ObfsUDPLabel.Text = "Obfs UDP";
			this.ObfsUDPLabel.Visible = false;
			this.CheckObfsUDP.AutoSize = true;
			this.CheckObfsUDP.Location = new global::System.Drawing.Point(100, 384);
			this.CheckObfsUDP.Name = "CheckObfsUDP";
			this.CheckObfsUDP.Size = new global::System.Drawing.Size(147, 19);
			this.CheckObfsUDP.TabIndex = 28;
			this.CheckObfsUDP.Text = "Recommend checked";
			this.CheckObfsUDP.UseVisualStyleBackColor = true;
			this.CheckObfsUDP.Visible = false;
			this.TCPProtocolLabel.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.TCPProtocolLabel.AutoSize = true;
			this.TCPProtocolLabel.Location = new global::System.Drawing.Point(42, 120);
			this.TCPProtocolLabel.Margin = new global::System.Windows.Forms.Padding(3, 5, 3, 5);
			this.TCPProtocolLabel.Name = "TCPProtocolLabel";
			this.TCPProtocolLabel.Size = new global::System.Drawing.Size(52, 15);
			this.TCPProtocolLabel.TabIndex = 24;
			this.TCPProtocolLabel.Text = "Protocol";
			this.UDPoverTCPLabel.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.UDPoverTCPLabel.AutoSize = true;
			this.UDPoverTCPLabel.Location = new global::System.Drawing.Point(14, 361);
			this.UDPoverTCPLabel.Margin = new global::System.Windows.Forms.Padding(3, 5, 3, 5);
			this.UDPoverTCPLabel.Name = "UDPoverTCPLabel";
			this.UDPoverTCPLabel.Size = new global::System.Drawing.Size(80, 15);
			this.UDPoverTCPLabel.TabIndex = 23;
			this.UDPoverTCPLabel.Text = "UDPoverTCP";
			this.UDPoverTCPLabel.Visible = false;
			this.CheckUDPoverUDP.AutoSize = true;
			this.CheckUDPoverUDP.Location = new global::System.Drawing.Point(100, 359);
			this.CheckUDPoverUDP.Name = "CheckUDPoverUDP";
			this.CheckUDPoverUDP.Size = new global::System.Drawing.Size(185, 19);
			this.CheckUDPoverUDP.TabIndex = 26;
			this.CheckUDPoverUDP.Text = "UDP over UDP if not checked";
			this.CheckUDPoverUDP.UseVisualStyleBackColor = true;
			this.CheckUDPoverUDP.Visible = false;
			this.LabelNote.Anchor = global::System.Windows.Forms.AnchorStyles.Left;
			this.LabelNote.AutoSize = true;
			this.LabelNote.Location = new global::System.Drawing.Point(102, 284);
			this.LabelNote.Margin = new global::System.Windows.Forms.Padding(5);
			this.LabelNote.Name = "LabelNote";
			this.LabelNote.Size = new global::System.Drawing.Size(173, 15);
			this.LabelNote.TabIndex = 29;
			this.LabelNote.Text = "NOT all server support belows";
			this.PasswordLabel.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.PasswordLabel.AutoSize = true;
			this.PasswordLabel.Location = new global::System.Drawing.Point(12, 61);
			this.PasswordLabel.Name = "PasswordLabel";
			this.PasswordLabel.Size = new global::System.Drawing.Size(82, 19);
			this.PasswordLabel.TabIndex = 31;
			this.PasswordLabel.Text = "Password";
			this.PasswordLabel.UseVisualStyleBackColor = true;
			this.PasswordLabel.CheckedChanged += new global::System.EventHandler(this.PasswordLabel_CheckedChanged);
			this.TCPoverUDPLabel.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.TCPoverUDPLabel.AutoSize = true;
			this.TCPoverUDPLabel.Location = new global::System.Drawing.Point(14, 336);
			this.TCPoverUDPLabel.Margin = new global::System.Windows.Forms.Padding(3, 5, 3, 5);
			this.TCPoverUDPLabel.Name = "TCPoverUDPLabel";
			this.TCPoverUDPLabel.Size = new global::System.Drawing.Size(80, 15);
			this.TCPoverUDPLabel.TabIndex = 23;
			this.TCPoverUDPLabel.Text = "TCPoverUDP";
			this.TCPoverUDPLabel.Visible = false;
			this.CheckTCPoverUDP.AutoSize = true;
			this.CheckTCPoverUDP.Location = new global::System.Drawing.Point(100, 334);
			this.CheckTCPoverUDP.Name = "CheckTCPoverUDP";
			this.CheckTCPoverUDP.Size = new global::System.Drawing.Size(181, 19);
			this.CheckTCPoverUDP.TabIndex = 26;
			this.CheckTCPoverUDP.Text = "TCP over TCP if not checked";
			this.CheckTCPoverUDP.UseVisualStyleBackColor = true;
			this.CheckTCPoverUDP.Visible = false;
			this.TCPProtocolComboBox.Anchor = (global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.TCPProtocolComboBox.DropDownStyle = global::System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.TCPProtocolComboBox.FormattingEnabled = true;
			this.TCPProtocolComboBox.Items.AddRange(new object[]
			{
				"origin",
				"verify_simple",
				"verify_deflate",
				"verify_sha1",
				"auth_simple",
				"auth_sha1",
				"auth_sha1_v2"
			});
			this.TCPProtocolComboBox.Location = new global::System.Drawing.Point(100, 116);
			this.TCPProtocolComboBox.Name = "TCPProtocolComboBox";
			this.TCPProtocolComboBox.Size = new global::System.Drawing.Size(215, 23);
			this.TCPProtocolComboBox.TabIndex = 32;
			this.labelObfsParam.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.labelObfsParam.AutoSize = true;
			this.labelObfsParam.Location = new global::System.Drawing.Point(22, 177);
			this.labelObfsParam.Name = "labelObfsParam";
			this.labelObfsParam.Size = new global::System.Drawing.Size(72, 15);
			this.labelObfsParam.TabIndex = 33;
			this.labelObfsParam.Text = "Obfs param";
			this.textObfsParam.Anchor = (global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.textObfsParam.Location = new global::System.Drawing.Point(100, 174);
			this.textObfsParam.Name = "textObfsParam";
			this.textObfsParam.Size = new global::System.Drawing.Size(215, 21);
			this.textObfsParam.TabIndex = 35;
			this.labelGroup.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.labelGroup.AutoSize = true;
			this.labelGroup.Location = new global::System.Drawing.Point(53, 231);
			this.labelGroup.Name = "labelGroup";
			this.labelGroup.Size = new global::System.Drawing.Size(41, 15);
			this.labelGroup.TabIndex = 33;
			this.labelGroup.Text = "Group";
			this.TextGroup.Anchor = (global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.TextGroup.Location = new global::System.Drawing.Point(100, 228);
			this.TextGroup.MaxLength = 32;
			this.TextGroup.Name = "TextGroup";
			this.TextGroup.Size = new global::System.Drawing.Size(215, 21);
			this.TextGroup.TabIndex = 12;
			this.TextGroup.WordWrap = false;
			this.checkAdvSetting.AutoSize = true;
			this.checkAdvSetting.Location = new global::System.Drawing.Point(6, 282);
			this.checkAdvSetting.Name = "checkAdvSetting";
			this.checkAdvSetting.Size = new global::System.Drawing.Size(88, 19);
			this.checkAdvSetting.TabIndex = 37;
			this.checkAdvSetting.Text = "Adv. Setting";
			this.checkAdvSetting.UseVisualStyleBackColor = true;
			this.checkAdvSetting.CheckedChanged += new global::System.EventHandler(this.checkAdvSetting_CheckedChanged);
			this.labelUDPPort.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.labelUDPPort.AutoSize = true;
			this.labelUDPPort.Location = new global::System.Drawing.Point(36, 310);
			this.labelUDPPort.Margin = new global::System.Windows.Forms.Padding(3, 5, 3, 5);
			this.labelUDPPort.Name = "labelUDPPort";
			this.labelUDPPort.Size = new global::System.Drawing.Size(58, 15);
			this.labelUDPPort.TabIndex = 23;
			this.labelUDPPort.Text = "UDP Port";
			this.labelUDPPort.Visible = false;
			this.textUDPPort.Anchor = (global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.textUDPPort.Location = new global::System.Drawing.Point(100, 307);
			this.textUDPPort.MaxLength = 10;
			this.textUDPPort.Name = "textUDPPort";
			this.textUDPPort.Size = new global::System.Drawing.Size(215, 21);
			this.textUDPPort.TabIndex = 1;
			this.textUDPPort.Visible = false;
			this.textUDPPort.WordWrap = false;
			this.checkSSRLink.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.checkSSRLink.AutoSize = true;
			this.checkSSRLink.Checked = true;
			this.checkSSRLink.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.checkSSRLink.Location = new global::System.Drawing.Point(17, 256);
			this.checkSSRLink.Name = "checkSSRLink";
			this.checkSSRLink.Size = new global::System.Drawing.Size(77, 19);
			this.checkSSRLink.TabIndex = 38;
			this.checkSSRLink.Text = "SSR Link";
			this.checkSSRLink.UseVisualStyleBackColor = true;
			this.checkSSRLink.CheckedChanged += new global::System.EventHandler(this.checkSSRLink_CheckedChanged);
			this.labelRemarks.Anchor = global::System.Windows.Forms.AnchorStyles.Right;
			this.labelRemarks.AutoSize = true;
			this.labelRemarks.Location = new global::System.Drawing.Point(36, 204);
			this.labelRemarks.Name = "labelRemarks";
			this.labelRemarks.Size = new global::System.Drawing.Size(58, 15);
			this.labelRemarks.TabIndex = 39;
			this.labelRemarks.Text = "Remarks";
			this.panel2.Anchor = global::System.Windows.Forms.AnchorStyles.Top;
			this.panel2.AutoSize = true;
			this.panel2.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel2.Location = new global::System.Drawing.Point(373, 200);
			this.panel2.Name = "panel2";
			this.panel2.Size = new global::System.Drawing.Size(0, 0);
			this.panel2.TabIndex = 1;
			this.DeleteButton.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Right);
			this.DeleteButton.Location = new global::System.Drawing.Point(130, 0);
			this.DeleteButton.Margin = new global::System.Windows.Forms.Padding(0);
			this.DeleteButton.Name = "DeleteButton";
			this.DeleteButton.Size = new global::System.Drawing.Size(120, 34);
			this.DeleteButton.TabIndex = 7;
			this.DeleteButton.Text = "&Delete";
			this.DeleteButton.UseVisualStyleBackColor = true;
			this.DeleteButton.Click += new global::System.EventHandler(this.DeleteButton_Click);
			this.AddButton.Location = new global::System.Drawing.Point(0, 0);
			this.AddButton.Margin = new global::System.Windows.Forms.Padding(0);
			this.AddButton.Name = "AddButton";
			this.AddButton.Size = new global::System.Drawing.Size(120, 34);
			this.AddButton.TabIndex = 6;
			this.AddButton.Text = "&Add";
			this.AddButton.UseVisualStyleBackColor = true;
			this.AddButton.Click += new global::System.EventHandler(this.AddButton_Click);
			this.ServerGroupBox.AutoSize = true;
			this.ServerGroupBox.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ServerGroupBox.Controls.Add(this.tableLayoutPanel1);
			this.ServerGroupBox.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.ServerGroupBox.Location = new global::System.Drawing.Point(268, 0);
			this.ServerGroupBox.Margin = new global::System.Windows.Forms.Padding(12, 0, 0, 0);
			this.ServerGroupBox.Name = "ServerGroupBox";
			this.ServerGroupBox.Size = new global::System.Drawing.Size(332, 458);
			this.ServerGroupBox.TabIndex = 6;
			this.ServerGroupBox.TabStop = false;
			this.ServerGroupBox.Text = "Server";
			this.PictureQRcode.Anchor = global::System.Windows.Forms.AnchorStyles.None;
			this.PictureQRcode.BackColor = global::System.Drawing.SystemColors.Control;
			this.PictureQRcode.Location = new global::System.Drawing.Point(4, 105);
			this.PictureQRcode.Margin = new global::System.Windows.Forms.Padding(4);
			this.PictureQRcode.Name = "PictureQRcode";
			this.PictureQRcode.Size = new global::System.Drawing.Size(260, 200);
			this.PictureQRcode.SizeMode = global::System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.PictureQRcode.TabIndex = 13;
			this.PictureQRcode.TabStop = false;
			this.ServersListBox.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.ServersListBox.FormattingEnabled = true;
			this.ServersListBox.IntegralHeight = false;
			this.ServersListBox.ItemHeight = 15;
			this.ServersListBox.Location = new global::System.Drawing.Point(0, 0);
			this.ServersListBox.Margin = new global::System.Windows.Forms.Padding(0);
			this.ServersListBox.Name = "ServersListBox";
			this.ServersListBox.Size = new global::System.Drawing.Size(250, 280);
			this.ServersListBox.TabIndex = 5;
			this.ServersListBox.SelectedIndexChanged += new global::System.EventHandler(this.ServersListBox_SelectedIndexChanged);
			this.tableLayoutPanel2.AutoSize = true;
			this.tableLayoutPanel2.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel2.ColumnCount = 3;
			this.tableLayoutPanel2.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.Controls.Add(this.ServerGroupBox, 1, 0);
			this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel7, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel5, 2, 0);
			this.tableLayoutPanel2.Location = new global::System.Drawing.Point(12, 13);
			this.tableLayoutPanel2.Margin = new global::System.Windows.Forms.Padding(0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new global::System.Drawing.Size(883, 458);
			this.tableLayoutPanel2.TabIndex = 7;
			this.tableLayoutPanel7.AutoSize = true;
			this.tableLayoutPanel7.ColumnCount = 1;
			this.tableLayoutPanel7.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel7.Controls.Add(this.ServersListBox, 0, 0);
			this.tableLayoutPanel7.Controls.Add(this.LinkUpdate, 0, 2);
			this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel4, 0, 1);
			this.tableLayoutPanel7.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel7.Location = new global::System.Drawing.Point(3, 3);
			this.tableLayoutPanel7.Name = "tableLayoutPanel7";
			this.tableLayoutPanel7.RowCount = 3;
			this.tableLayoutPanel2.SetRowSpan(this.tableLayoutPanel7, 2);
			this.tableLayoutPanel7.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel7.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel7.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel7.Size = new global::System.Drawing.Size(250, 452);
			this.tableLayoutPanel7.TabIndex = 16;
			this.LinkUpdate.Anchor = global::System.Windows.Forms.AnchorStyles.None;
			this.LinkUpdate.AutoSize = true;
			this.LinkUpdate.Location = new global::System.Drawing.Point(61, 395);
			this.LinkUpdate.Margin = new global::System.Windows.Forms.Padding(5);
			this.LinkUpdate.Name = "LinkUpdate";
			this.LinkUpdate.Size = new global::System.Drawing.Size(127, 15);
			this.LinkUpdate.TabIndex = 15;
			this.LinkUpdate.TabStop = true;
			this.LinkUpdate.Text = "New version available";
			this.LinkUpdate.LinkClicked += new global::System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkUpdate_LinkClicked);
			this.tableLayoutPanel4.AutoSize = true;
			this.tableLayoutPanel4.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel4.ColumnCount = 2;
			this.tableLayoutPanel4.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel4.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel4.Controls.Add(this.DownButton, 1, 1);
			this.tableLayoutPanel4.Controls.Add(this.UpButton, 0, 1);
			this.tableLayoutPanel4.Controls.Add(this.DeleteButton, 1, 0);
			this.tableLayoutPanel4.Controls.Add(this.AddButton, 0, 0);
			this.tableLayoutPanel4.Dock = global::System.Windows.Forms.DockStyle.Bottom;
			this.tableLayoutPanel4.Location = new global::System.Drawing.Point(0, 285);
			this.tableLayoutPanel4.Margin = new global::System.Windows.Forms.Padding(0, 5, 0, 0);
			this.tableLayoutPanel4.Name = "tableLayoutPanel4";
			this.tableLayoutPanel4.RowCount = 2;
			this.tableLayoutPanel4.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.Size = new global::System.Drawing.Size(250, 68);
			this.tableLayoutPanel4.TabIndex = 8;
			this.DownButton.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Right);
			this.DownButton.Location = new global::System.Drawing.Point(130, 34);
			this.DownButton.Margin = new global::System.Windows.Forms.Padding(0);
			this.DownButton.Name = "DownButton";
			this.DownButton.Size = new global::System.Drawing.Size(120, 34);
			this.DownButton.TabIndex = 9;
			this.DownButton.Text = "Down";
			this.DownButton.UseVisualStyleBackColor = true;
			this.DownButton.Click += new global::System.EventHandler(this.DownButton_Click);
			this.UpButton.Location = new global::System.Drawing.Point(0, 34);
			this.UpButton.Margin = new global::System.Windows.Forms.Padding(0);
			this.UpButton.Name = "UpButton";
			this.UpButton.Size = new global::System.Drawing.Size(120, 34);
			this.UpButton.TabIndex = 8;
			this.UpButton.Text = "Up";
			this.UpButton.UseVisualStyleBackColor = true;
			this.UpButton.Click += new global::System.EventHandler(this.UpButton_Click);
			this.tableLayoutPanel5.AutoSize = true;
			this.tableLayoutPanel5.ColumnCount = 1;
			this.tableLayoutPanel5.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel3, 0, 1);
			this.tableLayoutPanel5.Controls.Add(this.PictureQRcode, 0, 0);
			this.tableLayoutPanel5.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel5.Location = new global::System.Drawing.Point(612, 3);
			this.tableLayoutPanel5.Margin = new global::System.Windows.Forms.Padding(12, 3, 3, 3);
			this.tableLayoutPanel5.Name = "tableLayoutPanel5";
			this.tableLayoutPanel5.RowCount = 2;
			this.tableLayoutPanel5.RowStyles.Add(new global::System.Windows.Forms.RowStyle(global::System.Windows.Forms.SizeType.Percent, 100f));
			this.tableLayoutPanel5.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel5.Size = new global::System.Drawing.Size(268, 452);
			this.tableLayoutPanel5.TabIndex = 17;
			this.tableLayoutPanel3.Anchor = global::System.Windows.Forms.AnchorStyles.Bottom;
			this.tableLayoutPanel3.AutoSize = true;
			this.tableLayoutPanel3.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.ColumnCount = 2;
			this.tableLayoutPanel3.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.ColumnStyles.Add(new global::System.Windows.Forms.ColumnStyle(global::System.Windows.Forms.SizeType.Absolute, 20f));
			this.tableLayoutPanel3.Controls.Add(this.MyCancelButton, 1, 0);
			this.tableLayoutPanel3.Controls.Add(this.OKButton, 0, 0);
			this.tableLayoutPanel3.Location = new global::System.Drawing.Point(56, 413);
			this.tableLayoutPanel3.Margin = new global::System.Windows.Forms.Padding(3, 3, 0, 3);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 1;
			this.tableLayoutPanel3.RowStyles.Add(new global::System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.Size = new global::System.Drawing.Size(159, 36);
			this.tableLayoutPanel3.TabIndex = 14;
			this.MyCancelButton.AutoSize = true;
			this.MyCancelButton.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.MyCancelButton.Dock = global::System.Windows.Forms.DockStyle.Right;
			this.MyCancelButton.Location = new global::System.Drawing.Point(84, 3);
			this.MyCancelButton.Margin = new global::System.Windows.Forms.Padding(3, 3, 0, 0);
			this.MyCancelButton.Name = "MyCancelButton";
			this.MyCancelButton.Size = new global::System.Drawing.Size(75, 33);
			this.MyCancelButton.TabIndex = 9;
			this.MyCancelButton.Text = "Cancel";
			this.MyCancelButton.UseVisualStyleBackColor = true;
			this.MyCancelButton.Click += new global::System.EventHandler(this.CancelButton_Click);
			this.OKButton.AutoSize = true;
			this.OKButton.DialogResult = global::System.Windows.Forms.DialogResult.OK;
			this.OKButton.Dock = global::System.Windows.Forms.DockStyle.Right;
			this.OKButton.Location = new global::System.Drawing.Point(3, 3);
			this.OKButton.Margin = new global::System.Windows.Forms.Padding(3, 3, 3, 0);
			this.OKButton.Name = "OKButton";
			this.OKButton.Size = new global::System.Drawing.Size(75, 33);
			this.OKButton.TabIndex = 8;
			this.OKButton.Text = "OK";
			this.OKButton.UseVisualStyleBackColor = true;
			this.OKButton.Click += new global::System.EventHandler(this.OKButton_Click);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.None;
			this.AutoSize = true;
			base.AutoSizeMode = global::System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = global::System.Drawing.SystemColors.Control;
			base.ClientSize = new global::System.Drawing.Size(906, 512);
			base.Controls.Add(this.tableLayoutPanel2);
			base.Controls.Add(this.panel2);
			this.Font = new global::System.Drawing.Font("Arial", 9f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 0);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ConfigForm";
			base.Padding = new global::System.Windows.Forms.Padding(12, 13, 12, 13);
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Servers";
			base.FormClosed += new global::System.Windows.Forms.FormClosedEventHandler(this.ConfigForm_FormClosed);
			base.Shown += new global::System.EventHandler(this.ConfigForm_Shown);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ServerGroupBox.ResumeLayout(false);
			this.ServerGroupBox.PerformLayout();
			((global::System.ComponentModel.ISupportInitialize)this.PictureQRcode).EndInit();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.tableLayoutPanel7.ResumeLayout(false);
			this.tableLayoutPanel7.PerformLayout();
			this.tableLayoutPanel4.ResumeLayout(false);
			this.tableLayoutPanel5.ResumeLayout(false);
			this.tableLayoutPanel5.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x04000012 RID: 18
		private global::System.Windows.Forms.Button AddButton;

		// Token: 0x04000032 RID: 50
		private global::System.Windows.Forms.CheckBox checkAdvSetting;

		// Token: 0x04000020 RID: 32
		private global::System.Windows.Forms.CheckBox CheckObfsUDP;

		// Token: 0x04000035 RID: 53
		private global::System.Windows.Forms.CheckBox checkSSRLink;

		// Token: 0x04000029 RID: 41
		private global::System.Windows.Forms.CheckBox CheckTCPoverUDP;

		// Token: 0x0400001F RID: 31
		private global::System.Windows.Forms.CheckBox CheckUDPoverUDP;

		// Token: 0x04000007 RID: 7
		private global::System.ComponentModel.IContainer components;

		// Token: 0x04000011 RID: 17
		private global::System.Windows.Forms.Button DeleteButton;

		// Token: 0x04000018 RID: 24
		private global::System.Windows.Forms.Button DownButton;

		// Token: 0x0400000E RID: 14
		private global::System.Windows.Forms.Label EncryptionLabel;

		// Token: 0x0400000F RID: 15
		private global::System.Windows.Forms.ComboBox EncryptionSelect;

		// Token: 0x04000009 RID: 9
		private global::System.Windows.Forms.Label IPLabel;

		// Token: 0x0400000B RID: 11
		private global::System.Windows.Forms.TextBox IPTextBox;

		// Token: 0x04000030 RID: 48
		private global::System.Windows.Forms.Label labelGroup;

		// Token: 0x04000024 RID: 36
		private global::System.Windows.Forms.Label LabelNote;

		// Token: 0x0400002D RID: 45
		private global::System.Windows.Forms.Label labelObfs;

		// Token: 0x0400002E RID: 46
		private global::System.Windows.Forms.Label labelObfsParam;

		// Token: 0x04000036 RID: 54
		private global::System.Windows.Forms.Label labelRemarks;

		// Token: 0x04000033 RID: 51
		private global::System.Windows.Forms.Label labelUDPPort;

		// Token: 0x04000025 RID: 37
		private global::System.Windows.Forms.LinkLabel LinkUpdate;

		// Token: 0x04000022 RID: 34
		private global::System.Windows.Forms.Button MyCancelButton;

		// Token: 0x0400002C RID: 44
		private global::System.Windows.Forms.ComboBox ObfsCombo;

		// Token: 0x0400001D RID: 29
		private global::System.Windows.Forms.Label ObfsUDPLabel;

		// Token: 0x04000023 RID: 35
		private global::System.Windows.Forms.Button OKButton;

		// Token: 0x04000010 RID: 16
		private global::System.Windows.Forms.Panel panel2;

		// Token: 0x04000027 RID: 39
		private global::System.Windows.Forms.CheckBox PasswordLabel;

		// Token: 0x0400000D RID: 13
		private global::System.Windows.Forms.TextBox PasswordTextBox;

		// Token: 0x0400001A RID: 26
		private global::System.Windows.Forms.PictureBox PictureQRcode;

		// Token: 0x04000015 RID: 21
		private global::System.Windows.Forms.TextBox RemarksTextBox;

		// Token: 0x04000013 RID: 19
		private global::System.Windows.Forms.GroupBox ServerGroupBox;

		// Token: 0x0400000A RID: 10
		private global::System.Windows.Forms.Label ServerPortLabel;

		// Token: 0x0400000C RID: 12
		private global::System.Windows.Forms.TextBox ServerPortTextBox;

		// Token: 0x04000014 RID: 20
		private global::System.Windows.Forms.ListBox ServersListBox;

		// Token: 0x04000008 RID: 8
		private global::System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

		// Token: 0x04000016 RID: 22
		private global::System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;

		// Token: 0x04000021 RID: 33
		private global::System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;

		// Token: 0x04000017 RID: 23
		private global::System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;

		// Token: 0x0400002B RID: 43
		private global::System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;

		// Token: 0x04000026 RID: 38
		private global::System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;

		// Token: 0x04000028 RID: 40
		private global::System.Windows.Forms.Label TCPoverUDPLabel;

		// Token: 0x0400002A RID: 42
		private global::System.Windows.Forms.ComboBox TCPProtocolComboBox;

		// Token: 0x0400001C RID: 28
		private global::System.Windows.Forms.Label TCPProtocolLabel;

		// Token: 0x04000031 RID: 49
		private global::System.Windows.Forms.TextBox TextGroup;

		// Token: 0x0400001E RID: 30
		private global::System.Windows.Forms.TextBox TextLink;

		// Token: 0x0400002F RID: 47
		private global::System.Windows.Forms.TextBox textObfsParam;

		// Token: 0x04000034 RID: 52
		private global::System.Windows.Forms.TextBox textUDPPort;

		// Token: 0x0400001B RID: 27
		private global::System.Windows.Forms.Label UDPoverTCPLabel;

		// Token: 0x04000019 RID: 25
		private global::System.Windows.Forms.Button UpButton;
	}
}
