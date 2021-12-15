using System.Reflection;

namespace HentaiPages.Models
{
    public class GalleryFilterNameDisplayPair
    {
        public string FieldName { get; set; }
        public string DisplayText { get; set; }
        public PropertyInfo Property { get; set; }
    }
}