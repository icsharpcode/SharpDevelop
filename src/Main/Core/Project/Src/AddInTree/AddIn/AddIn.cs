using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Description of AddIn.
	/// </summary>
	public class AddIn
	{
		Properties          properties = new Properties();
		List<Runtime>       runtimes   = new List<Runtime>();
		string              addInFileName = null;
		Dictionary<string, ExtensionPath> paths = new Dictionary<string, ExtensionPath>();
		
		public object CreateObject(string className)
		{
			foreach (Runtime runtime in runtimes) {
				object o = runtime.CreateInstance(Path.GetDirectoryName(addInFileName), className);
				if (o != null) {
					return o;
				}
			}
			return null;
		}
		
		public List<Runtime> Runtimes {
			get {
				return runtimes;
			}
		}
		
		public string FileName {
			get {
				return addInFileName;
			}
		}
		
		public Dictionary<string, ExtensionPath> Paths {
			get {
				return paths;
			}
		}
		
		public Properties Properties {
			get {
				return properties;
			}
		}
		
		AddIn()
		{
		}
		
		static void SetupAddIn(XmlTextReader reader, AddIn addIn)
		{	
			while (reader.Read()) {
				if (reader.NodeType == XmlNodeType.Element && reader.IsStartElement()) {
					switch (reader.LocalName) {
						case "Runtime":
							if (!reader.IsEmptyElement) {
								while (reader.Read()){
									if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Runtime") {
										break;
									}
									if (reader.NodeType == XmlNodeType.Element && reader.IsStartElement()) {
										switch (reader.LocalName) {
											case "Import":
												addIn.runtimes.Add(Runtime.Read(addIn, reader));
												break;
											default:
												throw new AddInLoadException("Unknown node in runtime section :" + reader.LocalName);
										}
									}
								}
							}
							break;
						case "Include":
							if (reader.AttributeCount != 1) {
								throw new AddInLoadException("Include requires ONE attribute.");
							}
							if (!reader.IsEmptyElement) {
								throw new AddInLoadException("Include nodes must be empty!");
							}
							string fileName = reader.GetAttribute(0);
							using (XmlTextReader includeReader = new XmlTextReader(fileName)) {
								SetupAddIn(includeReader, addIn);
							}
							break;
						case "Path":
							if (reader.AttributeCount != 1) {
								throw new AddInLoadException("Import node requires ONE attribute.");
							}
							string pathName = reader.GetAttribute(0);
							ExtensionPath extensionPath = addIn.GetExtensionPath(pathName);
							if (!reader.IsEmptyElement) {
								ExtensionPath.SetUp(extensionPath, reader, "Path");
							}
							break;
						default:
							throw new AddInLoadException("Unknown root path node:" + reader.LocalName);
					}
				}
			}
		}
		
		public ExtensionPath GetExtensionPath(string pathName)
		{
			if (!paths.ContainsKey(pathName)) {
				return paths[pathName] = new ExtensionPath(pathName, this);
			}
			return paths[pathName];
		}
		
		public static AddIn Load(TextReader textReader)
		{
			AddIn addIn = new AddIn();
			using (XmlTextReader reader = new XmlTextReader(textReader)) {
				while (reader.Read()){
					if (reader.IsStartElement()) {
						switch (reader.LocalName) {
							case "AddIn":
								addIn.properties = Properties.ReadFromAttributes(reader);
								SetupAddIn(reader, addIn);
								break;
							default:
								throw new AddInLoadException("Unknown add-in file.");
						}
					}
				}
			}
			return addIn;
		}
		
		public static AddIn Load(string fileName)
		{
			try {
				using (TextReader textReader = File.OpenText(fileName)) {
					AddIn addIn = Load(textReader);
					addIn.addInFileName = fileName;
					return addIn;
				}
			} catch (Exception e) {
				throw new AddInLoadException("Can't load " + fileName + " " + e);
			}
		}
	}
}
