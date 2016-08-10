using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Forms;
using Shadowsocks.Controller;
using Shadowsocks.Model;
using Shadowsocks.Properties;
using Shadowsocks.Util;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace Shadowsocks.View
{
	// Token: 0x02000004 RID: 4
	public class MenuViewController
	{
		// Token: 0x0600001E RID: 30 RVA: 0x000057EC File Offset: 0x000039EC
		public MenuViewController(ShadowsocksController controller)
		{
			this.controller = controller;
			this.LoadMenu();
			controller.EnableStatusChanged += new EventHandler(this.controller_EnableStatusChanged);
			controller.ConfigChanged += new EventHandler(this.controller_ConfigChanged);
			controller.PACFileReadyToOpen += new EventHandler<ShadowsocksController.PathEventArgs>(this.controller_FileReadyToOpen);
			controller.UserRuleFileReadyToOpen += new EventHandler<ShadowsocksController.PathEventArgs>(this.controller_FileReadyToOpen);
			controller.EnableGlobalChanged += new EventHandler(this.controller_EnableGlobalChanged);
			controller.Errored += new ErrorEventHandler(this.controller_Errored);
			controller.UpdatePACFromGFWListCompleted += new EventHandler<GFWListUpdater.ResultEventArgs>(this.controller_UpdatePACFromGFWListCompleted);
			controller.UpdatePACFromGFWListError += new ErrorEventHandler(this.controller_UpdatePACFromGFWListError);
			controller.ShowConfigFormEvent += new EventHandler(this.Config_Click);
			this._notifyIcon = new NotifyIcon();
			this.UpdateTrayIcon();
			this._notifyIcon.Visible = true;
			this._notifyIcon.ContextMenu = this.contextMenu1;
			this._notifyIcon.MouseClick += new MouseEventHandler(this.notifyIcon1_Click);
			this.updateChecker = new UpdateChecker();
			this.updateChecker.NewVersionFound += new EventHandler(this.updateChecker_NewVersionFound);
			this.LoadCurrentConfiguration();
			if (controller.GetConfiguration().isDefault)
			{
				this._isFirstRun = true;
				this.ShowConfigForm(false);
			}
			this.timerDelayCheckUpdate = new System.Timers.Timer(10000.0);
			this.timerDelayCheckUpdate.Elapsed += new ElapsedEventHandler(this.timer_Elapsed);
			this.timerDelayCheckUpdate.Start();
		}

		// Token: 0x0600003B RID: 59 RVA: 0x000066E2 File Offset: 0x000048E2
		private void AboutItem_Click(object sender, EventArgs e)
		{
		   
			Process.Start("https://github.com/breakwa11/shadowsocks-rss");
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00006904 File Offset: 0x00004B04
		private void AServerItem_Click(object sender, EventArgs e)
		{
			MenuItem menuItem = (MenuItem)sender;
			this.controller.SelectServerIndex((int)menuItem.Tag);
		}

		// Token: 0x0600004D RID: 77 RVA: 0x0000692E File Offset: 0x00004B2E
		private void CheckUpdate_Click(object sender, EventArgs e)
		{
			this.updateChecker.CheckUpdate(this.controller.GetCurrentConfiguration());
		}

		// Token: 0x06000034 RID: 52 RVA: 0x000065B7 File Offset: 0x000047B7
		private void configForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.configForm = null;
			Utils.ReleaseMemory();
			this.ShowFirstTimeBalloon();
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000065E2 File Offset: 0x000047E2
		private void Config_Click(object sender, EventArgs e)
		{
			this.ShowConfigForm(false);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00006033 File Offset: 0x00004233
		private void controller_ConfigChanged(object sender, EventArgs e)
		{
			this.LoadCurrentConfiguration();
			this.UpdateTrayIcon();
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00006074 File Offset: 0x00004274
		private void controller_EnableGlobalChanged(object sender, EventArgs e)
		{
			this.globalModeItem.Checked = this.controller.GetConfiguration().global;
			this.PACModeItem.Checked = !this.globalModeItem.Checked;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00006041 File Offset: 0x00004241
		private void controller_EnableStatusChanged(object sender, EventArgs e)
		{
			this.enableItem.Checked = this.controller.GetConfiguration().enabled;
			this.modeItem.Enabled = this.enableItem.Checked;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000059EF File Offset: 0x00003BEF
		private void controller_Errored(object sender, ErrorEventArgs e)
		{
			MessageBox.Show(e.GetException().ToString(), string.Format(I18N.GetString("Shadowsocks Error: {0}"), e.GetException().Message));
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000060AC File Offset: 0x000042AC
		private void controller_FileReadyToOpen(object sender, ShadowsocksController.PathEventArgs e)
		{
			string arguments = "/select, " + e.Path;
			Process.Start("explorer.exe", arguments);
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00006140 File Offset: 0x00004340
		private void controller_UpdatePACFromGFWListCompleted(object sender, GFWListUpdater.ResultEventArgs e)
		{
			GFWListUpdater gFWListUpdater = (GFWListUpdater)sender;
			string content = e.Success ? ((gFWListUpdater.update_type <= 1) ? I18N.GetString("PAC updated") : I18N.GetString("Domain white list list updated")) : I18N.GetString("No updates found. Please report to GFWList if you have problems with it.");
			this.ShowBalloonTip(I18N.GetString("Shadowsocks"), content, ToolTipIcon.Info, 1000);
			if (gFWListUpdater.update_type == 8)
			{
				this.controller.ToggleBypass(this.httpWhiteListItem.Checked);
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00006109 File Offset: 0x00004309
		private void controller_UpdatePACFromGFWListError(object sender, ErrorEventArgs e)
		{
			GFWListUpdater arg_06_0 = (GFWListUpdater)sender;
			this.ShowBalloonTip(I18N.GetString("Failed to update PAC file"), e.GetException().Message, ToolTipIcon.Error, 5000);
			Logging.LogUsefulException(e.GetException());
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00006A30 File Offset: 0x00004C30
		private void CopyAddress_Click(object sender, EventArgs e)
		{
			IDataObject dataObject = Clipboard.GetDataObject();
			if (dataObject.GetDataPresent(DataFormats.Text))
			{
				string[] array = ((string)dataObject.GetData(DataFormats.Text)).Split(new string[]
				{
					"\r\n"
				}, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < array.Length; i++)
				{
					string ssURL = array[i];
					this.controller.AddServerBySSURL(ssURL);
				}
				this.ShowConfigForm(true);
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00005C0A File Offset: 0x00003E0A
		private MenuItem CreateMenuGroup(string text, MenuItem[] items)
		{
			return new MenuItem(I18N.GetString(text), items);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00005BFC File Offset: 0x00003DFC
		private MenuItem CreateMenuItem(string text, EventHandler click)
		{
			return new MenuItem(I18N.GetString(text), click);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000069E8 File Offset: 0x00004BE8
		private void DisconnectCurrent_Click(object sender, EventArgs e)
		{
			Configuration currentConfiguration = this.controller.GetCurrentConfiguration();
			for (int i = 0; i < currentConfiguration.configs.Count; i++)
			{
				currentConfiguration.configs[i].GetConnections().CloseAll();
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000066F0 File Offset: 0x000048F0
		private void DonateItem_Click(object sender, EventArgs e)
		{
			this._notifyIcon.BalloonTipTitle = I18N.GetString("Why donate?");
			this._notifyIcon.BalloonTipText = I18N.GetString("You can not donate!");
			this._notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
			this._notifyIcon.ShowBalloonTip(0);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00006893 File Offset: 0x00004A93
		private void EditPACFileItem_Click(object sender, EventArgs e)
		{
			this.controller.TouchPACFile();
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000068F5 File Offset: 0x00004AF5
		private void EditUserRuleFileForGFWListItem_Click(object sender, EventArgs e)
		{
			this.controller.TouchUserRuleFile();
		}

		// Token: 0x0600003F RID: 63 RVA: 0x0000678E File Offset: 0x0000498E
		private void EnableItem_Click(object sender, EventArgs e)
		{
			this.controller.ToggleEnable(!this.enableItem.Checked);
		}

		// Token: 0x0600003D RID: 61
		[DllImport("user32.dll")]
		private static extern short GetAsyncKeyState(Keys vKey);

		// Token: 0x06000040 RID: 64 RVA: 0x000067A9 File Offset: 0x000049A9
		private void GlobalModeItem_Click(object sender, EventArgs e)
		{
			this.controller.ToggleGlobal(true);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00006828 File Offset: 0x00004A28
		private void HttpWhiteListItem_Click(object sender, EventArgs e)
		{
			this.httpWhiteListItem.Checked = !this.httpWhiteListItem.Checked;
			if (this.httpWhiteListItem.Checked && !File.Exists(Path.Combine(Application.StartupPath, PACServer.BYPASS_FILE)))
			{
				this.controller.UpdateBypassListFromDefault();
				return;
			}
			this.controller.ToggleBypass(this.httpWhiteListItem.Checked);
		}

		// Token: 0x0600002F RID: 47 RVA: 0x000062E8 File Offset: 0x000044E8
		private void LoadCurrentConfiguration()
		{
			Configuration configuration = this.controller.GetConfiguration();
			this.UpdateServersMenu();
			this.enableItem.Checked = configuration.enabled;
			this.modeItem.Enabled = configuration.enabled;
			this.globalModeItem.Checked = configuration.global;
			this.PACModeItem.Checked = !configuration.global;
			this.SelectRandomItem.Checked = configuration.random;
			this.sameHostForSameTargetItem.Checked = configuration.sameHostForSameTarget;
			this.httpWhiteListItem.Checked = configuration.bypassWhiteList;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00005C18 File Offset: 0x00003E18
		private void LoadMenu()
		{
			this.contextMenu1 = new ContextMenu(new MenuItem[]
			{
				this.enableItem = this.CreateMenuItem("Enable System Proxy", new EventHandler(this.EnableItem_Click)),
				this.modeItem = this.CreateMenuGroup("Mode", new MenuItem[]
				{
					this.PACModeItem = this.CreateMenuItem("PAC", new EventHandler(this.PACModeItem_Click)),
					this.globalModeItem = this.CreateMenuItem("Global", new EventHandler(this.GlobalModeItem_Click))
				}),
				this.CreateMenuGroup("PAC ", new MenuItem[]
				{
					this.updateFromGFWListItem = this.CreateMenuItem("Update Local PAC from Lan IP List", new EventHandler(this.UpdatePACFromLanIPListItem_Click)),
					this.updateFromGFWListItem = this.CreateMenuItem("Update Local PAC from Chn White List", new EventHandler(this.UpdatePACFromCNWhiteListItem_Click)),
					this.updateFromGFWListItem = this.CreateMenuItem("Update Local PAC from Chn IP List", new EventHandler(this.UpdatePACFromCNIPListItem_Click)),
					this.updateFromGFWListItem = this.CreateMenuItem("Update Local PAC from GFWList", new EventHandler(this.UpdatePACFromGFWListItem_Click)),
					new MenuItem("-"),
					this.updateFromGFWListItem = this.CreateMenuItem("Update Local PAC from Chn Only List", new EventHandler(this.UpdatePACFromCNOnlyListItem_Click)),
					new MenuItem("-"),
					this.editLocalPACItem = this.CreateMenuItem("Edit Local PAC File...", new EventHandler(this.EditPACFileItem_Click)),
					this.editGFWUserRuleItem = this.CreateMenuItem("Edit User Rule for GFWList...", new EventHandler(this.EditUserRuleFileForGFWListItem_Click))
				}),
				new MenuItem("-"),
				this.ServersItem = this.CreateMenuGroup("Servers", new MenuItem[]
				{
					this.SeperatorItem = new MenuItem("-"),
					this.ConfigItem = this.CreateMenuItem("Edit Servers...", new EventHandler(this.Config_Click))
				}),
				this.CreateMenuItem("Global Settings...", new EventHandler(this.Setting_Click)),
				new MenuItem("-"),
				this.SelectRandomItem = this.CreateMenuItem("Enable balance", new EventHandler(this.SelectRandomItem_Click)),
				this.sameHostForSameTargetItem = this.CreateMenuItem("Same host for same address", new EventHandler(this.SelectSameHostForSameTargetItem_Click)),
				this.httpWhiteListItem = this.CreateMenuItem("Enable domain white list(http proxy only)", new EventHandler(this.HttpWhiteListItem_Click)),
				this.UpdateItem = this.CreateMenuItem("Update available", new EventHandler(this.UpdateItem_Clicked)),
				new MenuItem("-"),
				this.CreateMenuItem("Scan QRCode from Screen...", new EventHandler(this.ScanQRCodeItem_Click)),
				this.CreateMenuItem("Copy Address from clipboard...", new EventHandler(this.CopyAddress_Click)),
				new MenuItem("-"),
				this.CreateMenuItem("Server Statistic...", new EventHandler(this.ShowServerLogItem_Click)),
				this.CreateMenuItem("Disconnect Current", new EventHandler(this.DisconnectCurrent_Click)),
				this.CreateMenuGroup("Help", new MenuItem[]
				{
					this.CreateMenuItem("Check Update", new EventHandler(this.CheckUpdate_Click)),
					this.CreateMenuItem("Show Logs...", new EventHandler(this.ShowLogItem_Click)),
					this.CreateMenuItem("About...", new EventHandler(this.AboutItem_Click)),
					this.CreateMenuItem("Donate...", new EventHandler(this.DonateItem_Click))
				}),
				new MenuItem("-"),
				this.CreateMenuItem("Quit", new EventHandler(this.Quit_Click))
			});
			this.UpdateItem.Visible = false;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000062CE File Offset: 0x000044CE
		private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
		{
			this._notifyIcon.BalloonTipClicked -= new EventHandler(this.notifyIcon1_BalloonTipClicked);
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00006740 File Offset: 0x00004940
		private void notifyIcon1_Click(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
			{
				if (e.Button == MouseButtons.Middle)
				{
					this.ShowServerLogForm();
				}
				return;
			}
			MenuViewController.GetAsyncKeyState(Keys.ShiftKey);
			if (MenuViewController.GetAsyncKeyState(Keys.ShiftKey) < 0)
			{
				this.ShowSettingForm();
				return;
			}
			this.ShowConfigForm(false);
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00006ED5 File Offset: 0x000050D5
		private void openURLFromQRCode(object sender, FormClosedEventArgs e)
		{
			Process.Start(this._urlToOpen);
		}

		// Token: 0x06000041 RID: 65 RVA: 0x000067B7 File Offset: 0x000049B7
		private void PACModeItem_Click(object sender, EventArgs e)
		{
			this.controller.ToggleGlobal(false);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00006A9B File Offset: 0x00004C9B
		private void QRCodeItem_Click(object sender, EventArgs e)
		{
			new QRCodeForm(this.controller.GetSSLinkForCurrentServer()).Show();
		}

		// Token: 0x06000039 RID: 57 RVA: 0x000065F4 File Offset: 0x000047F4
		private void Quit_Click(object sender, EventArgs e)
		{
			this.controller.Stop();
			if (this.configForm != null)
			{
				this.configForm.Close();
				this.configForm = null;
			}
			if (this.serverLogForm != null)
			{
				this.serverLogForm.Close();
				this.serverLogForm = null;
			}
			if (this.timerDelayCheckUpdate != null)
			{
				this.timerDelayCheckUpdate.Elapsed -= new ElapsedEventHandler(this.timer_Elapsed);
				this.timerDelayCheckUpdate.Stop();
				this.timerDelayCheckUpdate = null;
			}
			this._notifyIcon.Visible = false;
			Application.Exit();
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00006AB4 File Offset: 0x00004CB4
		private void ScanQRCodeItem_Click(object sender, EventArgs e)
		{
			Screen[] allScreens = Screen.AllScreens;
			for (int i = 0; i < allScreens.Length; i++)
			{
				Screen screen = allScreens[i];
				using (Bitmap bitmap = new Bitmap(screen.Bounds.Width, screen.Bounds.Height))
				{
					using (Graphics graphics = Graphics.FromImage(bitmap))
					{
						graphics.CopyFromScreen(screen.Bounds.X, screen.Bounds.Y, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
					}
					int num = 10;
					for (int j = 0; j < num; j++)
					{
						int num2 = (int)((double)bitmap.Width * (double)j / 2.5 / (double)num);
						int num3 = (int)((double)bitmap.Height * (double)j / 2.5 / (double)num);
						Rectangle srcRect = new Rectangle(num2, num3, bitmap.Width - num2 * 2, bitmap.Height - num3 * 2);
						Bitmap bitmap2 = new Bitmap(screen.Bounds.Width, screen.Bounds.Height);
						double num4 = (double)screen.Bounds.Width / (double)srcRect.Width;
						using (Graphics graphics2 = Graphics.FromImage(bitmap2))
						{
							graphics2.DrawImage(bitmap, new Rectangle(0, 0, bitmap2.Width, bitmap2.Height), srcRect, GraphicsUnit.Pixel);
						}
						BinaryBitmap image = new BinaryBitmap(new HybridBinarizer(new BitmapLuminanceSource(bitmap2)));
						Result result = new QRCodeReader().decode(image);
						if (result != null)
						{
							bool arg_19A_0 = this.controller.AddServerBySSURL(result.Text);
							QRCodeSplashForm qRCodeSplashForm = new QRCodeSplashForm();
							if (arg_19A_0)
							{
								qRCodeSplashForm.FormClosed += new FormClosedEventHandler(this.splash_FormClosed);
							}
							else
							{
								if (!result.Text.StartsWith("http://") && !result.Text.StartsWith("https://"))
								{
									MessageBox.Show(I18N.GetString("Failed to decode QRCode"));
									return;
								}
								this._urlToOpen = result.Text;
								qRCodeSplashForm.FormClosed += new FormClosedEventHandler(this.openURLFromQRCode);
							}
							double num5 = 2147483647.0;
							double num6 = 2147483647.0;
							double num7 = 0.0;
							double num8 = 0.0;
							ResultPoint[] resultPoints = result.ResultPoints;
							for (int k = 0; k < resultPoints.Length; k++)
							{
								ResultPoint resultPoint = resultPoints[k];
								num5 = Math.Min(num5, (double)resultPoint.X);
								num6 = Math.Min(num6, (double)resultPoint.Y);
								num7 = Math.Max(num7, (double)resultPoint.X);
								num8 = Math.Max(num8, (double)resultPoint.Y);
							}
							num5 /= num4;
							num6 /= num4;
							num7 /= num4;
							num8 /= num4;
							double num9 = (num7 - num5) * 0.20000000298023224;
							num5 += -num9 + (double)num2;
							num7 += num9 + (double)num2;
							num6 += -num9 + (double)num3;
							num8 += num9 + (double)num3;
							qRCodeSplashForm.Location = new Point(screen.Bounds.X, screen.Bounds.Y);
							qRCodeSplashForm.TargetRect = new Rectangle((int)num5 + screen.Bounds.X, (int)num6 + screen.Bounds.Y, (int)num7 - (int)num5, (int)num8 - (int)num6);
							qRCodeSplashForm.Size = new Size(bitmap.Width, bitmap.Height);
							qRCodeSplashForm.Show();
							return;
						}
					}
				}
			}
			MessageBox.Show(I18N.GetString("No QRCode found. Try to zoom in or move it to the center of the screen."));
             
           
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000067C5 File Offset: 0x000049C5
		private void SelectRandomItem_Click(object sender, EventArgs e)
		{
			this.SelectRandomItem.Checked = !this.SelectRandomItem.Checked;
			this.controller.ToggleSelectRandom(this.SelectRandomItem.Checked);
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000067F6 File Offset: 0x000049F6
		private void SelectSameHostForSameTargetItem_Click(object sender, EventArgs e)
		{
			this.sameHostForSameTargetItem.Checked = !this.sameHostForSameTargetItem.Checked;
			this.controller.ToggleSameHostForSameTargetRandom(this.sameHostForSameTargetItem.Checked);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000065D9 File Offset: 0x000047D9
		private void serverLogForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.serverLogForm = null;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000065CB File Offset: 0x000047CB
		private void settingsForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.settingsForm = null;
			Utils.ReleaseMemory();
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000065EB File Offset: 0x000047EB
		private void Setting_Click(object sender, EventArgs e)
		{
			this.ShowSettingForm();
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000060D6 File Offset: 0x000042D6
		private void ShowBalloonTip(string title, string content, ToolTipIcon icon, int timeout)
		{
			this._notifyIcon.BalloonTipTitle = title;
			this._notifyIcon.BalloonTipText = content;
			this._notifyIcon.BalloonTipIcon = icon;
			this._notifyIcon.ShowBalloonTip(timeout);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00006444 File Offset: 0x00004644
		private void ShowConfigForm(bool addNode)
		{
			if (this.configForm != null)
			{
				this.configForm.Activate();
				return;
			}
			this.configForm = new ConfigForm(this.controller, this.updateChecker, addNode ? -1 : -2);
			this.configForm.Show();
			this.configForm.Activate();
			this.configForm.BringToFront();
			this.configForm.FormClosed += new FormClosedEventHandler(this.configForm_FormClosed);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00006684 File Offset: 0x00004884
		private void ShowFirstTimeBalloon()
		{
			if (this._isFirstRun)
			{
				this._notifyIcon.BalloonTipTitle = I18N.GetString("ShadowsocksR is here");
				this._notifyIcon.BalloonTipText = I18N.GetString("You can turn on/off ShadowsocksR in the context menu");
				this._notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
				this._notifyIcon.ShowBalloonTip(0);
				this._isFirstRun = false;
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00006948 File Offset: 0x00004B48
		private void ShowLogItem_Click(object sender, EventArgs e)
		{
			try
			{
				Process.Start("explorer.exe", Logging.LogFile);
				return;
			}
			catch
			{
			}
			try
			{
				string arguments = "/n,/select," + Logging.LogFile;
				Process.Start("explorer.exe", arguments);
			}
			catch
			{
				this._notifyIcon.BalloonTipTitle = "Show log failed";
				this._notifyIcon.BalloonTipText = "try open the 'temp' directory by yourself";
				this._notifyIcon.BalloonTipIcon = ToolTipIcon.Error;
				this._notifyIcon.ShowBalloonTip(0);
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00006528 File Offset: 0x00004728
		private void ShowServerLogForm()
		{
			if (this.serverLogForm != null)
			{
				this.serverLogForm.Activate();
				this.serverLogForm.Update();
				if (this.serverLogForm.WindowState == FormWindowState.Minimized)
				{
					this.serverLogForm.WindowState = FormWindowState.Normal;
					return;
				}
			}
			else
			{
				this.serverLogForm = new ServerLogForm(this.controller);
				this.serverLogForm.Show();
				this.serverLogForm.Activate();
				this.serverLogForm.BringToFront();
				this.serverLogForm.FormClosed += new FormClosedEventHandler(this.serverLogForm_FormClosed);
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000069E0 File Offset: 0x00004BE0
		private void ShowServerLogItem_Click(object sender, EventArgs e)
		{
			this.ShowServerLogForm();
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000064BC File Offset: 0x000046BC
		private void ShowSettingForm()
		{
			if (this.settingsForm != null)
			{
				this.settingsForm.Activate();
				return;
			}
			this.settingsForm = new SettingsForm(this.controller);
			this.settingsForm.Show();
			this.settingsForm.Activate();
			this.settingsForm.BringToFront();
			this.settingsForm.FormClosed += new FormClosedEventHandler(this.settingsForm_FormClosed);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00006ECC File Offset: 0x000050CC
		private void splash_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.ShowConfigForm(true);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x0000596C File Offset: 0x00003B6C
		private void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			this.updateChecker.CheckUpdate(this.controller.GetConfiguration());
			if (this.timerDelayCheckUpdate != null)
			{
				this.timerDelayCheckUpdate.Elapsed -= new ElapsedEventHandler(this.timer_Elapsed);
				this.timerDelayCheckUpdate.Stop();
				this.timerDelayCheckUpdate = new System.Timers.Timer(7200000.0);
				this.timerDelayCheckUpdate.Elapsed += new ElapsedEventHandler(this.timer_Elapsed);
				this.timerDelayCheckUpdate.Start();
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000061C0 File Offset: 0x000043C0
		private void updateChecker_NewVersionFound(object sender, EventArgs e)
		{
			if (this.updateChecker.LatestVersionNumber == null || this.updateChecker.LatestVersionNumber.Length == 0)
			{
				Logging.Log(LogLevel.Error, "connect to update server error");
			}
			else
			{
				if (!this.UpdateItem.Visible)
				{
					this.ShowBalloonTip(string.Format(I18N.GetString("{0} {1} Update Found"), "ShadowsocksR", this.updateChecker.LatestVersionNumber), I18N.GetString("Click menu to download"), ToolTipIcon.Info, 5000);
					this._notifyIcon.BalloonTipClicked += new EventHandler(this.notifyIcon1_BalloonTipClicked);
					this.timerDelayCheckUpdate.Elapsed -= new ElapsedEventHandler(this.timer_Elapsed);
					this.timerDelayCheckUpdate.Stop();
					this.timerDelayCheckUpdate = null;
				}
				this.UpdateItem.Visible = true;
				this.UpdateItem.Text = string.Format(I18N.GetString("New version {0} {1} available"), "ShadowsocksR", this.updateChecker.LatestVersionNumber);
			}
			this._isFirstRun = false;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x000062BB File Offset: 0x000044BB
		private void UpdateItem_Clicked(object sender, EventArgs e)
		{
			Process.Start(this.updateChecker.LatestVersionURL);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000068E3 File Offset: 0x00004AE3
		private void UpdatePACFromCNIPListItem_Click(object sender, EventArgs e)
		{
			this.controller.UpdatePACFromOnlinePac("https://raw.githubusercontent.com/breakwa11/gfw_whitelist/master/ssr/ss_cnip.pac");
		}

		// Token: 0x06000049 RID: 73 RVA: 0x000068D1 File Offset: 0x00004AD1
		private void UpdatePACFromCNOnlyListItem_Click(object sender, EventArgs e)
		{
			this.controller.UpdatePACFromOnlinePac("https://raw.githubusercontent.com/breakwa11/gfw_whitelist/master/ssr/ss_r_white.pac");
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000068BF File Offset: 0x00004ABF
		private void UpdatePACFromCNWhiteListItem_Click(object sender, EventArgs e)
		{
			this.controller.UpdatePACFromOnlinePac("https://raw.githubusercontent.com/breakwa11/gfw_whitelist/master/ssr/ss_white.pac");
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000068A0 File Offset: 0x00004AA0
		private void UpdatePACFromGFWListItem_Click(object sender, EventArgs e)
		{
			this.controller.UpdatePACFromGFWList();
		}

		// Token: 0x06000047 RID: 71 RVA: 0x000068AD File Offset: 0x00004AAD
		private void UpdatePACFromLanIPListItem_Click(object sender, EventArgs e)
		{
			this.controller.UpdatePACFromOnlinePac("https://raw.githubusercontent.com/breakwa11/gfw_whitelist/master/ssr/ss_lanip.pac");
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00006384 File Offset: 0x00004584
		private void UpdateServersMenu()
		{
			Menu.MenuItemCollection menuItems = this.ServersItem.MenuItems;
			while (menuItems[0] != this.SeperatorItem)
			{
				menuItems.RemoveAt(0);
			}
			Configuration configuration = this.controller.GetConfiguration();
			for (int i = 0; i < configuration.configs.Count; i++)
			{
				MenuItem menuItem = new MenuItem(configuration.configs[i].FriendlyName());
				menuItem.Tag = i;
				menuItem.Click += new EventHandler(this.AServerItem_Click);
				menuItems.Add(i, menuItem);
			}
			if (configuration.index >= 0 && configuration.index < configuration.configs.Count)
			{
				menuItems[configuration.index].Checked = true;
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00005A1C File Offset: 0x00003C1C
		private void UpdateTrayIcon()
		{
			Graphics expr_0A = Graphics.FromHwnd(IntPtr.Zero);
			int num = (int)expr_0A.DpiX;
			expr_0A.Dispose();
			Bitmap bitmap;
			if (num < 97)
			{
				bitmap = Resources.ss16;
			}
			else if (num < 121)
			{
				bitmap = Resources.ss20;
			}
			else
			{
				bitmap = Resources.ss24;
			}
			Configuration configuration = this.controller.GetConfiguration();
			bool enabled = configuration.enabled;
			bool global = configuration.global;
			bool arg_A5_0 = configuration.random;
			double num2 = 1.0;
			double num3 = 1.0;
			double num4 = 1.0;
			double num5 = 1.0;
			if (!enabled)
			{
				num4 = 0.4;
			}
			else if (!global)
			{
				num5 = 0.4;
			}
			if (!arg_A5_0)
			{
				num3 = 0.4;
			}
			Bitmap bitmap2 = new Bitmap(bitmap);
			for (int i = 0; i < bitmap2.Width; i++)
			{
				for (int j = 0; j < bitmap2.Height; j++)
				{
					Color pixel = bitmap.GetPixel(i, j);
					bitmap2.SetPixel(i, j, Color.FromArgb((int)((byte)((double)pixel.A * num2)), (int)((byte)((double)pixel.R * num3)), (int)((byte)((double)pixel.G * num4)), (int)((byte)((double)pixel.B * num5))));
				}
			}
			bitmap = bitmap2;
			this._notifyIcon.Icon = Icon.FromHandle(bitmap.GetHicon());
			string text = "ShadowsocksR 3.8.4.2\n" + (enabled ? (I18N.GetString("System Proxy On: ") + (global ? I18N.GetString("Global") : I18N.GetString("PAC"))) : string.Format(I18N.GetString("Running: Port {0}"), configuration.localPort)) + "\n" + configuration.GetCurrentServer(null, false, false).FriendlyName();
			this._notifyIcon.Text = text.Substring(0, Math.Min(63, text.Length));
		}

		// Token: 0x0400004A RID: 74
		private ConfigForm configForm;

		// Token: 0x0400003F RID: 63
		private MenuItem ConfigItem;

		// Token: 0x0400003A RID: 58
		private ContextMenu contextMenu1;

		// Token: 0x04000037 RID: 55
		private ShadowsocksController controller;

		// Token: 0x04000045 RID: 69
		private MenuItem editGFWUserRuleItem;

		// Token: 0x04000043 RID: 67
		private MenuItem editLocalPACItem;

		// Token: 0x0400003C RID: 60
		private MenuItem enableItem;

		// Token: 0x04000041 RID: 65
		private MenuItem globalModeItem;

		// Token: 0x04000048 RID: 72
		private MenuItem httpWhiteListItem;

		// Token: 0x0400003D RID: 61
		private MenuItem modeItem;

		// Token: 0x04000042 RID: 66
		private MenuItem PACModeItem;

		// Token: 0x04000047 RID: 71
		private MenuItem sameHostForSameTargetItem;

		// Token: 0x04000046 RID: 70
		private MenuItem SelectRandomItem;

		// Token: 0x0400003E RID: 62
		private MenuItem SeperatorItem;

		// Token: 0x0400004C RID: 76
		private ServerLogForm serverLogForm;

		// Token: 0x04000040 RID: 64
		private MenuItem ServersItem;

		// Token: 0x0400004B RID: 75
		private SettingsForm settingsForm;

		// Token: 0x0400004E RID: 78
		private System.Timers.Timer timerDelayCheckUpdate;

		// Token: 0x04000038 RID: 56
		private UpdateChecker updateChecker;

		// Token: 0x04000044 RID: 68
		private MenuItem updateFromGFWListItem;

		// Token: 0x04000049 RID: 73
		private MenuItem UpdateItem;

		// Token: 0x0400003B RID: 59
		private bool _isFirstRun;

		// Token: 0x04000039 RID: 57
		private NotifyIcon _notifyIcon;

		// Token: 0x0400004D RID: 77
		private string _urlToOpen;
	}
}
