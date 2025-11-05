# File List Plugin Provider

[![Build, Release](https://github.com/DKorablin/Plugin.FileListPluginProvider/actions/workflows/release.yml/badge.svg)](https://github.com/DKorablin/Plugin.FileListPluginProvider/actions/workflows/release.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A SAL (Simple Application Loader) plugin provider that loads plugins from the file system based on an XML configuration file. This provider is ideal when you need to specify an exact list of plugins to load or when your application requires a fixed set of plugins.

## Features

- 🎯 **Declarative Plugin Loading** - Define plugins in a simple XML file
- 🔧 **Configurable Plugin Paths** - Support for multiple plugin directories
- 📦 **Multi-Framework Support** - Targets .NET Framework 3.5 and .NET Standard 2.0
- 🔌 **SAL Integration** - Seamless integration with the SAL plugin system
- ⚙️ **Flexible Configuration** - Configure via app.config or command-line arguments

## Installation
To install the File List Plugin Provider Plugin, follow these steps:
1. Download the latest release from the [Releases](https://github.com/DKorablin/Plugin.Winlogon/releases)
2. Extract the downloaded ZIP file to a desired location.
3. Use the provided [Flatbed.Dialog (Lite)](https://dkorablin.github.io/Flatbed-Dialog-Lite) executable or download one of the supported host applications:
	- [Flatbed.Dialog](https://dkorablin.github.io/Flatbed-Dialog)
	- [Flatbed.MDI](https://dkorablin.github.io/Flatbed-MDI)
	- [Flatbed.MDI (WPF)](https://dkorablin.github.io/Flatbed-MDI-Avalon)
	- [Flatbed.WorkerService](https://dkorablin.github.io/Flatbed-WorkerService)

## Usage

### Basic Setup

1. **Create a Plugin List XML File**

   Create a file named `Plugins.List.xml` in your plugin directory:

   ```xml
   <?xml version="1.0" encoding="utf-8" standalone="no"?>
   <Plugins>
       <Plugin Assembly="MyPlugin.dll">
           <Instance>MyNamespace.MyPlugin</Instance>
       </Plugin>
       <Plugin Assembly="AnotherPlugin.dll">
           <Instance>AnotherNamespace.Plugin1</Instance>
           <Instance>AnotherNamespace.Plugin2</Instance>
       </Plugin>
   </Plugins>
   ```

2. **Configure Plugin Paths**

   You can specify plugin paths using one of the following methods:

   **Option A: App.config / Web.config**
   ```xml
   <configuration>
       <appSettings>
           <add key="SAL_Path" value="C:\Plugins|C:\MorePlugins" />
       </appSettings>
   </configuration>
   ```

   **Option B: Command-line Arguments**
   ```bash
   MyApp.exe /SAL_Path:C:\Plugins|C:\MorePlugins
   ```

   > **Note:** Multiple paths should be separated by the pipe character `|`

3. **Use the Plugin Provider**

   The provider will automatically scan all specified directories for `Plugins.List.xml` files and load the plugins defined within.

## XML Configuration Reference

### File Structure

The `Plugins.List.xml` file must have the following structure:

```xml
<?xml version="1.0" encoding="utf-8" standalone="no"?>
<Plugins>
    <!-- One or more Plugin elements -->
    <Plugin Assembly="assembly-file-name.dll">
        <Instance>Fully.Qualified.ClassName</Instance>
        <!-- Additional instances from the same assembly -->
    </Plugin>
</Plugins>
```

### Elements

#### `<Plugins>` (Root Element)
- **Required:** Yes
- **Description:** The root container for all plugin definitions

#### `<Plugin>` Element
- **Required:** At least one
- **Attributes:**
  - `Assembly` (**required**): The file name with extension of the assembly containing the plugin(s)
- **Child Elements:** One or more `<Instance>` elements

#### `<Instance>` Element
- **Required:** At least one per `<Plugin>`
- **Content:** The fully qualified class name (including namespace) that implements the `IPlugin` interface
- **Example:** `MyCompany.MyProduct.MyPluginClass`

### Example Configuration

```xml
<?xml version="1.0" encoding="utf-8" standalone="no"?>
<Plugins>
    <Plugin Assembly="Kernel.Empty.dll">
        <Instance>Kernel.Empty.PluginWindows</Instance>
    </Plugin>
    <Plugin Assembly="Plugin.Browser.dll">
        <Instance>Plugin.Browser.PluginWindows</Instance>
    </Plugin>
    <Plugin Assembly="Plugin.Configuration.dll">
        <Instance>Plugin.Configuration.PluginWindows</Instance>
    </Plugin>
</Plugins>
```

## Plugin Path Resolution

The plugin provider resolves plugin paths in the following order of priority:

1. **App.config/Web.config** - `SAL_Path` appSettings value
2. **Command-line Arguments** - `/SAL_Path:` argument
3. **Current Directory** - `Environment.CurrentDirectory`
4. **Assembly Location** - Directory containing the plugin provider assembly

## Requirements

- **Plugin Classes:** All plugin classes must implement the `IPlugin` interface from SAL.Flatbed
- **Assembly Format:** Plugin assemblies must be valid .NET assemblies (`.dll` files)
- **XML File:** Each plugin directory must contain a `Plugins.List.xml` file

## Target Frameworks

- .NET Framework 3.5
- .NET Standard 2.0

## Dependencies

- [SAL.Flatbed](https://www.nuget.org/packages/SAL.Flatbed/) (v1.2.11+)
- System.Configuration.ConfigurationManager (.NET Standard 2.0 only)

## Building from Source

```bash
git clone https://github.com/DKorablin/Plugin.FileListPluginProvider.git
cd Plugin.FileListPluginProvider
dotnet build
```

## Related Projects

- [SAL.Flatbed](https://github.com/DKorablin/Flatbed) - Software Abstraction Layer framework
- [Flatbed.Dialog.Lite](https://github.com/DKorablin/Flatbed.Dialog.Lite) - Dialog components for SAL

## Support

If you encounter any issues or have questions, please [open an issue](https://github.com/DKorablin/Plugin.FileListPluginProvider/issues) on GitHub.