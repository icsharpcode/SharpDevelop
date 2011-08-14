// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;

namespace SearchAndReplace
{
	public class DirectoryDocumentIterator : IDocumentIterator
	{
		string searchDirectory;
		string fileMask;
		bool   searchSubdirectories;
		
		List<string> files    = null;
		int              curIndex = -1;
		
		public DirectoryDocumentIterator(string searchDirectory, string fileMask, bool searchSubdirectories)
		{
			this.searchDirectory      = searchDirectory;
			this.fileMask             = fileMask;
			this.searchSubdirectories = searchSubdirectories;
			
			Reset();
		}
		
		public string CurrentFileName {
			get {
				if (curIndex < 0 || curIndex >= files.Count) {
					return null;
				}
				
				return files[curIndex].ToString();;
			}
		}
				
		public ProvidedDocumentInformation Current {
			get {
				if (curIndex < 0 || curIndex >= files.Count) {
					return null;
				}
				string fileName = files[curIndex].ToString();
				if (!File.Exists(fileName) || !SearchReplaceUtilities.IsSearchable(fileName)) {
					++curIndex;
					return Current;
				}
				IDocument document;
				foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
					if (content.PrimaryFileName != null &&
					    FileUtility.IsEqualFileName(content.PrimaryFileName, fileName) &&
					    content is ITextEditorProvider) {
						document = ((ITextEditorProvider)content).TextEditor.Document;
						return new ProvidedDocumentInformation(document,
						                                       fileName,
						                                       0);
					}
				}
				ITextSource fileContent;
				try {
					fileContent = ParserService.GetParseableFileContent(fileName);
				} catch (Exception) {
					return null;
				}
				return new ProvidedDocumentInformation(fileContent, 
				                                       fileName, 
				                                       0);
			}
		}
		
		public bool MoveForward() 
		{
			if (curIndex == -1) {
				files = FileUtility.SearchDirectory(this.searchDirectory, this.fileMask, this.searchSubdirectories);
			}
			return ++curIndex < files.Count;
		}
		
		public bool MoveBackward()
		{
			if (curIndex == -1) {
				curIndex = files.Count - 1;
				return true;
			}
			return --curIndex >= -1;
		}
		
		
		public void Reset() 
		{
			curIndex = -1;
		}
	}
}
