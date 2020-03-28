using System;
using System.IO;
using System.Linq;
using System.Text;
using GitStat.Core.Contracts;
using GitStat.Persistence;

namespace GitStat.ImportConsole
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Import der Commits in die Datenbank");
            using (IUnitOfWork unitOfWorkImport = new UnitOfWork())
            {
                Console.WriteLine("Datenbank löschen");
                unitOfWorkImport.DeleteDatabase();
                Console.WriteLine("Datenbank migrieren");
                unitOfWorkImport.MigrateDatabase();
                Console.WriteLine("Commits werden von commits.txt eingelesen");
                var commits = ImportController.ReadFromCsv();
                if (commits.Length == 0)
                {
                    Console.WriteLine("!!! Es wurden keine Commits eingelesen");
                    return;
                }
                Console.WriteLine(
                    $"  Es wurden {commits.Count()} Commits eingelesen, werden in Datenbank gespeichert ...");
                unitOfWorkImport.CommitRepository.AddRange(commits);
                int countDevelopers = commits.GroupBy(c => c.Developer).Count();
                int savedRows = unitOfWorkImport.SaveChanges();
                Console.WriteLine(
                    $"{countDevelopers} Developers und {savedRows - countDevelopers} Commits wurden in Datenbank gespeichert!");
                Console.WriteLine();
                var csvCommits = commits.Select(c =>
                    $"{c.Developer.Name};{c.Date};{c.Message};{c.HashCode};{c.FilesChanges};{c.Insertions};{c.Deletions}");
                File.WriteAllLines("commits.csv", csvCommits, Encoding.UTF8);


                var developerStatitic = commits
                    .GroupBy(c => c.Developer)
                    .Select(d => new
                    {
                        Name = d.Key.Name,
                        Commits = d.Count(),
                        FileChanges = d.Sum(d => d.FilesChanges),
                        Insertions = d.Sum(d => d.Insertions),
                        Deletions = d.Sum(d => d.Deletions)
                    })
                    .OrderByDescending(_ => _.Commits);

                Console.WriteLine("Statistik der Commits der Developer");
                Console.WriteLine("-----------------------------------");
                Console.WriteLine($"{"Developer",-20}{"Commits",-20}{"FileChanges",-20}{"Insertions",-20}{"Deletions"}");
                foreach (var developer in developerStatitic)
                {
                    Console.WriteLine($"{developer.Name, -20}{developer.Commits, -20}{developer.FileChanges, -20}{developer.Insertions, -20}{developer.Deletions, -20}");
                }
                
            }
            Console.WriteLine("Datenbankabfragen");
            Console.WriteLine("=================");
            using (IUnitOfWork unitOfWork = new UnitOfWork())
            {
                Console.WriteLine("Commits der letzten 4 Wochen");
                Console.WriteLine("----------------------------");
                Console.WriteLine($"{"Developer",-20}{"Date",-20}{"Commits",-20}{"FileChanges",-20}{"Insertions",-20}{"Deletions"}");
                var commits = unitOfWork.CommitRepository.GetCommitYoungerEqualThen(DateTime.Parse("2019, 03, 01"));
                foreach (var commit in commits)
                {
                    Console.WriteLine($"{commit.Name,-20}{commit.Date.ToShortDateString(),-20}{commit.FilesChanges,-20}{commit.Insertions,-20}{commit.Deletions,-20}");
                }
                Console.WriteLine();

                Console.WriteLine("Commmit mit Id 4");
                Console.WriteLine("----------------");
                var commitWithId4 = unitOfWork.CommitRepository.GetCommitWithId(4);
                Console.WriteLine($"{commitWithId4.Name,-20}{commitWithId4.Date,-20}{commitWithId4.FileChanges,-20}{commitWithId4.Insertions,-20}{commitWithId4.Deletions,-20}");
                Console.WriteLine();

                Console.WriteLine("Statistik der Commits der Developer");
                Console.WriteLine("-----------------------------------");
                Console.WriteLine($"{"Developer",-20}{"Commits",-20}{"FileChanges",-20}{"Insertions",-20}{"Deletions"}");

                var developers = unitOfWork.DeveloperRepository.GetDeveloperStatistik();
                foreach (var developer in developers)
                {
                    Console.WriteLine($"{developer.Name,-20}{developer.Commits,-20}{developer.FileChanges,-20}{developer.Insertions,-20}{developer.Deletions,-20}");
                }
            }
            Console.Write("Beenden mit Eingabetaste ...");
            Console.ReadLine();
        }

    }
}
