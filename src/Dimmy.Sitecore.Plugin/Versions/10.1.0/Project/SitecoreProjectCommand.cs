using Dimmy.Cli.Commands;

namespace Dimmy.Sitecore.Plugin.Versions._10._1._0.Project
{
    public abstract class SitecoreProjectCommand<TSubCommandArgument> : Command<TSubCommandArgument>, ISitecoreProjectCommand
        where TSubCommandArgument : CommandArgument
    {
        
    }
}