// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Input;

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
	/// 3. Call <see cref="StartCoreServices"/>.
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
		AddInTreeImpl addInTree;
		string applicationName;
		
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
		}
		
		/// <summary>
		/// Find AddIns by searching all .addin files recursively in <paramref name="addInDir"/>.
		/// The AddIns that were found are added to the list of AddIn files to load.
		/// </summary>
		public void AddAddInsFromDirectory(string addInDir)
		{
			if (addInDir == null)
				throw new ArgumentNullException("addInDir");
			addInFiles.AddRange(Directory.GetFiles(addInDir, "*.addin", SearchOption.AllDirectories));
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
		/// in <c>/SharpDevelop/Autostart</c>.
		/// </summary>
		public void RunInitialization()
		{
			addInTree.Load(addInFiles, disabledAddIns);
			
			// perform service registration
			var container = (IServiceContainer)ServiceSingleton.ServiceProvider.GetService(typeof(IServiceContainer));
			if (container != null)
				addInTree.BuildItems<object>("/SharpDevelop/Services", container, false);
			
			// run workspace autostart commands
			LoggingService.Info("Running autostart commands...");
			foreach (ICommand command in addInTree.BuildItems<ICommand>("/SharpDevelop/Autostart", null, false)) {
				try {
					command.Execute(null);
				} catch (Exception ex) {
					// allow startup to continue if some commands fail
					ServiceSingleton.GetRequiredService<IMessageService>().ShowException(ex);
				}
			}
		}
		
		/// <summary>
		/// Starts the core services.
		/// This initializes the PropertyService and ResourceService.
		/// </summary>
		public void StartCoreServices(IPropertyService propertyService)
		{
			var container = ServiceSingleton.GetRequiredService<IServiceContainer>();
			var applicationStateInfoService = new ApplicationStateInfoService();
			addInTree = new AddInTreeImpl(applicationStateInfoService);
			
			container.AddService(typeof(IPropertyService), propertyService);
			container.AddService(typeof(IResourceService), new ResourceServiceImpl(
				Path.Combine(propertyService.DataDirectory, "resources"), propertyService));
			container.AddService(typeof(IAddInTree), addInTree);
			container.AddService(typeof(ApplicationStateInfoService), applicationStateInfoService);
			StringParser.RegisterStringTagProvider(new AppNameProvider { appName = applicationName });
		}
		
		sealed class AppNameProvider : IStringTagProvider
		{
			internal string appName;
			
			public string ProvideString(string tag, StringTagPair[] customTags)
			{
				if (string.Equals(tag, "AppName", StringComparison.OrdinalIgnoreCase))
					return appName;
				else
					return null;
			}
		}
	}
}
