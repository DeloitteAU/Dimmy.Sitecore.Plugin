namespace Dimmy.Sitecore.Plugin.Versions._10._0._0
{
    public class SitecoreInitialiseArgument : Sitecore.Plugin.SitecoreInitialiseArgument
    {
        public string CdHostName { get; set; } = "xm1cd.100.localhost";
        public string CmHostName { get; set; } = "xm1cm.100.localhost";
        public string IdHostName { get; set; } = "xm1id.100.localhost";
        public string TraefikIImage { get; set; } = "traefik:v2.2.0-windowsservercore-1809";

        // Commerce
        public string XcEngineAuthoringHostName { get; set; } = "authoring.localhost";

        public string XcBizFxHostName { get; set; } = "bizfx.100.localhost";

        public bool XcGlobalTrustedConnection { get; set; } = false;
        public bool XcSharedTrustedConnection { get; set; } = false;
        
        public string XcEngineGlobalDatabaseName { get; set; } = "SitecoreCommerce_Global";
        public string XcEngineSharedDatabaseName { get; set; } = "SitecoreCommerce_SharedEnvironments";
        
        public string XcEngineShopsHostName { get; set; } = "shops.100.localhost";

        public string XcEngineMinionsHostName { get; set; } = "minions.100.localhost";
       
        public string XcEngineOpsHostName { get; set; } = "ops.100.localhost";

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