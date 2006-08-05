// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Actions;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.TextEditor.Gui.InsightWindow;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class SharpDevelopTextAreaControl : TextEditorControl
	{
		readonly static string contextMenuPath       = "/SharpDevelop/ViewContent/DefaultTextEditor/ContextMenu";
		readonly static string editActionsPath       = "/AddIns/DefaultTextEditor/EditActions";
		readonly static string formatingStrategyPath = "/AddIns/DefaultTextEditor/Formatter";
		readonly static string advancedHighlighterPath = "/AddIns/DefaultTextEditor/AdvancedHighlighter";
		
		QuickClassBrowserPanel quickClassBrowserPanel = null;
		Control customQuickClassBrowserPanel = null;
		ErrorDrawer errorDrawer;
		IAdvancedHighlighter advancedHighlighter;
		
		public QuickClassBrowserPanel QuickClassBrowserPanel {
			get {
				return quickClassBrowserPanel;
			}
		}
		public Control CustomQuickClassBrowserPanel {
			get	{
				return customQuickClassBrowserPanel;
			}
			set	{
				if (customQuickClassBrowserPanel != null) {
					RemoveQuickClassBrowserPanel();
					customQuickClassBrowserPanel.Dispose();
				}
				customQuickClassBrowserPanel = value;
				ActivateQuickClassBrowserOnDemand();
			}
		}
		
		public SharpDevelopTextAreaControl()
		{
			errorDrawer = new ErrorDrawer(this);
			Document.FoldingManager.FoldingStrategy = new ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.ParserFoldingStrategy();
			Document.BookmarkManager.Factory = new Bookmarks.SDBookmarkFactory(Document.BookmarkManager);
			Document.BookmarkManager.Added   += new BookmarkEventHandler(BookmarkAdded);
			Document.BookmarkManager.Removed += new BookmarkEventHandler(BookmarkRemoved);
			GenerateEditActions();
			
			TextEditorProperties = new SharpDevelopTextEditorProperties();
		}
		
		void BookmarkAdded(object sender, BookmarkEventArgs e)
		{
			Bookmarks.SDBookmark b = e.Bookmark as Bookmarks.SDBookmark;
			if (b != null) {
				Bookmarks.BookmarkManager.AddMark(b);
			}
		}
		
		void BookmarkRemoved(object sender, BookmarkEventArgs e)
		{
			Bookmarks.SDBookmark b = e.Bookmark as Bookmarks.SDBookmark;
			if (b != null) {
				Bookmarks.BookmarkManager.RemoveMark(b);
			}
		}
		
		protected override void InitializeTextAreaControl(TextAreaControl newControl)
		{
			base.InitializeTextAreaControl(newControl);
			
			newControl.ShowContextMenu += delegate(object sender, MouseEventArgs e) {
				MenuService.ShowContextMenu(this, contextMenuPath, (Control)sender, e.X, e.Y);
			};
			newControl.TextArea.KeyEventHandler += new ICSharpCode.TextEditor.KeyEventHandler(HandleKeyPress);
			newControl.Caret.PositionChanged += new EventHandler(CaretPositionChanged);
			newControl.TextArea.ClipboardHandler.CopyText += new CopyTextEventHandler(ClipboardHandlerCopyText);
			
//			newControl.TextArea.IconBarMargin.Painted   += new MarginPaintEventHandler(PaintIconBarBreakPoints);
//			newControl.TextArea.IconBarMargin.MouseDown += new MarginMouseEventHandler(IconBarMouseDown);
			
			newControl.MouseWheel                       += new MouseEventHandler(TextAreaMouseWheel);
			newControl.DoHandleMousewheel = false;
		}
		
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing) {
				if (errorDrawer != null) {
					errorDrawer.Dispose();
					errorDrawer = null;
				}
				if (quickClassBrowserPanel != null) {
					quickClassBrowserPanel.Dispose();
					quickClassBrowserPanel = null;
				}
				if (customQuickClassBrowserPanel != null) {
					customQuickClassBrowserPanel.Dispose();
					customQuickClassBrowserPanel = null;
				}
				if (advancedHighlighter != null) {
					advancedHighlighter.Dispose();
					advancedHighlighter = null;
				}
				CloseCodeCompletionWindow(this, EventArgs.Empty);
				CloseInsightWindow(this, EventArgs.Empty);
			}
		}
		
		void CloseCodeCompletionWindow(object sender, EventArgs e)
		{
			if (codeCompletionWindow != null) {
				codeCompletionWindow.Closed -= new EventHandler(CloseCodeCompletionWindow);
				codeCompletionWindow.Dispose();
				codeCompletionWindow = null;
			}
		}
		
		void CloseInsightWindow(object sender, EventArgs e)
		{
			if (insightWindow != null) {
				insightWindow.Closed -= new EventHandler(CloseInsightWindow);
				insightWindow.Dispose();
				insightWindow = null;
			}
		}
		
		void TextAreaMouseWheel(object sender, MouseEventArgs e)
		{
			TextAreaControl textAreaControl = (TextAreaControl)sender;
			if (insightWindow != null && !insightWindow.IsDisposed && insightWindow.Visible) {
				insightWindow.HandleMouseWheel(e);
			} else if (codeCompletionWindow != null && !codeCompletionWindow.IsDisposed && codeCompletionWindow.Visible) {
				codeCompletionWindow.HandleMouseWheel(e);
			} else {
				textAreaControl.HandleMouseWheel(e);
			}
		}
		
		void ClipboardHandlerCopyText(object sender, CopyTextEventArgs e)
		{
			ICSharpCode.SharpDevelop.Gui.SideBarView.PutInClipboardRing(e.Text);
		}
		public override void OptionsChanged()
		{
			base.OptionsChanged();
			SharpDevelopTextEditorProperties sdtep = base.TextEditorProperties as SharpDevelopTextEditorProperties;
			
			if (sdtep != null) {
				if (!sdtep.ShowQuickClassBrowserPanel) {
					RemoveQuickClassBrowserPanel();
				} else {
					ActivateQuickClassBrowserOnDemand();
				}
			}
		}
		
		void CaretPositionChanged(object sender, EventArgs e)
		{
			StatusBarService.SetCaretPosition(ActiveTextAreaControl.TextArea.TextView.GetVisualColumn(ActiveTextAreaControl.Caret.Line, ActiveTextAreaControl.Caret.Column), ActiveTextAreaControl.Caret.Line, ActiveTextAreaControl.Caret.Column);
		}
		
		void GenerateEditActions()
		{
			#if DEBUG
			editactions[Keys.Control | Keys.OemPeriod] = new DebugCodeCompletionAction();
			#endif
			try {
				IEditAction[] actions = (IEditAction[])(AddInTree.GetTreeNode(editActionsPath).BuildChildItems(this)).ToArray(typeof(IEditAction));
				
				foreach (IEditAction action in actions) {
					foreach (Keys key in action.Keys) {
						editactions[key] = action;
					}
				}
			} catch (TreePathNotFoundException) {
				LoggingService.Warn("EditAction " + editActionsPath + " doesn't exists in the AddInTree");
			}
		}
		
		void RemoveQuickClassBrowserPanel()
		{
			if (quickClassBrowserPanel != null) {
				Controls.Remove(quickClassBrowserPanel);
				quickClassBrowserPanel.Dispose();
				quickClassBrowserPanel = null;
				textAreaPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
			}
			if (customQuickClassBrowserPanel != null) {
				if (Controls.Contains(customQuickClassBrowserPanel)) {
					Controls.Remove(customQuickClassBrowserPanel);
					customQuickClassBrowserPanel.Enabled = false;
					textAreaPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
				}
			}
		}
		void ShowQuickClassBrowserPanel()
		{
			if (quickClassBrowserPanel == null) {
				quickClassBrowserPanel = new QuickClassBrowserPanel(this);
				Controls.Add(quickClassBrowserPanel);
				textAreaPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			}
			if (customQuickClassBrowserPanel != null) {
				if (quickClassBrowserPanel != null)
					RemoveQuickClassBrowserPanel();
				if (!Controls.Contains(customQuickClassBrowserPanel)) {
					Controls.Add(customQuickClassBrowserPanel);
					customQuickClassBrowserPanel.Enabled = true;
					textAreaPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
				}
				return;
			}
		}
		public void ActivateQuickClassBrowserOnDemand()
		{
			SharpDevelopTextEditorProperties sdtep = base.TextEditorProperties as SharpDevelopTextEditorProperties;
			if (sdtep != null && sdtep.ShowQuickClassBrowserPanel && FileName != null) {
				
				bool quickClassPanelActive = ParserService.GetParser(FileName) != null;
				if (quickClassPanelActive) {
					ShowQuickClassBrowserPanel();
				} else {
					RemoveQuickClassBrowserPanel();
				}
			}
		}
		
		protected override void OnFileNameChanged(EventArgs e)
		{
			base.OnFileNameChanged(e);
			((Bookmarks.SDBookmarkFactory)Document.BookmarkManager.Factory).ChangeFilename(this.FileName);
			ActivateQuickClassBrowserOnDemand();
		}
		
		//// Alex: routine for pulsing parser thread
