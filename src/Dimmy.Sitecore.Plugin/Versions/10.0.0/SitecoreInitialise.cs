using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Dimmy.Engine.Pipelines;
using Dimmy.Engine.Pipelines.InitialiseProject;
using Dimmy.Engine.Services;
using NuGet.Packaging;

namespace Dimmy.Sitecore.Plugin.Versions._10._0._0
{
    public class SitecoreInitialise : SitecoreInitialiseBase<SitecoreInitialiseContext>
    {
        private readonly ICertificateService _certificateService;
        public override string Name => "sitecore-10.0.0";
        public override string Description => "Initialise a Sitecore 10.0.0 project.";

        protected override string Version => "10.0.0";

        public SitecoreInitialise(
            ICertificateService certificateService,
            Pipeline<Node<IInitialiseProjectContext>, IInitialiseProjectContext> initialiseProjectPipeline) : base(
            initialiseProjectPipeline)
        {
            _certificateService = certificateService;
        }

        protected override void DoHydrateCommand(Command command, SitecoreInitialiseContext arg)
        {
            command.AddOption(new Option<string>("--cd-host-name", $"the host name fo the CD server. Defaults to {arg.CdHostName}"));
            command.AddOption(new Option<string>("--cm-host-name", $"the host name fo the CM server. Defaults to {arg.CmHostName}"));
            command.AddOption(new Option<string>("--id-host-name", $"the host name fo the CD server. Defaults to {arg.IdHostName}"));
            command.AddOption(new Option<string>("--traefik-image", $"the docker isolation for traefik. Defaults to {arg.TraefikIImage}"));
        }
        protected override void DoInitialise(SitecoreInitialiseContext context)
        {
            var identityCertificatePassword = NonceService.Generate();

            var certificate = _certificateService.CreateSelfSignedCertificate("dimmy.sitecore.plugin", "localhost");
                
            var x509Certificate2Export = certificate.Export(X509ContentType.Pfx, identityCertificatePassword);
            var x509Certificate2Base64String = Convert.ToBase64String(x509Certificate2Export);

            context.PrivateVariables.AddRange(new Dictionary<string, string>
            {
                {"Sitecore.AdminPassword", NonceService.Generate()},
                {"Sitecore.Id.Secret", NonceService.Generate()},
                {"Sitecore.Id.CertificatePassword", identityCertificatePassword},
                {"Sitecore.Id.Certificate", x509Certificate2Base64String},
                {"Sitecore.Rep.ApiKey", NonceService.Generate()},
                {"Sitecore.Xc.Engine.Authoring.ClientId", NonceService.Generate()},
            });

            context.PublicVariables.AddRange(new Dictionary<string, string>
            {
                {"Traefik.Image", context.TraefikIImage},

                {"Redis.Image", $"{context.Registries["sxp"]}/sitecore-redis:10.0.0-{context.WindowsVersion}"},

                {"MsSql.Image", $"{context.Registries["sxp"]}/sitecore-{context.Topology}-mssql:10.0.0-{context.WindowsVersion}"},

                {"Solr.Image", $"{context.Registries["sxp"]}/sitecore-{context.Topology}-solr:10.0.0-{context.WindowsVersion}"},

                {"Sitecore.Id.Image", $"{context.Registries["sxp"]}/sitecore-id:10.0.0-{context.WindowsVersion}"},
                {"Sitecore.Id.HostName", context.IdHostName},

                {"Sitecore.Cd.Image", $"{context.Registries["sxp"]}/sitecore-{context.Topology}-cd:10.0.0-{context.WindowsVersion}"},
                {"Sitecore.Cd.HostName", context.CdHostName},

                {"Sitecore.Cm.Image", $"{context.Registries["sxp"]}/sitecore-{context.Topology}-cm:10.0.0-{context.WindowsVersion}"},
                {"Sitecore.Cm.HostName", context.CmHostName},

                {"Sitecore.Prc.Image", $"{context.Registries["sxp"]}/sitecore-{context.Topology}-prc:10.0.0-{context.WindowsVersion}"},

                {"Sitecore.Rep.Image", $"{context.Registries["sxp"]}/sitecore-{context.Topology}-rep:10.0.0-{context.WindowsVersion}"},

                {"Sitecore.XConnect.Image", $"{context.Registries["sxp"]}/sitecore-{context.Topology}-xconnect:10.0.0-{context.WindowsVersion}"},

                {"Sitecore.XDBCollection.Image", $"{context.Registries["sxp"]}/sitecore-{context.Topology}-xdbcollection:10.0.0-{context.WindowsVersion}"},

                {"Sitecore.XDBSearch.Image", $"{context.Registries["sxp"]}/sitecore-{context.Topology}-xdbsearch:10.0.0-{context.WindowsVersion}"},

                {"Sitecore.XDBAutomation.Image", $"{context.Registries["sxp"]}/sitecore-{context.Topology}-xdbautomation:10.0.0-{context.WindowsVersion}"},

                {"Sitecore.XDBAutomationRpt.Image", $"{context.Registries["sxp"]}/sitecore-{context.Topology}-xdbautomationrpt:10.0.0-{context.WindowsVersion}"},

                {"Sitecore.CortexProcessing.Image", $"{context.Registries["sxp"]}/sitecore-{context.Topology}-cortexprocessing:10.0.0-{context.WindowsVersion}"},

                {"Sitecore.CortexReporting.Image", $"{context.Registries["sxp"]}/sitecore-{context.Topology}-cortexreporting:10.0.0-{context.WindowsVersion}"},

                {"Sitecore.XBDRefData.Image", $"{context.Registries["sxp"]}/sitecore-{context.Topology}-xdbrefdata:10.0.0-{context.WindowsVersion}"},

                {"Sitecore.XDBSearchWorker.Image", $"{context.Registries["sxp"]}/sitecore-{context.Topology}-xdbsearchworker:10.0.0-{context.WindowsVersion}"},

                {"Sitecore.XDBAutomationWorker.Image", $"{context.Registries["sxp"]}/sitecore-{context.Topology}-xdbautomationworker:10.0.0-{context.WindowsVersion}"},

                {"Sitecore.CortexProcessingWorker.Image", $"{context.Registries["sxp"]}/sitecore-{context.Topology}-cortexprocessingworker:10.0.0-{context.WindowsVersion}"},

                // Commerce
                
                {"Sitecore.Xc.BizFx.Image", $"{context.Registries["sxc"]}/sitecore-xc-engine:10.0.0-{context.WindowsVersion}"},
                {"Sitecore.XC.BizFx.HostName", context.XcBizFxHostName},

                {"Sitecore.Xc.EngineAuthoring.Image", $"{context.Registries["sxc"]}/sitecore-xc-engine:10.0.0-{context.WindowsVersion}"},
                {"Sitecore.XC.EngineAuthoring.HostName", context.XcEngineAuthoringHostName},

                {"Sitecore.Xc.GlobalTrustedConnection", context.XcGlobalTrustedConnection.ToString()},
                {"Sitecore.Xc.SharedTrustedConnection", context.XcSharedTrustedConnection.ToString()},
                
                {"Sitecore.Xc.Engine.GlobalDatabaseName", context.XcEngineGlobalDatabaseName},
                {"Sitecore.Xc.Engine.SharedDatabaseName", context.XcEngineSharedDatabaseName},
                
                {"Sitecore.Xc.EngineShops.Image", $"{context.Registries["sxc"]}/sitecore-xc-engine:10.0.0-{context.WindowsVersion}"},
                {"Sitecore.XC.EngineShops.HostName", context.XcEngineShopsHostName},

                {"Sitecore.Xc.EngineMinions.Image", $"{context.Registries["sxc"]}/sitecore-xc-engine:10.0.0-{context.WindowsVersion}"},
                {"Sitecore.XC.EngineMinions.HostName", context.XcEngineMinionsHostName},

                {"Sitecore.Xc.EngineOps.Image", $"{context.Registries["sxc"]}/sitecore-xc-engine:10.0.0-{context.WindowsVersion}"},
                {"Sitecore.XC.EngineOps.HostName", context.XcEngineOpsHostName},

                {"Sitecore.Xc.Braintree.Environment", context.XcBraintreeEnvironment},
                {"Sitecore.Xc.Braintree.MerchantId", context.XcBraintreeMerchantId},
                {"Sitecore.Xc.Braintree.PublicKey", context.XcBraintreePublicKey},
                {"Sitecore.Xc.Braintree.PrivateKey", context.XcBraintreePrivateKey},
            });
            
            var templateFile = Path.Join(TemplatePath, $"{context.Topology}.docker-compose.template.yml");
            
            context.DockerComposeTemplatePath = templateFile;
        }
    }
}