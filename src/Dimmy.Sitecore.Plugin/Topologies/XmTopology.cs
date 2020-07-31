
namespace Dimmy.Sitecore.Plugin.Topologies
{
    public class XmTopology : Topology
    {
        public override string Name => "xm";
        public override string DockerComposeTemplate => "docker-compose.xm.template.yml";
    }
}
