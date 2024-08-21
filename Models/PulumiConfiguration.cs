using Pulumi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulumiGithub.Models
{
    public class PulumiConfiguration
    {
       
        public Output<string> githubToken { get; set; }
        public string YourUsername { get; set; }
        public string RepoName { get; set; }
        public string RepoDescription { get; set; }
        public string Visibility { get; set; }
        public Dictionary<string, RepoConfig> Repos { get; set; }
        public PulumiConfiguration(Config config)
        {
            githubToken = config.RequireSecret("githubToken");
            RepoDescription = config.Require("repoDescription");
            RepoName = config.Require("repoName");
            Visibility = config.Require("visibility");
            YourUsername = config.Require("yourUsername");


            // Extract nested repository configurations
            Repos = config.RequireObject<Dictionary<string, RepoConfig>>("repos");
        }
    }
}
