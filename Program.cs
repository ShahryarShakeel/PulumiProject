using System;
using System.Threading.Tasks;
using PulumiGithub.Models;
using PulumiGithub.Services;
using PulumiGithub.Utilities;

namespace PulumiGithub
{
    class Program
    {

        static Task<int> Main() {

            return Pulumi.Deployment.RunAsync<MyStack>();
       }
    }
    internal class MyStack : Pulumi.Stack
    {
        public GithubResourseService _githubreSourceService { get; set; }
        public RenamingRepoStructure _renamingRepoUtility { get; set; }
        public PulumiConfiguration configuration { get; set; }
        public MyStack()
        {
            var config = new Pulumi.Config();
            configuration=new PulumiConfiguration(config);
            _githubreSourceService = new GithubResourseService(configuration);
            _renamingRepoUtility = new RenamingRepoStructure(configuration);
            foreach (var repo in configuration.Repos)
            {
                
                var isCreate = repo.Value.isCreate;
                if (isCreate)
                {
                    var repository = _githubreSourceService.CreateGithubRepository(repo.Value);
                    repository.Name.Apply(async name =>
                    {
                        await Console.Out.WriteLineAsync(name);
                        // Ensure repository is created before renaming
                        await _renamingRepoUtility.RenameRepositoryContentsAsync(name, repo.Value);
                    });
                }
               
            }

            
               

            
        }


    }


}

