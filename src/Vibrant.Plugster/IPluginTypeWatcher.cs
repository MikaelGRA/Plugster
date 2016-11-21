using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vibrant.Plugster
{
   public interface IPluginTypeWatcher
   {
      void Start();

      void Stop();
   }
}
