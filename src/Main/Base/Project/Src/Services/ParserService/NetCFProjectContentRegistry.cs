// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using Microsoft.Win32;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;

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
				return ParserService.DefaultProjectContentRegistry.GetProjectContentForReference(itemInclude, itemFileName);
			}
			string netPath = GetInstallFolder();
			if (!string.IsNullOrEmpty(netPath) && File.Exists(Path.Combine(netPath, "mscorlib.dll"))) {
				string shortName = itemInclude;
				int pos = shortName.IndexOf(',');
				if (pos > 0)
					shortName = shortName.Substring(0, pos);
				
				if (File.Exists(Path.Combine(netPath, shortName + ".dll"))) {
					return CecilReader.LoadAssembly(Path.Combine(netPath, shortName + ".dll"), this);
				} else if (File.Exists(Path.Combine(netPath, shortName))) {
					// perhaps shortName includes file extension
					return CecilReader.LoadAssembly(Path.Combine(netPath, shortName), this);
				}
			} else {
				string message = "Warning: .NET Compact Framework SDK is not installed." + Environment.NewLine;
				if (!TaskService.BuildMessageViewCategory.Text.Contains(message)) {
					TaskService.BuildMessageViewCategory.AppendText(message);
				}
			}
			return ParserService.DefaultProjectContentRegistry.GetProjectContentForReference(itemInclude, itemFileName);
		}
	}
}
