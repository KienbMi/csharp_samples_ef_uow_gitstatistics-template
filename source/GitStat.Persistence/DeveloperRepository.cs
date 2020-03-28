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
            return _dbContext.Developers.Join(_dbContext.Commits,
                        developer => developer.Id,
                        commit => commit.Developer.Id,
                        (developer, commit) =>
                        new
                        {
                            developer.Id,
                            developer.Name,
                            commit.FilesChanges,
                            commit.Insertions,
                            commit.Deletions

                        })
                        .GroupBy(developer => developer.Name)
                        .Select(d => new
                        {
                            Name = d.Key,
                            Commits = d.Count(),
                            FileChanges = d.Sum(c => c.FilesChanges),
                            Insertions = d.Sum(c => c.Insertions),
                            Deletions = d.Sum(c => c.Deletions)
                        })
                        .OrderByDescending(d => d.Commits)
                        .AsEnumerable()
                        .Select(developer => (developer.Name, developer.Commits, developer.FileChanges, developer.Insertions, developer.Deletions))
                        .ToArray();




            //return _dbContext.Commits
            //           //.Include(d => d.Commits)
            //           //.ToArray()
            //           .GroupBy(c => c.DeveloperId)
            //           .Select(d => new {
            //                   Name = "Hansi",
            //                   Commits = d.Count(),
            //                   FileChanges = d.Sum(c => c.FilesChanges),
            //                   Insertions = d.Sum(c => c.Insertions),
            //                   Deletions = d.Sum(c => c.Insertions)
            //           })
            //           .ToArray()
            //           .OrderByDescending(d => d.Commits)
            //           .AsEnumerable()
            //           .Select(developer => (developer.Name, developer.Commits, developer.FileChanges, developer.Insertions, developer.Deletions))
            //           .ToArray();
        }
    }
}