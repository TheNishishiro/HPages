using System.Collections.Generic;

namespace HentaiPages.Models
{
    public class UploadResult
    {
        public Dictionary<string, List<long>> ImageIdsPair { get; set; }
        
        public UploadResult()
        {
            ImageIdsPair = new Dictionary<string, List<long>>();
        }
    }
}