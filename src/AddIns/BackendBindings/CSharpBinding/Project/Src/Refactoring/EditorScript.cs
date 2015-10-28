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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Commands;
using ICSharpCode.SharpDevelop.Editor.Dialogs;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Refactoring;
using ICSharpCode.SharpDevelop.Workbench;

namespace CSharpBinding.Refactoring
{
	/// <summary>
	/// Refactoring change script.
	/// </summary>
	sealed class EditorScript : DocumentScript
	{
		readonly ITextEditor editor;
		readonly TextSegmentCollection<TextSegment> textSegmentCollection;
		readonly SDRefactoringContext context;
		
		public EditorScript(ITextEditor editor, SDRefactoringContext context, CSharpFormattingOptions formattingOptions)
			: base(editor.Document, formattingOptions, context.TextEditorOptions)
		{
			this.editor = editor;
			this.context = context;
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
		
		public override void Select(int startOffset, int endOffset)
		{
			editor.Select(Math.Min(startOffset, endOffset), Math.Abs(endOffset - startOffset));
		}
		
		public override void Select(TextLocation start, TextLocation end)
		{
			Select(editor.Document.GetOffset(start), editor.Document.GetOffset(end));
		}
		
		public override Task Link(params AstNode[] nodes)
		{
			var segs = nodes.Select(node => GetSegment(node)).ToArray();
			InsertionContext c = new InsertionContext(editor.GetRequiredService<TextArea>(), segs.Min(seg => seg.Offset));
			c.InsertionPosition = segs.Max(seg => seg.EndOffset);
			
			var tcs = new TaskCompletionSource<bool>();
			c.Deactivated += (sender, e) => tcs.SetResult(true);
			
			if (segs.Length > 0) {
				// try to use node in identifier context to avoid the code completion popup.
				var identifier = nodes.OfType<Identifier>().FirstOrDefault();
				ISegment first;
				if (identifier == null)
					first = segs[0];
				else
					first = GetSegment(identifier);
				c.Link(first, segs.Except(new[]{first}).ToArray());
				c.RaiseInsertionCompleted(EventArgs.Empty);
			} else {
				c.RaiseInsertionCompleted(EventArgs.Empty);
				c.Deactivate(new SnippetEventArgs(DeactivateReason.NoActiveElements));
			}
			
			return tcs.Task;
		}
		
		public override void Rename(ISymbol symbol, string name = null)
		{
			RenameSymbolCommand.RunRename(symbol, name);
		}
		
		public override Task<Script> InsertWithCursor(string operation, InsertPosition defaultPosition, IList<AstNode> nodes)
		{
			// TODO : Use undo group
			var tcs = new TaskCompletionSource<Script>();
			var loc = editor.Caret.Location;
			var currentPart = context.UnresolvedFile.GetInnermostTypeDefinition(loc);
			var insertionPoints = InsertionPoint.GetInsertionPoints(editor.Document, currentPart);
			
			if (insertionPoints.Count == 0) {
				SD.MessageService.ShowErrorFormatted("No valid insertion point can be found in type '{0}'.", currentPart.Name);
				return tcs.Task;
			}
			
			TextArea area = editor.GetService<TextArea>();
			if (area == null) return tcs.Task;
			
			var layer = new InsertionCursorLayer(area, operation, insertionPoints);
			
			switch (defaultPosition) {
				case InsertPosition.Start:
					layer.CurrentInsertionPointIndex = 0;
					break;
				case InsertPosition.End:
					layer.CurrentInsertionPointIndex = insertionPoints.Count - 1;
					break;
				case InsertPosition.Before:
					for (int i = 0; i < insertionPoints.Count; i++) {
						if (insertionPoints[i].Location < loc)
							layer.CurrentInsertionPointIndex = i;
					}
					break;
				case InsertPosition.After:
					for (int i = 0; i < insertionPoints.Count; i++) {
						if (insertionPoints[i].Location > loc) {
							layer.CurrentInsertionPointIndex = i;
							break;
						}
					}
					break;
			}
			InsertWithCursorOnLayer(this, layer, tcs, nodes, editor.Document);
			return tcs.Task;
		}
		
		void InsertWithCursorOnLayer(EditorScript currentScript, InsertionCursorLayer layer, TaskCompletionSource<Script> tcs, IList<AstNode> nodes, IDocument target)
		{
			var doc = target as TextDocument;
			var op = new UndoOperation(layer, tcs);
			if (doc != null) {
				doc.UndoStack.Push(op);
			}
			layer.ScrollToInsertionPoint();
			layer.Exited += delegate(object s, InsertionCursorEventArgs args) {
				doc.UndoStack.StartContinuedUndoGroup();
				try {
					if (args.Success) {
						if (args.InsertionPoint.LineAfter == NewLineInsertion.None &&
						    args.InsertionPoint.LineBefore == NewLineInsertion.None && nodes.Count > 1) {
							args.InsertionPoint.LineAfter = NewLineInsertion.BlankLine;
						}
						
						var insertionPoint = args.InsertionPoint;
						if (nodes.All(n => n is EnumMemberDeclaration)) {
							insertionPoint.LineAfter = NewLineInsertion.Eol;
							insertionPoint.LineBefore = NewLineInsertion.None;
						}

						int offset = currentScript.GetCurrentOffset(insertionPoint.Location);
						int indentLevel = currentScript.GetIndentLevelAt(Math.Max(0, offset - 1));
						
						foreach (var node in nodes.Reverse()) {
							var output = currentScript.OutputNode(indentLevel, node);
							var text = output.Text;
							if (node is EnumMemberDeclaration) {
								if (insertionPoint != layer.InsertionPoints.Last()) {
									text += ",";
								} else {
									var parentEnum = currentScript.context.RootNode.GetNodeAt(insertionPoint.Location, n => (n is TypeDeclaration) && ((TypeDeclaration)n).ClassType == ClassType.Enum) as TypeDeclaration;
									if (parentEnum != null) {
										var lastMember = parentEnum.Members.LastOrDefault();
										if (lastMember != null) {
											var segment = currentScript.GetSegment(lastMember);
											currentScript.InsertText(segment.EndOffset, ",");
										}
									}
								}
							}
							int delta = insertionPoint.Insert(target, text);
							output.RegisterTrackedSegments(currentScript, delta + offset);
						}
						currentScript.FormatText(nodes);
						tcs.SetResult(currentScript);
					}
					layer.Dispose();
					DisposeOnClose();
				} finally {
					doc.UndoStack.EndUndoGroup();
				}
				op.Reset();
			};
		}
		
		readonly List<Script> startedScripts = new List<Script>();
		
		public override Task<Script> InsertWithCursor(string operation, ITypeDefinition parentType, Func<Script, RefactoringContext, IList<AstNode>> nodeCallback)
		{
			// TODO : Use undo group
			var tcs = new TaskCompletionSource<Script>();
			if (parentType == null)
				return tcs.Task;
			IUnresolvedTypeDefinition part = null;
			
			foreach (var p in parentType.Parts) {
				if (part == null || EntityModelContextUtils.IsBetterPart(p, part, ".cs"))
					part = p;
			}
			
			if (part == null)
				return tcs.Task;
			
			var fileName = new ICSharpCode.Core.FileName(part.Region.FileName);
			IViewContent document = SD.FileService.OpenFile(fileName);
			var area = document.GetService<TextArea>();
			
			if (area == null) return tcs.Task;
			
			var loc = part.Region.Begin;
			var parsedFile = SD.ParserService.ParseFile(fileName, area.Document, cancellationToken: context.CancellationToken);
			var declaringType = parsedFile.GetInnermostTypeDefinition(loc);
			EditorScript script;

			if (area.Document != context.Document) {
				script = new EditorScript(area.GetService<ITextEditor>(), SDRefactoringContext.Create(fileName, area.Document, loc, context.CancellationToken), FormattingOptions);
				startedScripts.Add(script);
			} else {
				script = this;
			}
			var nodes = nodeCallback (script, script.context);
			var insertionPoints = InsertionPoint.GetInsertionPoints(area.Document, part);
			
			if (insertionPoints.Count == 0) {
				SD.MessageService.ShowErrorFormatted("No valid insertion point can be found in type '{0}'.", part.Name);
				return tcs.Task;
			}
			
			var layer = new InsertionCursorLayer(area, operation, insertionPoints);
			area.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)area.TextView.InvalidateVisual);
			
			InsertWithCursorOnLayer(script, layer, tcs, nodes, area.Document);
			return tcs.Task;
		}
		
		bool isDisposed;
		void DisposeOnClose(bool force = false)
		{
			if (isDisposed)
				return;
			isDisposed = true;
			base.Dispose();
			// refresh parse information so that the issue can disappear immediately
			SD.ParserService.ParseAsync(editor.FileName, editor.Document).FireAndForget();
			foreach (var script in startedScripts)
				script.Dispose();
		}
		
		public override void Dispose()
		{
			DisposeOnClose();
		}
		
		class UndoOperation : IUndoableOperation
		{
			InsertionCursorLayer layer;
			TaskCompletionSource<Script> tcs;
			
			public UndoOperation(InsertionCursorLayer layer, TaskCompletionSource<Script> tcs)
			{
				this.layer = layer;
				this.tcs = tcs;
			}
			
			public void Reset()
			{
				layer = null;
				tcs = null;
			}
			
			public void Undo()
			{
				if (layer != null)
					layer.Dispose();
				layer = null;
				if (tcs != null)
					tcs.SetCanceled();
				tcs = null;
			}
			
			public void Redo()
			{
			}
		}
	}
}
