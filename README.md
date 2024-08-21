Install Pulumi: 
	https://www.pulumi.com/docs/get-started/install/

 Install .NET 8 SDK:
  https://dotnet.microsoft.com/en-us/download/dotnet/8.0

Open Project Directory on cmd and run following commands:
1. pulumi login
2. pulumi stack init dev (if not created already)
3. pulumi stack select dev
4. pulumi config set PulumiGithub:githubToken (your-git-hub-token-with-full-rights) --secret
5. set repoName,Description and visibility,templateOwnername,yourUsername,templateName,currentName,newName in pulumi.dev.yml file
6. pulumi preview
7. pulumi up (to create reposs (UI,Infra,API) and branches(main,dev) with readme.md and .gitignore files)
8. pulumi down(to revert all changes)
