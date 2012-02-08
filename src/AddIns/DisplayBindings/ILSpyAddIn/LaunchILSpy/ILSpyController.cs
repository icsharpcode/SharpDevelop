// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.ILSpyAddIn
{
	/// <summary>
	/// Controls .NET ILSpy.
	/// </summary>
	public static class ILSpyController
	{
		public static void TryGoTo(AbstractEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			while ((entity is IMember) && ((IMember)entity).GenericMember is AbstractEntity)
				entity = (AbstractEntity)((IMember)entity).GenericMember;
			
			// Try to find the assembly which contains the resolved type
			IProjectContent pc = entity.ProjectContent;
			ReflectionProjectContent rpc = pc as ReflectionProjectContent;
			string assemblyLocation = null;
			if (rpc != null) {
				assemblyLocation = GetAssemblyLocation(rpc);
			} else {
				IProject project = pc.Project as IProject;
				if (project != null) {
					assemblyLocation = project.OutputAssemblyFullPath;
				}
			}
			
			if (string.IsNullOrEmpty(assemblyLocation)) {
				MessageService.ShowWarning("ILSpy AddIn: Could not determine the assembly location for " + entity.FullyQualifiedName + ".");
				return;
			}
			
			string ilspyPath = GetILSpyExeFullPathInteractive();
			if (string.IsNullOrEmpty(ilspyPath))
				return;
			
			string commandLine = "/singleInstance \"" + assemblyLocation + "\" \"/navigateTo:" + entity.DocumentationTag + "\"";
			LoggingService.Debug(ilspyPath + " " + commandLine);
			Process.Start(ilspyPath, commandLine);
		}
		
		public static string GetAssemblyLocation(ReflectionProjectContent rpc)
		{
			if (rpc == null)
				throw new ArgumentNullException("rpc");
			// prefer GAC assemblies over reference assemblies:
			return rpc.RealAssemblyLocation;
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
			string path = PropertyService.Get(ILSpyExePathPropertyName);
			string askReason = null;
			
			if (String.IsNullOrEmpty(path)) {
				askReason = ResourceService.GetString("ILSpyAddIn.ILSpyPathNotSet");
			} else if (!File.Exists(path)) {
				askReason = ResourceService.GetString("ILSpyAddIn.ILSpyDoesNotExist");
			}
			
			if (askReason != null) {
				using(SetILSpyPathDialog dialog = new SetILSpyPathDialog(path, askReason)) {
					
					if (dialog.ShowDialog(WorkbenchSingleton.MainWin32Window) != DialogResult.OK || !File.Exists(dialog.SelectedFile)) {
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
