// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
		
		public static CompilerMessageView Instance {
			get {
				return instance;
			}
		}
		
		TextEditorControl textEditorControl = new TextEditorControl();
		Panel             myPanel           = new Panel();
		
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
					textEditorControl.Refresh();
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
		Properties 	properties	      = null;
		
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
			textEditorControl.Dock              = DockStyle.Fill;
			textEditorControl.ShowLineNumbers   = false;
			textEditorControl.ShowInvalidLines  = false;
			textEditorControl.EnableFolding     = false;
			textEditorControl.IsIconBarVisible  = false;
			textEditorControl.Document.ReadOnly = true;
			textEditorControl.ShowHRuler        = false;
			textEditorControl.ShowVRuler        = false;
			textEditorControl.ShowSpaces        = false;
			textEditorControl.ShowTabs          = false;
			textEditorControl.ShowEOLMarkers    = false;
			
			textEditorControl.ContextMenuStrip = MenuService.CreateContextMenu(this, "/SharpDevelop/Pads/CompilerMessageView/ContextMenu");
			
			properties = (Properties)PropertyService.Get(OutputWindowOptionsPanel.OutputWindowsProperty, new Properties());
			
			textEditorControl.Font     = FontSelectionPanel.ParseFont(properties.Get("DefaultFont", new Font("Courier New", 10).ToString()).ToString());
			properties.PropertyChanged += new PropertyChangedEventHandler(PropertyChanged);
			
			textEditorControl.ActiveTextAreaControl.TextArea.DoubleClick += TextEditorControlDoubleClick;
			
			ToolStrip toolStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/CompilerMessageView/Toolbar");
			toolStrip.Stretch   = true;
			toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			
			myPanel.Controls.AddRange(new Control[] { textEditorControl, toolStrip} );
			
			SetWordWrap();
			myPanel.ResumeLayout(false);
			SetText(messageCategories[selectedCategory]);
		}
		
		void SetWordWrap()
		{
//			bool wordWrap = properties.Get("WordWrap", true);
//			textEditorControl.WordWrap = wordWrap;
//			if (wordWrap) {
//				textEditorControl.ScrollBars = RichTextBoxScrollBars.ForcedBoth;
//			} else {
//				textEditorControl.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
//			}
		}
		
		public override void RedrawContent()
		{
//			messageCategory.Items.Clear();
//			foreach (MessageViewCategory category in messageCategories) {
//				messageCategory.Items.Add(StringParser.Parse(category.DisplayCategory));
//			}
		}
		
		#region Category handling
		public void AddCategory(MessageViewCategory category)
		{
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
			textEditorControl.Text = "";
			textEditorControl.Refresh();
			SelectCategory(category.Category);
		}
		
		void CategoryTextSet(object sender, TextEventArgs e)
		{
			WorkbenchSingleton.SafeThreadAsyncCall(this, "SetText", (MessageViewCategory)sender);
		}
		
		void CategoryTextAppended(object sender, TextEventArgs e)
		{
			WorkbenchSingleton.SafeThreadAsyncCall(this, "AppendText", (MessageViewCategory)sender, e.Text);
		}
		
		void AppendText(MessageViewCategory category, string text)
		{
			if (messageCategories[SelectedCategoryIndex] != category) {
				SetText(category);
				return;
			}
			if (text != null) {
				text = StringParser.Parse(text);
				textEditorControl.Document.ReadOnly = false;
				textEditorControl.Document.Insert(textEditorControl.Document.TextLength, text);
				textEditorControl.Document.ReadOnly = true;
				textEditorControl.ActiveTextAreaControl.Caret.Position = new Point(0, textEditorControl.Document.TotalNumberOfLines);
				textEditorControl.ActiveTextAreaControl.ScrollTo(textEditorControl.Document.TotalNumberOfLines);
			}
		}
		
		void SetText(MessageViewCategory category)
		{
			string text = StringParser.Parse(category.Text);
			if (text == null) {
				text = String.Empty;
			}
			textEditorControl.Text = text;
			textEditorControl.Refresh();
//			textEditorControl.Select(text.Length , 0);
//			textEditorControl.Select();
//			textEditorControl.ScrollToCaret();
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
		
		delegate void StringDelegate(string text);
		
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
			// Any text?
			if (textEditorControl.Text.Length > 0) {
				int line = textEditorControl.ActiveTextAreaControl.Caret.Line;
				string textLine = TextUtilities.GetLineAsString(textEditorControl.Document, line);
				
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
			}
			if (e.Key == "DefaultFont") {
				textEditorControl.Font = FontSelectionPanel.ParseFont(properties.Get("DefaultFont", new Font("Courier New", 10).ToString()).ToString());
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
				return true;
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
				return true;
			}
		}
		
		public void Cut()
		{
		}
		
		public void Copy()
		{
			new ICSharpCode.TextEditor.Actions.Copy().Execute(textEditorControl.ActiveTextAreaControl.TextArea);
		}
		
		public void Paste()
		{
		}
		
		public void Delete()
		{
		}
		
		public void SelectAll()
		{
			new ICSharpCode.TextEditor.Actions.SelectWholeDocument().Execute(textEditorControl.ActiveTextAreaControl.TextArea);
//			textEditorControl.SelectAll();
		}
		#endregion
		
	}
}
