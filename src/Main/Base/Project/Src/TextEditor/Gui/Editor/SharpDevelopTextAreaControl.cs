// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Actions;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Gui.InsightWindow;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

using System.Threading;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class SharpDevelopTextAreaControl : TextEditorControl
	{
		readonly static string contextMenuPath       = "/SharpDevelop/ViewContent/DefaultTextEditor/ContextMenu";
		readonly static string editActionsPath       = "/AddIns/DefaultTextEditor/EditActions";
		readonly static string formatingStrategyPath = "/AddIns/DefaultTextEditor/Formatter";
		
		QuickClassBrowserPanel quickClassBrowserPanel = null;
		ErrorDrawer errorDrawer;
		
		public QuickClassBrowserPanel QuickClassBrowserPanel {
			get {
				return quickClassBrowserPanel;
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
			
			TextAreaDragDropHandler dragDropHandler = new TextAreaDragDropHandler();
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
		
		public virtual ICompletionDataProvider CreateCodeCompletionDataProvider(bool ctrlSpace)
		{
			//ivoko: please do not touch or discuss with me: we use another CCDP
			return new CodeCompletionDataProvider(ctrlSpace, false);
		}
		
		protected override void InitializeTextAreaControl(TextAreaControl newControl)
		{
			base.InitializeTextAreaControl(newControl);
			
			newControl.ContextMenuStrip = MenuService.CreateContextMenu(this, contextMenuPath);
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
				errorDrawer.Dispose();
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
			try {
				IEditAction[] actions = (IEditAction[])(AddInTree.GetTreeNode(editActionsPath).BuildChildItems(this)).ToArray(typeof(IEditAction));
				
				foreach (IEditAction action in actions) {
					foreach (Keys key in action.Keys) {
						editactions[key] = action;
					}
				}
			} catch (TreePathNotFoundException) {
				Console.WriteLine(editActionsPath + " doesn't exists in the AddInTree");
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
		}
		void ShowQuickClassBrowserPanel()
		{
			if (quickClassBrowserPanel == null) {
				quickClassBrowserPanel = new QuickClassBrowserPanel(this);
				Controls.Add(quickClassBrowserPanel);
				textAreaPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
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
		
		bool HandleKeyPress(char ch)
		{
			string fileName = FileName;
			if (codeCompletionWindow != null && !codeCompletionWindow.IsDisposed) {
				codeCompletionWindow.ProcessKeyEvent(ch);
			}
			
			try {
				foreach (ICodeCompletionBinding ccBinding in CodeCompletionBindings) {
					if (ccBinding.HandleKeyPress(this, ch))
						return false;
				}
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
			} catch (Exception ex) {
				LogException(ex);
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
			int newCaretOffset   = ActiveTextAreaControl.TextArea.Caret.Offset;
			int finalCaretOffset = newCaretOffset;
			int firstLine        = Document.GetLineNumberForOffset(newCaretOffset);
			
			// save old properties, these properties cause strange effects, when not
			// be turned off (like insert curly braces or other formatting stuff)
			bool save1         = TextEditorProperties.AutoInsertCurlyBracket;
			IndentStyle save2  = TextEditorProperties.IndentStyle;
			TextEditorProperties.AutoInsertCurlyBracket = false;
			TextEditorProperties.IndentStyle            = IndentStyle.Auto;
			
			
			string templateText = StringParser.Parse(template.Text, new string[,] { { "Selection", selectedText } });
			
			BeginUpdate();
			for (int i =0; i < templateText.Length; ++i) {
				switch (templateText[i]) {
					case '|':
						finalCaretOffset = newCaretOffset;
						break;
					case '\r':
						break;
					case '\t':
//						new Tab().Execute(ActiveTextAreaControl.TextArea);
						break;
					case '\n':
						ActiveTextAreaControl.TextArea.Caret.Position = Document.OffsetToPosition(newCaretOffset);
						new Return().Execute(ActiveTextAreaControl.TextArea);
						newCaretOffset = ActiveTextAreaControl.TextArea.Caret.Offset;
						break;
					default:
						ActiveTextAreaControl.TextArea.InsertChar(templateText[i]);
						newCaretOffset = ActiveTextAreaControl.TextArea.Caret.Offset;
						break;
				}
			}
			int lastLine = Document.GetLineNumberForOffset(newCaretOffset);
			EndUpdate();
			Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.LinesBetween, firstLine, lastLine));
			Document.CommitUpdate();
			ActiveTextAreaControl.TextArea.Caret.Position = Document.OffsetToPosition(finalCaretOffset);
			TextEditorProperties.IndentStyle = IndentStyle.Smart;
			Document.FormattingStrategy.IndentLines(ActiveTextAreaControl.TextArea, firstLine, lastLine);
			
			// restore old property settings
			TextEditorProperties.AutoInsertCurlyBracket = save1;
			TextEditorProperties.IndentStyle            = save2;
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
