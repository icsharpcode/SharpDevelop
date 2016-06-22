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
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// The type of text held in this object.
	/// </summary>
	public enum XmlCompletionItemType {
		None = 0,
		XmlElement = 1,
		XmlAttribute = 2,
		NamespaceUri = 3,
		XmlAttributeValue = 4
	}
	
	/// <summary>
	/// Holds the text for  namespace, child element or attribute
	/// autocomplete (intellisense).
	/// </summary>
	public class XmlCompletionItem : DefaultCompletionItem, IComparable<XmlCompletionItem>
	{
		XmlCompletionItemType dataType = XmlCompletionItemType.XmlElement;
		string description = String.Empty;
		
		public XmlCompletionItem(string text)
			: this(text, String.Empty, XmlCompletionItemType.XmlElement)
		{
		}
		
		public XmlCompletionItem(string text, string description)
			: this(text, description, XmlCompletionItemType.XmlElement)
		{
		}

		public XmlCompletionItem(string text, XmlCompletionItemType dataType)
			: this(text, String.Empty, dataType)
		{
		}

		public XmlCompletionItem(string text, string description, XmlCompletionItemType dataType)
			: base(text)
		{
			this.description = description;
			this.dataType = dataType;
		}
		
		/// <summary>
		/// Returns the xml item's documentation as retrieved from
		/// the xs:annotation/xs:documentation element.
		/// </summary>
		public override string Description {
			get { return description; }
		}
		
		public XmlCompletionItemType DataType {
			get { return dataType; }
		}
		
		public override void Complete(CompletionContext context)
		{
			base.Complete(context);
			
			switch (dataType) {
				case XmlCompletionItemType.XmlAttribute:
					context.Editor.Document.Insert(context.EndOffset, "=\"\"");
					context.Editor.Caret.Offset--;
//					XmlCodeCompletionBinding.Instance.CtrlSpace(context.Editor);
					break;
			}
		}
		
		public override string ToString()
		{
			return "[" + Text + "]";
		}
		
		public override int GetHashCode()
		{
			return dataType.GetHashCode() ^ Text.GetHashCode();
		}
		
		public override bool Equals(object obj)
		{
			XmlCompletionItem item = obj as XmlCompletionItem;
			if (item != null) {
				return (dataType == item.dataType) && (Text == item.Text);
			}
			return false;
		}
		
		public int CompareTo(XmlCompletionItem other)
		{
			return Text.CompareTo(other.Text);
		}
	}
}
