using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HentaiPages.Database;
using HentaiPages.Database.Tables;
using HentaiPages.Models;
using HentaiPages.Utilities;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HentaiPages.Pages
{
    public class GalleryModel : PageModel
    {
        public readonly HentaiDbContext _db;

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 200;
        public int ImagesCount { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool ShowLikedOnly { get; set; } = false;
        public GalleryEntry[] ImageIds { get; set; }

        public GalleryModel(HentaiDbContext db)
        {
            _db = db;
        }

        public async Task OnGetAsync()
        {
            var imageQuery = _db.Images.Where(x => !ShowLikedOnly || ShowLikedOnly == x.Favourite)
                .OrderByDescending(x => x.ImageId)
                .Select(x => new {x.ImageId, x.ContentType});

            ImagesCount = imageQuery.Count();
            ImageIds = await imageQuery
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .Select(x => new GalleryEntry()
                {
                    Id = x.ImageId,
                    IsVideo = x.ContentType.Contains("mp4")
                })
                .ToArrayAsync();
        }

        public int GetPagedIndex(int id)
        {
            return PaginatedList<int>.CalculateIndex(id, CurrentPage, PageSize);
        }
    }
}
