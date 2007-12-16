// SharpDevelop samples
// Copyright (c) 2006, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.IO;
using System.Security;

using Microsoft.Win32;

namespace Mono.Build.Tasks
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
