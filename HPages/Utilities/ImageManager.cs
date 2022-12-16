using System;
using System.IO;
using HPages.Database;

namespace HPages.Utilities
{
	public static class ImageManager
	{
		public static string FolderPath { get; set; } = "./COLLECTION/";
		
		public static string ExtractToPhysicalPath(HentaiDbContext db, byte[] data)
		{
			if (data is null) return "";
			if (!Directory.Exists(FolderPath))
				Directory.CreateDirectory(FolderPath);

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