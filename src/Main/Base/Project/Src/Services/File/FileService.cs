// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
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
			ProjectService.SolutionLoaded += new SolutionEventHandler(ProjectServiceSolutionLoaded);
		}
		
		static void ProjectServiceSolutionLoaded(object sender, SolutionEventArgs e)
		{
			RecentOpen.AddLastProject(e.Solution.FileName);
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
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (content.IsUntitled) {
					if (content.UntitledName == fileName) {
						return true;
					}
				} else if (content.FileName == fileName) {
					return true;
				}
				if (content.WorkbenchWindow == null || content.WorkbenchWindow.SubViewContents == null)
					continue;
				foreach(object subViewContent in content.WorkbenchWindow.SubViewContents) {
					IViewContent viewContent = subViewContent as IViewContent;
					if (viewContent != null && viewContent.FileName != null) {
						try {
							if (Path.GetFullPath(viewContent.FileName.ToUpper()) == Path.GetFullPath(fileName.ToUpper())) {
								return true;
							}
						} catch (Exception) {
						}
					}
				}
			}
			return false;
		}
		public static void OpenFile(string fileName)
		{
			// test, if file fileName exists
			if (!fileName.StartsWith("http://")) {
				System.Diagnostics.Debug.Assert(FileUtility.IsValidFileName(fileName));
				
				// test, if an untitled file should be opened
				if (!Path.IsPathRooted(fileName)) {
					foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
						if (content.IsUntitled && content.UntitledName == fileName) {
							content.WorkbenchWindow.SelectWindow();
							return;
						}
					}
				} else if (!FileUtility.TestFileExists(fileName)) {
					return;
				}
			}
			
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				// WINDOWS DEPENDENCY : ToUpper()
				if (content.FileName != null) {
					try {
						if (fileName.StartsWith("http://") ? content.FileName == fileName : FileUtility.IsEqualFile(content.FileName, fileName)) {
							content.WorkbenchWindow.SelectWindow();
							return;
						}
					} catch (Exception) {
					}
				}
				if (content.WorkbenchWindow == null || content.WorkbenchWindow.SubViewContents == null)
					continue;
				foreach(object subViewContent in content.WorkbenchWindow.SubViewContents) {
					IViewContent viewContent = subViewContent as IViewContent;
					if (viewContent != null && viewContent.FileName != null) {
						try {
							if (fileName.StartsWith("http://") ? viewContent.FileName == fileName :
							    Path.GetFullPath(viewContent.FileName.ToUpper()) == Path.GetFullPath(fileName.ToUpper())) {
								viewContent.WorkbenchWindow.SelectWindow();
								return;
							}
						} catch (Exception) {
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
			OnFileRemoving(new FileEventArgs(fileName, isDirectory));
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
			OnFileRemoved(new FileEventArgs(fileName, isDirectory));
		}
		
		public static void RenameFile(string oldName, string newName, bool isDirectory)
		{
			OnFileRenaming(new FileRenameEventArgs(oldName, newName, isDirectory));
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
//				return;
			}
			OnFileRenamed(new FileRenameEventArgs(oldName, newName, isDirectory));
		}
		
		public static void JumpToFilePosition(string fileName, int line, int column)
		{
			if (fileName == null || fileName.Length == 0) {
				return;
			}
			OpenFile(fileName);
			IWorkbenchWindow window = GetOpenFile(fileName);
			if (window == null) {
				return;
			}
			IViewContent content = window.ViewContent;
			if (content.WorkbenchWindow.SubViewContents == null) {
				if (content is IPositionable) {
					window.SwitchView(0);
					((IPositionable)content).JumpTo(Math.Max(0, line), Math.Max(0, column));
				}
			}
			else
			{
				int i = 0;
				foreach(object subViewContent in content.WorkbenchWindow.SubViewContents) {
					IViewContent viewContent = subViewContent as IViewContent;
					if (viewContent != null && viewContent.FileName != null) {
						try {
							if (FileUtility.IsEqualFile(viewContent.FileName, fileName)) {
								if (viewContent is IPositionable) {
									window.SwitchView(i);
									((IPositionable)viewContent).JumpTo(Math.Max(0, line), Math.Max(0, column));
								}
							}
						} catch (Exception) {
						}
					}
					i++;
				}
			}
			
		}
		
		static void OnFileRemoved(FileEventArgs e)
		{
			if (FileRemoved != null) {
				FileRemoved(null, e);
			}
		}
		
		static void OnFileRemoving(FileEventArgs e) 
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
		
		static void OnFileRenaming(FileRenameEventArgs e) {
			if (FileRenaming != null) {
				FileRenaming(null, e);
			}
		}
		
		public static event FileRenameEventHandler FileRenaming;
		public static event FileRenameEventHandler FileRenamed;
		
		public static event FileEventHandler FileRemoving;
		public static event FileEventHandler FileRemoved;
	}
}
