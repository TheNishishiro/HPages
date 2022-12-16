using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using HPages.Database;
using HPages.Database.Tables;
using HPages.Models.Enums;
using Microsoft.EntityFrameworkCore;
using SimpleImageComparisonClassLibrary;
using SimpleImageComparisonClassLibrary.ExtensionMethods;
using SimpleImageComparisonClassLibrary.Models;

namespace HWorker.Actions
{
    public class DoTask
    {
        private readonly WorkerTask _workerTask;
        private readonly HentaiDbContext _dbContext;
        public DoTask(WorkerTask workerTask, HentaiDbContext dbContext)
        {
            _workerTask = workerTask;
            _dbContext = dbContext;
        }
        
        public void DoTask_FindSimilarity()
        {
            const float similarityThreshold = 0.05f;
            var image = _dbContext.Images.AsNoTracking()
                .Where(c => c.ImageId == _workerTask.ObjectId)
                .Select(x=>new {x.PixelData, x.ImageId})
                .FirstOrDefault();
            if (image == null || string.IsNullOrEmpty(image.PixelData))
                return;
            
            var isDuplicate = _dbContext.SimilarityScores.Any(x => x.ChildImageId == image.ImageId);
            if (isDuplicate)
                return;
            
            var commonSize = new Size(100, 100);
            
            var lockBitmapSource = new LockBitmap(new Bitmap(commonSize.Width, commonSize.Height));
            lockBitmapSource.SetPixels(image.PixelData, commonSize.Width);
            var source = lockBitmapSource.GetBitmap();
            
            var ids = _dbContext.Images.AsNoTracking().Where(c => c.ImageId != _workerTask.ObjectId)
                .Select(x => x.ImageId).ToList().OrderBy(x=>x);

            
            foreach (var i in ids.Chunk(500))
            {
                var imageDatas = _dbContext.Images.AsNoTracking().Where(c => i.Contains(c.ImageId)).Select(x=>new{x.PixelData, x.ImageId}).ToList();
                foreach (var imageData in imageDatas)
                {
                    if (imageData?.PixelData is null) continue;

                    try
                    {
                        var lockBitmap = new LockBitmap(new Bitmap(commonSize.Width, commonSize.Height));
                        lockBitmap.SetPixels(imageData.PixelData, commonSize.Width);
                        var similarityScore = ImageTool.GetPercentageDifference(source, lockBitmap.GetBitmap());

                        if (similarityScore <= similarityThreshold)
                        {
                            var t = _dbContext.SimilarityScores.AsNoTracking().Any(x => x.ParentImageId == imageData.ImageId);
                            var newSimilarity = new Similarity()
                            {
                                ParentImageId = _workerTask.ObjectId.GetValueOrDefault(),
                                ChildImageId = imageData.ImageId,
                                SimilarityScore = similarityScore
                            };

                            if (!t)
                            {
                                _dbContext.SimilarityScores.Add(newSimilarity);
                            }
                            else
                            {
                                newSimilarity.ParentImageId = imageData.ImageId;
                                newSimilarity.ChildImageId = _workerTask.ObjectId.GetValueOrDefault();
                                _dbContext.SimilarityScores.Add(newSimilarity);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            _dbContext.SaveChanges();

            _workerTask.ResultId = TaskResponse.Success;
        }

        public void DoTask_HashImage()
        {
            Console.WriteLine($"{_workerTask.ObjectId}");
            var image = _dbContext.Images.FirstOrDefault(x => x.ImageId == _workerTask.ObjectId);

            if (image?.ImagePath != null)
            {
                using var ms = new MemoryStream(System.IO.File.ReadAllBytes(image.ImagePath));
                using var imgFromStream = Image.FromStream(ms);
                var img = ImageTool.ResizeImage(imgFromStream.GetGrayScaleVersion(), new Size(100,100));
                    
                var lockBitmap = new LockBitmap((Bitmap)img);
                image.PixelData = lockBitmap.GetPixels();
            }

            _dbContext.SaveChanges();
            
            _workerTask.ResultId = TaskResponse.Success;
        }
    }
}