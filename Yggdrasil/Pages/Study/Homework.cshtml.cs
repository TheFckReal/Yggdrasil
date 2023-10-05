using System.ComponentModel.DataAnnotations;
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
        [BindProperty]
        public List<InputModel> Input { get; set; }

        public HomeworkModel(IHomeworkService homeworkService)
        {
            _homeworkService = homeworkService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var receivedSubjects = await _homeworkService.GetSubjectsAsync(true);
            Input = new List<InputModel>();
            for (var i = 0; i < receivedSubjects.Count; i++)
            {
                Input.Add(new InputModel()
                {
                    Name = receivedSubjects[i].Name,
                    SubjectId = receivedSubjects[i].Id,
                    TeacherName = receivedSubjects[i].Name
                });
                Input[i].Homeworks = new List<InputModel.HomeworkInputModel>();
                foreach (var receivedHomeworks in receivedSubjects[i].Homeworks)
                {
                    Input[i].Homeworks.Add(new()
                    {
                        Deadline = receivedHomeworks.Deadline,
                        Description = receivedHomeworks.Description,
                        Finished = receivedHomeworks.Finished,
                        Id = receivedHomeworks.Id,
                        Files = receivedHomeworks.Files.ToList()
                    });
                }

            }
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            return Content(JsonSerializer.Serialize(Input));
        }

        public class InputModel
        {
            public int SubjectId { get; set; }
            public string Name { get; set; } = null!;
            public string? TeacherName { get; set; }
            public List<HomeworkInputModel> Homeworks { get; set; } = new();

            public class HomeworkInputModel
            {
                public int Id { get; set; }
                public string? Description { get; set; }
                public DateTime Deadline { get; set; }
                [Required]
                public bool Finished { get; set; }
                public List<DbModels.File>? Files { get; set; }
            }
        }
    }
}
