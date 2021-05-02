using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HentaiPages.Database.Tables
{
    public class Image
    {
        public long ImageId { get; set; }
        public byte[] Data { get; set; }
        public string ContentType { get; set; }
        public DateTime UploadDate { get; set; }
        public bool Favourite { get; set; }
        public ICollection<TagsImages> Tags { get; set; }
    }
}
