using Dimmy.Cli.Commands;

namespace Dimmy.Sitecore.Plugin.Versions._10._0._0.Project
{
    public abstract class SitecoreProjectCommand<TSubCommandArgument> : Command<TSubCommandArgument>, ISitecoreProjectCommand
        where TSubCommandArgument : CommandArgument
    {
        
    }
}