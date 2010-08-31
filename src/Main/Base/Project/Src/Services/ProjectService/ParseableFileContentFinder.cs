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
	/// <summary>
	/// Can be used to create ITextBuffer for ProjectItems.
	/// This class is thread-safe.
	/// </summary>
	public class ParseableFileContentFinder
	{
		FileName[] viewContentFileNamesCollection = WorkbenchSingleton.SafeThreadFunction(FileService.GetOpenFiles).ToArray();
		
		/// <summary>
		/// Retrieves the file contents for the specified project items.
		/// </summary>
		public ITextBuffer Create(FileName fileName)
		{
			foreach (FileName name in viewContentFileNamesCollection) {
				if (FileUtility.IsEqualFileName(name, fileName))
					return WorkbenchSingleton.SafeThreadFunction(ParserService.GetParseableFileContent, fileName.ToString());
			}
			try {
				return new StringTextBuffer(ICSharpCode.AvalonEdit.Utils.FileReader.ReadFileContent(fileName, ParserService.DefaultFileEncoding));
			} catch (IOException) {
				return null;
			} catch (UnauthorizedAccessException) {
				return null;
			}
		}
	}
}
