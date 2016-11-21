using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Vibrant.Plugster
{
   public static class Plugin
   {
      public static IUnloadablePluginContainer<TPlugin> Load<TPlugin>(
         string domainName,
         string assemblyFullPath,
         string applicationBase,
         bool isTrusted )
      {
         var setup = new AppDomainSetup();
         setup.ApplicationBase = applicationBase ?? AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

         PermissionSet permissions = null;
         if( isTrusted )
         {
            permissions = new PermissionSet( PermissionState.Unrestricted );
         }
         else
         {
            permissions = new PermissionSet( PermissionState.None );
            permissions.AddPermission(
                new SecurityPermission(
                    SecurityPermissionFlag.Execution ) );
            permissions.AddPermission(
                new FileIOPermission(
                    FileIOPermissionAccess.AllAccess, setup.ApplicationBase ) );
         }

         return Load<TPlugin>( domainName, assemblyFullPath, setup, permissions );
      }

      public static IUnloadablePluginContainer<TPlugin> Load<TPlugin>(
         string domainName,
         string assemblyFullPath,
         AppDomainSetup setup,
         PermissionSet permissions )
      {
         StrongName fullTrustAssembly = typeof( PluginLoader ).Assembly.Evidence.GetHostEvidence<StrongName>();

         AppDomain domain = AppDomain.CreateDomain(
            domainName,
            null,
            setup,
            permissions,
            fullTrustAssembly );

         string assemblyPath = typeof( PluginLoader ).Assembly.ManifestModule.FullyQualifiedName;
         PluginLoader loader = (PluginLoader)Activator.CreateInstanceFrom( domain, assemblyPath, typeof( PluginLoader ).FullName ).Unwrap();
         
         TPlugin instance = (TPlugin)loader.LoadPlugin( assemblyFullPath, typeof( TPlugin ).AssemblyQualifiedName );
         AssemblyName name = loader.GetPluginAssemblyName();

         var container = new PluginContainer<TPlugin>( domain, loader, name, instance );

         return container;
      }
   }
}
