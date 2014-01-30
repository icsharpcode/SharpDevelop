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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop
{
	// TODO make obsolete, exists only for compatiblity with 4.x
	public static class FileService
	{
		/// <summary>
		/// Checks if the path is valid <b>and shows a MessageBox if it is not valid</b>.
		/// Do not use in non-UI methods.
		/// </summary>
		public static bool CheckFileName(string path)
		{
			return SD.FileService.CheckFileName(path);
		}
		
		/// <summary>
		/// Checks that a single directory entry (file or subdirectory) name is valid
		///  <b>and shows a MessageBox if it is not valid</b>.
		/// </summary>
		/// <param name="name">A single file name not the full path</param>
		public static bool CheckDirectoryEntryName(string name)
		{
			return SD.FileService.CheckDirectoryEntryName(name);
		}
		
		public static bool IsOpen(string fileName)
		{
			return SD.FileService.IsOpen(FileName.Create(fileName));
		}
		
		/// <summary>
		/// Opens a view content for the specified file and switches to the opened view
		/// or switches to and returns the existing view content for the file if it is already open.
		/// </summary>
		/// <param name="fileName">The name of the file to open.</param>
		/// <returns>The existing or opened <see cref="IViewContent"/> for the specified file.</returns>
		public static IViewContent OpenFile(string fileName)
		{
			return SD.FileService.OpenFile(FileName.Create(fileName));
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
			return SD.FileService.OpenFile(FileName.Create(fileName), switchToOpenedView);
		}
		
		/// <summary>
		/// Opens a new unsaved file.
		/// </summary>
		/// <param name="defaultName">The (unsaved) name of the to open</param>
		/// <param name="content">Content of the file to create</param>
		public static IViewContent NewFile(string defaultName, string content)
		{
			return SD.FileService.NewFile(defaultName, content);
		}
		
		/// <summary>
		/// Opens a new unsaved file.
		/// </summary>
		/// <param name="defaultName">The (unsaved) name of the to open</param>
		/// <param name="content">Content of the file to create</param>
		public static IViewContent NewFile(string defaultName, byte[] content)
		{
			return SD.FileService.NewFile(defaultName, content);
		}
		
		/// <summary>
		/// Gets a list of the names of the files that are open as primary files
		/// in view contents.
		/// </summary>
		public static IList<FileName> GetOpenFiles()
		{
			return SD.FileService.OpenPrimaryFiles.ToArray();
		}
		
		/// <summary>
		/// Gets the IViewContent for a fileName. Returns null if the file is not opened currently.
		/// </summary>
		public static IViewContent GetOpenFile(string fileName)
		{
			return SD.FileService.GetOpenFile(FileName.Create(fileName));
		}
		
		/// <summary>
		/// Removes a file, raising the appropriate events. This method may show message boxes.
		/// </summary>
		public static void RemoveFile(string fileName, bool isDirectory)
		{
			SD.FileService.RemoveFile(fileName, isDirectory);
		}
		
		/// <summary>
		/// Renames or moves a file, raising the appropriate events. This method may show message boxes.
		/// </summary>
		public static bool RenameFile(string oldName, string newName, bool isDirectory)
		{
			return SD.FileService.RenameFile(oldName, newName, isDirectory);
		}
		
		/// <summary>
		/// Copies a file, raising the appropriate events. This method may show message boxes.
		/// </summary>
		public static bool CopyFile(string oldName, string newName, bool isDirectory, bool overwrite)
		{
			return SD.FileService.CopyFile(oldName, newName, isDirectory, overwrite);
		}
		
		/// <summary>
		/// Opens the specified file and jumps to the specified file position.
		/// Line and column start counting at 1.
		/// </summary>
		public static IViewContent JumpToFilePosition(string fileName, int line, int column)
		{
			return SD.FileService.JumpToFilePosition(FileName.Create(fileName), line, column);
		}
		
		/// <summary>
		/// Creates a FolderBrowserDialog that will initially select the
		/// specified folder. If the folder does not exist then the default
		/// behaviour of the FolderBrowserDialog is used where it selects the
		/// desktop folder.
		/// </summary>
		[Obsolete("Use SD.FileService.BrowseForFolder() instead.")]
		public static FolderBrowserDialog CreateFolderBrowserDialog(string description, string selectedPath = null)
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			dialog.Description = StringParser.Parse(description);
			if (selectedPath != null && selectedPath.Length > 0 && Directory.Exists(selectedPath)) {
				dialog.RootFolder = Environment.SpecialFolder.MyComputer;
				dialog.SelectedPath = selectedPath;
			}
			return dialog;
		}
		
		#region Static event firing methods
		
		/// <summary>
		/// Fires the event handlers for a file being created.
		/// </summary>
		/// <param name="fileName">The name of the file being created. This should be a fully qualified path.</param>
		/// <param name="isDirectory">Set to true if this is a directory</param>
		public static bool FireFileReplacing(string fileName, bool isDirectory)
		{
			return SD.FileService.FireFileReplacing(fileName, isDirectory);
		}
		
		/// <summary>
		/// Fires the event handlers for a file being replaced.
		/// </summary>
		/// <param name="fileName">The name of the file being created. This should be a fully qualified path.</param>
		/// <param name="isDirectory">Set to true if this is a directory</param>
		public static void FireFileReplaced(string fileName, bool isDirectory)
		{
			SD.FileService.FireFileReplaced(fileName, isDirectory);
		}
		
		/// <summary>
		/// Fires the event handlers for a file being created.
		/// </summary>
		/// <param name="fileName">The name of the file being created. This should be a fully qualified path.</param>
		/// <param name="isDirectory">Set to true if this is a directory</param>
		public static void FireFileCreated(string fileName, bool isDirectory)
		{
			SD.FileService.FireFileCreated(fileName, isDirectory);
		}
		
		#endregion Static event firing methods
		
		#region Events
		
		public static event EventHandler<FileRenamingEventArgs> FileRenaming {
			add { SD.FileService.FileRenaming += value; }
			remove { SD.FileService.FileRenaming -= value; }
		}
		public static event EventHandler<FileRenameEventArgs> FileRenamed {
			add { SD.FileService.FileRenamed += value; }
			remove { SD.FileService.FileRenamed -= value; }
		}
		
		public static event EventHandler<FileRenamingEventArgs> FileCopying {
			add { SD.FileService.FileCopying += value; }
			remove { SD.FileService.FileCopying -= value; }
		}
		public static event EventHandler<FileRenameEventArgs> FileCopied {
			add { SD.FileService.FileCopied += value; }
			remove { SD.FileService.FileCopied -= value; }
		}
		
		public static event EventHandler<FileCancelEventArgs> FileRemoving {
			add { SD.FileService.FileRemoving += value; }
			remove { SD.FileService.FileRemoving -= value; }
		}
		public static event EventHandler<FileEventArgs> FileRemoved {
			add { SD.FileService.FileRemoved += value; }
			remove { SD.FileService.FileRemoved -= value; }
		}
		
		public static event EventHandler<FileEventArgs> FileCreated {
			add { SD.FileService.FileCreated += value; }
			remove { SD.FileService.FileCreated -= value; }
		}
		public static event EventHandler<FileCancelEventArgs> FileReplacing {
			add { SD.FileService.FileReplacing += value; }
			remove { SD.FileService.FileReplacing -= value; }
		}
		public static event EventHandler<FileEventArgs> FileReplaced {
			add { SD.FileService.FileReplaced += value; }
			remove { SD.FileService.FileReplaced -= value; }
		}
		
		#endregion Events
	}
}
