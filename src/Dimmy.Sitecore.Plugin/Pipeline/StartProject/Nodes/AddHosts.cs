using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dimmy.Engine.Pipelines;
using Dimmy.Engine.Pipelines.StartProject;
using Dimmy.Engine.Services.Hosts;
using SharpHostsFile;

namespace Dimmy.Sitecore.Plugin.Pipeline.StartProject.Nodes
{
    public class AddHosts : Node<IStartProjectContext>
    {
        private readonly IHostsFileService _hostsFileService;

        public AddHosts(IHostsFileService hostsFileService)
        {
            _hostsFileService = hostsFileService;
        }
        public override async Task DoExecute(IStartProjectContext input)
        {
            var hostsFileEntries = new List<HostsFileEntryBase>();
            foreach (var service in input.DockerComposeFileConfig.ServiceDefinitions)
            {
                if (!service.Labels.ContainsKey("traefik.enable")) continue;
                
                var host = service
                    .Labels
                    .Single(l => l.Key.EndsWith("rule"))
                    .Value;

                var hostName = host
                    .Replace("Host(`", "")
                    .Replace("`)", "");
                
                var hostsFileMapEntry = new HostsFileMapEntry(
                    IPAddress.Loopback,
                    hostName,
                    $"{service.Name} Host"
                );
                hostsFileEntries.Add(hostsFileMapEntry);
            }
            
            _hostsFileService.AddHostsFileEntry(hostsFileEntries);
        }
    }
}