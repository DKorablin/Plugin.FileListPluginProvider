using System;

namespace Plugin.FileListPluginProvider.Data
{
	internal class PluginInfo
	{
		public String AssemblyName { get; }

		public String[] Instance { get; }

		public PluginInfo(String assemblyName, String[] instance)
		{
			if(String.IsNullOrEmpty(assemblyName))
				throw new ArgumentNullException(nameof(assemblyName));
			if(instance == null || instance.Length == 0)
				throw new ArgumentNullException(nameof(instance));

			this.AssemblyName = assemblyName;
			this.Instance = instance;
		}
	}
}