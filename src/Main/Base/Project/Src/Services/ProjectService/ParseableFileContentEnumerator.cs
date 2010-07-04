// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ParseableFileContentEntry
	{
		public FileName FileName { get; private set; }
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
		
		internal ParseableFileContentEntry(FileName fileName, FileName[] viewContentFileNamesCollection)
		{
			this.FileName = fileName;
			foreach (FileName name in viewContentFileNamesCollection) {
				if (FileUtility.IsEqualFileName(name, this.FileName)) {
					openContent = WorkbenchSingleton.SafeThreadFunction(ParserService.GetParseableFileContent, this.FileName.ToString());
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
		FileName[] viewContentFileNamesCollection = WorkbenchSingleton.SafeThreadFunction(FileService.GetOpenFiles).ToArray();
		
		/// <summary>
		/// Retrieves the file contents for the specified project items.
		/// </summary>
		public ParseableFileContentEntry Create(FileName fileName)
		{
			return new ParseableFileContentEntry(fileName, viewContentFileNamesCollection);
		}
	}
}
