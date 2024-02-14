using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace General.DynamicAssembly
{
    // This is a collectible (unloadable) AssemblyLoadContext that loads the dependencies
    // of the plugin from the plugin's binary directory.
    public class HostAssemblyLoadContext : AssemblyLoadContext
    {
        // Resolver of the locations of the assemblies that are dependencies of the
        // main plugin assembly.
        private AssemblyDependencyResolver _resolver;

        public HostAssemblyLoadContext(string pluginPath) : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        // The Load method override causes all the dependencies present in the plugin's binary directory to get loaded
        // into the HostAssemblyLoadContext together with the plugin assembly itself.
        // NOTE: The Interface assembly must not be present in the plugin's binary directory, otherwise we would
        // end up with the assembly being loaded twice. Once in the default context and once in the HostAssemblyLoadContext.
        // The types present on the host and plugin side would then not match even though they would have the same names.
        protected override Assembly? Load(AssemblyName name)
        {
            string? assemblyPath = _resolver.ResolveAssemblyToPath(name);
            if (assemblyPath != null)
            {
                Console.WriteLine($"Loading assembly {assemblyPath} into the HostAssemblyLoadContext");
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        // EO: Next code is mostly for reference ONLY

        // It is important to mark this method as NoInlining, otherwise the JIT could decide
        // to inline it into the Main method. That could then prevent successful unloading
        // of the plugin because some of the MethodInfo / Type / Plugin.Interface / HostAssemblyLoadContext
        // instances may get lifetime extended beyond the point when the plugin is expected to be
        // unloaded.
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ExecuteAndUnload(string assemblyPath, Action<Assembly, HostAssemblyLoadContext> ActionToExecute, out WeakReference<HostAssemblyLoadContext> alcWeakRef)
        {
            // Create the unloadable HostAssemblyLoadContext
            var alc = new HostAssemblyLoadContext(assemblyPath);

            // Create a weak reference to the AssemblyLoadContext that will allow us to detect
            // when the unload completes.
            alcWeakRef = new WeakReference<HostAssemblyLoadContext>(alc);

            // Load the plugin assembly into the HostAssemblyLoadContext.
            // NOTE: the assemblyPath must be an absolute path.
            Assembly a = alc.LoadFromAssemblyPath(assemblyPath);

            // Get the plugin interface by calling the PluginClass.GetInterface method via reflection.
            ActionToExecute(a, alc);

    //        Type pluginType = a.GetType("Plugin.PluginClass");
    //        MethodInfo getInterface = pluginType.GetMethod("GetInterface", BindingFlags.Static | BindingFlags.Public);


            //       Plugin.Interface plugin = (Plugin.Interface)getInterface.Invoke(null, null);

            // Now we can call methods of the plugin using the interface
            //       string result = plugin.GetMessage();
            //       Plugin.Version version = plugin.GetVersion();

            //       Console.WriteLine($"Response from the plugin: GetVersion(): {version}, GetMessage(): {result}");

            // This initiates the unload of the HostAssemblyLoadContext. The actual unloading doesn't happen
            // right away, GC has to kick in later to collect all the stuff.
            alc.Unload();
        }

    }
}
