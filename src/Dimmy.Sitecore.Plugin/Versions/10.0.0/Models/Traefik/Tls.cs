using System.Collections.Generic;

namespace Dimmy.Sitecore.Plugin.Versions._10._0._0.Models.Traefik
{
    public class Tls
    {
        public IList<Certificate> Certificates { get; set; } = new List<Certificate>();

    }
}