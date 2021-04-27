using System.Collections.Generic;

namespace Dimmy.Sitecore.Plugin.Models.Traefik
{
    public class Tls
    {
        public IList<Certificate> Certificates { get; set; } = new List<Certificate>();
    }
}