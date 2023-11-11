using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GOPH.Areas.Identity.Pages.Account.Manager
{
    public class IndexModel : PageModel
    {
        public  IActionResult OnGet()
        {
            return RedirectToPage("/Account/Manager/ProFile");
        }
    }
}
