// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Highlighting
{
	/// <summary>
	/// A RichTextWriter that writes into a document and .
	/// </summary>
	public class RichTextModelWriter : PlainRichTextWriter
	{
		readonly RichTextModel richTextModel;
		readonly DocumentTextWriter documentTextWriter;
		readonly Stack<HighlightingColor> colorStack = new Stack<HighlightingColor>();
		HighlightingColor currentColor;
		int currentColorBegin = -1;
		
		/// <summary>
		/// Creates a new RichTextModelWriter that inserts into document, starting at insertionOffset.
		/// </summary>
		public RichTextModelWriter(RichTextModel richTextModel, IDocument document, int insertionOffset)
			: base(new DocumentTextWriter(document, insertionOffset))
		{
			if (richTextModel == null)
				throw new ArgumentNullException("richTextModel");
			this.richTextModel = richTextModel;
			this.documentTextWriter = (DocumentTextWriter)base.textWriter;
			if (richTextModel.DocumentLength == 0)
				currentColor = HighlightingColor.Empty;
			else
				currentColor = richTextModel.GetHighlightingAt(Math.Max(0, insertionOffset - 1));
		}
		
		/// <summary>
		/// Gets/Sets the current insertion offset.
		/// </summary>
		public int InsertionOffset {
			get { return documentTextWriter.InsertionOffset; }
			set { documentTextWriter.InsertionOffset = value; }
		}
		
		
		/// <inheritdoc/>
		protected override void BeginUnhandledSpan()
		{
			colorStack.Push(currentColor);
		}
		
		void BeginColorSpan()
		{
			WriteIndentationIfNecessary();
			colorStack.Push(currentColor);
			currentColor = currentColor.Clone();
			currentColorBegin = documentTextWriter.InsertionOffset;
		}
		
		/// <inheritdoc/>
		public override void EndSpan()
		{
			currentColor = colorStack.Pop();
			currentColorBegin = documentTextWriter.InsertionOffset;
		}
		
		/// <inheritdoc/>
		protected override void AfterWrite()
		{
			base.AfterWrite();
			richTextModel.SetHighlighting(currentColorBegin, documentTextWriter.InsertionOffset - currentColorBegin, currentColor);
		}
		
		/// <inheritdoc/>
		public override void BeginSpan(Color foregroundColor)
		{
			BeginColorSpan();
			currentColor.Foreground = new SimpleHighlightingBrush(foregroundColor);
			currentColor.Freeze();
		}
		
		/// <inheritdoc/>
		public override void BeginSpan(FontFamily fontFamily)
		{
			BeginUnhandledSpan(); // TODO
		}
		
		/// <inheritdoc/>
		public override void BeginSpan(FontStyle fontStyle)
		{
			BeginColorSpan();
			currentColor.FontStyle = fontStyle;
			currentColor.Freeze();
		}
		
		/// <inheritdoc/>
		public override void BeginSpan(FontWeight fontWeight)
		{
			BeginColorSpan();
			currentColor.FontWeight = fontWeight;
			currentColor.Freeze();
		}
		
		/// <inheritdoc/>
		public override void BeginSpan(HighlightingColor highlightingColor)
		{
			BeginColorSpan();
			currentColor.MergeWith(highlightingColor);
			currentColor.Freeze();
		}
	}
}
