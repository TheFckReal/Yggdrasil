using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using Yggdrasil.DbModels;
using Yggdrasil.OptionsModels;
using Yggdrasil.Services;
using static System.Net.Mime.MediaTypeNames;
using File = System.IO.File;

namespace Yggdrasil.Pages.Study
{
    public class HomeworkAddModel : PageModel
    {
        private readonly IHomeworkService _homeworkService;
        private readonly IOptions<FileSettings> _fileSettings;
        public HomeworkAddModel(IHomeworkService homeworkService, IOptions<FileSettings> fileOption)
        {
            _homeworkService = homeworkService;
            _fileSettings = fileOption;
        }

        public Subject InputedSubject { get; set; } = null!;
        public InputModel Input { get; set; } = null!;

        public IActionResult OnPost([Required] int? subjectId)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("Homework");
            }
            var foundedSubject = _homeworkService.GetSubject(subjectId!.Value);
            if (foundedSubject is null)
                return BadRequest();
            InputedSubject = foundedSubject;
            return Page();
        }

        public async Task<IActionResult> OnPostInsertAsync([Required] InputModel input)
        {
            Input = input;
            InputedSubject = new() { Name = input?.SubjectName ?? "Default" };
            if (!ModelState.IsValid)
            {
                return Page();
            }
            else
            {
                if (input!.FormFiles is not null)
                {
                    int maxFiles = _fileSettings.Value.MaxFilesPerHomework;
                    if (input.FormFiles.Count() > maxFiles)
                    {
                        ModelState.AddModelError(nameof(input.FormFiles), $"Нельзя загружать более {maxFiles} файлов");
                        return Page();
                    }

                    long sizeLimit = _fileSettings.Value.FileSizeLimit;
                    var permittedExtensions = _fileSettings.Value.AllowedExtensions!;
                    foreach (var file in input.FormFiles)
                    {
                        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                        if (file.Length > sizeLimit || string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError(nameof(input.FormFiles), $"Размер файла не может превышать {sizeLimit / (1024 * 1024)} Мб, иметь формат, отличный от .txt и .pdf, а также нельзя загружать более {maxFiles} файлов");
                            return Page();
                        }
                    }
                }
            }

            Homework newHomework = new Homework()
            {
                Deadline = input.Deadline!.Value,
                Description = input.Description,
                Finished = input.Finished,
                Subjectid = input.SubjectId!.Value,
            };
            if (input.FormFiles is not null)
                foreach (var file in input.FormFiles)
                {
                    using MemoryStream memoryStream = new MemoryStream();
                    await file.CopyToAsync(memoryStream);
                    newHomework.Files.Add(new()
                    {
                        Name = Path.GetFileName(file.FileName),
                        Data = memoryStream.ToArray(),
                    });
                }

            await _homeworkService.InsertHomeworkAsync(newHomework);
            return RedirectToPage("Homework");
        }

        public class InputModel
        {
            [Required]
            public int? SubjectId { get; set; }
            [MaxLength(128)]
            public string? Description { get; set; } = null!;
            [Required]
            [DataType(DataType.DateTime)]
            public DateTime? Deadline { get; set; }
            public bool Finished { get; set; }
            public IEnumerable<IFormFile>? FormFiles { get; set; } = null!;
            public string? SubjectName { get; set; } = null;
        }


    }

}
