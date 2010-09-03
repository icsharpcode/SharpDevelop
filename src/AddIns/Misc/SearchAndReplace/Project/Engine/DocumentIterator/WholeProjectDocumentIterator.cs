// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop;
using System;
using System.Collections;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SearchAndReplace
{
	public class WholeProjectDocumentIterator : IDocumentIterator
	{
		ArrayList files    = new ArrayList();
		int       curIndex = -1;
		
		public WholeProjectDocumentIterator()
		{
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
				if (!File.Exists(files[curIndex].ToString())) {
					++curIndex;
					return Current;
				}
				IDocument document;
				string fileName = files[curIndex].ToString();
				foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
					if (content.PrimaryFileName != null &&
					    FileUtility.IsEqualFileName(content.PrimaryFileName, fileName) &&
					    content is ITextEditorProvider)
					{
						document = (((ITextEditorProvider)content).TextEditor).Document;
						return new ProvidedDocumentInformation(document,
						                                       fileName,
						                                       0);
					}
				}
				ITextBuffer fileContent;
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
			files.Clear();
			if (ProjectService.CurrentProject != null) {
				foreach (ProjectItem item in ProjectService.CurrentProject.Items) {
					if (item is FileProjectItem && SearchReplaceUtilities.IsSearchable(item.FileName)) {
						files.Add(item.FileName);
					}
				}
			}
		
			curIndex = -1;
		}
	}
}
