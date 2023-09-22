using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Protocol;
using Yggdrasil.DbModels;
using Yggdrasil.Services;

namespace Yggdrasil.Pages.Study
{

    public class HomeworkModel : PageModel
    {
        private readonly IHomeworkService _homeworkService;
        public List<Subject> Subjects { get; set; }

        [BindProperty]
        public List<Homework> homeworks { get; set; }
        public HomeworkModel(IHomeworkService homeworkService)
        {
            _homeworkService = homeworkService;
        }

        public IActionResult OnGetAsync()
        {
            Subjects = _homeworkService.GetSubjects();
            return Page();
        }

        public IActionResult OnPost()
        {
            return Content(JsonSerializer.Serialize(homeworks));
        }
    }
}
