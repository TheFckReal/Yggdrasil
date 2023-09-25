using Microsoft.EntityFrameworkCore;
using Yggdrasil.DbModels;

namespace Yggdrasil.Services
{
    public interface IHomeworkService
    {
        public List<Subject> GetSubjects();
        public Subject? GetSubject(int id);
        public Task<List<Subject>> GetSubjectsAsync();
        public Homework? GetHomework(int id);
    }

    public class HomeworkService : IHomeworkService
    {
        private StudyDbContext _studyDbContext;

        public HomeworkService(StudyDbContext context)
        {
            _studyDbContext = context;
        }

        public Homework? GetHomework(int id)
        {
            Homework? homework = (from h in _studyDbContext.Homeworks where h.Id == id select h).FirstOrDefault();
            return homework;
        }

        public Subject? GetSubject(int id)
        {
            var subject = (from subj in _studyDbContext.Subjects join homework in _studyDbContext.Homeworks on subj.Id equals homework.Subjectid where subj.Id == id select subj).FirstOrDefault();
            return subject;
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
