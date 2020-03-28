using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GitStat.Core.Entities;
using GitStat.ImportConsole.ParserUtils;
using Utils;

namespace GitStat.ImportConsole
{
    public class ImportController
    {
        const string Filename = "commits.txt";

        /// <summary>
        /// Liefert die Commits mit dem dazugehörigen Developer
        /// </summary>
        public static Commit[] ReadFromCsv()
        {

            string filePath = MyFile.GetFullNameInApplicationTree(Filename);
            List<Commit> commits = new List<Commit>();
            Dictionary<string, Developer> developers = new Dictionary<string, Developer>();

            if (File.Exists(filePath) == false)
            {
                throw new Exception("File does not exist");
            }

            string[] lines = File.ReadAllLines(filePath);
            HeaderDto headerDto = null;
            FooterDto footerDto = null;

            foreach (string line in lines)
            {
                string[] data = line.Split(',');

                headerDto = MyParser.TryParseCommitHeader(line);
                if (headerDto != null)
                {
                    Developer developer;
                    if (developers.TryGetValue(headerDto.DeveloperName, out developer) == false)
                    {
                        developer = new Developer
                        {
                            Name = headerDto.DeveloperName
                        };
                        developers[headerDto.DeveloperName] = developer;
                    }

                    commits.Add(new Commit
                    {
                        HashCode = headerDto.HashCode,
                        Developer = developer,
                        Date = headerDto.CommitDate,
                        Message = headerDto.Message,
                        FilesChanges = 0,
                        Insertions = 0,
                        Deletions = 0
                    });
                }

                footerDto = MyParser.TryParseCommitFooter(line);
                if (footerDto != null && commits.Count > 0)
                {
                    Commit lastCommit = commits.Last();
                    lastCommit.FilesChanges = footerDto.FilesChanges;
                    lastCommit.Insertions = footerDto.Insertions;
                    lastCommit.Deletions = footerDto.Deletions;
                }
            }
            return commits.ToArray();
        }
    }
}
