namespace Dimmy.Sitecore.Plugin.Versions._10._0._0
{
    public class SitecoreInitialiseContext : Sitecore.Plugin.SitecoreInitialiseContext
    {
        public string CdHostName { get; set; } = "xm1cd.localhost";
        public string CmHostName { get; set; } = "xm1cm.localhost";
        public string IdHostName { get; set; } = "xm1id.localhost";
        public string TraefikIImage { get; set; } = "traefik:v2.2.0-windowsservercore-1809";

        // Commerce
        public string XcEngineAuthoringHostName { get; set; } = "authoring.localhost";

        public string XcBizFxHostName { get; set; } = "bizfx.localhost";

        public bool XcGlobalTrustedConnection { get; set; } = false;
        public bool XcSharedTrustedConnection { get; set; } = false;
        
        public string XcEngineGlobalDatabaseName { get; set; } = "SitecoreCommerce_Global";
        public string XcEngineSharedDatabaseName { get; set; } = "SitecoreCommerce_SharedEnvironments";
        
        public string XcEngineShopsHostName { get; set; } = "shops.localhost";

        public string XcEngineMinionsHostName { get; set; } = "minions.localhost";
       
        public string XcEngineOpsHostName { get; set; } = "ops.localhost";

        public string XcBraintreeEnvironment { get; set; } = "sandbox";
        public string XcBraintreeMerchantId { get; set; }
        public string XcBraintreePublicKey { get; set; }
        public string XcBraintreePrivateKey { get; set; }
        public SitecoreInitialiseContext()
        {
            Registries.Add("sxp", "scr.sitecore.com/sxp");
            Registries.Add("sxc", "scr.sitecore.com/sxc");
            Topology = "xm1";
            WindowsVersion = "ltsc2019";
        }
    }
}