using System.CommandLine;

namespace Dimmy.Sitecore.Plugin
{
    public interface ISitecoreInitialiseVersion
    {
        string Name { get; }
        string Description { get; }
        SitecoreInitialiseArgument HydrateCommand(Command command);
    }
}