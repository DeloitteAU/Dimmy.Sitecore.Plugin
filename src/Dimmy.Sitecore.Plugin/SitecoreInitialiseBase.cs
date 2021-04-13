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
using SharpCompress.Compressors;
using SharpCompress.Compressors.Deflate;

namespace Dimmy.Sitecore.Plugin
{
    public abstract class SitecoreInitialiseBase<TArgument> : InitialiseSubCommand
        where TArgument : SitecoreInitialiseArgument, new()
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
            var arg = new TArgument();

            
            command.AddOption(new Option<string>(
                "--license-path", 
                "Path to the Sitecore License"));
            
            
            command.AddOption(new Option<string>(
                "--registry", 
                "registry to pull docker images from"));
            
            command.AddOption(new Option<Dictionary<string, string>>("--registries", parseArgument: result =>
            {
                if (result.Argument.Name == "registry")
                {
                    return new Dictionary<string, string>
                    {
                        {"defult", result.Tokens.First().Value}
                    };
                }
                return result
                    .Tokens
                    .Select(t => t.Value.Split('='))
                    .ToDictionary(p => p[0], p => p[1]);
            }, description: "registry to pull docker images from"));
            

            command.AddOption(new Option<string>(
                "--topology", 
                $"The Sitecore topology. Defaults to {arg.Topology}. Options: \n {string.Join('\n', Topologies)}"));
            
            DoHydrateCommand(command, arg);
            command.Handler = CommandHandler.Create((TArgument arg) => Initialise(arg));
        }

        private void Initialise(TArgument argument)
        {
            var context = new InitialiseProjectContext
            {
                PrivateVariables = new Dictionary<string, string>(),
                PublicVariables = new Dictionary<string, string>()
            };
            
            context.PrivateVariables.Add("MsSql.SaPassword", NonceService.Generate());
            context.PrivateVariables.Add("Sitecore.License", CreateEncodedSitecoreLicense(argument));
            context.PrivateVariables.Add("Sitecore.TelerikEncryptionKey", NonceService.Generate());

            context.MetaData.Add("SitecoreVersion", Version);
            context.MetaData.Add(Constants.MetaData.SitecoreTopology, argument.Topology);

            context.WorkingPath = argument.WorkingPath;
            context.SourceCodePath = argument.SourceCodePath;
            context.Name = argument.Name;
            if (!string.IsNullOrEmpty(argument.DockerComposeTemplate))
            {
                context.DockerComposeTemplatePath = argument.DockerComposeTemplate;
            }
            else
            {
                context.DockerComposeTemplatePath =
                    Path.Join(TemplatePath, $"{argument.Topology}.docker-compose.template.yml");
            }

            DoInitialise(argument, context);
            
            InitialiseProjectPipeline.Execute(context);
        }

        protected abstract void DoHydrateCommand(Command command, TArgument arg);
        protected abstract void DoInitialise(TArgument argument, InitialiseProjectContext context);
        
        protected string CreateEncodedSitecoreLicense(TArgument si)
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