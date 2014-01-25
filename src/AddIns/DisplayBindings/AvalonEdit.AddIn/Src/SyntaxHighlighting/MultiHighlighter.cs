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
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.NRefactory.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public class MultiHighlighter : IHighlighter
	{
		readonly IHighlighter[] nestedHighlighters;
		readonly IDocument document;
		
		public MultiHighlighter(IDocument document, params IHighlighter[] nestedHighlighters)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			if (nestedHighlighters == null)
				throw new ArgumentNullException("additionalHighlighters");

			foreach (var highlighter in nestedHighlighters) {
				if (highlighter == null)
					throw new ArgumentException("nulls not allowed!");
				if (document != highlighter.Document)
					throw new ArgumentException("all highlighters must be assigned to the same document!");
			}
			
			this.nestedHighlighters = nestedHighlighters;
			this.document = document;
		}
		
		public event HighlightingStateChangedEventHandler HighlightingStateChanged {
			add {
				foreach (var highlighter in nestedHighlighters) {
					highlighter.HighlightingStateChanged += value;
				}
			}
			remove {
				foreach (var highlighter in nestedHighlighters) {
					highlighter.HighlightingStateChanged -= value;
				}
			}
		}
		
		public IDocument Document {
			get {
				return document;
			}
		}
		
		public HighlightingColor DefaultTextColor {
			get {
				if (nestedHighlighters.Length > 0)
					return nestedHighlighters[0].DefaultTextColor;
				return null;
			}
		}
		
		public IEnumerable<HighlightingColor> GetColorStack(int lineNumber)
		{
			List<HighlightingColor> list = new List<HighlightingColor>();
			for (int i = nestedHighlighters.Length - 1; i >= 0; i--) {
				var s = nestedHighlighters[i].GetColorStack(lineNumber);
				if (s != null)
					list.AddRange(s);
			}
			return list;
		}
		
		public HighlightedLine HighlightLine(int lineNumber)
		{
			HighlightedLine line = new HighlightedLine(document, document.GetLineByNumber(lineNumber));
			foreach (IHighlighter h in nestedHighlighters) {
				line.MergeWith(h.HighlightLine(lineNumber));
			}
			return line;
		}
		
		public void UpdateHighlightingState(int lineNumber)
		{
			foreach (var h in nestedHighlighters) {
				h.UpdateHighlightingState(lineNumber);
			}
		}
		
		public void BeginHighlighting()
		{
			foreach (var h in nestedHighlighters) {
				h.BeginHighlighting();
			}
		}
		
		public void EndHighlighting()
		{
			foreach (var h in nestedHighlighters) {
				h.EndHighlighting();
			}
		}
		
		public HighlightingColor GetNamedColor(string name)
		{
			foreach (var h in nestedHighlighters) {
				var color = h.GetNamedColor(name);
				if (color != null)
					return color;
			}
			return null;
		}
		
		public void Dispose()
		{
			foreach (var h in nestedHighlighters) {
				h.Dispose();
			}
		}
	}
}
