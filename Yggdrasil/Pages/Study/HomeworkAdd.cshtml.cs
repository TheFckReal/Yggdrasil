using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Yggdrasil.DbModels;
using Yggdrasil.Services;
using static System.Net.Mime.MediaTypeNames;

namespace Yggdrasil.Pages.Study
{
    public class HomeworkAddModel : PageModel
    {
        private readonly IHomeworkService _homeworkService;
        private readonly IConfiguration _configuration;
        public HomeworkAddModel(IHomeworkService homeworkService, IConfiguration configuration)
        {
            _homeworkService = homeworkService;
            _configuration = configuration;
        }

        public Subject InputedSubject { get; set; }
        public InputModel Input { get; set; }

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
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            else
            {
                if (input.FormFiles is not null)
                {
                    if (input.FormFiles.Count() > 3)
                    {
                        ModelState.AddModelError(nameof(input.FormFiles), $"Нельзя загружать более 3 файлов");
                        return BadRequest();
                    } 

                    long sizeLimit = _configuration.GetValue<long>("FileSizeLimit");
                    string[] permittedExtensions = { ".txt", ".pdf" };
                    foreach (var file in input.FormFiles)
                    {
                        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                        if (file.Length > sizeLimit || string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError(nameof(input.FormFiles), $"Размер файла не может превышать {sizeLimit / 2048} Мб, иметь формат, отличный от .txt и .pdf, а также нельзя загружать более 3 файлов");
                            return BadRequest();
                        }
                    }
                }
            }
            if (input.FormFiles is not null)
            {
                foreach (var file in input.FormFiles)
                {
                    
                }
            }

            Homework newHomework = new Homework()
            {
                Deadline = input.Deadline!.Value,
                Description = input.Description,
                Finished = input.Finished,
                Subjectid = input.SubjectId!.Value,
            };
            foreach (var file in input.FormFiles)
            {
                using MemoryStream memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                newHomework.Files.Add(new ()
                {
                    Name = Path.GetRandomFileName(),
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
            public IEnumerable<IFormFile> FormFiles { get; set; } = null;
        }


    }

}
