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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class CreateNewFile : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectNode node = ProjectBrowserPad.Instance.CurrentProject;
			if (node != null) {
				if (node.Project.IsReadOnly)
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
			SD.UIService.ShowNewFileDialog(null, null);
		}
	}
	
	public class CloseFile : AbstractMenuCommand
	{
		public override void Run()
		{
			if (SD.Workbench.ActiveWorkbenchWindow != null) {
				SD.Workbench.ActiveWorkbenchWindow.CloseWindow(false);
			}
		}
	}

	public class SaveFile : AbstractMenuCommand
	{
		public override void Run()
		{
			Save(SD.Workbench.ActiveWorkbenchWindow);
		}
		
		internal static void Save(IWorkbenchWindow window)
		{
			foreach (var vc in window.ViewContents)
				Save(vc);
		}
		
		internal static void Save(IViewContent content)
		{
			if (content != null && content.IsDirty) {
				var customizedCommands = content.GetService<ICustomizedCommands>();
				if (customizedCommands != null && customizedCommands.SaveCommand()) {
					return;
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
			IViewContent content = SD.Workbench.ActiveViewContent;
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
			Save(SD.Workbench.ActiveWorkbenchWindow);
		}
		
		internal static void Save(IWorkbenchWindow window)
		{
			List<IViewContent> remainingViewContents = new List<IViewContent>();
			
			foreach (IViewContent content in window.ViewContents) {
				// try to run customized Save As Command, exclude ViewContent if successful
				var customizedCommands = content.GetService<ICustomizedCommands>();
				if (customizedCommands != null && customizedCommands.SaveAsCommand())
					continue;
				// exclude view only ViewContents
				if (content.IsViewOnly)
					continue;
				
				remainingViewContents.Add(content);
			}
			
			// save remaining files once (display Save As dialog)
			var files = remainingViewContents.SelectMany(content => content.Files).Distinct();
			
			foreach (var file in files)
				Save(file);
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
				
				if (fdiag.ShowDialog(SD.WinForms.MainWin32Window) == DialogResult.OK) {
					FileName fileName = FileName.Create(fdiag.FileName);
					if (!FileService.CheckFileName(fileName)) {
						return;
					}
					if (FileUtility.ObservedSave(new NamedFileOperationDelegate(file.SaveToDisk), fileName) == FileOperationResult.OK) {
						SD.FileService.RecentOpen.AddRecentFile(fileName);
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
			foreach (IViewContent content in SD.Workbench.ViewContentCollection) {
				var customizedCommands = content.GetService<ICustomizedCommands>();
				if (customizedCommands != null && content.IsDirty) {
					customizedCommands.SaveCommand();
				}
			}
			foreach (OpenedFile file in SD.FileService.OpenedFiles) {
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
				
				fdiag.Filter = ProjectService.GetAllFilesFilter();
				fdiag.FilterIndex     = 0;
				fdiag.Multiselect     = true;
				fdiag.CheckFileExists = true;
				
				if (fdiag.ShowDialog(SD.WinForms.MainWin32Window) == DialogResult.OK) {
					OpenFiles(Array.ConvertAll(fdiag.FileNames, FileName.Create));
				}
			}
		}
		
		protected virtual void OpenFiles(FileName[] fileNames)
		{
			foreach (var name in fileNames) {
				SD.FileService.OpenFile(name);
			}
		}
	}
	
	public class OpenFileWith : OpenFile
	{
		protected override void OpenFiles(FileName[] fileNames)
		{
			SD.FileService.ShowOpenWithDialog(fileNames);
		}
	}

	public class ExitWorkbenchCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			SD.Workbench.MainWindow.Close();
		}
	}

	public class ClearRecentFiles : AbstractMenuCommand
	{
		public override void Run()
		{
			SD.FileService.RecentOpen.ClearRecentFiles();
		}
	}

	public class ClearRecentProjects : AbstractMenuCommand
	{
		public override void Run()
		{
			SD.FileService.RecentOpen.ClearRecentProjects();
		}
	}
}
