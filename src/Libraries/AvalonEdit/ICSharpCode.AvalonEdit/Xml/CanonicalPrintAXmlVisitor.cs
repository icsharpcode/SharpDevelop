// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Linq;
using System.Text;

namespace ICSharpCode.AvalonEdit.Xml
{
	/// <summary>
	/// Converts the XML tree back to text in canonical form.
	/// See http://www.w3.org/TR/xml-c14n
	/// </summary>
	public class CanonicalPrintAXmlVisitor: AbstractAXmlVisitor
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
		
		/// <summary> Create canonical text from a document </summary>
		public static string Print(AXmlDocument doc)
		{
			CanonicalPrintAXmlVisitor visitor = new CanonicalPrintAXmlVisitor();
			visitor.VisitDocument(doc);
			return visitor.Output;
		}
		
		/// <summary> Visit RawDocument </summary>
		public override void VisitDocument(AXmlDocument document)
		{
			foreach(AXmlObject child in document.Children) {
				AXmlTag tag = child as AXmlTag;
				// Only procssing instructions or elements
				if (tag != null && tag.IsProcessingInstruction && tag.Name != "xml") {
					VisitTag(tag);
				} else if (child is AXmlElement) {
					VisitElement((AXmlElement)child);
				}
			}
		}
		
		/// <summary> Visit RawElement </summary>
		public override void VisitElement(AXmlElement element)
		{
			base.VisitElement(element);
		}
		
		/// <summary> Visit RawTag </summary>
		public override void VisitTag(AXmlTag tag)
		{
			if (tag.IsStartOrEmptyTag) {
				sb.Append('<');
				sb.Append(tag.Name);
				foreach(AXmlAttribute attr in tag.Children.OfType<AXmlAttribute>().OrderBy(a => a.Name)) {
					VisitAttribute(attr);
				}
				sb.Append('>');
				if (tag.IsEmptyTag) {
					// Use explicit start-end pair
					sb.AppendFormat("</{0}>", tag.Name);
				}
			} else if (tag.IsEndTag) {
				sb.AppendFormat("</{0}>", tag.Name);
			} else if (tag.IsProcessingInstruction) {
				sb.Append("<?");
				sb.Append(tag.Name);
				foreach(AXmlText text in tag.Children.OfType<AXmlText>()) {
					sb.Append(text.Value);
				}
				sb.Append("?>");
			} else if (tag.IsCData) {
				foreach(AXmlText text in tag.Children.OfType<AXmlText>()) {
					sb.Append(Escape(text.Value));
				}
			}
		}
		
		/// <summary> Visit RawAttribute </summary>
		public override void VisitAttribute(AXmlAttribute attribute)
		{
			sb.Append(' ');
			sb.Append(attribute.Name);
			sb.Append("=");
			sb.Append('"');
			sb.Append(Escape(attribute.Value));
			sb.Append('"');
		}
		
		/// <summary> Visit RawText </summary>
		public override void VisitText(AXmlText text)
		{
			sb.Append(Escape(text.Value));
		}
		
		string Escape(string text)
		{
			return text
				.Replace("&", "&amp;")
				.Replace("<", "&lt;")
				.Replace(">", "&gt;")
				.Replace("\"", "&quot;")
				.Replace("\u0009", "&#9;")
				.Replace("\u000A", "&#10;")
				.Replace("\u000D", "&#13;");
		}
	}
}
