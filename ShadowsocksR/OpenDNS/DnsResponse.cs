using System;

namespace OpenDNS
{
	// Token: 0x02000092 RID: 146
	public class DnsResponse
	{
		// Token: 0x0600053F RID: 1343 RVA: 0x0002B91C File Offset: 0x00029B1C
		public DnsResponse(int ID, bool AA, bool TC, bool RD, bool RA, int RC)
		{
			this._QueryID = ID;
			this._AuthorativeAnswer = AA;
			this._IsTruncated = TC;
			this._RecursionDesired = RD;
			this._RecursionAvailable = RA;
			this._ResponseCode = (ResponseCodes)RC;
			this._ResourceRecords = new ResourceRecordCollection();
			this._Answers = new ResourceRecordCollection();
			this._Authorities = new ResourceRecordCollection();
			this._AdditionalRecords = new ResourceRecordCollection();
		}

		// Token: 0x1700008D RID: 141
		public ResourceRecordCollection AdditionalRecords
		{
			// Token: 0x0600053D RID: 1341 RVA: 0x0002B7CC File Offset: 0x000299CC
			get
			{
				return this._AdditionalRecords;
			}
		}

		// Token: 0x1700008B RID: 139
		public ResourceRecordCollection Answers
		{
			// Token: 0x0600053B RID: 1339 RVA: 0x0002B7BC File Offset: 0x000299BC
			get
			{
				return this._Answers;
			}
		}

		// Token: 0x17000086 RID: 134
		public bool AuthorativeAnswer
		{
			// Token: 0x06000536 RID: 1334 RVA: 0x0002B794 File Offset: 0x00029994
			get
			{
				return this._AuthorativeAnswer;
			}
		}

		// Token: 0x1700008C RID: 140
		public ResourceRecordCollection Authorities
		{
			// Token: 0x0600053C RID: 1340 RVA: 0x0002B7C4 File Offset: 0x000299C4
			get
			{
				return this._Authorities;
			}
		}

		// Token: 0x17000087 RID: 135
		public bool IsTruncated
		{
			// Token: 0x06000537 RID: 1335 RVA: 0x0002B79C File Offset: 0x0002999C
			get
			{
				return this._IsTruncated;
			}
		}

		// Token: 0x17000085 RID: 133
		public int QueryID
		{
			// Token: 0x06000535 RID: 1333 RVA: 0x0002B78C File Offset: 0x0002998C
			get
			{
				return this._QueryID;
			}
		}

		// Token: 0x17000089 RID: 137
		public bool RecursionAvailable
		{
			// Token: 0x06000539 RID: 1337 RVA: 0x0002B7AC File Offset: 0x000299AC
			get
			{
				return this._RecursionAvailable;
			}
		}

		// Token: 0x17000088 RID: 136
		public bool RecursionRequested
		{
			// Token: 0x06000538 RID: 1336 RVA: 0x0002B7A4 File Offset: 0x000299A4
			get
			{
				return this._RecursionDesired;
			}
		}

		// Token: 0x1700008E RID: 142
		public ResourceRecordCollection ResourceRecords
		{
			// Token: 0x0600053E RID: 1342 RVA: 0x0002B7D4 File Offset: 0x000299D4
			get
			{
				if (this._ResourceRecords.Count == 0 && this._Answers.Count > 0 && this._Authorities.Count > 0 && this._AdditionalRecords.Count > 0)
				{
					foreach (ResourceRecord value in this.Answers)
					{
						this._ResourceRecords.Add(value);
					}
					foreach (ResourceRecord value2 in this.Authorities)
					{
						this._ResourceRecords.Add(value2);
					}
					foreach (ResourceRecord value3 in this.AdditionalRecords)
					{
						this._ResourceRecords.Add(value3);
					}
				}
				return this._ResourceRecords;
			}
		}

		// Token: 0x1700008A RID: 138
		public ResponseCodes ResponseCode
		{
			// Token: 0x0600053A RID: 1338 RVA: 0x0002B7B4 File Offset: 0x000299B4
			get
			{
				return this._ResponseCode;
			}
		}

		// Token: 0x0400038E RID: 910
		private ResourceRecordCollection _AdditionalRecords;

		// Token: 0x0400038C RID: 908
		private ResourceRecordCollection _Answers;

		// Token: 0x04000386 RID: 902
		private bool _AuthorativeAnswer;

		// Token: 0x0400038D RID: 909
		private ResourceRecordCollection _Authorities;

		// Token: 0x04000387 RID: 903
		private bool _IsTruncated;

		// Token: 0x04000385 RID: 901
		private int _QueryID;

		// Token: 0x04000389 RID: 905
		private bool _RecursionAvailable;

		// Token: 0x04000388 RID: 904
		private bool _RecursionDesired;

		// Token: 0x0400038B RID: 907
		private ResourceRecordCollection _ResourceRecords;

		// Token: 0x0400038A RID: 906
		private ResponseCodes _ResponseCode;
	}
}
