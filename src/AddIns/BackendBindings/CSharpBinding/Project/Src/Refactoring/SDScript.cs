// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;

namespace CSharpBinding.Refactoring
{
	/// <summary>
	/// Refactoring change script.
	/// </summary>
	sealed class SDScript : Script
	{
		int indentationSize = 4;
		readonly ITextEditor editor;
		readonly TextSegmentCollection<TextSegment> textSegmentCollection = new TextSegmentCollection<TextSegment>();
		readonly OffsetChangeMap offsetChangeMap = new OffsetChangeMap();
		readonly List<Action> actions = new List<Action>();
		
		public SDScript(ITextEditor editor, string eolMarker) : base(eolMarker, new CSharpFormattingOptions())
		{
			this.editor = editor;
		}
		
		public override int GetCurrentOffset(TextLocation originalDocumentLocation)
		{
			int offset = editor.Document.GetOffset(originalDocumentLocation);
			return offsetChangeMap.GetNewOffset(offset, AnchorMovementType.Default);
		}
		
		public override int GetCurrentOffset(int originalDocumentOffset)
		{
			return offsetChangeMap.GetNewOffset(originalDocumentOffset, AnchorMovementType.Default);
		}
		
		public override void Replace(int offset, int length, string newText)
		{
			var changeMapEntry = new OffsetChangeMapEntry(offset, length, newText.Length);
			textSegmentCollection.UpdateOffsets(changeMapEntry);
			offsetChangeMap.Add(changeMapEntry);
			
			actions.Add(delegate { editor.Document.Replace(offset, length, newText); });
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
			int oldOffset = offsetChangeMap.Invert().GetNewOffset(offset, AnchorMovementType.Default);
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
		}
		
		public override void InsertWithCursor(string operation, AstNode node, InsertPosition defaultPosition)
		{
		}
		
		public override void FormatText(int offset, int length)
		{
		}
		
		public override void Select(int offset, int length)
		{
			actions.Add(delegate { editor.Select(offset, length); });
		}
		
		public override void Link(params AstNode[] nodes)
		{
		}
		
		public override void Dispose()
		{
			foreach (var action in actions)
				action();
		}
	}
}
