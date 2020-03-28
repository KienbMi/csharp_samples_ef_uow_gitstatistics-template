using System;
using System.Collections.Generic;
using System.Linq;
using GitStat.Core.Contracts;
using GitStat.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GitStat.Persistence
{
    public class CommitRepository : ICommitRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CommitRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddRange(Commit[] commits)
        {
            _dbContext.Commits.AddRange(commits);
        }

        public (string Name, DateTime Date, int FilesChanges, int Insertions, int Deletions)[] GetCommitYoungerEqualThen(DateTime fromDate)
        {
            return _dbContext.Commits
               .Where(c => c.Date >= fromDate)
               .OrderBy(c => c.Date)
               .Include(c => c.Developer)
               .AsEnumerable()
               .Select(c => (c.Developer.Name, c.Date, c.FilesChanges, c.Insertions, c.Deletions))
               .ToArray();
        }

        public (string Name, DateTime Date, int FileChanges, int Insertions, int Deletions) GetCommitWithId(int id)
        {
            return _dbContext.Commits
                .Where(c => c.Id == id)
                .Include(c => c.Developer)
                .AsEnumerable()
                .Select(c => (c.Developer.Name, c.Date, c.FilesChanges, c.Insertions, c.Deletions))
                .FirstOrDefault();
        }
    }
}