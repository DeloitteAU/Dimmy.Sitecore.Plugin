using System.CommandLine;
using System.Linq;
using Dimmy.Engine.Pipelines;
using Dimmy.Engine.Pipelines.InitialiseProject;

namespace Dimmy.Sitecore.Plugin.Versions._9._3._0
{
    public class SitecoreInitialise : SitecoreInitialiseBase<SitecoreInitialiseArgument>
    {
        public override string Name => "sitecore-9.3.0";
        public override string Description => "Initialise a Sitecore 9.3.0 project.";

        protected override string Version => "9.3.0";

        public SitecoreInitialise(
            Pipeline<Node<IInitialiseProjectContext>, IInitialiseProjectContext> initialiseProjectPipeline)
            : base(initialiseProjectPipeline)
        {
        }

        protected override void DoHydrateCommand(Command command, SitecoreInitialiseArgument arg)
        {
        }

        protected override void DoInitialise(SitecoreInitialiseArgument argument, InitialiseProjectContext context)
        {
            context.PrivateVariables.Add("Sitecore.CM.Port", "44001");
            context.PrivateVariables.Add("Sitecore.CD.Port", "44002");
            context.PrivateVariables.Add("Sitecore.Sq.Port", "44010");
            context.PrivateVariables.Add("Sitecore.Solr.Port", "44011");
            
            var windowsServerCore = $"9.3.0-windowsservercore-{argument.WindowsServerCoreVersion}";
            var nanoServer = $"9.3.0-nanoserver-{argument.NanoServerVersion}";
            var registry = argument.Registries.First().Value;
            context.PublicVariables.Add("MsSql.Image", $"{registry}/sitecore-{argument.Topology}-sqldev:{windowsServerCore}");
            context.PublicVariables.Add("Solr.Image", $"{registry}/sitecore-{argument.Topology}-solr:{nanoServer}");
            context.PublicVariables.Add("Sitecore.XConnect.Image", $"{registry}/sitecore-{argument.Topology}-xconnect:{windowsServerCore}");
            context.PublicVariables.Add("Sitecore.XConnectAutomationEngine.Image", $"{registry}/sitecore-{argument.Topology}-xconnect-automationengine:{windowsServerCore}");
            context.PublicVariables.Add("Sitecore.XConnectIndexWorker.Image", $"{registry}/sitecore-{argument.Topology}-xconnect-indexworker:{windowsServerCore}");
            context.PublicVariables.Add("Sitecore.XConnectProcessingEngine.Image", $"{registry}/sitecore-{argument.Topology}-xconnect-processingengine:{windowsServerCore}");
            context.PublicVariables.Add("Sitecore.CD.Image", $"{registry}/sitecore-{argument.Topology}-cd:{windowsServerCore}");
            context.PublicVariables.Add("Sitecore.CM.Image", $"{registry}/sitecore-{argument.Topology}-standalone:{windowsServerCore}");
        }
    }
}