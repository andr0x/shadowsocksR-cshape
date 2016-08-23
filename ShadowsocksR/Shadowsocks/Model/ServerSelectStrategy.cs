using System;
using System.Collections.Generic;

namespace Shadowsocks.Model
{
	// Token: 0x02000023 RID: 35
	public class ServerSelectStrategy
	{
		// Token: 0x06000155 RID: 341 RVA: 0x0001088C File Offset: 0x0000EA8C
		private double Algorithm2(ServerSpeedLog serverSpeedLog)
		{
			if (serverSpeedLog.ErrorContinurousTimes > 30L)
			{
				return 1.0;
			}
			if (serverSpeedLog.TotalConnectTimes < 3L)
			{
				return 500.0;
			}
			if (serverSpeedLog.AvgConnectTime < 0L)
			{
				return 1.0;
			}
			if (serverSpeedLog.AvgConnectTime <= 20L)
			{
				return 500.0;
			}
			long num = serverSpeedLog.TotalConnectTimes - serverSpeedLog.TotalDisconnectTimes;
			double num2 = 10000.0 / (double)serverSpeedLog.AvgConnectTime - (double)(num * 5L);
			if (num2 > 500.0)
			{
				num2 = 500.0;
			}
			num2 -= (double)(serverSpeedLog.ErrorContinurousTimes * 10L);
			if (num2 < 1.0)
			{
				num2 = 1.0;
			}
			return num2;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x0001094C File Offset: 0x0000EB4C
		private double Algorithm3(ServerSpeedLog serverSpeedLog)
		{
			if (serverSpeedLog.ErrorContinurousTimes > 30L)
			{
				return 1.0;
			}
			if (serverSpeedLog.TotalConnectTimes < 3L)
			{
				return 500.0;
			}
			if (serverSpeedLog.AvgConnectTime < 0L)
			{
				return 1.0;
			}
			long num = serverSpeedLog.TotalConnectTimes - serverSpeedLog.TotalDisconnectTimes;
			double num2 = 20.0 / (double)(serverSpeedLog.AvgConnectTime / 500L + 1L) - (double)num;
			if (num2 > 500.0)
			{
				num2 = 500.0;
			}
			num2 -= (double)(serverSpeedLog.ErrorContinurousTimes * 2L);
			if (num2 < 1.0)
			{
				num2 = 1.0;
			}
			return num2;
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00010848 File Offset: 0x0000EA48
		private int lowerBound(List<double> data, double target)
		{
			int i = 0;
			int num = data.Count - 1;
			while (i < num)
			{
				int num2 = (i + num) / 2;
				if (data[num2] >= target)
				{
					num = num2;
				}
				else if (data[num2] < target)
				{
					i = num2 + 1;
				}
			}
			return i;
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00010A00 File Offset: 0x0000EC00
		public int Select(List<Server> configs, int curIndex, int algorithm, bool forceChange = false)
		{
			if (this.randomGennarator == null)
			{
				this.randomGennarator = new Random();
				this.lastSelectIndex = -1;
			}
			if (configs.Count <= this.lastSelectIndex || this.lastSelectIndex < 0 || !configs[this.lastSelectIndex].isEnable())
			{
				this.lastSelectIndex = -1;
				this.lastSelectTime = DateTime.Now;
				this.lastUserSelectIndex = -1;
			}
			if (this.lastUserSelectIndex != curIndex)
			{
				if (configs.Count > curIndex && curIndex >= 0 && configs[curIndex].isEnable())
				{
					this.lastSelectIndex = curIndex;
				}
				this.lastUserSelectIndex = curIndex;
			}
			if (configs.Count > 0)
			{
				List<ServerSelectStrategy.ServerIndex> list = new List<ServerSelectStrategy.ServerIndex>();
				for (int i = 0; i < configs.Count; i++)
				{
					if ((!forceChange || this.lastSelectIndex != i) && configs[i].isEnable() && (algorithm != 6 || configs.Count <= this.lastSelectIndex || this.lastSelectIndex < 0 || !(configs[this.lastSelectIndex].group != configs[i].group)))
					{
						list.Add(new ServerSelectStrategy.ServerIndex(i, configs[i]));
					}
				}
				if (list.Count == 0)
				{
					int num = this.lastSelectIndex;
					if (num >= 0 && num < configs.Count && configs[num].isEnable())
					{
						list.Add(new ServerSelectStrategy.ServerIndex(num, configs[num]));
					}
				}
				int num2 = -1;
				if (list.Count > 0)
				{
					if (algorithm == 0)
					{
						this.lastSelectIndex = (this.lastSelectIndex + 1) % configs.Count;
						for (int j = 0; j < configs.Count; j++)
						{
							if (configs[this.lastSelectIndex].isEnable())
							{
								num2 = this.lastSelectIndex;
								break;
							}
							this.lastSelectIndex = (this.lastSelectIndex + 1) % configs.Count;
						}
					}
					else if (algorithm == 1)
					{
						num2 = this.randomGennarator.Next(list.Count);
						num2 = list[num2].index;
					}
					else
					{
						if (algorithm == 3 || algorithm == 5 || algorithm == 6)
						{
							if (algorithm == 5)
							{
								if ((DateTime.Now - this.lastSelectTime).TotalSeconds > 600.0)
								{
									this.lastSelectTime = DateTime.Now;
								}
								else if (configs.Count > this.lastSelectIndex && this.lastSelectIndex >= 0 && configs[this.lastSelectIndex].isEnable() && !forceChange)
								{
									return this.lastSelectIndex;
								}
							}
							List<double> list2 = new List<double>();
							double num3 = 0.0;
							foreach (ServerSelectStrategy.ServerIndex current in list)
							{
								double num4 = this.Algorithm3(current.server.ServerSpeedLog());
								list2.Add(num3 + num4);
								num3 += num4;
							}
							double target = this.randomGennarator.NextDouble() * num3;
							num2 = this.lowerBound(list2, target);
							num2 = list[num2].index;
							this.lastSelectIndex = num2;
							return num2;
						}
						List<double> list3 = new List<double>();
						double num5 = 0.0;
						foreach (ServerSelectStrategy.ServerIndex current2 in list)
						{
							double num6 = this.Algorithm2(current2.server.ServerSpeedLog());
							list3.Add(num5 + num6);
							num5 += num6;
						}
						if (algorithm == 4 && this.randomGennarator.Next(3) == 0 && configs[curIndex].isEnable())
						{
							this.lastSelectIndex = curIndex;
							return curIndex;
						}
						double target2 = this.randomGennarator.NextDouble() * num5;
						num2 = this.lowerBound(list3, target2);
						num2 = list[num2].index;
						this.lastSelectIndex = num2;
						return num2;
					}
				}
				this.lastSelectIndex = num2;
				return num2;
			}
			return -1;
		}

		// Token: 0x04000104 RID: 260
		private int lastSelectIndex;

		// Token: 0x04000105 RID: 261
		private DateTime lastSelectTime;

		// Token: 0x04000106 RID: 262
		private int lastUserSelectIndex;

		// Token: 0x04000103 RID: 259
		private Random randomGennarator;

		// Token: 0x020000A3 RID: 163
		private enum SelectAlgorithm
		{
			// Token: 0x04000432 RID: 1074
			OneByOne,
			// Token: 0x04000433 RID: 1075
			Random,
			// Token: 0x04000434 RID: 1076
			LowLatency,
			// Token: 0x04000435 RID: 1077
			LowException,
			// Token: 0x04000436 RID: 1078
			SelectedFirst,
			// Token: 0x04000437 RID: 1079
			Timer,
			// Token: 0x04000438 RID: 1080
			LowExceptionInGroup
		}

		// Token: 0x020000A4 RID: 164
		private struct ServerIndex
		{
			// Token: 0x06000557 RID: 1367 RVA: 0x0002AE04 File Offset: 0x00029004
			public ServerIndex(int i, Server s)
			{
				this.index = i;
				this.server = s;
			}

			// Token: 0x04000439 RID: 1081
			public int index;

			// Token: 0x0400043A RID: 1082
			public Server server;
		}
	}
}
