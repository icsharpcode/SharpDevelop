// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit.Folding;

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public class HtmlFoldParser : IFoldParser
	{
		List<HtmlElementFold> folds = new List<HtmlElementFold>();
		Stack<HtmlElementFold> foldStack = new Stack<HtmlElementFold>();
		HtmlReader htmlReader;
		IHtmlReaderFactory htmlReaderFactory;
		
		public HtmlFoldParser(IHtmlReaderFactory htmlReaderFactory)
		{
			this.htmlReaderFactory = htmlReaderFactory;
		}
		
		public IEnumerable<NewFolding> GetFolds(string html)
		{
			ClearPreviousFolds();
			htmlReader = CreateHtmlReader(html);
			while (htmlReader.Read()) {
				if (htmlReader.IsEmptyElement) {
					// No folds for empty elements.
				} else if (htmlReader.IsEndElement) {
					AddFoldForCompletedElement();
				} else {
					SaveFoldStartOnStack();
				}
			}
			SortFoldsByStartOffset();
			return folds;
		}
		
		void ClearPreviousFolds()
		{
			folds.Clear();
		}
		
		HtmlReader CreateHtmlReader(string html)
		{
			return htmlReaderFactory.CreateHtmlReader(html);
		}
		
		void SaveFoldStartOnStack()
		{
			var fold = new HtmlElementFold() {
				ElementName = htmlReader.Value,
				StartOffset = htmlReader.Offset,
				Line = htmlReader.Line
			};
			foldStack.Push(fold);
		}
		
		void AddFoldForCompletedElement()
		{
			if (foldStack.Any()) {
				var fold = foldStack.Pop();
				if (fold.ElementName == htmlReader.Value) {
					fold.EndOffset = htmlReader.EndOffset;
					AddFoldIfEndElementOnDifferentLineToStartElement(fold);
				} else {
					AddFoldForCompletedElement();
				}
			}
		}
		
		void AddFoldIfEndElementOnDifferentLineToStartElement(HtmlElementFold fold)
		{
			if (htmlReader.Line > fold.Line) {
				folds.Add(fold);
			}
		}
		
		void SortFoldsByStartOffset()
		{
			folds.Sort((fold1, fold2) => fold1.StartOffset.CompareTo(fold2.StartOffset));
		}
	}
}
