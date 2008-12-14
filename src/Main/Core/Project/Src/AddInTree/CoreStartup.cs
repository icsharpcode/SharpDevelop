// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Class that helps starting up ICSharpCode.Core.
	/// </summary>
	/// <remarks>
	/// Initializing ICSharpCode.Core requires initializing several static classes
	/// and the <see cref="AddInTree"/>. <see cref="CoreStartup"/> does this work
	/// for you, provided you use it like this:
	/// 1. Create a new CoreStartup instance
	/// 2. (Optional) Set the values of the properties.
	/// 3. Call <see cref="StartCoreServices()"/>.
	/// 4. Add "preinstalled" AddIns using <see cref="AddAddInsFromDirectory"/>
	///    and <see cref="AddAddInFile"/>.
	/// 5. (Optional) Call <see cref="ConfigureExternalAddIns"/> to support
	///    disabling AddIns and installing external AddIns
	/// 6. (Optional) Call <see cref="ConfigureUserAddIns"/> to support installing
	///    user AddIns.
	/// 7. Call <see cref="RunInitialization"/>.
	/// </remarks>
	public sealed class CoreStartup
	{
		List<string> addInFiles = new List<string>();
		List<string> disabledAddIns = new List<string>();
		bool externalAddInsConfigured;
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
		/// Use null to use the default path "%ApplicationData%\%ApplicationName%",
		/// where %ApplicationData% is the system setting for
		/// "c:\documents and settings\username\application data"
		/// and %ApplicationName% is the application name you used in the
		/// CoreStartup constructor call.
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
		
		/// <summary>
		/// Creates a new CoreStartup instance.
		/// </summary>
		/// <param name="applicationName">
		/// The name of your application.
		/// This is used as default title for message boxes,
		/// default name for the configuration directory etc.
		/// </param>
		public CoreStartup(string applicationName)
		{
			if (applicationName == null)
				throw new ArgumentNullException("applicationName");
			this.applicationName = applicationName;
			propertiesName = applicationName + "Properties";
			MessageService.DefaultMessageBoxTitle = applicationName;
			MessageService.ProductName = applicationName;
		}
		
		/// <summary>
		/// Find AddIns by searching all .addin files recursively in <paramref name="addInDir"/>.
		/// The found AddIns are added to the list of AddIn files to load.
		/// </summary>
		public void AddAddInsFromDirectory(string addInDir)
		{
			if (addInDir == null)
				throw new ArgumentNullException("addInDir");
			addInFiles.AddRange(FileUtility.SearchDirectory(addInDir, "*.addin"));
		}
		
		/// <summary>
		/// Add the specified .addin file to the list of AddIn files to load.
		/// </summary>
		public void AddAddInFile(string addInFile)
		{
			if (addInFile == null)
				throw new ArgumentNullException("addInFile");
			addInFiles.Add(addInFile);
		}
		
		/// <summary>
		/// Use the specified configuration file to store information about
		/// disabled AddIns and external AddIns.
		/// You have to call this method to support the <see cref="AddInManager"/>.
		/// </summary>
		/// <param name="addInConfigurationFile">
		/// The name of the file used to store the list of disabled AddIns
		/// and the list of installed external AddIns.
		/// A good value for this parameter would be
		/// <c>Path.Combine(<see cref="PropertyService.ConfigDirectory"/>, "AddIns.xml")</c>.
		/// </param>
		public void ConfigureExternalAddIns(string addInConfigurationFile)
		{
			externalAddInsConfigured = true;
			AddInManager.ConfigurationFileName = addInConfigurationFile;
			AddInManager.LoadAddInConfiguration(addInFiles, disabledAddIns);
		}
		
		/// <summary>
		/// Configures user AddIn support.
		/// </summary>
		/// <param name="addInInstallTemp">
		/// The AddIn installation temporary directory.
		/// ConfigureUserAddIns will install the AddIns from this directory and
		/// store the parameter value in <see cref="AddInManager.AddInInstallTemp"/>.
		/// </param>
		/// <param name="userAddInPath">
		/// The path where user AddIns are installed to.
		/// AddIns from this directory will be loaded.
		/// </param>
		public void ConfigureUserAddIns(string addInInstallTemp, string userAddInPath)
		{
			if (!externalAddInsConfigured) {
				throw new InvalidOperationException("ConfigureExternalAddIns must be called before ConfigureUserAddIns");
			}
			AddInManager.AddInInstallTemp = addInInstallTemp;
			AddInManager.UserAddInPath = userAddInPath;
			if (Directory.Exists(addInInstallTemp)) {
				AddInManager.InstallAddIns(disabledAddIns);
			}
			if (Directory.Exists(userAddInPath)) {
				AddAddInsFromDirectory(userAddInPath);
			}
		}
		
		/// <summary>
		/// Initializes the AddIn system.
		/// This loads the AddIns that were added to the list,
		/// then it executes the <see cref="ICommand">commands</see>
		/// in <c>/Workspace/Autostart</c>.
		/// </summary>
		public void RunInitialization()
		{
			AddInTree.Load(addInFiles, disabledAddIns);
			
			// run workspace autostart commands
			LoggingService.Info("Running autostart commands...");
			foreach (ICommand command in AddInTree.BuildItems<ICommand>("/Workspace/Autostart", null, false)) {
				try {
					command.Run();
				} catch (Exception ex) {
					// allow startup to continue if some commands fail
					MessageService.ShowError(ex);
				}
			}
		}
		
		/// <summary>
		/// Starts the core services.
		/// This initializes the PropertyService and ResourceService.
		/// </summary>
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
			StringParser.Properties["AppName"] = applicationName;
		}
	}
}
