using System;
using System.Collections.Generic;
using System.Text;

namespace GitStat.ImportConsole.ParserUtils
{
    public class FooterDto
    {
        public int FilesChanges { get; set; }
        public int Insertions { get; set; }
        public int Deletions { get; set; }
    }
}
