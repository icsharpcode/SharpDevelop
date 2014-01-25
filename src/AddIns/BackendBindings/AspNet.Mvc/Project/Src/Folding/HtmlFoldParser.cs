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
