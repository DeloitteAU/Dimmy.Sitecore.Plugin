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
            
            command.AddOption(new Option<string>("--cd-isolation", $"the docker isolation for CD. Defaults to {arg.CdIsolation}"));
            command.AddOption(new Option<string>("--cm-isolation", $"the docker isolation for CM. Defaults to {arg.CmIsolation}"));
            command.AddOption(new Option<string>("--id-isolation", $"the docker isolation for ID. Defaults to {arg.IdIsolation}"));
            command.AddOption(new Option<string>("--traefik-isolation", $"the docker isolation for traefik. Defaults to {arg.TraefikIsolation}"));
            command.AddOption(new Option<string>("--traefik-image", $"the docker isolation for traefik. Defaults to {arg.TraefikIImage}"));
            command.AddOption(new Option<string>("--redis-isolation", $"the docker isolation for redis. Defaults to {arg.RedisIsolation}"));
            command.AddOption(new Option<string>("--mssql-isolation", $"the docker isolation for MsSql. Defaults to {arg.MssqlIsolation}"));
            command.AddOption(new Option<string>("--solr-isolation", $"the docker isolation for Solr. Defaults to {arg.SolrIsolation}"));
            command.AddOption(new Option<string>("--xconnect-isolation", $"the docker isolation for xconnect. Defaults to {arg.XConnectIsolation}"));
            command.AddOption(new Option<string>("--prc-isolation", $"the docker isolation for Prc. Defaults to {arg.PrcIsolation}"));
            command.AddOption(new Option<string>("--rep-isolation", $"the docker isolation for Rep. Defaults to {arg.RepIsolation}"));
            command.AddOption(new Option<string>("--xdb-Search-isolation", $"the docker isolation for xDB Search. Defaults to {arg.XDBSearchIsolation}"));
            command.AddOption(new Option<string>("--xdb-automation-isolation", $"the docker isolation for xDB Automation. Defaults to {arg.XDBAutomationIsolation}"));
            command.AddOption(new Option<string>("--xdb-automation-rpt-isolation", $"the docker isolation for xDB Automation Rpt. Defaults to {arg.XDBAutomationRptIsolation}"));
            command.AddOption(new Option<string>("--cortex-processing-isolation", $"the docker isolation for Cortex Processing. Defaults to {arg.CortexProcessingIsolation}"));
            command.AddOption(new Option<string>("--cortex-reporting-isolation", $"the docker isolation for Cortex Reporting. Defaults to {arg.CortexReportingIsolation}"));
            command.AddOption(new Option<string>("--xdb-ref-data--isolation", $"the docker isolation for xBD Ref Data. Defaults to {arg.XBDRefDataIsolation}"));
            command.AddOption(new Option<string>("--xdb-search-worker-isolation", $"the docker isolation for xDB Search Worker. Defaults to {arg.XDBSearchWorkerIsolation}"));
            command.AddOption(new Option<string>("--xdb-automation-worker-isolation", $"the docker isolation for xDB Automation Worker. Defaults to {arg.XDBAutomationWorkerIsolation}"));
            command.AddOption(new Option<string>("--cortex-processing-worker-isolation", $"the docker isolation for Cortex Processing Worker. Defaults to {arg.CortexProcessingWorkerIsolation}"));
            
            command.AddOption(new Option<string>("--engine-authoring-isolation", $"the docker isolation for Commerce engine authoring. Defaults to {arg.XcEngineAuthoringIsolation}"));
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
                {"Traefik.Isolation", arg.TraefikIsolation},
                
                {"Redis.Image", $"{arg.Registries["sxp"]}/sitecore-redis:10.0.0-{arg.WindowsVersion}"},
                {"Redis.Isolation", arg.RedisIsolation},
                
                {"MsSql.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-mssql:10.0.0-{arg.WindowsVersion}"},
                {"MsSql.Isolation", arg.MssqlIsolation},
                
                {"Solr.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-solr:10.0.0-{arg.WindowsVersion}"},
                {"Solr.Isolation", arg.SolrIsolation},
                
                {"Sitecore.Id.Image", $"{arg.Registries["sxp"]}/sitecore-id:10.0.0-{arg.WindowsVersion}"},
                {Constants.IdHostName, arg.IdHostName},
                {"Sitecore.Id.Isolation", arg.IdIsolation},
                
                {"Sitecore.Cd.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-cd:10.0.0-{arg.WindowsVersion}"},
                {Constants.CdHostName, arg.CdHostName},
                {"Sitecore.Cd.Isolation", arg.CdIsolation},
                
                {"Sitecore.Cm.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-cm:10.0.0-{arg.WindowsVersion}"},
                {Constants.CmHostName, arg.CmHostName},
                {"Sitecore.Cm.Isolation", arg.CmIsolation},
                
                {"Sitecore.Prc.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-prc:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.Prc.Isolation", arg.PrcIsolation},
                
                {"Sitecore.Rep.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-rep:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.Rep.Isolation", arg.RepIsolation},
                
                {"Sitecore.XConnect.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-xconnect:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.XConnect.Isolation", arg.XConnectIsolation},
                
                {"Sitecore.XDBCollection.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-xdbcollection:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.XDBCollection.Isolation", arg.XDBCollectionIsolation},
                
                {"Sitecore.XDBSearch.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-xdbsearch:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.XDBSearch.Isolation", arg.XDBSearchIsolation},
                
                {"Sitecore.XDBAutomation.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-xdbautomation:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.XDBAutomation.Isolation", arg.XDBAutomationIsolation},
                
                {"Sitecore.XDBAutomationRpt.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-xdbautomationrpt:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.XDBAutomationRpt.Isolation", arg.XDBAutomationRptIsolation},
                
                {"Sitecore.CortexProcessing.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-cortexprocessing:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.CortexProcessing.Isolation", arg.CortexProcessingIsolation},
                
                {"Sitecore.CortexReporting.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-cortexreporting:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.CortexReporting.Isolation", arg.CortexReportingIsolation},
                
                {"Sitecore.XBDRefData.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-xdbrefdata:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.XBDRefData.Isolation", arg.XBDRefDataIsolation},
                
                {"Sitecore.XDBSearchWorker.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-xdbsearchworker:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.XDBSearchWorker.Isolation", arg.XDBSearchWorkerIsolation},
                
                {"Sitecore.XDBAutomationWorker.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-xdbautomationworker:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.XDBAutomationWorker.Isolation", arg.XDBAutomationWorkerIsolation},
                
                {"Sitecore.CortexProcessingWorker.Image", $"{arg.Registries["sxp"]}/sitecore-{arg.Topology}-cortexprocessingworker:10.0.0-{arg.WindowsVersion}"},
                {"Sitecore.CortexProcessingWorker.Isolation", arg.CortexProcessingWorkerIsolation},
                
                // Commerce
                
                {"Sitecore.Xc.BizFx.Image", $"{arg.Registries["sxc"]}/sitecore-xc-engine:10.0.0-{arg.WindowsVersion}"},
                {Constants.BizFxHostName, arg.XcBizFxHostName},
                {"Sitecore.Xc.BizFx.Isolation", arg.XcBizFxIsolation},
                
                {"Sitecore.Xc.EngineAuthoring.Image", $"{arg.Registries["sxc"]}/sitecore-xc-engine:10.0.0-{arg.WindowsVersion}"},
                {Constants.EngineAuthoringHostName, arg.XcEngineAuthoringHostName},
                {"Sitecore.Xc.EngineAuthoring.Isolation", arg.XcEngineAuthoringIsolation},
                
                {"Sitecore.Xc.GlobalTrustedConnection", arg.XcGlobalTrustedConnection.ToString()},
                {"Sitecore.Xc.SharedTrustedConnection", arg.XcSharedTrustedConnection.ToString()},
                
                {"Sitecore.Xc.Engine.GlobalDatabaseName", arg.XcEngineGlobalDatabaseName},
                {"Sitecore.Xc.Engine.SharedDatabaseName", arg.XcEngineSharedDatabaseName},
                
                {"Sitecore.Xc.Engine.SharedDatabaseName", arg.XcEngineSharedDatabaseName},
                
                {"Sitecore.Xc.EngineShops.Image", $"{arg.Registries["sxc"]}/sitecore-xc-engine:10.0.0-{arg.WindowsVersion}"},
                {Constants.EngineShopsHostName, arg.XcEngineShopsHostName},
                {"Sitecore.Xc.EngineShops.Isolation", arg.XcEngineShopsIsolation},
                
                {"Sitecore.Xc.EngineMinions.Image", $"{arg.Registries["sxc"]}/sitecore-xc-engine:10.0.0-{arg.WindowsVersion}"},
                {Constants.EngineMinionsHostName, arg.XcEngineMinionsHostName},
                {"Sitecore.Xc.EngineMinions.Isolation", arg.XcEngineMinionsIsolation},
                
                {"Sitecore.Xc.EngineOps.Image", $"{arg.Registries["sxc"]}/sitecore-xc-engine:10.0.0-{arg.WindowsVersion}"},
                {Constants.EngineOpsHostName, arg.XcEngineOpsHostName},
                {"Sitecore.Xc.EngineOps.Isolation", arg.XcEngineOpsIsolation},
                
                
                
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