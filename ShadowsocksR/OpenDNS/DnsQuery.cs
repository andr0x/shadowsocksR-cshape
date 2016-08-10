using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace OpenDNS
{
	// Token: 0x02000091 RID: 145
	public class DnsQuery
	{
		// Token: 0x06000529 RID: 1321 RVA: 0x0002A8DD File Offset: 0x00028ADD
		public DnsQuery()
		{
			this.Port = 53;
			this.Servers = new ArrayList();
			this.QueryType = Types.A;
			this.QueryClass = Classes.IN;
		}

		// Token: 0x0600052A RID: 1322 RVA: 0x0002A906 File Offset: 0x00028B06
		public DnsQuery(string _Domain, Types _Type)
		{
			this.Port = 53;
			this.Servers = new ArrayList();
			this.QueryType = _Type;
			this.Domain = _Domain;
			this.QueryClass = Classes.IN;
			this.RecursionDesired = true;
		}

		// Token: 0x06000533 RID: 1331 RVA: 0x0002B705 File Offset: 0x00029905
		private bool CheckForServers()
		{
			if (this.Servers.Count == 0)
			{
				this.Servers = this.GetDefaultServers();
			}
			if (this.Servers.Count > 0)
			{
				return true;
			}
			throw new Exception("Abort: No DNS servers specified manually and could not get default ones.");
		}

		// Token: 0x06000532 RID: 1330 RVA: 0x0002B640 File Offset: 0x00029840
		private int ExtractName(int ResourceDataCursor, StringBuilder Name)
		{
			int num = (int)(this.data[ResourceDataCursor++] & 255);
			if (num == 0)
			{
				return ResourceDataCursor;
			}
			while ((num & 192) != 192)
			{
				if (ResourceDataCursor + num > this.length)
				{
					return -1;
				}
				Name.Append(Encoding.ASCII.GetString(this.data, ResourceDataCursor, num));
				ResourceDataCursor += num;
				if (ResourceDataCursor > this.length)
				{
					return -1;
				}
				num = (int)(this.data[ResourceDataCursor++] & 255);
				if (num != 0)
				{
					Name.Append(".");
				}
				if (num == 0)
				{
					return ResourceDataCursor;
				}
			}
			if (ResourceDataCursor >= this.length)
			{
				return -1;
			}
			int resourceDataCursor = (num & 63) << 8 | (int)(this.data[ResourceDataCursor++] & 255);
			this.ExtractName(resourceDataCursor, Name);
			return ResourceDataCursor;
		}

		// Token: 0x06000534 RID: 1332 RVA: 0x0002B73C File Offset: 0x0002993C
		private ArrayList GetDefaultServers()
		{
			ArrayList result = new ArrayList();
			try
			{
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Could not get DNS servers from network adapter: " + ex.Message, "OpenDNS");
			}
			finally
			{
			}
			return result;
		}

		// Token: 0x06000531 RID: 1329 RVA: 0x0002B614 File Offset: 0x00029814
		private string GetName()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.position = this.ExtractName(this.position, stringBuilder);
			return stringBuilder.ToString();
		}

		// Token: 0x06000530 RID: 1328 RVA: 0x0002AE74 File Offset: 0x00029074
		private void GetResourceRecord(int i, ResourceRecordCollection Container)
		{
			string name = this.GetName();
			byte[] arg_21_0 = this.data;
			int num = this.position;
			this.position = num + 1;
			Types arg_4B_0 = (Types)((arg_21_0[num] & 255) << 8);
			byte[] arg_44_0 = this.data;
			num = this.position;
			this.position = num + 1;
			Types types = (Types)(((int)arg_4B_0 )| (arg_44_0[num] & 255));
			byte[] arg_67_0 = this.data;
			num = this.position;
			this.position = num + 1;
			Classes arg_91_0 = (Classes)((arg_67_0[num] & 255) << 8);
			byte[] arg_8A_0 = this.data;
			num = this.position;
			this.position = num + 1;
			Classes @class = (Classes)(((int)arg_91_0) | (arg_8A_0[num] & 255));
			byte[] arg_AD_0 = this.data;
			num = this.position;
			this.position = num + 1;
			int arg_DB_0 = (arg_AD_0[num] & 255) << 24;
			byte[] arg_D1_0 = this.data;
			num = this.position;
			this.position = num + 1;
			int arg_FF_0 = arg_DB_0 | (arg_D1_0[num] & 255) << 16;
			byte[] arg_F6_0 = this.data;
			num = this.position;
			this.position = num + 1;
			int arg_121_0 = arg_FF_0 | (arg_F6_0[num] & 255) << 8;
			byte[] arg_11A_0 = this.data;
			num = this.position;
			this.position = num + 1;
			int timeToLive = arg_121_0 | (arg_11A_0[num] & 255);
			byte[] arg_13D_0 = this.data;
			num = this.position;
			this.position = num + 1;
			object arg_13E_0 = arg_13D_0[num];
			byte[] arg_159_0 = this.data;
			num = this.position;
			this.position = num + 1;
			object arg_15A_0 = arg_159_0[num];
			switch (types)
			{
			case Types.A:
			{
				byte[] expr_1AB = new byte[4];
				int arg_1CF_1 = 0;
				byte[] arg_1C7_0 = this.data;
				num = this.position;
				this.position = num + 1;
				expr_1AB[arg_1CF_1] = (byte)(arg_1C7_0[num] & 255);
				int arg_1F4_1 = 1;
				byte[] arg_1EC_0 = this.data;
				num = this.position;
				this.position = num + 1;
				expr_1AB[arg_1F4_1] = (byte)(arg_1EC_0[num] & 255);
				int arg_219_1 = 2;
				byte[] arg_211_0 = this.data;
				num = this.position;
				this.position = num + 1;
				expr_1AB[arg_219_1] = (byte)(arg_211_0[num] & 255);
				int arg_23E_1 = 3;
				byte[] arg_236_0 = this.data;
				num = this.position;
				this.position = num + 1;
				expr_1AB[arg_23E_1] = (byte)(arg_236_0[num] & 255);
				byte[] array = expr_1AB;
				string resourceAddress = string.Concat(new object[]
				{
					array[0],
					".",
					array[1],
					".",
					array[2],
					".",
					array[3]
				});
				Address value = new Address(name, types, @class, timeToLive, resourceAddress);
				Container.Add(value);
				return;
			}
			case Types.NS:
			case Types.CNAME:
				break;
			case (Types)3:
			case (Types)4:
				goto IL_772;
			case Types.SOA:
			{
				string name2 = this.GetName();
				string name3 = this.GetName();
				byte[] arg_40E_0 = this.data;
				num = this.position;
				this.position = num + 1;
				int arg_43C_0 = (arg_40E_0[num] & 255) << 24;
				byte[] arg_432_0 = this.data;
				num = this.position;
				this.position = num + 1;
				int arg_460_0 = arg_43C_0 | (arg_432_0[num] & 255) << 16;
				byte[] arg_457_0 = this.data;
				num = this.position;
				this.position = num + 1;
				int arg_482_0 = arg_460_0 | (arg_457_0[num] & 255) << 8;
				byte[] arg_47B_0 = this.data;
				num = this.position;
				this.position = num + 1;
				long serial = (long)(arg_482_0 | (arg_47B_0[num] & 255));
				byte[] arg_4A0_0 = this.data;
				num = this.position;
				this.position = num + 1;
				int arg_4CE_0 = (arg_4A0_0[num] & 255) << 24;
				byte[] arg_4C4_0 = this.data;
				num = this.position;
				this.position = num + 1;
				int arg_4F2_0 = arg_4CE_0 | (arg_4C4_0[num] & 255) << 16;
				byte[] arg_4E9_0 = this.data;
				num = this.position;
				this.position = num + 1;
				int arg_514_0 = arg_4F2_0 | (arg_4E9_0[num] & 255) << 8;
				byte[] arg_50D_0 = this.data;
				num = this.position;
				this.position = num + 1;
				long refresh = (long)(arg_514_0 | (arg_50D_0[num] & 255));
				byte[] arg_532_0 = this.data;
				num = this.position;
				this.position = num + 1;
				int arg_560_0 = (arg_532_0[num] & 255) << 24;
				byte[] arg_556_0 = this.data;
				num = this.position;
				this.position = num + 1;
				int arg_584_0 = arg_560_0 | (arg_556_0[num] & 255) << 16;
				byte[] arg_57B_0 = this.data;
				num = this.position;
				this.position = num + 1;
				int arg_5A6_0 = arg_584_0 | (arg_57B_0[num] & 255) << 8;
				byte[] arg_59F_0 = this.data;
				num = this.position;
				this.position = num + 1;
				long retry = (long)(arg_5A6_0 | (arg_59F_0[num] & 255));
				byte[] arg_5C4_0 = this.data;
				num = this.position;
				this.position = num + 1;
				int arg_5F2_0 = (arg_5C4_0[num] & 255) << 24;
				byte[] arg_5E8_0 = this.data;
				num = this.position;
				this.position = num + 1;
				int arg_616_0 = arg_5F2_0 | (arg_5E8_0[num] & 255) << 16;
				byte[] arg_60D_0 = this.data;
				num = this.position;
				this.position = num + 1;
				int arg_638_0 = arg_616_0 | (arg_60D_0[num] & 255) << 8;
				byte[] arg_631_0 = this.data;
				num = this.position;
				this.position = num + 1;
				long expire = (long)(arg_638_0 | (arg_631_0[num] & 255));
				byte[] arg_656_0 = this.data;
				num = this.position;
				this.position = num + 1;
				int arg_684_0 = (arg_656_0[num] & 255) << 24;
				byte[] arg_67A_0 = this.data;
				num = this.position;
				this.position = num + 1;
				int arg_6A8_0 = arg_684_0 | (arg_67A_0[num] & 255) << 16;
				byte[] arg_69F_0 = this.data;
				num = this.position;
				this.position = num + 1;
				int arg_6CA_0 = arg_6A8_0 | (arg_69F_0[num] & 255) << 8;
				byte[] arg_6C3_0 = this.data;
				num = this.position;
				this.position = num + 1;
				long minimum = (long)(arg_6CA_0 | (arg_6C3_0[num] & 255));
				SOA value2 = new SOA(name, types, @class, timeToLive, name2, name3, serial, refresh, retry, expire, minimum);
				Container.Add(value2);
				return;
			}
			default:
				switch (types)
				{
				case Types.PTR:
				case Types.MINFO:
				case Types.TXT:
					break;
				case Types.HINFO:
					goto IL_772;
				case Types.MX:
				{
					byte[] arg_72A_0 = this.data;
					num = this.position;
					this.position = num + 1;
					int arg_74E_0 = arg_72A_0[num] << 8;
					byte[] arg_747_0 = this.data;
					num = this.position;
					this.position = num + 1;
					int preference = arg_74E_0 | (arg_747_0[num] & 255);
					string name4 = this.GetName();
					MX value3 = new MX(name, types, @class, timeToLive, preference, name4);
					Container.Add(value3);
					return;
				}
				default:
				{
					if (types != Types.AAAA)
					{
						goto IL_772;
					}
					ushort[] array2 = new ushort[8];
					for (int j = 0; j < 8; j++)
					{
						array2[j] = (ushort)((int)(this.data[this.position + j * 2] & 255) << 8 | (int)(this.data[this.position + j * 2 + 1] & 255));
					}
					this.position += 16;
					string resourceAddress2 = string.Concat(new object[]
					{
						Convert.ToString((int)array2[0], 16),
						":",
						Convert.ToString((int)array2[1], 16),
						":",
						Convert.ToString((int)array2[2], 16),
						":",
						Convert.ToString((int)array2[3], 16),
						":",
						Convert.ToString((int)array2[4], 16),
						":",
						Convert.ToString((int)array2[5], 16),
						":",
						Convert.ToString((int)array2[6], 16),
						":",
						Convert.ToString((int)array2[7], 16)
					});
					Address value4 = new Address(name, types, @class, timeToLive, resourceAddress2);
					Container.Add(value4);
					return;
				}
				}
				break;
			}
			string name5 = this.GetName();
			ResourceRecord value5 = new ResourceRecord(name, types, @class, timeToLive, name5);
			Container.Add(value5);
			return;
			IL_772:
			Trace.WriteLine("Resource type did not match: " + types.ToString(), "RUY QDNS");
		}

		// Token: 0x0600052E RID: 1326 RVA: 0x0002AAF0 File Offset: 0x00028CF0
		private byte[] MakeQuery()
		{
			int num = new Random().Next(55555);
			byte[] array = new byte[512];
			for (int i = 0; i < 512; i++)
			{
				array[i] = 0;
			}
			array[0] = (byte)(num >> 8);
			array[1] = (byte)(num & 255);
			array[2] = 1;
			array[2] = (this.RecursionDesired ? (byte)(array[2] | 1) : (byte)(array[2] & 254));
			array[3] = 0;
			array[4] = 0;
			array[5] = 1;
			string[] array2 = this.Domain.Split(new char[]
			{
				'.'
			});
			int num2 = 12;
			for (int j = 0; j < array2.Length; j++)
			{
				string text = array2[j];
				array[num2++] = (byte)(text.Length & 255);
				byte[] bytes = Encoding.ASCII.GetBytes(text);
				for (int k = 0; k < bytes.Length; k++)
				{
					array[num2++] = bytes[k];
				}
			}
			array[num2++] = 0;
			array[num2++] = 0;
			array[num2++] = (byte)this.QueryType;
			array[num2++] = 0;
			array[num2++] = (byte)this.QueryClass;
			return array;
		}

		// Token: 0x0600052F RID: 1327 RVA: 0x0002AC24 File Offset: 0x00028E24
		private void ReadResponse()
		{
			int iD = ((int)(this.data[0] & 255) << 8) + (int)(this.data[1] & 255);
			byte arg_28_0 = this.data[2];
			byte arg_31_0 = this.data[2];
			bool aA = (this.data[2] & 4) == 4;
			bool tC = (this.data[2] & 2) == 2;
			bool rD = (this.data[2] & 1) == 1;
			bool rA = (this.data[3] & 128) == 128;
			byte arg_7B_0 = this.data[3];
			int rC = (int)(this.data[3] & 15);
			int num = (int)(this.data[4] & 255) << 8 | (int)(this.data[5] & 255);
			int num2 = (int)(this.data[6] & 255) << 8 | (int)(this.data[7] & 255);
			int num3 = (int)(this.data[8] & 255) << 8 | (int)(this.data[9] & 255);
			int num4 = (int)(this.data[10] & 255) << 8 | (int)(this.data[11] & 255);
			this._Response = new DnsResponse(iD, aA, tC, rD, rA, rC);
			this.position = 12;
			for (int i = 0; i < num; i++)
			{
				this.GetName();
				byte[] arg_154_0 = this.data;
				int num5 = this.position;
				this.position = num5 + 1;
				int arg_17E_0 = (arg_154_0[num5] & 255) << 8;
				byte[] arg_177_0 = this.data;
				num5 = this.position;
				this.position = num5 + 1;
				int arg_17F_0 = arg_17E_0 | (arg_177_0[num5] & 255);
				byte[] arg_19A_0 = this.data;
				num5 = this.position;
				this.position = num5 + 1;
				int arg_1C4_0 = (arg_19A_0[num5] & 255) << 8;
				byte[] arg_1BD_0 = this.data;
				num5 = this.position;
				this.position = num5 + 1;
				int arg_1C5_0 = arg_1C4_0 | (arg_1BD_0[num5] & 255);
			}
			for (int j = 0; j < num2; j++)
			{
				this.GetResourceRecord(j, this._Response.Answers);
			}
			for (int k = 0; k < num3; k++)
			{
				this.GetResourceRecord(k, this._Response.Authorities);
			}
			for (int l = 0; l < num4; l++)
			{
				this.GetResourceRecord(l, this._Response.AdditionalRecords);
			}
		}

		// Token: 0x0600052B RID: 1323 RVA: 0x0002A940 File Offset: 0x00028B40
		public bool Send()
		{
			this.CheckForServers();
			//using 
			IEnumerator enumerator = this.Servers.GetEnumerator();
			{
				while (enumerator.MoveNext())
				{
					string[] array = ((string)enumerator.Current).Split(new char[]
					{
						' '
					}, 2);
					int port = 53;
					if (array.Length == 2)
					{
						try
						{
							port = int.Parse(array[1]);
						}
						catch (Exception)
						{
						}
					}
					try
					{
						this.SendQuery2(array[0], port);
						break;
					}
					catch (Exception)
					{
					}
				}
			}
			return this.Response != null;
		}

		// Token: 0x0600052C RID: 1324 RVA: 0x0002A9F4 File Offset: 0x00028BF4
		private void SendQuery(string ipAddress)
		{
			if (ipAddress == null)
			{
				throw new ArgumentNullException();
			}
			UdpClient udpClient = new UdpClient(ipAddress, this.Port);
			byte[] array = this.MakeQuery();
			UdpClient arg_22_0 = udpClient;
			byte[] expr_1F = array;
			arg_22_0.Send(expr_1F, expr_1F.Length);
			IPEndPoint iPEndPoint = null;
			this.data = udpClient.Receive(ref iPEndPoint);
			this.length = this.data.Length;
			this.ReadResponse();
			udpClient.Close();
		}

		// Token: 0x0600052D RID: 1325 RVA: 0x0002AA54 File Offset: 0x00028C54
		private void SendQuery2(string ipAddress, int port)
		{
			int optionValue = 5000;
			if (ipAddress == null)
			{
				throw new ArgumentNullException();
			}
			byte[] buffer = this.MakeQuery();
			IPAddress expr_1C = IPAddress.Parse(ipAddress);
			EndPoint remoteEP = new IPEndPoint(expr_1C, port);
			Socket socket = new Socket(expr_1C.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
			socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, optionValue);
			socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, optionValue);
			socket.SendTo(buffer, remoteEP);
			this.data = new byte[512];
			this.length = socket.ReceiveFrom(this.data, ref remoteEP);
			this.ReadResponse();
			socket.Shutdown(SocketShutdown.Both);
		}

		// Token: 0x17000084 RID: 132
		public DnsResponse Response
		{
			// Token: 0x06000528 RID: 1320 RVA: 0x0002A8D5 File Offset: 0x00028AD5
			get
			{
				return this._Response;
			}
		}

		// Token: 0x0400037B RID: 891
		private byte[] data;

		// Token: 0x0400037E RID: 894
		public string Domain;

		// Token: 0x0400037D RID: 893
		private int length;

		// Token: 0x04000381 RID: 897
		public int Port;

		// Token: 0x0400037C RID: 892
		private int position;

		// Token: 0x04000380 RID: 896
		public Classes QueryClass;

		// Token: 0x0400037F RID: 895
		public Types QueryType;

		// Token: 0x04000383 RID: 899
		public bool RecursionDesired;

		// Token: 0x04000382 RID: 898
		public ArrayList Servers;

		// Token: 0x04000384 RID: 900
		private DnsResponse _Response;
	}
}
