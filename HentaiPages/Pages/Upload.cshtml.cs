using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HentaiPages.Database;
using HentaiPages.Database.Tables;
using HentaiPages.Services;
using HentaiPages.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace HentaiPages.Pages
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

        private readonly IUploadService _uploadService;

        public UploadModel(HentaiDbContext db, IUploadService uploadService)
        {
            _db = db;
            _uploadService = uploadService;
        }

        public async void OnGetAsync()
        {
            return;
        }

        public async Task<IActionResult> OnPostUploadFileAsync()
        {
            _uploadService.Reset();
            var currentHashes = new List<string>();
            foreach (var file in Images)
            {
                if (file.ContentType == "text/plain")
                {
                    var text = await ReadFormFileAsync(file);
                    var lines = text.Split('\n');
                    foreach (var line in lines)
                    {
                        var entry = line.Split(':');
                        if (entry.Length <= 1 || string.IsNullOrWhiteSpace(entry[1])) continue;
                
                        var indexes = entry[1].Split(',');
                        foreach (var index in indexes.Select(x => long.Parse(x.Trim())))
                        {
                            if (!_db.Images.AsNoTracking().Any(x=>x.ImageId == index)) continue;
                            var deleteImage = new Database.Tables.Image() {ImageId = index};
                            _db.Images.Remove(deleteImage);
                        }
                    }
                    await _db.SaveChangesAsync();
                    return RedirectToPage("/Index");
                }
                
                var data = await GetByteArrayFromImage(file);
                var imageHash = data.Hash();
                if (!string.IsNullOrWhiteSpace(imageHash))
                {
                    var similarIds = _db.Images.Where(x => x.Hash == imageHash).Select(x => x.ImageId).ToList();
                    if (currentHashes.Any(x => x == imageHash)) continue;

                    if (similarIds.Any() && !ForceUpload)
                    {
                        _uploadService.AddData(data, similarIds);
                        continue;
                    }

                    currentHashes.Add(imageHash);
                }

                _db.Images.Add(new Database.Tables.Image
                {
                    Data = data, Tags = new List<TagsImages>(), UploadDate = DateTime.Now,
                    ContentType = file.ContentType, Hash = imageHash
                });
            }

            using (var client = new WebClient())
            {
                if (!string.IsNullOrWhiteSpace(ImageUrl))
                {
                    foreach (var url in ImageUrl.Split('\n'))
                    {
                        var data = client.DownloadData(new Uri(url));
                        var imageHash = data.Hash();
                        var similarIds = _db.Images.Where(x => x.Hash == imageHash).Select(x => x.ImageId).ToList();
                        if (similarIds.Any() && !ForceUpload && !string.IsNullOrWhiteSpace(imageHash))
                        {
                            _uploadService.AddData(data, similarIds);
                            continue;
                        }
                        _db.Images.Add(new Database.Tables.Image
                        {
                            Data = data, Tags = new List<TagsImages>(), UploadDate = DateTime.Now,
                            ContentType = "application/octet-stream"
                        });
                    }
                }
            }

            await _db.SaveChangesAsync();
            
            if (!_uploadService.HasRepeats())
                return Page();
            return RedirectToPage("/UploadSummary");
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
