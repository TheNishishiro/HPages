using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HPages.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace HPages.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public readonly HentaiDbContext _db;

        public IndexModel(ILogger<IndexModel> logger, HentaiDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public void OnGet()
        {

        }
    }
}
