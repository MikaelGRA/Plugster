using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace Vibrant.Plugster
{
   public interface IPluginTypeManager
   {
      IEnumerable<IPluginContainer> GetPlugins();

      IPluginContainer LoadPlugin( string assemblyPath, AppDomainSetup setup, PermissionSet permissions );

      void UnloadPlugin( string assemblyPath );

      void RenamePlugin( string assemblyPath, string oldAssemblyPath );
   }

   public interface IPluginTypeManager<TPlugin> : IPluginTypeManager
   {
      new IEnumerable<IPluginContainer<TPlugin>> GetPlugins();

      new IPluginContainer<TPlugin> LoadPlugin( string assemblyPath, AppDomainSetup setup, PermissionSet permissions );
   }
}
