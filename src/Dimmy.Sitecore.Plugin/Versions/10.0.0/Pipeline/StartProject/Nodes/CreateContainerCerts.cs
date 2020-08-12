using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
                .WithNamingConvention(LowerCaseNamingConvention.Instance)
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
            

            var cert = _certificateService
                .CreateCertificate(hostName, hostName);


            var privateKeyBytes =cert.GetECDsaPrivateKey().ExportECPrivateKey();
                
            
            var builder = new StringBuilder("-----BEGIN RSA PRIVATE KEY");
            builder.AppendLine("-----");

            var base64PrivateKeyString = Convert.ToBase64String(privateKeyBytes);
            var offset = 0;
            const int LINE_LENGTH = 64;

            while (offset < base64PrivateKeyString.Length)
            {
                var lineEnd = Math.Min(offset + LINE_LENGTH, base64PrivateKeyString.Length);
                builder.AppendLine(base64PrivateKeyString.Substring(offset, lineEnd - offset));
                offset = lineEnd;
            }

            builder.Append("-----END RSA PRIVATE KEY");
            builder.AppendLine("-----");

            
            File.WriteAllText(certKeyPath, builder.ToString());
            File.WriteAllBytes(certPath, cert.Export(X509ContentType.Cert));

            return certificate;
        }
    }
}