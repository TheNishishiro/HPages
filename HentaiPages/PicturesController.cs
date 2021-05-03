using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HentaiPages.Database;
using HentaiPages.Database.Tables;
using HentaiPages.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HentaiPages
{
    [Route("api/images/[action]")]
    [AllowAnonymous]
    [ApiController]
    public class PicturesController : ControllerBase
    {
        private readonly HentaiDbContext _db;

        public PicturesController(HentaiDbContext db)
        {
            _db = db;
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
            return File(image?.Data, "application/octet-stream");
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
    }
}
