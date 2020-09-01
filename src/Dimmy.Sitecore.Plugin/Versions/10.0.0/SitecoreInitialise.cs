using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Dimmy.Engine.Commands;
using Dimmy.Engine.Commands.Project;
using Dimmy.Engine.Services;
using NuGet.Packaging;

namespace Dimmy.Sitecore.Plugin.Versions._10._0._0
{
    public class SitecoreInitialise : SitecoreInitialiseBase<SitecoreInitialiseArgument>
    {
        private readonly ICertificateService _certificateService;
        public override string Name => "sitecore-10.0.0";
        public override string Description => "Initialise a Sitecore 10.0.0 project.";

        protected override string Version => "10.0.0";

        public SitecoreInitialise(
            ICertificateService certificateService,
            ICommandHandler<InitialiseProject> initialiseProjectCommandHandler) : base(
            initialiseProjectCommandHandler)
        {
            _certificateService = certificateService;
        }

        protected override void DoHydrateCommand(Command command, SitecoreInitialiseArgument arg)
        {
            command.AddOption(new Option<string>("--cd-host-name", $"the host name fo the CD server. Defaults to {arg.CdHostName}"));
            command.AddOption(new Option<string>("--cm-host-name", $"the host name fo the CM server. Defaults to {arg.CmHostName}"));
            command.AddOption(new Option<string>("--id-host-name", $"the host name fo the CD server. Defaults to {arg.IdHostName}"));
            command.AddOption(new Option<string>("--traefik-image", $"the docker isolation for traefik. Defaults to {arg.TraefikIImage}"));
        }
        protected override void DoInitialise(SitecoreInitialiseArgument arg)
        {
            var identityCertificatePassword = NonceService.Generate();

            var certificate = _certificateService.CreateSelfSignedCertificate("dimmy.sitecore.plugin", "localhost");
                
            var x509Certificate2Export = certificate.Export(X509ContentType.Pfx, identityCertificatePassword);
            var x509Certificate2Base64String = Convert.ToBase64String(x509Certificate2Export);

            arg.PrivateVariables.AddRange(new Dictionary<string, string>
            {
                {"Sitecore.AdminPassword", NonceService.Generate()},
                {"Sitecore.Id.Secret", NonceService.Generate()},
                {"Sitecore.Id.CertificatePassword", identityCertificatePassword},
                {"Sitecore.Id.Certificate", x509Certificate2Base64String},
                {"Sitecore.Rep.ApiKey", NonceService.Generate()},
                {"Sitecore.Xc.Engine.Authoring.ClientId", NonceService.Generate()},
            });

            arg.PublicVariables.AddRange(new Dictionary<string, string>
            {
                {"Traefik.Image", arg.TraefikIImage},

                {"Redis.Image", $"{arg.Registries["sxp"]}/sitecore-redis:10.0.0-{arg.WindowsVersion}"},

                {"MsSql.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-mssql:10.0.0-{arg.WindowsVersion}"},

                {"Solr.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-solr:10.0.0-{arg.WindowsVersion}"},

                {"Sitecore.Id.Image", $"{arg.Registries["sxp"]}/sitecore-id:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.Id.HostName", arg.IdHostName},

                {"Sitecore.Cd.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-cd:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.Cd.HostName", arg.CdHostName},

                {"Sitecore.Cm.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-cm:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.Cm.HostName", arg.CmHostName},

                {"Sitecore.Prc.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-prc:10.0.0-{arg.WindowsVersion}"},

                {"Sitecore.Rep.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-rep:10.0.0-{arg.WindowsVersion}"},

                {"Sitecore.XConnect.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-xconnect:10.0.0-{arg.WindowsVersion}"},

                {"Sitecore.XDBCollection.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-xdbcollection:10.0.0-{arg.WindowsVersion}"},

                {"Sitecore.XDBSearch.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-xdbsearch:10.0.0-{arg.WindowsVersion}"},

                {"Sitecore.XDBAutomation.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-xdbautomation:10.0.0-{arg.WindowsVersion}"},

                {"Sitecore.XDBAutomationRpt.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-xdbautomationrpt:10.0.0-{arg.WindowsVersion}"},

                {"Sitecore.CortexProcessing.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-cortexprocessing:10.0.0-{arg.WindowsVersion}"},

                {"Sitecore.CortexReporting.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-cortexreporting:10.0.0-{arg.WindowsVersion}"},

                {"Sitecore.XBDRefData.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-xdbrefdata:10.0.0-{arg.WindowsVersion}"},

                {"Sitecore.XDBSearchWorker.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-xdbsearchworker:10.0.0-{arg.WindowsVersion}"},

                {"Sitecore.XDBAutomationWorker.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-xdbautomationworker:10.0.0-{arg.WindowsVersion}"},

                {"Sitecore.CortexProcessingWorker.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-cortexprocessingworker:10.0.0-{arg.WindowsVersion}"},

                // Commerce
                
                {"Sitecore.Xc.BizFx.Image", $"{arg.Registries["sxc"]}/sitecore-xc-engine:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.XC.BizFx.HostName", arg.XcBizFxHostName},

                {"Sitecore.Xc.EngineAuthoring.Image", $"{arg.Registries["sxc"]}/sitecore-xc-engine:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.XC.EngineAuthoring.HostName", arg.XcEngineAuthoringHostName},

                {"Sitecore.Xc.GlobalTrustedConnection", arg.XcGlobalTrustedConnection.ToString()},
                {"Sitecore.Xc.SharedTrustedConnection", arg.XcSharedTrustedConnection.ToString()},
                
                {"Sitecore.Xc.Engine.GlobalDatabaseName", arg.XcEngineGlobalDatabaseName},
                {"Sitecore.Xc.Engine.SharedDatabaseName", arg.XcEngineSharedDatabaseName},
                
                {"Sitecore.Xc.EngineShops.Image", $"{arg.Registries["sxc"]}/sitecore-xc-engine:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.XC.EngineShops.HostName", arg.XcEngineShopsHostName},

                {"Sitecore.Xc.EngineMinions.Image", $"{arg.Registries["sxc"]}/sitecore-xc-engine:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.XC.EngineMinions.HostName", arg.XcEngineMinionsHostName},

                {"Sitecore.Xc.EngineOps.Image", $"{arg.Registries["sxc"]}/sitecore-xc-engine:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.XC.EngineOps.HostName", arg.XcEngineOpsHostName},

                {"Sitecore.Xc.Braintree.Environment", arg.XcBraintreeEnvironment},
                {"Sitecore.Xc.Braintree.MerchantId", arg.XcBraintreeMerchantId},
                {"Sitecore.Xc.Braintree.PublicKey", arg.XcBraintreePublicKey},
                {"Sitecore.Xc.Braintree.PrivateKey", arg.XcBraintreePrivateKey},
            });
            
            var templateFile = Path.Join(TemplatePath, $"{arg.Topology}.docker-compose.template.yml");
            
            arg.DockerComposeTemplatePath = templateFile;

            InitialiseProjectCommandHandler.Handle(arg);
        }
    }
}