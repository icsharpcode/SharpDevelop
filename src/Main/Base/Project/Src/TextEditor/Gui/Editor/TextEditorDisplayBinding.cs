// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class TextEditorDisplayBinding : IDisplayBinding
	{
		static TextEditorDisplayBinding()
		{
			// load #D-specific syntax highlighting files here
			string modeDir = Path.Combine(PropertyService.ConfigDirectory, "modes");
			if (!Directory.Exists(modeDir)) {
				Directory.CreateDirectory(modeDir);
			}
			
			HighlightingManager.Manager.AddSyntaxModeFileProvider(new ICSharpCode.SharpDevelop.DefaultEditor.Codons.AddInTreeSyntaxModeProvider());
			HighlightingManager.Manager.AddSyntaxModeFileProvider(new FileSyntaxModeProvider(Path.Combine(PropertyService.DataDirectory, "modes")));
			HighlightingManager.Manager.AddSyntaxModeFileProvider(new FileSyntaxModeProvider(modeDir));
			ClipboardHandling.Initialize();
		}
		
		/// <summary>
		/// Force static constructor to be called. Otherwise other editor's such as the XML editor do not
		/// use custom syntax highlighting.
		/// </summary>
		public static void InitializeSyntaxModes()
		{
		}
		
		public virtual bool CanCreateContentForFile(string fileName)
		{
			return true;
		}
		
		protected virtual TextEditorDisplayBindingWrapper CreateWrapper(OpenedFile file)
		{
			return new TextEditorDisplayBindingWrapper(file);
		}
		
		public virtual IViewContent CreateContentForFile(OpenedFile file)
		{
			TextEditorDisplayBindingWrapper b2 = CreateWrapper(file);
			file.ForceInitializeView(b2); // load file to initialize folding etc.
			
			b2.textEditorControl.Dock = DockStyle.Fill;
			try {
				b2.textEditorControl.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategyForFile(file.FileName);
				b2.textEditorControl.InitializeAdvancedHighlighter();
			} catch (HighlightingDefinitionInvalidException ex) {
				b2.textEditorControl.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy();
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			b2.textEditorControl.InitializeFormatter();
			b2.textEditorControl.ActivateQuickClassBrowserOnDemand();
			
			return b2;
		}
	}
	
	public class TextEditorDisplayBindingWrapper : AbstractViewContent, IMementoCapable, IPrintable, IEditable, IUndoHandler, IPositionable, ITextEditorControlProvider, IParseInformationListener, IClipboardHandler, IContextHelpProvider, IToolsHost
	{
		internal readonly SharpDevelopTextAreaControl textEditorControl;
		
		public TextEditorControl TextEditorControl {
			get {
				return textEditorControl;
			}
		}
		
		public IDocument GetDocumentForFile(OpenedFile file)
		{
			if (file == this.PrimaryFile) {
				return this.TextEditorControl.Document;
			} else {
				return null;
			}
		}
		
		public bool EnableUndo {
			get {
				return textEditorControl.EnableUndo;
			}
		}
		public bool EnableRedo {
			get {
				return textEditorControl.EnableRedo;
			}
		}
		
		// ParserUpdateThread uses the text property via IEditable, I had an exception
		// because multiple threads were accessing the GapBufferStrategy at the same time.
		
		string GetText()
		{
			return textEditorControl.Document.TextContent;
		}
		
		void SetText(string value)
		{
			textEditorControl.Document.Replace(0, textEditorControl.Document.TextLength, value);
		}
		
		public string Text {
			get {
				if (WorkbenchSingleton.InvokeRequired)
					return WorkbenchSingleton.SafeThreadFunction<string>(GetText);
				else
					return GetText();
			}
			set {
				if (WorkbenchSingleton.InvokeRequired)
					WorkbenchSingleton.SafeThreadCall(SetText, value);
				else
					SetText(value);
			}
		}
		
		public PrintDocument PrintDocument {
			get {
				return textEditorControl.PrintDocument;
			}
		}
		
		public override Control Control {
			get {
				return textEditorControl;
			}
		}
		
		public void Undo()
		{
			this.textEditorControl.Undo();
		}
		
		public void Redo()
		{
			this.textEditorControl.Redo();
		}
		
		protected virtual SharpDevelopTextAreaControl CreateSharpDevelopTextAreaControl()
		{
			return new SharpDevelopTextAreaControl();
		}
		
		public TextEditorDisplayBindingWrapper(OpenedFile file) : base(file)
		{
			this.TabPageText = "${res:FormsDesigner.DesignTabPages.SourceTabPage}";
			
			textEditorControl = CreateSharpDevelopTextAreaControl();
			textEditorControl.RightToLeft = RightToLeft.No;
			textEditorControl.Document.DocumentChanged += new DocumentEventHandler(TextAreaChangedEvent);
			textEditorControl.ActiveTextAreaControl.Caret.CaretModeChanged += new EventHandler(CaretModeChanged);
			textEditorControl.ActiveTextAreaControl.Enter += new EventHandler(CaretUpdate);
			textEditorControl.ActiveTextAreaControl.Caret.PositionChanged += CaretUpdate;
			
			textEditorControl.FileName = file.FileName;
		}
		
		public void ShowHelp()
		{
			// Resolve expression at cursor and show help
			TextArea textArea = textEditorControl.ActiveTextAreaControl.TextArea;
			IDocument doc = textArea.Document;
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(textArea.MotherTextEditorControl.FileName);
			if (expressionFinder == null)
				return;
			LineSegment seg = doc.GetLineSegment(textArea.Caret.Line);
			string textContent = doc.TextContent;
			ExpressionResult expressionResult = expressionFinder.FindFullExpression(textContent, seg.Offset + textArea.Caret.Column);
			string expression = expressionResult.Expression;
			if (expression != null && expression.Length > 0) {
				ResolveResult result = ParserService.Resolve(expressionResult, textArea.Caret.Line + 1, textArea.Caret.Column + 1, textEditorControl.FileName, textContent);
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
		
		void TextAreaChangedEvent(object sender, DocumentEventArgs e)
		{
			this.PrimaryFile.MakeDirty();
			NavigationService.ContentChanging(this.textEditorControl, e);
		}
		
		public override void RedrawContent()
		{
			textEditorControl.OptionsChanged();
			textEditorControl.Refresh();
		}
		
		public override void Dispose()
		{
			if (this.PrimaryFile.IsUntitled) {
				ParserService.ClearParseInformation(this.PrimaryFile.FileName);
			}
			textEditorControl.Dispose();
			base.Dispose();
		}
		
		public override bool IsReadOnly {
			get {
				return textEditorControl.IsReadOnly;
			}
		}
		
		public override void Save(OpenedFile file, Stream stream)
		{
			if (file != PrimaryFile)
				throw new ArgumentException("file != PrimaryFile");
			
			if (!textEditorControl.CanSaveWithCurrentEncoding()) {
				if (MessageService.AskQuestion("The file cannot be saved with the current encoding " +
				                               textEditorControl.Encoding.EncodingName + " without losing data." +
				                               "\nDo you want to save it using UTF-8 instead?")) {
					textEditorControl.Encoding = System.Text.Encoding.UTF8;
				}
			}
			
			textEditorControl.SaveFile(stream);
		}
		
		public override void Load(OpenedFile file, Stream stream)
		{
			if (file != PrimaryFile)
				throw new ArgumentException("file != PrimaryFile");
			
			if (!file.IsUntitled) {
				textEditorControl.IsReadOnly = (File.GetAttributes(file.FileName) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
			}
			
			bool autodetectEncoding = true;
			textEditorControl.LoadFile(file.FileName, stream, true, autodetectEncoding);
			textEditorControl.FileLoaded();
			foreach (Bookmarks.SDBookmark mark in Bookmarks.BookmarkManager.GetBookmarks(file.FileName)) {
				mark.Document = textEditorControl.Document;
				textEditorControl.Document.BookmarkManager.AddMark(mark);
			}
			ForceFoldingUpdate();
		}
		
		public Properties CreateMemento()
		{
			Properties properties = new Properties();
			properties.Set("CaretOffset", textEditorControl.ActiveTextAreaControl.Caret.Offset);
			properties.Set("VisibleLine", textEditorControl.ActiveTextAreaControl.TextArea.TextView.FirstVisibleLine);
			if (textEditorControl.HighlightingExplicitlySet) {
				properties.Set("HighlightingLanguage", textEditorControl.Document.HighlightingStrategy.Name);
			}
			return properties;
		}
		
		public void SetMemento(Properties properties)
		{
			textEditorControl.ActiveTextAreaControl.Caret.Position =  textEditorControl.Document.OffsetToPosition(Math.Min(textEditorControl.Document.TextLength, Math.Max(0, properties.Get("CaretOffset", textEditorControl.ActiveTextAreaControl.Caret.Offset))));
//			textAreaControl.SetDesiredColumn();
			
			string highlightingName = properties.Get("HighlightingLanguage", string.Empty);
			if (!string.IsNullOrEmpty(highlightingName)) {
				if (highlightingName == textEditorControl.Document.HighlightingStrategy.Name) {
					textEditorControl.HighlightingExplicitlySet = true;
				} else {
					IHighlightingStrategy highlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy(highlightingName);
					if (highlightingStrategy != null) {
						textEditorControl.HighlightingExplicitlySet = true;
						textEditorControl.Document.HighlightingStrategy = highlightingStrategy;
					}
				}
			}
			textEditorControl.ActiveTextAreaControl.TextArea.TextView.FirstVisibleLine = properties.Get("VisibleLine", 0);
			
//			// insane check for cursor position, may be required for document reload.
//			int lineNr = textAreaControl.Document.GetLineNumberForOffset(textAreaControl.Document.Caret.Offset);
//			LineSegment lineSegment = textAreaControl.Document.GetLineSegment(lineNr);
//			textAreaControl.Document.Caret.Offset = Math.Min(lineSegment.Offset + lineSegment.Length, textAreaControl.Document.Caret.Offset);
//
//			textAreaControl.OptionsChanged();
//			textAreaControl.Refresh();
		}
		
		public override INavigationPoint BuildNavPoint()
		{
			int lineNumber = this.Line;
			LineSegment lineSegment = textEditorControl.Document.GetLineSegment(lineNumber);
			string txt = textEditorControl.Document.GetText(lineSegment);
			return new TextNavigationPoint(this.PrimaryFileName, lineNumber, this.Column, txt);
		}
		
		void CaretUpdate(object sender, EventArgs e)
		{
			CaretChanged(null, null);
			CaretModeChanged(null, null);
		}
		
		void CaretChanged(object sender, EventArgs e)
		{
			TextAreaControl activeTextAreaControl = textEditorControl.ActiveTextAreaControl;
			int line = activeTextAreaControl.Caret.Line;
			int col = activeTextAreaControl.Caret.Column;
			StatusBarService.SetCaretPosition(activeTextAreaControl.TextArea.TextView.GetVisualColumn(line, col) + 1, line + 1, col + 1);
			NavigationService.Log(this.BuildNavPoint());
		}
		
		void CaretModeChanged(object sender, EventArgs e)
		{
			StatusBarService.SetInsertMode(textEditorControl.ActiveTextAreaControl.Caret.CaretMode == CaretMode.InsertMode);
		}
		
		protected override void OnFileNameChanged(OpenedFile file)
		{
			base.OnFileNameChanged(file);
			Debug.Assert(file == this.Files[0]);
			
			string oldFileName = textEditorControl.FileName;
			string newFileName = file.FileName;
			
			if (Path.GetExtension(oldFileName) != Path.GetExtension(newFileName)) {
				if (textEditorControl.Document.HighlightingStrategy != null) {
					textEditorControl.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategyForFile(newFileName);
					textEditorControl.Refresh();
				}
			}
			
			SetIcon();
			
			ParserService.ClearParseInformation(oldFileName);
			textEditorControl.FileName = newFileName;
			ParserService.ParseViewContent(this);
		}
		
		protected override void OnWorkbenchWindowChanged()
		{
			base.OnWorkbenchWindowChanged();
			SetIcon();
		}
		
		void SetIcon()
		{
			if (this.WorkbenchWindow != null) {
				System.Drawing.Icon icon = WinFormsResourceService.GetIcon(IconService.GetImageForFile(this.PrimaryFileName));
				if (icon != null) {
					this.WorkbenchWindow.Icon = icon;
				}
			}
		}
		
		#region IPositionable implementation
		public void JumpTo(int line, int column)
		{
			textEditorControl.ActiveTextAreaControl.JumpTo(line, column);
			
			// we need to delay this call here because the text editor does not know its height if it was just created
			WorkbenchSingleton.SafeThreadAsyncCall(
				delegate {
					textEditorControl.ActiveTextAreaControl.CenterViewOn(
						line, (int)(0.3 * textEditorControl.ActiveTextAreaControl.TextArea.TextView.VisibleLineCount));
				});
		}
		
		public int Line {
			get {
				return textEditorControl.ActiveTextAreaControl.Caret.Line;
			}
		}
		
		public int Column {
			get {
				return textEditorControl.ActiveTextAreaControl.Caret.Column;
			}
		}
		
		#endregion
		
		public void ForceFoldingUpdate()
		{
			if (textEditorControl.TextEditorProperties.EnableFolding) {
				string fileName = textEditorControl.FileName;
				ParseInformation parseInfo = ParserService.GetParseInformation(fileName);
				if (parseInfo == null) {
					parseInfo = ParserService.ParseFile(fileName,
					                                    textEditorControl.Document.TextContent, false);
				}
				textEditorControl.Document.FoldingManager.UpdateFoldings(fileName, parseInfo);
				UpdateClassMemberBookmarks(parseInfo);
			}
		}
		
		public void ParseInformationUpdated(ParseInformation parseInfo)
		{
			if (textEditorControl.TextEditorProperties.EnableFolding) {
				WorkbenchSingleton.SafeThreadAsyncCall(ParseInformationUpdatedInvoked, parseInfo);
			}
		}
		
		void ParseInformationUpdatedInvoked(ParseInformation parseInfo)
		{
			try {
				textEditorControl.Document.FoldingManager.UpdateFoldings(TitleName, parseInfo);
				UpdateClassMemberBookmarks(parseInfo);
				textEditorControl.ActiveTextAreaControl.TextArea.Refresh(textEditorControl.ActiveTextAreaControl.TextArea.FoldMargin);
				textEditorControl.ActiveTextAreaControl.TextArea.Refresh(textEditorControl.ActiveTextAreaControl.TextArea.IconBarMargin);
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}
		
		void UpdateClassMemberBookmarks(ParseInformation parseInfo)
		{
			BookmarkManager bm = textEditorControl.Document.BookmarkManager;
			bm.RemoveMarks(new Predicate<Bookmark>(IsClassMemberBookmark));
			if (parseInfo == null) return;
			Debug.Assert(textEditorControl.Document.TotalNumberOfLines >= 1);
			if (textEditorControl.Document.TotalNumberOfLines < 1) {
				return;
			}
			foreach (IClass c in parseInfo.MostRecentCompilationUnit.Classes) {
				AddClassMemberBookmarks(bm, c);
			}
		}
		
		void AddClassMemberBookmarks(BookmarkManager bm, IClass c)
		{
			if (c.IsSynthetic) return;
			if (!c.Region.IsEmpty) {
				bm.AddMark(new Bookmarks.ClassBookmark(textEditorControl.Document, c));
			}
			foreach (IClass innerClass in c.InnerClasses) {
				AddClassMemberBookmarks(bm, innerClass);
			}
			foreach (IMethod m in c.Methods) {
				if (m.Region.IsEmpty || m.IsSynthetic) continue;
				bm.AddMark(new Bookmarks.MethodBookmark(textEditorControl.Document, m));
			}
			foreach (IProperty m in c.Properties) {
				if (m.Region.IsEmpty || m.IsSynthetic) continue;
				bm.AddMark(new Bookmarks.PropertyBookmark(textEditorControl.Document, m));
			}
			foreach (IField f in c.Fields) {
				if (f.Region.IsEmpty || f.IsSynthetic) continue;
				bm.AddMark(new Bookmarks.FieldBookmark(textEditorControl.Document, f));
			}
			foreach (IEvent e in c.Events) {
				if (e.Region.IsEmpty || e.IsSynthetic) continue;
				bm.AddMark(new Bookmarks.EventBookmark(textEditorControl.Document, e));
			}
		}
		
		bool IsClassMemberBookmark(Bookmark b)
		{
			return b is Bookmarks.ClassMemberBookmark || b is Bookmarks.ClassBookmark;
		}
		
		#region ICSharpCode.SharpDevelop.Gui.IClipboardHandler interface implementation
		public bool EnableCut {
			get {
				return !this.IsDisposed && textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableCut;
			}
		}
		
		public bool EnableCopy {
			get {
				return !this.IsDisposed && textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableCopy;
			}
		}
		
		public bool EnablePaste {
			get {
				return !this.IsDisposed && textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnablePaste;
			}
		}
		
		public bool EnableDelete {
			get {
				return !this.IsDisposed && textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableDelete;
			}
		}
		
		public bool EnableSelectAll {
			get {
				return !this.IsDisposed && textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableSelectAll;
			}
		}
		
		public void SelectAll()
		{
			textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.SelectAll(null, null);
		}
		
		public void Delete()
		{
			textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Delete(null, null);
		}
		
		public void Paste()
		{
			textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Paste(null, null);
		}
		
		public void Copy()
		{
			textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Copy(null, null);
		}
		
		public void Cut()
		{
			textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Cut(null, null);
		}
		#endregion
		
		Control IToolsHost.ToolsControl {
			get { return TextEditorSideBar.Instance; }
		}
		
		public override string ToString()
		{
			return "[" + GetType().Name + " " + this.PrimaryFileName + "]";
		}
	}
}
