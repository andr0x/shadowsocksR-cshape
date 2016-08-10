using System;
using System.Collections.Generic;

namespace Shadowsocks.Model
{
	// Token: 0x0200002F RID: 47
	public class ServerSpeedLog
	{
		// Token: 0x0600018F RID: 399 RVA: 0x0001256F File Offset: 0x0001076F
		public ServerSpeedLog()
		{
		}

		// Token: 0x06000190 RID: 400 RVA: 0x00012582 File Offset: 0x00010782
		public ServerSpeedLog(long upload, long download)
		{
			this.transUpload = upload;
			this.transDownload = download;
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0001365C File Offset: 0x0001185C
		public void AddConnectTime(int millisecond)
		{
			lock (this)
			{
				if (this.connectTime == null)
				{
					this.connectTime = new List<int>();
				}
				this.connectTime.Add(millisecond);
				this.sumConnectTime += millisecond;
				while (this.connectTime.Count > 20)
				{
					this.sumConnectTime -= this.connectTime[0];
					this.connectTime.RemoveAt(0);
				}
			}
		}

		// Token: 0x0600019E RID: 414 RVA: 0x00012F64 File Offset: 0x00011164
		public void AddConnectTimes()
		{
			lock (this)
			{
				this.totalConnectTimes += 1L;
			}
		}

		// Token: 0x0600019F RID: 415 RVA: 0x00012FA8 File Offset: 0x000111A8
		public void AddDisconnectTimes()
		{
			lock (this)
			{
				this.totalDisconnectTimes += 1L;
			}
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x00013438 File Offset: 0x00011638
		public void AddDownloadBytes(long bytes)
		{
			lock (this)
			{
				this.transDownload += bytes;
				if (this.downTransLog == null)
				{
					this.downTransLog = new List<TransLog>();
				}
				List<TransLog> list = this.downTransLog;
				if (list.Count > 0)
				{
					DateTime arg_55_0 = DateTime.Now;
					List<TransLog> expr_43 = list;
					if ((arg_55_0 - expr_43[expr_43.Count - 1].recvTime).TotalMilliseconds < 100.0)
					{
						List<TransLog> expr_6E = list;
						expr_6E[expr_6E.Count - 1].size += (int)bytes;
						return;
					}
				}
				list.Add(new TransLog((int)bytes, DateTime.Now));
				while (list.Count > 0 && DateTime.Now > list[0].recvTime.AddSeconds(2.0))
				{
					list.RemoveAt(0);
				}
			}
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x00013538 File Offset: 0x00011738
		public void AddDownloadRawBytes(long bytes)
		{
			lock (this)
			{
				this.transDownloadRaw += bytes;
			}
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x00013240 File Offset: 0x00011440
		public void AddErrorDecodeTimes()
		{
			lock (this)
			{
				this.errorDecodeTimes += 1L;
				this.errorContinurousTimes += 1L;
				this.errList.AddLast(new ErrorLog(3));
				if (this.lastError != 3)
				{
					this.lastError = 3;
				}
				this.Sweep();
			}
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x000132BC File Offset: 0x000114BC
		public void AddErrorEmptyTimes()
		{
			lock (this)
			{
				this.errorEmptyTimes += 1L;
				this.errorContinurousTimes += 1L;
				this.errList.AddLast(new ErrorLog(0));
				if (this.lastError != 4)
				{
					this.lastError = 4;
				}
				this.Sweep();
			}
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x00013148 File Offset: 0x00011348
		public void AddErrorTimes()
		{
			lock (this)
			{
				this.errorConnectTimes += 1L;
				this.errorContinurousTimes += 1L;
				this.errList.AddLast(new ErrorLog(1));
				if (this.lastError != 1)
				{
					this.lastError = 1;
				}
				this.Sweep();
			}
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x000130F0 File Offset: 0x000112F0
		public void AddNoErrorTimes()
		{
			lock (this)
			{
				this.errList.AddLast(new ErrorLog(0));
				this.errorEmptyTimes = 0L;
				this.Sweep();
			}
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x000131C4 File Offset: 0x000113C4
		public void AddTimeoutTimes()
		{
			lock (this)
			{
				this.errorTimeoutTimes += 1L;
				this.errorContinurousTimes += 1L;
				this.errList.AddLast(new ErrorLog(2));
				if (this.lastError != 2)
				{
					this.lastError = 2;
				}
				this.Sweep();
			}
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x00013338 File Offset: 0x00011538
		public void AddUploadBytes(long bytes)
		{
			lock (this)
			{
				this.transUpload += bytes;
				if (this.upTransLog == null)
				{
					this.upTransLog = new List<TransLog>();
				}
				List<TransLog> list = this.upTransLog;
				if (list.Count > 0)
				{
					DateTime arg_55_0 = DateTime.Now;
					List<TransLog> expr_43 = list;
					if ((arg_55_0 - expr_43[expr_43.Count - 1].recvTime).TotalMilliseconds < 100.0)
					{
						List<TransLog> expr_6E = list;
						expr_6E[expr_6E.Count - 1].size += (int)bytes;
						return;
					}
				}
				list.Add(new TransLog((int)bytes, DateTime.Now));
				while (list.Count > 0 && DateTime.Now > list[0].recvTime.AddSeconds(2.0))
				{
					list.RemoveAt(0);
				}
			}
		}

		// Token: 0x0600019D RID: 413 RVA: 0x00012E98 File Offset: 0x00011098
		public void Clear()
		{
			lock (this)
			{
				if (this.totalConnectTimes > this.totalDisconnectTimes)
				{
					this.totalConnectTimes -= this.totalDisconnectTimes;
				}
				else
				{
					this.totalConnectTimes = 0L;
				}
				this.totalDisconnectTimes = 0L;
				this.errorConnectTimes = 0L;
				this.errorTimeoutTimes = 0L;
				this.errorDecodeTimes = 0L;
				this.errorEmptyTimes = 0L;
				this.errList.Clear();
				this.lastError = 0;
				this.errorContinurousTimes = 0L;
				this.transUpload = 0L;
				this.transDownload = 0L;
				this.transDownloadRaw = 0L;
				this.maxTransDownload = 0L;
				this.maxTransUpload = 0L;
			}
		}

		// Token: 0x0600019B RID: 411 RVA: 0x00012DAC File Offset: 0x00010FAC
		public void ClearError()
		{
			lock (this)
			{
				if (this.totalConnectTimes > this.totalDisconnectTimes)
				{
					this.totalConnectTimes -= this.totalDisconnectTimes;
				}
				else
				{
					this.totalConnectTimes = 0L;
				}
				this.totalDisconnectTimes = 0L;
				this.errorConnectTimes = 0L;
				this.errorTimeoutTimes = 0L;
				this.errorDecodeTimes = 0L;
				this.errorEmptyTimes = 0L;
				this.errList.Clear();
				this.lastError = 0;
				this.errorContinurousTimes = 0L;
			}
		}

		// Token: 0x0600019C RID: 412 RVA: 0x00012E50 File Offset: 0x00011050
		public void ClearMaxSpeed()
		{
			lock (this)
			{
				this.maxTransDownload = 0L;
				this.maxTransUpload = 0L;
			}
		}

		// Token: 0x060001AA RID: 426 RVA: 0x000135D0 File Offset: 0x000117D0
		public void ResetContinurousTimes()
		{
			lock (this)
			{
				this.lastError = 0;
				this.errorEmptyTimes = 0L;
				this.errorContinurousTimes = 0L;
			}
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0001361C File Offset: 0x0001181C
		public void ResetEmptyTimes()
		{
			lock (this)
			{
				this.errorEmptyTimes = 0L;
			}
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x0001357C File Offset: 0x0001177C
		public void ResetErrorDecodeTimes()
		{
			lock (this)
			{
				this.lastError = 0;
				this.errorDecodeTimes = 0L;
				this.errorEmptyTimes = 0L;
				this.errorContinurousTimes = 0L;
			}
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x00012FEC File Offset: 0x000111EC
		protected void Sweep()
		{
			while (this.errList.Count > 0 && ((DateTime.Now - this.errList.First.Value.time).TotalMinutes >= 30.0 || this.errList.Count >= 100))
			{
				int errno = this.errList.First.Value.errno;
				this.errList.RemoveFirst();
				if (errno == 1)
				{
					if (this.errorConnectTimes > 0L)
					{
						this.errorConnectTimes -= 1L;
					}
				}
				else if (errno == 2)
				{
					if (this.errorTimeoutTimes > 0L)
					{
						this.errorTimeoutTimes -= 1L;
					}
				}
				else if (errno == 3)
				{
					if (this.errorDecodeTimes > 0L)
					{
						this.errorDecodeTimes -= 1L;
					}
				}
				else if (errno == 4 && this.errorEmptyTimes > 0L)
				{
					this.errorEmptyTimes -= 1L;
				}
			}
		}

		// Token: 0x06000191 RID: 401 RVA: 0x000125A4 File Offset: 0x000107A4
		public ServerSpeedLogShow Translate()
		{
			ServerSpeedLogShow serverSpeedLogShow = new ServerSpeedLogShow();
			lock (this)
			{
				this.Sweep();
				serverSpeedLogShow.avgDownloadBytes = this.AvgDownloadBytes;
				serverSpeedLogShow.avgUploadBytes = this.AvgUploadBytes;
				serverSpeedLogShow.avgConnectTime = this.AvgConnectTime;
				serverSpeedLogShow.maxDownloadBytes = this.maxTransDownload;
				serverSpeedLogShow.maxUploadBytes = this.maxTransUpload;
				serverSpeedLogShow.totalConnectTimes = this.totalConnectTimes;
				serverSpeedLogShow.totalDisconnectTimes = this.totalDisconnectTimes;
				serverSpeedLogShow.errorConnectTimes = this.errorConnectTimes;
				serverSpeedLogShow.errorTimeoutTimes = this.errorTimeoutTimes;
				serverSpeedLogShow.errorDecodeTimes = this.errorDecodeTimes;
				serverSpeedLogShow.errorEmptyTimes = this.errorEmptyTimes;
				serverSpeedLogShow.errorLogTimes = (long)this.errList.Count;
				serverSpeedLogShow.errorContinurousTimes = this.errorContinurousTimes;
				serverSpeedLogShow.totalUploadBytes = this.transUpload;
				serverSpeedLogShow.totalDownloadBytes = this.transDownload;
				serverSpeedLogShow.totalDownloadRawBytes = this.transDownloadRaw;
				serverSpeedLogShow.sumConnectTime = this.sumConnectTime;
			}
			return serverSpeedLogShow;
		}

		// Token: 0x1700001A RID: 26
		public long AvgConnectTime
		{
			// Token: 0x0600019A RID: 410 RVA: 0x00012CA0 File Offset: 0x00010EA0
			get
			{
				long result;
				lock (this)
				{
					if (this.connectTime != null)
					{
						if (this.connectTime.Count > 4)
						{
							List<int> list = new List<int>();
							foreach (int current in this.connectTime)
							{
								list.Add(current);
							}
							list.Sort();
							int num = 0;
							for (int i = 0; i < this.connectTime.Count / 2; i++)
							{
								num += list[i];
							}
							result = (long)(num / (this.connectTime.Count / 2));
							return result;
						}
						if (this.connectTime.Count > 0)
						{
							result = (long)(this.sumConnectTime / this.connectTime.Count);
							return result;
						}
					}
					result = -1L;
				}
				return result;
			}
		}

		// Token: 0x17000018 RID: 24
		public long AvgDownloadBytes
		{
			// Token: 0x06000198 RID: 408 RVA: 0x00012838 File Offset: 0x00010A38
			get
			{
				List<TransLog> list;
				lock (this)
				{
					if (this.downTransLog == null)
					{
						long result = 0L;
						return result;
					}
					list = new List<TransLog>();
					for (int i = 1; i < this.downTransLog.Count; i++)
					{
						list.Add(this.downTransLog[i]);
					}
				}
				long num = 0L;
				double num2 = 0.0;
				if (list.Count > 0)
				{
					DateTime arg_99_0 = DateTime.Now;
					List<TransLog> expr_79 = list;
					if (arg_99_0 > expr_79[expr_79.Count - 1].recvTime.AddSeconds(2.0))
					{
						return 0L;
					}
				}
				for (int j = 1; j < list.Count; j++)
				{
					num += (long)list[j].size;
				}
				long num3 = 0L;
				int num4 = 0;
				for (int k = 0; k < list.Count; k++)
				{
					num3 += (long)list[k].size;
					while (num4 + 100 <= k && (list[k].recvTime - list[num4].recvTime).TotalSeconds > 1.0)
					{
						long num5 = (long)((double)(num3 - (long)list[num4].size) / (list[k].recvTime - list[num4].recvTime).TotalSeconds);
						if (num5 > this.maxTransDownload)
						{
							this.maxTransDownload = num5;
						}
						num3 -= (long)list[num4].size;
						num4++;
					}
				}
				if (list.Count > 1)
				{
					List<TransLog> expr_1B7 = list;
					num2 = (expr_1B7[expr_1B7.Count - 1].recvTime - list[0].recvTime).TotalSeconds;
				}
				if (num2 > 0.1)
				{
					long num6 = (long)((double)num / num2);
					if (num6 > this.maxTransDownload)
					{
						this.maxTransDownload = num6;
					}
					return num6;
				}
				return 0L;
			}
		}

		// Token: 0x17000019 RID: 25
		public long AvgUploadBytes
		{
			// Token: 0x06000199 RID: 409 RVA: 0x00012A6C File Offset: 0x00010C6C
			get
			{
				List<TransLog> list;
				lock (this)
				{
					if (this.upTransLog == null)
					{
						long result = 0L;
						return result;
					}
					list = new List<TransLog>();
					for (int i = 1; i < this.upTransLog.Count; i++)
					{
						list.Add(this.upTransLog[i]);
					}
				}
				long num = 0L;
				double num2 = 0.0;
				if (list.Count > 0)
				{
					DateTime arg_99_0 = DateTime.Now;
					List<TransLog> expr_79 = list;
					if (arg_99_0 > expr_79[expr_79.Count - 1].recvTime.AddSeconds(2.0))
					{
						return 0L;
					}
				}
				for (int j = 1; j < list.Count; j++)
				{
					num += (long)list[j].size;
				}
				long num3 = 0L;
				int num4 = 0;
				for (int k = 0; k < list.Count; k++)
				{
					num3 += (long)list[k].size;
					while (num4 + 100 <= k && (list[k].recvTime - list[num4].recvTime).TotalSeconds > 1.0)
					{
						long num5 = (long)((double)(num3 - (long)list[num4].size) / (list[k].recvTime - list[num4].recvTime).TotalSeconds);
						if (num5 > this.maxTransUpload)
						{
							this.maxTransUpload = num5;
						}
						num3 -= (long)list[num4].size;
						num4++;
					}
				}
				if (list.Count > 1)
				{
					List<TransLog> expr_1B7 = list;
					num2 = (expr_1B7[expr_1B7.Count - 1].recvTime - list[0].recvTime).TotalSeconds;
				}
				if (num2 > 0.1)
				{
					long num6 = (long)((double)num / num2);
					if (num6 > this.maxTransUpload)
					{
						this.maxTransUpload = num6;
					}
					return num6;
				}
				return 0L;
			}
		}

		// Token: 0x17000014 RID: 20
		public long ErrorConnectTimes
		{
			// Token: 0x06000194 RID: 404 RVA: 0x00012738 File Offset: 0x00010938
			get
			{
				long result;
				lock (this)
				{
					result = this.errorConnectTimes;
				}
				return result;
			}
		}

		// Token: 0x17000017 RID: 23
		public long ErrorContinurousTimes
		{
			// Token: 0x06000197 RID: 407 RVA: 0x000127F8 File Offset: 0x000109F8
			get
			{
				long result;
				lock (this)
				{
					result = this.errorContinurousTimes;
				}
				return result;
			}
		}

		// Token: 0x17000016 RID: 22
		public long ErrorEncryptTimes
		{
			// Token: 0x06000196 RID: 406 RVA: 0x000127B8 File Offset: 0x000109B8
			get
			{
				long result;
				lock (this)
				{
					result = this.errorDecodeTimes;
				}
				return result;
			}
		}

		// Token: 0x17000015 RID: 21
		public long ErrorTimeoutTimes
		{
			// Token: 0x06000195 RID: 405 RVA: 0x00012778 File Offset: 0x00010978
			get
			{
				long result;
				lock (this)
				{
					result = this.errorTimeoutTimes;
				}
				return result;
			}
		}

		// Token: 0x17000012 RID: 18
		public long TotalConnectTimes
		{
			// Token: 0x06000192 RID: 402 RVA: 0x000126B8 File Offset: 0x000108B8
			get
			{
				long result;
				lock (this)
				{
					result = this.totalConnectTimes;
				}
				return result;
			}
		}

		// Token: 0x17000013 RID: 19
		public long TotalDisconnectTimes
		{
			// Token: 0x06000193 RID: 403 RVA: 0x000126F8 File Offset: 0x000108F8
			get
			{
				long result;
				lock (this)
				{
					result = this.totalDisconnectTimes;
				}
				return result;
			}
		}

		// Token: 0x0400016C RID: 364
		private const int avgTime = 2;

		// Token: 0x04000169 RID: 361
		private List<int> connectTime;

		// Token: 0x04000165 RID: 357
		private List<TransLog> downTransLog;

		// Token: 0x0400016B RID: 363
		private LinkedList<ErrorLog> errList = new LinkedList<ErrorLog>();

		// Token: 0x0400015C RID: 348
		private long errorConnectTimes;

		// Token: 0x04000161 RID: 353
		private long errorContinurousTimes;

		// Token: 0x0400015E RID: 350
		private long errorDecodeTimes;

		// Token: 0x0400015F RID: 351
		private long errorEmptyTimes;

		// Token: 0x0400015D RID: 349
		private long errorTimeoutTimes;

		// Token: 0x04000160 RID: 352
		private int lastError;

		// Token: 0x04000167 RID: 359
		private long maxTransDownload;

		// Token: 0x04000168 RID: 360
		private long maxTransUpload;

		// Token: 0x0400016A RID: 362
		private int sumConnectTime;

		// Token: 0x0400015A RID: 346
		private long totalConnectTimes;

		// Token: 0x0400015B RID: 347
		private long totalDisconnectTimes;

		// Token: 0x04000163 RID: 355
		private long transDownload;

		// Token: 0x04000164 RID: 356
		private long transDownloadRaw;

		// Token: 0x04000162 RID: 354
		private long transUpload;

		// Token: 0x04000166 RID: 358
		private List<TransLog> upTransLog;
	}
}
