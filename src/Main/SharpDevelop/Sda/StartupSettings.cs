// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Sda
{
	/// <summary>
	/// This class contains properties you can use to control how SharpDevelop is launched.
	/// </summary>
	[Serializable]
	public sealed class StartupSettings
	{
		bool useSharpDevelopErrorHandler = true;
		string applicationName = "SharpDevelop";
		string applicationRootPath;
		bool allowAddInConfigurationAndExternalAddIns = true;
		bool allowUserAddIns;
		string propertiesName;
		string configDirectory;
		string dataDirectory;
		string domPersistencePath;
		string resourceAssemblyName = "SharpDevelop";
		internal List<string> addInDirectories = new List<string>();
		internal List<string> addInFiles = new List<string>();
		
		/// <summary>
		/// Gets/Sets the name of the assembly to load the BitmapResources
		/// and English StringResources from.
		/// </summary>
		public string ResourceAssemblyName {
			get { return resourceAssemblyName; }
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				resourceAssemblyName = value;
			}
		}
		
		/// <summary>
		/// Gets/Sets whether the SharpDevelop exception box should be used for
		/// unhandled exceptions. The default is true.
		/// </summary>
		public bool UseSharpDevelopErrorHandler {
			get { return useSharpDevelopErrorHandler; }
			set { useSharpDevelopErrorHandler = value; }
		}
		
		/// <summary>
		/// Use the file <see cref="ConfigDirectory"/>\AddIns.xml to maintain
		/// a list of deactivated AddIns and list of AddIns to load from
		/// external locations.
		/// The default value is true.
		/// </summary>
		public bool AllowAddInConfigurationAndExternalAddIns {
			get { return allowAddInConfigurationAndExternalAddIns; }
			set { allowAddInConfigurationAndExternalAddIns = value; }
		}
		
		/// <summary>
		/// Allow user AddIns stored in the "application data" directory.
		/// The default is false.
		/// </summary>
		public bool AllowUserAddIns {
			get { return allowUserAddIns; }
			set { allowUserAddIns = value; }
		}
		
		/// <summary>
		/// Gets/Sets the application name used by the MessageService and some
		/// SharpDevelop windows. The default is "SharpDevelop".
		/// </summary>
		public string ApplicationName {
			get { return applicationName; }
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				applicationName = value;
			}
		}
		
		/// <summary>
		/// Gets/Sets the application root path to use.
		/// Use null (default) to use the base directory of the SharpDevelop AppDomain.
		/// </summary>
		public string ApplicationRootPath {
			get { return applicationRootPath; }
			set { applicationRootPath = value; }
		}
		
		/// <summary>
		/// Gets/Sets the directory used to store SharpDevelop properties,
		/// settings and user AddIns.
		/// Use null (default) to use "ApplicationData\ApplicationName"
		/// </summary>
		public string ConfigDirectory {
			get { return configDirectory; }
			set { configDirectory = value; }
		}
		
		/// <summary>
		/// Sets the data directory used to load resources.
		/// Use null (default) to use the default path "ApplicationRootPath\data".
		/// </summary>
		public string DataDirectory {
			get { return dataDirectory; }
			set { dataDirectory = value; }
		}
		
		/// <summary>
		/// Sets the name used for the properties file (without path or extension).
		/// Use null (default) to use the default name.
		/// </summary>
		public string PropertiesName {
			get { return propertiesName; }
			set { propertiesName = value; }
		}
		
		/// <summary>
		/// Sets the directory used to store the code completion cache.
		/// Use null (default) to disable the code completion cache.
		/// </summary>
		public string DomPersistencePath {
			get { return domPersistencePath; }
			set { domPersistencePath = value; }
		}
		
		/// <summary>
		/// Find AddIns by searching all .addin files recursively in <paramref name="addInDir"/>.
		/// </summary>
		public void AddAddInsFromDirectory(string addInDir)
		{
			if (addInDir == null)
				throw new ArgumentNullException("addInDir");
			addInDirectories.Add(addInDir);
		}
		
		/// <summary>
		/// Add the specified .addin file.
		/// </summary>
		public void AddAddInFile(string addInFile)
		{
			if (addInFile == null)
				throw new ArgumentNullException("addInFile");
			addInFiles.Add(addInFile);
		}
	}
}
