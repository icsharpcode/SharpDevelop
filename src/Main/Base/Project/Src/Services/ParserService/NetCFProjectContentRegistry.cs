// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Dom;
using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop
{
	public class NetCF20ProjectContentRegistry : ProjectContentRegistry
	{
		public override IProjectContent Mscorlib {
			get {
				return GetProjectContentForReference("mscorlib", "mscorlib");
			}
		}
		
		static string GetInstallFolder()
		{
			const string regkey = @"SOFTWARE\Microsoft\.NETCompactFramework\v2.0.0.0\WindowsCE\AssemblyFoldersEx";
			RegistryKey key = Registry.LocalMachine.OpenSubKey(regkey);
			if (key != null) {
				string dir = key.GetValue(null) as string;
				key.Close();
				return dir;
			}
			return null;
		}
		
		
		protected override IProjectContent LoadProjectContent(string itemInclude, string itemFileName)
		{
			if (File.Exists(itemFileName)) {
				// we cannot reuse project contents from the default registry because they would
				// reference the wrong mscorlib version, causing code-completion problems
				// when a CF application references a CF library
				return base.LoadProjectContent(itemInclude, itemFileName);
			}
			string netPath = GetInstallFolder();
			if (!string.IsNullOrEmpty(netPath) && File.Exists(Path.Combine(netPath, "mscorlib.dll"))) {
				string shortName = itemInclude;
				int pos = shortName.IndexOf(',');
				if (pos > 0)
					shortName = shortName.Substring(0, pos);
				
				if (File.Exists(Path.Combine(netPath, shortName + ".dll"))) {
					ReflectionProjectContent rpc = CecilReader.LoadAssembly(Path.Combine(netPath, shortName + ".dll"), this);
					if (rpc != null) {
						redirectedAssemblyNames.Add(shortName, rpc.AssemblyFullName);
					}
					return rpc;
				} else if (File.Exists(Path.Combine(netPath, shortName))) {
					// perhaps shortName includes file extension
					ReflectionProjectContent rpc = CecilReader.LoadAssembly(Path.Combine(netPath, shortName), this);
					if (rpc != null) {
						redirectedAssemblyNames.Add(Path.GetFileNameWithoutExtension(shortName), rpc.AssemblyFullName);
					}
					return rpc;
				}
			} else {
				string message = "Warning: .NET Compact Framework SDK is not installed." + Environment.NewLine;
				if (!TaskService.BuildMessageViewCategory.Text.Contains(message)) {
					TaskService.BuildMessageViewCategory.AppendText(message);
				}
			}
			return base.LoadProjectContent(itemInclude, itemFileName);
		}
	}
}
