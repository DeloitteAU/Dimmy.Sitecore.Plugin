using System;
using System.CommandLine;
using System.Security.Cryptography.X509Certificates;
using Dimmy.Engine.Pipelines;
using Dimmy.Engine.Pipelines.InitialiseProject;
using Dimmy.Engine.Services;

namespace Dimmy.Sitecore.Plugin.Versions._10._0._1
{
    public class SitecoreInitialise : SitecoreInitialiseBase<SitecoreInitialiseArgument>
    {
        private readonly ICertificateService _certificateService;
        public override string Name => "sitecore-10.0.1";
        public override string Description => "Initialise a Sitecore 10.0.1 project.";

        protected override string Version => "10.0.1";

        public SitecoreInitialise(
            ICertificateService certificateService,
            Pipeline<Node<IInitialiseProjectContext>, IInitialiseProjectContext> initialiseProjectPipeline) :
            base(initialiseProjectPipeline)
        {
            _certificateService = certificateService;
        }

        protected override void DoHydrateCommand(Command command, SitecoreInitialiseArgument arg)
        {
            command.AddOption(new Option<string>("--cd-host-name",
                $"the host name fo the CD server. Defaults to {arg.CdHostName}"));
            command.AddOption(new Option<string>("--cm-host-name",
                $"the host name fo the CM server. Defaults to {arg.CmHostName}"));
            command.AddOption(new Option<string>("--id-host-name",
                $"the host name fo the CD server. Defaults to {arg.IdHostName}"));
            command.AddOption(new Option<string>("--traefik-image",
                $"the docker isolation for traefik. Defaults to {arg.TraefikIImage}"));
        }

        protected override void DoInitialise(SitecoreInitialiseArgument argument, InitialiseProjectContext context)
        {
            var identityCertificatePassword = NonceService.Generate();

            var identityCertificate = _certificateService.CreateSelfSignedCertificate("dimmy.sitecore.plugin", "localhost");

            var x509IdentityCertificate2Export = identityCertificate.Export(X509ContentType.Pfx, identityCertificatePassword);
            var x509IdentityCertificate2Base64String = Convert.ToBase64String(x509IdentityCertificate2Export);

            context.PrivateVariables.Add("Sitecore.AdminPassword", NonceService.Generate());
            context.PrivateVariables.Add("Sitecore.Id.Secret", NonceService.Generate());
            context.PrivateVariables.Add("Sitecore.Id.CertificatePassword", identityCertificatePassword);
            context.PrivateVariables.Add("Sitecore.Id.Certificate", x509IdentityCertificate2Base64String);
            context.PrivateVariables.Add("Sitecore.Rep.ApiKey", NonceService.Generate());
            context.PrivateVariables.Add("Sitecore.Xc.Engine.Authoring.ClientId", NonceService.Generate());

            context.PublicVariables.Add("Sitecore.Xc.GlobalTrustedConnection",
                argument.XcGlobalTrustedConnection.ToString());
            context.PublicVariables.Add("Sitecore.Xc.SharedTrustedConnection",
                argument.XcSharedTrustedConnection.ToString());

            context.PublicVariables.Add("Sitecore.Xc.Engine.GlobalDatabaseName", argument.XcEngineGlobalDatabaseName);
            context.PublicVariables.Add("Sitecore.Xc.Engine.SharedDatabaseName", argument.XcEngineSharedDatabaseName);
            
            context.PublicVariables.Add("Sitecore.Xc.Braintree.Environment", argument.XcBraintreeEnvironment);
            context.PublicVariables.Add("Sitecore.Xc.Braintree.MerchantId", argument.XcBraintreeMerchantId);
            context.PublicVariables.Add("Sitecore.Xc.Braintree.PublicKey", argument.XcBraintreePublicKey);
            context.PublicVariables.Add("Sitecore.Xc.Braintree.PrivateKey", argument.XcBraintreePrivateKey);
        }
    }
}