// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
			string sdkPath = MonoSdkPath;
			if (sdkPath.Length > 0) {
				string toolPath = Path.Combine(sdkPath, String.Concat("bin\\", toolName));
				if (System.IO.File.Exists(toolPath)) {
					return String.Concat("\"", toolPath, "\"");
				}
			}
			
			// Assume the tool can be executed on the command line.
			
			string comspec = Environment.GetEnvironmentVariable("ComSpec");
			if (comspec != null) {
				return String.Concat(comspec, " /c ", Path.GetFileNameWithoutExtension(name));
			}
		
			return toolName;
		}
		
		public static string MonoSdkPath {
			get {
				if (monoSdkPath == null) {
					string defaultClr = DefaultMonoClr;
					if (defaultClr.Length > 0) {
						string keyName = String.Concat(MonoRootKeyName, "\\", defaultClr);
						monoSdkPath = ReadRegistryValue(keyName, "SdkInstallRoot");
					} else {
						monoSdkPath = String.Empty;
					}
				}
				return monoSdkPath;
			}
		}
		
		public static string DefaultMonoClr {
			get {
				if (defaultMonoClr == null) {
					defaultMonoClr = ReadRegistryValue(MonoRootKeyName, "DefaultClr");
				}
				return defaultMonoClr;
			}
		}
		
		static string ReadRegistryValue(string keyName, string name)
		{
			try {
				RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName);
				if (key != null) {
					string readValue = (string)key.GetValue(name);
					key.Close();
					return readValue;
				}
			} catch (SecurityException) { }
			
			return String.Empty;
		}
	}
}
