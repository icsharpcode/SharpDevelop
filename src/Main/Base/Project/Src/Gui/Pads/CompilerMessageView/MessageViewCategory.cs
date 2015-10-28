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
using System.Text;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This class represents a category with its text content used in the
	/// output pad (CompilerMessageView).
	/// </summary>
	public class MessageViewCategory : IOutputCategory
	{
		#region IOutputCategory implementation

		void IOutputCategory.Activate(bool bringPadToFront)
		{
			SD.OutputPad.CurrentCategory = this;
			if (bringPadToFront)
				SD.OutputPad.BringToFront();
		}

		void IOutputCategory.Clear()
		{
			ClearText();
		}

		void IOutputCategory.AppendText(RichText text)
		{
			AppendText(text.ToString());
		}

		void IOutputCategory.AppendLine(RichText text)
		{
			AppendLine(text.ToString());
		}

		string IOutputCategory.DisplayName {
			get { return displayCategory; }
		}

		#endregion

		#region Static methods to create MessageViewCategories
		/// <summary>
		/// Creates a new MessageViewCategory with the specified category
		/// and adds it to the CompilerMessageView pad.
		/// This method is thread-safe and works correctly even if called concurrently for the same
		/// category; only one messageViewCategory will be created.
		/// </summary>
		public static void Create(ref MessageViewCategory messageViewCategory, string category)
		{
			Create(ref messageViewCategory, category, category);
		}
		
		/// <summary>
		/// Creates a new MessageViewCategory with the specified category
		/// and adds it to the CompilerMessageView pad.
		/// This method is thread-safe and works correctly even if called concurrently for the same
		/// category; only one messageViewCategory will be created.
		/// </summary>
		public static void Create(ref MessageViewCategory messageViewCategory, string category, string displayCategory)
		{
			MessageViewCategory newMessageViewCategory = new MessageViewCategory(category, displayCategory);
			if (System.Threading.Interlocked.CompareExchange(ref messageViewCategory, newMessageViewCategory, null) == null) {
				// this thread was successful creating the category, so add it
				CompilerMessageView.Instance.AddCategory(newMessageViewCategory);
			}
		}
		#endregion
		
		string        category;
		string        displayCategory;
		readonly StringBuilder textBuilder = new StringBuilder();
		
		/// <summary>
		/// Gets the object on which the MessageViewCategory locks.
		/// </summary>
		public object SyncRoot {
			get {
				return textBuilder;
			}
		}
		
		public string Category {
			get {
				return category;
			}
		}
		
		public string DisplayCategory {
			get {
				return displayCategory;
			}
		}
		
		public string Text {
			get {
				lock (textBuilder) {
					return textBuilder.ToString();
				}
			}
		}
		
		public MessageViewCategory(string category) : this(category, category)
		{
		}
		
		public MessageViewCategory(string category, string displayCategory)
		{
			this.category        = category;
			this.displayCategory = displayCategory;
		}
		
		public void AppendLine(string text)
		{
			AppendText(text + Environment.NewLine);
		}
		
		public void AppendText(string text)
		{
			const int MaxTextSize = 50 * 1000 * 1000; // 50m chars = 100 MB
			string truncatedText = SD.ResourceService.GetString("MainWindow.Windows.OutputWindow.TextTooLong") + "\r\n";
			
			lock (textBuilder) {
				if (textBuilder.Length + text.Length > MaxTextSize) {
					int amountToCopy = MaxTextSize / 2 - text.Length;
					if (amountToCopy <= 0) {
						SetText(truncatedText + text.Substring(text.Length - MaxTextSize / 2, MaxTextSize / 2));
					} else {
						SetText(truncatedText + textBuilder.ToString(textBuilder.Length - amountToCopy, amountToCopy) + text);
					}
				} else {
					textBuilder.Append(text);
					OnTextAppended(new TextEventArgs(text));
				}
			}
		}
		
		public void SetText(string text)
		{
			lock (textBuilder) {
				// clear text:
				textBuilder.Length = 0;
				// reset capacity: we must shrink the textBuilder at some point to reclaim memory
				textBuilder.Capacity = text.Length + 16;
				textBuilder.Append(text);
				OnTextSet(new TextEventArgs(text));
			}
		}
		
		public void ClearText()
		{
			SetText(string.Empty);
		}
		
		protected virtual void OnTextAppended(TextEventArgs e)
		{
			if (TextAppended != null) {
				TextAppended(this, e);
			}
		}
		
		protected virtual void OnTextSet(TextEventArgs e)
		{
			if (TextSet != null) {
				TextSet(this, e);
			}
		}
		
		/// <summary>
		/// Is raised when text is appended to the MessageViewCategory.
		/// Warning: This event is raised inside a lock held by the MessageViewCategory. This is necessary
		/// to ensure TextAppended event handlers are called in the same order as text is appended to the category
		/// when there are multiple threads writing to the same category.
		/// </summary>
		public event TextEventHandler TextAppended;
		
		/// <summary>
		/// Is raised when text is appended to the MessageViewCategory.
		/// Warning: This event is raised inside a lock held by the MessageViewCategory. This is necessary
		/// to ensure TextAppended and TextSet event handlers are called in the same order as
		/// text is appended or set
		/// when there are multiple threads writing to the same category.
		/// </summary>
		public event TextEventHandler TextSet;
	}
}
