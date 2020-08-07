using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Dimmy.Engine.Commands;
using Dimmy.Engine.Commands.Project;
using Dimmy.Engine.Services;

namespace Dimmy.Sitecore.Plugin.Versions._10._0._0
{
    public class SitecoreInitialise : SitecoreInitialiseBase<SitecoreInitialiseArgument>
    {
        public override string Name => "sitecore-10.0.0";
        public override string Description => "Initialise a Sitecore 10.0.0 project.";

        protected override string Version => "10.0.0";

        public SitecoreInitialise(ICommandHandler<InitialiseProject> initialiseProjectCommandHandler) : base(
            initialiseProjectCommandHandler)
        {
        }

        protected override void DoHydrateCommand(Command command, SitecoreInitialiseArgument arg)
        {
            command.AddOption(new Option<string>("--cd-host-name", $"the host name fo the CD server. Defaults to {arg.CdHostName}"));
            command.AddOption(new Option<string>("--cm-host-name", $"the host name fo the CM server. Defaults to {arg.CmHostName}"));
            command.AddOption(new Option<string>("--id-host-name", $"the host name fo the CD server. Defaults to {arg.IdHostName}"));
            
            command.AddOption(new Option<string>("--traefik-isolation", $"the docker isolation for traefik. Defaults to {arg.TraefikIsolation}"));
            command.AddOption(new Option<string>("--traefik-image", $"the docker isolation for traefik. Defaults to {arg.TraefikIImage}"));
            
            command.AddOption(new Option<string>("--redis-isolation", $"the docker isolation for redis. Defaults to {arg.RedisIsolation}"));
            command.AddOption(new Option<string>("--mssql-isolation", $"the docker isolation for MsSql. Defaults to {arg.MssqlIsolation}"));
            command.AddOption(new Option<string>("--solr-isolation", $"the docker isolation for Solr. Defaults to {arg.SolrIsolation}"));
            
            command.Handler = CommandHandler.Create((SitecoreInitialiseArgument arg) => DoInitialise(arg));
        }
        private async Task DoInitialise(SitecoreInitialiseArgument arg)
        {
            var identityCertificatePassword = NonceService.Generate();

            var certificate = CreateCertificate("dimmy.sitecore.plugin", "localhost");
                
             var x509Certificate2Export = certificate.Export(X509ContentType.Pfx, identityCertificatePassword);
             var x509Certificate2Base64String = Convert.ToBase64String(x509Certificate2Export);

             arg.PrivateVariables = new Dictionary<string, string>
            {
                {"MsSql.SaPassword", NonceService.Generate()},
                {"Sitecore.TelerikEncryptionKey", NonceService.Generate()},
                {"Sitecore.AdminPassword", NonceService.Generate()},
                {"Sitecore.License", CreateEncodedSitecoreLicense(arg)},
                {"Sitecore.Id.Secret", NonceService.Generate()},
                {"Sitecore.Id.CertificatePassword", identityCertificatePassword},
                {"Sitecore.Id.Certificate", x509Certificate2Base64String},
            };
            
            arg.PublicVariables = new Dictionary<string, string>
            {
                {"Traefik.Image", arg.TraefikIImage},
                {"Traefik.Isolation", arg.TraefikIsolation},
                
                {"Redis.Image", $"{arg.Registry}/sitecore-redis:10.0.0-{arg.WindowsVersion}"},
                {"Redis.Isolation", arg.RedisIsolation},
                
                {"MsSql.Image", $"{arg.Registry}/sitecore-{arg.Topology}-mssql:10.0.0-{arg.WindowsVersion}"},
                {"MsSql.Isolation", arg.MssqlIsolation},
                
                {"Solr.Image", $"{arg.Registry}/sitecore-{arg.Topology}-solr:10.0.0-{arg.WindowsVersion}"},
                {"Solr.Isolation", arg.SolrIsolation},
                
                {"Sitecore.Id.Image", $"{arg.Registry}/sitecore-id:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.Id.HostName", arg.IdHostName},
                {"Sitecore.Id.Isolation", arg.IdIsolation},
                
                {"Sitecore.Cd.Image", $"{arg.Registry}/sitecore-{arg.Topology}-cd:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.Cd.HostName", arg.CdHostName},
                {"Sitecore.Cd.Isolation", arg.CdIsolation},
                
                {"Sitecore.Cm.Image", $"{arg.Registry}/sitecore-{arg.Topology}-cm:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.Cm.HostName", arg.CmHostName},
                {"Sitecore.Cm.Isolation", arg.CmIsolation},
            };
            
            var templateFile = Path.Join(TemplatePath, $"{arg.Topology}.docker-compose.template.yml");
            
            arg.DockerComposeTemplatePath = templateFile;
            
            await InitialiseProjectCommandHandler.Handle(arg);
        }
    }
}