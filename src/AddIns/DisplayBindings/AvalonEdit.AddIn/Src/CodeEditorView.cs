// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
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
using System.Windows.Media;

using ICSharpCode.AvalonEdit.AddIn.Options;
using ICSharpCode.AvalonEdit.AddIn.Snippets;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Editor.Commands;
using ICSharpCode.SharpDevelop.Refactoring;
using Ast = ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// The text editor use inside the CodeEditor.
	/// There can be two CodeEditorView instances in a single CodeEditor if split-view
	/// is enabled.
	/// </summary>
	public class CodeEditorView : SharpDevelopTextEditor
	{
		public ITextEditor Adapter { get; set; }
		
		BracketHighlightRenderer bracketRenderer;
		CaretReferencesRenderer caretReferencesRenderer;
		ContextActionsRenderer contextActionsRenderer;
		
		public CodeEditorView()
		{
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Help, OnHelpExecuted));
			
			UpdateCustomizedHighlighting();
			
			this.bracketRenderer = new BracketHighlightRenderer(this.TextArea.TextView);
			this.caretReferencesRenderer = new CaretReferencesRenderer(this);
			this.contextActionsRenderer = new ContextActionsRenderer(this);
			
			this.MouseHover += TextEditorMouseHover;
			this.MouseHoverStopped += TextEditorMouseHoverStopped;
			this.MouseLeave += TextEditorMouseLeave;
			this.TextArea.TextView.MouseDown += TextViewMouseDown;
			this.TextArea.Caret.PositionChanged += HighlightBrackets;
			
			SetupTabSnippetHandler();
		}
		
		protected override string FileName {
			get { return this.Adapter.FileName; }
		}
		
		protected override void OnOptionChanged(PropertyChangedEventArgs e)
		{
			base.OnOptionChanged(e);
			if (e.PropertyName == "HighlightBrackets")
				HighlightBrackets(null, e);
			else if (e.PropertyName == "EnableFolding")
				UpdateParseInformationForFolding();
			else if (e.PropertyName == "HighlightSymbol") {
				if (this.caretReferencesRenderer != null)
					this.caretReferencesRenderer.ClearHighlight();
			}
		}
		
		#region CaretPositionChanged - Bracket Highlighting
		/// <summary>
		/// Highlights matching brackets.
		/// </summary>
		void HighlightBrackets(object sender, EventArgs e)
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
		#endregion
		
		#region Custom Tab command (code snippet expansion)
		void SetupTabSnippetHandler()
		{
			var editingKeyBindings = this.TextArea.DefaultInputHandler.Editing.InputBindings.OfType<KeyBinding>();
			var tabBinding = editingKeyBindings.Single(b => b.Key == Key.Tab && b.Modifiers == ModifierKeys.None);
			this.TextArea.DefaultInputHandler.Editing.InputBindings.Remove(tabBinding);
			var newTabBinding = new KeyBinding(new CustomTabCommand(this, tabBinding.Command), tabBinding.Key, tabBinding.Modifiers);
			this.TextArea.DefaultInputHandler.Editing.InputBindings.Add(newTabBinding);
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
							snippet.TrackUsage("CustomTabCommand");
							
							using (editor.Document.RunUpdate()) {
								editor.Adapter.Document.Remove(wordStart, editor.CaretOffset - wordStart);
								snippet.CreateAvalonEditSnippet(editor.Adapter).Insert(editor.TextArea);
							}
							return;
						}
					}
				}
				baseCommand.Execute(parameter);
			}
		}
		#endregion
		
		#region OnKeyDown
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
		#endregion
		
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
					args.SetToolTip(markerWithToolTip.ToolTip);
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
		GoToDefinition goToDefinitionCommand;
		protected GoToDefinition GotoDefinitionCommand {
			get
			{
				if (goToDefinitionCommand == null)
					goToDefinitionCommand = new GoToDefinition();
				return goToDefinitionCommand;
			}
		}
		
		void TextViewMouseDown(object sender, MouseButtonEventArgs e)
		{
			// close existing debugger popup immediately on text editor mouse down
			TryCloseExistingPopup(false);
			
			if (options.CtrlClickGoToDefinition && e.ChangedButton == MouseButton.Left && Keyboard.Modifiers == ModifierKeys.Control) {
				// Ctrl+Click Go to definition
				var position = GetPositionFromPoint(e.GetPosition(this));
				if (position == null)
					return;
				Core.AnalyticsMonitorService.TrackFeature(typeof(GoToDefinition).FullName, "Ctrl+Click");
				this.GotoDefinitionCommand.Run(this.Adapter, this.Document.GetOffset(position.Value));
				e.Handled = true;
			}
		}
		#endregion
		
		#region CTRL+W extend selection
		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			if (e.Handled) return;
			if (e.Key == Key.W && Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) {
				// Select AST Node
				var editorLang = EditorContext.GetEditorLanguage(this.Adapter);
				if (editorLang == null) return;
				var parser = ParserFactory.CreateParser(editorLang.Value, new StringReader(this.Text));
				parser.ParseMethodBodies = true;
				parser.Lexer.SkipAllComments = false; // f
				parser.Parse();
				var parsedCU = parser.CompilationUnit;
				if (parsedCU == null) return;
				//var caretLocation = new Location(this.Adapter.Caret.Column, this.Adapter.Caret.Line);
				var selectionStart = this.Adapter.Document.OffsetToPosition(this.SelectionStart);
				var selectionEnd = this.Adapter.Document.OffsetToPosition(this.SelectionStart + this.SelectionLength);
				foreach (var node in parsedCU.Children) {
					// fix StartLocation / EndLocation
					node.AcceptVisitor(new ICSharpCode.NRefactory.Visitors.SetRegionInclusionVisitor(), null);
				}
				Ast.INode currentNode = parsedCU.Children.Select(
					n => EditorContext.FindInnermostNodeContainingSelection(n, selectionStart, selectionEnd)).Where(n => n != null).FirstOrDefault();
				if (currentNode == null) return;

				// whole node already selected -> expand selection
				if (currentNode.StartLocation == selectionStart && currentNode.EndLocation == selectionEnd) {
					var commentsBlankLines = parser.Lexer.SpecialTracker.CurrentSpecials;
					
					// if there is a comment block immediately before selection, or behind selection on the same line, add it to selection
					var comments = commentsBlankLines.Where(s => s is Comment).Cast<Comment>().ToList();
					int commentIndex = comments.FindIndex(c => c.EndPosition.Line == currentNode.StartLocation.Line);
					if (commentIndex >= 0 && IsWhitespaceBetween(this.Adapter.Document, comments[commentIndex].EndPosition, selectionStart)) {
						while (commentIndex >= 0 && comments[commentIndex].EndPosition.Line == selectionStart.Line)
						{
							var comment = comments[commentIndex];
							selectionStart = comment.StartPosition;
							if (comment.CommentStartsLine)
								selectionStart.Column = 1;
							commentIndex--;
						}
					}
					else {
						// if the extended selection would contain blank lines, extend the selection only to the blank lines/comments on both sides (use siblings)
						//   if the selection contains blank lines or comments on both sides, dont do this
						
						
						var parent = currentNode.Parent;
						if (parent == null)
							return;
						selectionStart = parent.StartLocation;
						selectionEnd = parent.EndLocation;
					}
				} else {
					// select current node
					selectionStart = currentNode.StartLocation;
					selectionEnd = currentNode.EndLocation;
				}
				int startOffset, endOffset;
				try {
					startOffset = this.Adapter.Document.PositionToOffset(selectionStart);
					endOffset = this.Adapter.Document.PositionToOffset(selectionEnd);
				} catch(ArgumentOutOfRangeException) {
					return;
				}
				this.Select(startOffset, endOffset - startOffset);
			}
		}
		
		bool IsWhitespaceBetween(IDocument document, Location startPos, Location endPos)
		{
			int startOffset = document.PositionToOffset(startPos);
			int endOffset = document.PositionToOffset(endPos);
			return string.IsNullOrWhiteSpace(document.GetText(startOffset, endOffset - startOffset));
		}
		#endregion
		
		public void JumpTo(int line, int column)
		{
			// closes Debugger popup on debugger step
			TryCloseExistingPopup(true);
			
			// the adapter sets the caret position and takes care of scrolling
			this.Adapter.JumpTo(line, column);
			this.Focus();
		}
		
		#region UpdateParseInformation - Folding
		void UpdateParseInformationForFolding()
		{
			UpdateParseInformationForFolding(ParserService.GetExistingParseInformation(this.Adapter.FileName));
		}
		
		bool disableParseInformationFolding;
		
		public bool DisableParseInformationFolding {
			get { return disableParseInformationFolding; }
			set {
				if (disableParseInformationFolding != value) {
					disableParseInformationFolding = value;
					UpdateParseInformationForFolding();
				}
			}
		}
		
		public void UpdateParseInformationForFolding(ParseInformation parseInfo)
		{
			if (!CodeEditorOptions.Instance.EnableFolding || disableParseInformationFolding)
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
		#endregion
		
		protected override IVisualLineTransformer CreateColorizer(IHighlightingDefinition highlightingDefinition)
		{
			return new CustomizableHighlightingColorizer(
				highlightingDefinition.MainRuleSet,
				FetchCustomizations(highlightingDefinition.Name));
		}
		
		// TODO: move this into SharpDevelopTextEditor
		public void UpdateCustomizedHighlighting()
		{
			string language = this.SyntaxHighlighting != null ? this.SyntaxHighlighting.Name : null;
			CustomizableHighlightingColorizer.ApplyCustomizationsToDefaultElements(this, FetchCustomizations(language));
			this.TextArea.TextView.Redraw(); // manually redraw if default elements didn't change but customized highlightings did
		}
		
		static IEnumerable<CustomizedHighlightingColor> FetchCustomizations(string languageName)
		{
			// Access CustomizedHighlightingColor.ActiveColors within enumerator so that always the latest version is used.
			// Using CustomizedHighlightingColor.ActiveColors.Where(...) would not work correctly!
			foreach (CustomizedHighlightingColor color in CustomizedHighlightingColor.ActiveColors) {
				if (color.Language == null || color.Language == languageName)
					yield return color;
			}
		}
	}
}
