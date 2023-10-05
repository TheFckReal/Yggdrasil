using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;
using Yggdrasil.DbModels;
using Yggdrasil.Services;

namespace Yggdrasil.Pages.Study
{
    public class DownloadModel : PageModel
    {
        private readonly IHomeworkService _homeworkService;

        public DownloadModel(IHomeworkService homeworkService)
        {
            _homeworkService = homeworkService;
        }

        [BindProperty(SupportsGet = true)]
        [Required]
        public int? FileId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            DbModels.File? file = await _homeworkService.GetHomeworkFileAsync(FileId!.Value);
            if (file == null)
                return BadRequest();
            string contentType = Path.GetExtension(file.Name) == ".pdf" ? "application/pdf" : "text/plain";
            return File(file.Data, contentType);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            DbModels.File? file = await _homeworkService.GetHomeworkFileAsync(FileId!.Value);
            if (file == null)
                return BadRequest();
            string contentType = Path.GetExtension(file.Name) == ".pdf" ? "application/pdf" : "text/plain";
            return File(file.Data, contentType);
        }

    }
}
