// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;

using ICSharpCode.AvalonEdit.AddIn.Options;
using ICSharpCode.AvalonEdit.AddIn.Snippets;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Editor.Commands;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// The text editor use inside the CodeEditor.
	/// There can be two CodeEditorView instances in a single CodeEditor if split-view
	/// is enabled.
	/// </summary>
	public class CodeEditorView : TextEditor
	{
		public ITextEditor Adapter { get; set; }
		
		CodeEditorOptions options;
		BracketHighlightRenderer bracketRenderer;
		
		public CodeEditorView()
		{
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Help, OnHelpExecuted));
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, OnPrint));
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.PrintPreview, OnPrintPreview));
			
			options = ICSharpCode.AvalonEdit.AddIn.Options.CodeEditorOptions.Instance;
			options.BindToTextEditor(this);
			
			bracketRenderer = new BracketHighlightRenderer(this.TextArea.TextView);
			
			this.MouseHover += TextEditorMouseHover;
			this.MouseHoverStopped += TextEditorMouseHoverStopped;
			this.MouseLeave += TextEditorMouseLeave;
			this.TextArea.TextView.MouseDown += TextViewMouseDown;
			this.TextArea.Caret.PositionChanged += TextArea_PositionChanged;
			
			var editingKeyBindings = this.TextArea.DefaultInputHandler.Editing.InputBindings.OfType<KeyBinding>();
			var tabBinding = editingKeyBindings.Single(b => b.Key == Key.Tab && b.Modifiers == ModifierKeys.None);
			tabBinding.Command = new CustomTabCommand(this, tabBinding.Command);
		}
		
		protected override void OnOptionChanged(PropertyChangedEventArgs e)
		{
			base.OnOptionChanged(e);
			if (e.PropertyName == "HighlightBrackets")
				TextArea_PositionChanged(null, e);
			else if (e.PropertyName == "EnableFolding")
				UpdateParseInformation();
		}
		
		void TextArea_PositionChanged(object sender, EventArgs e)
		{
			if (CodeEditorOptions.Instance.HighlightBrackets) {
				/*
				 * Special case: ITextEditor.Language guarantees that it never returns null.
				 * In this case however it can be null, since this code may be called while the document is loaded.
				 * ITextEditor.Language gets set in CodeEditorAdapter.FileNameChanged, which is called after
				 * loading of the document has finished.
				 * */
				if (this.Adapter.Language != null) {
					var bracketSearchResult = this.Adapter.Language.BracketSearcher.SearchBracket(this.Adapter.Document, this.TextArea.Caret.Offset);
					this.bracketRenderer.SetHighlight(bracketSearchResult);
				}
			} else {
				this.bracketRenderer.SetHighlight(null);
			}
		}
		
		sealed class CustomTabCommand : ICommand
		{
			CodeEditorView editor;
			ICommand baseCommand;
			
			public CustomTabCommand(CodeEditorView editor, ICommand baseCommand)
			{
				this.editor = editor;
				this.baseCommand = baseCommand;
			}
			
			public event EventHandler CanExecuteChanged {
				add {}
				remove {}
			}
			
			public bool CanExecute(object parameter)
			{
				return true;
			}
			
			public void Execute(object parameter)
			{
				if (editor.SelectionLength == 0) {
					int wordStart = DocumentUtilitites.FindPrevWordStart(editor.Adapter.Document, editor.CaretOffset);
					if (wordStart > 0) {
						string word = editor.Adapter.Document.GetText(wordStart, editor.CaretOffset - wordStart);
						CodeSnippet snippet = SnippetManager.Instance.FindSnippet(Path.GetExtension(editor.Adapter.FileName),
						                                                          word);
						if (snippet != null) {
							editor.Adapter.Document.Remove(wordStart, editor.CaretOffset - wordStart);
							snippet.CreateAvalonEditSnippet(editor.Adapter).Insert(editor.TextArea);
							return;
						}
					}
				}
				baseCommand.Execute(parameter);
			}
		}
		
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (!e.Handled && e.Key == Key.Escape && e.KeyboardDevice.Modifiers == ModifierKeys.None) {
				if (this.SelectionLength > 0) {
					this.SelectionLength = 0;
					e.Handled = true;
				}
			}
		}
		
		#region Help
		void OnHelpExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			ShowHelp();
		}
		
		public void ShowHelp()
		{
			// Resolve expression at cursor and show help
			TextArea textArea = this.TextArea;
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(this.Adapter.FileName);
			if (expressionFinder == null)
				return;
			string textContent = this.Text;
			ExpressionResult expressionResult = expressionFinder.FindFullExpression(textContent, textArea.Caret.Offset);
			string expression = expressionResult.Expression;
			if (expression != null && expression.Length > 0) {
				ResolveResult result = ParserService.Resolve(expressionResult, textArea.Caret.Line, textArea.Caret.Column, this.Adapter.FileName, textContent);
				TypeResolveResult trr = result as TypeResolveResult;
				if (trr != null) {
					HelpProvider.ShowHelp(trr.ResolvedClass);
				}
				MemberResolveResult mrr = result as MemberResolveResult;
				if (mrr != null) {
					HelpProvider.ShowHelp(mrr.ResolvedMember);
				}
			}
		}
		#endregion
		
		#region Tooltip
		ToolTip toolTip;
		Popup popup;
		
		void TextEditorMouseHover(object sender, MouseEventArgs e)
		{
			Debug.Assert(sender == this);
			ToolTipRequestEventArgs args = new ToolTipRequestEventArgs(this.Adapter);
			var pos = GetPositionFromPoint(e.GetPosition(this));
			args.InDocument = pos.HasValue;
			if (pos.HasValue) {
				args.LogicalPosition = AvalonEditDocumentAdapter.ToLocation(pos.Value);
			}
			
			TextMarkerService textMarkerService = this.Adapter.GetService(typeof(ITextMarkerService)) as TextMarkerService;
			if (args.InDocument && textMarkerService != null) {
				var markersAtOffset = textMarkerService.GetMarkersAtOffset(args.Editor.Document.PositionToOffset(args.LogicalPosition.Line, args.LogicalPosition.Column));
				
				ITextMarker markerWithToolTip = markersAtOffset.FirstOrDefault(marker => marker.ToolTip != null);
				
				if (markerWithToolTip != null) {
					args.ShowToolTip(markerWithToolTip.ToolTip);
				}
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
						toolTip.Closed += ToolTipClosed;
					}
					toolTip.PlacementTarget = this; // required for property inheritance
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
			TextViewPosition? logicalPos = GetPositionFromPoint(mousePos);
			if (logicalPos.HasValue) {
				var textView = this.TextArea.TextView;
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
			popup.Closed += PopupClosed;
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
		}
		
		void TextEditorMouseLeave(object sender, MouseEventArgs e)
		{
			if (popup != null && !popup.IsMouseOver) {
				// do not close popup if mouse moved from editor to popup
				TryCloseExistingPopup(false);
			}
		}

		void ToolTipClosed(object sender, RoutedEventArgs e)
		{
			toolTip = null;
		}
		
		void PopupClosed(object sender, EventArgs e)
		{
			popup = null;
		}
		#endregion
		
		#region Ctrl+Click Go To Definition
		void TextViewMouseDown(object sender, MouseButtonEventArgs e)
		{
			// close existing popup immediately on text editor mouse down
			TryCloseExistingPopup(false);
			if (options.CtrlClickGoToDefinition && e.ChangedButton == MouseButton.Left && Keyboard.Modifiers == ModifierKeys.Control) {
				var position = GetPositionFromPoint(e.GetPosition(this));
				if (position != null) {
					Core.AnalyticsMonitorService.TrackFeature(typeof(GoToDefinition).FullName, "Ctrl+Click");
					GoToDefinition.Run(this.Adapter, this.Document.GetOffset(position.Value));
					e.Handled = true;
				}
			}
		}
		#endregion
		
		public void JumpTo(int line, int column)
		{
			TryCloseExistingPopup(true);
			
			// the adapter sets the caret position and takes care of scrolling
			this.Adapter.JumpTo(line, column);
			this.Focus();
		}
		
		void UpdateParseInformation()
		{
			UpdateParseInformation(ParserService.GetExistingParseInformation(this.Adapter.FileName));
		}
		
		public void UpdateParseInformation(ParseInformation parseInfo)
		{
			if (!CodeEditorOptions.Instance.EnableFolding)
				parseInfo = null;
			
			IServiceContainer container = this.Adapter.GetService(typeof(IServiceContainer)) as IServiceContainer;
			ParserFoldingStrategy folding = container.GetService(typeof(ParserFoldingStrategy)) as ParserFoldingStrategy;
			if (parseInfo == null) {
				if (folding != null) {
					folding.Dispose();
					container.RemoveService(typeof(ParserFoldingStrategy));
				}
			} else {
				if (folding == null) {
					TextArea textArea = this.Adapter.GetService(typeof(TextArea)) as TextArea;
					folding = new ParserFoldingStrategy(textArea);
					container.AddService(typeof(ParserFoldingStrategy), folding);
				}
				folding.UpdateFoldings(parseInfo);
			}
		}
		
		void OnPrint(object sender, ExecutedRoutedEventArgs e)
		{
			PrintDialog printDialog = PrintPreviewViewContent.PrintDialog;
			if (printDialog.ShowDialog() == true) {
				FlowDocument fd = DocumentPrinter.CreateFlowDocumentForEditor(this);
				fd.ColumnGap = 0;
				fd.ColumnWidth = printDialog.PrintableAreaWidth;
				fd.PageHeight = printDialog.PrintableAreaHeight;
				fd.PageWidth = printDialog.PrintableAreaWidth;
				IDocumentPaginatorSource doc = fd;
				printDialog.PrintDocument(doc.DocumentPaginator, Path.GetFileName(this.Adapter.FileName));
			}
		}
		
		void OnPrintPreview(object sender, ExecutedRoutedEventArgs e)
		{
			PrintDialog printDialog = PrintPreviewViewContent.PrintDialog;
			FlowDocument fd = DocumentPrinter.CreateFlowDocumentForEditor(this);
			fd.ColumnGap = 0;
			fd.ColumnWidth = printDialog.PrintableAreaWidth;
			fd.PageHeight = printDialog.PrintableAreaHeight;
			fd.PageWidth = printDialog.PrintableAreaWidth;
			PrintPreviewViewContent.ShowDocument(fd, Path.GetFileName(this.Adapter.FileName));
		}
	}
}
