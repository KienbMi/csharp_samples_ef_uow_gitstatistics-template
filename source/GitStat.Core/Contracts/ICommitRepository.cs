using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GitStat.Core.Entities;

namespace GitStat.Core.Contracts
{
    public interface ICommitRepository
    {
        void AddRange(Commit[] commits);
        (string Name, DateTime Date, int FileChanges, int Insertions, int Deletions) GetCommitWithId(int id);
        (string Name, DateTime Date, int FilesChanges, int Insertions, int Deletions)[] GetCommitYoungerEqualThen(DateTime fromDate);
    }
}
