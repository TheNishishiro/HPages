using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HentaiPages.Database;

namespace HentaiUtility.Actions
{
    public static class DuplicatesFinder
    {
        public static void Find()
        {
            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Console.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().Location);
            using var _db = new HentaiDbContext();

            var indexes = _db.Images.Where(x => x.Hash != null).Select(x => x.ImageId).OrderBy(x=>x).ToList();
            var duplicates = new Dictionary<long, List<long>>();

            var maxIndex = indexes.Max();
            while (indexes.Count > 0)
            {
                var currentId = indexes.FirstOrDefault();
                var imageHash = _db.Images.Where(x => x.ImageId == currentId).Select(x => x.Hash).FirstOrDefault();
                var currentDuplicates = _db.Images.Where(x => x.Hash == imageHash && x.ImageId != currentId).Select(x => x.ImageId).ToList();

                duplicates.Add(currentId, new List<long>(currentDuplicates));
                indexes.RemoveAll(x => x == currentId);
                indexes.RemoveAll(x => currentDuplicates.Contains(x));
                Console.WriteLine($"{currentId}/{maxIndex}");
            }
            
            Console.WriteLine("Writting to file...");
            var sb = new StringBuilder();
            using var sw = new StreamWriter(@"duplicateIds.txt", false);
            foreach(var (key, value) in duplicates)
            {
                sw.WriteLine($"{key}: {string.Join(",", value)}");
                sb.AppendLine($"{key}: {string.Join(",", value)}");
            }
            Console.WriteLine(sb.ToString());
            Console.WriteLine("Saved");
        }
    }
}