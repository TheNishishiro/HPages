using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HPages.Database.Tables
{
    public class HImage
    {
        public int ImageId { get; set; }
        public string PixelData { get; set; }
        public string ContentType { get; set; }
        public DateTime UploadDate { get; set; }
        public bool Favourite { get; set; }
        public string ImagePath { get; set; }
        public ICollection<TagsImages> Tags { get; set; }
    }
}
