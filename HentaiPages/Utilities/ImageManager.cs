using System;
using System.IO;
using HentaiPages.Database;

namespace HentaiPages.Utilities
{
	public static class ImageManager
	{
		private const string FolderPath = @"D:\HentaiCollection\";
		
		public static string ExtractToPhysicalPath(HentaiDbContext db, byte[] data)
		{
			if (data is null) return "";

			var fileName = $"{Guid.NewGuid().ToString()}";
			using var stream = File.Create($"{FolderPath}{fileName}");
			stream.Write(data, 0, data.Length);
			return fileName;
		}
		
		public static byte[] GetData(string fileName)
		{
			return File.ReadAllBytes($"{FolderPath}{fileName}");
		}
		
		public static void DeleteData(string fileName)
		{
			File.Delete($"{FolderPath}{fileName}");
		}
		
		
	}
}