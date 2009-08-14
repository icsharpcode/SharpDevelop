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
}
