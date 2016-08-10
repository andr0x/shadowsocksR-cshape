using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Shadowsocks.Controller;
using Shadowsocks.Model;
using Shadowsocks.Properties;

namespace Shadowsocks.View
{
	// Token: 0x0200000A RID: 10
	public partial class SettingsForm : Form
	{
		// Token: 0x0600008E RID: 142 RVA: 0x0000A5C0 File Offset: 0x000087C0
		public SettingsForm(ShadowsocksController controller)
		{
			this.Font = SystemFonts.MessageBoxFont;
			this.InitializeComponent();
			base.Icon = Icon.FromHandle(Resources.ssw128.GetHicon());
			this.controller = controller;
			this.UpdateTexts();
			controller.ConfigChanged += new EventHandler(this.controller_ConfigChanged);
			this.LoadCurrentConfiguration();
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00003051 File Offset: 0x00001251
		private void CancelButton_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		// Token: 0x06000091 RID: 145 RVA: 0x0000A941 File Offset: 0x00008B41
		private void controller_ConfigChanged(object sender, EventArgs e)
		{
			this.LoadCurrentConfiguration();
		}

		// Token: 0x06000095 RID: 149 RVA: 0x0000AD39 File Offset: 0x00008F39
		private void LoadCurrentConfiguration()
		{
			this._modifiedConfiguration = this.controller.GetConfiguration();
			this.LoadSelectedServer();
		}

		// Token: 0x06000094 RID: 148 RVA: 0x0000AB7C File Offset: 0x00008D7C
		private void LoadSelectedServer()
		{
			this.checkShareOverLan.Checked = this._modifiedConfiguration.shareOverLan;
			this.ProxyPortTextBox.Text = this._modifiedConfiguration.localPort.ToString();
			this.ReconnectText.Text = this._modifiedConfiguration.reconnectTimes.ToString();
			this.checkAutoStartup.Checked = AutoStartup.Check();
			this.checkRandom.Checked = this._modifiedConfiguration.random;
			this.RandomComboBox.SelectedIndex = this._modifiedConfiguration.randomAlgorithm;
			this.TTLText.Text = this._modifiedConfiguration.TTL.ToString();
			this.DNSText.Text = this._modifiedConfiguration.dns_server;
			this.CheckSockProxy.Checked = this._modifiedConfiguration.proxyEnable;
			this.checkBoxPacProxy.Checked = this._modifiedConfiguration.pacDirectGoProxy;
			this.comboProxyType.SelectedIndex = this._modifiedConfiguration.proxyType;
			this.TextS5Server.Text = this._modifiedConfiguration.proxyHost;
			this.TextS5Port.Text = this._modifiedConfiguration.proxyPort.ToString();
			this.TextS5User.Text = this._modifiedConfiguration.proxyAuthUser;
			this.TextS5Pass.Text = this._modifiedConfiguration.proxyAuthPass;
			this.TextUserAgent.Text = this._modifiedConfiguration.proxyUserAgent;
			this.TextAuthUser.Text = this._modifiedConfiguration.authUser;
			this.TextAuthPass.Text = this._modifiedConfiguration.authPass;
			this.CheckAutoBan.Checked = this._modifiedConfiguration.autoBan;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x0000AD54 File Offset: 0x00008F54
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
			this.controller.SaveServersConfig(this._modifiedConfiguration);
			base.Close();
		}

		// Token: 0x06000093 RID: 147 RVA: 0x0000A960 File Offset: 0x00008B60
		private int SaveOldSelectedServer()
		{
			try
			{
				int num = int.Parse(this.ProxyPortTextBox.Text);
				Configuration.CheckPort(num);
				int arg_1DD_0 = 0;
				this._modifiedConfiguration.shareOverLan = this.checkShareOverLan.Checked;
				this._modifiedConfiguration.localPort = num;
				this._modifiedConfiguration.reconnectTimes = int.Parse(this.ReconnectText.Text);
				if (this.checkAutoStartup.Checked != AutoStartup.Check() && !AutoStartup.Set(this.checkAutoStartup.Checked))
				{
					MessageBox.Show(I18N.GetString("Failed to update registry"));
				}
				this._modifiedConfiguration.random = this.checkRandom.Checked;
				this._modifiedConfiguration.randomAlgorithm = this.RandomComboBox.SelectedIndex;
				this._modifiedConfiguration.TTL = int.Parse(this.TTLText.Text);
				this._modifiedConfiguration.dns_server = this.DNSText.Text;
				this._modifiedConfiguration.proxyEnable = this.CheckSockProxy.Checked;
				this._modifiedConfiguration.pacDirectGoProxy = this.checkBoxPacProxy.Checked;
				this._modifiedConfiguration.proxyType = this.comboProxyType.SelectedIndex;
				this._modifiedConfiguration.proxyHost = this.TextS5Server.Text;
				this._modifiedConfiguration.proxyPort = int.Parse(this.TextS5Port.Text);
				this._modifiedConfiguration.proxyAuthUser = this.TextS5User.Text;
				this._modifiedConfiguration.proxyAuthPass = this.TextS5Pass.Text;
				this._modifiedConfiguration.proxyUserAgent = this.TextUserAgent.Text;
				this._modifiedConfiguration.authUser = this.TextAuthUser.Text;
				this._modifiedConfiguration.authPass = this.TextAuthPass.Text;
				this._modifiedConfiguration.autoBan = this.CheckAutoBan.Checked;
				return arg_1DD_0;
			}
			catch (Exception arg_1E0_0)
			{
				MessageBox.Show(arg_1E0_0.Message);
			}
			return -1;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x0000A61E File Offset: 0x0000881E
		private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.controller.ConfigChanged -= new EventHandler(this.controller_ConfigChanged);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x0000A949 File Offset: 0x00008B49
		private void ShowWindow()
		{
			base.Opacity = 1.0;
			base.Show();
		}

		// Token: 0x06000090 RID: 144 RVA: 0x0000A638 File Offset: 0x00008838
		private void UpdateTexts()
		{
			this.Text = string.Concat(new string[]
			{
				I18N.GetString("Global Settings"),
				"(",
				this.controller.GetCurrentConfiguration().shareOverLan ? "any" : "local",
				":",
				this.controller.GetCurrentConfiguration().localPort.ToString(),
				I18N.GetString(" Version"),
				"3.8.4.2 blue)"
			});
			this.ListenGroup.Text = I18N.GetString(this.ListenGroup.Text);
			this.checkShareOverLan.Text = I18N.GetString(this.checkShareOverLan.Text);
			this.ProxyPortLabel.Text = I18N.GetString("Proxy Port");
			this.ReconnectLabel.Text = I18N.GetString("Reconnect Times");
			this.TTLLabel.Text = I18N.GetString("TTL");
			this.checkAutoStartup.Text = I18N.GetString(this.checkAutoStartup.Text);
			this.checkRandom.Text = I18N.GetString(this.checkRandom.Text);
			this.CheckAutoBan.Text = I18N.GetString("AutoBan");
			this.Socks5ProxyGroup.Text = I18N.GetString(this.Socks5ProxyGroup.Text);
			this.checkBoxPacProxy.Text = I18N.GetString(this.checkBoxPacProxy.Text);
			this.CheckSockProxy.Text = I18N.GetString("Proxy On");
			this.LabelS5Server.Text = I18N.GetString("Server IP");
			this.LabelS5Port.Text = I18N.GetString("Server Port");
			this.LabelS5Server.Text = I18N.GetString("Server IP");
			this.LabelS5Port.Text = I18N.GetString("Server Port");
			this.LabelS5Username.Text = I18N.GetString("Username");
			this.LabelS5Password.Text = I18N.GetString("Password");
			this.LabelAuthUser.Text = I18N.GetString("Username");
			this.LabelAuthPass.Text = I18N.GetString("Password");
			this.LabelRandom.Text = I18N.GetString("Balance");
			for (int i = 0; i < this.comboProxyType.Items.Count; i++)
			{
				this.comboProxyType.Items[i] = I18N.GetString(this.comboProxyType.Items[i].ToString());
			}
			for (int j = 0; j < this.RandomComboBox.Items.Count; j++)
			{
				this.RandomComboBox.Items[j] = I18N.GetString(this.RandomComboBox.Items[j].ToString());
			}
			this.OKButton.Text = I18N.GetString("OK");
			this.MyCancelButton.Text = I18N.GetString("Cancel");
		}

		// Token: 0x0400008B RID: 139
		private ShadowsocksController controller;

		// Token: 0x0400008C RID: 140
		private Configuration _modifiedConfiguration;
	}
}
