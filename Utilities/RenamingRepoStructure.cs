using Octokit;
using Pulumi;
using PulumiGithub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PulumiGithub.Utilities
{
    public class RenamingRepoStructure
    {
        private PulumiConfiguration _configuration { get; set; }
        public RenamingRepoStructure(PulumiConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task RenameRepositoryContentsAsync(string repositoryName, RepoConfig repoConfig)
        {
            await Task.Run(() =>
            {
                _configuration.githubToken.Apply(async token =>
                {
                    var client = new GitHubClient(new Octokit.ProductHeaderValue("PulumiGithub"))
                    {
                        Credentials = new Credentials(token)
                    };

                    await RenameContents(client, _configuration.YourUsername, repositoryName, ".", repoConfig.currentName, repoConfig.newName);
                });


            });



        }
        #region RenameContents
        private async Task RenameContents(GitHubClient client, string owner, string repo, string path, string currentName, string newName)
        {
            var renameTasks = new List<Task>();
            try
            {
                var repository = await client.Repository.Get(owner, repo);
                if (repository != null)
                {
                    var contents = await client.Repository.Content.GetAllContents(owner, repo, path);

                    foreach (var content in contents)
                    {
                        var newPath = ReplaceTextCaseInsensitive(content.Path, currentName, newName);
                        Console.WriteLine($"{content.Path} changed to {newPath} ({!content.Path.Equals(newPath, StringComparison.OrdinalIgnoreCase)})");

                        if (content.Type == Octokit.ContentType.Dir)
                        {
                            // Recursively process directories
                            await RenameContents(client, owner, repo, content.Path, currentName, newName);
                        }
                        else
                        {
                            // Rename files regardless of directory name
                            if (!content.Path.Equals(newPath, StringComparison.OrdinalIgnoreCase))
                            {
                                await RenameFile(client, owner, repo, content.Path, newPath, currentName, newName);
                            }
                            else
                            {
                                await UpdateFile(client, owner, repo, content.Path, currentName, newName);
                            }
                        }
                    }
                }

                await Console.Out.WriteLineAsync("All files and directories renamed successfully.");
            }
            catch (NotFoundException)
            {
                await Console.Out.WriteLineAsync($"{repo} Not Found Yet");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
        #endregion

        #region RenameFile
        private async Task RenameFile(GitHubClient client, string owner, string repo, string oldFilePath, string newFilePath, string currentName, string newName)
        {
            var fileContents = await client.Repository.Content.GetAllContents(owner, repo, oldFilePath);
            var file = fileContents.FirstOrDefault();

            if (file == null) return;

            // Case-insensitive replacement for content
            var fileContent = ReplaceTextCaseInsensitive(file.Content, currentName, newName);
            var fileSha = file.Sha;

            // Create the new file with the updated content
            var createRequest = new CreateFileRequest($"Renaming {oldFilePath} to {newFilePath}", fileContent, "main");
            await client.Repository.Content.CreateFile(owner, repo, newFilePath, createRequest);

            // Delete the old file
            var deleteRequest = new DeleteFileRequest($"Deleting original file {oldFilePath}", fileSha);
            await client.Repository.Content.DeleteFile(owner, repo, oldFilePath, deleteRequest);

            Console.WriteLine($"File '{oldFilePath}' successfully renamed to '{newFilePath}' and content updated.");
        }
        #endregion

        #region UpdateFile
        private async Task UpdateFile(GitHubClient client, string owner, string repo, string filePath, string currentName, string newName)
        {
            var fileContents = await client.Repository.Content.GetAllContents(owner, repo, filePath);
            var file = fileContents.FirstOrDefault();

            if (file == null) return;

            // Case-insensitive replacement
            var fileContent = ReplaceTextCaseInsensitive(file.Content, currentName, newName);
            var fileSha = file.Sha;

            var updateRequest = new UpdateFileRequest($"Updating file content from {currentName} to {newName}", fileContent, fileSha);
            await client.Repository.Content.UpdateFile(owner, repo, filePath, updateRequest);

            Console.WriteLine($"File '{filePath}' successfully updated content from '{currentName}' to '{newName}'.");
        }
        #endregion

        private string ReplaceTextCaseInsensitive(string input, string search, string replacement)
        {
            var regex = new Regex(Regex.Escape(search), RegexOptions.IgnoreCase);
            return regex.Replace(input, replacement);
        }
    }
}
