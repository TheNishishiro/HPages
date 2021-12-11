using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HentaiPages.Database;
using HentaiPages.Database.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using SimpleImageComparisonClassLibrary;
using Image = HentaiPages.Database.Tables.Image;

namespace HentaiPages
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
            var image = await _db.Images.FirstOrDefaultAsync(c => c.ImageId == id);
            image.Data = null;
            return image;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult> GetImageDataById(long id)
        {
            var image = await _db.Images.FirstOrDefaultAsync(c => c.ImageId == id);
            return File(image?.Data, image?.ContentType);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<bool>> ToggleLikeImage(long id)
        {
            var image = await _db.Images.FirstOrDefaultAsync(c => c.ImageId == id);
            
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
            var image = await _db.Images.FirstOrDefaultAsync(c => c.ImageId == id);

            if (image == null)
                return false;

            _db.Remove(image);
            await _db.SaveChangesAsync();

            return true;
        }

        [HttpGet]
        public async Task<long> Random()
        {
            var ids = await _db.Images.Select(x => x.ImageId).ToListAsync();
            var id = ids[rnd.Next(ids.Count)];
            return id;
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

            int processed = 0;
            using var sw = new StreamWriter(@"D:\compareInfo.txt");

            foreach (var i in ids)
            {
                var imageData = await _db.Images.Where(c => c.ImageId == i).Select(x=>x.Data).FirstOrDefaultAsync();

                try
                {
                    using var ms2 = new MemoryStream(imageData);
                    var imgFromStream = System.Drawing.Image.FromStream(ms2);
                    var img = ImageTool.ResizeImage(imgFromStream, commonSize);
                    sw.WriteLine($"{i}/{ids.Count}");
                    var difference = ImageTool.GetPercentageDifference(source, img);
                    if (difference < 0.1f)
                        duplicates.Add(i);
                }
                catch (Exception)
                {
                }
            }
            return duplicates;
        }
    }
}
