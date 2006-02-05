// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

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
					WorkbenchSingleton.SafeThreadCall((MethodInvoker)InitializeInstance);
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
				if (selectedCategory != value) {
					selectedCategory = value;
					textEditorControl.Text = (value < 0) ? "" : messageCategories[value].Text;
					//textEditorControl.Refresh();
					OnSelectedCategoryIndexChanged(EventArgs.Empty);
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
			/*textEditorControl.ShowLineNumbers   = false;
			textEditorControl.ShowInvalidLines  = false;
			textEditorControl.EnableFolding     = false;
			textEditorControl.IsIconBarVisible  = false;
			textEditorControl.Document.ReadOnly = true;
			textEditorControl.ShowHRuler        = false;
			textEditorControl.ShowVRuler        = false;
			textEditorControl.ShowSpaces        = false;
			textEditorControl.ShowTabs          = false;
			textEditorControl.ShowEOLMarkers    = false;*/
			textEditorControl.ReadOnly = true;
			
			textEditorControl.ContextMenuStrip = MenuService.CreateContextMenu(this, "/SharpDevelop/Pads/CompilerMessageView/ContextMenu");
			
			properties = (Properties)PropertyService.Get(OutputWindowOptionsPanel.OutputWindowsProperty, new Properties());
			
			textEditorControl.Font = FontSelectionPanel.ParseFont(properties.Get("DefaultFont", ResourceService.CourierNew10.ToString()).ToString());
			properties.PropertyChanged += new PropertyChangedEventHandler(PropertyChanged);
			
			//textEditorControl.ActiveTextAreaControl.TextArea.DoubleClick += TextEditorControlDoubleClick;
			textEditorControl.DoubleClick += TextEditorControlDoubleClick;
			
			toolStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/CompilerMessageView/Toolbar");
			toolStrip.Stretch   = true;
			toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			
			myPanel.Controls.AddRange(new Control[] { textEditorControl, toolStrip} );
			
			SetWordWrap();
			myPanel.ResumeLayout(false);
			SetText(messageCategories[selectedCategory], messageCategories[selectedCategory].Text);
		}
		
		void SetWordWrap()
		{
			bool wordWrap = this.WordWrap;
			textEditorControl.WordWrap = wordWrap;
			if (wordWrap) {
				textEditorControl.ScrollBars = RichTextBoxScrollBars.ForcedBoth;
			} else {
				textEditorControl.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
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
			category.Cleared      += new EventHandler(CategoryTextCleared);
			category.TextSet      += new TextEventHandler(CategoryTextSet);
			category.TextAppended += new TextEventHandler(CategoryTextAppended);
			
			OnMessageCategoryAdded(EventArgs.Empty);
		}
		
		void CategoryTextCleared(object sender, EventArgs e)
		{
			WorkbenchSingleton.SafeThreadAsyncCall(this, "ClearText", sender);
		}
		void ClearText(MessageViewCategory category)
		{
			if (messageCategories[SelectedCategoryIndex] == category) {
				textEditorControl.Text = String.Empty;
				//textEditorControl.Refresh();
			}
		}
		
		void CategoryTextSet(object sender, TextEventArgs e)
		{
			WorkbenchSingleton.SafeThreadAsyncCall(this, "SetText", (MessageViewCategory)sender, e.Text);
		}
		
		object appendCallLock = new object();
		volatile int pendingAppendCalls = 0;
		
		void CategoryTextAppended(object sender, TextEventArgs e)
		{
			lock (appendCallLock) {
				pendingAppendCalls += 1;
				if (pendingAppendCalls < 5) {
					WorkbenchSingleton.SafeThreadAsyncCall(this, "AppendText", sender, ((MessageViewCategory)sender).Text, e.Text);
				} else if (pendingAppendCalls == 5) {
					WorkbenchSingleton.SafeThreadAsyncCall(this, "AppendTextCombined", sender);
				}
			}
		}
		
		const int WM_SETREDRAW = 0x00B;
		
		[System.Security.SuppressUnmanagedCodeSecurityAttribute]
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
		
		void SetUpdate(bool update)
		{
			SendMessage(textEditorControl.Handle, WM_SETREDRAW, update ? new IntPtr(1) : IntPtr.Zero, IntPtr.Zero);
		}
		
		void AppendTextCombined(MessageViewCategory category)
		{
			Application.DoEvents();
			Thread.Sleep(50);
			Application.DoEvents();
			lock (appendCallLock) {
				SetUpdate(false);
				SetText(category, category.Text);
				SetUpdate(true);
				textEditorControl.SelectionStart = textEditorControl.TextLength;
				LoggingService.Debug("Replaced " + pendingAppendCalls + " appends with one set call");
				pendingAppendCalls = 0;
			}
		}
		
		void AppendText(MessageViewCategory category, string fullText, string text)
		{
			lock (appendCallLock) {
				if (pendingAppendCalls >= 5) {
					return;
				}
				pendingAppendCalls -= 1;
			}
			if (messageCategories[SelectedCategoryIndex] != category) {
				SelectCategory(category.Category, fullText);
				return;
			}
			if (text != null) {
				text = StringParser.Parse(text);
				textEditorControl.AppendText(text);
				textEditorControl.SelectionStart = textEditorControl.TextLength;
				/*textEditorControl.Document.ReadOnly = false;
				textEditorControl.Document.Insert(textEditorControl.Document.TextLength, text);
				textEditorControl.Document.ReadOnly = true;
				textEditorControl.ActiveTextAreaControl.Caret.Position = new Point(0, textEditorControl.Document.TotalNumberOfLines);
				textEditorControl.ActiveTextAreaControl.ScrollTo(textEditorControl.Document.TotalNumberOfLines);*/
			}
		}
		
		void SetText(MessageViewCategory category, string text)
		{
			if (messageCategories[SelectedCategoryIndex] != category) {
				SelectCategory(category.Category);
				return;
			}
			if (text == null) {
				text = String.Empty;
			} else {
				text = StringParser.Parse(text);
			}
			textEditorControl.Text = text;
			//textEditorControl.Refresh();
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
					textEditorControl.Text = text;
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
					FileService.JumpToFilePosition(Path.GetFullPath(lineReference.FileName), lineReference.Line, lineReference.Column);
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
				textEditorControl.Font = FontSelectionPanel.ParseFont(properties.Get("DefaultFont", ResourceService.CourierNew10.ToString()).ToString());
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
