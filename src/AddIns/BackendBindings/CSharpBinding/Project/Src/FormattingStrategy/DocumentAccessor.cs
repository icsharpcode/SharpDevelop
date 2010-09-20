// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit.Indentation.CSharp;
using ICSharpCode.SharpDevelop.Editor;

namespace CSharpBinding.FormattingStrategy
{
	/// <summary>
	/// Adapter IDocumentAccessor -> IDocument
	/// </summary>
	public sealed class DocumentAccessor : IDocumentAccessor
	{
		readonly IDocument doc;
		readonly int minLine;
		readonly int maxLine;
		
		/// <summary>
		/// Creates a new DocumentAccessor.
		/// </summary>
		public DocumentAccessor(IDocument document)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			doc = document;
			this.minLine = 1;
			this.maxLine = doc.TotalNumberOfLines;
		}
		
		/// <summary>
		/// Creates a new DocumentAccessor that indents only a part of the document.
		/// </summary>
		public DocumentAccessor(IDocument document, int minLine, int maxLine)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			doc = document;
			this.minLine = minLine;
			this.maxLine = maxLine;
		}
		
		int num;
		string text;
		IDocumentLine line;
		
		/// <inheritdoc/>
		public bool IsReadOnly {
			get {
				return num < minLine;
			}
		}
		
		/// <inheritdoc/>
		public int LineNumber {
			get {
				return num;
			}
		}
		
		bool lineDirty;
		
		/// <inheritdoc/>
		public string Text {
			get { return text; }
			set {
				if (num < minLine) return;
				text = value;
				lineDirty = true;
			}
		}
		
		/// <inheritdoc/>
		public bool MoveNext()
		{
			if (lineDirty) {
				DocumentUtilitites.SmartReplaceLine(doc, line, text);
				lineDirty = false;
			}
			++num;
			if (num > maxLine) return false;
			line = doc.GetLine(num);
			text = line.Text;
			return true;
		}
	}
}
