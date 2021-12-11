using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using HentaiPages.Database;
using Microsoft.EntityFrameworkCore;
using SimpleImageComparisonClassLibrary;

namespace DupsFinder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var _db = new HentaiDbContext();
            var ids = _db.Images.Select(x=>x.ImageId).OrderBy(x => x).ToList();
            var duplicates = new List<long>();
            
            while (ids.Count > 0)
            {
                var currentId = ids.FirstOrDefault();
                ids.RemoveAll(x => x == currentId);

                var (dups, toRemove) = FindDupsForImage(_db, currentId, ids);
                ids.RemoveAll(x => dups.Contains(x));
                ids.RemoveAll(x => toRemove.Contains(x));
                
                duplicates.AddRange(dups);
                
                Console.WriteLine("Constructing file...");
                using var sw = new StreamWriter(@".\duplicateIds.txt", false);
                foreach(var id in duplicates)
                {
                    sw.WriteLine(id);
                }
            }  
        }

        public static (List<long>, List<long>) FindDupsForImage(HentaiDbContext db, long id, List<long> ids)
        {
            var duplicates = new List<long>();
            var toRemove = new List<long>();
            
            var image = db.Images.FirstOrDefault(c => c.ImageId == id);
            if (image == null)
                return (duplicates, toRemove);

            var commonSize = new Size(500, 500);
            
            using var ms = new MemoryStream(image.Data);
            var source = ImageTool.ResizeImage(Image.FromStream(ms), commonSize);
            var processed = 0;
            var maxId = ids.Max();
            foreach (var i in ids)
            {
                var imageData = db.Images.Where(c => c.ImageId == i).Select(x=>x.Data).FirstOrDefault();

                try
                {
                    using var ms2 = new MemoryStream(imageData);
                    var imgFromStream = Image.FromStream(ms2);
                    var img = ImageTool.ResizeImage(imgFromStream, commonSize);
                    var difference = ImageTool.GetPercentageDifference(source, img);
                    if (difference < 0.1f)
                        duplicates.Add(i);
                    
                    Console.WriteLine($"{++processed}/{ids.Count}/{id}/{maxId}");
                }
                catch (Exception)
                {
                    toRemove.Add(i);
                }
            }
            return (duplicates, toRemove);
        }
    }
}