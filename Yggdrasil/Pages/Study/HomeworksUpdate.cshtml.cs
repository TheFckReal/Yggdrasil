using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Yggdrasil.DbModels;
using Yggdrasil.Services;

namespace Yggdrasil.Pages.Study
{
    public class HomeworksUpdateModel : PageModel
    {
        [BindProperty]
        public int Id { get; set; }
        public IHomeworkService _homeworkService { get; init; }

        public HomeworksUpdateModel(IHomeworkService homeworkService)
        {
            _homeworkService = homeworkService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IActionResult OnGet(int id, string? subjectName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var receivedHomework = _homeworkService.GetHomework(id) ?? new Homework();
            Input = new InputModel()
            {
                SubjectName = subjectName,
                InputHomework = new()
                {
                    Deadline = receivedHomework.Deadline,
                    Description = receivedHomework.Description,
                    Id = receivedHomework.Id,
                    Finished = receivedHomework.Finished
                }
            };
            return Page();
        }

        public class InputModel
        {
            public string SubjectName { get; set; } = null!;
            public HomeworkInputModel InputHomework { get; set; } = null!;
            public class HomeworkInputModel
            {
                [Required]
                public int Id { get; set; }
                public string? Description { get; set; }
                [Required]
                [DataType(DataType.DateTime)]
                public DateTime Deadline { get; set; }
                [Required]
                public bool Finished { get; set; }
            }
        }
    }
}
