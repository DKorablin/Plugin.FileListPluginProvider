using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: Guid("d505490e-747c-4d05-becd-8661b85c1711")]
[assembly: System.CLSCompliant(true)]

#if NETSTANDARD
[assembly: AssemblyMetadata("ProjectUrl", "https://dkorablin.ru/project/Default.aspx?File=105")]
#else

[assembly: AssemblyTitle("Plugin.FileListPluginProvider")]
[assembly: AssemblyDescription("Plugin provider using XML file to specify assemblies and instances to load")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("Danila Korablin")]
[assembly: AssemblyProduct("Plugin loader from file system by list")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2016-2020")]
#endif