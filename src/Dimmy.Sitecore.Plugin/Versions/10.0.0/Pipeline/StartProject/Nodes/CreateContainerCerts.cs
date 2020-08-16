using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Dimmy.Engine.Models.Yaml;
using Dimmy.Engine.Pipelines;
using Dimmy.Engine.Pipelines.StartProject;
using Dimmy.Engine.Services;
using Dimmy.Sitecore.Plugin.Versions._10._0._0.Models.Traefik;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Dimmy.Sitecore.Plugin.Versions._10._0._0.Pipeline.StartProject.Nodes
{
    public class CreateContainerCerts : Node<IStartProjectContext>
    {
        private readonly ICertificateService _certificateService;

        public override void DoExecute(IStartProjectContext input)
        {
            var traefikCertsPath = Path.Combine(input.WorkingPath, "traefik", "certs");
            if (!Directory.Exists(traefikCertsPath))
                Directory.CreateDirectory(traefikCertsPath);
          
            var traefikConfig = new Config();
            traefikConfig.Tls.Certificates.Add(
                CreateCert(input.Project, Constants.CdHostName, traefikCertsPath)
                );
            
            traefikConfig.Tls.Certificates.Add(
                    CreateCert(input.Project, Constants.CmHostName, traefikCertsPath)
            );
            
            traefikConfig.Tls.Certificates.Add(
                    CreateCert(input.Project, Constants.IdHostName, traefikCertsPath)
            );
            
            var traefikConfigPath = Path.Combine(input.WorkingPath, "traefik", "config", "dynamic");
            if (!Directory.Exists(traefikConfigPath))
                Directory.CreateDirectory(traefikConfigPath);
            
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            
            var traefikConfigYaml = serializer.Serialize(traefikConfig);

            File.WriteAllText(
                Path.Combine(traefikConfigPath, "certs_config.yaml"),
                traefikConfigYaml);
        }

        public CreateContainerCerts(ICertificateService certificateService)
        {
            _certificateService = certificateService;
        }

        private Certificate CreateCert(ProjectYaml project, string sitecoreIdHostname,
            string traefikCertsPath)
        {
            var hostName = project.VariableDictionary[sitecoreIdHostname];
            
            var certsPath = Path.Combine(traefikCertsPath, $"{hostName}");

            var certKeyPath = $"{certsPath}.key";
            var certPath = $"{certsPath}.crt";
            
            var certificate = new Certificate
            {
                CertFile = $@"C:\etc\traefik\certs\{hostName}.crt",
                KeyFile = $@"C:\etc\traefik\certs\{hostName}.key"
            };
            
            if(File.Exists(certificate.CertFile))
             return certificate;

            var baseDirectory = AppContext.BaseDirectory;
            var dimmyCert = Path.Combine(baseDirectory, "dimmy.pfx");
            var x509 = new X509Certificate2(dimmyCert);
            
            var cert = _certificateService.CreateSignedCertificate(hostName, hostName, x509);

            File.WriteAllText(certKeyPath, _certificateService.CreateKeyString(cert));
            File.WriteAllText(certPath, _certificateService.CreateCertificateString(cert));

            return certificate;
        }
    }
}