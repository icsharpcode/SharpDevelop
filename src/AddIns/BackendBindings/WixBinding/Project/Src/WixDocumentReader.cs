// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.WixBinding
{
	public class WixDocumentReader
	{
		XmlTextReader reader;
		
		/// <summary>
		/// Class used to store the line number and dialog id of the 
		/// dialog element start tag.
		/// </summary>
		class DialogStartElement
		{
			int line = -1;
			string id = String.Empty;
			
			public DialogStartElement(int line, string id)
			{
				this.id = id;
				this.line = line;
			}
			
			public int Line {
				get { return line; }
			}
			
			public string Id {
				get { return id; }
			}
		}
		
		public WixDocumentReader(string document)
			: this(new StringReader(document))
		{
		}
		
		public WixDocumentReader(TextReader textReader)
		{
			reader = new XmlTextReader(textReader);
		}
		
		public ReadOnlyCollection<string> GetDialogIds()
		{
			using (reader) {
				List<string> dialogIds = new List<string>();
				object dialogElementName = reader.NameTable.Add("Dialog");
				while (reader.Read()) {
					switch (reader.NodeType) {
						case XmlNodeType.Element:
							if (IsElementMatch(dialogElementName, reader.LocalName)) {
								AddDialogId(dialogIds);
							}
							break;
					}
				}
				return new ReadOnlyCollection<string>(dialogIds);
			}
		}
		
		/// <summary>
		/// Checks that the atomised element names match.
		/// </summary>
		bool IsElementMatch(object elementName, object currentElementName)
		{
			return elementName == currentElementName;
		}
		
		/// <summary>
		/// Reads the dialog id and adds it to the list of dialogs found so far.
		/// </summary>
		void AddDialogId(List<string> dialogIds)
		{
			string id = GetIdAttributeValueFromCurrentNode();
			if (id.Length > 0) {
				dialogIds.Add(id);
			}
		}
		
		string GetIdAttributeValueFromCurrentNode()
		{
			if (reader.MoveToAttribute("Id")) {
				if (reader.Value != null) {
					return reader.Value;
				}
			}
			return String.Empty;
		}
		
		/// <summary>
		/// Gets the line and column where the specified element starts. The column
		/// returned is the column containing the opening tag (&lt;).
		/// </summary>
		public Location GetStartElementLocation(string elementName, string elementIdAttribute)
		{
			using (reader) {			
				object elementNameObject = reader.NameTable.Add(elementName);
				while (reader.Read()) {
					switch (reader.NodeType) {
						case XmlNodeType.Element:
							if (IsElementMatch(elementNameObject, reader.LocalName)) {
								Location location = GetStartElementLocationIfMatch(elementIdAttribute);
								if (!location.IsEmpty) {
									return location;
								}
							}
							break;
					}
				}
			}
			return Location.Empty;
		}
		
		/// <summary>
		/// Gets the line and column position if the element's attribute id matches the 
		/// element at the current reader position.
		/// </summary>
		Location GetStartElementLocationIfMatch(string idAttributeValue)
		{
			// Store the column and line position since the call to GetIdFromCurrentNode will
			// move to the <Dialog> Id attribute.
			int line = reader.LineNumber;	
			int column = reader.LinePosition - 1; // Take off 1 to so the '<' is located.

			if (idAttributeValue == GetIdAttributeValueFromCurrentNode()) {
				return new Location(column, line);
			}
			return Location.Empty;
		}
		
		/// <summary>
		/// Location line numbers are one based.
		/// </summary>
		public Location GetEndElementLocation(string name, string id)
		{
			using (reader) {
				bool startElementFound = false;
				object elementName = reader.NameTable.Add(name);
				while (reader.Read()) {
					switch (reader.NodeType) {
						case XmlNodeType.Element:
							if (IsElementMatch(elementName, reader.LocalName)) {
								Location location = GetStartElementLocationIfMatch(id);
								startElementFound = !location.IsEmpty;
							}
							break;
						case XmlNodeType.EndElement:
							if (startElementFound) {
								if (IsElementMatch(elementName, reader.LocalName)) {
									// Take off an extra 2 from the line position so we get the 
									// correct column for the < tag rather than the element name.
									return new Location(reader.LinePosition - 2, reader.LineNumber);
								}
							}
							break;
					}
				}
			}
			return Location.Empty;
		}
		
		/// <summary>
		/// Gets the dialog id at the specified line.
		/// </summary>
		/// <param name="index">Line numbers start from one.</param>
		public string GetDialogId(int line)
		{
			DialogStartElement dialogStartElement = null;
			
			using (reader) {
				object dialogElementName = reader.NameTable.Add("Dialog");
				while (reader.Read()) {
					switch (reader.NodeType) {
						case XmlNodeType.Element:
							if (IsElementMatch(dialogElementName, reader.LocalName)) {
								if (line < reader.LineNumber) {
									return null;
								} else if (line == reader.LineNumber) {
									return GetIdAttributeValueFromCurrentNode();
								} else if (reader.IsStartElement()) {
									dialogStartElement = new DialogStartElement(reader.LineNumber, GetIdAttributeValueFromCurrentNode());
								}
							}
							break;
						case XmlNodeType.EndElement:
							if (IsElementMatch(dialogElementName, reader.LocalName)) {
								if (line > dialogStartElement.Line && line <= reader.LineNumber) {
									return dialogStartElement.Id;
								} 
							}
							break;
					}
				}
			}
			return null;
		}
		
		/// <summary>
		/// Gets the region (start line, column to end line, column) of the xml
		/// element that has the specified id. This includes the start and end tags, the start column is the column
		/// containing the start tag and the end column is the column containing the final
		/// end tag marker.
		/// </summary>
		/// <remarks>
		/// The lines and columns in the region are one based.
		/// </remarks>
		/// <param name="name">The name of the element.</param>
		/// <param name="id">The id attribute value of the element.</param>
		public DomRegion GetElementRegion(string name, string id)
		{
			Location startLocation = Location.Empty;
			
			using (reader) {			
				int nestedElementsCount = -1;
				object elementName = reader.NameTable.Add(name);
				while (reader.Read()) {
					switch (reader.NodeType) {
						case XmlNodeType.Element:
							if (IsElementMatch(elementName, reader.LocalName)) {
								if (nestedElementsCount == -1) {
									bool isEmptyElement = reader.IsEmptyElement;
									startLocation = GetStartElementLocationIfMatch(id);
									if (!startLocation.IsEmpty) {
										nestedElementsCount = 0;
										if (isEmptyElement) {
											Location endLocation = GetEmptyElementEnd();
											return DomRegion.FromLocation(startLocation, endLocation);
										}
									}
								} else if (!reader.IsEmptyElement) {
									++nestedElementsCount;
								}
							}
							break;
						case XmlNodeType.EndElement:
							if (!startLocation.IsEmpty && IsElementMatch(elementName, reader.LocalName)) {
								if (nestedElementsCount == 0) {
									Location endLocation = GetEndElementEnd();
									return DomRegion.FromLocation(startLocation, endLocation);
								}
								--nestedElementsCount;
							}
							break;
					}
				}
			}
			return DomRegion.Empty;
		}
		
		/// <summary>
		/// Determines the end of the empty element including the final end tag marker
		/// (the greater than sign). This method moves the XmlTextReader to the end
		/// of the element tag.
		/// </summary>
		Location GetEmptyElementEnd()
		{
			reader.ReadStartElement();
			int line = reader.LineNumber;
			
			// Take off one as we have moved passed the end tag
			// column.
			int column = reader.LinePosition - 1;
			return new Location(column, line);
		}
		
		/// <summary>
		/// Determines the end of the end element including the final end tag marker
		/// (the greater than sign). This method moves the XmlTextReader to the end of
		/// the end tag.
		/// </summary>
		Location GetEndElementEnd()
		{
			reader.ReadEndElement();
			int line = reader.LineNumber;
			
			// Take off one as we have moved passed the end tag column.
			int column = reader.LinePosition - 1;
			
			// If ReadEndElement has moved to the start of another element
			// take off one from the column value otherwise the column
			// value includes the start tag of the next element.
			if (reader.NodeType == XmlNodeType.Element) {
				--column;
			}
			return new Location(column, line);
		}
	}
}
