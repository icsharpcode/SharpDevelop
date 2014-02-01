/*
 * Created by SharpDevelop.
 * User: WheizWork
 * Date: 16.01.2014
 * Time: 00:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Threading;
using System.Xml;
using ICSharpCode.Core;
using NoGoop.Controls;

namespace NoGoop.ObjBrowser
{
	/// <summary>
	/// Description of PropertyService.
	/// </summary>
	public class PropertyService : PropertyServiceImpl
	{
		const string PropertiesName = "ComponentInspectorProperties";
		
		DirectoryName dataDirectory;
		DirectoryName configDirectory;
		FileName propertiesFileName;
		
		public PropertyService()
			: base(LoadPropertiesFromStream(GetConfigDirectory().CombineFile(PropertiesName + ".xml")))
		{
			this.configDirectory = GetConfigDirectory();
			this.dataDirectory = new DirectoryName(Environment.CurrentDirectory);
			propertiesFileName = configDirectory.CombineFile(PropertiesName + ".xml");
		}
		
		private static DirectoryName GetConfigDirectory()
		{
			return new DirectoryName(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			                                      "ICSharpCode"));
		}
		
		public override DirectoryName ConfigDirectory {
			get {
				return configDirectory;
			}
		}
		
		public override DirectoryName DataDirectory {
			get {
				return dataDirectory;
			}
		}
		static Properties LoadPropertiesFromStream(FileName fileName)
		{
			if (!File.Exists(fileName)) {
				return new Properties();
			}
			try {
				return Properties.Load(fileName);
			} catch (XmlException ex) {
				ErrorDialog.Show("Error loading properties: " + ex.Message + "\nSettings have been restored to default values.");
			} catch (IOException ex) {
				ErrorDialog.Show("Error loading properties: " + ex.Message + "\nSettings have been restored to default values.");
			}
			return new Properties();
		}
		
		public override void Save()
		{
			ComponentInspectorProperties.Update();
			this.MainPropertiesContainer.Save(propertiesFileName);
		}
	}
}
