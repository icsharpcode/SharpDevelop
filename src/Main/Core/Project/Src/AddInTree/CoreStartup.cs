/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 25.11.2005
 * Time: 18:56
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Class that helps starting up ICSharpCode.Core.
	/// </summary>
	public class CoreStartup
	{
		List<string> addInFiles = new List<string>();
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
		
		public void RunInitialization()
		{
			LoggingService.Info("Loading AddInTree...");
			AddInTree.Load(addInFiles);
			
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
