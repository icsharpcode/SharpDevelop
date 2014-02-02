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
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.AvalonEdit.Utils;
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
			layer.Exited += delegate(object s, InsertionCursorEventArgs args) {
				doc.UndoStack.StartContinuedUndoGroup();
				try {
					if (args.Success) {
						if (args.InsertionPoint.LineAfter == NewLineInsertion.None &&
						    args.InsertionPoint.LineBefore == NewLineInsertion.None && nodes.Count > 1) {
							args.InsertionPoint.LineAfter = NewLineInsertion.BlankLine;
						}

						int offset = currentScript.GetCurrentOffset(args.InsertionPoint.Location);
						int indentLevel = currentScript.GetIndentLevelAt(offset);
						
						foreach (var node in nodes.Reverse()) {
							var output = currentScript.OutputNode(indentLevel, node);
							int delta = args.InsertionPoint.Insert(target, output.Text);
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
			var part = parentType.Parts.FirstOrDefault ();
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
			
			if (declaringType.Kind == TypeKind.Enum) {
				foreach (var node in nodes.Reverse()) {
					int indentLevel = GetIndentLevelAt(area.Document.GetOffset(declaringType.BodyRegion.Begin));
					var output = OutputNode(indentLevel, node);
					var point = insertionPoints[0];
					var offset = area.Document.GetOffset(point.Location);
					var text = output.Text + ",";
					var delta = point.Insert(area.Document, text);
					output.RegisterTrackedSegments(script, delta + offset);
				}
				tcs.SetResult(script);
				return tcs.Task;
			}
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
	
	class InsertionCursorLayer : Canvas, IDisposable
	{
		readonly string operation;
		readonly InsertionPoint[] insertionPoints;
		readonly TextArea editor;
		
		public int CurrentInsertionPoint { get; set; }
		int insertionPointNextToMouse = -1;
		
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
			this.editor.TextView.InsertLayer(this, KnownLayer.Text, LayerInsertionPosition.Above);
			this.editor.TextView.ScrollOffsetChanged += TextViewScrollOffsetChanged;
			AddGroupBox();
			ScrollToInsertionPoint();
		}
		
		static readonly Pen markerPen = new Pen(Brushes.Blue, 1);
		static readonly Pen tempMarkerPen = new Pen(Brushes.Gray, 1);
		
		protected override void OnRender(DrawingContext drawingContext)
		{
			DrawLineForInsertionPoint(CurrentInsertionPoint, markerPen, drawingContext);
			if (insertionPointNextToMouse > -1 && insertionPointNextToMouse != CurrentInsertionPoint)
				DrawLineForInsertionPoint(insertionPointNextToMouse, tempMarkerPen, drawingContext);
			
			SetGroupBoxPosition(); // HACK
		}

		void DrawLineForInsertionPoint(int index, Pen pen, DrawingContext drawingContext)
		{
			var currentInsertionPoint = insertionPoints[index];
			var pos = editor.TextView.GetVisualPosition(new TextViewPosition(currentInsertionPoint.Location), VisualYPosition.LineTop);
			var endPos = new Point(pos.X + editor.TextView.ActualWidth * 0.6, pos.Y);
			pos -= editor.TextView.ScrollOffset;
			endPos -= editor.TextView.ScrollOffset;
			var pixelSize = PixelSnapHelpers.GetPixelSize(this);
			drawingContext.DrawLine(pen, PixelSnapHelpers.Round(pos, pixelSize), PixelSnapHelpers.Round(endPos, pixelSize));
		}
		
		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			return new PointHitTestResult(this, hitTestParameters.HitPoint);
		}
		
		protected override void OnMouseMove(MouseEventArgs e)
		{
			insertionPointNextToMouse = FindNextInsertionPoint(e.GetPosition(this));
			// don't set e.Handled = true; so that the event continues bubbling
			InvalidateVisual();
			base.OnMouseMove(e);
		}
		
		protected override void OnQueryCursor(QueryCursorEventArgs e)
		{
			if (FindNextInsertionPoint(e.GetPosition(this)) >= 0)
				e.Cursor = Cursors.UpArrow;
			else
				e.Cursor = Cursors.Arrow;
			e.Handled = true;
		}
		
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed) {
				if (e.ClickCount > 1) {
					FireExited(true);
				} else {
					insertionPointNextToMouse = FindNextInsertionPoint(e.GetPosition(this));
					if (insertionPointNextToMouse >= 0)
						CurrentInsertionPoint = insertionPointNextToMouse;
					InvalidateVisual();
				}
				e.Handled = true;
			}
			base.OnMouseDown(e);
		}

		int FindNextInsertionPoint(Point point)
		{
			var position = editor.TextView.GetPosition(point + editor.TextView.ScrollOffset);
			if (position == null) return -1;
			
			int insertionPoint = -1;
			int mouseLocationLine = position.Value.Location.Line;
			int currentLocationLine = -10;
			
			for (int i = 0; i < insertionPoints.Length; i++) {
				var line = insertionPoints[i].Location.Line;
				var diff = Math.Abs(line - mouseLocationLine);
				if (Math.Abs(currentLocationLine - mouseLocationLine) > diff && diff < 2) {
					insertionPoint = i;
					currentLocationLine = line;
				}
			}
			return insertionPoint;
		}
		
		void TextViewScrollOffsetChanged(object sender, EventArgs e)
		{
			InvalidateVisual();
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
				AddBinding(EditingCommands.MoveDownByPage, ModifierKeys.None, Key.PageDown, MoveMarkerPage(false));
				AddBinding(EditingCommands.MoveUpByPage, ModifierKeys.None, Key.PageUp, MoveMarkerPage(true));
				AddBinding(EditingCommands.MoveToLineStart, ModifierKeys.None, Key.Home, MoveMarkerFull(true));
				AddBinding(EditingCommands.MoveToLineEnd, ModifierKeys.None, Key.End, MoveMarkerFull(false));
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
					layer.ScrollToInsertionPoint();
				};
			}
			
			ExecutedRoutedEventHandler MoveMarkerPage(bool up)
			{
				return (sender, e) => {
					TextLocation current = layer.insertionPoints[layer.CurrentInsertionPoint].Location;
					double currentVPos = layer.editor.TextView.GetVisualTopByDocumentLine(current.Line);
					
					int newIndex = layer.CurrentInsertionPoint;
					
					double newVPos;
					do {
						if (up) {
							newIndex--;
							if (newIndex < 0) {
								newIndex = 0;
								break;
							}
						} else {
							newIndex++;
							if (newIndex >= layer.insertionPoints.Length) {
								newIndex = layer.insertionPoints.Length - 1;
								break;
							}
						}
						newVPos = layer.editor.TextView.GetVisualTopByDocumentLine(layer.insertionPoints[newIndex].Location.Line);
					} while (Math.Abs(currentVPos - newVPos) < layer.editor.ActualHeight);
					layer.CurrentInsertionPoint = newIndex;
					layer.InvalidateVisual();
					layer.ScrollToInsertionPoint();
				};
			}
			
			ExecutedRoutedEventHandler MoveMarkerFull(bool up)
			{
				return (sender, e) => {
					if (up)
						layer.CurrentInsertionPoint = 0;
					else
						layer.CurrentInsertionPoint = layer.insertionPoints.Length - 1;
					layer.InvalidateVisual();
					layer.ScrollToInsertionPoint();
				};
			}
		}
		
		public void Dispose()
		{
			editor.TextView.Layers.Remove(this);
			editor.ActiveInputHandler = editor.DefaultInputHandler;
			editor.TextView.ScrollOffsetChanged -= TextViewScrollOffsetChanged;
		}
		
		void InsertCode(object sender, ExecutedRoutedEventArgs e)
		{
			FireExited(true);
		}

		void ScrollToInsertionPoint()
		{
			var location = insertionPoints[CurrentInsertionPoint].Location;
			editor.GetService<TextEditor>().ScrollTo(location.Line, location.Column);
			SetGroupBoxPosition();
		}

		void SetGroupBoxPosition()
		{
			var location = insertionPoints[CurrentInsertionPoint].Location;
			var boxPosition = editor.TextView.GetVisualPosition(new TextViewPosition(location), VisualYPosition.LineMiddle) - editor.TextView.ScrollOffset + new Vector(editor.TextView.ActualWidth * 0.6 - 5, -groupBox.ActualHeight / 2.0);
			Canvas.SetTop(groupBox, boxPosition.Y);
			Canvas.SetLeft(groupBox, boxPosition.X);
		}
		
		void Cancel(object sender, ExecutedRoutedEventArgs e)
		{
			FireExited(false);
		}

		void FireExited(bool success)
		{
			if (Exited != null) {
				Exited(this, new InsertionCursorEventArgs(insertionPoints[CurrentInsertionPoint], success));
			}
		}
		
		GroupBox groupBox;
		
		void AddGroupBox()
		{
			var content = new StackPanel {
				Children = {
					new TextBlock {
						Text = "Use Up/Down to move to another location.\r\n" +
							"Press Enter to select the location.\r\n" +
							"Press Esc to cancel this operation."
					}
				}
			};
			
			groupBox = new GroupBox {
				Background = Brushes.White,
				BorderBrush = Brushes.Blue,
				BorderThickness = new Thickness(1),
				Header = operation,
				Content = content
			};
			
			Children.Add(groupBox);
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
