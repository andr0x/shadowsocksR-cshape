using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Shadowsocks.Model;
using Shadowsocks.Util;
using SimpleJson;

namespace Shadowsocks.Controller
{
	// Token: 0x0200003B RID: 59
	internal class APIServer : Listener.Service
	{
		// Token: 0x0600021D RID: 541 RVA: 0x00014CE9 File Offset: 0x00012EE9
		public APIServer(ShadowsocksController controller, Configuration config)
		{
			this._controller = controller;
			this._config = config;
		}

		// Token: 0x0600021F RID: 543 RVA: 0x00014EA0 File Offset: 0x000130A0
		private bool CheckEnd(string request)
		{
			int num = request.IndexOf("\r\n\r\n");
			if (request.StartsWith("POST "))
			{
				if (num > 0)
				{
					string arg_3E_0 = request.Substring(0, num);
					string text = request.Substring(num + 4);
					string[] array = arg_3E_0.Split(new string[]
					{
						"\r\n"
					}, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < array.Length; i++)
					{
						string text2 = array[i];
						if (text2.StartsWith("Content-Length: "))
						{
							try
							{
								if (int.Parse(text2.Substring("Content-Length: ".Length)) <= text.Length)
								{
									return true;
								}
							}
							catch (FormatException)
							{
								break;
							}
						}
					}
					return false;
				}
			}
			else if (num + 4 == request.Length)
			{
				return true;
			}
			return false;
		}

