using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Yggdrasil.DbModels;
using Yggdrasil.Services;

namespace Yggdrasil.Pages.Study
{
    public class HomeworksUpdateModel : PageModel
    {
        private readonly IHomeworkService _homeworkService;

        public HomeworksUpdateModel(IHomeworkService homeworkService)
        {
            _homeworkService = homeworkService;
        }

        [BindProperty] public InputModel Input { get; set; } = new InputModel();

        public IActionResult OnGet(int id, string? subjectName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (subjectName is not null)
            {
                subjectName = char.ToUpper(subjectName[0]) + subjectName.Substring(1);
            }
            Homework? receivedHomework = _homeworkService.GetHomework(id);
            if (receivedHomework is not null)
            {
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
            }
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (!ModelState.IsValid || Input.InputHomework is null)
            {
                return BadRequest();
            }
            await _homeworkService.DeleteHomeworkAsync(Input.InputHomework.Id);
            return RedirectToPage("Homework");
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            if (!ModelState.IsValid || Input.InputHomework is null)
            {
                return Page();
            }

            await _homeworkService.UpdateHomeworkAsync(new()
            {
                Id = Input.InputHomework.Id,
                Deadline = Input.InputHomework.Deadline!.Value,
                Description = Input.InputHomework.Description,
                Finished = Input.InputHomework.Finished
            });
            return RedirectToPage("Homework");
        }

        public class InputModel
        {
            public string? SubjectName { get; set; }
            public HomeworkInputModel? InputHomework { get; set; } = null!;
            public class HomeworkInputModel
            {
                [Required]
                public int Id { get; set; }
                [MaxLength(128, ErrorMessage = "Описание задания не может превышать 128 символов")]
                public string? Description { get; set; }
                [Required(ErrorMessage = "Дедлайн должен быть задан")]
                [DataType(DataType.DateTime, ErrorMessage = "Некорректный формат ввода")]
                public DateTime? Deadline { get; set; }
                [Required]
                public bool Finished { get; set; }
            }
        }
    }
}
