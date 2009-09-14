// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Linq;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using System.Windows.Threading;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public class AvalonEditViewContent
		: AbstractViewContent, IEditable, IMementoCapable, ITextEditorProvider, IPositionable, IParseInformationListener, IToolsHost
	{
		readonly CodeEditor codeEditor = new CodeEditor();
		
		public AvalonEditViewContent(OpenedFile file)
		{
			this.TabPageText = "${res:FormsDesigner.DesignTabPages.SourceTabPage}";
			
			this.Files.Add(file);
			file.ForceInitializeView(this);
			codeEditor.Document.Changed += textEditor_Document_Changed;
			codeEditor.CaretPositionChanged += CaretChanged;
			codeEditor.TextCopied += codeEditor_TextCopied;
		}
		
		void codeEditor_TextCopied(object sender, ICSharpCode.AvalonEdit.Editing.TextEventArgs e)
		{
			TextEditorSideBar.Instance.PutInClipboardRing(e.Text);
		}
		
		void textEditor_Document_Changed(object sender, DocumentChangeEventArgs e)
		{
			if (!isLoading) {
				PrimaryFile.IsDirty = true;
			}
		}
		
		public CodeEditor CodeEditor {
			get { return codeEditor; }
		}
		
		public override object Control {
			get { return codeEditor; }
		}
		
		public override void Save(OpenedFile file, Stream stream)
		{
			if (file != PrimaryFile)
				return;
			codeEditor.Save(stream);
		}
		
		bool isLoading;
		
		public override void Load(OpenedFile file, Stream stream)
		{
			if (file != PrimaryFile)
				return;
			isLoading = true;
			try {
				BookmarksDetach();
				codeEditor.PrimaryTextEditor.SyntaxHighlighting =
					HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(file.FileName));
				
				codeEditor.Load(stream);
				// we set the file name after loading because this will place the fold markers etc.
				codeEditor.FileName = file.FileName;
				BookmarksAttach();
			} finally {
				isLoading = false;
			}
		}
		
		protected override void OnFileNameChanged(OpenedFile file)
		{
			base.OnFileNameChanged(file);
			if (file == PrimaryFile) {
				if (!string.IsNullOrEmpty(codeEditor.FileName))
					ParserService.ClearParseInformation(codeEditor.FileName);
				
				codeEditor.FileName = file.FileName;
				
				ParserService.BeginParse(file.FileName, codeEditor.DocumentAdapter);
				
				BookmarksNotifyNameChange(file.FileName);
			}
		}
		
		public override INavigationPoint BuildNavPoint()
		{
			int lineNumber = this.Line;
			string txt = codeEditor.Document.GetLineByNumber(lineNumber).Text;
			return new TextNavigationPoint(this.PrimaryFileName, lineNumber, this.Column, txt);
		}
		
		void CaretChanged(object sender, EventArgs e)
		{
			NavigationService.Log(this.BuildNavPoint());
		}
		
		#region Bookmark Handling
		void BookmarksAttach()
		{
			foreach (SDBookmark bookmark in BookmarkManager.GetBookmarks(codeEditor.FileName)) {
				bookmark.Document = codeEditor.DocumentAdapter;
				codeEditor.IconBarManager.Bookmarks.Add(bookmark);
			}
			BookmarkManager.Added += BookmarkManager_Added;
			BookmarkManager.Removed += BookmarkManager_Removed;
		}

		void BookmarksDetach()
		{
			BookmarkManager.Added -= BookmarkManager_Added;
			BookmarkManager.Removed -= BookmarkManager_Removed;
			foreach (SDBookmark bookmark in codeEditor.IconBarManager.Bookmarks.OfType<SDBookmark>()) {
				if (bookmark.Document == codeEditor.DocumentAdapter) {
					bookmark.Document = null;
				}
			}
			codeEditor.IconBarManager.Bookmarks.Clear();
		}
		
		void BookmarkManager_Removed(object sender, BookmarkEventArgs e)
		{
			codeEditor.IconBarManager.Bookmarks.Remove(e.Bookmark);
			if (e.Bookmark.Document == codeEditor.DocumentAdapter) {
				e.Bookmark.Document = null;
			}
		}
		
		void BookmarkManager_Added(object sender, BookmarkEventArgs e)
		{
			if (FileUtility.IsEqualFileName(this.PrimaryFileName, e.Bookmark.FileName)) {
				codeEditor.IconBarManager.Bookmarks.Add(e.Bookmark);
				e.Bookmark.Document = codeEditor.DocumentAdapter;
			}
		}
		
		void BookmarksNotifyNameChange(string newFileName)
		{
			foreach (SDBookmark bookmark in codeEditor.IconBarManager.Bookmarks.OfType<SDBookmark>()) {
				bookmark.FileName = newFileName;
			}
		}
		#endregion
		
		public override void Dispose()
		{
			base.Dispose();
			BookmarksDetach();
			codeEditor.Dispose();
		}
		
		public override string ToString()
		{
			return "[" + GetType().Name + " " + this.PrimaryFileName + "]";
		}
		
		#region IEditable
		public ITextBuffer CreateSnapshot()
		{
			return codeEditor.DocumentAdapter.CreateSnapshot();
		}
		
		/// <summary>
		/// Gets the document text.
		/// </summary>
		public string Text {
			get {
				return codeEditor.Document.Text;
			}
		}
		#endregion
		
		#region IMementoCapable
		public Properties CreateMemento()
		{
			Properties memento = new Properties();
			memento.Set("CaretOffset", codeEditor.ActiveTextEditor.CaretOffset);
			memento.Set("ScrollPositionY", codeEditor.ActiveTextEditor.VerticalOffset);
			return memento;
		}
		
		public void SetMemento(Properties memento)
		{
			codeEditor.PrimaryTextEditor.ScrollToVerticalOffset(memento.Get("ScrollPositionY", 0.0));
			try {
				codeEditor.PrimaryTextEditor.CaretOffset = memento.Get("CaretOffset", 0);
			} catch (ArgumentOutOfRangeException) {
				// ignore caret out of range - maybe file was changed externally?
			}
		}
		#endregion
		
		#region ITextEditorProvider
		public ITextEditor TextEditor {
			get { return codeEditor.ActiveTextEditorAdapter; }
		}
		
		public IDocument GetDocumentForFile(OpenedFile file)
		{
			if (file == this.PrimaryFile)
				return codeEditor.DocumentAdapter;
			else
				return null;
		}
		#endregion
		
		#region IPositionable
		public int Line {
			get { return this.TextEditor.Caret.Line; }
		}
		
		public int Column {
			get { return this.TextEditor.Caret.Column; }
		}
		
		public void JumpTo(int line, int column)
		{
			codeEditor.ActiveTextEditor.JumpTo(line, column);
		}
		#endregion
		
		#region IParseInformationListener
		ParseInformation updateParseInfoTo;
		
		public void ParseInformationUpdated(ParseInformation parseInfo)
		{
			// When parse information is updated quickly in succession, only do a single update
			// to the latest version.
			updateParseInfoTo = parseInfo;
			codeEditor.Dispatcher.BeginInvoke(
				DispatcherPriority.Background,
				new Action(
					delegate {
						if (updateParseInfoTo != null) {
							codeEditor.ParseInformationUpdated(updateParseInfoTo);
							updateParseInfoTo = null;
						}
					}));
		}
		#endregion
		
		object IToolsHost.ToolsContent {
			get { return TextEditorSideBar.Instance; }
		}
	}
}
