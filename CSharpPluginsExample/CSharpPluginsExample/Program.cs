using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using PluginInterface;
// ReSharper disable InvertIf
// ReSharper disable LoopCanBeConvertedToQuery

namespace CSharpPluginsExample
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += ReflectionOnlyAssemblyResolve;

            var pluginsFolder = Directory.GetCurrentDirectory() + ConfigurationManager.AppSettings.Get("pluginsFolder");
            var assemblyPaths = Directory.GetFiles(pluginsFolder, "*.dll").ToList();

            var appdomain = AppDomain.CreateDomain("plugins container");
            var myPlugins = new List<IMyPlugin>();
            
            foreach (var assembly in assemblyPaths.Select(Assembly.ReflectionOnlyLoadFrom))
            {
                foreach (var type in assembly.DefinedTypes)
                {
                    if (IsValidType(type))
                    {
                        var p = appdomain.CreateInstanceFromAndUnwrap(assembly.Location, type.FullName) as IMyPlugin;
                        myPlugins.Add(p);
                    }
                }
            }

            myPlugins.ForEach(i => Console.WriteLine(i.Execute(() => Assembly.GetCallingAssembly().GetName().Name)));

            AppDomain.Unload(appdomain);

            Console.ReadKey();
        }

        private static bool IsValidType(TypeInfo type)
        {
            return type.IsClass &&
                   !type.IsAbstract &&
                   type.ImplementedInterfaces.Any(i => i.GUID == typeof (IMyPlugin).GUID) &&
                   type.BaseType == typeof (MarshalByRefObject);
        }

        private static Assembly ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var strTempAssmbPath = "";

            var objExecutingAssemblies = Assembly.GetExecutingAssembly();
            var arrReferencedAssmbNames = objExecutingAssemblies.GetReferencedAssemblies();

            if (arrReferencedAssmbNames.Any(
                strAssmbName =>
                    strAssmbName.FullName.Substring(0, strAssmbName.FullName.IndexOf(",", StringComparison.Ordinal)) ==
                    args.Name.Substring(0, args.Name.IndexOf(",", StringComparison.Ordinal))))
            {
                strTempAssmbPath = Path.GetDirectoryName(objExecutingAssemblies.Location) + "\\" +
                                   args.Name.Substring(0, args.Name.IndexOf(",", StringComparison.Ordinal)) + ".dll";
            }
            var myAssembly = Assembly.ReflectionOnlyLoadFrom(strTempAssmbPath);

            return myAssembly;
        }
    }
}