using GitStat.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GitStat.Persistence
{
    public class DeveloperRepository : IDeveloperRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public DeveloperRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public (string Name, int Commits, int FileChanges, int Insertions, int Deletions)[] GetDeveloperStatistik()
        {
            return _dbContext.Developers
                .Select(d => new
                {
                    Name = d.Name,
                    Commits = d.Commits.Count(),
                    FileChanges = d.Commits.Sum(c => c.FilesChanges),
                    Insertions = d.Commits.Sum(c => c.Insertions),
                    Deletions = d.Commits.Sum(c => c.Deletions)
                })
                .OrderByDescending(d => d.Commits)
                .AsEnumerable()
                .Select(developer => (developer.Name, developer.Commits, developer.FileChanges, developer.Insertions, developer.Deletions))
                .ToArray();
        }
    }
}