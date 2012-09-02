// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.ILSpyAddIn.ViewContent
{
	class DecompiledTextEditorAdapter : AvalonEditTextEditorAdapter
	{
		public DecompiledTextEditorAdapter(TextEditor textEditor) : base(textEditor)
		{}
		
		public string DecompiledFileName { get; set; }
		
		public override ICSharpCode.Core.FileName FileName {
			get { return ICSharpCode.Core.FileName.Create(DecompiledFileName); }
		}
	}
	
	/// <summary>
	/// Equivalent to AE.AddIn CodeEditor, but without editing capabilities.
	/// </summary>
	class CodeView : Grid, IDisposable, IPositionable
	{
		public event EventHandler DocumentChanged;
		
		readonly DecompiledTextEditorAdapter adapter;
		readonly IconBarManager iconBarManager;
		readonly IconBarMargin iconMargin;
		readonly TextMarkerService textMarkerService;
		
		public CodeView(string decompiledFileName)
		{
			this.adapter = new DecompiledTextEditorAdapter(new SharpDevelopTextEditor { IsReadOnly = true }) {
				DecompiledFileName = decompiledFileName
			};
			this.Children.Add(adapter.TextEditor);
			adapter.TextEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
			
			// add margin
			this.iconMargin = new IconBarMargin(iconBarManager = new IconBarManager());
			this.adapter.TextEditor.TextArea.LeftMargins.Insert(0, iconMargin);
			this.adapter.TextEditor.TextArea.TextView.VisualLinesChanged += delegate { iconMargin.InvalidateVisual(); };
			
			// add marker service
			this.textMarkerService = new TextMarkerService(adapter.TextEditor.Document);
			this.adapter.TextEditor.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);
			this.adapter.TextEditor.TextArea.TextView.LineTransformers.Add(textMarkerService);
			this.adapter.TextEditor.TextArea.TextView.Services.AddService(typeof(ITextMarkerService), textMarkerService);
			this.adapter.TextEditor.TextArea.TextView.Services.AddService(typeof(IBookmarkMargin), iconBarManager);
			// DON'T add the editor in textview ervices - will mess the setting of breakpoints
			
			// add events
			this.adapter.TextEditor.MouseHover += TextEditorMouseHover;
			this.adapter.TextEditor.MouseHoverStopped += TextEditorMouseHoverStopped;
			this.adapter.TextEditor.MouseLeave += TextEditorMouseLeave;
			
			this.adapter.TextEditor.TextArea.DefaultInputHandler.NestedInputHandlers.Add(new SearchInputHandler(this.adapter.TextEditor.TextArea));
		}

		#region Popup
		ToolTip toolTip;
		Popup popup;
		
		void TextEditorMouseHover(object sender, MouseEventArgs e)
		{
			ToolTipRequestEventArgs args = new ToolTipRequestEventArgs(this.adapter);
			var pos = adapter.TextEditor.GetPositionFromPoint(e.GetPosition(this));
			args.InDocument = pos.HasValue;
			if (pos.HasValue) {
				args.LogicalPosition = AvalonEditDocumentAdapter.ToLocation(pos.Value.Location);
			}
			
			if (!args.Handled) {
				// if request wasn't handled by a marker, pass it to the ToolTipRequestService
				ToolTipRequestService.RequestToolTip(args);
			}
			
			if (args.ContentToShow != null) {
				var contentToShowITooltip = args.ContentToShow as ITooltip;
				
				if (contentToShowITooltip != null && contentToShowITooltip.ShowAsPopup) {
					if (!(args.ContentToShow is UIElement)) {
						throw new NotSupportedException("Content to show in Popup must be UIElement: " + args.ContentToShow);
					}
					if (popup == null) {
						popup = CreatePopup();
					}
					if (TryCloseExistingPopup(false)) {
						// when popup content decides to close, close the popup
						contentToShowITooltip.Closed += (closedSender, closedArgs) => { popup.IsOpen = false; };
						popup.Child = (UIElement)args.ContentToShow;
						//ICSharpCode.SharpDevelop.Debugging.DebuggerService.CurrentDebugger.IsProcessRunningChanged
						SetPopupPosition(popup, e);
						popup.IsOpen = true;
					}
					e.Handled = true;
				} else {
					if (toolTip == null) {
						toolTip = new ToolTip();
						toolTip.Closed += delegate { toolTip = null; };
					}
					toolTip.PlacementTarget = this.adapter.TextEditor; // required for property inheritance
					
					if(args.ContentToShow is string) {
						toolTip.Content = new TextBlock
						{
							Text = args.ContentToShow as string,
							TextWrapping = TextWrapping.Wrap
						};
					}
					else
						toolTip.Content = args.ContentToShow;
					
					toolTip.IsOpen = true;
					e.Handled = true;
				}
			} else {
				// close popup if mouse hovered over empty area
				if (popup != null) {
					e.Handled = true;
				}
				TryCloseExistingPopup(false);
			}
		}
		
		bool TryCloseExistingPopup(bool mouseClick)
		{
			bool canClose = true;
			if (popup != null) {
				var popupContentITooltip = popup.Child as ITooltip;
				if (popupContentITooltip != null) {
					canClose = popupContentITooltip.Close(mouseClick);
				}
				if (canClose) {
					popup.IsOpen = false;
				}
			}
			return canClose;
		}
		
		void SetPopupPosition(Popup popup, MouseEventArgs mouseArgs)
		{
			var popupPosition = GetPopupPosition(mouseArgs);
			popup.HorizontalOffset = popupPosition.X;
			popup.VerticalOffset = popupPosition.Y;
		}
		
		/// <summary> Returns Popup position based on mouse position, in device independent units </summary>
		Point GetPopupPosition(MouseEventArgs mouseArgs)
		{
			Point mousePos = mouseArgs.GetPosition(this);
			Point positionInPixels;
			// align Popup with line bottom
			TextViewPosition? logicalPos = adapter.TextEditor.GetPositionFromPoint(mousePos);
			if (logicalPos.HasValue) {
				var textView = adapter.TextEditor.TextArea.TextView;
				positionInPixels =
					textView.PointToScreen(
						textView.GetVisualPosition(logicalPos.Value, VisualYPosition.LineBottom) - textView.ScrollOffset);
				positionInPixels.X -= 4;
			} else {
				positionInPixels = PointToScreen(mousePos + new Vector(-4, 6));
			}
			// use device independent units, because Popup Left/Top are in independent units
			return positionInPixels.TransformFromDevice(this);
		}
		
		Popup CreatePopup()
		{
			popup = new Popup();
			popup.Closed += (s, e) => popup = null;
			popup.AllowsTransparency = true;
			popup.PlacementTarget = this; // required for property inheritance
			popup.Placement = PlacementMode.Absolute;
			popup.StaysOpen = true;
			return popup;
		}
		
		void TextEditorMouseHoverStopped(object sender, MouseEventArgs e)
		{
			if (toolTip != null) {
				toolTip.IsOpen = false;
				e.Handled = true;
			}
			
			TextEditorMouseLeave(sender, e);
		}
		
		void TextEditorMouseLeave(object sender, MouseEventArgs e)
		{
			if (popup != null && !popup.IsMouseOver) {
				// do not close popup if mouse moved from editor to popup
				TryCloseExistingPopup(false);
			}
		}
		
		#endregion
		
		public TextDocument Document {
			get { return adapter.TextEditor.Document; }
			set {
				adapter.TextEditor.Document = value;
				if (DocumentChanged != null) {
					DocumentChanged(value, EventArgs.Empty);
				}
			}
		}
		
		public ITextEditor TextEditor {
			get {
				return adapter;
			}
		}
		
		public IconBarManager IconBarManager {
			get { return iconBarManager; }
		}
		
		public void Dispose()
		{
		}
		
		public void UnfoldAndScroll(int lineNumber)
		{
			if (lineNumber <= 0 || lineNumber > adapter.Document.TotalNumberOfLines)
				return;
			
//			var line = adapter.TextEditor.Document.GetLineByNumber(lineNumber);
			
//			// unfold
//			var foldings = foldingManager.GetFoldingsContaining(line.Offset);
//			if (foldings != null) {
//				foreach (var folding in foldings) {
//					if (folding.IsFolded) {
//						folding.IsFolded = false;
//					}
//				}
//			}
			
			// scroll to
			adapter.TextEditor.ScrollTo(lineNumber, 0);
		}
		
		public void Redraw(ISegment segment, System.Windows.Threading.DispatcherPriority priority)
		{
			this.adapter.TextEditor.TextArea.TextView.Redraw(segment, priority);
		}
		
		public int Line {
			get {
				return this.adapter.Caret.Line;
			}
		}
		
		public int Column {
			get {
				return this.adapter.Caret.Column;
			}
		}
		
		public void JumpTo(int line, int column)
		{
			this.adapter.JumpTo(line, column);
		}
	}
}
