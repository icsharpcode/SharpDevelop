// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// SuppressMessage justification: we're just adding position info to XmlDocument and don't want to fix
	// any of it's other issues.
	
	/// <summary>
	/// Class derived from System.Xml.XmlDocument that remembers line/column information for elements and attributes
	/// when loading from a <see cref="XmlTextReader"/> or other <see cref="IXmlLineInfo"/>-implementing reader.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1058:TypesShouldNotExtendCertainBaseTypes")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
	public class PositionXmlDocument : XmlDocument
	{
		IXmlLineInfo lineInfo; // a reference to the XmlReader, only set during load time
		
		/// <summary>
		/// Creates a PositionXmlElement.
		/// </summary>
		public override XmlElement CreateElement(string prefix, string localName, string namespaceURI)
		{
			return new PositionXmlElement(prefix, localName, namespaceURI, this, lineInfo);
		}
		
		/// <summary>
		/// Creates a PositionXmlAttribute.
		/// </summary>
		public override XmlAttribute CreateAttribute(string prefix, string localName, string namespaceURI)
		{
			return new PositionXmlAttribute(prefix, localName, namespaceURI, this, lineInfo);
		}
		
		/// <summary>
		/// Loads the XML document from the specified <see cref="XmlReader"/>.
		/// </summary>
		public override void Load(XmlReader reader)
		{
			lineInfo = reader as IXmlLineInfo;
			try {
				base.Load(reader);
			} finally {
				lineInfo = null;
			}
		}
	}
	
	/// <summary>
	/// XML Element with line/column information.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
	public class PositionXmlElement : XmlElement, IXmlLineInfo
	{
		internal PositionXmlElement (string prefix, string localName, string namespaceURI, XmlDocument doc, IXmlLineInfo lineInfo)
			: base(prefix, localName, namespaceURI, doc)
		{
			if (lineInfo != null) {
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}
		
		int lineNumber;
		int linePosition;
		bool hasLineInfo;
		
		/// <summary>
		/// Gets whether the element has line information.
		/// </summary>
		public bool HasLineInfo()
		{
			return hasLineInfo;
		}
		
		/// <summary>
		/// Gets the line number.
		/// </summary>
		public int LineNumber {
			get { return lineNumber; }
		}
		
		/// <summary>
		/// Gets the line position (column).
		/// </summary>
		public int LinePosition {
			get { return linePosition; }
		}
	}
	
	/// <summary>
	/// XML Attribute with line/column information.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
	public class PositionXmlAttribute : XmlAttribute, IXmlLineInfo
	{
		internal PositionXmlAttribute (string prefix, string localName, string namespaceURI, XmlDocument doc, IXmlLineInfo lineInfo)
			: base(prefix, localName, namespaceURI, doc)
		{
			if (lineInfo != null) {
				this.lineNumber = lineInfo.LineNumber;
				this.linePosition = lineInfo.LinePosition;
				this.hasLineInfo = true;
			}
		}
		
		int lineNumber;
		int linePosition;
		bool hasLineInfo;
		
		/// <summary>
		/// Gets whether the element has line information.
		/// </summary>
		public bool HasLineInfo()
		{
			return hasLineInfo;
		}
		
		/// <summary>
		/// Gets the line number.
		/// </summary>
		public int LineNumber {
			get { return lineNumber; }
		}
		
		/// <summary>
		/// Gets the line position (column).
		/// </summary>
		public int LinePosition {
			get { return linePosition; }
		}
	}
}
