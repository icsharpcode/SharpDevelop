// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;

using ICSharpCode.SharpDevelop.Dom;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Internal.Undo;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class TextEditorDisplayBinding : IDisplayBinding
	{
		// load #D-specific syntax highlighting files here
		// don't know if this could be solved better by new codons,
		// but this will do
		static TextEditorDisplayBinding()
		{
			
			
			string modeDir = Path.Combine(PropertyService.ConfigDirectory, "modes");
			if (!Directory.Exists(modeDir)) {
				Directory.CreateDirectory(modeDir);
			}
			
			HighlightingManager.Manager.AddSyntaxModeFileProvider(new ICSharpCode.SharpDevelop.DefaultEditor.Codons.AddInTreeSyntaxModeProvider());
			HighlightingManager.Manager.AddSyntaxModeFileProvider(new FileSyntaxModeProvider(modeDir));
			HighlightingManager.Manager.AddSyntaxModeFileProvider(new FileSyntaxModeProvider(Path.Combine(PropertyService.DataDirectory, "modes")));
		}
		
		public virtual bool CanCreateContentForFile(string fileName)
		{
			return true;
		}
		
		public virtual bool CanCreateContentForLanguage(string language)
		{
			return true;
		}
		
		public virtual IViewContent CreateContentForFile(string fileName)
		{
			TextEditorDisplayBindingWrapper b2 = new TextEditorDisplayBindingWrapper();
			
			b2.textAreaControl.Dock = DockStyle.Fill;
			b2.Load(fileName);
			b2.textAreaControl.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategyForFile(fileName);
			b2.textAreaControl.InitializeFormatter();
			b2.ForceFoldingUpdate();
			b2.textAreaControl.ActivateQuickClassBrowserOnDemand();
			return b2;
		}
		
		public virtual IViewContent CreateContentForLanguage(string language, string content)
		{
			TextEditorDisplayBindingWrapper b2 = new TextEditorDisplayBindingWrapper();
			
			b2.textAreaControl.Document.TextContent = StringParser.Parse(content);
			b2.textAreaControl.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy(language);
			b2.textAreaControl.InitializeFormatter();
			b2.textAreaControl.ActivateQuickClassBrowserOnDemand();
			return b2;
		}
	}
	
	public class TextEditorDisplayBindingWrapper : AbstractViewContent, IMementoCapable, IPrintable, IEditable, IUndoHandler, IPositionable, ITextEditorControlProvider, IParseInformationListener, IClipboardHandler, IContextHelpProvider
	{
		public SharpDevelopTextAreaControl textAreaControl = null;
		
		public TextEditorControl TextEditorControl {
			get {
				return textAreaControl;
			}
		}
		
		// KSL Start, New lines
		protected FileSystemWatcher watcher;
		protected bool wasChangedExternally = false;
		// KSL End
		
		public bool EnableUndo {
			get {
				return textAreaControl.EnableUndo;
			}
		}
		public bool EnableRedo {
			get {
				return textAreaControl.EnableRedo;
			}
		}
		
		// ParserUpdateThread uses the text property via IEditable, I had an exception
		// because multiple threads were accessing the GapBufferStrategy at the same time.
		
		string GetText()
		{
			return textAreaControl.Document.TextContent;
		}
		
		void SetText(string value)
		{
			textAreaControl.Document.TextContent = value;
		}
		
		public string Text {
			get {
				if (WorkbenchSingleton.InvokeRequired)
					return (string)WorkbenchSingleton.SafeThreadCall(this, "GetText", null);
				else
					return GetText();
			}
			set {
				if (WorkbenchSingleton.InvokeRequired)
					WorkbenchSingleton.SafeThreadCall(this, "SetText", value);
				else
					SetText(value);
			}
		}
		
		public PrintDocument PrintDocument {
			get {
				return textAreaControl.PrintDocument;
			}
		}
		
		public override Control Control {
			get {
				return textAreaControl;
			}
		}
		
		public override string TabPageText {
			get {
				return "${res:FormsDesigner.DesignTabPages.SourceTabPage}";
			}
		}
		
		public override string UntitledName {
			get {
				return base.UntitledName;
			}
			set {
				base.UntitledName = value;
				textAreaControl.FileName = value;
				ForceFoldingUpdate();
			}
		}
		
		
		protected override void OnFileNameChanged(System.EventArgs e)
		{
			base.OnFileNameChanged(e);
			textAreaControl.FileName = base.FileName;
		}
		
		public void Undo()
		{
			this.textAreaControl.Undo();
		}
		
		public void Redo()
		{
			this.textAreaControl.Redo();
		}
		
		protected virtual SharpDevelopTextAreaControl CreateSharpDevelopTextAreaControl()
		{
			return new SharpDevelopTextAreaControl();
		}
		
		public TextEditorDisplayBindingWrapper()
		{
			textAreaControl = CreateSharpDevelopTextAreaControl();
			textAreaControl.RightToLeft = RightToLeft.No;
			textAreaControl.Document.DocumentChanged += new DocumentEventHandler(TextAreaChangedEvent);
			textAreaControl.ActiveTextAreaControl.Caret.CaretModeChanged += new EventHandler(CaretModeChanged);
			textAreaControl.ActiveTextAreaControl.Enter += new EventHandler(CaretUpdate);
			
			// KSL Start, New lines
//			textAreaControl.FileNameChanged += new EventHandler(FileNameChangedEvent);
			((Form)ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench).Activated += new EventHandler(GotFocusEvent);
			// KSL End
			
			
		}
		// KSL Start, new event handlers
		
		public void ShowHelp()
		{
			// Resolve expression at cursor and show help
			TextArea textArea = textAreaControl.ActiveTextAreaControl.TextArea;
			IDocument doc = textArea.Document;
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(textArea.MotherTextEditorControl.FileName);
			if (expressionFinder == null)
				return;
			LineSegment seg = doc.GetLineSegment(textArea.Caret.Line);
			string textContent = doc.TextContent;
			ExpressionResult expressionResult = expressionFinder.FindFullExpression(textContent, seg.Offset + textArea.Caret.Column);
			string expression = expressionResult.Expression;
			if (expression != null && expression.Length > 0) {
				ResolveResult result = ParserService.Resolve(expressionResult, textArea.Caret.Line + 1, textArea.Caret.Column + 1, textAreaControl.FileName, textContent);
				TypeResolveResult trr = result as TypeResolveResult;
				if (trr != null) {
					ICSharpCode.SharpDevelop.Dom.HelpProvider.ShowHelp(trr.ResolvedClass);
				}
				MemberResolveResult mrr = result as MemberResolveResult;
				if (mrr != null) {
					ICSharpCode.SharpDevelop.Dom.HelpProvider.ShowHelp(mrr.ResolvedMember);
				}
			}
		}
		
		protected void SetWatcher()
		{
			try {
				if (this.watcher == null) {
					this.watcher = new FileSystemWatcher();
					this.watcher.Changed += new FileSystemEventHandler(this.OnFileChangedEvent);
				} else {
					this.watcher.EnableRaisingEvents = false;
				}
				this.watcher.Path = Path.GetDirectoryName(textAreaControl.FileName);
				this.watcher.Filter = Path.GetFileName(textAreaControl.FileName);
				this.watcher.NotifyFilter = NotifyFilters.LastWrite;
				this.watcher.EnableRaisingEvents = true;
			} catch (Exception) {
				if (watcher != null) {
					watcher.Dispose();
				}
				watcher = null;
			}
		}
		protected virtual void GotFocusEvent(object sender, EventArgs e)
		{
			if (wasChangedExternally) {
				wasChangedExternally = false;
				
				string message = StringParser.Parse("${res:ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.TextEditorDisplayBinding.FileAlteredMessage}", new string[,] {{"File", Path.GetFullPath(textAreaControl.FileName)}});
				if (MessageBox.Show(message,
				                    StringParser.Parse("${res:MainWindow.DialogName}"),
				                    MessageBoxButtons.YesNo,
				                    MessageBoxIcon.Question) == DialogResult.Yes) {
					Load(textAreaControl.FileName);
				} else {
					IsDirty = true;
				}
			}
		}
		
		void OnFileChangedEvent(object sender, FileSystemEventArgs e)
		{
			if(e.ChangeType != WatcherChangeTypes.Deleted) {
				wasChangedExternally = true;
				if (textAreaControl.IsHandleCreated)
					textAreaControl.BeginInvoke(new MethodInvoker(OnFileChangedEventInvoked));
			}
		}
		
		void OnFileChangedEventInvoked()
		{
			if (((Form)ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench).Focused) {
				GotFocusEvent(this, EventArgs.Empty);
			}
		}
		
		// KSL End
		
		void TextAreaChangedEvent(object sender, DocumentEventArgs e)
		{
			IsDirty = true;
		}
		
		public override void RedrawContent()
		{
			textAreaControl.OptionsChanged();
			textAreaControl.Refresh();
		}
		
		public override void Dispose()
		{
			if (this.IsUntitled) {
				ParserService.ClearParseInformation(this.UntitledName);
			}
			if (WorkbenchSingleton.MainForm != null) {
				WorkbenchSingleton.MainForm.Activated -= new EventHandler(GotFocusEvent);
			}
			if (this.watcher != null) {
				this.watcher.Dispose();
				this.watcher = null;
			}
			textAreaControl.Dispose();
			base.Dispose();
		}
		
		public override bool IsReadOnly {
			get {
				return textAreaControl.IsReadOnly;
			}
		}
		
		public override void Save(string fileName)
		{
			OnSaving(EventArgs.Empty);
			// KSL, Start new line
			if (watcher != null) {
				this.watcher.EnableRaisingEvents = false;
			}
			// KSL End
			
			if (!textAreaControl.CanSaveWithCurrentEncoding()) {
				if (MessageService.AskQuestion("The file cannot be saved with the current encoding " +
				                               textAreaControl.Encoding.EncodingName + " without losing data." +
				                               "\nDo you want to save it using UTF-8 instead?")) {
					textAreaControl.Encoding = System.Text.Encoding.UTF8;
				}
			}
			
			textAreaControl.SaveFile(fileName);
			if (fileName != this.FileName) {
				ParserService.ClearParseInformation(this.FileName ?? this.UntitledName);
				FileName  = fileName;
				ParserService.ParseViewContent(this);
			}
			TitleName = Path.GetFileName(fileName);
			IsDirty   = false;
			
			SetWatcher();
			OnSaved(new SaveEventArgs(true));
		}
		
		public override void Load(string fileName)
		{
			textAreaControl.IsReadOnly = (File.GetAttributes(fileName) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
			
			bool autodetectEncoding = true;
			textAreaControl.LoadFile(fileName, true, autodetectEncoding);
			FileName  = fileName;
			TitleName = Path.GetFileName(fileName);
			IsDirty     = false;
			SetWatcher();
			foreach (Bookmarks.SDBookmark mark in Bookmarks.BookmarkManager.GetBookmarks(fileName)) {
				mark.Document = textAreaControl.Document;
				textAreaControl.Document.BookmarkManager.Marks.Add(mark);
			}
		}
		
		public Properties CreateMemento()
		{
			Properties properties = new Properties();
			properties.Set("CaretOffset", textAreaControl.ActiveTextAreaControl.Caret.Offset);
			properties.Set("VisibleLine", textAreaControl.ActiveTextAreaControl.TextArea.TextView.FirstVisibleLine);
			properties.Set("HighlightingLanguage", textAreaControl.Document.HighlightingStrategy.Name);
			properties.Set("Foldings", textAreaControl.Document.FoldingManager.SerializeToString());
			return properties;
		}
		
		public void SetMemento(Properties memento)
		{
			Properties properties = (Properties)memento;
			textAreaControl.ActiveTextAreaControl.Caret.Position =  textAreaControl.Document.OffsetToPosition(Math.Min(textAreaControl.Document.TextLength, Math.Max(0, properties.Get("CaretOffset", textAreaControl.ActiveTextAreaControl.Caret.Offset))));
//			textAreaControl.SetDesiredColumn();
			
			if (textAreaControl.Document.HighlightingStrategy.Name != properties.Get("HighlightingLanguage", textAreaControl.Document.HighlightingStrategy.Name)) {
				IHighlightingStrategy highlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy(properties.Get("HighlightingLanguage", textAreaControl.Document.HighlightingStrategy.Name));
				if (highlightingStrategy != null) {
					textAreaControl.Document.HighlightingStrategy = highlightingStrategy;
				}
			}
			textAreaControl.ActiveTextAreaControl.TextArea.TextView.FirstVisibleLine = properties.Get("VisibleLine", 0);
			
			textAreaControl.Document.FoldingManager.DeserializeFromString(properties.Get("Foldings", ""));
//			// insane check for cursor position, may be required for document reload.
//			int lineNr = textAreaControl.Document.GetLineNumberForOffset(textAreaControl.Document.Caret.Offset);
//			LineSegment lineSegment = textAreaControl.Document.GetLineSegment(lineNr);
//			textAreaControl.Document.Caret.Offset = Math.Min(lineSegment.Offset + lineSegment.Length, textAreaControl.Document.Caret.Offset);
//
//			textAreaControl.OptionsChanged();
//			textAreaControl.Refresh();
		}
		
		void CaretUpdate(object sender, EventArgs e)
		{
			CaretChanged(null, null);
			CaretModeChanged(null, null);
		}
		
		void CaretChanged(object sender, EventArgs e)
		{
			Point    pos       = textAreaControl.Document.OffsetToPosition(textAreaControl.ActiveTextAreaControl.Caret.Offset);
			LineSegment line   = textAreaControl.Document.GetLineSegment(pos.Y);
			
			StatusBarService.SetCaretPosition(pos.X + 1, pos.Y + 1, textAreaControl.ActiveTextAreaControl.Caret.Offset - line.Offset + 1);
		}
		
		void CaretModeChanged(object sender, EventArgs e)
		{
			
			StatusBarService.SetInsertMode(textAreaControl.ActiveTextAreaControl.Caret.CaretMode == CaretMode.InsertMode);
		}
		
		public override string FileName {
			set {
				if (Path.GetExtension(FileName) != Path.GetExtension(value)) {
					if (textAreaControl.Document.HighlightingStrategy != null) {
						textAreaControl.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategyForFile(value);
						textAreaControl.Refresh();
					}
				}
				base.FileName  = value;
				base.TitleName = Path.GetFileName(value);
			}
		}
		public void JumpTo(int line, int column)
		{
			textAreaControl.ActiveTextAreaControl.JumpTo(line, column);
		}
		
		public void ForceFoldingUpdate()
		{
			if (textAreaControl.TextEditorProperties.EnableFolding) {
				string fileName = textAreaControl.FileName;
				ParseInformation parseInfo = ParserService.GetParseInformation(fileName);
				if (parseInfo == null) {
					parseInfo = ParserService.ParseFile(fileName,
					                                    textAreaControl.Document.TextContent, false, false);
				}
				textAreaControl.Document.FoldingManager.UpdateFoldings(fileName, parseInfo);
				UpdateClassMemberBookmarks(parseInfo);
			}
		}
		
		public void ParseInformationUpdated(ParseInformation parseInfo)
		{
			if (textAreaControl.TextEditorProperties.EnableFolding) {
				WorkbenchSingleton.SafeThreadAsyncCall(this, "ParseInformationUpdatedInvoked", parseInfo);
			}
		}
		
		void ParseInformationUpdatedInvoked(ParseInformation parseInfo)
		{
			try {
				textAreaControl.Document.FoldingManager.UpdateFoldings(TitleName, parseInfo);
				UpdateClassMemberBookmarks(parseInfo);
				textAreaControl.ActiveTextAreaControl.TextArea.Refresh(textAreaControl.ActiveTextAreaControl.TextArea.FoldMargin);
				textAreaControl.ActiveTextAreaControl.TextArea.Refresh(textAreaControl.ActiveTextAreaControl.TextArea.IconBarMargin);
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}
		
		void UpdateClassMemberBookmarks(ParseInformation parseInfo)
		{
			BookmarkManager bm = textAreaControl.Document.BookmarkManager;
			bm.RemoveMarks(new Predicate<Bookmark>(IsClassMemberBookmark));
			if (parseInfo == null) return;
			foreach (IClass c in parseInfo.MostRecentCompilationUnit.Classes) {
				AddClassMemberBookmarks(bm, c);
			}
		}
		
		void AddClassMemberBookmarks(BookmarkManager bm, IClass c)
		{
			if (c.IsSynthetic) return;
			if (!c.Region.IsEmpty) {
				bm.AddMark(new Bookmarks.ClassBookmark(textAreaControl.Document, c));
			}
			foreach (IClass innerClass in c.InnerClasses) {
				AddClassMemberBookmarks(bm, innerClass);
			}
			foreach (IMethod m in c.Methods) {
				if (m.Region.IsEmpty || m.IsSynthetic) continue;
				bm.AddMark(new Bookmarks.MethodBookmark(textAreaControl.Document, m));
			}
			foreach (IProperty m in c.Properties) {
				if (m.Region.IsEmpty || m.IsSynthetic) continue;
				bm.AddMark(new Bookmarks.PropertyBookmark(textAreaControl.Document, m));
			}
			foreach (IField f in c.Fields) {
				if (f.Region.IsEmpty || f.IsSynthetic) continue;
				bm.AddMark(new Bookmarks.FieldBookmark(textAreaControl.Document, f));
			}
			foreach (IEvent e in c.Events) {
				if (e.Region.IsEmpty || e.IsSynthetic) continue;
				bm.AddMark(new Bookmarks.EventBookmark(textAreaControl.Document, e));
			}
		}
		
		bool IsClassMemberBookmark(Bookmark b)
		{
			return b is Bookmarks.ClassMemberBookmark || b is Bookmarks.ClassBookmark;
		}
		
		#region ICSharpCode.SharpDevelop.Gui.IClipboardHandler interface implementation
		public bool EnableCut {
			get {
				return textAreaControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableCut;
			}
		}
		
		public bool EnableCopy {
			get {
				return textAreaControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableCopy;
			}
		}
		
		public bool EnablePaste {
			get {
				return textAreaControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnablePaste;
			}
		}
		
		public bool EnableDelete {
			get {
				return textAreaControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableDelete;
			}
		}
		
		public bool EnableSelectAll {
			get {
				return textAreaControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableSelectAll;
			}
		}
		
		public void SelectAll()
		{
			textAreaControl.ActiveTextAreaControl.TextArea.ClipboardHandler.SelectAll(null, null);
		}
		
		public void Delete()
		{
			textAreaControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Delete(null, null);
		}
		
		public void Paste()
		{
			textAreaControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Paste(null, null);
		}
		
		public void Copy()
		{
			textAreaControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Copy(null, null);
		}
		
		public void Cut()
		{
			textAreaControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Cut(null, null);
		}
		#endregion
	}
}
