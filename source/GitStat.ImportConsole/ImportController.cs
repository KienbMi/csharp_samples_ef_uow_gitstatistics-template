using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GitStat.Core.Entities;
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
            bool headerFound = false;
            int year = 0, month = 0, day = 0;
            string developerName = string.Empty;
            string hashCode = string.Empty;
            string message = string.Empty;

            foreach (string line in lines)
            {
                string[] data = line.Split(',');
                if (data.Length >= 4)
                {
                    string[] dateString = data[2].Split('-');
                    if (dateString.Length == 3
                        && int.TryParse(dateString[0], out year)
                        && int.TryParse(dateString[1], out month)
                        && int.TryParse(dateString[2], out day))
                    {
                        headerFound = true;

                        hashCode = data[0];
                        developerName = data[1];
                        for (int i = 3; i < data.Length; i++)
                        {
                            if (i == 3)
                            {
                                message = data[i];
                            }
                            else
                            {
                                message += $",{data[i]}";
                            }
                        }
                    }
                }

                if (line.Contains("file"))
                {
                    if (headerFound)
                    {
                        bool footerIsValid = true;
                        int changes = 0, insertions = 0, deletions = 0;
                        for (int i = 0; i < data.Length && footerIsValid; i++)
                        {
                            string[] footerPart = data[i].Split(' ');
                            if (footerPart.Length < 2)
                            {
                                footerIsValid = false;
                            }

                            if (footerIsValid)
                            {
                                switch (i)
                                {
                                    case 0:
                                        {
                                            footerIsValid = int.TryParse(footerPart[1], out changes);
                                            if (footerPart[2].Contains("file") == false)
                                            {
                                                footerIsValid = false;
                                            }
                                            break;
                                        }
                                    case 1:
                                    case 2:
                                        {
                                            if (footerPart[2].Contains("insertion"))
                                            {
                                                footerIsValid = int.TryParse(footerPart[1], out insertions);
                                            }
                                            else if (footerPart[2].Contains("deletions"))
                                            {
                                                footerIsValid = int.TryParse(footerPart[1], out deletions);
                                            }
                                            break;
                                        }
                                }
                            }
                        }
                        if (footerIsValid)
                        {
                            Developer developer;
                            if (developers.TryGetValue(developerName, out developer) == false)
                            {
                                developer = new Developer
                                {
                                    Name = developerName
                                };
                                developers[developerName] = developer;
                            }

                            commits.Add(new Commit
                            {
                                HashCode = hashCode,
                                Developer = developer,
                                Date = new DateTime(year, month, day),
                                Message = message,
                                FilesChanges = changes,
                                Insertions = insertions,
                                Deletions = deletions
                            });
                            headerFound = false;
                        }
                    }
                }
            }
            return commits.ToArray();
        }
    }
}
