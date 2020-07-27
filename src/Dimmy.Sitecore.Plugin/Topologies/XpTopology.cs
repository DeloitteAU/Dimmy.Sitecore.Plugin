namespace Dimmy.Sitecore.Plugin.Topologies
{
    public class XpTopology:Topology
    {
        public override string Name => "xp";
        public override string DockerComposeTemplate => "docker-compose.xp.template.yml";
    }
}
