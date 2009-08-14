// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Text;

namespace ICSharpCode.AvalonEdit.XmlParser
{
	/// <summary>
	/// Converts the XML tree back to text.
	/// The text should exactly match the original.
	/// </summary>
	public class PrettyPrintXmlVisitor: AbstractXmlVisitor
	{
		StringBuilder sb = new StringBuilder();
		
		/// <summary>
		/// Gets the pretty printed text
		/// </summary>
		public string Output {
			get {
				return sb.ToString();
			}
		}
		
		/// <summary> Visit RawDocument </summary>
		public override void VisitDocument(RawDocument document)
		{
			base.VisitDocument(document);
		}
		
		/// <summary> Visit RawElement </summary>
		public override void VisitElement(RawElement element)
		{
			base.VisitElement(element);
		}
		
		/// <summary> Visit RawTag </summary>
		public override void VisitTag(RawTag tag)
		{
			sb.Append(tag.OpeningBracket);
			sb.Append(tag.Name);
			base.VisitTag(tag);
			sb.Append(tag.ClosingBracket);
		}
		
		/// <summary> Visit RawAttribute </summary>
		public override void VisitAttribute(RawAttribute attribute)
		{
			sb.Append(attribute.Name);
			sb.Append(attribute.EqualsSign);
			sb.Append(attribute.QuotedValue);
		}
		
		/// <summary> Visit RawText </summary>
		public override void VisitText(RawText text)
		{
			sb.Append(text.EscapedValue);
		}
	}
}
