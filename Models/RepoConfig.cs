using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulumiGithub.Models
{
    public class RepoConfig
    {
        public string suffix { get; set; }
        public string templateOwnername { get; set; }
        public string templateName { get; set; }
        public bool isCreate { get; set; }
        public string currentName { get; set; }
        public string newName { get; set; }
    }
}
