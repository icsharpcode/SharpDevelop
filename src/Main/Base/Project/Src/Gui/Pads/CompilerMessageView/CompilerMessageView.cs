// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This class displays the errors and warnings which the compiler outputs and
	/// allows the user to jump to the source of the warning / error
	/// </summary>
	public class CompilerMessageView : AbstractPadContent, IClipboardHandler
	{
		static CompilerMessageView instance;
		
		/// <summary>
		/// Gets the instance of the CompilerMessageView. This property is thread-safe, but
		/// most instance methods of the CompilerMessageView aren't.
		/// </summary>
		public static CompilerMessageView Instance {
			get {
				if (instance == null)
					WorkbenchSingleton.SafeThreadCall(InitializeInstance);
				return instance;
			}
		}
		
		static void InitializeInstance()
		{
			WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).CreatePad();
		}
		
		//TextEditorControl textEditorControl = new TextEditorControl();
		RichTextBox textEditorControl = new RichTextBox();
		Panel myPanel = new Panel();
		ToolStrip toolStrip;
		
		List<MessageViewCategory> messageCategories = new List<MessageViewCategory>();
		
		int selectedCategory = 0;
		public int SelectedCategoryIndex {
			get {
				return selectedCategory;
			}
			set {
				WorkbenchSingleton.AssertMainThread();
				if (selectedCategory != value) {
					selectedCategory = value;
					DisplayActiveCategory();
					OnSelectedCategoryIndexChanged(EventArgs.Empty);
				}
			}
		}
		
		void DisplayActiveCategory()
		{
			WorkbenchSingleton.DebugAssertMainThread();
			if (selectedCategory < 0) {
				textEditorControl.Text = "";
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
		
		public override Control Control {
			get {
				return myPanel;
			}
		}
		
		public CompilerMessageView()
		{
			instance = this;
			
			AddCategory(TaskService.BuildMessageViewCategory);
			
			myPanel.SuspendLayout();
			textEditorControl.Dock = DockStyle.Fill;
			textEditorControl.BorderStyle = BorderStyle.FixedSingle;
			textEditorControl.BackColor = SystemColors.Window;
			textEditorControl.LinkClicked += delegate(object sender, LinkClickedEventArgs e) {
				FileService.OpenFile("browser://" + e.LinkText);
			};
			
			// auto-scrolling on RichTextBox only works when HideSelection=false.
			// See comments to http://weblogs.asp.net/jdanforth/archive/2004/01/23/62026.aspx
			textEditorControl.HideSelection = false;
			
			textEditorControl.ReadOnly = true;
			
			textEditorControl.ContextMenuStrip = MenuService.CreateContextMenu(this, "/SharpDevelop/Pads/CompilerMessageView/ContextMenu");
			
			properties = (Properties)PropertyService.Get(OutputWindowOptionsPanel.OutputWindowsProperty, new Properties());
			
			textEditorControl.Font = FontSelectionPanel.ParseFont(properties.Get("DefaultFont", WinFormsResourceService.DefaultMonospacedFont.ToString()).ToString());
			properties.PropertyChanged += new PropertyChangedEventHandler(PropertyChanged);
			
			//textEditorControl.ActiveTextAreaControl.TextArea.DoubleClick += TextEditorControlDoubleClick;
			textEditorControl.DoubleClick += TextEditorControlDoubleClick;
			
			toolStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/CompilerMessageView/Toolbar");
			toolStrip.Stretch   = true;
			toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			
			myPanel.Controls.AddRange(new Control[] { textEditorControl, toolStrip} );
			
			SetWordWrap();
			myPanel.ResumeLayout(false);
			DisplayActiveCategory();
			ProjectService.SolutionLoaded += SolutionLoaded;
		}

		void SolutionLoaded(object sender, SolutionEventArgs e)
		{
			foreach (MessageViewCategory category in messageCategories) {
				category.ClearText();
			}
		}
		
		void SetWordWrap()
		{
			bool wordWrap = this.WordWrap;
			textEditorControl.WordWrap = wordWrap;
			if (wordWrap) {
				textEditorControl.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
			} else {
				textEditorControl.ScrollBars = RichTextBoxScrollBars.ForcedBoth;
			}
		}
		
		public override void RedrawContent()
		{
//			messageCategory.Items.Clear();
//			foreach (MessageViewCategory category in messageCategories) {
//				messageCategory.Items.Add(StringParser.Parse(category.DisplayCategory));
//			}
		}
		
		#region Category handling
		/// <summary>
		/// Adds a category to the compiler message view. This method is thread-safe.
		/// </summary>
		public void AddCategory(MessageViewCategory category)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				WorkbenchSingleton.SafeThreadAsyncCall((Action<MessageViewCategory>)AddCategory, category);
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
				if (appendCalls.Count == 1)
					WorkbenchSingleton.SafeThreadAsyncCall(ProcessAppendText);
				waitForMainThread = appendCalls.Count > 2000;
			}
			if (waitForMainThread && WorkbenchSingleton.InvokeRequired) {
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
			
			//NativeMethods.SetWindowRedraw(textEditorControl.Handle, false);
			if (clear) {
				textEditorControl.Text = text;
			} else {
				textEditorControl.SelectionStart = textEditorControl.TextLength;
				textEditorControl.SelectedText = text;
			}
			//NativeMethods.SetWindowRedraw(textEditorControl.Handle, true);
			textEditorControl.SelectionStart = textEditorControl.TextLength;
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
			if (!this.IsVisible) {
				ActivateThisPad();
			}
		}
		
		void SelectCategory(string categoryName, string text)
		{
			for (int i = 0; i < messageCategories.Count; ++i) {
				MessageViewCategory category = (MessageViewCategory)messageCategories[i];
				if (category.Category == categoryName) {
					selectedCategory = i;
					textEditorControl.Text = StringParser.Parse(text);
					//textEditorControl.Refresh();
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
		/// Makes this pad visible (usually BEFORE build or debug events)
		/// </summary>
		void ActivateThisPad()
		{
			WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(this.GetType().FullName);
		}
		
		/// <summary>
		/// Occurs when the mouse pointer is over the control and a
		/// mouse button is pressed.
		/// </summary>
		void TextEditorControlDoubleClick(object sender, EventArgs e)
		{
			string fullText = textEditorControl.Text;
			// Any text?
			if (fullText.Length > 0) {
				//int line = textEditorControl.ActiveTextAreaControl.Caret.Line;
				//string textLine = TextUtilities.GetLineAsString(textEditorControl.Document, line);
				Point clickPos = textEditorControl.PointToClient(Control.MousePosition);
				int index = textEditorControl.GetCharIndexFromPosition(clickPos);
				int start = index;
				// find start of current line
				while (--start > 0 && fullText[start - 1] != '\n');
				// find end of current line
				while (++index < fullText.Length && fullText[index] != '\n');
				
				string textLine = fullText.Substring(start, index - start);
				
				FileLineReference lineReference = OutputTextLineParser.GetFileLineReference(textLine);
				if (lineReference != null) {
					// Open matching file.
					FileService.JumpToFilePosition(lineReference.FileName, lineReference.Line, lineReference.Column);
				}
			}
		}
		
		/// <summary>
		/// Changes wordwrap settings if that property has changed.
		/// </summary>
		void PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.Key == "WordWrap") {
				SetWordWrap();
				ToolbarService.UpdateToolbar(toolStrip);
			}
			if (e.Key == "DefaultFont") {
				textEditorControl.Font = FontSelectionPanel.ParseFont(properties.Get("DefaultFont", WinFormsResourceService.DefaultMonospacedFont.ToString()).ToString());
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
				//return textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableCopy;
				return textEditorControl.SelectionLength > 0;
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
				//return textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableSelectAll;
				return textEditorControl.TextLength > 0;
			}
		}
		
		public void Cut()
		{
		}
		
		public void Copy()
		{
			//textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Copy(null, null);
			textEditorControl.Copy();
		}
		
		public void Paste()
		{
		}
		
		public void Delete()
		{
		}
		
		public void SelectAll()
		{
			//textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.SelectAll(null, null);
			textEditorControl.SelectAll();
		}
		#endregion
	}
}
