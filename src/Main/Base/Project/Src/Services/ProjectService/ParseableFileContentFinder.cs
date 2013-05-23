// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
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
		FileName[] viewContentFileNamesCollection = SD.MainThread.InvokeIfRequired(() => SD.FileService.OpenedFiles.Select(f => f.FileName).ToArray());
		
		public ITextSource CreateForOpenFile(FileName fileName)
		{
			foreach (FileName name in viewContentFileNamesCollection) {
				if (FileUtility.IsEqualFileName(name, fileName))
					return SD.FileService.GetFileContentForOpenFile(fileName);
			}
			return null;
		}
		
		/// <summary>
		/// Retrieves the file contents for the specified project items.
		/// </summary>
		public ITextSource Create(FileName fileName)
		{
			ITextSource textSource = CreateForOpenFile(fileName);
			if (textSource != null)
				return textSource;
			try {
				return SD.FileService.GetFileContentFromDisk(fileName);
			} catch (IOException) {
				return null;
			} catch (UnauthorizedAccessException) {
				return null;
			}
		}
	}
}
