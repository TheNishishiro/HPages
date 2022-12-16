using System;
using System.Linq;
using HPages.Database;
using HPages.Database.Tables;
using HWorker.Services;

namespace HWorker.Actions
{
    public static class DbScanner
    {
        public static void FindDuplicates()
        {
            using var db = new HentaiDbContext();
            var settings = SettingsService.Load();
            var indexes = db.Images.Where(x => x.PixelData != null && x.ImageId > settings.LastDuplicationIndex).Select(x => x.ImageId).OrderBy(x=>x).ToList();
            
            var i = 1;
            foreach (var currentId in indexes)
            {
                Console.WriteLine($"{i++}/{indexes.Count}");
                var task = new DoTask(new WorkerTask()
                {
                    ObjectId = currentId
                }, db);

                task.DoTask_FindSimilarity();
                settings.LastDuplicationIndex = currentId;
                SettingsService.Save(settings);
            }
        }
    }
}