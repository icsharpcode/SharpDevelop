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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
namespace CSharpBinding.Refactoring
{
	class InsertionCursorLayer : Canvas, IDisposable
	{
		readonly string operation;

		readonly InsertionPoint[] insertionPoints;

		readonly TextArea editor;

		public int CurrentInsertionPoint {
			get;
			set;
		}

		int insertionPointNextToMouse = -1;

		public event EventHandler<InsertionCursorEventArgs> Exited;

		public static readonly RoutedCommand ExitCommand = new RoutedCommand("Exit", typeof(InsertionCursorLayer), new InputGestureCollection {
		                                                                     	new KeyGesture(Key.Escape)
		                                                                     });

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
		}

		static readonly Pen markerPen = new Pen(Brushes.Blue, 2);

		static readonly Pen tempMarkerPen = new Pen(Brushes.Gray, 2);

		protected override void OnRender(DrawingContext drawingContext)
		{
			DrawLineForInsertionPoint(CurrentInsertionPoint, markerPen, drawingContext);
			if (insertionPointNextToMouse > -1 && insertionPointNextToMouse != CurrentInsertionPoint)
				DrawLineForInsertionPoint(insertionPointNextToMouse, tempMarkerPen, drawingContext);
			SetGroupBoxPosition();
			// HACK: why OnRender() override? we could just use Line objects instead
		}

		Point GetLinePosition(int index)
		{
			var currentInsertionPoint = insertionPoints[index];
			var textViewPosition = new TextViewPosition(currentInsertionPoint.Location);
			bool isEmptyLine = DocumentUtilities.IsEmptyLine(editor.Document, textViewPosition.Line);
			var pos = editor.TextView.GetVisualPosition(textViewPosition, isEmptyLine ? VisualYPosition.LineMiddle : VisualYPosition.LineTop);
			return pos - editor.TextView.ScrollOffset;
		}
		
		void DrawLineForInsertionPoint(int index, Pen pen, DrawingContext drawingContext)
		{
			var pos = GetLinePosition(index);
			var endPos = new Point(pos.X + editor.TextView.ActualWidth * 0.6, pos.Y);
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
				}
				else {
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
			if (position == null)
				return -1;
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

			internal InputHandler(InsertionCursorLayer layer) : base(layer.editor)
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
				return (sender, e) =>  {
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
				return (sender, e) =>  {
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
						}
						else {
							newIndex++;
							if (newIndex >= layer.insertionPoints.Length) {
								newIndex = layer.insertionPoints.Length - 1;
								break;
							}
						}
						newVPos = layer.editor.TextView.GetVisualTopByDocumentLine(layer.insertionPoints[newIndex].Location.Line);
					}
					while (Math.Abs(currentVPos - newVPos) < layer.editor.ActualHeight);
					layer.CurrentInsertionPoint = newIndex;
					layer.InvalidateVisual();
					layer.ScrollToInsertionPoint();
				};
			}

			ExecutedRoutedEventHandler MoveMarkerFull(bool up)
			{
				return (sender, e) =>  {
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

		internal void ScrollToInsertionPoint()
		{
			var location = insertionPoints[CurrentInsertionPoint].Location;
			editor.GetService<TextEditor>().ScrollTo(location.Line, location.Column);
			SetGroupBoxPosition();
		}

		void SetGroupBoxPosition()
		{
			var boxPosition = GetLinePosition(CurrentInsertionPoint) + new Vector(editor.TextView.ActualWidth * 0.6 - 5, -groupBox.ActualHeight / 2.0);
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
				Children =  {
					new TextBlock {
						Text = "Use Up/Down to move to another location.\r\n" + "Press Enter to select the location.\r\n" + "Press Esc to cancel this operation."
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
	
	class InsertionCursorEventArgs : EventArgs
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

