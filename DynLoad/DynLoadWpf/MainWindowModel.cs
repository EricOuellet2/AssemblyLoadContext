using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using General.DynamicAssembly;
using OptimizerInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DynLoadWpf
{

    // See project for reference "Unloading" : C:\Users\cn1629\Source\Repos\samples\core\tutorials\Unloading\Unloading.sln

    public class MainWindowModel : ObservableObject
    {
        public RelayCommand RelayCommandTest { get; }

        public MainWindowModel()
        {
            RelayCommandTest = new RelayCommand(CommandTestExecute);
        }

        private void CommandTestExecute()
        {
            string currentAssemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            string path = Path.Combine(currentAssemblyDirectory, $"..\\..\\..\\..");

            path = Path.Combine(path, "Optimizer", "bin", "Debug", "net8.0", "Optimizer.dll");

            if (!File.Exists(path))
            {
                throw new InvalidOperationException("You should compile 'Optimizer' independently first. There is no dependendy defined to it from the exe.");
            }

            HostAssemblyLoadContext.ExecuteAndUnload(path, OptimizerAction, out WeakReference<HostAssemblyLoadContext> alcWeakRef);
        }

        private void OptimizerAction(Assembly asm, HostAssemblyLoadContext alc)
        {
            var types = asm.GetTypes().Where(type => type.GetInterface(nameof(IOptimizer)) != null).ToList();
            if (types.Count != 1)
            {
                return;
            }

            Type typeIOptimizer = types[0];
            MethodInfo? getInstance = typeIOptimizer.GetMethod("GetInstance", BindingFlags.Static | BindingFlags.Public);
            if (getInstance != null)
            {
                object? obj = getInstance.Invoke(null, null);
                if (obj != null)
                {
                    Debug.Assert(obj.GetType().Name == "Optimizer");
                    var iopt = obj as IOptimizer;
                    Debug.Assert(iopt != null); // ASSERTION FAIL *******

                    if (obj is IOptimizer opt) // NEVER TRUE *******
                    {
                        opt.Optimize();   // HERE Never called. Thers is no IOptimizer found when loaded from AssemblyLoadContext
                    }
                }
            }
        }
    }
}
