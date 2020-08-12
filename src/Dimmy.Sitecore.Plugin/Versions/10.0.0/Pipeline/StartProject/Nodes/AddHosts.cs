using System.Collections.Generic;
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
            _addHostCommandHandler.Handle(new AddHostsFileMapEntries
            {
                HostsFileMapEntries = new List<HostsFileEntryBase>
                {
                    BuildHostsFileMapEntry(input, Constants.CdHostName, "Sitecore CD Host"),
                    BuildHostsFileMapEntry(input, Constants.CmHostName, "Sitecore CM Host"),
                    BuildHostsFileMapEntry(input, Constants.IdHostName, "Sitecore ID Host")
                }
            });

        }

        private HostsFileMapEntry BuildHostsFileMapEntry(IStartProjectContext input, string sitecoreIdHostname, string comment)
        {
            var hostName = input.Project.VariableDictionary[sitecoreIdHostname];

            return new HostsFileMapEntry(
                IPAddress.Loopback,
                hostName,
                comment
            );

        }
    }
}