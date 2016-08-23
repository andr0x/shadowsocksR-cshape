using System;
using System.IO;
using System.IO.Compression;

namespace Shadowsocks.Controller
{
	// Token: 0x0200003B RID: 59
	public class FileManager
	{
		// Token: 0x06000212 RID: 530 RVA: 0x00014D88 File Offset: 0x00012F88
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

		// Token: 0x06000214 RID: 532 RVA: 0x00014E44 File Offset: 0x00013044
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

		// Token: 0x06000215 RID: 533 RVA: 0x00014EBC File Offset: 0x000130BC
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

		// Token: 0x06000213 RID: 531 RVA: 0x00014DD8 File Offset: 0x00012FD8
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
