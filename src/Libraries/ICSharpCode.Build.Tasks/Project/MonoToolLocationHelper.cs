// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using Microsoft.Win32;
using System;
using System.IO;
using System.Security;

namespace ICSharpCode.Build.Tasks
{
	/// <summary>
	/// Utility to locate Mono compilers.
	/// </summary>
	public class MonoToolLocationHelper
	{
		public static readonly string MonoRootKeyName = @"Software\Novell\Mono";
		
		static string defaultMonoClr = null;
		static string monoSdkPath = null;
		static string monoFrameworkAssemblyPath = null;
		static string monoFrameworkVersion11Path = null;
		static string monoFrameworkVersion20Path = null;
		static bool monoInstalled;
		static bool checkedForMonoInstallation;
		
		MonoToolLocationHelper()
		{
		}
		
		/// <summary>
		/// Gets the full path, including the filename, of the specified
		/// Mono tool.
		/// </summary>
		public static string GetPathToTool(string name)
		{
			// Look for Mono install path in registry.
			
			string toolName = Path.ChangeExtension(name, ".bat");
			string sdkPath = GetPathToMonoSdk();
			if (sdkPath.Length > 0) {
				string toolPath = Path.Combine(sdkPath, toolName);
				if (System.IO.File.Exists(toolPath)) {
					return toolPath;
				}
			}
			
			// Assume the tool can be executed on the command line.
			
			string comspec = Environment.GetEnvironmentVariable("ComSpec");
			if (comspec != null) {
				return String.Concat(comspec, " /c ", Path.GetFileNameWithoutExtension(name));
			}
		
			return toolName;
		}
		
		public static string GetPathToMonoSdk()
		{
			if (monoSdkPath == null) {
				string defaultClr = GetDefaultMonoClr();
				if (defaultClr.Length > 0) {
					string keyName = String.Concat(MonoRootKeyName, "\\", defaultClr);
					monoSdkPath = ReadRegistryValue(keyName, "SdkInstallRoot");
					if (monoSdkPath.Length > 0) {
						monoSdkPath = Path.Combine(monoSdkPath, "bin");
					}
				} else {
					monoSdkPath = String.Empty;
				}
			}
			return monoSdkPath;
		}
		
		public static string GetDefaultMonoClr()
		{
			if (defaultMonoClr == null) {
				defaultMonoClr = ReadRegistryValue(MonoRootKeyName, "DefaultClr");
			}
			return defaultMonoClr;
		}
		
		/// <summary>
		/// Determines whether Mono is installed.
		/// </summary>
		public static bool IsMonoInstalled {
			get {
				if (!checkedForMonoInstallation) {
					checkedForMonoInstallation = true;
					string baseFolder = GetBasePathToMonoFramework();
					if (baseFolder != null && baseFolder.Length > 0) {
						monoInstalled = Directory.Exists(baseFolder);
					}
				}
				return monoInstalled;
			}
		}

		public static string GetPathToMonoFramework(TargetMonoFrameworkVersion version)
		{
			switch (version) {
				case TargetMonoFrameworkVersion.Version11:
					return GetPathToMonoFrameworkVersion11();
				case TargetMonoFrameworkVersion.Version20:
					return GetPathToMonoFrameworkVersion20();
			}
			return null;
		}
		
		/// <summary>
		/// Gets the full path to the Mono GAC
		/// </summary>
		public static string GetPathToMonoGac()
		{
			string frameworkFolder = GetBasePathToMonoFramework();
			if (frameworkFolder != null && frameworkFolder.Length > 0) {
				string gacFolder = Path.Combine(frameworkFolder, @"mono\gac");
				if (Directory.Exists(gacFolder)) {
					return gacFolder;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Gets the base path to the mono framework assemblies.
		/// </summary>
		static string GetBasePathToMonoFramework()
		{
			if (monoFrameworkAssemblyPath == null) {
				string defaultClr = GetDefaultMonoClr();
				if (defaultClr.Length > 0) {
					string keyName = String.Concat(MonoRootKeyName, "\\", defaultClr);
					monoFrameworkAssemblyPath = ReadRegistryValue(keyName, "FrameworkAssemblyDirectory");
				}
			}
			return monoFrameworkAssemblyPath;
		}
		
		static string GetPathToMonoFrameworkVersion11()
		{
			if (monoFrameworkVersion11Path == null) {
				monoFrameworkVersion11Path = GetPathToMonoFramework(@"mono\1.0");
			}
			return monoFrameworkVersion11Path;
		}
		
		static string GetPathToMonoFrameworkVersion20()
		{
			if (monoFrameworkVersion20Path == null) {
				monoFrameworkVersion20Path = GetPathToMonoFramework(@"mono\2.0");
			}
			return monoFrameworkVersion20Path;
		}
		
		static string GetPathToMonoFramework(string subFolder)
		{
			string monoFrameworkBaseFolder = GetBasePathToMonoFramework();
			if (monoFrameworkBaseFolder != null && monoFrameworkBaseFolder.Length > 0) {
				return Path.Combine(monoFrameworkBaseFolder, subFolder);
			}
			return null;
		}
		
		static string ReadRegistryValue(string keyName, string name)
		{
			try {
				RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName);
				if (key != null) {
					string readValue = (string)key.GetValue(name);
					key.Close();
					if (readValue != null) {
						return readValue;
					}
				}
			} catch (SecurityException) { }
			
			return String.Empty;
		}
	}
}
