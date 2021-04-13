using System.CommandLine;
using Dimmy.Engine.Pipelines;
using Dimmy.Engine.Pipelines.InitialiseProject;

namespace Dimmy.Sitecore.Plugin.Versions._9._1._1
{
    public class SitecoreInitialise :SitecoreInitialiseBase<SitecoreInitialiseArgument>
    {
        public override string Name => "sitecore-9.1.1";
        public override string Description => "Initialise a Sitecore 9.1.1 project.";
        
        protected override string Version => "9.1.1";
        
        public SitecoreInitialise(Pipeline<Node<IInitialiseProjectContext>, IInitialiseProjectContext> initialiseProjectPipeline)
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
            context.PrivateVariables.Add("Sitecore.Sq.lPort", "44010");
            context.PrivateVariables.Add("Sitecore.Solr.Port", "44011");

            var windowsServerCore = $"{Version}-windowsservercore-{argument.WindowsServerCoreVersion}";
            var nanoServer = $"{Version}-nanoserver-{argument.NanoServerVersion}";
            context.PublicVariables.Add("MsSql.Image", $"{argument.Registry}/sitecore-{argument.Topology}-sqldev:{windowsServerCore}");
            context.PublicVariables.Add("Solr.Image", $"{argument.Registry}/sitecore-{argument.Topology}-solr:{nanoServer}");
            context.PublicVariables.Add("Sitecore.XConnect.Image", $"{argument.Registry}/sitecore-{argument.Topology}-xconnect:{windowsServerCore}");
            context.PublicVariables.Add("Sitecore.XConnectAutomationEngine.Image", $"{argument.Registry}/sitecore-{argument.Topology}-xconnect-automationengine:{windowsServerCore}");
            context.PublicVariables.Add("Sitecore.XConnectIndexWorker.Image", $"{argument.Registry}/sitecore-{argument.Topology}-xconnect-indexworker:{windowsServerCore}");
            context.PublicVariables.Add("Sitecore.XConnectProcessingEngine.Image", $"{argument.Registry}/sitecore-{argument.Topology}-xconnect-processingengine:{windowsServerCore}");
            context.PublicVariables.Add("Sitecore.CD.Image", $"{argument.Registry}/sitecore-{argument.Topology}-cd:{windowsServerCore}");
            context.PublicVariables.Add("Sitecore.CM.Image", $"{argument.Registry}/sitecore-{argument.Topology}-standalone:{windowsServerCore}");
        }
    }
}