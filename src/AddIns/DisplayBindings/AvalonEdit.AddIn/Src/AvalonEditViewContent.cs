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

namespace ICSharpCode.AvalonEdit.AddIn
{
	public class AvalonEditViewContent : AbstractViewContent, IEditable, IMementoCapable, ITextEditorProvider, IPositionable, IParseInformationListener
	{
		readonly CodeEditor codeEditor = new CodeEditor();
		
		public AvalonEditViewContent(OpenedFile file)
		{
			this.TabPageText = "${res:FormsDesigner.DesignTabPages.SourceTabPage}";
			
			this.Files.Add(file);
			file.ForceInitializeView(this);
			codeEditor.Document.Changed += textEditor_Document_Changed;
			codeEditor.CaretPositionChanged += CaretChanged;
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
				codeEditor.FileName = file.FileName;
				codeEditor.PrimaryTextEditor.SyntaxHighlighting =
					HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(file.FileName));
				
				codeEditor.Load(stream);
				BookmarksAttach();
			} finally {
				isLoading = false;
			}
		}
		
		protected override void OnFileNameChanged(OpenedFile file)
		{
			base.OnFileNameChanged(file);
			if (file == PrimaryFile) {
				codeEditor.FileName = file.FileName;
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
			codeEditor.DisposeLanguageBinding();
			// Unload document on dispose.
			codeEditor.Document = null;
		}
		
		public override string ToString()
		{
			return "[" + GetType().Name + " " + this.PrimaryFileName + "]";
		}
		
		#region IEditable
		/// <summary>
		/// Thread-safe snapshot creation.
		/// </summary>
		public ITextBuffer CreateSnapshot()
		{
			return new ICSharpCode.SharpDevelop.Editor.AvalonEdit.AvalonEditTextSourceAdapter(codeEditor.Document.CreateSnapshot());
		}
		
		/// <summary>
		/// Thread-safe text getter.
		/// </summary>
		public string Text {
			get {
				return WorkbenchSingleton.SafeThreadFunction(() => codeEditor.Document.Text);
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
			codeEditor.JumpTo(line, column);
		}
		#endregion
		
		#region IParseInformationListener
		public void ParseInformationUpdated(ParseInformation parseInfo)
		{
			WorkbenchSingleton.SafeThreadAsyncCall(codeEditor.ParseInformationUpdated, parseInfo);
		}
		#endregion
	}
}
