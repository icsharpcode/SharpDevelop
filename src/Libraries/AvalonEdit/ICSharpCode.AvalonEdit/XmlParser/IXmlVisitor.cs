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
}
