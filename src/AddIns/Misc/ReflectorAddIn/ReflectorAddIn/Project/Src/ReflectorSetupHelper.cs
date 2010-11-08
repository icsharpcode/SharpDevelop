// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text.RegularExpressions;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ReflectorAddIn.Windows;

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
				SetReflectorPath dialog = new SetReflectorPath(path, askReason);
				dialog.Owner = WorkbenchSingleton.MainWindow;
				bool? result = dialog.ShowDialog();
				if (!result.HasValue || !result.Value || !File.Exists(dialog.SelectedFile))
					return null;
				
				path = dialog.SelectedFile;
				PropertyService.Set(ReflectorExePathPropertyName, path);
			}
			
			return path;
		}
		
		internal static void OpenReflectorExeFullPathInteractiver() {
			string path = PropertyService.Get(ReflectorExePathPropertyName);
			string askReason = null;
			
			SetReflectorPath dialog = new SetReflectorPath(path, askReason);
			dialog.Owner = WorkbenchSingleton.MainWindow;
			
			bool? result = dialog.ShowDialog();
			if (!result.HasValue || !result.Value || !File.Exists(dialog.SelectedFile))
				return;
			
			PropertyService.Set(ReflectorExePathPropertyName, dialog.SelectedFile);
		}
	}
}
