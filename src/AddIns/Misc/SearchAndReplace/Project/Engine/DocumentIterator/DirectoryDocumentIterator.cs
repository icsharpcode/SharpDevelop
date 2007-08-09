// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Document;

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
					    content is ITextEditorControlProvider) {
						document = ((ITextEditorControlProvider)content).TextEditorControl.Document;
						return new ProvidedDocumentInformation(document,
						                                       fileName,
						                                       0);
					}
				}
				ITextBufferStrategy strategy = null;
				try {
					strategy = StringTextBufferStrategy.CreateTextBufferFromFile(fileName);
				} catch (Exception) {
					return null;
				}
				return new ProvidedDocumentInformation(strategy, 
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
