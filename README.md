# File List Plugin Provider
If you want to specify exact plugins to load or you application required fixed plugins to load you can use current plugin provider, where you can specify xml file with a list of plugins an instances to load. The have to implement IPlugin interface.

Of the plugins is stored elsewhere you can specify plugins path by command argument SAL_Path and separate by «;» if you want to specify more than one path.

In each specified folder plugin will search for file Plugins.List.xml. File must contains root element Plugins, witch will contain a list of Plugin elements. Each Plugin element, must contains at least one attribute Assembly and array of child Instance elements:

    Assembly — File name with extension in current folder
    Instance — Full class name with namespace, witch implement interface IPlugin.