// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;

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
		public ITextSource Create(FileName fileName)
		{
			foreach (FileName name in viewContentFileNamesCollection) {
				if (FileUtility.IsEqualFileName(name, fileName))
					return WorkbenchSingleton.SafeThreadFunction(ParserService.GetParseableFileContent, fileName.ToString());
			}
			try {
				return new StringTextSource(ICSharpCode.AvalonEdit.Utils.FileReader.ReadFileContent(fileName, ParserService.DefaultFileEncoding));
			} catch (IOException) {
				return null;
			} catch (UnauthorizedAccessException) {
				return null;
			}
		}
	}
}
