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
				DisplayBindingService.AttachSubWindows(newContent);
				WorkbenchSingleton.Workbench.ShowView(newContent);
			}
		}
		
		public static bool IsOpen(string fileName)
		{
			return GetOpenFile(fileName) != null;
		}
		
		public static IWorkbenchWindow OpenFile(string fileName)
		{
			LoggingService.Info("Open file " + fileName);
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
					if (isURL ? content.FileName == fileName : FileUtility.IsEqualFileName(content.FileName, fileName)) {
						content.WorkbenchWindow.SelectWindow();
						return content.WorkbenchWindow;
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
				DisplayBindingService.AttachSubWindows(newContent);
				
				WorkbenchSingleton.Workbench.ShowView(newContent);
				return newContent.WorkbenchWindow;
			} else {
				throw new ApplicationException("Can't create display binding for language " + language);
			}
		}
		
		public static IWorkbenchWindow GetOpenFile(string fileName)
		{
			if (fileName != null && fileName.Length > 0) {
				foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
					string contentName = content.IsUntitled ? content.UntitledName : content.FileName;
					if (contentName != null) {
						if (FileUtility.IsEqualFileName(fileName, contentName))
							return content.WorkbenchWindow;
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
			if (FileUtility.IsEqualFileName(oldName, newName))
				return;
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
			if (content is IPositionable) {
				window.SwitchView(0);
				((IPositionable)content).JumpTo(Math.Max(0, line), Math.Max(0, column));
			}
			return content;
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
