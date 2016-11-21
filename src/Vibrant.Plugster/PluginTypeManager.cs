using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading.Tasks;
using Vibrant.Plugster.Exceptions;

namespace Vibrant.Plugster
{
   internal class PluginTypeManager<TPlugin> : IPluginTypeManager<TPlugin>
   {
      private readonly object _sync = new object();
      private Dictionary<string, PluginContainer<TPlugin>> _plugins;
      private PluginManager _manager;

      public PluginTypeManager( PluginManager manager )
      {
         _manager = manager;
         _plugins = new Dictionary<string, PluginContainer<TPlugin>>();
      }

      public IPluginContainer<TPlugin> LoadPlugin( string assemblyPath, AppDomainSetup setup, PermissionSet permissions )
      {
         lock( _sync )
         {
            PluginContainer<TPlugin> container;
            if( !_plugins.TryGetValue( assemblyPath, out container ) )
            {
               container = (PluginContainer<TPlugin>)Plugin.Load<TPlugin>( assemblyPath, assemblyPath, setup, permissions );

               _plugins.Add( assemblyPath, container );

               _manager.OnPluginLoaded( container );
            }

            return container;
         }
      }

      IPluginContainer IPluginTypeManager.LoadPlugin( string assemblyPath, AppDomainSetup setup, PermissionSet permissions )
      {
         return LoadPlugin( assemblyPath, setup, permissions );
      }

      public void UnloadPlugin( string assemblyPath )
      {
         lock( _sync )
         {
            PluginContainer<TPlugin> container;
            if( _plugins.TryGetValue( assemblyPath, out container ) )
            {
               AppDomain.Unload( container.Domain );

               _plugins.Remove( assemblyPath );

               _manager.OnPluginUnloaded( container );
            }
         }
      }

      public void RenamePlugin( string assemblyPath, string oldAssemblyPath )
      {
         lock( _sync )
         {
            PluginContainer<TPlugin> container;
            if( _plugins.TryGetValue( oldAssemblyPath, out container ) )
            {
               _plugins.Remove( oldAssemblyPath );
               _plugins.Add( assemblyPath, container );
            }
         }
      }

      public IEnumerable<IPluginContainer<TPlugin>> GetPlugins()
      {
         lock( _sync )
         {
            return _plugins.Values.ToList<IPluginContainer<TPlugin>>();
         }
      }

      IEnumerable<IPluginContainer> IPluginTypeManager.GetPlugins()
      {
         return GetPlugins();
      }
   }
}
