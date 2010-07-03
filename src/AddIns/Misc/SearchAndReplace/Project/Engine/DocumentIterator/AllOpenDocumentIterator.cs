// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Editor;
using System;
using System.Linq;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SearchAndReplace
{
	public class AllOpenDocumentIterator : IDocumentIterator
	{
		int  startIndex = -1;
		int  curIndex   = -1;
		bool resetted   = true;
		
		public AllOpenDocumentIterator()
		{
			Reset();
		}
		
		public string CurrentFileName {
			get {
				IViewContent viewContent = GetCurrentTextEditorViewContent();
				if (viewContent != null) {
					return viewContent.PrimaryFileName;
				}
				return null;
			}
		}
		
		IViewContent GetCurrentTextEditorViewContent()
		{
			GetCurIndex();
			if (curIndex >= 0) {
				IViewContent viewContent = WorkbenchSingleton.Workbench.ViewContentCollection.ToList()[curIndex];
				if (viewContent is ITextEditorProvider) {
					return viewContent;
				}
			}
			return null;
		}
		
		public ProvidedDocumentInformation Current {
			get {
				IViewContent viewContent = GetCurrentTextEditorViewContent();
				if (viewContent != null) {
					ITextEditor textEditor = (((ITextEditorProvider)viewContent).TextEditor);
					return new ProvidedDocumentInformation(textEditor.Document,
					                                       CurrentFileName,
					                                       textEditor);
				}
				return null;
			}
		}
		
		void GetCurIndex()
		{
			IViewContent[] viewContentCollection = WorkbenchSingleton.Workbench.ViewContentCollection.ToArray();
			int viewCount = WorkbenchSingleton.Workbench.ViewContentCollection.Count;
			if (curIndex == -1 || curIndex >= viewCount) {
				for (int i = 0; i < viewCount; ++i) {
					if (WorkbenchSingleton.Workbench.ActiveViewContent == viewContentCollection[i]) {
						curIndex = i;
						return;
					}
				}
				curIndex = -1;
			}
		}
		
		public bool MoveForward()
		{
			GetCurIndex();
			if (curIndex < 0) {
				return false;
			}
			
			if (resetted) {
				resetted = false;
				return true;
			}
			
			curIndex = (curIndex + 1) % WorkbenchSingleton.Workbench.ViewContentCollection.Count;
			if (curIndex == startIndex) {
				return false;
			}
			return true;
		}
		
		public bool MoveBackward()
		{
			GetCurIndex();
			if (curIndex < 0) {
				return false;
			}
			if (resetted) {
				resetted = false;
				return true;
			}
			
			if (curIndex == 0) {
				curIndex = WorkbenchSingleton.Workbench.ViewContentCollection.Count - 1;
			}
			
			if (curIndex > 0) {
				--curIndex;
				return true;
			}
			return false;
		}
		
		public void Reset()
		{
			curIndex = -1;
			GetCurIndex();
			startIndex = curIndex;
			resetted = true;
		}
	}
}
