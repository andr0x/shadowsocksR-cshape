using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;
using Shadowsocks.Obfs;

namespace Shadowsocks.Controller
{
	// Token: 0x02000045 RID: 69
	public class Logging
	{
		// Token: 0x06000264 RID: 612 RVA: 0x00009AEF File Offset: 0x00007CEF
		public static void Debug(object o)
		{
		}

		// Token: 0x06000268 RID: 616 RVA: 0x000178C8 File Offset: 0x00015AC8
		public static void Log(LogLevel level, string s)
		{
			string[] array = new string[]
			{
				"Debug",
				"Info",
				"Warn",
				"Error",
				"Assert"
			};
			Console.WriteLine("[" + array[(int)level] + "]" + s);
		}

		// Token: 0x06000269 RID: 617 RVA: 0x00009AEF File Offset: 0x00007CEF
		public static void LogBin(LogLevel level, string info, byte[] data, int length)
		{
		}

		// Token: 0x06000267 RID: 615 RVA: 0x0001765C File Offset: 0x0001585C
		public static bool LogSocketException(string remarks, string server, Exception e)
		{
			if (DateTime.Now.ToString("yyyy-MM") != Logging.date)
			{
				Logging.OpenLogFile();
			}
			if (e is ObfsException)
			{
				ObfsException ex = (ObfsException)e;
				Logging.Log(LogLevel.Error, string.Concat(new string[]
				{
					"Proxy server [",
					remarks,
					"(",
					server,
					")] ",
					ex.Message
				}));
				return true;
			}
			if (e is NullReferenceException)
			{
				return true;
			}
			if (e is ObjectDisposedException)
			{
				return true;
			}
			if (e is SocketException)
			{
				SocketException ex2 = (SocketException)e;
				if (ex2.SocketErrorCode != SocketError.ConnectionAborted && ex2.SocketErrorCode != SocketError.ConnectionReset && ex2.SocketErrorCode != SocketError.NotConnected)
				{
					if (ex2.SocketErrorCode == (SocketError)(-2147467259))
					{
						return true;
					}
					if (ex2.ErrorCode == 11004)
					{
						Logging.Log(LogLevel.Warn, string.Concat(new string[]
						{
							"Proxy server [",
							remarks,
							"(",
							server,
							")] DNS lookup failed"
						}));
						return true;
					}
					if (ex2.SocketErrorCode == SocketError.HostNotFound)
					{
						Logging.Log(LogLevel.Warn, string.Concat(new string[]
						{
							"Proxy server [",
							remarks,
							"(",
							server,
							")] Host not found"
						}));
						return true;
					}
					if (ex2.SocketErrorCode == SocketError.ConnectionRefused)
					{
						Logging.Log(LogLevel.Warn, string.Concat(new string[]
						{
							"Proxy server [",
							remarks,
							"(",
							server,
							")] connection refused"
						}));
						return true;
					}
					if (ex2.SocketErrorCode == SocketError.NetworkUnreachable)
					{
						Logging.Log(LogLevel.Warn, string.Concat(new string[]
						{
							"Proxy server [",
							remarks,
							"(",
							server,
							")] network unreachable"
						}));
						return true;
					}
					if (ex2.SocketErrorCode == SocketError.TimedOut)
					{
						return true;
					}
					if (ex2.SocketErrorCode == SocketError.Shutdown)
					{
						return true;
					}
					Logging.Log(LogLevel.Info, string.Concat(new string[]
					{
						"Proxy server [",
						remarks,
						"(",
						server,
						")] ",
						Convert.ToString(ex2.SocketErrorCode),
						":",
						ex2.Message
					}));
					Console.WriteLine(Logging.ToString(new StackTrace().GetFrames()));
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000266 RID: 614 RVA: 0x000175A4 File Offset: 0x000157A4
		public static void LogUsefulException(Exception e)
		{
			if (DateTime.Now.ToString("yyyy-MM") != Logging.date)
			{
				Logging.OpenLogFile();
			}
			if (e is SocketException)
			{
				SocketException ex = (SocketException)e;
				if (ex.SocketErrorCode != SocketError.ConnectionAborted && ex.SocketErrorCode != SocketError.ConnectionReset && ex.SocketErrorCode != SocketError.NotConnected && ex.SocketErrorCode != (SocketError)(-2147467259) && ex.SocketErrorCode != SocketError.Shutdown)
				{
					Console.WriteLine(e);
					Console.WriteLine(Logging.ToString(new StackTrace().GetFrames()));
					return;
				}
			}
			else
			{
				Console.WriteLine(e);
				Console.WriteLine(Logging.ToString(new StackTrace().GetFrames()));
			}
		}

		// Token: 0x06000263 RID: 611 RVA: 0x000174A8 File Offset: 0x000156A8
		public static bool OpenLogFile()
		{
			bool result;
			try
			{
				string text = Path.Combine(Application.StartupPath, "temp");
				Logging.LogFilePath = text;
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				Logging.date = DateTime.Now.ToString("yyyy-MM");
				Logging.LogFileName = "shadowsocks_" + Logging.date + ".log";
				Logging.LogFile = Path.Combine(text, Logging.LogFileName);
				StreamWriterWithTimestamp expr_75 = new StreamWriterWithTimestamp(new FileStream(Logging.LogFile, FileMode.Append));
				expr_75.AutoFlush = true;
				Console.SetOut(expr_75);
				Console.SetError(expr_75);
				result = true;
			}
			catch (IOException arg_8B_0)
			{
				Console.WriteLine(arg_8B_0.ToString());
				result = false;
			}
			return result;
		}

		// Token: 0x06000265 RID: 613 RVA: 0x00017560 File Offset: 0x00015760
		private static string ToString(StackFrame[] stacks)
		{
			string text = string.Empty;
			for (int i = 0; i < stacks.Length; i++)
			{
				StackFrame stackFrame = stacks[i];
				text += string.Format("{0}\r\n", stackFrame.GetMethod().ToString());
			}
			return text;
		}

		// Token: 0x040001DF RID: 479
		protected static string date;

		// Token: 0x040001DC RID: 476
		public static string LogFile;

		// Token: 0x040001DE RID: 478
		public static string LogFileName;

		// Token: 0x040001DD RID: 477
		public static string LogFilePath;
	}
}
