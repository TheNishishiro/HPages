using HentaiPages.Database;
using HentaiPages.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HentaiPages.Pages
{
    public class UploadSummary : PageModel
    {
        private readonly HentaiDbContext _db;
        public readonly IUploadService UploadService;

        public UploadSummary(HentaiDbContext db, IUploadService uploadService)
        {
            _db = db;
            UploadService = uploadService;
        }
        
        public void OnGet()
        {
            
        }
    }
}