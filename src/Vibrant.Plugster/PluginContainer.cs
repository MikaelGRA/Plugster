using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Vibrant.Plugster
{
   internal class PluginContainer<TPlugin> : IUnloadablePluginContainer<TPlugin>
   {
      public PluginContainer( AppDomain domain, PluginLoader loader, AssemblyName name, TPlugin plugin )
      {
         Domain = domain;
         Plugin = plugin;
         Loader = loader;
         Name = name;
      }

      public AssemblyName Name { get; private set; }

      public TPlugin Plugin { get; private set; }

      internal AppDomain Domain { get; private set; }

      internal PluginLoader Loader { get; private set; }

      object IPluginContainer.Plugin
      {
         get
         {
            return Plugin;
         }
      }

      public void Unload()
      {
         AppDomain.Unload( Domain );
      }
   }
}
