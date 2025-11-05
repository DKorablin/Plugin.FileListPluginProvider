using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Plugin.FileListPluginProvider.Data;
using Plugin.FilePluginProvider;
using SAL.Flatbed;

namespace Plugin.FileListPluginProvider
{
	public class Plugin : IPluginProvider
	{
		private TraceSource _trace;

		#region Properties
		private TraceSource Trace { get => this._trace ?? (this._trace = Plugin.CreateTraceSource<Plugin>()); }

		/// <summary>Arguments transferred to the main application</summary>
		private FilePluginArgs Args { get; set; }

		private IHost Host { get; }

		/// <summary>Plugins list monitoring</summary>
		private List<FileSystemWatcher> Monitors { get; set; }

		IPluginProvider IPluginProvider.ParentProvider { get; set; }
		#endregion Properties

		public Plugin(IHost host)
			=> this.Host = host ?? throw new ArgumentNullException(nameof(host));

		Boolean IPlugin.OnConnection(ConnectMode mode)
		{
			this.Args = new FilePluginArgs();
			this.Monitors = new List<FileSystemWatcher>();
			return true;
		}

		Boolean IPlugin.OnDisconnection(DisconnectMode mode)
		{
			if(mode == DisconnectMode.UserClosed)
				throw new NotSupportedException("Plugin Provider can't be unloaded");
			else
			{
				if(this.Monitors != null)//TODO: Plugin.Dialog - 2 times Unload is invoked...
					foreach(FileSystemWatcher monitor in this.Monitors)
						monitor.Dispose();
				this.Monitors = null;
				return true;
			}
		}

		void IPluginProvider.LoadPlugins()
		{
			foreach(String pluginPath in this.Args.PluginPath)
				if(Directory.Exists(pluginPath))
				{
					String xmlFilePath = Path.Combine(pluginPath, Constant.ListFileName);
					if(File.Exists(xmlFilePath))
					{
						this.AnalyzeXmlFile(pluginPath, ConnectMode.Startup);
						this.AddMonitor(pluginPath);
					} else
						this.Trace.TraceInformation("FileList {0} not found in target directory", xmlFilePath);
				}
		}

		Assembly IPluginProvider.ResolveAssembly(String assemblyName)
		{
			if(String.IsNullOrEmpty(assemblyName))
				throw new ArgumentNullException(nameof(assemblyName));

			AssemblyName targetName = new AssemblyName(assemblyName);
			foreach(String pluginPath in this.Args.PluginPath)
				if(Directory.Exists(pluginPath))
					foreach(String file in Directory.GetFiles(pluginPath, "*.*", SearchOption.AllDirectories))
						if(FilePluginArgs.CheckFileExtension(file)) //Searching only files with .dll extension (UPD: Added logic to unify plugins extensions)
							try
							{
								AssemblyName name = AssemblyName.GetAssemblyName(file);
								if(name.FullName == targetName.FullName)
									return Assembly.LoadFile(file);
								//return assembly;//TODO: Reference DLL can't be loaded from memory!
							} catch(BadImageFormatException)
							{
								// Ignore invalid libraries
							} catch(FileLoadException)
							{
								// Ignore invalid libraries
							} catch(Exception exc)
							{
								exc.Data.Add("Library", file);
								this.Trace.TraceData(TraceEventType.Error, 1, exc);
							}

			this.Trace.TraceEvent(TraceEventType.Warning, 5, "The provider {2} is unable to locate the assembly {0} in the path {1}", assemblyName, String.Join(",", this.Args.PluginPath), this.GetType());
			IPluginProvider parentProvider = ((IPluginProvider)this).ParentProvider;
			return parentProvider?.ResolveAssembly(assemblyName);
		}

		public void RebuildXmlFile()
		{
			foreach(String pluginPath in this.Args.PluginPath)
			{
				String fileName = Plugin.GetUniqueFileName(pluginPath, Constant.ListFileName, 0);

				List<PluginInfo> items = new List<PluginInfo>();
				foreach(IPluginDescription plugin in this.Host.Plugins)
				{
					if(plugin.Source.StartsWith(pluginPath, StringComparison.OrdinalIgnoreCase))
					{
						String assemblyName = Path.GetFileName(plugin.Source);
						String fullClassName = plugin.Instance.GetType().FullName;
						items.Add(new PluginInfo(assemblyName, new String[] { fullClassName, }));
					}
				}

				if(items.Count > 0)
				{
					XmlFile file = new XmlFile(Path.Combine(pluginPath, fileName));
					file.SavePluginInfo(items.ToArray());
				}
			}
		}

		private void AnalyzeXmlFile(String pluginPath, ConnectMode mode)
		{
			XmlFile file = new XmlFile(Path.Combine(pluginPath, Constant.ListFileName));
			foreach(PluginInfo info in file.ReadPluginInfo())
			{
				String assemblyPath = Path.Combine(pluginPath, info.AssemblyName);
				try
				{
					Boolean loaded = false;
					// Check that the plugin with this source hasn't yet been loaded if it's already loaded by the parent provider.
					// Loading from the file system, so the source must be unique.
					foreach(IPluginDescription item in this.Host.Plugins)
						if(assemblyPath.Equals(item.Source, StringComparison.OrdinalIgnoreCase))
						{
							loaded = true;
							break;
						}

					if(!loaded)
					{
						Assembly assembly = Assembly.LoadFile(assemblyPath);

						foreach(String type in info.Instance)//TODO: Need to check ID of each instance
							this.Host.Plugins.LoadPlugin(assembly, type, assemblyPath, mode);
					}
				} catch(BadImageFormatException exc)//Plugin loading error. I could read the title of the file being loaded, but I'm too lazy.
				{
					exc.Data.Add("Library", assemblyPath);
					this.Trace.TraceData(TraceEventType.Error, 1, exc);
				} catch(Exception exc)
				{
					exc.Data.Add("Library", assemblyPath);
					this.Trace.TraceData(TraceEventType.Error, 1, exc);
				}
			}
		}

		private void AddMonitor(String pluginPath)
		{
			FileSystemWatcher monitor = new FileSystemWatcher(pluginPath, Constant.ListFileName);
			monitor.Changed += (sender, args) => this.Trace.TraceInformation("File {0} {1}", args.FullPath, args.ChangeType);
			monitor.EnableRaisingEvents = true;
			this.Monitors.Add(monitor);
		}

		/// <summary>Get a unique file name that is unique to the file system.</summary>
		/// <param name="path">Path to the folder in which to search for the file.</param>
		/// <param name="fileName">File name.</param>
		/// <param name="index">File index, which will be substituted for the file name.</param>
		/// <returns>A unique file name in the <paramref name="path"/> folder.</returns>
		private static String GetUniqueFileName(String path, String fileName, Int32 index)
		{
			String file = index > 0
				? $"{Path.GetFileNameWithoutExtension(fileName)}({index}){Path.GetExtension(fileName)}"
				: fileName;

			return File.Exists(Path.Combine(path, file))
				? Plugin.GetUniqueFileName(path, fileName, ++index)
				: file;
		}

		private static TraceSource CreateTraceSource<T>(String name = null) where T : IPlugin
		{
			TraceSource result = new TraceSource(typeof(T).Assembly.GetName().Name + name);
			result.Switch.Level = SourceLevels.All;
			result.Listeners.Remove("Default");
			result.Listeners.AddRange(System.Diagnostics.Trace.Listeners);
			return result;
		}
	}
}