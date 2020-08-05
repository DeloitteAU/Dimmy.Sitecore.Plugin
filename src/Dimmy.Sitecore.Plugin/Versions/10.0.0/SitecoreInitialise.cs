using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Dimmy.Engine.Commands;
using Dimmy.Engine.Commands.Project;
using Dimmy.Engine.Services;

namespace Dimmy.Sitecore.Plugin.Versions._10._0._0
{
    public class SitecoreInitialise : SitecoreInitialiseVersion<SitecoreInitialiseArgument>
    {
        public override string Name => "10.0.0";
        public override string Description => "Initialise a Sitecore 10.0.0 project.";
        public override Sitecore.Plugin.SitecoreInitialiseArgument HydrateCommand(Command command)
        {
            var sitecoreInitialiseArgument = new SitecoreInitialiseArgument();
            command.AddOption(new Option<string>("--cd-host-name", $"the host name fo the CD server. Defaults to {sitecoreInitialiseArgument.CdHostName}"));
            command.AddOption(new Option<string>("--cm-host-name", $"the host name fo the CM server. Defaults to {sitecoreInitialiseArgument.CmHostName}"));
            command.AddOption(new Option<string>("--id-host-name", $"the host name fo the CD server. Defaults to {sitecoreInitialiseArgument.IdHostName}"));
            
            command.AddOption(new Option<string>("--traefik-isolation", $"the docker isolation for traefik. Defaults to {sitecoreInitialiseArgument.TraefikIsolation}"));
            command.AddOption(new Option<string>("--traefik-image", $"the docker isolation for traefik. Defaults to {sitecoreInitialiseArgument.TraefikIImage}"));
            
            command.AddOption(new Option<string>("--redis-isolation", $"the docker isolation for redis. Defaults to {sitecoreInitialiseArgument.RedisIsolation}"));
            command.AddOption(new Option<string>("--mssql-isolation", $"the docker isolation for MsSql. Defaults to {sitecoreInitialiseArgument.MssqlIsolation}"));
            command.AddOption(new Option<string>("--solr-isolation", $"the docker isolation for Solr. Defaults to {sitecoreInitialiseArgument.SolrIsolation}"));
            
            command.Handler = CommandHandler.Create((SitecoreInitialiseArgument arg) => DoInitialise(arg));

            return sitecoreInitialiseArgument;
        }
        private async Task DoInitialise(SitecoreInitialiseArgument arg)
        {
            var identityCertificatePassword = NonceService.Generate();
            arg.PrivateVariables = new Dictionary<string, string>
            {
                {"MsSql.SaPassword", NonceService.Generate()},
                {"Sitecore.TelerikEncryptionKey", NonceService.Generate()},
                {"Sitecore.AdminPassword", NonceService.Generate()},
                {"Sitecore.License", CreateEncodedSitecoreLicense(arg)},
                {"Sitecore.Id.Secret", NonceService.Generate()},
                {"Sitecore.Id.CertificatePassword", identityCertificatePassword},
                {"Sitecore.Id.Certificate", CreateEncodedCertificate(identityCertificatePassword)},
                {"Sitecore.SqlPort", "44010"},
                {"Sitecore.SolrPort", "44011"}
            };
            
            arg.PublicVariables = new Dictionary<string, string>
            {
                {"Traefik.Image", arg.TraefikIImage},
                {"Traefik.Isolation", arg.TraefikIsolation},
                
                {"Redis.Image", $"{arg.Registry}/sitecore-redis:10.0.0-1909"},
                {"Redis.Isolation", arg.RedisIsolation},
                
                {"MsSql.Image", $"{arg.Registry}/sitecore-xm1-mssql:10.0.0-1909"},
                {"MsSql.Isolation", arg.MssqlIsolation},
                
                {"Solr.Image", $"{arg.Registry}/sitecore-xm1-solr:10.0.0-1909"},
                {"Solr.Isolation", arg.SolrIsolation},
                
                {"Sitecore.Id.Image", $"{arg.Registry}/sitecore-id:10.0.0-1909"},
                {"Sitecore.Id.HostName", arg.IdHostName},
                {"Sitecore.Id.Isolation", arg.IdIsolation},
                
                {"Sitecore.Cd.Image", $"{arg.Registry}/sitecore-xm1-cd:10.0.0-1909"},
                {"Sitecore.Cd.HostName", arg.CdHostName},
                {"Sitecore.Cd.Isolation", arg.CdIsolation},
                
                {"Sitecore.Cm.Image", $"{arg.Registry}/sitecore-xm1-cm:10.0.0-1909"},
                {"Sitecore.Cm.HostName", arg.CmHostName},
                {"Sitecore.Cm.Isolation", arg.CmIsolation},
            };
            
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var templateFile = Path.Join(assemblyPath, "Versions/10.0.0/docker-compose.xm1.template.yml");
            
            arg.DockerComposeTemplatePath = templateFile;
            
            await InitialiseProjectCommandHandler.Handle(arg);
        }

        public SitecoreInitialise(ICommandHandler<InitialiseProject> initialiseProjectCommandHandler) : base(initialiseProjectCommandHandler)
        {
        }
    }
}