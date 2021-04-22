using System;
using System.CommandLine;
using System.Security.Cryptography.X509Certificates;
using Dimmy.Engine.Pipelines;
using Dimmy.Engine.Pipelines.InitialiseProject;
using Dimmy.Engine.Services;

namespace Dimmy.Sitecore.Plugin.Versions._10._0._1
{
    public class SitecoreInitialise : SitecoreInitialiseBase<SitecoreInitialiseArgument>
    {
        private readonly ICertificateService _certificateService;
        public override string Name => "sitecore-10.0.1";
        public override string Description => "Initialise a Sitecore 10.0.1 project.";

        protected override string Version => "10.0.1";

        public SitecoreInitialise(
            ICertificateService certificateService,
            Pipeline<Node<IInitialiseProjectContext>, IInitialiseProjectContext> initialiseProjectPipeline) :
            base(initialiseProjectPipeline)
        {
            _certificateService = certificateService;
        }

        protected override void DoHydrateCommand(Command command, SitecoreInitialiseArgument arg)
        {
            command.AddOption(new Option<string>("--cd-host-name",
                $"the host name fo the CD server. Defaults to {arg.CdHostName}"));
            command.AddOption(new Option<string>("--cm-host-name",
                $"the host name fo the CM server. Defaults to {arg.CmHostName}"));
            command.AddOption(new Option<string>("--id-host-name",
                $"the host name fo the CD server. Defaults to {arg.IdHostName}"));
            command.AddOption(new Option<string>("--traefik-image",
                $"the docker isolation for traefik. Defaults to {arg.TraefikIImage}"));
        }

