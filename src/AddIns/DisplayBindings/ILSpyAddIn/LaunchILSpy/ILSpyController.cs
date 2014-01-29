// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Documentation;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.ILSpyAddIn
{
	/// <summary>
	/// Controls .NET ILSpy.
	/// </summary>
	public static class ILSpyController
	{
		public static void OpenInILSpy(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			// Get the underlying entity for generic instance members
			if (entity is IMember)
				entity = ((IMember)entity).MemberDefinition;
			
			// Try to find the assembly which contains the resolved type
			var assemblyLocation = entity.ParentAssembly.GetRuntimeAssemblyLocation();
			
			if (string.IsNullOrEmpty(assemblyLocation)) {
				MessageService.ShowWarning("ILSpy AddIn: Could not determine the assembly location for " + entity.ParentAssembly.AssemblyName + ".");
				return;
			}
			
			string ilspyPath = GetILSpyExeFullPathInteractive();
			if (string.IsNullOrEmpty(ilspyPath))
				return;
			
			string commandLine = "/singleInstance \"" + assemblyLocation + "\" \"/navigateTo:" + IdStringProvider.GetIdString(entity) + "\"";
			LoggingService.Debug(ilspyPath + " " + commandLine);
			Process.Start(ilspyPath, commandLine);
		}
		
		#region Find ILSpy
		internal const string ILSpyExePathPropertyName = "ILSpyAddIn.ILSpyExePath";
		
		/// <summary>
		/// Gets the full path of ILSpy.exe, asking the user for it if it
		/// has not been set.
		/// </summary>
		/// <param name="owner">The owner for the path selection dialog, if it is needed.</param>
		/// <returns>The full path of ILSpy.exe, or <c>null</c> if the path was unknown and the user cancelled the path selection dialog.</returns>
		internal static string GetILSpyExeFullPathInteractive()
		{
			string path = PropertyService.Get(ILSpyExePathPropertyName, "");
			string askReason = null;
			
			if (String.IsNullOrEmpty(path)) {
				askReason = ResourceService.GetString("ILSpyAddIn.ILSpyPathNotSet");
			} else if (!File.Exists(path)) {
				askReason = ResourceService.GetString("ILSpyAddIn.ILSpyDoesNotExist");
			}
			
			if (askReason != null) {
				using(SetILSpyPathDialog dialog = new SetILSpyPathDialog(path, askReason)) {
					
					if (dialog.ShowDialog(SD.WinForms.MainWin32Window) != DialogResult.OK || !File.Exists(dialog.SelectedFile)) {
						return null;
					}
					
					path = dialog.SelectedFile;
					PropertyService.Set(ILSpyExePathPropertyName, path);
					
				}
			}
			
			return path;
		}
		#endregion
	}
}
