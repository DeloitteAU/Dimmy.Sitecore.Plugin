using System.Net;
using Dimmy.Engine.Commands;
using Dimmy.Engine.Commands.HostSystem;
using Dimmy.Engine.Pipelines;
using Dimmy.Engine.Pipelines.StartProject;

namespace Dimmy.Sitecore.Plugin.Versions._10._0._0.Pipeline.StartProject.Nodes
{
    public class AddHosts : Node<StartProjectContext>
    {
        private readonly ICommandHandler<AddHost> _addHostCommandHandler;

        public AddHosts(ICommandHandler<AddHost> addHostCommandHandler)
        {
            _addHostCommandHandler = addHostCommandHandler;
        }
        public override void DoExecute(StartProjectContext input)
        {
            DoAddHostEntry(input, Constants.CdHostName, "Sitecore CD Host");
            DoAddHostEntry(input, Constants.CmHostName, "Sitecore CM Host");
            DoAddHostEntry(input, Constants.IdHostName, "Sitecore ID Host");
        }

        private void DoAddHostEntry(StartProjectContext input, string sitecoreIdHostname, string comment)
        {
            var hostName = input.Project.VariableDictionary[sitecoreIdHostname];
            _addHostCommandHandler.Handle(new AddHost
            {
                Address = IPAddress.Loopback,
                Hostname = hostName,
                Comment = comment
            });
        }
    }
}