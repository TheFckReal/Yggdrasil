using Microsoft.EntityFrameworkCore;
using Yggdrasil.DbModels;
using static Yggdrasil.Services.HomeworkService;

namespace Yggdrasil.Services
{
    public interface IHomeworkService
    {
        public List<Subject> GetSubjects();
        public Subject? GetSubject(int id);
        public Task<List<Subject>> GetSubjectsAsync();
        public Homework? GetHomework(int id);
        public Task DeleteHomeworkAsync(int id);
        public Task UpdateHomeworkAsync(HomeworkForUpdate newHomework);
        public Task InsertHomeworkAsync(Homework newHomework);
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
            subjects = _studyDbContext.Subjects.Include(x => x.Homeworks).ToList();
            return subjects;
        }

        public async Task<List<Subject>> GetSubjectsAsync()
        {
            List<Subject> subjects = new List<Subject>();
            subjects = await _studyDbContext.Subjects.Include(x => x.Homeworks).ToListAsync();
            return subjects;
        }

        public async Task DeleteHomeworkAsync(int id)
        {
            await _studyDbContext.Homeworks.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task UpdateHomeworkAsync(HomeworkForUpdate newHomework)
        {
            var dbHomework = await _studyDbContext.Homeworks.FirstOrDefaultAsync(x => x.Id == newHomework.Id);
            if (dbHomework is not null)
            {
                dbHomework.Finished = newHomework.Finished;
                dbHomework.Deadline = newHomework.Deadline;
                dbHomework.Description = newHomework.Description;
                await _studyDbContext.SaveChangesAsync();
            }
        }

        public async Task InsertHomeworkAsync(Homework newHomework)
        {
            await _studyDbContext.AddAsync(newHomework);
            await _studyDbContext.SaveChangesAsync();
        }
    }

    public class HomeworkForUpdate
    {
        public int Id { get; set; }
        public bool Finished { get; set; }
        public string? Description { get; set; }
        public DateTime Deadline { get; set; }
    }
}