//		protected void PulseParser() {
//			lock(DefaultParserService.ParserPulse) {
//				Monitor.Pulse(DefaultParserService.ParserPulse);
//			}
//		}
		//// ALex: end of mod
		
		static ICodeCompletionBinding[] codeCompletionBindings;
		
		public static ICodeCompletionBinding[] CodeCompletionBindings {
			get {
				if (codeCompletionBindings == null) {
					try {
						codeCompletionBindings = (ICodeCompletionBinding[])(AddInTree.GetTreeNode("/AddIns/DefaultTextEditor/CodeCompletion").BuildChildItems(null)).ToArray(typeof(ICodeCompletionBinding));
					} catch (TreePathNotFoundException) {
						codeCompletionBindings = new ICodeCompletionBinding[] {};
					}
				}
				return codeCompletionBindings;
			}
		}
		
		InsightWindow insightWindow = null;
		CodeCompletionWindow codeCompletionWindow = null;
		bool inHandleKeyPress;
		
		bool HandleKeyPress(char ch)
		{
			if (inHandleKeyPress)
				return false;
			inHandleKeyPress = true;
			try {
				if (codeCompletionWindow != null && !codeCompletionWindow.IsDisposed) {
					if (codeCompletionWindow.ProcessKeyEvent(ch)) {
						return true;
					}
					if (codeCompletionWindow != null && !codeCompletionWindow.IsDisposed) {
						// code-completion window is still opened but did not want to handle
						// the keypress -> don't try to restart code-completion
						return false;
					}
				}
				
				if (CodeCompletionOptions.EnableCodeCompletion) {
					foreach (ICodeCompletionBinding ccBinding in CodeCompletionBindings) {
						if (ccBinding.HandleKeyPress(this, ch))
							return false;
					}
				}
				if (ch == ' ') {
					string word = GetWordBeforeCaret();
					if (word != null) {
						CodeTemplateGroup templateGroup = CodeTemplateLoader.GetTemplateGroupPerFilename(FileName);
						if (templateGroup != null) {
							foreach (CodeTemplate template in templateGroup.Templates) {
								if (template.Shortcut == word) {
									if (word.Length > 0) {
										int newCaretOffset = DeleteWordBeforeCaret();
										//// set new position in text area
										ActiveTextAreaControl.TextArea.Caret.Position = Document.OffsetToPosition(newCaretOffset);
									}
									
									InsertTemplate(template);
									return true;
								}
							}
						}
					}
				}
			} catch (Exception ex) {
				LogException(ex);
			} finally {
				inHandleKeyPress = false;
			}
			return false;
		}
		
		public void ShowInsightWindow(IInsightDataProvider insightDataProvider)
		{
			if (insightWindow == null || insightWindow.IsDisposed) {
				insightWindow = new InsightWindow(((Form)WorkbenchSingleton.Workbench), this, FileName);
				insightWindow.Closed += new EventHandler(CloseInsightWindow);
			}
			insightWindow.AddInsightDataProvider(insightDataProvider);
			insightWindow.ShowInsightWindow();
		}
		
		public bool InsightWindowVisible {
			get {
				return insightWindow != null;
			}
		}
		
		public void ShowCompletionWindow(ICompletionDataProvider completionDataProvider, char ch)
		{
			codeCompletionWindow = CodeCompletionWindow.ShowCompletionWindow((Form)WorkbenchSingleton.Workbench, this, this.FileName, completionDataProvider, ch);
			if (codeCompletionWindow != null) {
				codeCompletionWindow.Closed += new EventHandler(CloseCodeCompletionWindow);
			}
		}
		
		private void LogException(Exception ex)
		{
			ICSharpCode.Core.MessageService.ShowError(ex);
		}
		
		public string GetWordBeforeCaret()
		{
			int start = TextUtilities.FindPrevWordStart(Document, ActiveTextAreaControl.TextArea.Caret.Offset);
			return Document.GetText(start, ActiveTextAreaControl.TextArea.Caret.Offset - start);
		}
		
		public int DeleteWordBeforeCaret()
		{
			int start = TextUtilities.FindPrevWordStart(Document, ActiveTextAreaControl.TextArea.Caret.Offset);
			Document.Remove(start, ActiveTextAreaControl.TextArea.Caret.Offset - start);
			return start;
		}
		
		/// <remarks>
		/// This method inserts a code template at the current caret position
		/// </remarks>
		public void InsertTemplate(CodeTemplate template)
		{
			string selectedText = String.Empty;
			if (base.ActiveTextAreaControl.TextArea.SelectionManager.HasSomethingSelected) {
				selectedText = base.ActiveTextAreaControl.TextArea.SelectionManager.SelectedText;
				ActiveTextAreaControl.TextArea.Caret.Position = ActiveTextAreaControl.TextArea.SelectionManager.SelectionCollection[0].StartPosition;
				base.ActiveTextAreaControl.TextArea.SelectionManager.RemoveSelectedText();
			}
			
			// save old properties, these properties cause strange effects, when not
			// be turned off (like insert curly braces or other formatting stuff)
			
			string templateText = StringParser.Parse(template.Text, new string[,] { { "Selection", selectedText } });
			int finalCaretOffset = templateText.IndexOf('|');
			if (finalCaretOffset >= 0) {
				templateText = templateText.Remove(finalCaretOffset, 1);
			} else {
				finalCaretOffset = templateText.Length;
			}
			int caretOffset = ActiveTextAreaControl.TextArea.Caret.Offset;
			
			BeginUpdate();
			int beginLine = ActiveTextAreaControl.TextArea.Caret.Line;
			Document.Insert(caretOffset, templateText);
			
			ActiveTextAreaControl.TextArea.Caret.Position = Document.OffsetToPosition(caretOffset + finalCaretOffset);
			int endLine = Document.OffsetToPosition(caretOffset + templateText.Length).Y;
			
			IndentStyle save1 = TextEditorProperties.IndentStyle;
			TextEditorProperties.IndentStyle = IndentStyle.Smart;
			Console.WriteLine("Indent between {0} and {1}", beginLine, endLine);
			Document.FormattingStrategy.IndentLines(ActiveTextAreaControl.TextArea, beginLine, endLine);
			EndUpdate();
			Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
			Document.CommitUpdate();
			
			// restore old property settings
			TextEditorProperties.IndentStyle = save1;
		}
		
		protected override void OnReloadHighlighting(object sender, EventArgs e)
		{
			base.OnReloadHighlighting(sender, e);
			InitializeAdvancedHighlighter();
		}
		
		public void InitializeAdvancedHighlighter()
		{
			if (advancedHighlighter != null) {
				advancedHighlighter.Dispose();
				advancedHighlighter = null;
			}
			string highlighterPath = advancedHighlighterPath + "/" + Document.HighlightingStrategy.Name;
			if (AddInTree.ExistsTreeNode(highlighterPath)) {
				IList<IAdvancedHighlighter> highlighter = AddInTree.BuildItems<IAdvancedHighlighter>(highlighterPath, this);
				if (highlighter != null && highlighter.Count > 0) {
					advancedHighlighter = highlighter[0];
					advancedHighlighter.Initialize(this);
					Document.HighlightingStrategy = new AdvancedHighlightingStrategy((DefaultHighlightingStrategy)Document.HighlightingStrategy, advancedHighlighter);
				}
			}
		}
		
		public void InitializeFormatter()
		{
			string formatterPath = formatingStrategyPath + "/" + Document.HighlightingStrategy.Name;
			if (AddInTree.ExistsTreeNode(formatterPath)) {
				IFormattingStrategy[] formatter = (IFormattingStrategy[])(AddInTree.GetTreeNode(formatterPath).BuildChildItems(this)).ToArray(typeof(IFormattingStrategy));
				if (formatter != null && formatter.Length > 0) {
					Document.FormattingStrategy = formatter[0];
				}
			}
		}
		
		public override string GetRangeDescription(int selectedItem, int itemCount)
		{
			
			StringParser.Properties["CurrentMethodNumber"]  = selectedItem.ToString("##");
			StringParser.Properties["NumberOfTotalMethods"] = itemCount.ToString("##");
			return StringParser.Parse("${res:ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.InsightWindow.NumberOfText}");
		}
		
//		public override IDeclarationViewWindow CreateDeclarationViewWindow()
//		{
//			return new HtmlDeclarationViewWindow();
//		}
		//
	}
}
