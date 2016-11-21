using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vibrant.Plugster.Exceptions
{

   [Serializable]
   public class PluginVersionException : Exception
   {
      public PluginVersionException() { }
      public PluginVersionException( string message ) : base( message ) { }
      public PluginVersionException( string message, Exception inner ) : base( message, inner ) { }
      protected PluginVersionException(
       System.Runtime.Serialization.SerializationInfo info,
       System.Runtime.Serialization.StreamingContext context ) : base( info, context ) { }
   }
}
