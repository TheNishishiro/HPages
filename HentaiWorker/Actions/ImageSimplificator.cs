using System;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HentaiPages.Database;
using HentaiPages.Utilities;
using SimpleImageComparisonClassLibrary;
using SimpleImageComparisonClassLibrary.ExtensionMethods;
using SimpleImageComparisonClassLibrary.Models;

namespace HentaiWorker.Actions
{
    public static class ImageSimplificator
    {
        public static void SimplifyImages()
        {
            using var db = new HentaiDbContext();
            var imageIds = db.Images.Select(x => x.ImageId).ToList();

            var chunks = imageIds.ChunkBy(1000).Count;
            var currentChunk = 0;
            foreach (var imageIdsChunked in imageIds.ChunkBy(1000))
            {
                Console.WriteLine($"{currentChunk++}/{chunks}");
                var images = db.Images.Where(x => imageIdsChunked.Contains(x.ImageId)).ToList();
                foreach (var image in images)
                {
                    try
                    {
                        Console.WriteLine($"{image.ImageId}");
                        using var ms = new MemoryStream(ImageManager.GetData(image.ImagePath));
                        var imgFromStream = Image.FromStream(ms);
                        var img = ImageTool.ResizeImage(imgFromStream.GetGrayScaleVersion(), new Size(100,100));
                    
                        var lockBitmap = new LockBitmap((Bitmap)img);
                        image.PixelData = lockBitmap.GetPixels();
                    }
                    catch (Exception e)
                    {
                    }
                }

                db.SaveChanges();
            }
        }
    }
}