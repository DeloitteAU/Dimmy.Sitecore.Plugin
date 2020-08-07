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

        public SitecoreInitialiseArgument()
        {
            Registry = "scr.sitecore.com/sxp";
            Topology = "xm1";
            WindowsVersion = "1909";
        }
    }
}