using System;
using System.Collections.Generic;

namespace Shadowsocks.Model
{
	// Token: 0x0200002D RID: 45
	public class ServerSpeedLog
	{
		// Token: 0x0600017B RID: 379 RVA: 0x00011BE6 File Offset: 0x0000FDE6
		public ServerSpeedLog()
		{
		}

		// Token: 0x0600017C RID: 380 RVA: 0x00011BF9 File Offset: 0x0000FDF9
		public ServerSpeedLog(long upload, long download)
		{
			this.transUpload = upload;
			this.transDownload = download;
		}

		// Token: 0x06000198 RID: 408 RVA: 0x00012DEC File Offset: 0x00010FEC
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

		// Token: 0x0600018A RID: 394 RVA: 0x000125F4 File Offset: 0x000107F4
		public void AddConnectTimes()
		{
			lock (this)
			{
				this.totalConnectTimes += 1L;
			}
		}

		// Token: 0x0600018B RID: 395 RVA: 0x00012638 File Offset: 0x00010838
		public void AddDisconnectTimes()
		{
			lock (this)
			{
				this.totalDisconnectTimes += 1L;
			}
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00012B48 File Offset: 0x00010D48
		public void AddDownloadBytes(long bytes)
		{
			DateTime now = DateTime.Now;
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
					DateTime arg_57_0 = now;
					List<TransLog> expr_45 = list;
					if ((arg_57_0 - expr_45[expr_45.Count - 1].recvTime).TotalMilliseconds < 100.0)
					{
						List<TransLog> expr_71 = list;
						expr_71[expr_71.Count - 1].size += (int)bytes;
						List<TransLog> expr_8D = list;
						expr_8D[expr_8D.Count - 1].endTime = now;
						return;
					}
				}
				if (list.Count > 0)
				{
					int i;
					for (i = list.Count - 1; i >= 0; i--)
					{
						if (!(list[i].recvTime > now) || i <= 0)
						{
							list.Insert(i + 1, new TransLog((int)bytes, now));
							break;
						}
					}
					if (i == -1)
					{
						list.Insert(0, new TransLog((int)bytes, now));
					}
				}
				else
				{
					list.Add(new TransLog((int)bytes, now));
				}
				while (list.Count > 0 && now > list[0].recvTime.AddSeconds(3.0))
				{
					list.RemoveAt(0);
				}
			}
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00012CC8 File Offset: 0x00010EC8
		public void AddDownloadRawBytes(long bytes)
		{
			lock (this)
			{
				this.transDownloadRaw += bytes;
			}
		}

		// Token: 0x06000190 RID: 400 RVA: 0x000128D0 File Offset: 0x00010AD0
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

		// Token: 0x06000191 RID: 401 RVA: 0x0001294C File Offset: 0x00010B4C
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

		// Token: 0x0600018E RID: 398 RVA: 0x000127D8 File Offset: 0x000109D8
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

		// Token: 0x0600018D RID: 397 RVA: 0x00012780 File Offset: 0x00010980
		public void AddNoErrorTimes()
		{
			lock (this)
			{
				this.errList.AddLast(new ErrorLog(0));
				this.errorEmptyTimes = 0L;
				this.Sweep();
			}
		}

		// Token: 0x0600018F RID: 399 RVA: 0x00012854 File Offset: 0x00010A54
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

