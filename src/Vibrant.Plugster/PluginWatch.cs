using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace Vibrant.Plugster
{
   public class PluginWatch
   {
      public PluginWatch( Type pluginType, string path, AppDomainSetup setup, PermissionSet permissions )
      {
         PluginType = pluginType;
         Path = path;
         Setup = setup;
         Permissions = permissions;
      }

      public Type PluginType { get; private set; }

      public string Path { get; private set; }

      public AppDomainSetup Setup { get; private set; }

      public PermissionSet Permissions { get; private set; }
   }
}
