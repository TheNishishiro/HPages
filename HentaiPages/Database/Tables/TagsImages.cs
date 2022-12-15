using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HentaiPages.Database.Tables
{
    public class TagsImages
    {
        public int ImageId { get; set; }
        public HImage Image { get; set; }
        public int TagsId { get; set; }
        public Tags Tag { get; set; }
    }
}
