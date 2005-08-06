// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Core
{
	public class FileService
	{
		static string currentFile;
		static RecentOpen       recentOpen = null;
		
		public static RecentOpen RecentOpen {
			get {
				if (recentOpen == null) {
					recentOpen = RecentOpen.FromXmlElement(PropertyService.Get("RecentOpen", new Properties()));
				}
				return recentOpen;
			}
		}
		public static void Unload()
		{
			PropertyService.Set("RecentOpen", RecentOpen.ToProperties());
		}
		public static string CurrentFile {
			get {
				return currentFile;
			}
			set {
				currentFile = value;
			}
		}
		
		static FileService()
		{
			ProjectService.SolutionLoaded += ProjectServiceSolutionLoaded;
		}
		
		static void ProjectServiceSolutionLoaded(object sender, SolutionEventArgs e)
		{
			RecentOpen.AddLastProject(e.Solution.FileName);
		}
		
		public static bool CheckFileName(string fileName)
		{
			if (FileUtility.IsValidFileName(fileName))
				return true;
			MessageService.ShowMessage(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.SaveFile.InvalidFileNameError}", new string[,] {{"FileName", fileName}}));
			return false;
		}
		
		class LoadFileWrapper
		{
			IDisplayBinding binding;
			
			public LoadFileWrapper(IDisplayBinding binding)
			{
				this.binding = binding;
			}
			
			public void Invoke(string fileName)
			{
				IViewContent newContent = binding.CreateContentForFile(fileName);
				WorkbenchSingleton.Workbench.ShowView(newContent);
				DisplayBindingService.AttachSubWindows(newContent.WorkbenchWindow);
			}
		}
		
		public static bool IsOpen(string fileName)
		{
			return GetOpenFile(fileName) != null;
		}
		
		public static IWorkbenchWindow OpenFile(string fileName)
		{
			// test, if file fileName exists
			bool isURL = fileName.IndexOf("://") > 0;
			if (!isURL) {
				System.Diagnostics.Debug.Assert(FileUtility.IsValidFileName(fileName));
				
				// test, if an untitled file should be opened
				if (!Path.IsPathRooted(fileName)) {
					foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
						if (content.IsUntitled && content.UntitledName == fileName) {
							content.WorkbenchWindow.SelectWindow();
							return content.WorkbenchWindow;
						}
					}
				} else if (!FileUtility.TestFileExists(fileName)) {
					return null;
				}
			}
			
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (content.FileName != null) {
					try {
						if (isURL ? content.FileName == fileName : FileUtility.IsEqualFileName(content.FileName, fileName)) {
							content.WorkbenchWindow.SelectWindow();
							return content.WorkbenchWindow;
						}
					} catch (Exception) {
						// TODO: what kind of exception is ignored here?
					}
				}
				if (content.WorkbenchWindow == null || content.WorkbenchWindow.SubViewContents == null)
					continue;
				foreach(object subViewContent in content.WorkbenchWindow.SubViewContents) {
					IViewContent viewContent = subViewContent as IViewContent;
					if (viewContent != null && viewContent.FileName != null) {
						try {
							if (isURL ? viewContent.FileName == fileName : FileUtility.IsEqualFileName(viewContent.FileName, fileName)) {
								viewContent.WorkbenchWindow.SelectWindow();
								return content.WorkbenchWindow;
							}
						} catch (Exception) {
							// TODO: what kind of exception is ignored here?
						}
					}
				}
			}
			
			IDisplayBinding binding = DisplayBindingService.GetBindingPerFileName(fileName);
			
			if (binding != null) {
				if (FileUtility.ObservedLoad(new NamedFileOperationDelegate(new LoadFileWrapper(binding).Invoke), fileName) == FileOperationResult.OK) {
					FileService.RecentOpen.AddLastFile(fileName);
				}
			} else {
				throw new ApplicationException("Can't open " + fileName + ", no display codon found.");
			}
			return GetOpenFile(fileName);
		}
		
		public static IWorkbenchWindow NewFile(string defaultName, string language, string content)
		{
			IDisplayBinding binding = DisplayBindingService.GetBindingPerLanguageName(language);
			
			if (binding != null) {
				IViewContent newContent = binding.CreateContentForLanguage(language, content);
				if (newContent == null) {
					throw new ApplicationException(String.Format("Created view content was null{3}DefaultName:{0}{3}Language:{1}{3}Content:{2}", defaultName, language, content, Environment.NewLine));
				}
				newContent.UntitledName = defaultName;
				newContent.IsDirty      = false;
				WorkbenchSingleton.Workbench.ShowView(newContent);
				
				DisplayBindingService.AttachSubWindows(newContent.WorkbenchWindow);
				return newContent.WorkbenchWindow;
			} else {
				throw new ApplicationException("Can't create display binding for language " + language);
			}
		}
		
		public static IWorkbenchWindow GetOpenFile(string fileName)
		{
			if (fileName != null && fileName.Length > 0) {
				string normalizedFileName = (fileName.StartsWith("http://") ? fileName : Path.IsPathRooted(fileName) ? Path.GetFullPath(fileName) : fileName).ToLower();
				foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
					string normalizedContentName = content.IsUntitled ? content.UntitledName : (content.FileName == null ? "" : (content.FileName.StartsWith("http://") ? content.FileName : Path.GetFullPath(content.FileName)));
					normalizedContentName = normalizedContentName.ToLower();
					
					if (normalizedContentName == normalizedFileName) {
						return content.WorkbenchWindow;
					}
					if (content.WorkbenchWindow == null || content.WorkbenchWindow.SubViewContents == null)
						continue;
					foreach(object subViewContent in content.WorkbenchWindow.SubViewContents) {
						IViewContent viewContent = subViewContent as IViewContent;
						if (viewContent != null && viewContent.FileName != null) {
							string normalizedViewContentName = viewContent.IsUntitled ? viewContent.UntitledName : (viewContent.FileName == null ? "" : (viewContent.FileName.StartsWith("http://") ? viewContent.FileName : Path.GetFullPath(viewContent.FileName)));
							normalizedViewContentName = normalizedViewContentName.ToLower();
							
							if (normalizedViewContentName == normalizedFileName) {
								return content.WorkbenchWindow;
							}
						}
					}
				}
			}
			return null;
		}
		
		public static void RemoveFile(string fileName, bool isDirectory)
		{
			FileCancelEventArgs eargs = new FileCancelEventArgs(fileName, isDirectory);
			OnFileRemoving(eargs);
			if (eargs.Cancel)
				return;
			if (!eargs.OperationAlreadyDone) {
				if (isDirectory) {
					try {
						if (Directory.Exists(fileName)) {
							Directory.Delete(fileName, true);
						}
					} catch (Exception e) {
						MessageService.ShowError(e, "Can't remove directory " + fileName);
//					return;
					}
				} else {
					try {
						if (File.Exists(fileName)) {
							File.Delete(fileName);
						}
					} catch (Exception e) {
						MessageService.ShowError(e, "Can't remove file " + fileName);
//					return;
					}
				}
			}
			OnFileRemoved(new FileEventArgs(fileName, isDirectory));
		}
		
		public static void RenameFile(string oldName, string newName, bool isDirectory)
		{
			FileRenamingEventArgs eargs = new FileRenamingEventArgs(oldName, newName, isDirectory);
			OnFileRenaming(eargs);
			if (eargs.Cancel)
				return;
			if (!eargs.OperationAlreadyDone) {
				try {
					if (isDirectory) {
						if (Directory.Exists(oldName)) {
							Directory.Move(oldName, newName);
						}
					} else {
						if (File.Exists(oldName)) {
							File.Move(oldName, newName);
						}
					}
				} catch (Exception e) {
					if (isDirectory) {
						MessageService.ShowError(e, "Can't rename directory " + oldName);
					} else {
						MessageService.ShowError(e, "Can't rename file " + oldName);
					}
					return;
				}
			}
			OnFileRenamed(new FileRenameEventArgs(oldName, newName, isDirectory));
		}
		
		public static IViewContent JumpToFilePosition(string fileName, int line, int column)
		{
			if (fileName == null || fileName.Length == 0) {
				return null;
			}
			OpenFile(fileName);
			IWorkbenchWindow window = GetOpenFile(fileName);
			if (window == null) {
				return null;
			}
			IViewContent content = window.ViewContent;
			if (content.WorkbenchWindow.SubViewContents == null) {
				if (content is IPositionable) {
					window.SwitchView(0);
					((IPositionable)content).JumpTo(Math.Max(0, line), Math.Max(0, column));
				}
				return content;
			}
			else
			{
				int i = 0;
				foreach(object subViewContent in content.WorkbenchWindow.SubViewContents) {
					IViewContent viewContent = subViewContent as IViewContent;
					if (viewContent != null && viewContent.FileName != null) {
						try {
							if (FileUtility.IsEqualFileName(viewContent.FileName, fileName)) {
								if (viewContent is IPositionable) {
									window.SwitchView(i);
									((IPositionable)viewContent).JumpTo(Math.Max(0, line), Math.Max(0, column));
								}
								return viewContent;
							}
						} catch (Exception) {
						}
					}
					i++;
				}
			}
			return null;
		}
		
		static void OnFileRemoved(FileEventArgs e)
		{
			if (FileRemoved != null) {
				FileRemoved(null, e);
			}
		}
		
		static void OnFileRemoving(FileCancelEventArgs e)
		{
			if (FileRemoving != null) {
				FileRemoving(null, e);
			}
		}
		
		static void OnFileRenamed(FileRenameEventArgs e)
		{
			if (FileRenamed != null) {
				FileRenamed(null, e);
			}
		}
		
		static void OnFileRenaming(FileRenamingEventArgs e) {
			if (FileRenaming != null) {
				FileRenaming(null, e);
			}
		}
		
		public static event EventHandler<FileRenamingEventArgs> FileRenaming;
		public static event EventHandler<FileRenameEventArgs> FileRenamed;
		
		public static event EventHandler<FileCancelEventArgs> FileRemoving;
		public static event EventHandler<FileEventArgs> FileRemoved;
	}
}
