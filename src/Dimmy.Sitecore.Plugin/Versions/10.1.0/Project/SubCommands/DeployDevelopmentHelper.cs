using System.CommandLine;
using System.IO;
using System.Linq;
using System.Reflection;
using Dimmy.Cli.Extensions;
using Dimmy.Engine.Pipelines;
using Dimmy.Engine.Pipelines.CopyFileToContainer;
using Dimmy.Engine.Services.Projects;

namespace Dimmy.Sitecore.Plugin.Versions._10._1._0.Project.SubCommands
{
    public class DeployDevelopmentHelper:SitecoreProjectCommand<DeployDevelopmentHelperArgument>
    {
        private readonly IProjectService _projectService;
        private readonly Pipeline<Node<ICopyFileToContainerContext>, ICopyFileToContainerContext> _copyFileToContainerPipeline;

        public string TemplatePath
        {
            get
            {
                var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return Path.Join(assemblyPath, $"Versions/10.1.0");
            }
        }
        
        public DeployDevelopmentHelper(
            IProjectService projectService,
            Pipeline<Node<ICopyFileToContainerContext>, ICopyFileToContainerContext> copyFileToContainerPipeline)
        {
            _projectService = projectService;
            _copyFileToContainerPipeline = copyFileToContainerPipeline;
        }
        
        public override Command BuildCommand()
        {
            var buildCommand = new Command("DeployDevelopmentHelper")
            {
                new Option<string>("--project-id",
                    "The Id of the Project you wish to attach to. Omit for context project"),
                new Option<string>("--working-path", "Working Path"),
            };
            return buildCommand;
        }

        public override void CommandAction(DeployDevelopmentHelperArgument arg)
        {
            var runningProject = _projectService.ResolveRunningProject(arg);
            
            var cd = runningProject.Services.Single(r => r.Name == "cd");
            var cm = runningProject.Services.Single(r => r.Name == "cm");
            
            _copyFileToContainerPipeline.Execute(new CopyFileToContainerContext
            {
                WorkingPath = runningProject.WorkingPath,
                TargetFilePath = $"{TemplatePath}\\Dimmy.DevelopmentHelper._10._1._0.dll",
                DestinationFilePath = "C:\\inetpub\\wwwroot\\bin",
                ContainerId = cd.ContainerId
            });
            
            
            
        }
    }
}