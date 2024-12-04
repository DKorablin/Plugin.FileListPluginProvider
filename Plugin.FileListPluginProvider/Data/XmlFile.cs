using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Plugin.FileListPluginProvider.Properties;

namespace Plugin.FileListPluginProvider.Data
{
	internal class XmlFile
	{
		private readonly String _filePath;

		public XmlFile(String filePath)
		{
			if(String.IsNullOrEmpty(filePath))
				throw new ArgumentNullException(nameof(filePath));

			this._filePath = filePath;
		}

		/// <summary>Load XML file from <paramref name="xmlFilePath"/> and check with XSD</summary>
		/// <param name="xsd">Plugin array schema</param>
		/// <param name="xmlFilePath">Path to XML file with plugins description</param>
		/// <returns>Loaded XML file with checked schema or exception</returns>
		private static XmlDocument LoadDocument(String xsd, String xmlFilePath)
		{
			XmlDocument document = new XmlDocument();

			using(MemoryStream xsdStream = new MemoryStream(Encoding.UTF8.GetBytes(xsd)))
			{
				XmlReader xsdReader = XmlReader.Create(xsdStream);
				XmlReaderSettings settings = new XmlReaderSettings()
				{
					ValidationType=ValidationType.Schema,
				};
				settings.Schemas.Add(null, xsdReader);

				XmlReader xmlReader = XmlReader.Create(xmlFilePath, settings);
				document.Load(xmlReader);
			}

			return document;
		}

		public IEnumerable<PluginInfo> ReadPluginInfo()
		{
			XmlDocument document = XmlFile.LoadDocument(Resources.xsdPlugins_List,this._filePath);

			foreach(XmlNode node in document.SelectNodes("/Plugins/Plugin"))
			{
				XmlAttribute assembly = node.Attributes["Assembly"];

				XmlNodeList instanceNodes = node.SelectNodes("Instance");
				String[] instance = new String[instanceNodes.Count];
				for(Int32 loop = 0; loop < instance.Length; loop++)
					instance[loop] = instanceNodes[loop].InnerText;

				yield return new PluginInfo(assembly.Value, instance);
			}
		}

		public void SavePluginInfo(PluginInfo[] plugins)
		{
			XmlDocument document = new XmlDocument();
			XmlDeclaration head = document.CreateXmlDeclaration("1.0", "utf-8", "no");
			document.AppendChild(head);
			XmlNode rootNode = document.CreateNode(XmlNodeType.Element, "Plugins", null);
			foreach(PluginInfo plugin in plugins)
			{
				XmlNode node = document.CreateNode(XmlNodeType.Element, "Plugin", null);

				XmlAttribute assembly = document.CreateAttribute("Assembly");
				assembly.Value = plugin.AssemblyName;
				node.Attributes.Append(assembly);

				foreach(String instance in plugin.Instance)
				{
					XmlNode instanceNode = document.CreateNode(XmlNodeType.Element, "Instance", null);
					instanceNode.Value = instance;
					node.AppendChild(instanceNode);
				}

				rootNode.AppendChild(node);
			}
			document.AppendChild(rootNode);

			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
			};
			using(XmlWriter writer = XmlWriter.Create(this._filePath))
				document.Save(writer);
		}
	}
}