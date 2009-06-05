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
	public enum XmlCompletionDataType {
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
	public class XmlCompletionItem : DefaultCompletionItem
	{
		XmlCompletionDataType dataType = XmlCompletionDataType.XmlElement;
		string description = string.Empty;
		
		public XmlCompletionItem(string text)
			: this(text, string.Empty, XmlCompletionDataType.XmlElement)
		{
		}
		
		public XmlCompletionItem(string text, string description)
			: this(text, description, XmlCompletionDataType.XmlElement)
		{
		}

		public XmlCompletionItem(string text, XmlCompletionDataType dataType)
			: this(text, string.Empty, dataType)
		{
		}

		public XmlCompletionItem(string text, string description, XmlCompletionDataType dataType)
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
			get {
				return description;
			}
		}
		
		public override void Complete(CompletionContext context)
		{
			base.Complete(context);
			
			switch (dataType) {
				case XmlCompletionDataType.NamespaceUri:
					context.Editor.Document.Insert(context.StartOffset, "\"");
					context.Editor.Document.Insert(context.EndOffset, "\"");
					break;
				case XmlCompletionDataType.XmlAttribute:
					context.Editor.Document.Insert(context.EndOffset, "=\"\"");
					context.Editor.Caret.Offset--;
					XmlCodeCompletionBinding.Instance.CtrlSpace(context.Editor);
					break;
			}
		}
		
		public override string ToString()
		{
			return "[" + this.Text + "]";
		}
		
	}
}
