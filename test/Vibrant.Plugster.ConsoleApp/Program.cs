using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Vibrant.Plugster.ConsoleApp
{
   public class Program
   {
      public static void Main( string[] args )
      {
         var manager = new PluginManager( true );

         IUnloadablePluginContainer<IMyPlugin> container;

         container = Plugin.Load<IMyPlugin>(
            "1",
            Path.GetFullPath( @"Plugins\v1\Vibrant.Plugster.SayHelloImpl.dll" ),
            null,
            false );
         Console.WriteLine( container.Name );
         container.Plugin.SayHello();
         container.Unload();


         //var fullPath = Path.GetFullPath( @"..\Vibrant.Plugster.SayHelloImpl\bin\Debug\net45\Vibrant.Plugster.SayHelloImpl.dll" );

         //var container = manager.LoadPlugin<IMyPlugin>( Path.GetFullPath( @"Plugins\v1\Vibrant.Plugster.SayHelloImpl.dll" ) );
         //Console.WriteLine( container.Name );
         //container.Plugin.SayHello();

         //container = manager.LoadPlugin<IMyPlugin>( Path.GetFullPath( @"Plugins\v2\Vibrant.Plugster.SayHelloImpl.dll" ) );
         //Console.WriteLine( container.Name );
         //container.Plugin.SayHello();

         //manager.UnloadPlugin<IMyPlugin>( Path.GetFullPath( @"Plugins\v1\Vibrant.Plugster.SayHelloImpl.dll" ) );

         //container = manager.LoadPlugin<IMyPlugin>( Path.GetFullPath( @"Plugins\v2\Vibrant.Plugster.SayHelloImpl.dll" ) );
         //Console.WriteLine( container.Name );
         //container.Plugin.SayHello();
      }
   }
}
