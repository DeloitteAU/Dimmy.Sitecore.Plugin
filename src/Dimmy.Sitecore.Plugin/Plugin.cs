﻿using Dimmy.Cli;
using Dimmy.Cli.Commands.Project;
using Dimmy.Cli.Commands.Project.SubCommands;
using Dimmy.Sitecore.Plugin.Versions._10._0._0;
using SimpleInjector;

namespace Dimmy.Sitecore.Plugin
{
    public class Plugin : IPlugin
    {
        public void Bootstrap(Container container)
        { 
            container.RegisterDecorator(
            typeof(IProjectSubCommand), typeof(StartDecorator), Predicate);
        }

        private bool Predicate(DecoratorPredicateContext context)
        {
            return context.ImplementationType == typeof(Start);
        }
    }
}