using System.CommandLine;
using Dimmy.Cli.Commands.Project;
using Dimmy.Cli.Commands.Project.SubCommands;
using Dimmy.Engine.Services;
using Dimmy.Engine.Services.Projects;

namespace Dimmy.Sitecore.Plugin.Versions._10._0._0
{
    public class StartDecorator : ProjectSubCommand<StartArgument>
    {
        private readonly IProjectSubCommand _decorated;
        private readonly ICertificateService _certificateService;
        private readonly IProjectService _projectService;

        public StartDecorator(
            IProjectSubCommand decorated,
            ICertificateService certificateService,
            IProjectService projectService)
        {
            _decorated = decorated;
            _certificateService = certificateService;
            _projectService = projectService;
        }
        
        public override Command GetCommand()
        {
            return _decorated.GetCommand();
        }

        protected override void CommandAction(StartArgument arg)
        {
            var project = _projectService.GetProject(arg.WorkingPath);

            if (project.Project.MetaData[Constants.MetaData.SitecoreVersion] == "10.0.0")
            {
                var cdHostName = project.Project.VariableDictionary[Constants.CdHostName];
                var cdCert = _certificateService
                    .CreateCertificate(cdHostName, cdHostName);
                
                var cmHostName = project.Project.VariableDictionary[Constants.CmHostName];
                var cmCert = _certificateService
                    .CreateCertificate(cmHostName, cmHostName);
                
                var idHostName = project.Project.VariableDictionary[Constants.IdHostName];
                var idCert = _certificateService
                    .CreateCertificate(idHostName, idHostName);
            }
            
            _decorated.CommandAction(arg);
        }
    }
}