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
	/// Visitor for the XML tree
	/// </summary>
	public interface IXmlVisitor
	{
		/// <summary> Visit RawDocument </summary>
		void VisitDocument(RawDocument document);
		
		/// <summary> Visit RawElement </summary>
		void VisitElement(RawElement element);
		
		/// <summary> Visit RawTag </summary>
		void VisitTag(RawTag tag);
		
		/// <summary> Visit RawAttribute </summary>
		void VisitAttribute(RawAttribute attribute);
		
		/// <summary> Visit RawText </summary>
		void VisitText(RawText text);
	}
	
	/// <summary>
	/// Derive from this class to create visitor for the XML tree
	/// </summary>
	public abstract class AbstractXmlVisitor : IXmlVisitor
	{
		/// <summary> Visit RawDocument </summary>
		public virtual void VisitDocument(RawDocument document)
		{
			foreach(RawObject child in document.Children) child.AcceptVisitor(this);
		}
		
		/// <summary> Visit RawElement </summary>
		public virtual void VisitElement(RawElement element)
		{
			foreach(RawObject child in element.Children) child.AcceptVisitor(this);
		}
		
		/// <summary> Visit RawTag </summary>
		public virtual void VisitTag(RawTag tag)
		{
			foreach(RawObject child in tag.Children) child.AcceptVisitor(this);
		}
		
		/// <summary> Visit RawAttribute </summary>
		public virtual void VisitAttribute(RawAttribute attribute)
		{
			
		}
		
		/// <summary> Visit RawText </summary>
		public virtual void VisitText(RawText text)
		{
			
		}
	}
	
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
