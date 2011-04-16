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
			string assemblyLocation = FindAssemblyInNetGac(new DomAssemblyName(rpc.AssemblyFullName));
			if (string.IsNullOrEmpty(assemblyLocation)) {
				// use file only if assembly isn't in GAC:
				assemblyLocation = rpc.AssemblyLocation;
			}
			return assemblyLocation;
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
		
		#region FindAssemblyInGac
		// This region is based on code from Mono.Cecil:
		
		// Author:
		//   Jb Evain (jbevain@gmail.com)
		//
		// Copyright (c) 2008 - 2010 Jb Evain
		//
		// Permission is hereby granted, free of charge, to any person obtaining
		// a copy of this software and associated documentation files (the
		// "Software"), to deal in the Software without restriction, including
		// without limitation the rights to use, copy, modify, merge, publish,
		// distribute, sublicense, and/or sell copies of the Software, and to
		// permit persons to whom the Software is furnished to do so, subject to
		// the following conditions:
		//
		// The above copyright notice and this permission notice shall be
		// included in all copies or substantial portions of the Software.
		//
		// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
		// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
		// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
		// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
		// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
		// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
		// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
		//

		static readonly string[] gac_paths = { GacInterop.GacRootPathV2, GacInterop.GacRootPathV4 };
		static readonly string[] gacs = { "GAC_MSIL", "GAC_32", "GAC" };
		static readonly string[] prefixes = { string.Empty, "v4.0_" };
		
		/// <summary>
		/// Gets the file name for an assembly stored in the GAC.
		/// </summary>
		public static string FindAssemblyInNetGac (DomAssemblyName reference)
		{
			// without public key, it can't be in the GAC
			if (reference.PublicKeyToken == null)
				return null;
			
			for (int i = 0; i < 2; i++) {
				for (int j = 0; j < gacs.Length; j++) {
					var gac = Path.Combine (gac_paths [i], gacs [j]);
					var file = GetAssemblyFile (reference, prefixes [i], gac);
					if (File.Exists (file))
						return file;
				}
			}

			return null;
		}
		
		static string GetAssemblyFile (DomAssemblyName reference, string prefix, string gac)
		{
			var gac_folder = new StringBuilder ()
				.Append (prefix)
				.Append (reference.Version)
				.Append ("__");

			gac_folder.Append (reference.PublicKeyToken);

			return Path.Combine (
				Path.Combine (
					Path.Combine (gac, reference.ShortName), gac_folder.ToString ()),
				reference.ShortName + ".dll");
		}
		#endregion
	}
}
