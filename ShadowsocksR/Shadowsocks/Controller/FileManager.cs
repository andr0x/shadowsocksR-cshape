using System;
using System.IO;
using System.IO.Compression;

namespace Shadowsocks.Controller
{
	// Token: 0x0200003D RID: 61
	public class FileManager
	{
		// Token: 0x06000227 RID: 551 RVA: 0x00015618 File Offset: 0x00013818
		public static bool ByteArrayToFile(string fileName, byte[] content)
		{
			try
			{
				FileStream expr_08 = new FileStream(fileName, FileMode.Create, FileAccess.Write);
				expr_08.Write(content, 0, content.Length);
				expr_08.Close();
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception caught in process: {0}", ex.ToString());
			}
			return false;
		}

		// Token: 0x06000229 RID: 553 RVA: 0x000156D4 File Offset: 0x000138D4
		public static byte[] DeflateCompress(byte[] content, int index, int count, out int size)
		{
			size = 0;
			try
			{
				MemoryStream memoryStream = new MemoryStream();
				using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
				{
					deflateStream.Write(content, index, count);
				}
				byte[] array = memoryStream.ToArray();
				size = array.Length;
				return array;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception caught in process: {0}", ex.ToString());
			}
			return null;
		}

		// Token: 0x0600022A RID: 554 RVA: 0x0001574C File Offset: 0x0001394C
		public static byte[] DeflateDecompress(byte[] content, int index, int count, out int size)
		{
			size = 0;
			try
			{
				byte[] array = new byte[16384];
				DeflateStream deflateStream = new DeflateStream(new MemoryStream(content, index, count), CompressionMode.Decompress);
				while (true)
				{
					int num = deflateStream.Read(array, size, array.Length - size);
					if (num == 0)
					{
						break;
					}
					size += num;
					byte[] array2 = new byte[array.Length * 2];
					array.CopyTo(array2, 0);
					array = array2;
				}
				return array;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception caught in process: {0}", ex.ToString());
			}
			return null;
		}

		// Token: 0x06000228 RID: 552 RVA: 0x00015668 File Offset: 0x00013868
		public static void UncompressFile(string fileName, byte[] content)
		{
			FileStream fileStream = File.Create(fileName);
			byte[] array = new byte[4096];
			using (GZipStream gZipStream = new GZipStream(new MemoryStream(content), CompressionMode.Decompress, false))
			{
				while (true)
				{
					int num = gZipStream.Read(array, 0, array.Length);
					if (num == 0)
					{
						break;
					}
					fileStream.Write(array, 0, num);
				}
			}
			fileStream.Close();
		}
	}
}
