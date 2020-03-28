using System;
using System.Collections.Generic;
using System.Text;

namespace GitStat.ImportConsole.ParserUtils
{
    public class HeaderDto
    {
        public string HashCode { get; set; }
        public string DeveloperName { get; set; }
        public string Message { get; set; }
        public DateTime CommitDate { get; set; }

    }
}
