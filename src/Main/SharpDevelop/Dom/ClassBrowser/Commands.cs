// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor.Commands;
using ICSharpCode.SharpDevelop.Parser;
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
			IAssemblyReferenceModel assemblyReferenceModel = parameter as IAssemblyReferenceModel;
			return ((assemblyModel != null) && assemblyModel.Context.IsValid) || (assemblyReferenceModel != null);
		}
		
		public override void Execute(object parameter)
		{
			var classBrowser = SD.GetService<IClassBrowser>();
			if (classBrowser != null) {
				IAssemblyModel assemblyModel = parameter as IAssemblyModel;
				if (assemblyModel == null) {
					IAssemblyReferenceModel assemblyReference = parameter as IAssemblyReferenceModel;
					if (assemblyReference == null) {
						// Neither assembly model, nor a assembly reference model
						return;
					}
					
					// Model is an assembly reference
					IAssemblyParserService assemblyParserService = SD.GetRequiredService<IAssemblyParserService>();
					DefaultAssemblySearcher searcher = new DefaultAssemblySearcher(assemblyReference.ParentAssemblyModel.Location);
					var resolvedFile = searcher.FindAssembly(assemblyReference.AssemblyName);
					assemblyModel = assemblyParserService.GetAssemblyModelSafe(resolvedFile);
				}
				
				if (assemblyModel != null) {
					// Try to remove AssemblyModel from list of UnpinnedAssemblies
					classBrowser.UnpinnedAssemblies.Assemblies.RemoveAll(a => a.FullAssemblyName == assemblyModel.FullAssemblyName);
					
					if (!classBrowser.MainAssemblyList.Assemblies.Contains(assemblyModel))
						classBrowser.MainAssemblyList.Assemblies.Add(assemblyModel);
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
			DebuggerService.CurrentDebugger.Start(new ProcessStartInfo {
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
}
