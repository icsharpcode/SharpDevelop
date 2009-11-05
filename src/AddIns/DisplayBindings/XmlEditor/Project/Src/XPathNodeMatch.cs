// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;
using System.Xml.XPath;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Stores an XmlNode and its associated line number and position after an 
	/// XPath query has been evaluated.
	/// </summary>
	public class XPathNodeMatch : IXmlLineInfo
	{
		int? lineNumber;
		int linePosition;
		string nodeValue;
		string displayValue;
		XPathNodeType nodeType;
		
		/// <summary>
		/// Creates an XPathNodeMatch from the navigator which should be position on the
		/// node.
		/// </summary>
		/// <remarks>
		/// We deliberately use the OuterXml when we find a Namespace since the
		/// navigator location returned starts from the xmlns attribute.
		/// </remarks>
		public XPathNodeMatch(XPathNavigator currentNavigator)
		{
			SetLineNumbers(currentNavigator as IXmlLineInfo);
			nodeType = currentNavigator.NodeType;
			switch (nodeType) {
				case XPathNodeType.Text:
					SetTextValue(currentNavigator);
					break;
				case XPathNodeType.Comment:
					SetCommentValue(currentNavigator);
					break;
				case XPathNodeType.Namespace:
					SetNamespaceValue(currentNavigator);
					break;
				case XPathNodeType.Element:
					SetElementValue(currentNavigator);
					break;
				case XPathNodeType.ProcessingInstruction:
					SetProcessingInstructionValue(currentNavigator);
					break;
				case XPathNodeType.Attribute:
					SetAttributeValue(currentNavigator);
					break;
				default:
					nodeValue = currentNavigator.LocalName;
					displayValue = nodeValue;
					break;
			}
		}
		
		/// <summary>
		/// Line numbers are zero based.
		/// </summary>
		public int LineNumber {
			get { return lineNumber.GetValueOrDefault(0); }
		}
		
		/// <summary>
		/// Line positions are zero based.
		/// </summary>
		public int LinePosition {
			get { return linePosition; }
		}
		
		public bool HasLineInfo()
		{
			return lineNumber.HasValue;
		}
		
		/// <summary>
		/// Gets the text value of the node.
		/// </summary>
		public string Value {
			get { return nodeValue; }
		}
		
		/// <summary>
		/// Gets the node display value. This includes the angle brackets if it is
		/// an element, for example.
		/// </summary>
		public string DisplayValue {
			get { return displayValue; }
		}
		
		public XPathNodeType NodeType {
			get { return nodeType; }
		}
		
		void SetElementValue(XPathNavigator navigator)
		{
			nodeValue = navigator.Name;
			if (navigator.IsEmptyElement) {
				displayValue = String.Concat("<", nodeValue, "/>");
			} else {
				displayValue = String.Concat("<", nodeValue, ">");
			}
		}
		
		void SetTextValue(XPathNavigator navigator)
		{
			nodeValue = navigator.Value;
			displayValue = nodeValue;
		}
		
		void SetCommentValue(XPathNavigator navigator)
		{
			nodeValue = navigator.Value;
			displayValue = navigator.OuterXml;
		}
		
		void SetNamespaceValue(XPathNavigator navigator)
		{
			nodeValue = navigator.OuterXml;
			displayValue = nodeValue;
		}
		
		void SetProcessingInstructionValue(XPathNavigator navigator)
		{
			nodeValue = navigator.Name;
			displayValue = navigator.OuterXml;
		}
		
		void SetAttributeValue(XPathNavigator navigator)
		{
			nodeValue = navigator.Name;
			displayValue = String.Concat("@", nodeValue);
		}
		
		/// <summary>
		/// Takes one of the xml line number so the numbers are now zero
		/// based instead of one based.
		/// </summary>
		/// <remarks>A namespace query (e.g. //namespace::*) will return
		/// a line info of -1, -1 for the xml namespace. Which looks like
		/// a bug in the XPathDocument class.</remarks>
		void SetLineNumbers(IXmlLineInfo lineInfo)
		{
			if (lineInfo.HasLineInfo() && lineInfo.LineNumber > 0) {
				lineNumber = lineInfo.LineNumber - 1;
				linePosition = lineInfo.LinePosition - 1;
			}
		}
	}
}
