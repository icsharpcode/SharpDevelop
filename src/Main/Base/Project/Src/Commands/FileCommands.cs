// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class CreateNewFile : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectNode node = ProjectBrowserPad.Instance.CurrentProject;
			if (node != null) {
				if (node.Project.ReadOnly)
				{
					MessageService.ShowWarningFormatted("${res:Dialog.NewFile.ReadOnlyProjectWarning}", node.Project.FileName);
				}
				else
				{
					int result = MessageService.ShowCustomDialog("${res:Dialog.NewFile.AddToProjectQuestionTitle}",
					                                             "${res:Dialog.NewFile.AddToProjectQuestion}",
					                                             "${res:Dialog.NewFile.AddToProjectQuestionProject}",
					                                             "${res:Dialog.NewFile.AddToProjectQuestionStandalone}");
					if (result == 0) {
						node.AddNewItemsToProject();
						return;
					} else if (result == -1) {
						return;
					}
				}
				
			}
			using (NewFileDialog nfd = new NewFileDialog(null)) {
				nfd.ShowDialog(WorkbenchSingleton.MainWin32Window);
			}
		}
	}
	
	public class CloseFile : AbstractMenuCommand
	{
		public override void Run()
		{
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.CloseWindow(false);
			}
		}
	}

	public class SaveFile : AbstractMenuCommand
	{
		public override void Run()
		{
			Save(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow);
		}
		
		internal static void Save(IWorkbenchWindow window)
		{
			window.ViewContents.ForEach(Save);
		}
		
		internal static void Save(IViewContent content)
		{
			if (content != null && content.IsDirty) {
				if (content is ICustomizedCommands) {
					if (((ICustomizedCommands)content).SaveCommand()) {
						return;
					}
				}
				if (content.IsViewOnly) {
					return;
				}
				
				foreach (OpenedFile file in content.Files.ToArray()) {
					if (file.IsDirty)
						Save(file);
				}
			}
		}
		
		public static void Save(OpenedFile file)
		{
			if (file.IsUntitled) {
				SaveFileAs.Save(file);
			} else {
				FileAttributes attr = FileAttributes.ReadOnly | FileAttributes.Directory | FileAttributes.Offline | FileAttributes.System;
				if (File.Exists(file.FileName) && (File.GetAttributes(file.FileName) & attr) != 0) {
					SaveFileAs.Save(file);
				} else {
					FileUtility.ObservedSave(new NamedFileOperationDelegate(file.SaveToDisk), file.FileName, FileErrorPolicy.ProvideAlternative);
				}
			}
		}
	}
	
	public class ReloadFile : AbstractMenuCommand
	{
		public override void Run()
		{
			IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
			if (content == null)
				return;
			OpenedFile file = content.PrimaryFile;
			if (file == null || file.IsUntitled)
				return;
			if (file.IsDirty == false
			    || MessageService.AskQuestion("${res:ICSharpCode.SharpDevelop.Commands.ReloadFile.ReloadFileQuestion}"))
			{
				try
				{
					file.ReloadFromDisk();
				}
				catch(FileNotFoundException)
				{
					MessageService.ShowWarning("${res:ICSharpCode.SharpDevelop.Commands.ReloadFile.FileDeletedMessage}");
					return;
				}
			}
		}
	}
	
	public class SaveFileAs : AbstractMenuCommand
	{
		public override void Run()
		{
			Save(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow);
		}
		
		internal static void Save(IWorkbenchWindow window)
		{
			List<IViewContent> remainingViewContents = new List<IViewContent>();
			
			foreach (IViewContent content in window.ViewContents) {
				// try to run customized Save As Command, exclude ViewContent if successful
				if (content is ICustomizedCommands && (content as ICustomizedCommands).SaveAsCommand())
					continue;
				// exclude view only ViewContents
				if (content.IsViewOnly)
					continue;
				
				remainingViewContents.Add(content);
			}
			
			// save remaining files once (display Save As dialog)
			var files = remainingViewContents.SelectMany(content => content.Files).Distinct();
			
			files.ForEach(Save);
		}
		
		internal static void Save(OpenedFile file)
		{
			Debug.Assert(file != null);
			
			using (SaveFileDialog fdiag = new SaveFileDialog()) {
				fdiag.OverwritePrompt = true;
				fdiag.AddExtension    = true;
				
				var fileFilters = ProjectService.GetFileFilters();
				fdiag.Filter = String.Join("|", fileFilters);
				for (int i = 0; i < fileFilters.Count; ++i) {
					if (fileFilters[i].ContainsExtension(Path.GetExtension(file.FileName))) {
						fdiag.FilterIndex = i + 1;
						break;
					}
				}
				
				if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainWin32Window) == DialogResult.OK) {
					string fileName = fdiag.FileName;
					if (!FileService.CheckFileName(fileName)) {
						return;
					}
					if (FileUtility.ObservedSave(new NamedFileOperationDelegate(file.SaveToDisk), fileName) == FileOperationResult.OK) {
						FileService.RecentOpen.AddLastFile(fileName);
						MessageService.ShowMessage(fileName, "${res:ICSharpCode.SharpDevelop.Commands.SaveFile.FileSaved}");
					}
				}
			}
		}
	}
	
	public class SaveAllFiles : AbstractMenuCommand
	{
		public static void SaveAll()
		{
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (content is ICustomizedCommands && content.IsDirty) {
					((ICustomizedCommands)content).SaveCommand();
				}
			}
			foreach (OpenedFile file in FileService.OpenedFiles) {
				if (file.IsDirty) {
					SaveFile.Save(file);
				}
			}
		}
		
		public override void Run()
		{
			SaveAll();
		}
	}
	
	public class OpenFile : AbstractMenuCommand
	{
		public override void Run()
		{
			using (OpenFileDialog fdiag  = new OpenFileDialog()) {
				fdiag.AddExtension    = true;
				
				var fileFilters  = ProjectService.GetFileFilters();
				fdiag.Filter     = String.Join("|", fileFilters);
				bool foundFilter = false;
				
				// search filter like in the current open file
				if (!foundFilter) {
					IViewContent content = WorkbenchSingleton.Workbench.ActiveViewContent;
					if (content != null) {
						string extension = Path.GetExtension(content.PrimaryFileName);
						if (string.IsNullOrEmpty(extension) == false) {
							for (int i = 0; i < fileFilters.Count; ++i) {
								if (fileFilters[i].ContainsExtension(extension)) {
									fdiag.FilterIndex = i + 1;
									foundFilter = true;
									break;
								}
							}
						}
					}
				}
				
				if (!foundFilter) {
					fdiag.FilterIndex = fileFilters.Count;
				}
				
				fdiag.Multiselect     = true;
				fdiag.CheckFileExists = true;
				
				if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainWin32Window) == DialogResult.OK) {
					OpenFiles(fdiag.FileNames);
				}
			}
		}
		
		protected virtual void OpenFiles(string[] fileNames)
		{
			foreach (string name in fileNames) {
				FileService.OpenFile(name);
			}
		}
	}
	
	public class OpenFileWith : OpenFile
	{
		protected override void OpenFiles(string[] fileNames)
		{
			OpenFilesWith(fileNames);
		}
		
		/// <summary>
		/// Shows the OpenWith dialog for the specified files.
		/// </summary>
		public static void OpenFilesWith(string[] fileNames)
		{
			if (fileNames.Length == 0)
				return;
			
			List<DisplayBindingDescriptor> codons = DisplayBindingService.GetCodonsPerFileName(fileNames[0]).ToList();
			for (int i = 1; i < fileNames.Length; i++) {
				var codonsForThisFile = DisplayBindingService.GetCodonsPerFileName(fileNames[i]);
				codons.RemoveAll(c => !codonsForThisFile.Contains(c));
			}
			if (codons.Count == 0)
				return;
			
			int defaultCodonIndex = codons.IndexOf(DisplayBindingService.GetDefaultCodonPerFileName(fileNames[0]));
			if (defaultCodonIndex < 0)
				defaultCodonIndex = 0;
			using (OpenWithDialog dlg = new OpenWithDialog(codons, defaultCodonIndex, Path.GetExtension(fileNames[0]))) {
				if (dlg.ShowDialog(WorkbenchSingleton.MainWin32Window) == DialogResult.OK) {
					foreach (string fileName in fileNames) {
						FileUtility.ObservedLoad(new FileService.LoadFileWrapper(dlg.SelectedBinding.Binding, true).Invoke, fileName);
					}
				}
			}
		}
	}

	public class ExitWorkbenchCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			WorkbenchSingleton.MainWindow.Close();
		}
	}

	public class ClearRecentFiles : AbstractMenuCommand
	{
		public override void Run()
		{
			FileService.RecentOpen.ClearRecentFiles();
		}
	}

	public class ClearRecentProjects : AbstractMenuCommand
	{
		public override void Run()
		{
			FileService.RecentOpen.ClearRecentProjects();
		}
	}
}
