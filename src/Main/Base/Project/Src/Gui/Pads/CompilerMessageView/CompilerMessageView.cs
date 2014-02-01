// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.WinForms;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This class displays the errors and warnings which the compiler outputs and
	/// allows the user to jump to the source of the warning / error
	/// </summary>
	public class CompilerMessageView : AbstractPadContent, IClipboardHandler, IOutputPad
	{
		#region IOutputPad implementation

		IOutputCategory IOutputPad.CreateCategory(string displayName)
		{
			var cat = new MessageViewCategory(displayName, displayName);
			AddCategory(cat);
			return cat;
		}

		void IOutputPad.RemoveCategory(IOutputCategory category)
		{
			throw new NotImplementedException();
		}

		IOutputCategory IOutputPad.CurrentCategory {
			get {
				return this.SelectedMessageViewCategory;
			}
			set {
				int index = messageCategories.IndexOf(value as MessageViewCategory);
				if (index >= 0)
					SelectedCategoryIndex = index;
			}
		}
		
		IOutputCategory IOutputPad.BuildCategory {
			get {
				return TaskService.BuildMessageViewCategory;
			}
		}

		#endregion

		static CompilerMessageView instance;
		
		/// <summary>
		/// Gets the instance of the CompilerMessageView. This property is thread-safe, but
		/// most instance methods of the CompilerMessageView aren't.
		/// </summary>
		public static CompilerMessageView Instance {
			get {
				if (instance == null)
					SD.MainThread.InvokeIfRequired(InitializeInstance);
				return instance;
			}
		}
		
		static void InitializeInstance()
		{
			SD.Workbench.GetPad(typeof(CompilerMessageView)).CreatePad();
		}
		
		#region MessageViewLinkElementGenerator
		class MessageViewLinkElementGenerator : LinkElementGenerator
		{
			public MessageViewLinkElementGenerator(Regex regex)
				: base(regex)
			{
				RequireControlModifierForClick = false;
			}
			
			protected override Uri GetUriFromMatch(Match match)
			{
				return new Uri(match.Groups[1].Value.Trim());
			}
			
			protected override VisualLineElement ConstructElementFromMatch(Match m)
			{
				Uri uri = GetUriFromMatch(m);
				if (uri == null)
					return null;
				var linkText = new VisualLineMessageViewLinkText(CurrentContext.VisualLine, m.Length);
				linkText.NavigateUri = uri;
				linkText.RequireControlModifierForClick = this.RequireControlModifierForClick;
				linkText.Line = int.Parse(m.Groups[2].Value);
				if (m.Groups.Count > 3)
					linkText.Column = int.Parse(m.Groups[3].Value);
				return linkText;
			}
			
			public static void RegisterGenerators(TextView textView)
			{
				// C#:
				textView.ElementGenerators.Add(new MessageViewLinkElementGenerator(
					new Regex(@"\b(\w:[/\\].*?)\((\d+),(\d+)\)")));
				// NUnit:
				textView.ElementGenerators.Add(new MessageViewLinkElementGenerator(
					new Regex(@"\b(\w:[/\\].*?):line\s(\d+)?$")));
				// C++:
				textView.ElementGenerators.Add(new MessageViewLinkElementGenerator(
					new Regex(@"\b(\w:[/\\].*?)\((\d+)\)")));
			}
		}
		
		class VisualLineMessageViewLinkText : VisualLineLinkText
		{
			/// <summary>
			/// Creates a visual line text element with the specified length.
			/// It uses the <see cref="ITextRunConstructionContext.VisualLine"/> and its
			/// <see cref="VisualLineElement.RelativeTextOffset"/> to find the actual text string.
			/// </summary>
			public VisualLineMessageViewLinkText(VisualLine parentVisualLine, int length) : base(parentVisualLine, length)
			{
				this.RequireControlModifierForClick = false;
			}
			
			public int Line { get; set; }
			public int Column { get; set; }
			
			protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
			{
				if (e.ChangedButton == MouseButton.Left && !e.Handled && LinkIsClickable() && NavigateUri.IsFile) {
					FileService.JumpToFilePosition(NavigateUri.LocalPath, Line, Column);
					e.Handled = true;
				}
			}
			
			protected override VisualLineText CreateInstance(int length)
			{
				return new VisualLineMessageViewLinkText(ParentVisualLine, length) {
					NavigateUri = this.NavigateUri,
					Line = this.Line,
					Column = this.Column,
					TargetName = this.TargetName,
					RequireControlModifierForClick = this.RequireControlModifierForClick
				};
			}
		}
		#endregion
		
		TextEditor textEditor = new TextEditor();
		DockPanel panel = new DockPanel();
		ToolBar toolStrip;
		
		List<MessageViewCategory> messageCategories = new List<MessageViewCategory>();
		
		int selectedCategory = 0;
		public int SelectedCategoryIndex {
			get {
				return selectedCategory;
			}
			set {
				SD.MainThread.VerifyAccess();
				if (selectedCategory != value) {
					selectedCategory = value;
					DisplayActiveCategory();
					OnSelectedCategoryIndexChanged(EventArgs.Empty);
				}
			}
		}
		
		void DisplayActiveCategory()
		{
			SD.MainThread.VerifyAccess();
			if (selectedCategory < 0) {
				textEditor.Text = "";
			} else {
				lock (messageCategories[selectedCategory].SyncRoot) {
					// accessing a categories' text takes its lock - but we have to take locks in the same
					// order as in the Append calls to prevent a deadlock
					EnqueueAppend(new AppendCall(messageCategories[selectedCategory], messageCategories[selectedCategory].Text, true));
				}
			}
		}
		
		public bool WordWrap {
			get {
				return properties.Get("WordWrap", true);
			}
			set {
				properties.Set("WordWrap", value);
			}
		}
		
		public MessageViewCategory SelectedMessageViewCategory {
			get {
				if (selectedCategory >= 0) {
					return messageCategories[selectedCategory];
				}
				return null;
			}
		}
		
		// The compiler message view properties.
		Properties properties  = null;
		
		public List<MessageViewCategory> MessageCategories {
			get {
				return messageCategories;
			}
		}
		
		public override object Control {
			get {
				return panel;
			}
		}
		
		public CompilerMessageView()
		{
			instance = this;
			
			AddCategory(TaskService.BuildMessageViewCategory);
			
			textEditor.IsReadOnly = true;
			
			textEditor.ContextMenu = MenuService.CreateContextMenu(this, "/SharpDevelop/Pads/CompilerMessageView/ContextMenu");
			
			properties = PropertyService.NestedProperties(OutputWindowOptionsPanel.OutputWindowsProperty);
			
			SetTextEditorFont();
			
			properties.PropertyChanged += new PropertyChangedEventHandler(PropertyChanged);
			
			MessageViewLinkElementGenerator.RegisterGenerators(textEditor.TextArea.TextView);
			textEditor.TextArea.TextView.ElementGenerators.OfType<LinkElementGenerator>().ForEach(x => x.RequireControlModifierForClick = false);
			
			toolStrip = ToolBarService.CreateToolBar(panel, this, "/SharpDevelop/Pads/CompilerMessageView/Toolbar");
			toolStrip.SetValue(DockPanel.DockProperty, Dock.Top);
			
			panel.Children.Add(toolStrip);
			panel.Children.Add(textEditor);
			
			SetWordWrap();
			DisplayActiveCategory();
			SD.ProjectService.CurrentSolutionChanged += OnSolutionLoaded;
			
			SearchPanel.Install(textEditor);
		}

		void OnSolutionLoaded(object sender, EventArgs e)
		{
			foreach (MessageViewCategory category in messageCategories) {
				category.ClearText();
			}
		}
		
		
		private bool IsFontChanged (string propName)
		{
			if ((propName == OutputWindowOptionsPanel.FontSizeName) || (propName == OutputWindowOptionsPanel.FontFamilyName)) {
				return true;
			}
			return false;
		}
		
		private void SetWordWrap()
		{
			bool wordWrap = this.WordWrap;
			textEditor.WordWrap = wordWrap;
		}
		
		
		private void SetTextEditorFont()
		{
			var fontDescription =  OutputWindowOptionsPanel.DefaultFontDescription();
			textEditor.FontFamily = new FontFamily(fontDescription.Item1);
			textEditor.FontSize = fontDescription.Item2;
		}
		
		#region Category handling
		/// <summary>
		/// Adds a category to the compiler message view. This method is thread-safe.
		/// </summary>
		public void AddCategory(MessageViewCategory category)
		{
			if (SD.MainThread.InvokeRequired) {
				SD.MainThread.InvokeAsyncAndForget(() => AddCategory(category));
				return;
			}
			messageCategories.Add(category);
			category.TextSet      += new TextEventHandler(CategoryTextSet);
			category.TextAppended += new TextEventHandler(CategoryTextAppended);
			
			OnMessageCategoryAdded(EventArgs.Empty);
		}
		
		void CategoryTextSet(object sender, TextEventArgs e)
		{
			EnqueueAppend(new AppendCall((MessageViewCategory)sender, e.Text, true));
		}
		
		struct AppendCall
		{
			internal readonly MessageViewCategory Category;
			internal readonly string Text;
			internal readonly bool ClearCategory;
			
			public AppendCall(MessageViewCategory category, string text, bool clearCategory)
			{
				this.Category = category;
				this.Text = text;
				this.ClearCategory = clearCategory;
			}
		}
		
		readonly object appendLock = new object();
		List<AppendCall> appendCalls = new List<AppendCall>();
		
		void CategoryTextAppended(object sender, TextEventArgs e)
		{
			EnqueueAppend(new AppendCall((MessageViewCategory)sender, e.Text, false));
		}
		
		void EnqueueAppend(AppendCall appendCall)
		{
			bool waitForMainThread;
			lock (appendLock) {
				appendCalls.Add(appendCall);
				if (appendCalls.Count == 1) {
					SD.MainThread.InvokeAsyncAndForget(ProcessAppendText);
				}
				waitForMainThread = appendCalls.Count > 2000;
			}
			if (waitForMainThread && SD.MainThread.InvokeRequired) {
				int sleepLength = 20;
				do {
					Thread.Sleep(sleepLength);
					sleepLength += 20;
					lock (appendLock)
						waitForMainThread = appendCalls.Count > 2000;
					//if (waitForMainThread) LoggingService.Debug("Extending sleep (" + sleepLength + ")");
				} while (waitForMainThread);
			}
		}
		
		void ProcessAppendText()
		{
			List<AppendCall> appendCalls;
			lock (appendLock) {
				appendCalls = this.appendCalls;
				this.appendCalls = new List<AppendCall>();
			}
			Debug.Assert(appendCalls.Count > 0);
			if (appendCalls.Count == 0)
				return;
			
			MessageViewCategory newCategory = appendCalls[appendCalls.Count - 1].Category;
			if (messageCategories[SelectedCategoryIndex] != newCategory) {
				SelectCategory(newCategory.Category);
				return;
			}
			
			bool clear;
			string text;
			if (appendCalls.Count == 1) {
				//LoggingService.Debug("CompilerMessageView: Single append.");
				clear = appendCalls[0].ClearCategory;
				text = appendCalls[0].Text;
			} else {
				if (LoggingService.IsDebugEnabled) {
					LoggingService.Debug("CompilerMessageView: Combined " + appendCalls.Count + " appends.");
				}
				
				clear = false;
				StringBuilder b = new StringBuilder();
				foreach (AppendCall append in appendCalls) {
					if (append.Category == newCategory) {
						if (append.ClearCategory) {
							b.Length = 0;
							clear = true;
						}
						b.Append(append.Text);
					}
				}
				text = b.ToString();
			}
			
			if (clear)
				textEditor.Text = text;
			else
				textEditor.AppendText(text);
			
			textEditor.ScrollToEnd();
		}
		
		public void SelectCategory(string categoryName)
		{
			for (int i = 0; i < messageCategories.Count; ++i) {
				MessageViewCategory category = (MessageViewCategory)messageCategories[i];
				if (category.Category == categoryName) {
					SelectedCategoryIndex = i;
					break;
				}
			}
		}
		
		void SelectCategory(string categoryName, string text)
		{
			for (int i = 0; i < messageCategories.Count; ++i) {
				MessageViewCategory category = (MessageViewCategory)messageCategories[i];
				if (category.Category == categoryName) {
					selectedCategory = i;
					textEditor.Text = StringParser.Parse(text);
					OnSelectedCategoryIndexChanged(EventArgs.Empty);
					break;
				}
			}
		}
		
		public MessageViewCategory GetCategory(string categoryName)
		{
			foreach (MessageViewCategory category in messageCategories) {
				if (category.Category == categoryName) {
					return category;
				}
			}
			return null;
		}
		#endregion
		
		/// <summary>
		/// Changes wordwrap settings if that property has changed.
		/// </summary>
		void PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == OutputWindowOptionsPanel.WordWrapName) {
				SetWordWrap();
				ToolBarService.UpdateStatus(toolStrip.Items);
			}
			if (IsFontChanged(e.PropertyName)) {
				SetTextEditorFont();
			}
		}
		
		
		protected virtual void OnMessageCategoryAdded(EventArgs e)
		{
			if (MessageCategoryAdded != null) {
				MessageCategoryAdded(this, e);
			}
		}
		
		protected virtual void OnSelectedCategoryIndexChanged(EventArgs e)
		{
			if (SelectedCategoryIndexChanged != null) {
				SelectedCategoryIndexChanged(this, e);
			}
		}
		
		public event EventHandler MessageCategoryAdded;
		public event EventHandler SelectedCategoryIndexChanged;
		
		#region ICSharpCode.SharpDevelop.Gui.IClipboardHandler interface implementation
		
		public bool EnableCut {
			get {
				return false;
			}
		}
		
		public bool EnableCopy {
			get {
				return textEditor.SelectionLength > 0;
			}
		}
		
		public bool EnablePaste {
			get {
				return false;
			}
		}
		
		public bool EnableDelete {
			get {
				return false;
			}
		}
		
		public bool EnableSelectAll {
			get {
				return textEditor.Document.TextLength > 0;
			}
		}
		
		public void Cut()
		{
		}
		
		public void Copy()
		{
			textEditor.Copy();
		}
		
		public void Paste()
		{
		}
		
		public void Delete()
		{
		}
		
		public void SelectAll()
		{
			textEditor.SelectAll();
		}
		#endregion
	}
}
