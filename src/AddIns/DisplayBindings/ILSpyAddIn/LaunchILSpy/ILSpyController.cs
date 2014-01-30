// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
