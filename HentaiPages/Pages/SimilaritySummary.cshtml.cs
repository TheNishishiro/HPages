using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HentaiPages.Database;
using HentaiPages.Database.Tables;
using HentaiPages.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HentaiPages.Pages
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

            foreach (var duplicateList in duplications.Select(duplication => _db.Images
                         .AsNoTracking()
                         .Where(x => 
                             x.ImageId == duplication.oldImageId || duplication.duplicateIds.Contains(x.ImageId))
                            .AsEnumerable()
                            .Select(x =>
                            {
                                var imageData = ImageManager.GetData(x.ImagePath);
                                using var ms = new MemoryStream(imageData);
                                var imgFromStream = Image.FromStream(ms);
    
                                return new {imgFromStream.Width, imgFromStream.Height, x.ImageId, x.ImagePath};
                            })
                            .OrderByDescending(x=>x.Height+x.Width)
                            .Select(x=>new{x.Width, x.Height, x.ImageId, x.ImagePath})
                            .ToList()))
            {
                foreach (var duplicateIdToDelete in duplicateList.Skip(1))
                {
                    ImageManager.DeleteData(duplicateIdToDelete.ImagePath);
                    _db.Remove(new HImage(){ImageId =duplicateIdToDelete.ImageId});
                    _db.RemoveRange(_db.SimilarityScores.Where(x =>
                        x.ChildImageId == duplicateIdToDelete.ImageId || x.ParentImageId == duplicateIdToDelete.ImageId));
                }
            }

            await _db.SaveChangesAsync();
            return Page();
        }
    }
}