using System.IO;
using System.Security.Cryptography.X509Certificates;
using Dimmy.Engine.Models.Yaml;
using Dimmy.Engine.Pipelines;
using Dimmy.Engine.Pipelines.StartProject;
using Dimmy.Engine.Services;

namespace Dimmy.Sitecore.Plugin.Versions._10._0._0.Pipeline.StartProject.Nodes
{
    public class CreateContainerCerts : Node<StartProjectContext>
    {
        private readonly ICertificateService _certificateService;

        public override void DoExecute(StartProjectContext input)
        {
            var traefikCertsPath = Path.Combine(input.ProjectInstance.WorkingPath, "traefik", "certs");
            if (!Directory.Exists(traefikCertsPath))
                Directory.CreateDirectory(traefikCertsPath);
            
            CreateCert(input.Project, Constants.CdHostName, traefikCertsPath);
            CreateCert(input.Project, Constants.CmHostName, traefikCertsPath);
            CreateCert(input.Project, Constants.IdHostName, traefikCertsPath);
        }

        public CreateContainerCerts(ICertificateService certificateService)
        {
            _certificateService = certificateService;
        }

        private void CreateCert(ProjectYaml project, string sitecoreIdHostname,
            string traefikCertsPath)
        {
            var hostName = project.VariableDictionary[sitecoreIdHostname];
            
            var certPath = Path.Combine(traefikCertsPath, $"{hostName}.crt");
            if(File.Exists(certPath))
                return;
            
            var idCert = _certificateService
                .CreateCertificate(hostName, hostName);

            File.WriteAllBytes(certPath, idCert.Export(X509ContentType.Cert));
        }
    }
}