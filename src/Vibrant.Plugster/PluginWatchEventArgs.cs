using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vibrant.Plugster
{
   public class PluginWatchEventArgs : EventArgs
   {
      public PluginWatchEventArgs( string fullPath )
      {
         FullPath = fullPath;
      }

      public string FullPath { get; private set; }
   }
}
