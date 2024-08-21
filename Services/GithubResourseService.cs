using Octokit;
using Pulumi;
using Pulumi.Github;
using Pulumi.Github.Inputs;
using PulumiGithub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulumiGithub.Services
{
    public class GithubResourseService
    {
        private PulumiConfiguration _configuration { get; set; }
        private Pulumi.Github.Provider githubProvider { get; set; }
        public GithubResourseService(PulumiConfiguration configuration)
        {
            _configuration= configuration;
            githubProvider = CreateGithubProvider(configuration.githubToken);
        }
        private Pulumi.Github.Provider CreateGithubProvider(Output<string> githubToken)
        {
            return new Pulumi.Github.Provider("github-provider", new ProviderArgs
            {
                Token = githubToken
            });
        }
        public Pulumi.Github.Repository CreateGithubRepository(RepoConfig repoConfig)
        {
            return new Pulumi.Github.Repository($"{_configuration.RepoName}-{repoConfig.suffix}", new RepositoryArgs
            {
                Name = $"{_configuration.RepoName}-{repoConfig.suffix}",
                Description = $"{_configuration.RepoDescription} - {repoConfig.suffix}",
                Visibility = _configuration.Visibility,
                AutoInit = true,
                Template = new RepositoryTemplateArgs
                {
                    Owner = repoConfig.templateOwnername,
                    Repository = repoConfig.templateName,
                    IncludeAllBranches = true
                }
            }, new CustomResourceOptions { Provider = githubProvider });
        }


    }
}
