using System;
using System.Linq;
using HentaiPages.Database;
using HentaiPages.Utilities;
using Microsoft.EntityFrameworkCore;

namespace HentaiUtility.Actions
{
    public static class ImageHashing
    {
        public static void Hash()
        {
            using var _db = new HentaiDbContext();

            Console.WriteLine("Getting unhashed images...");
            var imageIds = _db.Images
                .AsNoTracking()
                .Where(x => x.Hash == null)
                .Select(x => new {x.ImageId, x.ContentType})
                .ToList()
                .Where(x => !x.ContentType.Contains("gif") && !x.ContentType.Contains("mp4"))
                .Select(x => x.ImageId)
                .ToList();

            Console.WriteLine("Paging results...");
            var chunks = imageIds.ChunkBy(200);
            var chunkCount = chunks.Count;
            var currentChunkId = 0;
            foreach (var idChunk in chunks)
            {
                currentChunkId++;
                var imageInChunkId = 0;
                foreach (var imageId in idChunk)
                {
                    try
                    {
                        var image = _db.Images.FirstOrDefault(x => x.ImageId == imageId);
                        image.Hash = image.Data.Hash();
                        image.HasHash = true;
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                    Console.WriteLine($"{++imageInChunkId}/{currentChunkId}/{chunkCount}");
                }

                Console.WriteLine("Saving...");
                _db.SaveChanges();
                Console.WriteLine("Saved.");
            }
        }
    }
}