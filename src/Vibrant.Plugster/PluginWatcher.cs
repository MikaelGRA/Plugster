using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Vibrant.Plugster
{
   public class PluginWatcher : IDisposable
   {
      public event EventHandler<PluginWatchEventArgs> FileTimeout;
      public event EventHandler<PluginWatchExceptionEventArgs> PluginLoadFailed;
      public event EventHandler<PluginWatchExceptionEventArgs> PluginUnloadFailed;

      private readonly object _sync = new object();
      private readonly TaskFactory _taskFactory;
      private readonly PluginManager _manager;
      private readonly List<IPluginTypeWatcher> _typeWatchers;

      public PluginWatcher( PluginManager manager, IEnumerable<PluginWatch> watches, bool publishOnCurrentSyncronizationContext )
      {
         _manager = manager;

         _typeWatchers = new List<IPluginTypeWatcher>();
         foreach( var watch in watches )
         {
            var typeWatcher = new PluginTypeWatcher( _manager, this, watch, TimeSpan.FromSeconds( 5 ) );
            _typeWatchers.Add( typeWatcher );
         }

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

      public void Start()
      {
         lock( _sync )
         {
            foreach( var watcher in _typeWatchers )
            {
               watcher.Start();
            }
         }
      }

      public void Stop()
      {
         lock( _sync )
         {
            foreach( var watcher in _typeWatchers )
            {
               watcher.Stop();
            }
         }
      }

      internal async void OnFailedUnloadingPlugin( string fullPath, Exception e )
      {
         await _taskFactory.StartNew( () => PluginUnloadFailed?.Invoke( this, new PluginWatchExceptionEventArgs( fullPath, e ) ) ).ConfigureAwait( false );
      }

      internal async void OnFailedLoadingPlugin( string fullPath, Exception e )
      {
         await _taskFactory.StartNew( () => PluginLoadFailed?.Invoke( this, new PluginWatchExceptionEventArgs( fullPath, e ) ) ).ConfigureAwait( false );
      }

      internal async void OnTimedOut( string fullPath )
      {
         await _taskFactory.StartNew( () => FileTimeout?.Invoke( this, new PluginWatchEventArgs( fullPath ) ) ).ConfigureAwait( false );
      }

      #region IDisposable Support

      private bool _disposed = false; // To detect redundant calls

      protected virtual void Dispose( bool disposing )
      {
         if( !_disposed )
         {
            if( disposing )
            {
               lock( _sync )
               {
                  foreach( var watcher in _typeWatchers )
                  {
                     var disp = watcher as IDisposable;
                     if( disp != null )
                     {
                        disp.Dispose();
                     }
                  }
                  _typeWatchers.Clear();
               }
            }

            _disposed = true;
         }
      }

      // This code added to correctly implement the disposable pattern.
      public void Dispose()
      {
         Dispose( true );
      }

      #endregion
   }
}
