using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Reflection;
using Dimmy.Cli.Commands.Project.SubCommands;
using Dimmy.Engine.Pipelines;
using Dimmy.Engine.Pipelines.InitialiseProject;
using Dimmy.Engine.Services;
using NuGet.Packaging;
using SharpCompress.Compressors;
using SharpCompress.Compressors.Deflate;

namespace Dimmy.Sitecore.Plugin
{
    public abstract class SitecoreInitialiseBase<TContext> : InitialiseSubCommand
        where TContext : SitecoreInitialiseContext, new()
    {
        protected readonly Pipeline<Node<IInitialiseProjectContext>, IInitialiseProjectContext> InitialiseProjectPipeline;

        public string TemplatePath
        {
            get
            {
                var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return Path.Join(assemblyPath, $"Versions/{Version}");
            }
        }
        protected abstract string Version { get; }

        private IEnumerable<string> Topologies
        {
            get
            {
                var templates = Directory.GetFiles(TemplatePath);

                foreach (var template in templates)
                {
                    yield return Path.GetFileName(template).Split(".")[0];
                }
            }
        }

        protected SitecoreInitialiseBase(
            Pipeline<Node<IInitialiseProjectContext>, IInitialiseProjectContext> initialiseProjectPipeline)
        {
            InitialiseProjectPipeline = initialiseProjectPipeline;
            
        }

        public override void HydrateCommand(Command command)
        {
            var arg = new TContext();
            
            command.AddOption(new Option<string>(
                "--license-path", 
                "Path to the Sitecore License"));
            
            command.AddOption(new Option<Dictionary<string, string>>("--registry", parseArgument: result =>
            {
                return result
                    .Tokens
                    .Select(t => t.Value.Split('='))
                    .ToDictionary(p => p[0], p => p[1]);
            }));
            
            command.AddOption(new Option<string>(
                "--topology", 
                $"The Sitecore topology. Defaults to {arg.Topology}. Options: \n {string.Join('\n', Topologies)}"));
            
            arg.PrivateVariables = new Dictionary<string, string>();
            arg.PublicVariables = new Dictionary<string, string>();

            DoHydrateCommand(command, arg);
            command.Handler = CommandHandler.Create((TContext arg) => Initialise(arg));
        }

        private void Initialise(TContext context)
        {
            context.PrivateVariables.AddRange(new Dictionary<string, string>
            {
                {"MsSql.SaPassword", NonceService.Generate()},
                {"Sitecore.License", CreateEncodedSitecoreLicense(context)},
                {"Sitecore.TelerikEncryptionKey", NonceService.Generate()},
            });
            
            context.MetaData.Add("SitecoreVersion", Version);
            context.MetaData.Add(Constants.MetaData.SitecoreTopology, context.Topology);
            
            DoInitialise(context);
            
            InitialiseProjectPipeline.Execute(context);
        }

        protected abstract void DoHydrateCommand(Command command, TContext arg);
        protected abstract void DoInitialise(TContext context);
        
        protected string CreateEncodedSitecoreLicense(TContext si)
        {
            var licenseBytes = File.ReadAllBytes(si.LicensePath);
            using var licenseMemoryStream = new MemoryStream();
            using var licenseGZipStream = new GZipStream(licenseMemoryStream, CompressionMode.Compress);
            licenseGZipStream.Write(licenseBytes, 0, licenseBytes.Length);
            licenseGZipStream.Close();
            licenseMemoryStream.Close();

            var sitecoreLicense = Convert.ToBase64String(licenseMemoryStream.ToArray());
            return sitecoreLicense;
        }
    }
}