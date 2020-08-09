using System.Collections.Generic;
using System.CommandLine;
using Dimmy.Engine.Commands;
using Dimmy.Engine.Commands.Project;
using NuGet.Packaging;

namespace Dimmy.Sitecore.Plugin.Versions._9._3._0
{
    public class SitecoreInitialise :SitecoreInitialiseBase<SitecoreInitialiseArgument>
    {
        public override string Name => "sitecore-9.3.0";
        public override string Description => "Initialise a Sitecore 9.3.0 project.";
        
        protected override string Version => "9.3.0";
        
        public SitecoreInitialise(ICommandHandler<InitialiseProject> initialiseProjectCommandHandler) : base(initialiseProjectCommandHandler)
        {
        }
        protected override void DoHydrateCommand(Command command, SitecoreInitialiseArgument arg)
        {
        }

        protected override void DoInitialise(SitecoreInitialiseArgument arg)
        {
            arg.PrivateVariables.AddRange(new Dictionary<string, string>
            {
                {"Sitecore.CM.Port", "44001"},
                {"Sitecore.CD.Port", "44002"},
                {"Sitecore.Sq.lPort", "44010"},
                {"Sitecore.Solr.Port", "44011"}
            });
            
            var windowsServerCore = $"9.3.0-windowsservercore-{arg.WindowsServerCoreVersion}";
            var nanoServer = $"9.3.0-nanoserver-{arg.NanoServerVersion}";
            arg.PublicVariables.AddRange(new Dictionary<string, string>
            {
                {"MsSql.Image", $"{arg.Registry}/sitecore-{arg.Topology}-sqldev:{windowsServerCore}"},
                {"Solr.Image", $"{arg.Registry}/sitecore-{arg.Topology}-solr:{nanoServer}"},
                {"Sitecore.XConnect.Image", $"{arg.Registry}/sitecore-{arg.Topology}-xconnect:{windowsServerCore}"},
                {"Sitecore.XConnectAutomationEngine.Image", $"{arg.Registry}/sitecore-{arg.Topology}-xconnect-automationengine:{windowsServerCore}"},
                {"Sitecore.XConnectIndexWorker.Image", $"{arg.Registry}/sitecore-{arg.Topology}-xconnect-indexworker:{windowsServerCore}"},
                {"Sitecore.XConnectProcessingEngine.Image", $"{arg.Registry}/sitecore-{arg.Topology}-xconnect-processingengine:{windowsServerCore}"},
                {"Sitecore.CD.Image", $"{arg.Registry}/sitecore-{arg.Topology}-cd:{windowsServerCore}"},
                {"Sitecore.CM.Image", $"{arg.Registry}/sitecore-{arg.Topology}-standalone:{windowsServerCore}"},
            });
        }
    }
}