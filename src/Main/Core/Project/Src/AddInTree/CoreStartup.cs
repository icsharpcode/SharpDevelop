// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Class that helps starting up ICSharpCode.Core.
	/// </summary>
	public class CoreStartup
	{
		List<string> addInFiles = new List<string>();
		List<string> disabledAddIns = new List<string>();
		string propertiesName;
		string configDirectory;
		string dataDirectory;
		string applicationName;
		
		/// <summary>
		/// Sets the name used for the properties (only name, without path or extension).
		/// Must be set before StartCoreServices() is called.
		/// </summary>
		public string PropertiesName {
			get {
				return propertiesName;
			}
			set {
				if (value == null || value.Length == 0)
					throw new ArgumentNullException("value");
				propertiesName = value;
			}
		}
		
		/// <summary>
		/// Sets the directory name used for the property service.
		/// Must be set before StartCoreServices() is called.
		/// Use null to use the default path "ApplicationData\ApplicationName".
		/// </summary>
		public string ConfigDirectory {
			get {
				return configDirectory;
			}
			set {
				configDirectory = value;
			}
		}
		
		/// <summary>
		/// Sets the data directory used to load resources.
		/// Must be set before StartCoreServices() is called.
		/// Use null to use the default path "ApplicationRootPath\data".
		/// </summary>
		public string DataDirectory {
			get {
				return dataDirectory;
			}
			set {
				dataDirectory = value;
			}
		}
		
		public CoreStartup(string applicationName)
		{
			if (applicationName == null)
				throw new ArgumentNullException("applicationName");
			this.applicationName = applicationName;
			propertiesName = applicationName + "Properties";
			MessageService.DefaultMessageBoxTitle = applicationName;
		}
		
		public void AddAddInsFromDirectory(string addInDir)
		{
			addInFiles.AddRange(FileUtility.SearchDirectory(addInDir, "*.addin"));
		}
		
		
		public void LoadAddInConfiguration(string configurationFile)
		{
			LoadAddInConfiguration(configurationFile, addInFiles, disabledAddIns);
		}
		
		/// <summary>
		/// Loads a configuration file.
		/// The 'file' from XML elements in the form "&lt;AddIn file='full path to .addin file'&gt;" will
		/// be added to <paramref name="addInFiles"/>, the 'addin' element from
		/// "&lt;Disable addin='addin identity'&gt;" will be added to <paramref name="disabledAddIns"/>,
		/// all other XML elements are ignored.
		/// </summary>
		public static void LoadAddInConfiguration(string configurationFile, List<string> addInFiles, List<string> disabledAddIns)
		{
			using (XmlTextReader reader = new XmlTextReader(configurationFile)) {
				while (reader.Read()) {
					if (reader.NodeType == XmlNodeType.Element) {
						if (reader.Name == "AddIn") {
							string fileName = reader.GetAttribute("file");
							if (fileName != null && fileName.Length > 0) {
								addInFiles.Add(fileName);
							}
						} else if (reader.Name == "Disable") {
							string addIn = reader.GetAttribute("addin");
							if (addIn != null && addIn.Length > 0) {
								disabledAddIns.Add(addIn);
							}
						}
					}
				}
			}
		}
		
		public static void SaveAddInConfiguration(string configurationFile, List<string> addInFiles, List<string> disabledAddIns)
		{
			using (XmlTextWriter writer = new XmlTextWriter(configurationFile, Encoding.UTF8)) {
				writer.Formatting = Formatting.Indented;
				writer.WriteStartDocument();
				writer.WriteStartElement("AddInConfiguration");
				foreach (string file in addInFiles) {
					writer.WriteStartElement("AddIn");
					writer.WriteAttributeString("file", file);
					writer.WriteEndElement();
				}
				foreach (string name in disabledAddIns) {
					writer.WriteStartElement("Disable");
					writer.WriteAttributeString("addin", name);
					writer.WriteEndElement();
				}
				writer.WriteEndDocument();
			}
		}
		
		public void RunInitialization()
		{
			AddInTree.Load(addInFiles, disabledAddIns);
			
			// run workspace autostart commands
			LoggingService.Info("Running autostart commands...");
			foreach (ICommand command in AddInTree.BuildItems("/Workspace/Autostart", null, false)) {
				command.Run();
			}
		}
		
		public void StartCoreServices()
		{
			if (configDirectory == null)
				configDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				                               applicationName);
			PropertyService.InitializeService(configDirectory,
			                                  dataDirectory ?? Path.Combine(FileUtility.ApplicationRootPath, "data"),
			                                  propertiesName);
			PropertyService.Load();
			ResourceService.InitializeService(FileUtility.Combine(PropertyService.DataDirectory, "resources"));
		}
	}
}
