using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HPages.Database;
using HPages.Database.Tables;
using HPages.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HPages.Pages
{
    public class SimilaritySummary : PageModel
    {
        private readonly HentaiDbContext _db;
        public Dictionary<int, List<int>> Duplications { get; set; }

        public SimilaritySummary(HentaiDbContext db)
        {
            Duplications = new Dictionary<int, List<int>>();
            _db = db;
        }
        
        public async void OnGetAsync()
        {
            var duplications = _db.SimilarityScores.ToList().GroupBy(x=>x.ParentImageId).Select(x=>new {x.Key, duplications = x.Select(z=>z.ChildImageId).Distinct()}).ToList();
            foreach (var duplication in duplications)
            {
                if (!Duplications.ContainsKey(duplication.Key))
                    Duplications.Add(duplication.Key, new List<int>());
                foreach (var dup in duplication.duplications)
                    Duplications[duplication.Key].Add(dup);
            }
        }

        public async Task<IActionResult> OnPostResolveAsync()
        {
            var duplications = _db.SimilarityScores
                .AsNoTracking()
                .Where(x=>x.SimilarityScore < 0.01)
                .AsEnumerable()
                .GroupBy(x => x.ParentImageId)
                .Select(x=> new {oldImageId = x.Key, duplicateIds = x.Select(z=>z.ChildImageId).Distinct().ToList()})
                .Distinct()
                .ToList();

            var pathToDelete = new List<string>();

            foreach (var duplicateList in duplications.Select(duplication => _db.Images
                         .AsNoTracking()
                         .Where(x => 
                             x.ImageId == duplication.oldImageId || duplication.duplicateIds.Contains(x.ImageId))
                            .AsEnumerable()
                            .Select(x =>
                            {
                                var imageData = ImageManager.GetData(x.ImagePath);
                                Image imgFromStream = null;
                                try
                                {
                                    if (imageData is not null)
                                    {
                                        using var ms = new MemoryStream(imageData);
                                        imgFromStream = Image.FromStream(ms);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }

                                var Width = imageData is null ? -1 : (imgFromStream?.Width).GetValueOrDefault();
                                var Height = imageData is null ? -1 : (imgFromStream?.Height).GetValueOrDefault();
                                return new {Width, Height, x.ImageId, x.ImagePath};
                            })
                            .OrderByDescending(x=>x.Height+x.Width)
                            .Select(x=>new{x.Width, x.Height, x.ImageId, x.ImagePath})
                            .ToList()))
            {
                foreach (var duplicateIdToDelete in duplicateList.Skip(1))
                {
                    pathToDelete.Add(duplicateIdToDelete.ImagePath);
                    _db.Remove(new HImage(){ImageId = duplicateIdToDelete.ImageId});
                    _db.RemoveRange(_db.SimilarityScores.Where(x =>
                        x.ChildImageId == duplicateIdToDelete.ImageId || x.ParentImageId == duplicateIdToDelete.ImageId));
                }
            }

            await _db.SaveChangesAsync();
            foreach (var path in pathToDelete)
            {
                ImageManager.DeleteData(path);
            }
            return Page();
        }
    }
}