using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Vibrant.Plugster
{
   public interface IPluginContainer
   {
      AssemblyName Name { get; }

      object Plugin { get; }
   }

   public interface IUnloadablePluginContainer : IPluginContainer
   {
      void Unload();
   }

   public interface IPluginContainer<TPlugin> : IPluginContainer
   {
      new TPlugin Plugin { get; }
   }

   public interface IUnloadablePluginContainer<TPlugin> : IPluginContainer<TPlugin>, IUnloadablePluginContainer
   {
   }
}
