using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HPages.Database;
using HPages.Database.Tables;
using HPages.Models.Enums;
using HPages.Services;
using HPages.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SimpleImageComparisonClassLibrary;
using SimpleImageComparisonClassLibrary.ExtensionMethods;
using SimpleImageComparisonClassLibrary.Models;
using WebP.Net;

namespace HPages.Pages
{
    public class UploadModel : PageModel
    {
        private readonly HentaiDbContext _db;
        [BindProperty(SupportsGet = true)]
        public List<IFormFile> Images { get; set; }
        [BindProperty(SupportsGet = true)]
        public string ImageUrl { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool ForceUpload { get; set; }
        public int TasksToRun { get; set; }

        private readonly IUploadService _uploadService;

        public string[] IgnoredTypes = new[]
        {
            "video/mp4",
            "video/webm",
            "image/gif",
        };

        public UploadModel(HentaiDbContext db, IUploadService uploadService)
        {
            _db = db;
            _uploadService = uploadService;
        }

        public async void OnGetAsync()
        {
            TasksToRun = await _db.Tasks.CountAsync(x => x.FinishDate == null);
            return;
        }

        public async Task<IActionResult> OnPostUploadFileAsync()
        {
            var images = new List<HImage>();
            foreach (var file in Images)
            {
                var data = await GetByteArrayFromImage(file);

                try
                {
                    string hash = null;
                    if (!IgnoredTypes.Contains(file.ContentType))
                    {
                        Image img;
                        if (file.ContentType == "image/webp")
                        {
                            using var webp = new WebPObject(data);
                            img = ImageTool.ResizeImage(webp.GetImage().GetGrayScaleVersion(), new Size(100, 100));
                        }
                        else
                        {
                            await using var ms = new MemoryStream(data);
                            using var imgFromStream = Image.FromStream(ms);
                            img = ImageTool.ResizeImage(imgFromStream.GetGrayScaleVersion(), new Size(100, 100));
                        }

                        var lockBitmap = new LockBitmap((Bitmap) img);
                        hash = lockBitmap.GetPixels();
                        if (_db.Images.Any(x => x.PixelData == hash) && !ForceUpload)
                            continue;
                    }

                    images.Add(new HImage
                    {
                        ImagePath = ImageManager.ExtractToPhysicalPath(_db, data), 
                        Tags = new List<TagsImages>(), 
                        UploadDate = DateTime.Now,
                        ContentType = file.ContentType,
                        PixelData = hash
                    });
                }
                catch (Exception e)
                {
                    throw new Exception($"{e.Message}\n{e.StackTrace}\n{e.InnerException?.Message}\n{file.FileName}\n{file.ContentType}");
                }
            }

            using (var client = new WebClient())
            {
                if (!string.IsNullOrWhiteSpace(ImageUrl))
                {
                    foreach (var url in ImageUrl.Split('\n'))
                    {
                        var data = client.DownloadData(new Uri(url));
                        images.Add(new HImage
                        {
                            ImagePath = ImageManager.ExtractToPhysicalPath(_db, data),
                            Tags = new List<TagsImages>(), 
                            UploadDate = DateTime.Now,
                            ContentType = "application/octet-stream"
                        });
                    }
                }
            }

            _db.Images.AddRange(images);
            await _db.SaveChangesAsync();

            foreach (var insertedImage in images)
            {
                _db.Tasks.Add(new WorkerTask()
                {
                    Type = TaskType.FindSimilar,
                    ObjectId = insertedImage.ImageId,
                    PostDate = DateTime.Now
                });
            }
            await _db.SaveChangesAsync();
            
            return Page();
        }

        private async Task<byte[]> GetByteArrayFromImage(IFormFile file)
        {
            using (var target = new MemoryStream())
            {
                await file.CopyToAsync(target);
                return target.ToArray();
            }
        }
        
        public static async Task<string> ReadFormFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return await Task.FromResult((string)null);
            }
    
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
