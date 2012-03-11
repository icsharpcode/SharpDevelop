// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using CSharpBinding.Parser;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Parser;

namespace CSharpBinding.Refactoring
{
	/// <summary>
	/// Refactoring change script.
	/// </summary>
	sealed class SDScript : DocumentScript
	{
		readonly ITextEditor editor;
		readonly TextSegmentCollection<TextSegment> textSegmentCollection;
		
		public SDScript(ITextEditor editor, string eolMarker) : base(editor.Document, new CSharpFormattingOptions())
		{
			this.editor = editor;
			this.eolMarker = eolMarker;
			this.textSegmentCollection = new TextSegmentCollection<TextSegment>((TextDocument)editor.Document);
		}
		
		protected override ISegment CreateTrackedSegment(int offset, int length)
		{
			var segment = new TextSegment();
			segment.StartOffset = offset;
			segment.Length = length;
			textSegmentCollection.Add(segment);
			return segment;
		}
		
		public override void FormatText(AstNode node)
		{
			var parseInfo = ParserService.Parse(editor.FileName, editor.Document) as CSharpFullParseInformation;
			if (parseInfo != null) {
				//var startLocation = editor.Document.GetLocation(offset);
				//var endLocation = editor.Document.GetLocation(offset + length);
				//var node = parseInfo.CompilationUnit.GetNodeContaining(startLocation, endLocation);
				var formatter = new AstFormattingVisitor(new CSharpFormattingOptions(), editor.Document, false, 4);
				parseInfo.CompilationUnit.AcceptVisitor(formatter);
				var segment = GetSegment(node);
				formatter.ApplyChanges(segment.Offset, segment.Length);
			} else {
				base.FormatText(node);
			}
		}
		
		public override void Select(AstNode node)
		{
			var segment = GetSegment(node);
			int startOffset = segment.Offset;
			int endOffset = segment.EndOffset;
			// If the area to select includes a newline (e.g. node is a statement),
			// exclude that newline from the selection.
			if (endOffset > startOffset && editor.Document.GetLineByOffset(endOffset).Offset == endOffset) {
				endOffset = editor.Document.GetLineByOffset(endOffset).PreviousLine.EndOffset;
			}
			editor.Select(startOffset, endOffset - startOffset);
		}
		
		public override void Link(params AstNode[] nodes)
		{
			// TODO
		}
	}
}