		// Token: 0x06000192 RID: 402 RVA: 0x000129C8 File Offset: 0x00010BC8
		public void AddUploadBytes(long bytes)
		{
			DateTime now = DateTime.Now;
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
					DateTime arg_57_0 = now;
					List<TransLog> expr_45 = list;
					if ((arg_57_0 - expr_45[expr_45.Count - 1].recvTime).TotalMilliseconds < 100.0)
					{
						List<TransLog> expr_71 = list;
						expr_71[expr_71.Count - 1].size += (int)bytes;
						List<TransLog> expr_8D = list;
						expr_8D[expr_8D.Count - 1].endTime = now;
						return;
					}
				}
				if (list.Count > 0)
				{
					int i;
					for (i = list.Count - 1; i >= 0; i--)
					{
						if (!(list[i].recvTime > now) || i <= 0)
						{
							list.Insert(i + 1, new TransLog((int)bytes, now));
							break;
						}
					}
					if (i == -1)
					{
						list.Insert(0, new TransLog((int)bytes, now));
					}
				}
				else
				{
					list.Add(new TransLog((int)bytes, now));
				}
				while (list.Count > 0 && now > list[0].recvTime.AddSeconds(3.0))
				{
					list.RemoveAt(0);
				}
			}
		}

		// Token: 0x06000189 RID: 393 RVA: 0x00012528 File Offset: 0x00010728
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

		// Token: 0x06000187 RID: 391 RVA: 0x0001243C File Offset: 0x0001063C
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

		// Token: 0x06000188 RID: 392 RVA: 0x000124E0 File Offset: 0x000106E0
		public void ClearMaxSpeed()
		{
			lock (this)
			{
				this.maxTransDownload = 0L;
				this.maxTransUpload = 0L;
			}
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00012D60 File Offset: 0x00010F60
		public void ResetContinurousTimes()
		{
			lock (this)
			{
				this.lastError = 0;
				this.errorEmptyTimes = 0L;
				this.errorContinurousTimes = 0L;
			}
		}

		// Token: 0x06000197 RID: 407 RVA: 0x00012DAC File Offset: 0x00010FAC
		public void ResetEmptyTimes()
		{
			lock (this)
			{
				this.errorEmptyTimes = 0L;
			}
		}

		// Token: 0x06000195 RID: 405 RVA: 0x00012D0C File Offset: 0x00010F0C
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

		// Token: 0x0600018C RID: 396 RVA: 0x0001267C File Offset: 0x0001087C
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

		// Token: 0x0600017D RID: 381 RVA: 0x00011C1C File Offset: 0x0000FE1C
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
			// Token: 0x06000186 RID: 390 RVA: 0x00012330 File Offset: 0x00010530
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
			// Token: 0x06000184 RID: 388 RVA: 0x00011EB0 File Offset: 0x000100B0
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
					if (arg_99_0 > expr_79[expr_79.Count - 1].recvTime.AddSeconds(3.0))
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
					while (num4 + 3 <= k && (list[k].endTime - list[num4].recvTime).TotalSeconds > 1.5)
					{
						long num5 = (long)((double)(num3 - (long)list[num4].size) / (list[k].endTime - list[num4].recvTime).TotalSeconds);
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
					List<TransLog> expr_1B6 = list;
					num2 = (expr_1B6[expr_1B6.Count - 1].endTime - list[0].recvTime).TotalSeconds;
				}
				if (num2 > 0.2)
				{
					long num6 = (long)((double)num / num2);
					if (num2 > 1.0 && num6 > this.maxTransDownload)
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
			// Token: 0x06000185 RID: 389 RVA: 0x000120F0 File Offset: 0x000102F0
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
					if (arg_99_0 > expr_79[expr_79.Count - 1].recvTime.AddSeconds(3.0))
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
					while (num4 + 3 <= k && (list[k].endTime - list[num4].recvTime).TotalSeconds > 1.5)
					{
						long num5 = (long)((double)(num3 - (long)list[num4].size) / (list[k].endTime - list[num4].recvTime).TotalSeconds);
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
					List<TransLog> expr_1B6 = list;
					num2 = (expr_1B6[expr_1B6.Count - 1].endTime - list[0].recvTime).TotalSeconds;
				}
				if (num2 > 0.2)
				{
					long num6 = (long)((double)num / num2);
					if (num2 > 1.0 && num6 > this.maxTransUpload)
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
			// Token: 0x06000180 RID: 384 RVA: 0x00011DB0 File Offset: 0x0000FFB0
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
			// Token: 0x06000183 RID: 387 RVA: 0x00011E70 File Offset: 0x00010070
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
			// Token: 0x06000182 RID: 386 RVA: 0x00011E30 File Offset: 0x00010030
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
			// Token: 0x06000181 RID: 385 RVA: 0x00011DF0 File Offset: 0x0000FFF0
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
			// Token: 0x0600017E RID: 382 RVA: 0x00011D30 File Offset: 0x0000FF30
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
			// Token: 0x0600017F RID: 383 RVA: 0x00011D70 File Offset: 0x0000FF70
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

		// Token: 0x04000162 RID: 354
		private const int avgTime = 3;

		// Token: 0x0400015F RID: 351
		private List<int> connectTime;

		// Token: 0x0400015B RID: 347
		private List<TransLog> downTransLog;

		// Token: 0x04000161 RID: 353
		private LinkedList<ErrorLog> errList = new LinkedList<ErrorLog>();

		// Token: 0x04000152 RID: 338
		private long errorConnectTimes;

		// Token: 0x04000157 RID: 343
		private long errorContinurousTimes;

		// Token: 0x04000154 RID: 340
		private long errorDecodeTimes;

		// Token: 0x04000155 RID: 341
		private long errorEmptyTimes;

		// Token: 0x04000153 RID: 339
		private long errorTimeoutTimes;

		// Token: 0x04000156 RID: 342
		private int lastError;

		// Token: 0x0400015D RID: 349
		private long maxTransDownload;

		// Token: 0x0400015E RID: 350
		private long maxTransUpload;

		// Token: 0x04000160 RID: 352
		private int sumConnectTime;

		// Token: 0x04000150 RID: 336
		private long totalConnectTimes;

		// Token: 0x04000151 RID: 337
		private long totalDisconnectTimes;

		// Token: 0x04000159 RID: 345
		private long transDownload;

		// Token: 0x0400015A RID: 346
		private long transDownloadRaw;

		// Token: 0x04000158 RID: 344
		private long transUpload;

		// Token: 0x0400015C RID: 348
		private List<TransLog> upTransLog;
	}
}
