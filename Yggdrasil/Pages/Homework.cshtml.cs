using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Protocol;
using Yggdrasil.Models;

namespace Yggdrasil.Pages
{

    public class HomeworkModel : PageModel
    {
        [BindProperty]
        public List<Homework> Homeworks { get; set; }

        public void OnGet()
        {
            Homeworks = new List<Homework>()
            {
                new Homework()
                {
                    Deadline = DateTime.Now.AddDays(5), Finished = false, Description = "������� ������� 3 � �������",
                    Subject = "��������� ������������ ��������������",
                    Id = 0
                },
                new Homework()
                {
                    Deadline = DateTime.Now.AddDays(5), Finished = true, Description = "������� ������� 4 � �������",
                    Subject = "��������� ������������ ��������������",
                    Id = 1
                },
                new Homework()
                {
                    Deadline = DateTime.Now.AddDays(1), Finished = false, Description = "������� ������� 4 � �������",
                    Subject = "���������� ����������",
                    Id = 2
                }
            };
        }

        public IActionResult OnPost()
        {
            
            return Content(JsonSerializer.Serialize(Homeworks));
        }
    }
}
