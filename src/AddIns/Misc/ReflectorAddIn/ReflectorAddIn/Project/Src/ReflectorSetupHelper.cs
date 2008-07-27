// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email="chhornung@googlemail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ReflectorAddIn
{
	internal static class ReflectorSetupHelper
	{
		internal const string ReflectorExePathPropertyName = "ReflectorAddIn.ReflectorExePath";
		
		/// <summary>
		/// Gets the full path of Reflector.exe, asking the user for it if it
		/// has not been set.
		/// </summary>
		/// <param name="owner">The owner for the path selection dialog, if it is needed.</param>
		/// <returns>The full path of Reflector.exe, or <c>null</c> if the path was unknown and the user cancelled the path selection dialog.</returns>
		internal static string GetReflectorExeFullPathInteractive(IWin32Window owner) {
			string path = PropertyService.Get(ReflectorExePathPropertyName);
			string askReason = null;
			
			if (String.IsNullOrEmpty(path)) {
				askReason = ResourceService.GetString("ReflectorAddIn.ReflectorPathNotSet");
			} else if (!File.Exists(path)) {
				askReason = ResourceService.GetString("ReflectorAddIn.ReflectorDoesNotExist");
			}
			
			if (askReason != null) {
				using(SetReflectorPathDialog dialog = new SetReflectorPathDialog(path, askReason)) {
					
					if (dialog.ShowDialog(owner) != DialogResult.OK || !File.Exists(dialog.SelectedFile)) {
						return null;
					}
					
					path = dialog.SelectedFile;
					PropertyService.Set(ReflectorExePathPropertyName, path);
					
				}
			}
			
			return path;
		}
		
		/// <summary>
		/// Sets up Reflector at the given full path to Reflector.exe so that
		/// it loads the IpcServer AddIn.
		/// </summary>
		internal static bool SetupReflector(string path) {
			
			const string IpcServerDllName = "Reflector.IpcServer.dll";
			const string IpcServerAddInDllName = "Reflector.IpcServer.AddIn.dll";
			const string ReflectorConfigName = "Reflector.cfg";
			
			try {
				
				// Copy the DLLs
				string targetPath = Path.GetDirectoryName(path);
				string sourcePath = Path.GetDirectoryName(new Uri(typeof(ReflectorSetupHelper).Assembly.CodeBase, UriKind.Absolute).LocalPath);
				
				string ipcServerDllTargetPath = Path.Combine(targetPath, IpcServerDllName);
				string ipcServerAddInDllTargetPath = Path.Combine(targetPath, IpcServerAddInDllName);
				
				File.Copy(Path.Combine(sourcePath, IpcServerDllName), ipcServerDllTargetPath, true);
				File.Copy(Path.Combine(sourcePath, IpcServerAddInDllName), ipcServerAddInDllTargetPath, true);
				
				// Adjust the configuration
				string cfgFileName = Path.Combine(targetPath, ReflectorConfigName);
				string cfgContent;
				if (File.Exists(cfgFileName)) {
					cfgContent = File.ReadAllText(cfgFileName);
				} else {
					cfgContent = Environment.NewLine;
				}
				
				int insertionIndex = -1;
				bool ipcServerAddInFound = false;
				Match m = Regex.Match(cfgContent, @"\[AddInManager\](?:\s*""(?<1>[^""]*)"")*\s*(?:\[|\z)", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);
				if (m != null && m.Success) {
					
					foreach (Capture c in m.Groups[1].Captures) {
						if (String.Equals(c.Value, ipcServerAddInDllTargetPath, StringComparison.InvariantCultureIgnoreCase)) {
							ipcServerAddInFound = true;
						}
						insertionIndex = c.Index + c.Length + 1;
					}
					
				} else {
					
					cfgContent += Environment.NewLine + "[AddInManager]" + Environment.NewLine;
					insertionIndex = cfgContent.Length - Environment.NewLine.Length;
					
				}
				
				if (!ipcServerAddInFound) {
					if (insertionIndex == -1) {
						MessageService.ShowError("Cannot configure Reflector because the configuration file could not be parsed.");
					} else {
						cfgContent = cfgContent.Insert(insertionIndex, Environment.NewLine + "\"" + ipcServerAddInDllTargetPath + "\"");
						File.Copy(cfgFileName, Path.ChangeExtension(cfgFileName, ".cfg.bak"), true);
						File.WriteAllText(cfgFileName, cfgContent, System.Text.Encoding.Default);
					}
				}
				
				return true;
				
				
			} catch (IOException ex) {
				MessageService.ShowWarning(ResourceService.GetString("ReflectorAddIn.ErrorReflectorSetup") + Environment.NewLine + ex.Message);
			} catch (UnauthorizedAccessException ex) {
				MessageService.ShowWarning(ResourceService.GetString("ReflectorAddIn.ErrorReflectorSetup") + Environment.NewLine + ex.Message);
			}
			
			return false;
			
		}
	}
}
