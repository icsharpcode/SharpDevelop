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
using System.Linq;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ParseableFileContentEntry
	{
		public string FileName { get; private set; }
		ITextBuffer openContent;
		
		public ITextBuffer GetContent()
		{
			if (openContent != null)
				return openContent;
			try {
				return new StringTextBuffer(ICSharpCode.AvalonEdit.Utils.FileReader.ReadFileContent(FileName, ParserService.DefaultFileEncoding));
			} catch (IOException) {
				return null;
			} catch (UnauthorizedAccessException) {
				return null;
			}
		}
		
		internal ParseableFileContentEntry(ProjectItem item, IList<string> viewContentFileNamesCollection)
		{
			this.FileName = item.FileName;
			foreach (string name in viewContentFileNamesCollection) {
				if (FileUtility.IsEqualFileName(name, this.FileName)) {
					openContent = WorkbenchSingleton.SafeThreadFunction(ParserService.GetParseableFileContent, this.FileName);
					break;
				}
			}
		}
	}
	
	/// <summary>
	/// Can be used to create ParseableFileContentEntry for ProjectItems.
	/// This class is thread-safe.
	/// </summary>
	public class ParseableFileContentFinder
	{
		IList<string> viewContentFileNamesCollection = WorkbenchSingleton.SafeThreadFunction(FileService.GetOpenFiles);
		
		/// <summary>
		/// Retrieves the file contents for the specified project items.
		/// </summary>
		public ParseableFileContentEntry Create(ProjectItem p)
		{
			return new ParseableFileContentEntry(p, viewContentFileNamesCollection);
		}
	}
}
