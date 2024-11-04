using Data;
using Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Host.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
      

        public IndexModel(ILogger<IndexModel> logger , ApplicationDbContext context)
        {
            _logger = logger;
           
        }

        public void OnGet()
        {
           
        }
    }
}