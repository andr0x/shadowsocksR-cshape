using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;
using Shadowsocks.Obfs;

namespace Shadowsocks.Controller
{
	// Token: 0x02000043 RID: 67
	public class Logging
	{
		// Token: 0x0600024F RID: 591 RVA: 0x00009C9F File Offset: 0x00007E9F
		public static void Debug(object o)
		{
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0001704C File Offset: 0x0001524C
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

		// Token: 0x06000254 RID: 596 RVA: 0x00009C9F File Offset: 0x00007E9F
		public static void LogBin(LogLevel level, string info, byte[] data, int length)
		{
		}

		// Token: 0x06000252 RID: 594 RVA: 0x00016DE0 File Offset: 0x00014FE0
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

		// Token: 0x06000251 RID: 593 RVA: 0x00016D28 File Offset: 0x00014F28
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

		// Token: 0x0600024E RID: 590 RVA: 0x00016C2C File Offset: 0x00014E2C
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

		// Token: 0x06000250 RID: 592 RVA: 0x00016CE4 File Offset: 0x00014EE4
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

		// Token: 0x040001D5 RID: 469
		protected static string date;

		// Token: 0x040001D2 RID: 466
		public static string LogFile;

		// Token: 0x040001D4 RID: 468
		public static string LogFileName;

		// Token: 0x040001D3 RID: 467
		public static string LogFilePath;
	}
}
