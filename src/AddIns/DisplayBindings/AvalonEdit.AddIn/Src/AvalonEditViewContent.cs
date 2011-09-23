// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.AddIn.Options;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public interface ICodeEditorProvider : ITextEditorProvider
	{
		CodeEditor CodeEditor { get; }
	}
	
	public class AvalonEditViewContent
		: AbstractViewContent, IEditable, IMementoCapable, ICodeEditorProvider, IPositionable
	{
		readonly CodeEditor codeEditor = new CodeEditor();
		IAnalyticsMonitorTrackedFeature trackedFeature;
		
		public AvalonEditViewContent(OpenedFile file, Encoding fixedEncodingForLoading = null)
		{
			if (fixedEncodingForLoading != null) {
				codeEditor.UseFixedEncoding = true;
				codeEditor.PrimaryTextEditor.Encoding = fixedEncodingForLoading;
			}
			this.TabPageText = "${res:FormsDesigner.DesignTabPages.SourceTabPage}";
			
			if (file.FileName != null) {
				string filetype = Path.GetExtension(file.FileName);
				if (!ProjectService.GetFileFilters().Any(f => f.ContainsExtension(filetype)))
					filetype = ".?";
				trackedFeature = AnalyticsMonitorService.TrackFeature(typeof(AvalonEditViewContent), "open" + filetype.ToLowerInvariant());
			}
			
			this.Files.Add(file);
			file.ForceInitializeView(this);
			
			file.IsDirtyChanged += PrimaryFile_IsDirtyChanged;
			codeEditor.Document.UndoStack.PropertyChanged += codeEditor_Document_UndoStack_PropertyChanged;
			codeEditor.CaretPositionChanged += CaretChanged;
			codeEditor.TextCopied += codeEditor_TextCopied;
		}
		
		void codeEditor_Document_UndoStack_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (!isLoading)
				PrimaryFile.IsDirty = !codeEditor.Document.UndoStack.IsOriginalFile;
		}
		
		void PrimaryFile_IsDirtyChanged(object sender, EventArgs e)
		{
			var document = codeEditor.Document;
			if (document != null) {
				var undoStack = document.UndoStack;
				if (this.PrimaryFile.IsDirty) {
					if (undoStack.IsOriginalFile)
						undoStack.DiscardOriginalFileMarker();
				} else {
					undoStack.MarkAsOriginalFile();
				}
			}
		}
		
		void codeEditor_TextCopied(object sender, ICSharpCode.AvalonEdit.Editing.TextEventArgs e)
		{
			TextEditorSideBar.Instance.PutInClipboardRing(e.Text);
		}
		
		public CodeEditor CodeEditor {
			get { return codeEditor; }
		}
		
		public override object Control {
			get { return codeEditor; }
		}
		
		public override object InitiallyFocusedControl {
			get { return codeEditor.PrimaryTextEditor.TextArea; }
		}
		
		public override void Save(OpenedFile file, Stream stream)
		{
			if (file != PrimaryFile)
				return;
			
			if (codeEditor.CanSaveWithCurrentEncoding()) {
				codeEditor.Save(stream);
			} else {
				int r = MessageService.ShowCustomDialog(
					"${res:Dialog.Options.IDEOptions.TextEditor.General.FontGroupBox.FileEncodingGroupBox}",
					StringParser.Parse("${res:AvalonEdit.FileEncoding.EncodingCausesDataLoss}",
					                   new StringTagPair("encoding", codeEditor.Encoding.EncodingName)),
					0, -1,
					"${res:AvalonEdit.FileEncoding.EncodingCausesDataLoss.UseUTF8}",
					"${res:AvalonEdit.FileEncoding.EncodingCausesDataLoss.Continue}");
				if (r == 1) {
					// continue saving with data loss
					MemoryStream ms = new MemoryStream();
					codeEditor.Save(ms);
					ms.Position = 0;
					ms.WriteTo(stream);
					ms.Position = 0;
					// Read back the version we just saved to show the data loss to the user (he'll be able to press Undo).
					using (StreamReader reader = new StreamReader(ms, codeEditor.Encoding, false)) {
						codeEditor.Document.Text = reader.ReadToEnd();
					}
					return;
				} else {
					// unfortunately we don't support cancel within IViewContent.Save, so we'll use the safe choice of UTF-8 instead
					codeEditor.Encoding = System.Text.Encoding.UTF8;
					codeEditor.Save(stream);
				}
			}
		}
		
		bool isLoading;
		
		public override void Load(OpenedFile file, Stream stream)
		{
			if (file != PrimaryFile)
				return;
			isLoading = true;
			try {
				BookmarksDetach();
				codeEditor.SyntaxHighlighting =
					HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(file.FileName));
				
				if (!file.IsUntitled) {
					codeEditor.PrimaryTextEditor.IsReadOnly = (File.GetAttributes(file.FileName) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
				}
				
				codeEditor.Load(stream);
				// Load() causes the undo stack to think stuff changed, so re-mark the file as original if necessary
				if (!this.PrimaryFile.IsDirty) {
					codeEditor.Document.UndoStack.MarkAsOriginalFile();
				}
				
				// we set the file name after loading because this will place the fold markers etc.
				codeEditor.FileName = FileName.Create(file.FileName);
				BookmarksAttach();
			} finally {
				isLoading = false;
			}
		}
		
		protected override void OnFileNameChanged(OpenedFile file)
		{
			base.OnFileNameChanged(file);
			if (file == PrimaryFile) {
				FileName oldFileName = codeEditor.FileName;
				FileName newFileName = FileName.Create(file.FileName);
				
				if (!string.IsNullOrEmpty(oldFileName))
					ParserService.ClearParseInformation(oldFileName);
				
				
				BookmarksNotifyNameChange(oldFileName, newFileName);
				// changing the filename on the codeEditor raises several events; ensure
				// we got our state updated first (bookmarks, persistent anchors) before other code
				// processes the file name change
				
				codeEditor.FileName = newFileName;
				
				ParserService.BeginParse(file.FileName, codeEditor.DocumentAdapter);
			}
		}
		
		public override INavigationPoint BuildNavPoint()
		{
			int lineNumber = this.Line;
			string txt = codeEditor.Document.GetText(codeEditor.Document.GetLineByNumber(lineNumber));
			return new TextNavigationPoint(this.PrimaryFileName, lineNumber, this.Column, txt);
		}
		
		void CaretChanged(object sender, EventArgs e)
		{
			NavigationService.Log(this.BuildNavPoint());
			var document = codeEditor.Document;
			int lineOffset = document.GetLineByNumber(this.Line).Offset;
			int chOffset = this.Column;
			int col = 1;
			for (int i = 1; i < chOffset; i++) {
				if (document.GetCharAt(lineOffset + i - 1) == '\t')
					col += CodeEditorOptions.Instance.IndentationSize;
				else
					col += 1;
			}
			WorkbenchSingleton.StatusBar.SetCaretPosition(col, this.Line, chOffset);
		}
		
		public override bool IsReadOnly {
			get { return codeEditor.PrimaryTextEditor.IsReadOnly; }
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
			
			PermanentAnchorService.AttachDocument(codeEditor.FileName, codeEditor.DocumentAdapter);
		}

		void BookmarksDetach()
		{
			if (codeEditor.FileName != null) {
				PermanentAnchorService.DetachDocument(codeEditor.FileName, codeEditor.DocumentAdapter);
			}
			
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
		
		void BookmarksNotifyNameChange(FileName oldFileName, FileName newFileName)
		{
			PermanentAnchorService.RenameDocument(oldFileName, newFileName, codeEditor.DocumentAdapter);
			
			foreach (SDBookmark bookmark in codeEditor.IconBarManager.Bookmarks.OfType<SDBookmark>()) {
				bookmark.FileName = newFileName;
			}
		}
		#endregion
		
		public override void Dispose()
		{
			if (trackedFeature != null)
				trackedFeature.EndTracking();
			this.PrimaryFile.IsDirtyChanged -= PrimaryFile_IsDirtyChanged;
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
	}
}
