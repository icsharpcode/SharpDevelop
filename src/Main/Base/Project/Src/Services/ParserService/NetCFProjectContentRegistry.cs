// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Dom;
using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop
{
	public abstract class NetCFProjectContentRegistry : ProjectContentRegistry
	{
		public override IProjectContent Mscorlib {
			get {
				string folder = GetInstallFolder();
				if (folder != null)
					return GetProjectContentForReference("mscorlib", Path.Combine(folder, "mscorlib.dll"));
				else
					return GetProjectContentForReference("mscorlib", "mscorlib");
			}
		}
		
		protected abstract string AssemblyFolderRegKey {
			get;
		}
		
		string GetInstallFolder()
		{
			RegistryKey key = Registry.LocalMachine.OpenSubKey(AssemblyFolderRegKey);
			if (key != null) {
				string dir = key.GetValue(null) as string;
				key.Close();
				return dir;
			}
			return null;
		}
		
		protected override System.Reflection.Assembly GetDefaultAssembly(string shortName)
		{
			// do not use default assemblies, but pick the CF version
			return null;
		}
	}
	
	public class NetCF20ProjectContentRegistry : NetCFProjectContentRegistry
	{
		protected override string AssemblyFolderRegKey {
			get {
				return @"SOFTWARE\Microsoft\.NETCompactFramework\v2.0.0.0\WindowsCE\AssemblyFoldersEx";
			}
		}
	}
	
	public class NetCF35ProjectContentRegistry : NetCFProjectContentRegistry
	{
		protected override string AssemblyFolderRegKey {
			get {
				return @"SOFTWARE\Microsoft\.NETCompactFramework\v3.5.0.0\WindowsCE\AssemblyFoldersEx";
			}
		}
	}
}
