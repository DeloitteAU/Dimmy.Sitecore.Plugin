using System.IO;
using System.Security.Cryptography.X509Certificates;
using Dimmy.Engine.Pipelines;
using Dimmy.Engine.Pipelines.StartProject;
using Dimmy.Engine.Services;
using Dimmy.Engine.Services.Projects;

namespace Dimmy.Sitecore.Plugin.Versions._10._0._0.Pipeline.StartProject.Nodes
{
    public class CreateContainerCerts : Node<StartProjectContext>
    {
        private readonly ICertificateService _certificateService;
        private readonly IProjectService _projectService;

        public CreateContainerCerts(
            ICertificateService certificateService,
            IProjectService projectService)
        {
            _certificateService = certificateService;
            _projectService = projectService;
        }

        public override void DoExecute(StartProjectContext input)
        {
            var traefikCertsPath = Path.Combine(input.WorkingPath, "traefik", "certs");
            if(!Directory.Exists(traefikCertsPath))
                return;
            
            var project = _projectService.GetProject(input.WorkingPath);

            if (project.Project.MetaData[Constants.MetaData.SitecoreVersion] != "10.0.0")
                return;
            
            var cdHostName = project.Project.VariableDictionary[Constants.CdHostName];
            var cdCert = _certificateService
                .CreateCertificate(cdHostName, cdHostName);
            File.WriteAllBytes(Path.Combine(traefikCertsPath, "xm1cm.localhost.crt"), cdCert.Export(X509ContentType.Cert));
            

            var cmHostName = project.Project.VariableDictionary[Constants.CmHostName];
            var cmCert = _certificateService
                .CreateCertificate(cmHostName, cmHostName);
            File.WriteAllBytes(Path.Combine(traefikCertsPath, "xm1cm.localhost.crt"), cmCert.Export(X509ContentType.Cert));

            var idHostName = project.Project.VariableDictionary[Constants.IdHostName];
            var idCert = _certificateService
                .CreateCertificate(idHostName, idHostName);
            File.WriteAllBytes(Path.Combine(traefikCertsPath, "xm1cm.localhost.crt"), idCert.Export(X509ContentType.Cert));

        }
    }
}