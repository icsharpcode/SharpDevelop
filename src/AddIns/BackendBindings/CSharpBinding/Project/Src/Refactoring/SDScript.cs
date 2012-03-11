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
	sealed class SDScript : Script
	{
		int indentationSize = 4;
		readonly ITextEditor editor;
		readonly IDocument originalDocument;
		readonly TextSegmentCollection<TextSegment> textSegmentCollection;
		readonly IDisposable undoGroup;
		
		public SDScript(ITextEditor editor, string eolMarker) : base(eolMarker, new CSharpFormattingOptions())
		{
			this.editor = editor;
			this.textSegmentCollection = new TextSegmentCollection<TextSegment>((TextDocument)editor.Document);
			this.originalDocument = editor.Document.CreateDocumentSnapshot();
			undoGroup = editor.Document.OpenUndoGroup();
		}
		
		public override int GetCurrentOffset(TextLocation originalDocumentLocation)
		{
			int offset = originalDocument.GetOffset(originalDocumentLocation);
			return GetCurrentOffset(offset);
		}
		
		public override int GetCurrentOffset(int originalDocumentOffset)
		{
			return originalDocument.Version.MoveOffsetTo(editor.Document.Version, originalDocumentOffset, AnchorMovementType.Default);
		}
		
		public override void Replace(int offset, int length, string newText)
		{
			editor.Document.Replace(offset, length, newText);
		}
		
		protected override ISegment CreateTrackedSegment(int offset, int length)
		{
			var segment = new TextSegment();
			segment.StartOffset = offset;
			segment.Length = length;
			textSegmentCollection.Add(segment);
			return segment;
		}
		
		protected override int GetIndentLevelAt(int offset)
		{
			int oldOffset = editor.Document.Version.MoveOffsetTo(originalDocument.Version, offset, AnchorMovementType.Default);
			var line = editor.Document.GetLineByOffset(oldOffset);
			int spaces = 0;
			int indentationLevel = 0;
			for (int i = line.Offset; i < offset; i++) {
				char c = editor.Document.GetCharAt(i);
				if (c == '\t') {
					spaces = 0;
					indentationLevel++;
				} else if (c == ' ') {
					spaces++;
					if (spaces == indentationSize) {
						spaces = 0;
						indentationLevel++;
					}
				} else {
					break;
				}
			}
			return indentationLevel;
		}
		
		public override void Rename(IEntity entity, string name)
		{
			// TODO
		}
		
		public override void InsertWithCursor(string operation, AstNode node, InsertPosition defaultPosition)
		{
			// TODO
		}
		
		public override void FormatText(int offset, int length)
		{
			var parseInfo = ParserService.Parse(editor.FileName, editor.Document) as CSharpFullParseInformation;
			if (parseInfo != null) {
				//var startLocation = editor.Document.GetLocation(offset);
				//var endLocation = editor.Document.GetLocation(offset + length);
				//var node = parseInfo.CompilationUnit.GetNodeContaining(startLocation, endLocation);
				var formatter = new AstFormattingVisitor(new CSharpFormattingOptions(), editor.Document, false, 4);
				parseInfo.CompilationUnit.AcceptVisitor(formatter);
				formatter.ApplyChanges(offset, length);
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
		
		public override void Select(int offset, int length)
		{
			editor.Select(offset, length);
		}
		
		public override void Link(params AstNode[] nodes)
		{
			// TODO
		}
		
		public override void Dispose()
		{
			undoGroup.Dispose();
		}
	}
}
