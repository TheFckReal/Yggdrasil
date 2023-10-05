using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Yggdrasil.DbModels;
using Yggdrasil.OptionsModels;
using Yggdrasil.Services;

namespace Yggdrasil.Pages.Study
{
    public class HomeworksUpdateModel : PageModel
    {
        private readonly IHomeworkService _homeworkService;
        private readonly IOptions<FileSettings> _fileSettings;
        private readonly IFileService _fileService;
        public HomeworksUpdateModel(IHomeworkService homeworkService, IOptions<FileSettings> fileSettings, IFileService fileService)
        {
            _homeworkService = homeworkService;
            _fileSettings = fileSettings;
            _fileService = fileService;
        }

        [BindProperty] public InputModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, string? subjectName)
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
                        Finished = receivedHomework.Finished,
                    }
                };
                var exisingFiles = await _homeworkService.GetNoDataFilesByHomeworkIdAsync(id);
                foreach (var file in exisingFiles)
                {
                    Input.InputHomework.ExistingFiles.Add(new()
                    {
                        Id = file.Id,
                        Name = file.Name
                    });
                }

                Input.InputHomework.NewFiles = new(new IFormFile[_fileSettings.Value.MaxFilesPerHomework - exisingFiles.Count()]); // Ошибка с capacity и индексатором
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

            if (!_fileService.IsFilesValidForHomework(Input.InputHomework.NewFiles!, true))
            {
                ModelState.AddModelError(nameof(Input.InputHomework.NewFiles), _fileService.ErrorMessageForHomework());
                return Page();
            } else if (Input.InputHomework.NewFiles.Count + Input.InputHomework.ExistingFiles.Count > _fileSettings.Value.MaxFilesPerHomework)
            {
                ModelState.AddModelError(nameof(Input.InputHomework.NewFiles), _fileService.ErrorMessageForHomework());
                return Page();
            }

            HomeworkForUpdate updateHomework = new()
            {
                Id = Input.InputHomework.Id,
                Deadline = Input.InputHomework.Deadline!.Value,
                Description = Input.InputHomework.Description,
                Finished = Input.InputHomework.Finished
            };
            if (Input.InputHomework.NewFiles is not null)
            {
                updateHomework.Files = new();
                foreach (var file in Input.InputHomework.NewFiles)
                {
                    using MemoryStream memoryStream = new MemoryStream();
                    await file.CopyToAsync(memoryStream);
                    updateHomework.Files.Add(new()
                    {
                        Name = Path.GetFileName(file.FileName),
                        Data = memoryStream.ToArray(),
                    });
                }
            }
            await _homeworkService.UpdateHomeworkAsync(updateHomework);
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

                public List<IFormFile> NewFiles { get; set; } = new();
                public List<InputFile> ExistingFiles { get; set; } = new();
                public class InputFile
                {
                    [Required]
                    [MaxLength(50, ErrorMessage = "Название файла не может превышать 50 символов")]
                    public string Name { get; set; } = null!;
                    [Required]
                    public int Id { get; set; }
                }
            }
        }
    }
}
