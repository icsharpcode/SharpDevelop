// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email="chhornung@googlemail.com"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Gui;
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
		internal static string GetReflectorExeFullPathInteractive() {
			string path = PropertyService.Get(ReflectorExePathPropertyName);
			string askReason = null;
			
			if (String.IsNullOrEmpty(path)) {
				askReason = ResourceService.GetString("ReflectorAddIn.ReflectorPathNotSet");
			} else if (!File.Exists(path)) {
				askReason = ResourceService.GetString("ReflectorAddIn.ReflectorDoesNotExist");
			}
			
			if (askReason != null) {
				using(SetReflectorPathDialog dialog = new SetReflectorPathDialog(path, askReason)) {
					
					if (dialog.ShowDialog(WorkbenchSingleton.MainForm) != DialogResult.OK || !File.Exists(dialog.SelectedFile)) {
						return null;
					}
					
					path = dialog.SelectedFile;
					PropertyService.Set(ReflectorExePathPropertyName, path);
					
				}
			}
			
			return path;
		}
	}
}
