using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Dimmy.Engine.Pipelines;
using Dimmy.Engine.Pipelines.StartProject;
using Dimmy.Engine.Services;
using Dimmy.Sitecore.Plugin.Versions._10._0._0;
using Dimmy.Sitecore.Plugin.Versions._10._0._0.Models.Traefik;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Dimmy.Sitecore.Plugin.Pipeline.StartProject.Nodes
{
    public class CreateContainerCerts : SitecoreStartNode
    {
        private readonly ICertificateService _certificateService;

        public override async Task DoExecute(IStartProjectContext input)
        {
            var traefikCertsPath = Path.Combine(input.WorkingPath, "traefik", "certs");
            if (!Directory.Exists(traefikCertsPath))
                Directory.CreateDirectory(traefikCertsPath);
            
            var traefikConfig = new Config();
            
            foreach (var service in input.DockerComposeFileConfig.ServiceDefinitions)
            {
                if (!service.Labels.ContainsKey("traefik.enable")) continue;
                var host = service
                    .Labels
                    .Single(l => l.Key.EndsWith("rule"))
                    .Value;

                var hostName = GetHostNameFromEnvironmentalVariables(host, input);

                var certificate = CreateCert(hostName, traefikCertsPath);
                traefikConfig.Tls.Certificates.Add(certificate);
            }
            
            var traefikConfigPath = Path.Combine(input.WorkingPath, "traefik", "config", "dynamic");
            if (!Directory.Exists(traefikConfigPath))
                Directory.CreateDirectory(traefikConfigPath);
            
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            
            var traefikConfigYaml = serializer.Serialize(traefikConfig);

            await File.WriteAllTextAsync(
                Path.Combine(traefikConfigPath, "certs_config.yaml"),
                traefikConfigYaml);
        }

        public CreateContainerCerts(ICertificateService certificateService)
        {
            _certificateService = certificateService;
        }

        private Certificate CreateCert(string hostName, string traefikCertsPath)
        {
            
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