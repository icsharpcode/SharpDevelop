// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core;


namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This class represents a category with its text content used in the 
	/// output pad (CompilerMessageView).
	/// </summary>
	public class MessageViewCategory
	{
		string        category;
		string        displayCategory;
		StringBuilder textBuilder = new StringBuilder();
		
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
				return textBuilder.ToString();
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
		
		public void AppendText(string text)
		{
			textBuilder.Append(text);
			OnTextAppended(new TextEventArgs(text));
		}
		
		public void SetText(string text)
		{
			textBuilder.Length = 0;
			textBuilder.Append(text);
			OnTextSet(new TextEventArgs(text));
		}
		
		public void ClearText()
		{
			textBuilder.Length = 0;
			OnCleared(EventArgs.Empty);
		}
		
		protected virtual void OnTextAppended(TextEventArgs e) 
		{
			if (TextAppended != null) {
				TextAppended(this, e);
			}
		}
		
		
		protected virtual void OnCleared(EventArgs e)
		{
			if (Cleared != null) {
				Cleared(this, e);
			}
		}
		
		protected virtual void OnTextSet(TextEventArgs e)
		{
			if (TextSet != null) {
				TextSet(this, e);
			}
		}
		
		public event TextEventHandler TextAppended;
		public event TextEventHandler TextSet;
		public event EventHandler     Cleared;
	}
}
