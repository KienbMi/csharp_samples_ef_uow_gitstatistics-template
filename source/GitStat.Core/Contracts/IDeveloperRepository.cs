using GitStat.Core.Entities;

namespace GitStat.Core.Contracts
{
    public interface IDeveloperRepository
    {
        (string Name, int Commits, int FileChanges, int Insertions, int Deletions)[] GetDeveloperStatistik();
    }
}
