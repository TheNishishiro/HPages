using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HentaiPages.Database;
using HentaiPages.Database.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleImageComparisonClassLibrary;
using Image = HentaiPages.Database.Tables.Image;

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
        public async Task<Image> GetImageById(long id)
        {
            var image = await _db.Images
                .AsNoTracking()
                .Where(c => c.ImageId == id)
                .Select(x=>new Image()
                {
                    ContentType = x.ContentType,
                    Favourite = x.Favourite,
                    ImageId = x.ImageId,
                    UploadDate = x.UploadDate
                })
                .FirstOrDefaultAsync();
            image.Data = null;
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
                .Select(x=>new Image()
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
            var image = await _db.Images.AsNoTracking().Where(c => c.ImageId == id).Select(x=>new {x.Data, x.ContentType}).FirstOrDefaultAsync();
            return File(image?.Data, image?.ContentType);
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

            if (image == null)
                return false;

            _db.Remove(image);
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
            
            using var ms = new MemoryStream(image.Data);
            var source = ImageTool.ResizeImage(System.Drawing.Image.FromStream(ms), commonSize);

            var ids = await _db.Images.Where(c => c.ImageId != id).Select(x=>x.ImageId).ToListAsync();

            var processed = 0;
            using var sw = new StreamWriter(@"./compareInfo.txt");

            foreach (var i in ids)
            {
                var imageData = await _db.Images.Where(c => c.ImageId == i).Select(x=>x.Data).FirstOrDefaultAsync();

                try
                {
                    using var ms2 = new MemoryStream(imageData);
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
