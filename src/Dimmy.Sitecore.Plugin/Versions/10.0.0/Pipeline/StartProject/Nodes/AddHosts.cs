using System.Collections.Generic;
using System.Linq;
using System.Net;
using Dimmy.Engine.Commands;
using Dimmy.Engine.Commands.HostSystem;
using Dimmy.Engine.Pipelines;
using Dimmy.Engine.Pipelines.StartProject;
using SharpHostsFile;

namespace Dimmy.Sitecore.Plugin.Versions._10._0._0.Pipeline.StartProject.Nodes
{
    public class AddHosts : Node<IStartProjectContext>
    {
        private readonly ICommandHandler<AddHostsFileMapEntries> _addHostCommandHandler;

        public AddHosts(ICommandHandler<AddHostsFileMapEntries> addHostCommandHandler)
        {
            _addHostCommandHandler = addHostCommandHandler;
        }
        public override void DoExecute(IStartProjectContext input)
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
            
            
            _addHostCommandHandler.Handle(new AddHostsFileMapEntries
            {
                HostsFileMapEntries = hostsFileEntries
            });
        }

    
    }
}