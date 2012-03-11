// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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
using System.Diagnostics;
using ICSharpCode.NRefactory.Editor;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	/// <summary>
	/// Script implementation based on IDocument.
	/// </summary>
	public class DocumentScript : Script
	{
		readonly IDocument currentDocument;
		readonly IDocument originalDocument;
		readonly IDisposable undoGroup;
		
		public DocumentScript(IDocument document, CSharpFormattingOptions formattingOptions) : base(formattingOptions)
		{
			this.originalDocument = document.CreateDocumentSnapshot();
			this.currentDocument = document;
			Debug.Assert(currentDocument.Version.CompareAge(originalDocument.Version) == 0);
			this.undoGroup = document.OpenUndoGroup();
		}
		
		public override void Dispose()
		{
			if (undoGroup != null)
				undoGroup.Dispose();
			base.Dispose();
		}
		
		public override void Replace(int offset, int length, string newText)
		{
			currentDocument.Replace(offset, length, newText);
		}
		
		public override string GetText(int offset, int length)
		{
			return currentDocument.GetText(offset, length);
		}
		
		public override IDocumentLine GetLineByOffset(int offset)
		{
			return currentDocument.GetLineByOffset(offset);
		}
		
		public override int GetCurrentOffset(TextLocation originalDocumentLocation)
		{
			int offset = originalDocument.GetOffset(originalDocumentLocation);
			return GetCurrentOffset(offset);
		}
		
		public override int GetCurrentOffset(int originalDocumentOffset)
		{
			return originalDocument.Version.MoveOffsetTo(currentDocument.Version, originalDocumentOffset, AnchorMovementType.Default);
		}
		
		public override void FormatText(int offset, int length)
		{
			var cu = CompilationUnit.Parse(currentDocument, "dummy.cs");
			var formatter = new AstFormattingVisitor(this.FormattingOptions, currentDocument);
			cu.AcceptVisitor(formatter);
			formatter.ApplyChanges(offset, length);
		}
		
		protected override int GetIndentLevelAt(int offset)
		{
			int oldOffset = currentDocument.Version.MoveOffsetTo(originalDocument.Version, offset, AnchorMovementType.Default);
			var line = currentDocument.GetLineByOffset(oldOffset);
			int spaces = 0;
			int indentationLevel = 0;
			for (int i = line.Offset; i < offset; i++) {
				char c = currentDocument.GetCharAt(i);
				if (c == '\t') {
					spaces = 0;
					indentationLevel++;
				} else if (c == ' ') {
					spaces++;
					if (spaces == 4) {
						spaces = 0;
						indentationLevel++;
					}
				} else {
					break;
				}
			}
			return indentationLevel;
		}
		
		protected override ISegment CreateTrackedSegment(int offset, int length)
		{
			return new TrackedSegment(this, offset, offset + length);
		}
		
		sealed class TrackedSegment : ISegment
		{
			readonly DocumentScript script;
			readonly ITextSourceVersion originalVersion;
			readonly int originalStart;
			readonly int originalEnd;
			
			public TrackedSegment(DocumentScript script, int originalStart, int originalEnd)
			{
				this.script = script;
				this.originalVersion = script.currentDocument.Version;
				this.originalStart = originalStart;
				this.originalEnd = originalEnd;
			}
			
			public int Offset {
				get { return originalVersion.MoveOffsetTo(script.currentDocument.Version, originalStart, AnchorMovementType.Default); }
			}
			
			public int Length {
				get { return this.EndOffset - this.Offset; }
			}
			
			public int EndOffset {
				get { return originalVersion.MoveOffsetTo(script.currentDocument.Version, originalEnd, AnchorMovementType.Default); }
			}
		}
	}
}
