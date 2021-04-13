namespace Dimmy.Sitecore.Plugin.Versions._9._1._0
{
    public class SitecoreInitialiseArgument : Sitecore.Plugin.SitecoreInitialiseArgument
    {
        public string NanoServerVersion { get; set; } = "2004";
        public string WindowsServerCoreVersion { get; set; } = "2004";
        
        public string CdHostName { get; set; } = "xmcd.901.localhost";
        public string CmHostName { get; set; } = "xmcm.901.localhost";
        public string TraefikIImage { get; set; } = "traefik:v2.2.0-windowsservercore-1809";
        
        public SitecoreInitialiseArgument()
        {
            Topology = "xm";
            WindowsVersion = "2004";
        }
    }
}