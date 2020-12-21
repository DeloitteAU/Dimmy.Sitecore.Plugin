using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using Dimmy.Engine.Pipelines;
using Dimmy.Engine.Pipelines.InitialiseProject;
using NuGet.Packaging;

namespace Dimmy.Sitecore.Plugin.Versions._9._1._0
{
    public class SitecoreInitialise :SitecoreInitialiseBase<SitecoreInitialiseContext>
    {
        public override string Name => "sitecore-9.1.0";
        public override string Description => "Initialise a Sitecore 9.1.0 project.";
        
        protected override string Version => "9.1.0";
        
        public SitecoreInitialise(Pipeline<Node<IInitialiseProjectContext>, IInitialiseProjectContext> initialiseProjectPipeline)
            : base(initialiseProjectPipeline)
        {
        }
        protected override void DoHydrateCommand(Command command, SitecoreInitialiseContext arg)
        {
        }

        protected override void DoInitialise(SitecoreInitialiseContext context)
        {
            context.PrivateVariables.AddRange(new Dictionary<string, string>
            {
                {"Sitecore.CM.Port", "44001"},
                {"Sitecore.CD.Port", "44002"},
                {"Sitecore.Sq.lPort", "44010"},
                {"Sitecore.Solr.Port", "44011"}
            });
            
            var windowsServerCore = $"9.3.0-windowsservercore-{context.WindowsServerCoreVersion}";
            var nanoServer = $"9.3.0-nanoserver-{context.NanoServerVersion}";
            var registry = context.Registries.First().Value;
            context.PublicVariables.AddRange(new Dictionary<string, string>
            {
                {"MsSql.Image", $"{registry}/sitecore-{context.Topology}-sqldev:{windowsServerCore}"},
                {"Solr.Image", $"{registry}/sitecore-{context.Topology}-solr:{nanoServer}"},
                {"Sitecore.XConnect.Image", $"{registry}/sitecore-{context.Topology}-xconnect:{windowsServerCore}"},
                {"Sitecore.XConnectAutomationEngine.Image", $"{registry}/sitecore-{context.Topology}-xconnect-automationengine:{windowsServerCore}"},
                {"Sitecore.XConnectIndexWorker.Image", $"{registry}/sitecore-{context.Topology}-xconnect-indexworker:{windowsServerCore}"},
                {"Sitecore.XConnectProcessingEngine.Image", $"{registry}/sitecore-{context.Topology}-xconnect-processingengine:{windowsServerCore}"},
                {"Sitecore.CD.Image", $"{registry}/sitecore-{context.Topology}-cd:{windowsServerCore}"},
                {"Sitecore.CM.Image", $"{registry}/sitecore-{context.Topology}-standalone:{windowsServerCore}"},
            });
        }
    }
}