// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using CSharpBinding.Parser;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Parser;

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
		
		static readonly Task completedTask = Task.FromResult<object>(null);
		
		public override Task Link(params AstNode[] nodes)
		{
			// TODO
			return completedTask;
		}
		
		public override Task<Script> InsertWithCursor(string operation, InsertPosition defaultPosition, IList<AstNode> nodes)
		{
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
			area.TextView.InsertLayer(layer, KnownLayer.Text, LayerInsertionPosition.Above);

			switch (defaultPosition) {
				case InsertPosition.Start:
					layer.CurrentInsertionPoint = 0;
					break;
				case InsertPosition.End:
					layer.CurrentInsertionPoint = insertionPoints.Count - 1;
					break;
				case InsertPosition.Before:
					for (int i = 0; i < insertionPoints.Count; i++) {
						if (insertionPoints[i].Location < loc)
							layer.CurrentInsertionPoint = i;
					}
					break;
				case InsertPosition.After:
					for (int i = 0; i < insertionPoints.Count; i++) {
						if (insertionPoints[i].Location > loc) {
							layer.CurrentInsertionPoint = i;
							break;
						}
					}
					break;
			}

			layer.Exited += delegate(object s, InsertionCursorEventArgs args) {
				if (args.Success) {
					if (args.InsertionPoint.LineAfter == NewLineInsertion.None &&
					    args.InsertionPoint.LineBefore == NewLineInsertion.None && nodes.Count > 1) {
						args.InsertionPoint.LineAfter = NewLineInsertion.BlankLine;
					}
					foreach (var node in nodes.Reverse ()) {
						int indentLevel = GetIndentLevelAt(editor.Document.GetOffset(insertionPoints[0].Location));
						var output = OutputNode(indentLevel, node);
						var offset = editor.Document.GetOffset(args.InsertionPoint.Location);
						var delta = args.InsertionPoint.Insert (editor.Document, output.Text);
						output.RegisterTrackedSegments (this, delta + offset);
					}
					tcs.SetResult(this);
				}
				area.TextView.Layers.Remove(layer);
				layer.Dispose();
			};
			return tcs.Task;
		}
		
		public override Task<Script> InsertWithCursor(string operation, ITypeDefinition parentType, Func<Script, RefactoringContext, IList<AstNode>> nodeCallback)
		{
			return base.InsertWithCursor(operation, parentType, nodeCallback);
		}
		
		public override void Dispose()
		{
			base.Dispose();
			// refresh parse information so that the issue can disappear immediately
			SD.ParserService.ParseAsync(editor.FileName, editor.Document).FireAndForget();
		}
	}
	
	class InsertionCursorLayer : UIElement, IDisposable
	{
		string operation;
		InsertionPoint[] insertionPoints;
		readonly TextArea editor;
		
		public int CurrentInsertionPoint { get; set; }
		
		public event EventHandler<InsertionCursorEventArgs> Exited;
		
		public static readonly RoutedCommand ExitCommand = new RoutedCommand(
			"Exit", typeof(InsertionCursorLayer),
			new InputGestureCollection { new KeyGesture(Key.Escape) }
		);
		
		public InsertionCursorLayer(TextArea editor, string operation, IList<InsertionPoint> insertionPoints)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			this.editor = editor;
			this.operation = operation;
			this.insertionPoints = insertionPoints.ToArray();
			this.editor.ActiveInputHandler = new InputHandler(this);
		}
		
		static readonly Pen markerPen = new Pen(Brushes.Blue, 1);
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			var currentInsertionPoint = insertionPoints[CurrentInsertionPoint];
			var pos = editor.TextView.GetVisualPosition(new TextViewPosition(currentInsertionPoint.Location), VisualYPosition.LineMiddle);
			var endPos = new Point(pos.X + editor.TextView.ActualWidth * 0.6, pos.Y);
			drawingContext.DrawLine(markerPen, pos, endPos);
		}
		
		class InputHandler : TextAreaDefaultInputHandler
		{
			readonly InsertionCursorLayer layer;
			
			internal InputHandler(InsertionCursorLayer layer)
				: base(layer.editor)
			{
				this.layer = layer;
				AddBinding(EditingCommands.MoveDownByLine, ModifierKeys.None, Key.Down, MoveMarker(false));
				AddBinding(EditingCommands.MoveUpByLine, ModifierKeys.None, Key.Up, MoveMarker(true));
				AddBinding(EditingCommands.EnterParagraphBreak, ModifierKeys.None, Key.Enter, layer.InsertCode);
				AddBinding(ExitCommand, ModifierKeys.None, Key.Escape, layer.Cancel);
			}
			
			ExecutedRoutedEventHandler MoveMarker(bool up)
			{
				return (sender, e) => {
					if (up)
						layer.CurrentInsertionPoint = Math.Max(0, layer.CurrentInsertionPoint - 1);
					else
						layer.CurrentInsertionPoint = Math.Min(layer.insertionPoints.Length - 1, layer.CurrentInsertionPoint + 1);
					layer.InvalidateVisual();
				};
			}
		}
		
		public void Dispose()
		{
			this.editor.ActiveInputHandler = this.editor.DefaultInputHandler;
		}
		
		void InsertCode(object sender, ExecutedRoutedEventArgs e)
		{
			if (Exited != null) {
				Exited(this, new InsertionCursorEventArgs(insertionPoints[CurrentInsertionPoint], true));
			}
		}
		
		void Cancel(object sender, ExecutedRoutedEventArgs e)
		{
			if (Exited != null) {
				Exited(this, new InsertionCursorEventArgs(insertionPoints[CurrentInsertionPoint], false));
			}
		}
	}
	
	public class InsertionCursorEventArgs : EventArgs
	{
		public InsertionPoint InsertionPoint { get; private set; }
		public bool Success { get; private set; }
		
		public InsertionCursorEventArgs(InsertionPoint insertionPoint, bool success)
		{
			if (insertionPoint == null)
				throw new ArgumentNullException("insertionPoint");
			this.InsertionPoint = insertionPoint;
			this.Success = success;
		}
	}
}
