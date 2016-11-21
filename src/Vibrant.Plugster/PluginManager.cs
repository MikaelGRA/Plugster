using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Vibrant.Plugster.Exceptions;

namespace Vibrant.Plugster
{
   public class PluginManager
   {
      public event EventHandler<PluginEventArgs> PluginLoaded;
      public event EventHandler<PluginEventArgs> PluginUnloaded;

      private readonly object _sync = new object();
      private readonly TaskFactory _taskFactory;
      private readonly Dictionary<Type, IPluginTypeManager> _managers;

      public PluginManager( bool publishOnCurrentSyncronizationContext )
      {
         _managers = new Dictionary<Type, IPluginTypeManager>();

         if( publishOnCurrentSyncronizationContext )
         {
            if( SynchronizationContext.Current != null )
            {
               _taskFactory = new TaskFactory( TaskScheduler.FromCurrentSynchronizationContext() );
            }
            else
            {
               _taskFactory = new TaskFactory( TaskScheduler.Default );
            }
         }
         else
         {
            _taskFactory = new TaskFactory( TaskScheduler.Default );
         }
      }

      public List<IPluginContainer> GetAllLoadedPlugins()
      {
         lock( _sync )
         {
            return _managers.Values.SelectMany( x => x.GetPlugins() ).ToList();
         }
      }

      public List<IPluginContainer> GetAllLoadedPluginsOf( Type pluginType )
      {
         return FindManagerOf( pluginType ).GetPlugins().ToList();
      }

      public List<IPluginContainer<TPlugin>> GetAllLoadedPluginsOf<TPlugin>()
      {
         return FindManagerOf<TPlugin>().GetPlugins().ToList();
      }

      public IPluginContainer<TPlugin> LoadPlugin<TPlugin>( string assemblyFullPath, AppDomainSetup setup, PermissionSet permissions )
      {
         return (IPluginContainer<TPlugin>)LoadPlugin( typeof( TPlugin ), assemblyFullPath, setup, permissions );
      }

      public IPluginContainer LoadPlugin( Type pluginType, string assemblyFullPath, AppDomainSetup setup, PermissionSet permissions )
      {
         // Find the correct managear
         var manager = FindManagerOf( pluginType );

         // Get/load the plugin
         var container = manager.LoadPlugin( assemblyFullPath, setup, permissions );

         return container;
      }

      public void UnloadPlugin<TPlugin>( string assemblyFullPath )
      {
         UnloadPlugin( typeof( TPlugin ), assemblyFullPath );
      }

      public void UnloadPlugin( Type pluginType, string assemblyFullPath )
      {
         var manager = FindManagerOf( pluginType );

         manager.UnloadPlugin( assemblyFullPath );
      }

      internal void RenamePlugin( Type pluginType, string assemblyFullPath, string oldAssemblyFullPath )
      {
         var manager = FindManagerOf( pluginType );
      }

      private IPluginTypeManager<TPlugin> FindManagerOf<TPlugin>()
      {
         return (IPluginTypeManager<TPlugin>)FindManagerOf( typeof( TPlugin ) );
      }

      private IPluginTypeManager FindManagerOf( Type pluginType )
      {
         lock( _sync )
         {
            IPluginTypeManager manager;
            if( !_managers.TryGetValue( pluginType, out manager ) )
            {
               manager = (IPluginTypeManager)typeof( PluginTypeManager<> )
                  .MakeGenericType( new[] { pluginType } )
                  .GetConstructor( new[] { typeof( PluginManager ) } )
                  .Invoke( new object[] { this } );

               _managers.Add( pluginType, manager );
            }
            return manager;
         }
      }

      internal async void OnPluginLoaded( IPluginContainer container )
      {
         await _taskFactory.StartNew( () => PluginLoaded?.Invoke( this, new PluginEventArgs( container ) ) ).ConfigureAwait( false );
      }

      internal async void OnPluginUnloaded( IPluginContainer container )
      {
         await _taskFactory.StartNew( () => PluginUnloaded?.Invoke( this, new PluginEventArgs( container ) ) ).ConfigureAwait( false );
      }
   }
}
