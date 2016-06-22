// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;

using ICSharpCode.AvalonEdit;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpSnippetCompiler.Core
{
	public class MainViewContent : IViewContent, ITextEditorProvider, IPositionable
	{
		IWorkbenchWindow workbenchWindow;
		TextEditor textEditor;
		SharpSnippetTextEditorAdapter adapter;
		SnippetFile file;
		IconBarManager iconBarManager;
		
		public MainViewContent(string fileName, IWorkbenchWindow workbenchWindow)
		{
			this.textEditor = new TextEditor();
			this.textEditor.FontFamily = new FontFamily("Consolas");
			this.adapter = new SharpSnippetTextEditorAdapter(textEditor);
			this.workbenchWindow = workbenchWindow;
			textEditor.TextArea.TextView.Services.AddService(typeof(ITextEditor), adapter);
			this.LoadFile(fileName);
			
			iconBarManager = new IconBarManager();
			this.textEditor.TextArea.LeftMargins.Insert(0, new IconBarMargin(iconBarManager));
			
			var textMarkerService = new TextMarkerService(textEditor.Document);
			textEditor.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);
			textEditor.TextArea.TextView.LineTransformers.Add(textMarkerService);
			textEditor.TextArea.TextView.Services.AddService(typeof(ITextMarkerService), textMarkerService);
			textEditor.TextArea.TextView.Services.AddService(typeof(IBookmarkMargin), iconBarManager);
			
			BookmarkManager.Added += BookmarkManager_Added;
			BookmarkManager.Removed += BookmarkManager_Removed;
		}
		
		public event EventHandler TabPageTextChanged;
		public event EventHandler TitleNameChanged;
		public event EventHandler Disposed;
		public event EventHandler IsDirtyChanged;
//		
//		public bool EnableUndo {
//			get { return textEditor.EnableUndo; }
//		}
//		
//		public bool EnableRedo {
//			get { return textEditor.EnableRedo; }
//		}
//		
//		public void Undo()
//		{
//			textEditor.Undo();
//		}
//		
//		public void Redo()
//		{
//			textEditor.Redo();
//		}
//		
//		public bool EnableCut {
//			get { return textEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableCut; }
//		}
//		
//		public bool EnableCopy {
//			get { return textEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableCopy; }
//		}
//		
//		public bool EnablePaste {
//			get { return textEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.EnablePaste; }
//		}
//		
//		public bool EnableDelete {
//			get { return textEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableDelete; }
//		}
//		
//		public bool EnableSelectAll {
//			get { return textEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableSelectAll; }
//		}
//		
//		public void Cut()
//		{
//			textEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.Cut(null, null);
//		}
//		
//		public void Copy()
//		{
//			textEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.Copy(null, null);
//		}
//		
//		public void Paste()
//		{
//			textEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.Paste(null, null);
//		}
//		
//		public void Delete()
//		{
//			textEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.Delete(null, null);
//		}
//		
//		public void SelectAll()
//		{
//			textEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.SelectAll(null, null);
//		}
		
		public Control Control {
			get { return textEditor; }
		}
		
		public IWorkbenchWindow WorkbenchWindow {
			get { return workbenchWindow; }
			set { workbenchWindow = value; }
		}
		
		public string TabPageText {
			get { return Path.GetFileName(file.FileName); }
		}
		
		public string TitleName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IList<OpenedFile> Files {
			get {
				throw new NotImplementedException();
			}
		}
		
		public OpenedFile PrimaryFile {
			get { return file; }
		}
		
		public FileName PrimaryFileName {
			get { return file.FileName; }
		}
		
		public bool IsDisposed {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsReadOnly {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsViewOnly {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ICollection<IViewContent> SecondaryViewContents {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsDirty {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void Save(OpenedFile file, Stream stream)
		{
			throw new NotImplementedException();
		}
		
		public void Load(OpenedFile file, Stream stream)
		{
			throw new NotImplementedException();
		}
		
		public INavigationPoint BuildNavPoint()
		{
			throw new NotImplementedException();
		}
		
		public bool SupportsSwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
		{
			throw new NotImplementedException();
		}
		
		public bool SupportsSwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView)
		{
			throw new NotImplementedException();
		}
		
		public void SwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
		{
			throw new NotImplementedException();
		}
		
		public void SwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView)
		{
			throw new NotImplementedException();
		}
		
		public void Dispose()
		{
			BookmarkManager.Added -= BookmarkManager_Added;
			BookmarkManager.Removed -= BookmarkManager_Removed;
		}
		
		public IDocument GetDocumentForFile(OpenedFile file)
		{
			return null;
		}
		
		public void JumpTo(int line, int column)
		{
			adapter.JumpTo(line, column);
		}
		
		public int Line {
			get { return textEditor.TextArea.Caret.Line; }
		}
		
		public int Column {
			get { return textEditor.TextArea.Caret.Column; }
		}
		
//		public void ParseInformationUpdated(ParseInformation parseInfo)
//		{
//			if (textEditor.TextEditorProperties.EnableFolding) {
//				WorkbenchSingleton.SafeThreadAsyncCall(ParseInformationUpdatedInvoked, parseInfo);
//			}
//		}
		
		public delegate string GetTextHelper();
		
		public string Text {
			get {
				if (WorkbenchSingleton.InvokeRequired) {
					return (string)WorkbenchSingleton.SafeThreadFunction(GetText);
				} else {
					return GetText();
				}
			}
			set {
				if (WorkbenchSingleton.InvokeRequired) {
					WorkbenchSingleton.SafeThreadCall(SetText, value);
				} else {
					SetText(value);
				}
			}
		}
		
		protected virtual void OnTabPageTextChanged(EventArgs e)
		{
			if (TabPageTextChanged != null) {
				TabPageTextChanged(this, e);
			}
		}
		
		protected virtual void OnTitleNameChanged(EventArgs e)
		{
			if (TitleNameChanged != null) {
				TitleNameChanged(this, e);
			}
		}
		
		protected virtual void OnDisposed(EventArgs e)
		{
			if (Disposed != null) {
				Disposed(this, e);
			}
		}
		
		protected virtual void OnIsDirtyChanged(EventArgs e)
		{
			if (IsDirtyChanged != null) {
				IsDirtyChanged(this, e);
			}
		}
//		
//		void ParseInformationUpdatedInvoked(ParseInformation parseInfo)
//		{
//			try {
//				textEditor.Document.FoldingManager.UpdateFoldings(file.FileName, parseInfo);
//				textEditor.ActiveTextAreaControl.TextArea.Refresh(textEditor.ActiveTextAreaControl.TextArea.FoldMargin);
//				textEditor.ActiveTextAreaControl.TextArea.Refresh(textEditor.ActiveTextAreaControl.TextArea.IconBarMargin);
//			} catch (Exception ex) {
//				MessageService.ShowError(ex);
//			}
//		}
		
		string GetText()
		{
			return textEditor.Document.Text;
		}
		
		void SetText(string value)
		{
			textEditor.Document.Replace(0, textEditor.Document.TextLength, value);
		}
		
		public ITextEditor TextEditor {
			get { return adapter; }
		}
		
		public event EventHandler InfoTipChanged;
		
		protected virtual void OnInfoTipChanged(EventArgs e)
		{
			if (InfoTipChanged != null) {
				InfoTipChanged(this, e);
			}
		}
		
		object IViewContent.Control {
			get { return this.TextEditor; }
		}
		
		public object InitiallyFocusedControl {
			get { return this.TextEditor; }
		}
		
		public string InfoTip {
			get { return String.Empty; }
		}
		
		public bool CloseWithSolution {
			get { return false; }
		}
		
		public object GetService(Type serviceType)
		{
			return null;
		}
		
		public void LoadFile(string fileName)
		{
			this.file = new SnippetFile(fileName);
			adapter.LoadFile(fileName);
		}
		
		public void Save()
		{
			textEditor.Save(file.FileName);
		}
		
		void BookmarkManager_Removed(object sender, BookmarkEventArgs e)
		{
			iconBarManager.Bookmarks.Remove(e.Bookmark);
			if (e.Bookmark.Document == adapter.Document) {
				e.Bookmark.Document = null;
			}
		}
		
		void BookmarkManager_Added(object sender, BookmarkEventArgs e)
		{
			if (FileUtility.IsEqualFileName(this.PrimaryFileName, e.Bookmark.FileName)) {
				iconBarManager.Bookmarks.Add(e.Bookmark);
				e.Bookmark.Document = adapter.Document;
			}
		}
	}
}
