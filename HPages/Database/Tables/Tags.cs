using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HPages.Database.Tables
{
    public class Tags
    {
        public int TagsId { get; set; }
        public string Name { get; set; }
        public ICollection<TagsImages> Images { get; set; }
    }
}
