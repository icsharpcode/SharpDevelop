// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	public static class FileService
	{
		static bool serviceInitialized;
		
		#region RecentOpen
		static RecentOpen recentOpen = null;
		
		public static RecentOpen RecentOpen {
			get {
				if (recentOpen == null) {
					recentOpen = RecentOpen.FromXmlElement(PropertyService.Get("RecentOpen", new Properties()));
				}
				return recentOpen;
			}
		}
		
		static void ProjectServiceSolutionLoaded(object sender, SolutionEventArgs e)
		{
			RecentOpen.AddLastProject(e.Solution.FileName);
		}
		#endregion
		
		public static void Unload()
		{
			if (recentOpen != null) {
				PropertyService.Set("RecentOpen", recentOpen.ToProperties());
			}
			ProjectService.SolutionLoaded -= ProjectServiceSolutionLoaded;
			ParserService.LoadSolutionProjectsThreadEnded -= ParserServiceLoadSolutionProjectsThreadEnded;
			serviceInitialized = false;
		}
		
		internal static void InitializeService()
		{
			if (!serviceInitialized) {
				ProjectService.SolutionLoaded += ProjectServiceSolutionLoaded;
				ParserService.LoadSolutionProjectsThreadEnded += ParserServiceLoadSolutionProjectsThreadEnded;
				serviceInitialized = true;
			}
		}
		
		#region OpenedFile
		static Dictionary<FileName, OpenedFile> openedFileDict = new Dictionary<FileName, OpenedFile>();
		
		/// <summary>
		/// Gets a collection containing all currently opened files.
		/// The returned collection is a read-only copy of the currently opened files -
		/// it will not reflect future changes of the list of opened files.
		/// </summary>
		public static ICollection<OpenedFile> OpenedFiles {
			get {
				return openedFileDict.Values.ToArray();
			}
		}
		
		/// <summary>
		/// Gets an opened file, or returns null if the file is not opened.
		/// </summary>
		public static OpenedFile GetOpenedFile(string fileName)
		{
			return GetOpenedFile(FileName.Create(fileName));
		}
		
		/// <summary>
		/// Gets an opened file, or returns null if the file is not opened.
		/// </summary>
		public static OpenedFile GetOpenedFile(FileName fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			
			WorkbenchSingleton.AssertMainThread();
			
			OpenedFile file;
			openedFileDict.TryGetValue(fileName, out file);
			return file;
		}
		
		/// <summary>
		/// Gets or creates an opened file.
		/// Warning: the opened file will be a file without any views attached.
		/// Make sure to attach a view to it, or call CloseIfAllViewsClosed on the OpenedFile to
		/// unload the OpenedFile instance if no views were attached to it.
		/// </summary>
		public static OpenedFile GetOrCreateOpenedFile(string fileName)
		{
			return GetOrCreateOpenedFile(FileName.Create(fileName));
		}
		
		/// <summary>
		/// Gets or creates an opened file.
		/// Warning: the opened file will be a file without any views attached.
		/// Make sure to attach a view to it, or call CloseIfAllViewsClosed on the OpenedFile to
		/// unload the OpenedFile instance if no views were attached to it.
		/// </summary>
		public static OpenedFile GetOrCreateOpenedFile(FileName fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			
			OpenedFile file;
			if (!openedFileDict.TryGetValue(fileName, out file)) {
				openedFileDict[fileName] = file = new FileServiceOpenedFile(fileName);
			}
			return file;
		}
		
		/// <summary>
		/// Creates a new untitled OpenedFile.
		/// </summary>
		public static OpenedFile CreateUntitledOpenedFile(string defaultName, byte[] content)
		{
			if (defaultName == null)
				throw new ArgumentNullException("defaultName");
			
			OpenedFile file = new FileServiceOpenedFile(content);
			file.FileName = new FileName(file.GetHashCode() + "/" + defaultName);
			openedFileDict[file.FileName] = file;
			return file;
		}
		
		/// <summary>Called by OpenedFile.set_FileName to update the dictionary.</summary>
		internal static void OpenedFileFileNameChange(OpenedFile file, FileName oldName, FileName newName)
		{
			if (oldName == null) return; // File just created with NewFile where name is being initialized.
			
			LoggingService.Debug("OpenedFileFileNameChange: " + oldName + " => " + newName);
			
			if (openedFileDict[oldName] != file)
				throw new ArgumentException("file must be registered as oldName");
			if (openedFileDict.ContainsKey(newName)) {
				OpenedFile oldFile = openedFileDict[newName];
				if (oldFile.CurrentView != null) {
					if (oldFile.CurrentView.WorkbenchWindow != null)
						oldFile.CurrentView.WorkbenchWindow.CloseWindow(true);
				} else {
					throw new ArgumentException("there already is a file with the newName");
				}
			}
			openedFileDict.Remove(oldName);
			openedFileDict[newName] = file;
		}
		
		/// <summary>Called by OpenedFile.UnregisterView to update the dictionary.</summary>
		internal static void OpenedFileClosed(OpenedFile file)
		{
			OpenedFile existing;
			if (openedFileDict.TryGetValue(file.FileName, out existing) && existing != file)
				throw new ArgumentException("file must be registered");
			
			openedFileDict.Remove(file.FileName);
			LoggingService.Debug("OpenedFileClosed: " + file.FileName);
		}
		#endregion
		
		/// <summary>
		/// Checks if the path is valid <b>and shows a MessageBox if it is not valid</b>.
		/// Do not use in non-UI methods.
		/// </summary>
		public static bool CheckFileName(string path)
		{
			if (FileUtility.IsValidPath(path))
				return true;
			MessageService.ShowMessage(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.SaveFile.InvalidFileNameError}", new StringTagPair("FileName", path)));
			return false;
		}
		
		/// <summary>
		/// Checks that a single directory entry (file or subdirectory) name is valid.
		/// </summary>
		/// <param name="name">A single file name not the full path</param>
		public static bool CheckDirectoryEntryName(string name)
		{
			if (FileUtility.IsValidDirectoryEntryName(name))
				return true;
			MessageService.ShowMessage(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.SaveFile.InvalidFileNameError}", new StringTagPair("FileName", name)));
			return false;
		}
		
		/// <summary>
		/// Checks that a single directory name is valid.
		/// </summary>
		/// <param name="name">A single directory name not the full path</param>
		[Obsolete("Use CheckDirectoryEntryName instead")]
		public static bool CheckDirectoryName(string name)
		{
			return CheckDirectoryEntryName(name);
		}
		
		internal sealed class LoadFileWrapper
		{
			readonly IDisplayBinding binding;
			readonly bool switchToOpenedView;
			
			public LoadFileWrapper(IDisplayBinding binding, bool switchToOpenedView)
			{
				this.binding = binding;
				this.switchToOpenedView = switchToOpenedView;
			}
			
			public void Invoke(string fileName)
			{
				OpenedFile file = FileService.GetOrCreateOpenedFile(FileName.Create(fileName));
				try {
					IViewContent newContent = binding.CreateContentForFile(file);
					if (newContent != null) {
						DisplayBindingService.AttachSubWindows(newContent, false);
						WorkbenchSingleton.Workbench.ShowView(newContent, switchToOpenedView);
					}
				} finally {
					file.CloseIfAllViewsClosed();
				}
			}
		}
		
		static void ParserServiceLoadSolutionProjectsThreadEnded(object sender, EventArgs e)
		{
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection.ToArray()) {
				DisplayBindingService.AttachSubWindows(content, true);
			}
		}
		
		public static bool IsOpen(string fileName)
		{
			return GetOpenFile(fileName) != null;
		}
		
		/// <summary>
		/// Opens a view content for the specified file and switches to the opened view
		/// or switches to and returns the existing view content for the file if it is already open.
		/// </summary>
		/// <param name="fileName">The name of the file to open.</param>
		/// <returns>The existing or opened <see cref="IViewContent"/> for the specified file.</returns>
		public static IViewContent OpenFile(string fileName)
		{
			return OpenFile(fileName, true);
		}
		
		/// <summary>
		/// Opens a view content for the specified file
		/// or returns the existing view content for the file if it is already open.
		/// </summary>
		/// <param name="fileName">The name of the file to open.</param>
		/// <param name="switchToOpenedView">Specifies whether to switch to the view for the specified file.</param>
		/// <returns>The existing or opened <see cref="IViewContent"/> for the specified file.</returns>
		public static IViewContent OpenFile(string fileName, bool switchToOpenedView)
		{
			fileName = FileUtility.NormalizePath(fileName);
			LoggingService.Info("Open file " + fileName);
			
			IViewContent viewContent = GetOpenFile(fileName);
			if (viewContent != null) {
				if (switchToOpenedView) {
					viewContent.WorkbenchWindow.SelectWindow();
				}
				return viewContent;
			}
			
			IDisplayBinding binding = DisplayBindingService.GetBindingPerFileName(fileName);
			
			if (binding == null) {
				binding = new ErrorFallbackBinding("Could not find any display binding for " + Path.GetFileName(fileName));
			}
			if (FileUtility.ObservedLoad(new NamedFileOperationDelegate(new LoadFileWrapper(binding, switchToOpenedView).Invoke), fileName) == FileOperationResult.OK) {
				FileService.RecentOpen.AddLastFile(fileName);
			}
			return GetOpenFile(fileName);
		}
		
		/// <summary>
		/// Opens a new unsaved file.
		/// </summary>
		/// <param name="defaultName">The (unsaved) name of the to open</param>
		/// <param name="content">Content of the file to create</param>
		public static IViewContent NewFile(string defaultName, string content)
		{
			return NewFile(defaultName, ParserService.DefaultFileEncoding.GetBytesWithPreamble(content));
		}
		
		/// <summary>
		/// Opens a new unsaved file.
		/// </summary>
		/// <param name="defaultName">The (unsaved) name of the to open</param>
		/// <param name="content">Content of the file to create</param>
		public static IViewContent NewFile(string defaultName, byte[] content)
		{
			if (defaultName == null)
				throw new ArgumentNullException("defaultName");
			if (content == null)
				throw new ArgumentNullException("content");
			
			IDisplayBinding binding = DisplayBindingService.GetBindingPerFileName(defaultName);
			
			if (binding == null) {
				binding = new ErrorFallbackBinding("Can't create display binding for file " + defaultName);
			}
			OpenedFile file = CreateUntitledOpenedFile(defaultName, content);
			
			IViewContent newContent = binding.CreateContentForFile(file);
			if (newContent == null) {
				LoggingService.Warn("Created view content was null - DefaultName:" + defaultName);
				file.CloseIfAllViewsClosed();
				return null;
			}
			
			DisplayBindingService.AttachSubWindows(newContent, false);
			
			WorkbenchSingleton.Workbench.ShowView(newContent);
			return newContent;
		}
		
		/// <summary>
		/// Gets a list of the names of the files that are open as primary files
		/// in view contents.
		/// </summary>
		public static IList<FileName> GetOpenFiles()
		{
			List<FileName> fileNames = new List<FileName>();
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				FileName contentName = content.PrimaryFileName;
				if (contentName != null && !fileNames.Contains(contentName))
					fileNames.Add(contentName);
			}
			return fileNames;
		}
		
		/// <summary>
		/// Gets the IViewContent for a fileName. Returns null if the file is not opened currently.
		/// </summary>
		public static IViewContent GetOpenFile(string fileName)
		{
			if (fileName != null && fileName.Length > 0) {
				foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
					string contentName = content.PrimaryFileName;
					if (contentName != null) {
						if (FileUtility.IsEqualFileName(fileName, contentName))
							return content;
					}
				}
			}
			return null;
		}
		
		public static bool DeleteToRecycleBin {
			get {
				return PropertyService.Get("SharpDevelop.DeleteToRecycleBin", true);
			}
			set {
				PropertyService.Set("SharpDevelop.DeleteToRecycleBin", value);
			}
		}
		
		public static bool SaveUsingTemporaryFile {
			get {
				return PropertyService.Get("SharpDevelop.SaveUsingTemporaryFile", true);
			}
			set {
				PropertyService.Set("SharpDevelop.SaveUsingTemporaryFile", value);
			}
		}
		
		public static int DefaultFileEncodingCodePage {
			get { return PropertyService.Get("SharpDevelop.DefaultFileEncoding", 65001); }
			set { PropertyService.Set("SharpDevelop.DefaultFileEncoding", value); }
		}
		
		static readonly ReadOnlyCollection<EncodingInfo> allEncodings = Encoding.GetEncodings().OrderBy(e => e.DisplayName).ToArray().AsReadOnly();
		
		public static ReadOnlyCollection<EncodingInfo> AllEncodings {
			get { return allEncodings; }
		}
		
		public static EncodingInfo DefaultFileEncoding {
			get {
				int cp = FileService.DefaultFileEncodingCodePage;
				return allEncodings.Single(e => e.CodePage == cp);
			}
			set {
				FileService.DefaultFileEncodingCodePage = value.CodePage;
			}
		}
		
		/// <summary>
		/// Removes a file, raising the appropriate events. This method may show message boxes.
		/// </summary>
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
							if (DeleteToRecycleBin)
								NativeMethods.DeleteToRecycleBin(fileName);
							else
								Directory.Delete(fileName, true);
						}
					} catch (Exception e) {
						MessageService.ShowHandledException(e, "Can't remove directory " + fileName);
					}
				} else {
					try {
						if (File.Exists(fileName)) {
							if (DeleteToRecycleBin)
								NativeMethods.DeleteToRecycleBin(fileName);
							else
								File.Delete(fileName);
						}
					} catch (Exception e) {
						MessageService.ShowHandledException(e, "Can't remove file " + fileName);
					}
				}
			}
			OnFileRemoved(new FileEventArgs(fileName, isDirectory));
		}
		
		/// <summary>
		/// Renames or moves a file, raising the appropriate events. This method may show message boxes.
		/// </summary>
		public static bool RenameFile(string oldName, string newName, bool isDirectory)
		{
			if (FileUtility.IsEqualFileName(oldName, newName))
				return false;
			FileChangeWatcher.DisableAllChangeWatchers();
			try {
				FileRenamingEventArgs eargs = new FileRenamingEventArgs(oldName, newName, isDirectory);
				OnFileRenaming(eargs);
				if (eargs.Cancel)
					return false;
				if (!eargs.OperationAlreadyDone) {
					try {
						if (isDirectory && Directory.Exists(oldName)) {
							
							if (Directory.Exists(newName)) {
								MessageService.ShowMessage(StringParser.Parse("${res:Gui.ProjectBrowser.FileInUseError}"));
								return false;
							}
							Directory.Move(oldName, newName);
							
						} else if (File.Exists(oldName)) {
							if (File.Exists(newName)) {
								MessageService.ShowMessage(StringParser.Parse("${res:Gui.ProjectBrowser.FileInUseError}"));
								return false;
							}
							File.Move(oldName, newName);
						}
					} catch (Exception e) {
						if (isDirectory) {
							MessageService.ShowHandledException(e, "Can't rename directory " + oldName);
						} else {
							MessageService.ShowHandledException(e, "Can't rename file " + oldName);
						}
						return false;
					}
				}
				OnFileRenamed(new FileRenameEventArgs(oldName, newName, isDirectory));
				return true;
			} finally {
				FileChangeWatcher.EnableAllChangeWatchers();
			}
		}
		
		/// <summary>
		/// Copies a file, raising the appropriate events. This method may show message boxes.
		/// </summary>
		public static bool CopyFile(string oldName, string newName, bool isDirectory, bool overwrite)
		{
			if (FileUtility.IsEqualFileName(oldName, newName))
				return false;
			FileRenamingEventArgs eargs = new FileRenamingEventArgs(oldName, newName, isDirectory);
			OnFileCopying(eargs);
			if (eargs.Cancel)
				return false;
			if (!eargs.OperationAlreadyDone) {
				try {
					if (isDirectory && Directory.Exists(oldName)) {
						
						if (!overwrite && Directory.Exists(newName)) {
							MessageService.ShowMessage(StringParser.Parse("${res:Gui.ProjectBrowser.FileInUseError}"));
							return false;
						}
						FileUtility.DeepCopy(oldName, newName, overwrite);
						
					} else if (File.Exists(oldName)) {
						if (!overwrite && File.Exists(newName)) {
							MessageService.ShowMessage(StringParser.Parse("${res:Gui.ProjectBrowser.FileInUseError}"));
							return false;
						}
						File.Copy(oldName, newName, overwrite);
					}
				} catch (Exception e) {
					if (isDirectory) {
						MessageService.ShowHandledException(e, "Can't copy directory " + oldName);
					} else {
						MessageService.ShowHandledException(e, "Can't copy file " + oldName);
					}
					return false;
				}
			}
			OnFileCopied(new FileRenameEventArgs(oldName, newName, isDirectory));
			return true;
		}
		
		/// <summary>
		/// Opens the specified file and jumps to the specified file position.
		/// Line and column start counting at 1.
		/// </summary>
		public static IViewContent JumpToFilePosition(string fileName, int line, int column)
		{
			LoggingService.InfoFormatted("FileService\n\tJumping to File Position:  [{0} : {1}x{2}]", fileName, line, column);
			
			if (fileName == null || fileName.Length == 0) {
				return null;
			}
			
			NavigationService.SuspendLogging();
			bool loggingResumed = false;
			
			try {
				IViewContent content = OpenFile(fileName);
				if (content is IPositionable) {
					// TODO: enable jumping to a particular view
					content.WorkbenchWindow.ActiveViewContent = content;
					NavigationService.ResumeLogging();
					loggingResumed = true;
					((IPositionable)content).JumpTo(Math.Max(1, line), Math.Max(1, column));
				} else {
					NavigationService.ResumeLogging();
					loggingResumed = true;
					NavigationService.Log(content);
				}
				
				return content;
				
			} finally {
				LoggingService.InfoFormatted("FileService\n\tJumped to File Position:  [{0} : {1}x{2}]", fileName, line, column);
				
				if (!loggingResumed) {
					NavigationService.ResumeLogging();
				}
			}
		}
		
		/// <summary>
		/// Creates a FolderBrowserDialog that will initially select the
		/// specified folder. If the folder does not exist then the default
		/// behaviour of the FolderBrowserDialog is used where it selects the
		/// desktop folder.
		/// </summary>
		public static FolderBrowserDialog CreateFolderBrowserDialog(string description, string selectedPath)
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			dialog.Description = StringParser.Parse(description);
			if (selectedPath != null && selectedPath.Length > 0 && Directory.Exists(selectedPath)) {
				dialog.RootFolder = Environment.SpecialFolder.MyComputer;
				dialog.SelectedPath = selectedPath;
			}
			return dialog;
		}
		
		/// <summary>
		/// Creates a FolderBrowserDialog that will initially select the
		/// desktop folder.
		/// </summary>
		public static FolderBrowserDialog CreateFolderBrowserDialog(string description)
		{
			return CreateFolderBrowserDialog(description, null);
		}
		
		#region Event Handlers
		
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
		
		static void OnFileCopied(FileRenameEventArgs e)
		{
			if (FileCopied != null) {
				FileCopied(null, e);
			}
		}
		
		static void OnFileCopying(FileRenamingEventArgs e) {
			if (FileCopying != null) {
				FileCopying(null, e);
			}
		}
		
		#endregion Event Handlers
		
		#region Static event firing methods
		
		/// <summary>
		/// Fires the event handlers for a file being created.
		/// </summary>
		/// <param name="fileName">The name of the file being created. This should be a fully qualified path.</param>
		/// <param name="isDirectory">Set to true if this is a directory</param>
		public static bool FireFileReplacing(string fileName, bool isDirectory)
		{
			FileCancelEventArgs e = new FileCancelEventArgs(fileName, isDirectory);
			if (FileReplacing != null) {
				FileReplacing(null, e);
			}
			return !e.Cancel;
		}
		
		/// <summary>
		/// Fires the event handlers for a file being replaced.
		/// </summary>
		/// <param name="fileName">The name of the file being created. This should be a fully qualified path.</param>
		/// <param name="isDirectory">Set to true if this is a directory</param>
		public static void FireFileReplaced(string fileName, bool isDirectory)
		{
			if (FileReplaced != null) {
				FileReplaced(null, new FileEventArgs(fileName, isDirectory));
			}
		}
		
		/// <summary>
		/// Fires the event handlers for a file being created.
		/// </summary>
		/// <param name="fileName">The name of the file being created. This should be a fully qualified path.</param>
		/// <param name="isDirectory">Set to true if this is a directory</param>
		public static void FireFileCreated(string fileName, bool isDirectory)
		{
			if (FileCreated != null) {
				FileCreated(null, new FileEventArgs(fileName, isDirectory));
			}
		}
		
		#endregion Static event firing methods
		
		#region Events
		
		public static event EventHandler<FileEventArgs> FileCreated;
		
		public static event EventHandler<FileRenamingEventArgs> FileRenaming;
		public static event EventHandler<FileRenameEventArgs> FileRenamed;
		
		public static event EventHandler<FileRenamingEventArgs> FileCopying;
		public static event EventHandler<FileRenameEventArgs> FileCopied;
		
		public static event EventHandler<FileCancelEventArgs> FileRemoving;
		public static event EventHandler<FileEventArgs> FileRemoved;
		
		public static event EventHandler<FileCancelEventArgs> FileReplacing;
		public static event EventHandler<FileEventArgs> FileReplaced;
		
		#endregion Events
		
		sealed class ErrorFallbackBinding : IDisplayBinding
		{
			string errorMessage;
			
			public ErrorFallbackBinding(string errorMessage)
			{
				this.errorMessage = errorMessage;
			}
			
			public bool CanCreateContentForFile(string fileName)
			{
				return true;
			}
			
			public IViewContent CreateContentForFile(OpenedFile file)
			{
				return new SimpleViewContent(errorMessage) { TitleName = Path.GetFileName(file.FileName) };
			}
			
			public bool IsPreferredBindingForFile(string fileName)
			{
				return false;
			}
			
			public double AutoDetectFileContent(string fileName, Stream fileContent, string detectedMimeType)
			{
				return double.NegativeInfinity;
			}
		}
	}
}
