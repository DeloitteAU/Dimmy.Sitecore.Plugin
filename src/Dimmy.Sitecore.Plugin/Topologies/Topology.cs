using System;
using System.Collections.Generic;
using System.IO;

namespace Dimmy.Sitecore.Plugin.Topologies
{
    public abstract class Topology: ITopology
    {
        protected abstract string DockerComposeTemplateManifestResourceName { get; }
        public abstract string Name { get; }

        public string DockerComposeTemplate
        {
            get
            {
                if (string.IsNullOrEmpty(DockerComposeTemplateManifestResourceName))
                {
                    throw new InvalidOperationException("Could not load manifest resource stream.");
                }

                using var stream = GetType().Assembly.GetManifestResourceStream(DockerComposeTemplateManifestResourceName);

                if (stream == null)
                {
                    throw new InvalidOperationException("Could not load manifest resource stream.");
                }
                using var reader = new StreamReader(stream);

                return reader.ReadToEnd();
                
            }
        }
        public abstract string DockerComposeTemplateName { get; }
        public Dictionary<string, string> VariableDictionary { get; }
    }
}