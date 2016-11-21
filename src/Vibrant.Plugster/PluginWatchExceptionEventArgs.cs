using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vibrant.Plugster
{
   public class PluginWatchExceptionEventArgs : PluginWatchEventArgs
   {
      public PluginWatchExceptionEventArgs( string fullPath, Exception exception ) : base( fullPath )
      {
         Exception = exception;
      }

      public Exception Exception { get; private set; }
   }
}
