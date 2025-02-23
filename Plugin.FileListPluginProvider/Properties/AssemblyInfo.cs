using System.Reflection;
using System.Runtime.InteropServices;

[assembly: Guid("d505490e-747c-4d05-becd-8661b85c1711")]
[assembly: System.CLSCompliant(true)]

#if NETSTANDARD || NETCOREAPP
[assembly: AssemblyMetadata("ProjectUrl", "https://dkorablin.ru/project/Default.aspx?File=105")]
#else

[assembly: AssemblyDescription("Plugin provider using XML file to specify assemblies and instances to load")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2016-2025")]
#endif