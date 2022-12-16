using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HPages.Database;
using HPages.Database.Tables;
using HPages.Models;
using HPages.Services;
using HPages.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HPages.Pages
{
    [BindProperties(SupportsGet = true)]
    public class GalleryModel : PageModel
    {
        public readonly HentaiDbContext _db;

        private readonly IFilterService _filterService;
        public List<string> 
            AvailableTags = typeof(GalleryFilter).GetProperties().Select(x => x.Name).ToList();
        
        public GalleryModel(HentaiDbContext db, IFilterService filterService)
        {
            _db = db;
            _filterService = filterService;
        }

        [BindProperty(SupportsGet = true)] public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)] public int PageSize { get; set; } = 200;

        public int ImagesCount { get; set; }

        [BindProperty(SupportsGet = true)] public bool ShowLikedOnly { get; set; } = false;

        public GalleryEntry[] ImageIds { get; set; }

        [BindProperty(SupportsGet = true)] public GalleryFilter GalleryFilter { get; set; }

        public async Task OnGetAsync()
        {
            GalleryFilter = _filterService.GetCurrentFilters();
            await PageImages();
        }

        private async Task PageImages()
        {
            var imageQuery = _db.Images
                .AsNoTracking()
                .Include(x => x.Tags)
                .ThenInclude(x => x.Tag)
                .AsQueryable();
            imageQuery = FilterQuery(imageQuery);
            imageQuery = imageQuery.OrderByDescending(x => x.ImageId)
                .Select(x => new HImage {ImageId = x.ImageId, ContentType = x.ContentType});

            ImagesCount = await imageQuery.CountAsync();
            ImageIds = await imageQuery
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .Select(x => new GalleryEntry
                {
                    Id = x.ImageId,
                    IsVideo = x.ContentType.Contains("mp4")
                })
                .ToArrayAsync();
        }

        public async Task<IActionResult> OnPostApplyFiltersAsync()
        {
            _filterService.SetData(GalleryFilter);
            await PageImages();
            return Page();
        }

        public async Task<IActionResult> OnPostResetFiltersAsync()
        {
            _filterService.Reset();
            GalleryFilter = _filterService.GetCurrentFilters();
            await PageImages();
            return Page();
        }

        public async Task<IActionResult> OnPostUntaggedFilterAsync()
        {
            foreach (var propertyInfo in GalleryFilter.GetType().GetProperties())
            {
                propertyInfo.SetValue(GalleryFilter, false);
            }
            
            _filterService.SetData(GalleryFilter);
            await PageImages();
            return Page();
        }

        public int GetPagedIndex(int id)
        {
            return PaginatedList<int>.CalculateIndex(id, CurrentPage, PageSize);
        }

        private IQueryable<HImage> FilterQuery(IQueryable<HImage> query)
        {
            var filterProperties = GalleryFilter.GetType().GetProperties();
            foreach (var property in filterProperties)
            {
                var filterName = property.Name;
                if (filterName == "ShowLiked")
                    continue;
                
                var filterValue = (bool?)property.GetValue(GalleryFilter);
                query = filterValue switch
                {
                    true => query.Where(x => x.Tags.Any(z => z.Tag.Name == filterName)),
                    false => query.Where(x => x.Tags.All(z => z.Tag.Name != filterName)),
                    _ => query
                };
            }
            
            query = GalleryFilter.ShowLiked switch
            {
                true => query.Where(x => x.Favourite),
                false => query.Where(x => !x.Favourite),
                _ => query
            };
            

            return query;
        }
    }
}