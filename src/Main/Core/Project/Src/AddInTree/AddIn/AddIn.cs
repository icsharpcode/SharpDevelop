// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Resources;
using System.IO;
using System.Collections.Generic;
using System.Xml;

namespace ICSharpCode.Core
{
	public sealed class AddIn
	{
		Properties    properties = new Properties();
		List<Runtime> runtimes   = new List<Runtime>();
		
		string        addInFileName = null;
		AddInManifest      manifest = new AddInManifest();
		Dictionary<string, ExtensionPath> paths = new Dictionary<string, ExtensionPath>();
		AddInAction action = AddInAction.Disable;
		bool enabled;
		
		static bool hasShownErrorMessage = false;

		public object CreateObject(string className)
		{
			foreach (Runtime runtime in runtimes) {
				object o = runtime.CreateInstance(className);
				if (o != null) {
					return o;
				}
			}
			if (hasShownErrorMessage) {
				LoggingService.Error("Cannot create object: " + className);
			} else {
				hasShownErrorMessage = true;
				MessageService.ShowError("Cannot create object: " + className + "\nFuture missing objects will not cause an error message.");
			}
			return null;
		}
		
		public override string ToString()
		{
			return "[AddIn: " + Name + "]";
		}
		
		/// <summary>
		/// Action to execute when the application is restarted.
		/// </summary>
		public AddInAction Action {
			get {
				return action;
			}
			set {
				action = value;
			}
		}
		
		public List<Runtime> Runtimes {
			get {
				return runtimes;
			}
		}
		
		public Version Version {
			get {
				return manifest.PrimaryVersion;
			}
		}
		
		public string FileName {
			get {
				return addInFileName;
			}
		}
		
		public string Name {
			get {
				return properties["name"];
			}
		}
		
		public AddInManifest Manifest {
			get {
				return manifest;
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
		
		public bool Enabled {
			get {
				return enabled;
			}
			internal set {
				enabled = value;
				this.Action = value ? AddInAction.Enable : AddInAction.Disable;
			}
		}
		
		AddIn()
		{
		}
		
		static void SetupAddIn(XmlTextReader reader, AddIn addIn, string hintPath)
		{
			while (reader.Read()) {
				if (reader.NodeType == XmlNodeType.Element && reader.IsStartElement()) {
					switch (reader.LocalName) {
						case "StringResources":
						case "BitmapResources":
							if (reader.AttributeCount != 1) {
								throw new AddInLoadException("BitmapResources requires ONE attribute.");
							}
							
							if(hintPath == null) break;
							
							string filename = StringParser.Parse(reader.GetAttribute("file"));
							string path = Path.Combine(hintPath, filename);
							
							if(! File.Exists(path))
							{
								throw new AddInLoadException("Resource file '" + path + "' could not be found.");
							}
							
							ResourceManager resourceManager = ResourceManager.CreateFileBasedResourceManager(Path.GetFileNameWithoutExtension(path), Path.GetDirectoryName(path), null);
							
							if(reader.LocalName == "BitmapResources")
							{
								ResourceService.RegisterNeutralImages(resourceManager);
							}
							else
							{
								ResourceService.RegisterNeutralStrings(resourceManager);
							}
							break;
						case "Runtime":
							if (!reader.IsEmptyElement) {
								while (reader.Read()){
									if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Runtime") {
										break;
									}
									if (reader.NodeType == XmlNodeType.Element && reader.IsStartElement()) {
										switch (reader.LocalName) {
											case "Import":
												addIn.runtimes.Add(Runtime.Read(addIn, reader, hintPath));
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
							if (hintPath == null) {
								throw new AddInLoadException("Cannot use include nodes when hintPath was not specified (e.g. when AddInManager reads a .addin file)!");
							}
							string fileName = Path.Combine(hintPath, reader.GetAttribute(0));
							using (XmlTextReader includeReader = new XmlTextReader(fileName)) {
								SetupAddIn(includeReader, addIn, Path.GetDirectoryName(fileName));
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
						case "Manifest":
							addIn.Manifest.ReadManifestSection(reader, hintPath);
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
			return Load(textReader, null);
		}
		
		public static AddIn Load(TextReader textReader, string hintPath)
		{
			AddIn addIn = new AddIn();
			using (XmlTextReader reader = new XmlTextReader(textReader)) {
				while (reader.Read()){
					if (reader.IsStartElement()) {
						switch (reader.LocalName) {
							case "AddIn":
								addIn.properties = Properties.ReadFromAttributes(reader);
								SetupAddIn(reader, addIn, hintPath);
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
					AddIn addIn = Load(textReader, Path.GetDirectoryName(fileName));
					addIn.addInFileName = fileName;
					return addIn;
				}
			} catch (Exception e) {
				throw new AddInLoadException("Can't load " + fileName + " " + e);
			}
		}
	}
}
