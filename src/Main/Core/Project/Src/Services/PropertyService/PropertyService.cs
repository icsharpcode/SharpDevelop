// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
	public sealed class PropertyService
	{
		const string propertyFileName        = "SharpDevelopProperties.xml";
		const string propertyXmlRootNodeName = "SharpDevelopProperties";
		
		static string configDirectory = FileUtility.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".ICSharpCode", "SharpDevelop2") + Path.DirectorySeparatorChar;
		static string dataDirectory   = FileUtility.Combine(FileUtility.SharpDevelopRootPath, "data");
		
		static Properties properties  = new Properties();
		
		static PropertyService()
		{
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
			if (!Directory.Exists(configDirectory)) {
				Directory.CreateDirectory(configDirectory);
			}
			
			if (!LoadPropertiesFromStream(Path.Combine(configDirectory, propertyFileName))) {
				if (!LoadPropertiesFromStream(FileUtility.Combine(DataDirectory, "options", propertyFileName))) {
					MessageService.ShowError("Can't load properties file.");
				}
			}
		}
		
		public static bool LoadPropertiesFromStream(string fileName)
		{
			if (!File.Exists(fileName)) {
				return false;
			}
			using (XmlTextReader reader = new XmlTextReader(fileName)) {
				while (reader.Read()){
					if (reader.IsStartElement()) {
						switch (reader.LocalName) {
							case propertyXmlRootNodeName:
								properties.ReadProperties(reader, propertyXmlRootNodeName);
								return true;
						}
					}
				}
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
