// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
				case XmlCompletionItemType.NamespaceUri:
					context.Editor.Document.Insert(context.StartOffset, "\"");
					context.Editor.Document.Insert(context.EndOffset + 1, "\"");
					break;
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
			return base.GetHashCode();
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
