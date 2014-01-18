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
using System.Text;
using System.Threading;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <summary>
	/// Options for the <see cref="IFileService.UpdateFileModel()"/> method. 
	/// </summary>
	[Flags]
	public enum FileUpdateOptions
	{
		None = 0,
		/// <summary>
		/// If no view content exists for the file, it will be opened in a new view.
		/// The view content can then be used to save the changes performed by the update.
		/// If this option is not used, changes are automatically saved to disk if the file is not open in any view.
		/// </summary>
		OpenViewIfNoneExists = 1,
		/// <summary>
		/// The changes are saved to disk after the update, even if the file is currently open in a view
		/// (in that case, other changes from the view may be saved as well).
		/// </summary>
		SaveToDisk = 2,
	}
	
	/// <summary>
	/// Manages the list files opened by view contents so that multiple view contents opening the same file can synchronize.
	/// Also provides events that can be used to listen to file operations performed in the IDE.
	/// </summary>
	[SDService("SD.FileService")]
	public interface IFileService
	{
		#region Options
		IRecentOpen RecentOpen { get; }
		
		bool DeleteToRecycleBin { get; set; }
		bool SaveUsingTemporaryFile { get; set; }
		
		/// <summary>
		/// Gets the default file encoding.
		/// This property is thread-safe.
		/// </summary>
		Encoding DefaultFileEncoding { get; }
		
		EncodingInfo DefaultFileEncodingInfo { get; set; }
		
		IReadOnlyList<EncodingInfo> AllEncodings { get; }
		#endregion
		
		#region GetFileContent
		/// <summary>
		/// Gets the content of the specified file.
		/// If the file is currently open in SharpDevelop, retrieves a snapshot
		/// of the editor content.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe. This method involves waiting for the main thread, so using it while
		/// holding a lock can lead to deadlocks.
		/// This method never returns null.
		/// </remarks>
		ITextSource GetFileContent(FileName fileName);
		
		/// <inheritdoc cref="GetFileContent(FileName)"/>
		ITextSource GetFileContent(string fileName);
		
		/// <summary>
		/// Gets the file content for a file that is currently open.
		/// Returns null if the file is not open.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe. This method involves waiting for the main thread, so using it while
		/// holding a lock can lead to deadlocks.
		/// </remarks>
		ITextSource GetFileContentForOpenFile(FileName fileName);
		
		/// <summary>
		/// Gets the file content from disk, ignoring open files.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// This method never returns null.
		/// </remarks>
		ITextSource GetFileContentFromDisk(FileName fileName, CancellationToken cancellationToken = default(CancellationToken));
		#endregion
		
		#region BrowseForFolder
		/// <summary>
		/// Shows a 'browse for folder' dialog.
		/// </summary>
		/// <param name="description">Description shown in the dialog.</param>
		/// <param name="selectedPath">Optional: Initially selected folder.</param>
		/// <returns>The selected folder; or <c>null</c> if the user cancelled the dialog.</returns>
		DirectoryName BrowseForFolder(string description, string selectedPath = null);
		#endregion
		
		#region OpenedFiles
		/// <summary>
		/// Gets a collection containing all currently opened files.
		/// The returned collection is a read-only copy of the currently opened files -
		/// it will not reflect future changes of the list of opened files.
		/// </summary>
		/// <remarks>
		/// Accessing this property does not increase the reference count on the opened files in the collection.
		/// If you want to maintain a reference over a longer period of time (so that the existing reference might be released),
		/// you need to call <see cref="OpenedFile.AddReference()"/>.
		/// </remarks>
		IReadOnlyList<OpenedFile> OpenedFiles { get; }
		
		/// <summary>
		/// Gets an opened file, or returns null if the file is not opened.
		/// </summary>
		/// <remarks>
		/// This method does not increase the reference count on the opened files in the collection.
		/// If you want to maintain a reference over a longer period of time (so that the existing reference might be released),
		/// you need to call <see cref="OpenedFile.AddReference()"/>.
		/// </remarks>
		OpenedFile GetOpenedFile(FileName fileName);
		
		/// <inheritdoc cref="GetOpenedFile(FileName)"/>
		OpenedFile GetOpenedFile(string fileName);
		
		/// <summary>
		/// Creates a new OpenedFile for the specified file name.
		/// If the file is already open, an existing OpenedFile is returned instead (and its reference count is increased).
		/// 
		/// Every CreateOpenedFile() call <b>must</b> be paired with a <see cref="OpenedFile.ReleaseReference()"/> call!
		/// </summary>
		OpenedFile CreateOpenedFile(FileName fileName);
		
		/// <summary>
		/// Creates a new untitled OpenedFile.
		/// 
		/// Every CreateUntitledOpenedFile() call <b>must</b> be paired with a <see cref="OpenedFile.ReleaseReference()"/> call!
		/// </summary>
		OpenedFile CreateUntitledOpenedFile(string defaultName, byte[] content);
		
		/// <summary>
		/// Updates a file by performing actions on a model.
		/// </summary>
		/// <param name="fileName">The file to be updated.</param>
		/// <param name="modelProvider">The type of model to use for the update.</param>
		/// <param name="action">A delegate that performs the update.</param>
		/// <param name="options">Provides options regarding the file model update.</param>
		void UpdateFileModel<T>(FileName fileName, IFileModelProvider<T> modelProvider, Action<T> action, FileUpdateOptions options = FileUpdateOptions.None) where T : class;
		#endregion
		
		#region CheckFileName
		/// <summary>
		/// Checks if the path is valid <b>and shows a MessageBox if it is not valid</b>.
		/// Do not use in non-UI methods.
		/// </summary>
		/// <seealso cref="FileUtility.IsValidPath"/>
		bool CheckFileName(string path);
		
		/// <summary>
		/// Checks that a single directory entry (file or subdirectory) name is valid
		///  <b>and shows a MessageBox if it is not valid</b>.
		/// </summary>
		/// <param name="name">A single file name not the full path</param>
		/// <seealso cref="FileUtility.IsValidDirectoryEntryName"/>
		bool CheckDirectoryEntryName(string name);
		#endregion
		
		#region OpenFile (ViewContent)
		/// <summary>
		/// Gets whether the file is open in a view content.
		/// </summary>
		bool IsOpen(FileName fileName);
		
		/// <summary>
		/// Opens a view content for the specified file
		/// or returns the existing view content for the file if it is already open.
		/// </summary>
		/// <param name="fileName">The name of the file to open.</param>
		/// <param name="switchToOpenedView">Specifies whether to switch to the view for the specified file.</param>
		/// <returns>The existing or opened <see cref="IViewContent"/> for the specified file.</returns>
		IViewContent OpenFile(FileName fileName, bool switchToOpenedView = true);
		
		/// <summary>
		/// Opens a view content for the specified file using the specified display binding.
		/// </summary>
		/// <param name="fileName">The name of the file to open.</param>
		/// <param name="displayBinding">The display binding to use for opening the file.</param>
		/// <param name="switchToOpenedView">Specifies whether to switch to the view for the specified file.</param>
		/// <returns>The existing or opened <see cref="IViewContent"/> for the specified file.</returns>
		IViewContent OpenFileWith(FileName fileName, IDisplayBinding displayBinding, bool switchToOpenedView = true);
		
		/// <summary>
		/// Shows the 'Open With' dialog, allowing the user to pick a display binding for opening the specified files.
		/// </summary>
		IEnumerable<IViewContent> ShowOpenWithDialog(IEnumerable<FileName> fileNames, bool switchToOpenedView = true);
		
		/// <summary>
		/// Opens a new unsaved file.
		/// </summary>
		/// <param name="defaultName">The (unsaved) name of the to open</param>
		/// <param name="content">Content of the file to create</param>
		IViewContent NewFile(string defaultName, string content);
		
		/// <summary>
		/// Opens a new unsaved file.
		/// </summary>
		/// <param name="defaultName">The (unsaved) name of the to open</param>
		/// <param name="content">Content of the file to create</param>
		IViewContent NewFile(string defaultName, byte[] content);
		
		/// <summary>
		/// Gets a list of the names of the files that are open as primary files
		/// in view contents.
		/// </summary>
		IReadOnlyList<FileName> OpenPrimaryFiles { get; }
		
		/// <summary>
		/// Gets the IViewContent for a fileName. Returns null if the file is not opened currently.
		/// </summary>
		IViewContent GetOpenFile(FileName fileName);
		
		/// <summary>
		/// Opens the specified file and jumps to the specified file position.
		/// Line and column start counting at 1.
		/// </summary>
		IViewContent JumpToFilePosition(FileName fileName, int line, int column);
		#endregion
		
		#region Remove/Rename/Copy
		/// <summary>
		/// Removes a file, raising the appropriate events. This method may show message boxes.
		/// </summary>
		void RemoveFile(string fileName, bool isDirectory);
		
		/// <summary>
		/// Renames or moves a file, raising the appropriate events. This method may show message boxes.
		/// </summary>
		bool RenameFile(string oldName, string newName, bool isDirectory);
		
		/// <summary>
		/// Copies a file, raising the appropriate events. This method may show message boxes.
		/// </summary>
		bool CopyFile(string oldName, string newName, bool isDirectory, bool overwrite);
		
		event EventHandler<FileRenamingEventArgs> FileRenaming;
		event EventHandler<FileRenameEventArgs> FileRenamed;
		
		event EventHandler<FileRenamingEventArgs> FileCopying;
		event EventHandler<FileRenameEventArgs> FileCopied;
		
		event EventHandler<FileCancelEventArgs> FileRemoving;
		event EventHandler<FileEventArgs> FileRemoved;
		#endregion
		
		#region FileCreated/Replaced
		/// <summary>
		/// Fires the event handlers for a file being created.
		/// </summary>
		/// <param name="fileName">The name of the file being created. This should be a fully qualified path.</param>
		/// <param name="isDirectory">Set to true if this is a directory</param>
		/// <returns>True if the operation can proceed, false if an event handler cancelled the operation.</returns>
		bool FireFileReplacing(string fileName, bool isDirectory);
		
		/// <summary>
		/// Fires the event handlers for a file being replaced.
		/// </summary>
		/// <param name="fileName">The name of the file being created. This should be a fully qualified path.</param>
		/// <param name="isDirectory">Set to true if this is a directory</param>
		void FireFileReplaced(string fileName, bool isDirectory);
		
		/// <summary>
		/// Fires the event handlers for a file being created.
		/// </summary>
		/// <param name="fileName">The name of the file being created. This should be a fully qualified path.</param>
		/// <param name="isDirectory">Set to true if this is a directory</param>
		void FireFileCreated(string fileName, bool isDirectory);
		
		event EventHandler<FileEventArgs> FileCreated;
		event EventHandler<FileCancelEventArgs> FileReplacing;
		event EventHandler<FileEventArgs> FileReplaced;
		#endregion
	}
}
