// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.IO;
using System.Text;
using System.Xml;

namespace ICSharpCode.Core
{
	public static class PropertyService
	{
		static string propertyFileName;
		static string propertyXmlRootNodeName;
		
		static string configDirectory;
		static string dataDirectory;
		
		static Properties properties;
		
		public static bool Initialized
		{
			get
			{
				return properties != null;
			}
		}

		public static void InitializeService(string configDirectory, string dataDirectory, string propertiesName)
		{
			if (properties != null)
				throw new InvalidOperationException("Service is already initialized.");
			if (configDirectory == null || dataDirectory == null || propertiesName == null)
				throw new ArgumentNullException();
			properties = new Properties();
			PropertyService.configDirectory = configDirectory;
			PropertyService.dataDirectory = dataDirectory;
			propertyXmlRootNodeName = propertiesName;
			propertyFileName = propertiesName + ".xml";
			properties.PropertyChanged += new PropertyChangedEventHandler(PropertiesPropertyChanged);
		}
		
		public static string ConfigDirectory {
			get {
				return configDirectory;
			}
		}
		
		public static string DataDirectory {
			get {
				return dataDirectory;
			}
		}
		
		public static string Get(string property)
		{
			return properties[property];
		}
		
		public static T Get<T>(string property, T defaultValue)
		{
			return properties.Get(property, defaultValue);
		}
		
		public static void Set<T>(string property, T value)
		{
			properties.Set(property, value);
		}
		
		public static void Load()
		{
			if (properties == null)
				throw new InvalidOperationException("Service is not initialized.");
			if (!Directory.Exists(configDirectory)) {
				Directory.CreateDirectory(configDirectory);
			}
			
			if (!LoadPropertiesFromStream(Path.Combine(configDirectory, propertyFileName))) {
				LoadPropertiesFromStream(FileUtility.Combine(DataDirectory, "options", propertyFileName));
			}
		}
		
		public static bool LoadPropertiesFromStream(string fileName)
		{
			if (!File.Exists(fileName)) {
				return false;
			}
			try {
				using (XmlTextReader reader = new XmlTextReader(fileName)) {
					while (reader.Read()){
						if (reader.IsStartElement()) {
							if (reader.LocalName == propertyXmlRootNodeName) {
								properties.ReadProperties(reader, propertyXmlRootNodeName);
								return true;
							}
						}
					}
				}
			} catch (XmlException ex) {
				MessageService.ShowError("Error loading properties: " + ex.Message + "\nSettings have been restored to default values.");
			}
			return false;
		}
		
		public static void Save()
		{
			string fileName = Path.Combine(configDirectory, propertyFileName);
			using (XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.UTF8)) {
				writer.Formatting = Formatting.Indented;
				writer.WriteStartElement(propertyXmlRootNodeName);
				properties.WriteProperties(writer);
				writer.WriteEndElement();
			}
		}
		
		static void PropertiesPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null) {
				PropertyChanged(null, e);
			}
		}
		
		public static event PropertyChangedEventHandler PropertyChanged;
	}
}
