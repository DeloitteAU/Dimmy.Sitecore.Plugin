﻿using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Dimmy.Cli.Commands.Project.SubCommands;
using Dimmy.Engine.Commands;
using Dimmy.Engine.Commands.Project;
using SharpCompress.Compressors;
using SharpCompress.Compressors.Deflate;

namespace Dimmy.Sitecore.Plugin
{
    public abstract class SitecoreInitialiseBase<TArg> : InitialiseSubCommand
        where TArg : SitecoreInitialiseArgument, new()
    {
        protected readonly ICommandHandler<InitialiseProject> InitialiseProjectCommandHandler;
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
            ICommandHandler<InitialiseProject> initialiseProjectCommandHandler)
        {
            InitialiseProjectCommandHandler = initialiseProjectCommandHandler;
        }

        public override void HydrateCommand(Command command)
        {
            var arg = new TArg();
            
            command.AddOption(new Option<string>("--license-path", "Path to the Sitecore License"));
            command.AddOption(new Option<string>("--registry", $"Defaults to {arg.Registry}"));
            command.AddOption(new Option<string>("--topology", $"The Sitecore topology. Defaults to {arg.Topology}. Options: \n {string.Join('\n', Topologies)}"));
            DoHydrateCommand(command, arg);
        }

        protected abstract void DoHydrateCommand(Command command, TArg arg);

        protected string CreateEncodedSitecoreLicense(TArg si)
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
        
        protected string CreateEncodedCertificate(string password)
        {
            const string certificateName = "dimmy.sitecore.plugin";
            
            var sanBuilder = new SubjectAlternativeNameBuilder();
            sanBuilder.AddIpAddress(IPAddress.Loopback);
            sanBuilder.AddIpAddress(IPAddress.IPv6Loopback);
            sanBuilder.AddDnsName("localhost");
            sanBuilder.AddDnsName(Environment.MachineName);

            var distinguishedName = new X500DistinguishedName($"CN={certificateName}");

            using var rsa = RSA.Create(2048);

            var request = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);

            request.CertificateExtensions.Add(
                new X509KeyUsageExtension(
                    X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment |
                    X509KeyUsageFlags.DigitalSignature, false));
            
            request.CertificateExtensions.Add(
                new X509EnhancedKeyUsageExtension(
                    new OidCollection {new Oid("1.3.6.1.5.5.7.3.1")}, false));

            request.CertificateExtensions.Add(sanBuilder.Build());

            var certificate = request.CreateSelfSigned(new DateTimeOffset(DateTime.UtcNow.AddDays(-1)),
                new DateTimeOffset(DateTime.UtcNow.AddDays(3650)));
            certificate.FriendlyName = certificateName;

            var x509Certificate2Export = certificate.Export(X509ContentType.Pfx, password);
            var x509Certificate2Base64String = Convert.ToBase64String(x509Certificate2Export);

            return x509Certificate2Base64String;
        }
    }
}