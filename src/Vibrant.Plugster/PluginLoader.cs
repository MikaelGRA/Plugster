using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Threading.Tasks;
using Vibrant.Plugster.Exceptions;

namespace Vibrant.Plugster
{
   [PermissionSet( SecurityAction.Assert, Unrestricted = true )]
   internal class PluginLoader : MarshalByRefObject
   {
      private Assembly _assembly;

      public object LoadPlugin( string assemblyPath, string pluginBaseName )
      {
         _assembly = Assembly.LoadFile( assemblyPath );
         Type baseType = Type.GetType( pluginBaseName );
         Type marshalByRef = typeof( MarshalByRefObject );

         foreach( Type type in _assembly.GetTypes() )
         {
            if( !type.IsAbstract && baseType.IsAssignableFrom( type ) )
            {
               // ensure we marshal it by reference
               if( !marshalByRef.IsAssignableFrom( type ) )
               {
                  throw new PluginLoadException( "The implementing plugin MUST extend MarshalByRefObject." );
               }

               var ctor = type.GetConstructor( new Type[ 0 ] );
               if( ctor == null )
               {
                  throw new PluginLoadException( "The implementing plugin MUST define a default constructor." );
               }

               object instance = ctor.Invoke( new object[ 0 ] );

               return instance;
            }
         }

         throw new PluginLoadException( "Could not find any plugins in the specified assembly." );
      }

      public AssemblyName GetPluginAssemblyName()
      {
         return _assembly.GetName();
      }
   }
}
