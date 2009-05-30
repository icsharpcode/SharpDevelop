// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2932 $</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Holds the text for  namespace, child element or attribute
	/// autocomplete (intellisense).
	/// </summary>
	public class XmlCompletionItem : DefaultCompletionItem
	{
		DataType dataType = DataType.XmlElement;
		string description = String.Empty;
		
		/// <summary>
		/// The type of text held in this object.
		/// </summary>
		public enum DataType {
			XmlElement = 1,
			XmlAttribute = 2,
			NamespaceUri = 3,
			XmlAttributeValue = 4
		}
		
		public XmlCompletionItem(string text)
			: this(text, String.Empty, DataType.XmlElement)
		{
		}
		
		public XmlCompletionItem(string text, string description)
			: this(text, description, DataType.XmlElement)
		{
		}

		public XmlCompletionItem(string text, DataType dataType)
			: this(text, String.Empty, dataType)
		{
		}

		public XmlCompletionItem(string text, string description, DataType dataType)
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
				case DataType.NamespaceUri:
					context.Editor.Document.Insert(context.StartOffset, "\"");
					context.Editor.Document.Insert(context.EndOffset, "\"");
					break;
				case DataType.XmlAttribute:
					context.Editor.Document.Insert(context.EndOffset, "=\"\"");
					context.Editor.Caret.Offset--;
					XmlCodeCompletionBinding.Instance.CtrlSpace(context.Editor);
					break;
			}
		}
	}
}
