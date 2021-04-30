using Dimmy.Cli;
using SimpleInjector;

namespace Dimmy.Sitecore.Plugin
{
    public class Plugin : IPlugin
    {
        public void Bootstrap(Container container)
        {
            container.Collection.Register<Versions._10._1._0.Project.ISitecoreProjectCommand>(GetType().Assembly);
        }
    }
}