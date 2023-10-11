using System.Text;
using System.Text.RegularExpressions;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Yggdrasil.Pages.Study
{
    public class ScheduleModel : PageModel
    {
        private readonly HttpClient _client;
        public ScheduleModel(IHttpClientFactory clientClientFactory)
        {
            _client = clientClientFactory.CreateClient();
        }
        public string? Res { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var getResult = await _client.GetAsync(
                 $"https://kpfu.ru/week_sheadule_print?p_date={DateTime.Now.ToString("dd.MM.yy")}&p_group=&p_group_name=09-102&p_sub=6241");
            var parser = new HtmlParser();
            Encoding.GetEncoding("windows-1251");
            using (StreamReader sr = new StreamReader(await getResult.Content.ReadAsStreamAsync(),
                       Encoding.GetEncoding("windows-1251")))
            {
                var document = parser.ParseDocument(await sr.ReadToEndAsync());
                Res = document.QuerySelector("TABLE")?.OuterHtml;
                document.Close();
            }
            return Page();
        }
    }
}
