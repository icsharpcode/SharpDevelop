// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// An enumerator which enumerates through a list of project items, returning the
	/// parseable file content of each item.
	/// </summary>
	/// <remarks>
	/// This class is thread-safe in a very limited way:
	/// It can be created from every thread, but may be only used by the thread that created it.
	/// It automatically uses WorkbenchSingleton.SafeThreadCall for reading currently open
	/// files when it is created/accessed from a thread.
	/// </remarks>
	public class ParseableFileContentEnumerator : IEnumerator<KeyValuePair<string, string>>
	{
		void IEnumerator.Reset() {
			throw new NotSupportedException();
		}
		
		KeyValuePair<string, string> current;
		
		object IEnumerator.Current {
			get {
				return current;
			}
		}
		
		public KeyValuePair<string, string> Current {
			get {
				return current;
			}
		}
		
		public string CurrentFileName {
			get {
				return current.Key;
			}
		}
		
		public string CurrentFileContent {
			get {
				return current.Value;
			}
		}
		
		public void Dispose()
		{
		}
		
		IList<ProjectItem> projectItems;
		bool isOnMainThread;
		Encoding defaultEncoding;
		
		public ParseableFileContentEnumerator(IProject project) : this(project.Items) { }
		
		public ParseableFileContentEnumerator(IList<ProjectItem> projectItems)
		{
			isOnMainThread = !WorkbenchSingleton.InvokeRequired;
			this.projectItems = projectItems;
			if (projectItems.Count > 0) {
				nextItem = projectItems[0];
			}
			defaultEncoding = ParserService.DefaultFileEncoding;
		}
		
		string GetParseableFileContent(IProject project, string fileName)
		{
			// Loading the source files is done asynchronously:
			// While one file is parsed, the next is already loaded from disk.
			
			// Load file from memory if it is open
			OpenedFile file = FileService.GetOpenedFile(fileName);
			if (file != null) {
				string content;
				if (isOnMainThread) {
					content = GetFileContentFromFileDocumentProvider(file);
				} else {
					content = WorkbenchSingleton.SafeThreadFunction<OpenedFile, string>(GetFileContentFromFileDocumentProvider, file);
				}
				if (content != null) {
					return content;
				}
				
				using(Stream s = file.OpenRead()) {
					Encoding encoding = defaultEncoding;
					return ICSharpCode.TextEditor.Util.FileReader.ReadFileContent(s, ref encoding);
				}
			}
			
			// load file
			return ICSharpCode.TextEditor.Util.FileReader.ReadFileContent(fileName, defaultEncoding);
		}
		
		ProjectItem nextItem;
		int index = 0;
		
		public int ItemCount {
			get {
				return projectItems.Count;
			}
		}
		
		public int Index {
			get {
				return index;
			}
		}
		
		public bool MoveNext()
		{
			ProjectItem item = nextItem;
			nextItem = (++index < projectItems.Count) ? projectItems[index] : null;
			if (item == null) return false;
			
			if (ParserService.GetParser(item.FileName) == null)
				return MoveNext();
			
			string fileContent;
			try {
				fileContent = GetFileContent(item);
			} catch (FileNotFoundException ex) {
				LoggingService.Warn("ParseableFileContentEnumerator: " + ex.Message);
				return MoveNext(); // skip files that were not found
			} catch (IOException ex) {
				LoggingService.Warn("ParseableFileContentEnumerator: " + ex.Message);
				return MoveNext(); // skip invalid files
			}
			current = new KeyValuePair<string, string>(item.FileName, fileContent);
			return true;
		}
		
		string GetFileContent(ProjectItem item)
		{
			string fileName = item.FileName;
			if (IsFileOpen(fileName)) {
				string content;
				if (isOnMainThread)
					content = GetFileContentFromOpenFile(fileName);
				else
					content = WorkbenchSingleton.SafeThreadFunction<string, string>(GetFileContentFromOpenFile, fileName);
				if (content != null)
					return content;
			}
			return GetParseableFileContent(item.Project, fileName);
		}
		
		IList<string> viewContentFileNamesCollection;
		
		bool IsFileOpen(string fileName)
		{
			if (viewContentFileNamesCollection == null) {
				try {
					viewContentFileNamesCollection = WorkbenchSingleton.SafeThreadFunction<IList<string>>(FileService.GetOpenFiles);
				} catch (InvalidOperationException ex) {
					// can happen if the user closes SharpDevelop while the parser thread is running
					LoggingService.Warn(ex);
					viewContentFileNamesCollection = new string[0];
				}
			}
			foreach (string contentName in viewContentFileNamesCollection) {
				if (contentName != null) {
					if (FileUtility.IsEqualFileName(fileName, contentName))
						return true;
				}
			}
			return false;
		}
		
		string GetFileContentFromOpenFile(string fileName)
		{
			IViewContent viewContent = FileService.GetOpenFile(fileName);
			IEditable editable = viewContent as IEditable;
			if (editable != null) {
				return editable.Text;
			}
			return null;
		}
		
		static string GetFileContentFromFileDocumentProvider(OpenedFile file)
		{
			IFileDocumentProvider p = file.CurrentView as IFileDocumentProvider;
			if (p == null) return null;
			IDocument document = p.GetDocumentForFile(file);
			if (document == null) return null;
			return document.TextContent;
		}
	}
}