		// Token: 0x0600021E RID: 542 RVA: 0x00014D10 File Offset: 0x00012F10
		public bool Handle(byte[] firstPacket, int length, Socket socket)
		{
			bool result;
			try
			{
				string @string = Encoding.UTF8.GetString(firstPacket, 0, length);
				string[] arg_3D_0 = @string.Split(new string[]
				{
					"\r\n",
					"\r",
					"\n"
				}, StringSplitOptions.RemoveEmptyEntries);
				bool flag = false;
				bool flag2 = false;
				string[] array = arg_3D_0;
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					string[] array2 = text.Split(new char[]
					{
						':'
					}, 2);
					if (array2.Length == 2)
					{
						if (array2[0] == "Host" && array2[1].Trim() == ((IPEndPoint)socket.LocalEndPoint).ToString())
						{
							flag = true;
						}
					}
					else if (array2.Length == 1 && text.IndexOf("api?") > 0)
					{
						string expr_B9 = text;
						string text2 = expr_B9.Substring(expr_B9.IndexOf("api?") + 4);
						if (text.IndexOf("GET ") == 0 || text.IndexOf("POST ") == 0)
						{
							flag2 = true;
							text2 = text2.Substring(0, text2.IndexOf(" "));
						}
					}
				}
				if (flag & flag2)
				{
					this._local = socket;
					if (this.CheckEnd(@string))
					{
						this.process(@string);
					}
					else
					{
						this.connection_request = @string;
						socket.BeginReceive(this.connetionRecvBuffer, 0, 16384, SocketFlags.None, new AsyncCallback(this.HttpHandshakeRecv), null);
					}
					result = true;
				}
				else
				{
					result = false;
				}
			}
			catch (ArgumentException)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000220 RID: 544 RVA: 0x00014F60 File Offset: 0x00013160
		private void HttpHandshakeRecv(IAsyncResult ar)
		{
			try
			{
				int num = this._local.EndReceive(ar);
				if (num > 0)
				{
					string @string = Encoding.UTF8.GetString(this.connetionRecvBuffer, 0, num);
					this.connection_request += @string;
					if (this.CheckEnd(this.connection_request))
					{
						this.process(this.connection_request);
					}
					else
					{
						this._local.BeginReceive(this.connetionRecvBuffer, 0, 16384, SocketFlags.None, new AsyncCallback(this.HttpHandshakeRecv), null);
					}
				}
				else
				{
					Console.WriteLine("APIServer: failed to recv data in HttpHandshakeRecv");
					this._local.Shutdown(SocketShutdown.Both);
					this._local.Close();
				}
			}
			catch (Exception arg_9E_0)
			{
				Logging.LogUsefulException(arg_9E_0);
				try
				{
					this._local.Shutdown(SocketShutdown.Both);
					this._local.Close();
				}
				catch
				{
				}
			}
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0001504C File Offset: 0x0001324C
		protected string process(string request)
		{
			string text = request.Substring(0, request.IndexOf("\r\n"));
			string expr_14 = text;
			text = expr_14.Substring(expr_14.IndexOf("api?") + 4);
			text = text.Substring(0, text.IndexOf(" "));
			string[] arg_51_0 = text.Split(new char[]
			{
				'&'
			});
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string[] array = arg_51_0;
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i];
				if (text2.IndexOf('=') > 0)
				{
					int num = text2.IndexOf('=');
					string key = text2.Substring(0, num);
					string value = text2.Substring(num + 1);
					dictionary[key] = value;
				}
			}
			if (request.IndexOf("POST ") == 0)
			{
				array = request.Substring(request.IndexOf("\r\n\r\n") + 4).Split(new char[]
				{
					'&'
				});
				for (int i = 0; i < array.Length; i++)
				{
					string text3 = array[i];
					if (text3.IndexOf('=') > 0)
					{
						int num2 = text3.IndexOf('=');
						string key2 = text3.Substring(0, num2);
						string str = text3.Substring(num2 + 1);
						dictionary[key2] = Utils.urlDecode(str);
					}
				}
			}
			if (dictionary.ContainsKey("token") && dictionary.ContainsKey("app") && this._config.token.ContainsKey(dictionary["app"]) && this._config.token[dictionary["app"]] == dictionary["token"] && dictionary.ContainsKey("action"))
			{
				if (dictionary["action"] == "statistics")
				{
					Configuration config = this._config;
					ServerSpeedLogShow[] array2 = new ServerSpeedLogShow[config.configs.Count];
					Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
					for (int j = 0; j < config.configs.Count; j++)
					{
						array2[j] = config.configs[j].ServerSpeedLog().Translate();
						dictionary2[config.configs[j].id] = array2[j];
					}
					string text4 = SimpleJson.SimpleJson.SerializeObject(dictionary2);
					string s = string.Format("HTTP/1.1 200 OK\r\nServer: ShadowsocksR\r\nContent-Type: text/plain\r\nContent-Length: {0}\r\nConnection: Close\r\n\r\n", Encoding.UTF8.GetBytes(text4).Length) + text4;
					byte[] bytes = Encoding.UTF8.GetBytes(s);
					this._local.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, new AsyncCallback(this.SendCallback), this._local);
					return "";
				}
				if (dictionary["action"] == "config")
				{
					if (dictionary.ContainsKey("config"))
					{
						string text5 = "";
						string arg = "200 OK";
						if (!this._controller.SaveServersConfig(dictionary["config"]))
						{
							arg = "403 Forbid";
						}
						string s2 = string.Format("HTTP/1.1 {0}\r\nServer: ShadowsocksR\r\nContent-Type: text/plain\r\nContent-Length: {1}\r\nConnection: Close\r\n\r\n", arg, Encoding.UTF8.GetBytes(text5).Length) + text5;
						byte[] bytes2 = Encoding.UTF8.GetBytes(s2);
						this._local.BeginSend(bytes2, 0, bytes2.Length, SocketFlags.None, new AsyncCallback(this.SendCallback), this._local);
						return "";
					}
					Dictionary<string, string> token = this._config.token;
					this._config.token = new Dictionary<string, string>();
					string text6 = SimpleJson.SimpleJson.SerializeObject(this._config);
					this._config.token = token;
					string s3 = string.Format("HTTP/1.1 200 OK\r\nServer: ShadowsocksR\r\nContent-Type: text/plain\r\nContent-Length: {0}\r\nConnection: Close\r\n\r\n", Encoding.UTF8.GetBytes(text6).Length) + text6;
					byte[] bytes3 = Encoding.UTF8.GetBytes(s3);
					this._local.BeginSend(bytes3, 0, bytes3.Length, SocketFlags.None, new AsyncCallback(this.SendCallback), this._local);
					return "";
				}
			}
			byte[] bytes4 = Encoding.UTF8.GetBytes("");
			this._local.BeginSend(bytes4, 0, bytes4.Length, SocketFlags.None, new AsyncCallback(this.SendCallback), this._local);
			return "";
		}

		// Token: 0x06000222 RID: 546 RVA: 0x00015484 File Offset: 0x00013684
		private void SendCallback(IAsyncResult ar)
		{
			Socket socket = (Socket)ar.AsyncState;
			try
			{
				socket.Shutdown(SocketShutdown.Both);
				socket.Close();
			}
			catch
			{
			}
		}

		// Token: 0x040001AA RID: 426
		private string connection_request;

		// Token: 0x040001A9 RID: 425
		private byte[] connetionRecvBuffer = new byte[16384];

		// Token: 0x040001A8 RID: 424
		public const int RecvSize = 16384;

		// Token: 0x040001A7 RID: 423
		private Configuration _config;

		// Token: 0x040001A6 RID: 422
		private ShadowsocksController _controller;

		// Token: 0x040001AB RID: 427
		private Socket _local;
	}
}
