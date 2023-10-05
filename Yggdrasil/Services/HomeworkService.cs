using Microsoft.EntityFrameworkCore;
using Yggdrasil.DbModels;
using static Yggdrasil.Services.HomeworkService;
using File = Yggdrasil.DbModels.File;

namespace Yggdrasil.Services
{
    public interface IHomeworkService
    {
        public List<Subject> GetSubjects(bool withFiles = false);
        public Subject? GetSubject(int id);
        public Task<List<Subject>> GetSubjectsAsync(bool withFiles = false);
        public Homework? GetHomework(int id, bool includeFiles = false);
        public Task DeleteHomeworkAsync(int id);
        public Task UpdateHomeworkAsync(HomeworkForUpdate newHomework);
        public Task InsertHomeworkAsync(Homework newHomework);
        public Task<File?> GetHomeworkFileAsync(int id);
        public Task<IEnumerable<NoDataFile>> GetNoDataFilesByHomeworkIdAsync(int homeworkId);
    }

    public class HomeworkService : IHomeworkService
    {
        private readonly StudyDbContext _studyDbContext;

        public HomeworkService(StudyDbContext context)
        {
            _studyDbContext = context;
        }

        public Homework? GetHomework(int id, bool includeFiles = false)
        {
            Homework? homework;
            if (includeFiles)
                homework = _studyDbContext.Homeworks.Where(x => x.Id == id).Include(x => x.Files).FirstOrDefault();
            else
                homework = _studyDbContext.Homeworks.Where(x => x.Id == id).FirstOrDefault();
            return homework;
        }

        public async Task<Homework?> GetHomeworkAsync(int id, bool includeFiles = false)
        {
            Homework? homework;
            if (includeFiles)
                homework = await _studyDbContext.Homeworks.Where(x => x.Id == id).Include(x => x.Files).FirstOrDefaultAsync();
            else
                homework = await _studyDbContext.Homeworks.Where(x => x.Id == id).FirstOrDefaultAsync();
            return homework;
        }

        public Subject? GetSubject(int id)
        {
            var subject = (from subj in _studyDbContext.Subjects join homework in _studyDbContext.Homeworks on subj.Id equals homework.Subjectid where subj.Id == id select subj).FirstOrDefault();
            return subject;
        }

        public List<Subject> GetSubjects(bool withFiles = false)
        {
            List<Subject> subjects = new List<Subject>();
            if (withFiles)
                subjects = _studyDbContext.Subjects.Include(x => x.Homeworks).ThenInclude(x => x.Files).ToList();
            else
                subjects = _studyDbContext.Subjects.Include(x => x.Homeworks).ToList();
            return subjects;
        }

        public async Task<List<Subject>> GetSubjectsAsync(bool withFiles = false)
        {
            List<Subject> subjects = new List<Subject>();
            if (withFiles)
                subjects = await _studyDbContext.Subjects.Include(x => x.Homeworks).ThenInclude(x => x.Files).ToListAsync();
            else
                subjects = await _studyDbContext.Subjects.Include(x => x.Homeworks).ToListAsync();
            return subjects;
        }

        public async Task DeleteHomeworkAsync(int id)
        {
            await _studyDbContext.Homeworks.Where(x => x.Id == id).Include(x => x.Files).ExecuteDeleteAsync();
        }

        public async Task UpdateHomeworkAsync(HomeworkForUpdate newHomework)
        {
            var dbHomework = await _studyDbContext.Homeworks.Where(x => x.Id == newHomework.Id).Include(x => x.Files).FirstOrDefaultAsync();
            if (dbHomework is not null)
            {
                dbHomework.Finished = newHomework.Finished;
                dbHomework.Deadline = newHomework.Deadline;
                dbHomework.Description = newHomework.Description;
                if (newHomework.Files is not null)
                {
                    if (dbHomework.Files is null)
                    {
                        dbHomework.Files = new List<File>();
                    }
                    foreach (var file in newHomework.Files)
                    {
                        dbHomework.Files.Add(file);
                    }
                }
                await _studyDbContext.SaveChangesAsync();
            }
        }

        public async Task InsertHomeworkAsync(Homework newHomework)
        {
            await _studyDbContext.AddAsync(newHomework);
            await _studyDbContext.SaveChangesAsync();
        }

        public async Task<File?> GetHomeworkFileAsync(int id)
        {
            return await _studyDbContext.Files.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<NoDataFile>> GetNoDataFilesByHomeworkIdAsync(int homeworkId)
        {
            return await _studyDbContext.Files.Where(x => x.HomeworkId == homeworkId).Select(x => new NoDataFile()
            {
                Name = x.Name,
                Id = x.Id
            }).ToListAsync();
        }
    }

    public record NoDataFile
    {
        public int Id { get; set; }
        public string Name { get; set; } = null;
    }

    public class HomeworkForUpdate
    {
        public int Id { get; set; }
        public bool Finished { get; set; }
        public string? Description { get; set; }
        public DateTime Deadline { get; set; }
        public List<File>? Files { get; set; }
    }
}