        protected override void DoInitialise(SitecoreInitialiseArgument argument, InitialiseProjectContext context)
        {
            argument.Registries.Add("sxp", "scr.sitecore.com/sxp");
            argument.Registries.Add("sxc", "scr.sitecore.com/sxc");
            
            var identityCertificatePassword = NonceService.Generate();

            var identityCertificate = _certificateService.CreateSelfSignedCertificate("dimmy.sitecore.plugin", "localhost");

            var x509IdentityCertificate2Export = identityCertificate.Export(X509ContentType.Pfx, identityCertificatePassword);
            var x509IdentityCertificate2Base64String = Convert.ToBase64String(x509IdentityCertificate2Export);

            context.PrivateVariables.Add("Sitecore.AdminPassword", NonceService.Generate());
            context.PrivateVariables.Add("Sitecore.Id.Secret", NonceService.Generate());
            context.PrivateVariables.Add("Sitecore.Id.CertificatePassword", identityCertificatePassword);
            context.PrivateVariables.Add("Sitecore.Id.Certificate", x509IdentityCertificate2Base64String);
            context.PrivateVariables.Add("Sitecore.Rep.ApiKey", NonceService.Generate());
            context.PrivateVariables.Add("Sitecore.Xc.Engine.Authoring.ClientId", NonceService.Generate());

            context.PublicVariables.Add("Traefik.Image", argument.TraefikIImage);

            context.PublicVariables.Add("Redis.Image",
                $"{argument.Registries["sxp"]}/sitecore-redis:10.0.0-{argument.WindowsVersion}");

            context.PublicVariables.Add("MsSql.Image",
                $"{argument.Registries["sxp"]}/sitecore-{argument.Topology}-mssql:10.0.0-{argument.WindowsVersion}");

            context.PublicVariables.Add("Solr.Image",
                $"{argument.Registries["sxp"]}/sitecore-{argument.Topology}-solr:10.0.0-{argument.WindowsVersion}");

            context.PublicVariables.Add("Sitecore.Id.Image",
                $"{argument.Registries["sxp"]}/sitecore-id:10.0.0-{argument.WindowsVersion}");
            context.PublicVariables.Add("Sitecore.Id.HostName", argument.IdHostName);

            context.PublicVariables.Add("Sitecore.Cd.Image",
                $"{argument.Registries["sxp"]}/sitecore-{argument.Topology}-cd:10.0.0-{argument.WindowsVersion}");
            context.PublicVariables.Add("Sitecore.Cd.HostName", argument.CdHostName);

            context.PublicVariables.Add("Sitecore.Cm.Image",
                $"{argument.Registries["sxp"]}/sitecore-{argument.Topology}-cm:10.0.0-{argument.WindowsVersion}");
            context.PublicVariables.Add("Sitecore.Cm.HostName", argument.CmHostName);

            context.PublicVariables.Add("Sitecore.Prc.Image",
                $"{argument.Registries["sxp"]}/sitecore-{argument.Topology}-prc:10.0.0-{argument.WindowsVersion}");

            context.PublicVariables.Add("Sitecore.Rep.Image",
                $"{argument.Registries["sxp"]}/sitecore-{argument.Topology}-rep:10.0.0-{argument.WindowsVersion}");

            context.PublicVariables.Add("Sitecore.XConnect.Image",
                $"{argument.Registries["sxp"]}/sitecore-{argument.Topology}-xconnect:10.0.0-{argument.WindowsVersion}");

            context.PublicVariables.Add("Sitecore.XDBCollection.Image",
                $"{argument.Registries["sxp"]}/sitecore-{argument.Topology}-xdbcollection:10.0.0-{argument.WindowsVersion}");

            context.PublicVariables.Add("Sitecore.XDBSearch.Image",
                $"{argument.Registries["sxp"]}/sitecore-{argument.Topology}-xdbsearch:10.0.0-{argument.WindowsVersion}");

            context.PublicVariables.Add("Sitecore.XDBAutomation.Image",
                $"{argument.Registries["sxp"]}/sitecore-{argument.Topology}-xdbautomation:10.0.0-{argument.WindowsVersion}");

            context.PublicVariables.Add("Sitecore.XDBAutomationRpt.Image",
                $"{argument.Registries["sxp"]}/sitecore-{argument.Topology}-xdbautomationrpt:10.0.0-{argument.WindowsVersion}");

            context.PublicVariables.Add("Sitecore.CortexProcessing.Image",
                $"{argument.Registries["sxp"]}/sitecore-{argument.Topology}-cortexprocessing:10.0.0-{argument.WindowsVersion}");

            context.PublicVariables.Add("Sitecore.CortexReporting.Image",
                $"{argument.Registries["sxp"]}/sitecore-{argument.Topology}-cortexreporting:10.0.0-{argument.WindowsVersion}");

            context.PublicVariables.Add("Sitecore.XBDRefData.Image",
                $"{argument.Registries["sxp"]}/sitecore-{argument.Topology}-xdbrefdata:10.0.0-{argument.WindowsVersion}");

            context.PublicVariables.Add("Sitecore.XDBSearchWorker.Image",
                $"{argument.Registries["sxp"]}/sitecore-{argument.Topology}-xdbsearchworker:10.0.0-{argument.WindowsVersion}");

            context.PublicVariables.Add("Sitecore.XDBAutomationWorker.Image",
                $"{argument.Registries["sxp"]}/sitecore-{argument.Topology}-xdbautomationworker:10.0.0-{argument.WindowsVersion}");

            context.PublicVariables.Add("Sitecore.CortexProcessingWorker.Image",
                $"{argument.Registries["sxp"]}/sitecore-{argument.Topology}-cortexprocessingworker:10.0.0-{argument.WindowsVersion}");

            // Commerce

            context.PublicVariables.Add("Sitecore.Xc.BizFx.Image",
                $"{argument.Registries["sxc"]}/sitecore-xc-engine:10.0.0-{argument.WindowsVersion}");
            context.PublicVariables.Add("Sitecore.XC.BizFx.HostName", argument.XcBizFxHostName);

            context.PublicVariables.Add("Sitecore.Xc.EngineAuthoring.Image",
                $"{argument.Registries["sxc"]}/sitecore-xc-engine:10.0.0-{argument.WindowsVersion}");
            context.PublicVariables.Add("Sitecore.XC.EngineAuthoring.HostName", argument.XcEngineAuthoringHostName);

            context.PublicVariables.Add("Sitecore.Xc.GlobalTrustedConnection",
                argument.XcGlobalTrustedConnection.ToString());
            context.PublicVariables.Add("Sitecore.Xc.SharedTrustedConnection",
                argument.XcSharedTrustedConnection.ToString());

            context.PublicVariables.Add("Sitecore.Xc.Engine.GlobalDatabaseName", argument.XcEngineGlobalDatabaseName);
            context.PublicVariables.Add("Sitecore.Xc.Engine.SharedDatabaseName", argument.XcEngineSharedDatabaseName);

            context.PublicVariables.Add("Sitecore.Xc.EngineShops.Image",
                $"{argument.Registries["sxc"]}/sitecore-xc-engine:10.0.0-{argument.WindowsVersion}");
            context.PublicVariables.Add("Sitecore.XC.EngineShops.HostName", argument.XcEngineShopsHostName);

            context.PublicVariables.Add("Sitecore.Xc.EngineMinions.Image",
                $"{argument.Registries["sxc"]}/sitecore-xc-engine:10.0.0-{argument.WindowsVersion}");
            context.PublicVariables.Add("Sitecore.XC.EngineMinions.HostName", argument.XcEngineMinionsHostName);

            context.PublicVariables.Add("Sitecore.Xc.EngineOps.Image",
                $"{argument.Registries["sxc"]}/sitecore-xc-engine:10.0.0-{argument.WindowsVersion}");
            context.PublicVariables.Add("Sitecore.XC.EngineOps.HostName", argument.XcEngineOpsHostName);

            context.PublicVariables.Add("Sitecore.Xc.Braintree.Environment", argument.XcBraintreeEnvironment);
            context.PublicVariables.Add("Sitecore.Xc.Braintree.MerchantId", argument.XcBraintreeMerchantId);
            context.PublicVariables.Add("Sitecore.Xc.Braintree.PublicKey", argument.XcBraintreePublicKey);
            context.PublicVariables.Add("Sitecore.Xc.Braintree.PrivateKey", argument.XcBraintreePrivateKey);
        }
    }
}