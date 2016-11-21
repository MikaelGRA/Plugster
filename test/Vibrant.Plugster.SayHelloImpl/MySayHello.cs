using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vibrant.Plugster.ConsoleApp;

namespace Vibrant.Plugster.SayHelloImpl
{
   public class MySayHello : MarshalByRefObject, IMyPlugin
   {
      public void SayHello()
      {
         Console.WriteLine( "Hello from MySayHello v2" );

         var dir = new DirectoryInfo( AppDomain.CurrentDomain.BaseDirectory );
         var file = dir.GetFiles().First();

         var f = File.ReadAllBytes( file.FullName );
      }
   }
}
