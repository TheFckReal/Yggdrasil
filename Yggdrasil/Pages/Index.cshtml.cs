using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Yggdrasil.Pages
{
    public class IndexModel : PageModel
    {
        public string helloMessage { get; set; }
        public void OnGet()
        {
            helloMessage = "Hello from Get!";
        }
    }
}
