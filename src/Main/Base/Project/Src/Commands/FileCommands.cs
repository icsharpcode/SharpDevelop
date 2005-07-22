// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class CreateNewFile : AbstractMenuCommand
	{
		public override void Run()
		{
			using (NewFileDialog nfd = new NewFileDialog(null)) {
				nfd.Owner = (Form)WorkbenchSingleton.Workbench;
				nfd.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
			}
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.SelectWindow();
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
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window != null) {
				if (window.ViewContent.IsViewOnly) {
					return;
				}
				
				if (window.ViewContent.FileName == null) {
					SaveFileAs sfa = new SaveFileAs();
					sfa.Run();
				} else {
					FileAttributes attr = FileAttributes.ReadOnly | FileAttributes.Directory | FileAttributes.Offline | FileAttributes.System;
					if (File.Exists(window.ViewContent.FileName) && (File.GetAttributes(window.ViewContent.FileName) & attr) != 0) {
						SaveFileAs sfa = new SaveFileAs();
						sfa.Run();
					} else {
						
						
						ProjectService.MarkFileDirty(window.ViewContent.FileName);
						FileUtility.ObservedSave(new FileOperationDelegate(window.ViewContent.Save), window.ViewContent.FileName, FileErrorPolicy.ProvideAlternative);
					}
				}
			}
		}
	}
	
	public class ReloadFile : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window != null && window.ViewContent.FileName != null && !window.ViewContent.IsViewOnly) {
				
				if (MessageService.AskQuestion("${res:ICSharpCode.SharpDevelop.Commands.ReloadFile.ReloadFileQuestion}")) {
					Properties memento = null;
					if (window.ViewContent is IMementoCapable) {
						memento = ((IMementoCapable)window.ViewContent).CreateMemento();
					}
					window.ViewContent.Load(window.ViewContent.FileName);
					if (memento != null) {
						((IMementoCapable)window.ViewContent).SetMemento(memento);
					}
				}
			}
		}
	}
	
	public class SaveFileAs : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window != null) {
				if (window.ViewContent.IsViewOnly) {
					return;
				}
				if (window.ViewContent is ICustomizedCommands) {
					if (((ICustomizedCommands)window.ViewContent).SaveAsCommand()) {
						return;
					}
				}
				using (SaveFileDialog fdiag = new SaveFileDialog()) {
					fdiag.OverwritePrompt = true;
					fdiag.AddExtension    = true;
					
					string[] fileFilters  = (string[])(AddInTree.GetTreeNode("/SharpDevelop/Workbench/FileFilter").BuildChildItems(this)).ToArray(typeof(string));
					fdiag.Filter          = String.Join("|", fileFilters);
					for (int i = 0; i < fileFilters.Length; ++i) {
						if (fileFilters[i].IndexOf(Path.GetExtension(window.ViewContent.FileName == null ? window.ViewContent.UntitledName : window.ViewContent.FileName)) >= 0) {
							fdiag.FilterIndex = i + 1;
							break;
						}
					}
					
					if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
						string fileName = fdiag.FileName;
						
						
						
						if (!FileUtility.IsValidFileName(fileName)) {
							
							
							MessageService.ShowMessage(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.SaveFile.InvalidFileNameError}", new string[,] {{"FileName", fileName}}));
							
							return;
						}
						
						if (FileUtility.ObservedSave(new NamedFileOperationDelegate(window.ViewContent.Save), fileName) == FileOperationResult.OK) {
							FileService.RecentOpen.AddLastFile(fileName);
							
							MessageService.ShowMessage(fileName, "${res:ICSharpCode.SharpDevelop.Commands.SaveFile.FileSaved}");
						}
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
				if (content.IsViewOnly) {
					continue;
				}
				
				if (content.FileName == null) {
					if (content is ICustomizedCommands) {
						if (((ICustomizedCommands)content).SaveAsCommand()) {
							continue;
						}
					} else {
						using (SaveFileDialog fdiag = new SaveFileDialog()) {
							fdiag.OverwritePrompt = true;
							fdiag.AddExtension    = true;
							
							fdiag.Filter          = String.Join("|", (string[])(AddInTree.GetTreeNode("/SharpDevelop/Workbench/FileFilter").BuildChildItems(null)).ToArray(typeof(string)));
						
							if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
								string fileName = fdiag.FileName;
								// currently useless, because the fdiag.FileName can't
								// handle wildcard extensions :(
								if (Path.GetExtension(fileName).StartsWith("?") || Path.GetExtension(fileName) == "*") {
									fileName = Path.ChangeExtension(fileName, "");
								}
								if (FileUtility.ObservedSave(new NamedFileOperationDelegate(content.Save), fileName) == FileOperationResult.OK) {
									
									MessageService.ShowMessage(fileName, "${res:ICSharpCode.SharpDevelop.Commands.SaveFile.FileSaved}");
								}
							}
						}
					}
				} else {
					FileUtility.ObservedSave(new FileOperationDelegate(content.Save), content.FileName);
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
				
				string[] fileFilters  = (string[])(AddInTree.GetTreeNode("/SharpDevelop/Workbench/FileFilter").BuildChildItems(this)).ToArray(typeof(string));
				fdiag.Filter          = String.Join("|", fileFilters);
				bool foundFilter      = false;
				// search filter like in the current selected project
				// TODO: remove duplicate code (FolderNodeCommands has the same)
				
				IProject project = ProjectService.CurrentProject;
				
				if (project != null) {
					LanguageBindingDescriptor languageCodon = LanguageBindingService.GetCodonPerLanguageName(project.Language);
					
					for (int i = 0; !foundFilter && i < fileFilters.Length; ++i) {
						for (int j = 0; !foundFilter && j < languageCodon.Supportedextensions.Length; ++j) {
							if (fileFilters[i].IndexOf(languageCodon.Supportedextensions[j]) >= 0) {
								fdiag.FilterIndex = i + 1;
								foundFilter       = true;
								break;
							}
						}
					}
				}
				
				// search filter like in the current open file
				if (!foundFilter) {
					IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
					if (window != null) {
						for (int i = 0; i < fileFilters.Length; ++i) {
							if (fileFilters[i].IndexOf(Path.GetExtension(window.ViewContent.FileName == null ? window.ViewContent.UntitledName : window.ViewContent.FileName)) >= 0) {
								fdiag.FilterIndex = i + 1;
								break;
							}
						}
					}
				}
				
				fdiag.Multiselect     = true;
				fdiag.CheckFileExists = true;
				
				if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					foreach (string name in fdiag.FileNames) {
						FileService.OpenFile(name);
					}
				}
			}
		}
	}
	public class ExitWorkbenchCommand : AbstractMenuCommand
	{
		public override void Run()
		{			
			((Form)WorkbenchSingleton.Workbench).Close();
		}
	}
	
	public class Print : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window != null) {
				if (window.ViewContent is IPrintable) {
					PrintDocument pdoc = ((IPrintable)window.ViewContent).PrintDocument;
					if (pdoc != null) {
						using (PrintDialog ppd = new PrintDialog()) {
							ppd.Document  = pdoc;
							ppd.AllowSomePages = true;
							if (ppd.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) { // fixed by Roger Rubin
								pdoc.Print();
							}
						}
					} else {
						
						MessageService.ShowError("${res:ICSharpCode.SharpDevelop.Commands.Print.CreatePrintDocumentError}");
					}
				} else {
					
					MessageService.ShowError("${res:ICSharpCode.SharpDevelop.Commands.Print.CantPrintWindowContentError}");
				}
			}
		}
	}
	
	public class PrintPreview : AbstractMenuCommand
	{
		public override void Run()
		{
			try {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				
				if (window != null) {
					if (window.ViewContent is IPrintable) {
						using (PrintDocument pdoc = ((IPrintable)window.ViewContent).PrintDocument) {
							if (pdoc != null) {
								PrintPreviewDialog ppd = new PrintPreviewDialog();
								ppd.Owner     = (Form)WorkbenchSingleton.Workbench;
								ppd.TopMost   = true;
								ppd.Document  = pdoc;
								ppd.Show();
							} else {
								
								MessageService.ShowError("${res:ICSharpCode.SharpDevelop.Commands.Print.CreatePrintDocumentError}");
							}
						}
					}
				}
			} catch (System.Drawing.Printing.InvalidPrinterException) {
			}
		}
	}
	
	public class ClearRecentFiles : AbstractMenuCommand
	{
		public override void Run()
		{			
			try {
				
				FileService.RecentOpen.ClearRecentFiles();
			} catch {}
		}
	}
	
	public class ClearRecentProjects : AbstractMenuCommand
	{
		public override void Run()
		{			
			try {
				
				FileService.RecentOpen.ClearRecentProjects();
			} catch {}
		}
	}
}
