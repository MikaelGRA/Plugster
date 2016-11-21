using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Vibrant.Plugster
{
   internal class PluginTypeWatcher : IPluginTypeWatcher, IDisposable
   {
      private const int ERROR_SHARING_VIOLATION = 32;
      private const int ERROR_LOCK_VIOLATION = 33;

      private readonly object _sync = new object();
      private PluginManager _manager;
      private PluginWatcher _watcher;
      private FileSystemWatcher _fileWatcher;
      private string _path;
      private Type _pluginType;
      private TimeSpan _timeout;
      private PluginWatch _watch;

      public PluginTypeWatcher( PluginManager manager, PluginWatcher watcher, PluginWatch watch, TimeSpan timeout )
      {
         _watcher = watcher;
         _manager = manager;
         _timeout = timeout;
         _watch = watch;
      }

      public void Start()
      {
         lock( _sync )
         {
            if( _fileWatcher == null )
            {
               if( !Directory.Exists( _path ) )
               {
                  Directory.CreateDirectory( _path );
               }

               _fileWatcher = new FileSystemWatcher( _path, "*.dll" );

               _fileWatcher.Created += Watcher_Created;
               _fileWatcher.Changed += Watcher_Changed;
               _fileWatcher.Deleted += Watcher_Deleted;
               _fileWatcher.Renamed += Watcher_Renamed;
            }

            LoadPluginsInDirectory();

            _fileWatcher.EnableRaisingEvents = true;
         }
      }

      public void Stop()
      {
         lock( _sync )
         {
            if( _fileWatcher != null )
            {
               _fileWatcher.EnableRaisingEvents = false;
            }
         }
      }

      private void LoadPluginsInDirectory()
      {
         var files = Directory.GetFiles( _path, "*.dll", SearchOption.TopDirectoryOnly );
         foreach( var file in files )
         {
            try
            {
               _manager.LoadPlugin( _watch.PluginType, file, _watch.Setup, _watch.Permissions );
            }
            catch( Exception ex )
            {
               _watcher.OnFailedLoadingPlugin( file, ex );
            }
         }
      }

      private void Watcher_Renamed( object sender, RenamedEventArgs e )
      {
         _manager.RenamePlugin( _pluginType, e.OldFullPath, e.FullPath );
      }

      private void Watcher_Deleted( object sender, FileSystemEventArgs e )
      {
         try
         {
            // unload previous plugin
            _manager.UnloadPlugin( _pluginType, e.FullPath );
         }
         catch( Exception ex )
         {
            _watcher.OnFailedUnloadingPlugin( e.FullPath, ex );
         }
      }

      private void Watcher_Changed( object sender, FileSystemEventArgs e )
      {
         var isLocked = WaitForFileUnlock( e.FullPath );
         if( isLocked )
         {
            _watcher.OnTimedOut( e.FullPath );
            return;
         }

         try
         {
            // unload previous plugin
            _manager.UnloadPlugin( _pluginType, e.FullPath );
         }
         catch( Exception ex )
         {
            _watcher.OnFailedUnloadingPlugin( e.FullPath, ex );
         }

         try
         {
            // load new plugin
            _manager.LoadPlugin( _watch.PluginType, e.FullPath, _watch.Setup, _watch.Permissions );
         }
         catch( Exception ex )
         {
            _watcher.OnFailedLoadingPlugin( e.FullPath, ex );
         }
      }

      private void Watcher_Created( object sender, FileSystemEventArgs e )
      {
         var isLocked = WaitForFileUnlock( e.FullPath );
         if( isLocked )
         {
            _watcher.OnTimedOut( e.FullPath );
            return;
         }

         try
         {
            // load new plugin
            _manager.LoadPlugin( _watch.PluginType, e.FullPath, _watch.Setup, _watch.Permissions );
         }
         catch( Exception ex )
         {
            _watcher.OnFailedLoadingPlugin( e.FullPath, ex );
         }
      }

      private bool WaitForFileUnlock( string path )
      {
         var sw = Stopwatch.StartNew();
         var isLocked = true;
         while( sw.Elapsed < _timeout && isLocked )
         {
            isLocked = IsFileLocked( path );
            Thread.Sleep( 500 );
         }

         return isLocked;
      }

      private bool IsFileLocked( string file )
      {
         // http://stackoverflow.com/questions/10982104/wait-until-file-is-completely-written

         // check that problem is not in destination file
         if( File.Exists( file ) == true )
         {
            FileStream stream = null;
            try
            {
               stream = File.Open( file, FileMode.Open, FileAccess.ReadWrite, FileShare.None );
            }
            catch( Exception ex2 )
            {
               //_log.WriteLog(ex2, "Error in checking whether file is locked " + file);
               int errorCode = Marshal.GetHRForException( ex2 ) & ( ( 1 << 16 ) - 1 );
               if( ( ex2 is IOException ) && ( errorCode == ERROR_SHARING_VIOLATION || errorCode == ERROR_LOCK_VIOLATION ) )
               {
                  return true;
               }
            }
            finally
            {
               if( stream != null )
                  stream.Close();
            }
         }
         return false;
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
                  if( _fileWatcher != null )
                  {
                     _fileWatcher.Dispose();
                  }
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
