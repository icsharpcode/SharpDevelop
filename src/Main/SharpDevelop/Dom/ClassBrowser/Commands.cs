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
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor.Commands;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	/// <summary>
	/// OpenAssemblyFromFileCommand.
	/// </summary>
	class OpenAssemblyFromFileCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			var classBrowser = SD.GetService<IClassBrowser>();
			var modelFactory = SD.GetService<IModelFactory>();
			if ((classBrowser != null) && (modelFactory != null)) {
				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.Filter = "Assembly files (*.exe, *.dll)|*.exe;*.dll";
				openFileDialog.CheckFileExists = true;
				openFileDialog.CheckPathExists = true;
				if (openFileDialog.ShowDialog() ?? false) {
					IAssemblyModel assemblyModel = SD.AssemblyParserService.GetAssemblyModelSafe(new ICSharpCode.Core.FileName(openFileDialog.FileName), true);
					if (assemblyModel != null)
						classBrowser.MainAssemblyList.Assemblies.Add(assemblyModel);
				}
			}
		}
	}
	
	/// <summary>
	/// OpenAssemblyFromGACCommand.
	/// </summary>
	class OpenAssemblyFromGACCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			var classBrowser = SD.GetService<IClassBrowser>();
			var modelFactory = SD.GetService<IModelFactory>();
			if ((classBrowser != null) && (modelFactory != null)) {
				OpenFromGacDialog gacDialog = new OpenFromGacDialog();
				if (gacDialog.ShowDialog() ?? false)
				{
					foreach (string assemblyFile in gacDialog.SelectedFileNames) {
						IAssemblyModel assemblyModel = SD.AssemblyParserService.GetAssemblyModelSafe(new ICSharpCode.Core.FileName(assemblyFile), true);
						if (assemblyModel != null)
							classBrowser.MainAssemblyList.Assemblies.Add(assemblyModel);
					}
				}
			}
		}
	}
	
	/// <summary>
	/// PermanentlyAddModuleToWorkspaceCommand.
	/// </summary>
	class PermanentlyAddModuleToWorkspaceCommand : SimpleCommand
	{
		public override bool CanExecute(object parameter)
		{
			IAssemblyModel assemblyModel = parameter as IAssemblyModel;
			return ((assemblyModel != null) && assemblyModel.Context.IsValid) || (parameter is IProject) || (parameter is IAssemblyReferenceModel);
		}
		
		public override void Execute(object parameter)
		{
			var classBrowser = SD.GetService<IClassBrowser>();
			if (classBrowser != null) {
				IAssemblyModel assemblyModel = parameter as IAssemblyModel;
				if (assemblyModel == null) {
					// Node is a project?
					IProject project = parameter as IProject;
					if (project != null) {
						assemblyModel = project.AssemblyModel;
					}
				}
				
				if (assemblyModel == null) {
					// Node is an assembly reference?
					IAssemblyReferenceModel assemblyReference = parameter as IAssemblyReferenceModel;
					if (assemblyReference != null) {
						// Model is an assembly reference
						IAssemblyParserService assemblyParserService = SD.GetRequiredService<IAssemblyParserService>();
						IEntityModelContext entityModelContext = assemblyReference.ParentAssemblyModel.Context;
						CombinedAssemblySearcher searcher = new CombinedAssemblySearcher();
						if ((entityModelContext != null) && (entityModelContext.Project != null)) {
							searcher.AddSearcher(new ProjectAssemblyReferenceSearcher(entityModelContext.Project));
						}
						searcher.AddSearcher(new DefaultAssemblySearcher(assemblyReference.ParentAssemblyModel.Location));
						var resolvedFile = searcher.FindAssembly(assemblyReference.AssemblyName);
						if (resolvedFile != null) {
							assemblyModel = assemblyParserService.GetAssemblyModelSafe(resolvedFile);
						} else {
							// Assembly file not resolvable
							SD.MessageService.ShowWarningFormatted("Could not resolve reference '{0}'.", assemblyReference.AssemblyName.ShortName);
						}
					}
				}
				
				if (assemblyModel != null) {
					// Try to remove AssemblyModel from list of UnpinnedAssemblies
					classBrowser.UnpinnedAssemblies.Assemblies.RemoveAll(a => a.FullAssemblyName == assemblyModel.FullAssemblyName);
					
					if (!classBrowser.MainAssemblyList.Assemblies.Contains(assemblyModel))
						classBrowser.MainAssemblyList.Assemblies.Add(assemblyModel);
					
					// Bring the node into view
					classBrowser.GotoAssemblyModel(assemblyModel);
				}
			}
		}
	}
	
	/// <summary>
	/// RemoveAssemblyCommand.
	/// </summary>
	class RemoveAssemblyCommand : SimpleCommand
	{
		public override bool CanExecute(object parameter)
		{
			return parameter is AssemblyModel;
		}
		
		public override void Execute(object parameter)
		{
			var classBrowser = SD.GetService<IClassBrowser>();
			if (classBrowser != null) {
				IAssemblyModel assemblyModel = (IAssemblyModel) parameter;
				if (assemblyModel.IsUnpinned()) {
					classBrowser.UnpinnedAssemblies.Assemblies.Remove(assemblyModel);
				} else {
					classBrowser.MainAssemblyList.Assemblies.Remove(assemblyModel);
				}
			}
		}
	}
	
	/// <summary>
	/// RunAssemblyWithDebuggerCommand.
	/// </summary>
	class RunAssemblyWithDebuggerCommand : SimpleCommand
	{
		public override bool CanExecute(object parameter)
		{
			IAssemblyModel assemblyModel = parameter as IAssemblyModel;
			return (assemblyModel != null) && assemblyModel.Context.IsValid;
		}
		
		public override void Execute(object parameter)
		{
			IAssemblyModel assemblyModel = (IAssemblyModel) parameter;
			
			// Start debugger with given assembly
			SD.Debugger.Start(new ProcessStartInfo {
			                  	FileName = assemblyModel.Context.Location,
			                  	WorkingDirectory = Path.GetDirectoryName(assemblyModel.Context.Location)
			                  });
		}
	}
	
	/// <summary>
	/// OpenInClassBrowserCommand.
	/// </summary>
	class OpenInClassBrowserCommand : ResolveResultMenuCommand
	{
		public override void Run(ResolveResult symbol)
		{
			var classBrowser = SD.GetService<IClassBrowser>();
			var entity = GetSymbol(symbol) as IEntity;
			if ((classBrowser != null) && (entity != null)) {
				classBrowser.GoToEntity(entity);
			}
		}
	}
	
	/// <summary>
	/// AddProjectReferenceCommand
	/// </summary>
	class AddProjectReferenceCommand : SimpleCommand
	{
		public override bool CanExecute(object parameter)
		{
			var assemblyReferencesModel = parameter as IAssemblyReferencesModel;
			if (assemblyReferencesModel != null) {
				IAssemblyModel assemblyModel = assemblyReferencesModel.ParentAssemblyModel;
				if ((assemblyModel != null) && (assemblyModel.Context != null)) {
					return assemblyModel.Context.Project != null;
				}
			}
			
			return false;
		}
		
		public override void Execute(object parameter)
		{
			IAssemblyReferencesModel assemblyReferencesModel = parameter as IAssemblyReferencesModel;
			if (assemblyReferencesModel != null) {
				IAssemblyModel assemblyModel = assemblyReferencesModel.ParentAssemblyModel;
				if ((assemblyModel != null) && (assemblyModel.Context != null)) {
					IProject project = (parameter != null) ? assemblyModel.Context.Project : ProjectService.CurrentProject;
					if (project == null) {
						return;
					}
					using (SelectReferenceDialog selDialog = new SelectReferenceDialog(project)) {
						if (selDialog.ShowDialog(SD.WinForms.MainWin32Window) == System.Windows.Forms.DialogResult.OK) {
							foreach (ReferenceProjectItem reference in selDialog.ReferenceInformations) {
								ProjectService.AddProjectItem(project, reference);
							}
							project.Save();
						}
					}
				}
			}
		}
	}
	/// <summary>
	/// RemoveProjectReferenceCommand
	/// </summary>
	class RemoveProjectReferenceCommand : SimpleCommand
	{
		public override bool CanExecute(object parameter)
		{
			var assemblyReferenceModel = parameter as IAssemblyReferenceModel;
			if (assemblyReferenceModel != null) {
				IAssemblyModel assemblyModel = assemblyReferenceModel.ParentAssemblyModel;
				if ((assemblyModel != null) && (assemblyModel.Context != null)) {
					return assemblyModel.Context.Project != null;
				}
			}
			
			return false;
		}
		
		public override void Execute(object parameter)
		{
			IAssemblyReferenceModel assemblyReferenceModel = parameter as IAssemblyReferenceModel;
			if (assemblyReferenceModel != null) {
				IAssemblyModel assemblyModel = assemblyReferenceModel.ParentAssemblyModel;
				if ((assemblyModel != null) && (assemblyModel.Context != null)) {
					IProject project = (parameter != null) ? assemblyModel.Context.Project : ProjectService.CurrentProject;
					if (project == null) {
						return;
					}
					
					ProjectItem referenceProjectItem =
						project.Items.FirstOrDefault(
							item => {
								if (item.ItemType == ItemType.COMReference) {
									// Special handling for COM references: Their assembly names are prefixed with "Interop."
									return assemblyReferenceModel.AssemblyName.ShortName == "Interop." + item.Include;
								}
								return (item.Include == assemblyReferenceModel.AssemblyName.ShortName) && ItemType.ReferenceItemTypes.Contains(item.ItemType);
							});
					if (referenceProjectItem != null) {
						ProjectService.RemoveProjectItem(referenceProjectItem.Project, referenceProjectItem);
						project.Save();
					}
				}
			}
		}
	}
}
