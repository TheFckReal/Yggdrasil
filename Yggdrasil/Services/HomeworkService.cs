using Microsoft.EntityFrameworkCore;
using Yggdrasil.DbModels;

namespace Yggdrasil.Services
{
    public interface IHomeworkService
    {
        public List<Subject> GetSubjects();
        public Task<List<Subject>> GetSubjectsAsync();
    }

    public class HomeworkService : IHomeworkService
    {
        private StudyDbContext _studyDbContext;

        public HomeworkService(StudyDbContext context)
        {
            _studyDbContext = context;
        }

        public List<Subject> GetSubjects()
        {
            List<Subject> subjects = new List<Subject>();
            using (var context = _studyDbContext)
            {
                subjects = context.Subjects.Include(x => x.Homeworks).ToList();
            }

            return subjects;
        }

        public async Task<List<Subject>> GetSubjectsAsync()
        {
            List<Subject> subjects = new List<Subject>();
            await using (var context = _studyDbContext)
            {
                subjects = await context.Subjects.Include(x => x.Homeworks).ToListAsync();
            }
            return subjects;
        }
    }
}
