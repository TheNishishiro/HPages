using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HentaiPages.Database;
using HentaiPages.Database.Tables;
using HentaiPages.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleImageComparisonClassLibrary;

namespace HentaiPages.Controllers
{
    [Route("api/images/[action]")]
    [AllowAnonymous]
    [ApiController]
    public class PicturesController : ControllerBase
    {
        private readonly HentaiDbContext _db;
        private readonly Random rnd;

        public PicturesController(HentaiDbContext db)
        {
            _db = db;
            rnd = new Random();
        }

        [HttpGet("{id}")]
        public async Task<HImage> GetImageById(long id)
        {
            var image = await _db.Images
                .AsNoTracking()
                .Where(c => c.ImageId == id)
                .Select(x=>new HImage()
                {
                    ContentType = x.ContentType,
                    Favourite = x.Favourite,
                    ImageId = x.ImageId,
                    UploadDate = x.UploadDate,
                    ImagePath = x.ImagePath,
                    Tags = x.Tags
                })
                .FirstOrDefaultAsync();
            return image;
        }

        [HttpGet("{id}")]
        public async Task<List<string>> GetImageTagsById(long id)
        {
            return (await _db.Images
                .AsNoTracking()
                .Where(c => c.ImageId == id)
                .Include(x=>x.Tags)
                .ThenInclude(x=>x.Tag)
                .Select(x=>new HImage()
                {
                    ContentType = x.ContentType,
                    Favourite = x.Favourite,
                    ImageId = x.ImageId,
                    UploadDate = x.UploadDate,
                    Tags = x.Tags
                })
                .FirstOrDefaultAsync())
                ?.Tags?.Select(x=>x.Tag?.Name).ToList();
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult> GetImageDataById(long id)
        {
            var image = await _db.Images.AsNoTracking().Where(c => c.ImageId == id)
                .Select(x=>new {x.ImagePath, x.ContentType}).FirstOrDefaultAsync();
            if (image is null)
                return NotFound();
            Response.Headers.Add("Content-Disposition", "inline");
            var fileResult = new FileContentResult(ImageManager.GetData(image.ImagePath), image.ContentType);
            fileResult.EnableRangeProcessing = true;
            return fileResult;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<bool>> ToggleLikeImage(long id)
        {
            var image = await _db.Images.AsNoTracking().FirstOrDefaultAsync(c => c.ImageId == id);
            
            if (image == null)
                return false;

            image.Favourite = !image.Favourite;
            _db.Update(image);
            await _db.SaveChangesAsync();

            return true;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<bool>> DeleteImage(long id)
        {
            var image = await _db.Images.AsNoTracking().FirstOrDefaultAsync(c => c.ImageId == id);
            _db.RemoveRange(_db.SimilarityScores.Where(x => x.ChildImageId == id || x.ParentImageId == id).ToList());
            if (image is not null)
            {
                ImageManager.DeleteData(image.ImagePath);
                _db.Remove(image);
            }

            await _db.SaveChangesAsync();
            return true;
        }

        [HttpGet]
        public async Task<long> Random()
        {
            var ids = await _db.Images.AsNoTracking().Select(x => x.ImageId).ToListAsync();
            var id = ids[rnd.Next(ids.Count)];
            return id;
        }

        [HttpPost("{id}")]
        [Consumes("application/json")]
        public async Task SetTags([FromRoute] long id, [FromBody] List<string> activeTags)
        {
            var tags = activeTags.Select(x => new Tags()
            {
                Name = x
            }).ToList();

            var image = await _db.Images
                .AsNoTracking()
                .Where(c => c.ImageId == id)
                .Include(x=>x.Tags)
                .ThenInclude(x=>x.Tag)
                .FirstOrDefaultAsync();
            if (image.Tags is null)
                image.Tags = new List<TagsImages>();
            _db.Tags.RemoveRange(image.Tags.Select(x=>x.Tag));
            var imageTags = tags.Select(x => new TagsImages()
            {
                Image = image,
                Tag = x
            }).ToList();
            
            image.Tags = imageTags;
            _db.Update(image);
            await _db.SaveChangesAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<long>>> FindSimilar(long id)
        {
            var duplicates = new List<long>();
            
            var image = await _db.Images.FirstOrDefaultAsync(c => c.ImageId == id);
            if (image == null)
                return duplicates;

            var commonSize = new Size(500, 500);
            
            using var ms = new MemoryStream(ImageManager.GetData(image.ImagePath));
            var source = ImageTool.ResizeImage(System.Drawing.Image.FromStream(ms), commonSize);

            var ids = await _db.Images.Where(c => c.ImageId != id).Select(x=>x.ImageId).ToListAsync();

            var processed = 0;
            using var sw = new StreamWriter(@"./compareInfo.txt");

            foreach (var i in ids)
            {
                var imageData = await _db.Images.Where(c => c.ImageId == i).Select(x=>x.ImagePath).FirstOrDefaultAsync();

                try
                {
                    using var ms2 = new MemoryStream(ImageManager.GetData(image.ImagePath));
                    var imgFromStream = System.Drawing.Image.FromStream(ms2);
                    var img = ImageTool.ResizeImage(imgFromStream, commonSize);
                    sw.WriteLine($"{++processed}/{ids.Count}");
                    var difference = ImageTool.GetPercentageDifference(source, img);
                    if (difference < 0.1f)
                        duplicates.Add(i);
                }
                catch (Exception)
                {}
            }
            return duplicates;
        }
    }
}
