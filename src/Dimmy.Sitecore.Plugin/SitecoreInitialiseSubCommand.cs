using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dimmy.Cli.Commands.Project;
using Dimmy.Cli.Extensions;
using Dimmy.Engine.Commands;
using Dimmy.Engine.Commands.Project;
using Dimmy.Engine.Services;
using Dimmy.Sitecore.Plugin.Topologies;

namespace Dimmy.Sitecore.Plugin
{
    public class SitecoreInitialiseSubCommand: InitialiseSubCommand
    {
        private readonly IEnumerable<ITopology> _topologies;

        public SitecoreInitialiseSubCommand(
            IEnumerable<ITopology> topologies,
            ICommandHandler<InitialiseProject> initialiseProjectCommandHandler) : base(initialiseProjectCommandHandler)
        {
            _topologies = topologies;
        }

        protected override string Name => "sitecore";
        protected override string Description => "Initialise a Sitecore project.";

        protected override void HydrateCommand(Command command)
        {
            command.Handler = CommandHandler.Create((SitecoreInitialise si) =>DoInitialise(si));
            
            command.AddOption(new Option<string>("--license-path", "Path to the Sitecore License"));
            command.AddOption(new Option<string>("--topology-name", "The Sitecore topology you"));
            command.AddOption(new Option<string>("--sitecore-version", "The Sitecore Version"));
            command.AddOption(new Option<string>("--nano-server-version"));
            command.AddOption(new Option<string>("--windows-server-core-version"));
        }

        private async Task DoInitialise(SitecoreInitialise si)
        {
            GetUserInput(si);
            ResolveTemplate(si);
            await AddPrivateVariables(si);
            AddPublicVariables(si);
            await InitialiseProjectCommandHandler.Handle(si);
        }

        private static async Task AddPrivateVariables(SitecoreInitialise si)
        {
            si.PrivateVariables = new Dictionary<string, string>
            {
                {"Sitecore.SqlSaPassword", NonceService.Generate()},
                {"Sitecore.TelerikEncryptionKey", NonceService.Generate()},
                {"Sitecore.License", await CreatEncodedeSitecoreLicense(si)},
                {"Sitecore.CMPort", "44001"},
                {"Sitecore.CDPort", "44002"},
                {"Sitecore.SqlPort", "44010"},
                {"Sitecore.SolrPort", "44011"}
            };
        }

        private static void AddPublicVariables(SitecoreInitialise si)
        {
            var windowsServerCore = $"{si.SitecoreVersion}-windowsservercore-{si.WindowsServerCoreVersion}";
            var nanoServer = $"{si.SitecoreVersion}-nanoserver-${si.NanoServerVersion}";
            si.PublicVariables = new Dictionary<string, string>
            {
                {"SqlDockerImage", $"{si.Registry}/sitecore-xp-sqldev:{windowsServerCore}"},
                {"SolrDockerImage", $"{si.Registry}/sitecore-xp-solr:{nanoServer}"},
                {"XConnectDockerImage", $"{si.Registry}/sitecore-xp-xconnect:{windowsServerCore}"},
                {"XConnectAutomationEngineImage", $"{si.Registry}/sitecore-xp-xconnect-automationengine:{windowsServerCore}"},
                {"XConnectIndexWorkerImage", $"{si.Registry}/sitecore-xp-xconnect-indexworker:{windowsServerCore}"},
                {"XConnectProcessingEngineImage", $"{si.Registry}/sitecore-xp-xconnect-processingengine:{windowsServerCore}"},
                {"CDImage", $"{si.Registry}/sitecore-xp-cd:{windowsServerCore}"},
                {"CMImage", $"{si.Registry}/sitecore-xp-standalone:{windowsServerCore}"},
                {"VisualStudio.RemoteDebugger", @"C:\Program Files\Microsoft Visual Studio 16.0\Common7\IDE\Remote Debugger"},
            };
        }

        private void ResolveTemplate(SitecoreInitialise si)
        {
            if (!string.IsNullOrEmpty(si.DockerComposeTemplatePath) && !string.IsNullOrEmpty(si.TopologyName))
            {
                throw new MultipleDockerComposeTemplatesPassed();
            }

            if (!string.IsNullOrEmpty(si.TopologyName))
            {
                var topology = _topologies.Single(t => t.Name == si.TopologyName);
                var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var templateFile = Path.Join(assemblyPath, topology.DockerComposeTemplate);
                si.DockerComposeTemplatePath = templateFile;
            }
            else
            {
                throw new NoDockerComposeTemplatesPassed();
            }
        }

        private static async Task<string> CreatEncodedeSitecoreLicense(SitecoreInitialise si)
        {
            await using var licenseStream = File.OpenRead(si.LicensePath);
            var licenseMemoryStream = new MemoryStream();
            var licenseGZipStream = new GZipStream(licenseMemoryStream, CompressionLevel.Optimal, false);
            await licenseStream.CopyToAsync(licenseGZipStream);

            var sitecoreLicense = Convert.ToBase64String(licenseMemoryStream.ToArray());
            return sitecoreLicense;
        }

        private void GetUserInput(SitecoreInitialise sitecoreInitialise)
        {
            sitecoreInitialise.Name = sitecoreInitialise.Name.GetUserInput("Project Name:");
            sitecoreInitialise.SourceCodePath = sitecoreInitialise.SourceCodePath.GetUserInput("Source code path:");
            sitecoreInitialise.WorkingPath = sitecoreInitialise.WorkingPath.GetUserInput("Working path:");

            if (string.IsNullOrEmpty(sitecoreInitialise.TopologyName))
            {
                foreach (var topology in _topologies)
                {
                    Console.WriteLine(topology.Name);
                }
            }

            sitecoreInitialise.TopologyName = sitecoreInitialise.TopologyName.GetUserInput("Choose topology:");
            sitecoreInitialise.LicensePath = sitecoreInitialise.LicensePath.GetUserInput("license path:");
        }
    }
}
