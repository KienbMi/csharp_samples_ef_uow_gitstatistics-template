using GitStat.ImportConsole.ParserUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GitStat.ImportConsole.ParserUtils
{
    internal class MyParser
    {
        public static HeaderDto TryParseCommitHeader(string line)
        {
            if (line == null)
                return null;

            string[] data = line.Split(',');
            HeaderDto headerDto = null;

            if (data.Length >= 4)
            {
                int year, month, day;

                string[] dateString = data[2].Split('-');
                if (dateString.Length == 3
                    && int.TryParse(dateString[0], out year)
                    && int.TryParse(dateString[1], out month)
                    && int.TryParse(dateString[2], out day))
                {
                    string hashCode = data[0];
                    string developerName = data[1];
                    string message = message = data[3];
                    for (int i = 4; i < data.Length; i++)
                    {
                        message += $",{data[i]}";
                    }

                    headerDto = new HeaderDto
                    {
                        HashCode = hashCode,
                        DeveloperName = developerName,
                        CommitDate = new DateTime(year, month, day),
                        Message = message
                    };
                }
            }
            return headerDto;
        }
        public static FooterDto TryParseCommitFooter(string line)
        {
            if (line == null)
                return null;

            if (line.Contains("changed") == false)
                return null;

            if ((line.Contains("insertion") || line.Contains("deletion")) == false)
                return null;

            string[] data = line.Split(',');
            FooterDto footerDto = null;

            bool isValid = true;
            int changes = 0, insertions = 0, deletions = 0;

            for (int i = 0; i < data.Length && isValid; i++)
            {
                string[] footerPart = data[i].Split(' ');
                if (footerPart.Length < 2)
                {
                    isValid = false;
                }

                if (isValid)
                {
                    switch (i)
                    {
                        case 0:
                            {
                                isValid = int.TryParse(footerPart[1], out changes);
                                break;
                            }
                        case 1:
                        case 2:
                            {
                                if (footerPart[2].Contains("insertion"))
                                {
                                    isValid = int.TryParse(footerPart[1], out insertions);
                                }
                                else if (footerPart[2].Contains("deletion"))
                                {
                                    isValid = int.TryParse(footerPart[1], out deletions);
                                }
                                break;
                            }
                    }
                }
            }

            if (isValid)
            {
                footerDto = new FooterDto
                {
                    FilesChanges = changes,
                    Insertions = insertions,
                    Deletions = deletions
                };
            }
            return footerDto;
        }
    }
}
