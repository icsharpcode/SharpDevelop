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
