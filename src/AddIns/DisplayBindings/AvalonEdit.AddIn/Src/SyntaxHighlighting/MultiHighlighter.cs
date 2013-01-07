// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
