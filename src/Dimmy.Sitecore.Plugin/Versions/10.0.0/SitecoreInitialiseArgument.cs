namespace Dimmy.Sitecore.Plugin.Versions._10._0._0
{
    public class SitecoreInitialiseArgument : Sitecore.Plugin.SitecoreInitialiseArgument
    {
        public string CdHostName { get; set; } = "xm1cd.localhost";
        public string CdIsolation { get; set; } = "default";
        public string CmHostName { get; set; } = "xm1cm.localhost";
        public string CmIsolation { get; set; } = "default";
        public string IdHostName { get; set; } = "xm1id.localhost";
        public string IdIsolation { get; set; } = "default";
        public string TraefikIsolation { get; set; } = "hyperv";
        public string TraefikIImage { get; set; } = "traefik:v2.2.0-windowsservercore-1809";
        public string RedisIsolation { get; set; } = "default";
        public string MssqlIsolation { get; set; } = "default";
        public string SolrIsolation { get; set; } = "default";
        public string XConnectIsolation { get; set; } = "default";
        public string PrcIsolation { get; set; } = "default";
        public string RepIsolation { get; set; } = "default";
        public string XDBCollectionIsolation { get; set; } = "default";
        public string XDBSearchIsolation { get; set; } = "default";
        public string XDBAutomationIsolation { get; set; } = "default";
        public string XDBAutomationRptIsolation { get; set; } = "default";
        public string CortexProcessingIsolation { get; set; } = "default";
        public string CortexReportingIsolation { get; set; } = "default";
        public string XBDRefDataIsolation { get; set; } = "default";
        public string XDBSearchWorkerIsolation { get; set; } = "default";
        public string XDBAutomationWorkerIsolation { get; set; } = "default";
        public string CortexProcessingWorkerIsolation { get; set; } = "default";
        
        // Commerce
        public string XcEngineAuthoringHostName { get; set; } = "authoring.localhost";
        public string XcEngineAuthoringIsolation { get; set; } = "default";
        
        public string XcBizFxHostName { get; set; } = "bizfx.localhost";
        public string XcBizFxIsolation { get; set; } = "default";
        
        
        public bool XcGlobalTrustedConnection { get; set; } = false;
        public bool XcSharedTrustedConnection { get; set; } = false;
        
        
        public string XcEngineGlobalDatabaseName { get; set; } = "SitecoreCommerce_Global";
        public string XcEngineSharedDatabaseName { get; set; } = "SitecoreCommerce_SharedEnvironments";
        
        
        public string XcEngineShopsHostName { get; set; } = "shops.localhost";
        public string XcEngineShopsIsolation { get; set; } = "default";
        
        public string XcEngineMinionsHostName { get; set; } = "minions.localhost";
        public string XcEngineMinionsIsolation { get; set; } = "default";
        
        public string XcEngineOpsHostName { get; set; } = "ops.localhost";
        public string XcEngineOpsIsolation { get; set; } = "default";
        
        
        public string XcBraintreeEnvironment { get; set; } = "sandbox";
        public string XcBraintreeMerchantId { get; set; }
        public string XcBraintreePublicKey { get; set; }
        public string XcBraintreePrivateKey { get; set; }
        

        public SitecoreInitialiseArgument()
        {
            Registries.Add("sxp", "scr.sitecore.com/sxp");
            Registries.Add("sxc", "scr.sitecore.com/sxc");
            Topology = "xm1";
            WindowsVersion = "ltsc2019";
        }
    }
}