using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vibrant.Plugster
{
   public class PluginEventArgs : EventArgs
   {
      public PluginEventArgs( IPluginContainer container )
      {
         Container = container;
      }

      public IPluginContainer Container { get; private set; }
   }
}
