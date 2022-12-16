using System.Collections.Generic;

namespace HPages.Models
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