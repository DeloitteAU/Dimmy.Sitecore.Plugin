﻿namespace Dimmy.Sitecore.Plugin.Versions._9._3._0
{
    public class SitecoreInitialiseArgument : Sitecore.Plugin.SitecoreInitialiseArgument
    {
        public string NanoServerVersion { get; set; } = "1809";
        public string WindowsServerCoreVersion { get; set; } = "ltsc2019";
    }
}