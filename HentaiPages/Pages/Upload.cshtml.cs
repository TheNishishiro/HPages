using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HentaiPages.Database;
using HentaiPages.Database.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;

namespace HentaiPages.Pages
{
    public class UploadModel : PageModel
    {
        private readonly HentaiDbContext _db;
        [BindProperty(SupportsGet = true)]
        public List<IFormFile> Images { get; set; }
        [BindProperty(SupportsGet = true)]
        public string ImageUrl { get; set; }

        public UploadModel(HentaiDbContext db)
        {
            _db = db;
        }

        public async void OnGetAsync()
        {
            return;
        }

        public async Task<IActionResult> OnPostUploadFileAsync()
        {
            foreach (var file in Images)
            {
                _db.Images.Add(new Database.Tables.Image { Data = await GetByteArrayFromImage(file), Tags = new List<TagsImages>(), UploadDate = DateTime.Now, ContentType = file.ContentType });
            }

            using (WebClient client = new WebClient())
            {
                if (!string.IsNullOrWhiteSpace(ImageUrl))
                {
                    foreach (var url in ImageUrl.Split('\n'))
                    {
                        byte[] data = client.DownloadData(new Uri(url));
                        _db.Images.Add(new Database.Tables.Image { Data = data, Tags = new List<TagsImages>(), UploadDate = DateTime.Now, ContentType = "application/octet-stream" });
                    }
                }
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
    }
}
