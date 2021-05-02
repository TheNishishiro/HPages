using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HentaiPages.Database.Tables
{
    public class TagsImages
    {
        public long ImageId { get; set; }
        public Image Image { get; set; }
        public int TagsId { get; set; }
        public Tags Tag { get; set; }
    }
}
