using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vibrant.Plugster.Exceptions
{

   [Serializable]
   public class PluginLoadException : Exception
   {
      public PluginLoadException() { }
      public PluginLoadException( string message ) : base( message ) { }
      public PluginLoadException( string message, System.Exception inner ) : base( message, inner ) { }
      protected PluginLoadException(
       System.Runtime.Serialization.SerializationInfo info,
       System.Runtime.Serialization.StreamingContext context ) : base( info, context ) { }
   }
}
