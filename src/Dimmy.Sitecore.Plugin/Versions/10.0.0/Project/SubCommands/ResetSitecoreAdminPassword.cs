using System;
using System.CommandLine;
using System.Linq;
using Dimmy.Cli.Extensions;
using Dimmy.Engine.Services.Projects;
using Ductus.FluentDocker.Commands;
using Ductus.FluentDocker.Services;

namespace Dimmy.Sitecore.Plugin.Versions._10._0._0.Project.SubCommands
{
    public class ResetSitecoreAdminPassword : SitecoreSubCommand<ResetSitecoreAdminPasswordArgument>
    {
        private readonly IProjectService _projectService;
        private readonly IHostService _docker;

        public ResetSitecoreAdminPassword(
            IProjectService projectService,
            IHostService docker)
        {
            _projectService = projectService;
            _docker = docker;
        }
        
        public override void CommandAction(ResetSitecoreAdminPasswordArgument arg)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("There is a bug in the mssql container where you can only reset the password once per session (start and then stop).");
            Console.ResetColor();
            
            var runningProject = _projectService.ResolveRunningProject(arg);
            
            var mssql = runningProject.Services.Single(s => s.Name == "mssql");
            
            var execArgs = @$"powershell C:\SetSitecoreAdminPassword.ps1 -ResourcesDirectory '{arg.ResourcesDirectory}' -SitecoreAdminPassword '{arg.Password}' -SqlServer '127.0.0.1'";
            var responce = _docker.Host.Execute(mssql.ContainerId, execArgs);

            Console.WriteLine(string.Join('\n', responce.Log));
        }

        public override Command BuildCommand()
        {
            return new Command("AdminPasswordReset")
            {
                new Option<Guid>("--project-id",
                    "The Id of the Project you wish to attach to. Omit for context project"),
                new Option<string>("--working-path", "Working Path"),
                new Option<string>("--password", "The Password you want to reset the admin password to."),
            };
        }
    }
}