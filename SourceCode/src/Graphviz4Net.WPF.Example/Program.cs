#define NET462


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Graphviz4Net.WPF.Example
{
    public class Program
    {
        [STAThread]
        public static void Main( string[] args)
        {
            SetEntryAssembly();
            App.Main();
        }

        /// <summary>
        /// Use as first line in ad hoc tests (needed by XNA specifically)
        /// </summary>
        public static void SetEntryAssembly()
        {
#if NET462
            SetEntryAssembly(System.Reflection.Assembly.GetCallingAssembly());
#endif
        }

        /// <summary>
        /// Allows setting the Entry Assembly when needed.
        /// Use AssemblyUtilities.SetEntryAssembly() as first line in XNA ad hoc tests
        /// </summary>
        /// <param name="assembly">Assembly to set as entry assembly</param>
        public static void SetEntryAssembly(System.Reflection.Assembly assembly)
        {
#if NET462
            AppDomainManager manager = new AppDomainManager();
            FieldInfo entryAssemblyfield = manager.GetType().GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
            entryAssemblyfield.SetValue(manager, assembly);

            AppDomain domain = AppDomain.CurrentDomain;
            FieldInfo domainManagerField = domain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
            domainManagerField.SetValue(domain, manager);
#endif
        }

    }
}
