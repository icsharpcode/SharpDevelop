// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// A Wix document (.wxs or .wxi).
	/// </summary>
	public class WixDocument : XmlDocument, IWixPropertyValueProvider
	{
		public const string WixSourceFileExtension = ".wxs";
		public const string WixIncludeFileExtension = ".wxi";
				
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
				get {
					return line;
				}
			}
			
			public string Id {
				get {
					return id;
				}
			}
		}
		
		/// <summary>
		/// Default IFileLoader implementation which tries to load
		/// the bitmap from the specified filename. If the file does 
		/// not exist the bitmap is not loaded.
		/// </summary>
		class FileLoader : IFileLoader
		{
			public Bitmap GetBitmap(string fileName)
			{
				if (File.Exists(fileName)) {
					return new Bitmap(fileName);
				}
				return null;
			}
		}
		
		WixNamespaceManager namespaceManager;
		IFileLoader fileLoader;
		WixProject project;
		string fileName = String.Empty;
		
		public WixDocument() : this((WixProject)null)
		{
		}
		
		public WixDocument(WixProject project) : this(project, new FileLoader())
		{
		}
		
		/// <summary>
		/// Creates a new instance of the WixDocument class but overrides the 
		/// default file loading functionality of the WixDocument. 
		/// </summary>
		public WixDocument(WixProject project, IFileLoader fileLoader)
		{
			this.project = project;
			this.fileLoader = fileLoader;
			if (fileLoader == null) {
				throw new ArgumentException("Cannot be null.", "fileLoader");
			}
			namespaceManager = new WixNamespaceManager(NameTable);			
		}
		
		/// <summary>
		/// Gets the project that this document belongs to.
		/// </summary>
		public WixProject Project {
			get {
				return project;
			}
		}
		
		/// <summary>
		/// Gets or sets the filename for the Wix Document.
		/// </summary>
		public string FileName {
			get {
				return fileName;
			}
			set {
				fileName = value;
			}
		}
		
		/// <summary>
		/// Gets the dialogs from a Wix file.
		/// </summary>
		public static ReadOnlyCollection<string> GetDialogIds(string fileName)
		{
			using (XmlTextReader reader = new XmlTextReader(new FileStream(fileName, FileMode.Open, FileAccess.Read))) {
				return GetDialogIds(reader);
			}
		}
		
		/// <summary>
		/// Gets the dialogs from a Wix file.
		/// </summary>
		public static ReadOnlyCollection<string> GetDialogIds(TextReader reader)
		{
			XmlTextReader xmlReader = new XmlTextReader(reader);
			return GetDialogIds(xmlReader);
		}
		
		/// <summary>
		/// Gets the dialog Ids in a Wix file.
		/// </summary>
		public static ReadOnlyCollection<string> GetDialogIds(XmlTextReader reader)
		{
			using (reader) {
				List<string> dialogIds = new List<string>();
				object dialogElementName = reader.NameTable.Add("Dialog");
				while (reader.Read()) {
					switch (reader.NodeType) {
						case XmlNodeType.Element:
							if (IsElementMatch(dialogElementName, reader.LocalName)) {
								AddDialogId(reader, dialogIds);
							}
							break;
					}
				}
				return new ReadOnlyCollection<string>(dialogIds);
			}
		}

		/// <summary>
		/// Gets the line and column where the specified element starts. The column
		/// returned is the column containing the opening tag (&lt;).
		/// </summary>
		/// <param name="name">The element name.</param>
		/// <param name="id">The id attribute value.</param>
		public static Location GetStartElementLocation(TextReader reader, string name, string id)
		{
			XmlTextReader xmlReader = new XmlTextReader(reader);
			return GetStartElementLocation(xmlReader, name, id);			
		}
		
		/// <summary>
		/// Gets the line and column where the specified element starts. The column
		/// returned is the column containing the opening tag (&lt;).
		/// </summary>
		/// <param name="name">The element name.</param>
		/// <param name="id">The id attribute value.</param>
		public static Location GetStartElementLocation(XmlTextReader reader, string name, string id)
		{
			using (reader) {			
				object elementName = reader.NameTable.Add(name);
				while (reader.Read()) {
					switch (reader.NodeType) {
						case XmlNodeType.Element:
							if (IsElementMatch(elementName, reader.LocalName)) {
								Location location = GetStartElementLocationIfMatch(reader, id);
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
		/// Gets the line and column where the specified element ends. The column
		/// returned is the column containing the opening tag (&lt;) of the end element.
		/// </summary>
		/// <param name="name">The element name.</param>
		/// <param name="id">The id attribute value.</param>
		public static Location GetEndElementLocation(TextReader reader, string name, string id)
		{
			XmlTextReader xmlReader = new XmlTextReader(reader);
			return GetEndElementLocation(xmlReader, name, id);			
		}
		
		/// <summary>
		/// Gets the line and column where the specified element ends. The column
		/// returned is the column containing the opening tag (&lt;) of the end element.
		/// </summary>
		/// <param name="name">The element name.</param>
		/// <param name="id">The id attribute value.</param>
		public static Location GetEndElementLocation(XmlTextReader reader, string name, string id)
		{
			using (reader) {	
				bool startElementFound = false;
				object elementName = reader.NameTable.Add(name);
				while (reader.Read()) {
					switch (reader.NodeType) {
						case XmlNodeType.Element:
							if (IsElementMatch(elementName, reader.LocalName)) {
								Location location = GetStartElementLocationIfMatch(reader, id);
								startElementFound = !location.IsEmpty;
							}
							break;
						case XmlNodeType.EndElement:
							if (startElementFound) {
								if (IsElementMatch(elementName, reader.LocalName)) {
									// Take off an extra 2 from the line position so we get the 
									// correct column for the < tag rather than the element name.
									return new Location(reader.LinePosition - 3, reader.LineNumber - 1);
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
		/// <param name="index">Line numbers start from zero.</param>
		/// <returns>The dialog id found at the specified line;
		/// <see langword="null"/> if no dialog found.</returns>
		public static string GetDialogId(TextReader reader, int line)
		{
			XmlTextReader xmlReader = new XmlTextReader(reader);
			return GetDialogId(xmlReader, line);
		}
		
		/// <summary>
		/// Gets the dialog id at the specified line.
		/// </summary>
		/// <param name="index">Line numbers start from zero.</param>
		/// <returns>The dialog id found at the specified line;
		/// <see langword="null"/> if no dialog found.</returns>
		public static string GetDialogId(XmlTextReader reader, int line)
		{
			// Add one to line since XmlTextReader line numbers start from one.
			++line;
			
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
									return GetIdFromCurrentNode(reader);
								} else if (reader.IsStartElement()) {
									dialogStartElement = new DialogStartElement(reader.LineNumber, GetIdFromCurrentNode(reader));
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
		/// Checks the file extension to see if the file is a Wix file. The file
		/// can either be a Wix source file (.wxs) or a Wix include file (.wxi).
		/// </summary>
		public static bool IsWixFileName(string fileName)
		{
			if (fileName == null) {
				return false;
			}
			string extension = Path.GetExtension(fileName.ToLowerInvariant());
			switch (extension) {
				case WixSourceFileExtension:
					return true;
				case WixIncludeFileExtension:
					return true;
			}
			return false;
		}
		
		/// <summary>
		/// Checks whether the file extension is for a Wix source file (.wxs).
		/// </summary>
		public static bool IsWixSourceFileName(string fileName)
		{
			return String.Compare(Path.GetExtension(fileName), WixSourceFileExtension, true) == 0;
		}
		
		/// <summary>
		/// Converts a DomRegion to an ISegment for the given document.
		/// </summary>
		public static ISegment ConvertRegionToSegment(IDocument document, DomRegion region)
		{
			// Single line region
			if (region.BeginLine == region.EndLine) {
				ICSharpCode.TextEditor.Document.LineSegment documentSegment = document.GetLineSegment(region.BeginLine);
				return new WixDocumentLineSegment(documentSegment.Offset + region.BeginColumn, 
					region.EndColumn + 1 - region.BeginColumn);
			}
			
			// Multiple line region.
			int length = 0;
			int startOffset = 0;
			for (int line = region.BeginLine; line <= region.EndLine; ++line) {
				ICSharpCode.TextEditor.Document.LineSegment currentSegment = document.GetLineSegment(line);
				if (line == region.BeginLine) {
					length += currentSegment.TotalLength - region.BeginColumn;
					startOffset = currentSegment.Offset + region.BeginColumn;
				} else if (line < region.EndLine) {
					length += currentSegment.TotalLength;
				} else {
					length += region.EndColumn + 1;
				}
			}
			return new WixDocumentLineSegment(startOffset, length);
		}
		
		/// <summary>
		/// Creates an XML fragment string for the specified element.
		/// </summary>
		/// <param name="lineTerminator">The line terminator.</param>
		/// <param name="tabsToSpaces">Indicates whether tabs will be converted to spaces.</param>
		/// <param name="tabIndent">The indent to be used when converting tabs to spaces.</param>
		/// <returns>An XML fragment string without any Wix namespace attributes.</returns>
		public static string GetXml(XmlElement element, string lineTerminator, bool tabsToSpaces, int tabIndent)
		{
			StringBuilder xml = new StringBuilder();
			StringWriter stringWriter = new StringWriter(xml);
			XmlWriterSettings xmlWriterSettings = CreateXmlWriterSettings(lineTerminator, tabsToSpaces, tabIndent);
			using (XmlWriter xmlWriter = XmlTextWriter.Create(stringWriter, xmlWriterSettings)) {
				element.WriteTo(xmlWriter);
			}
			return xml.ToString().Replace(String.Concat(" xmlns=\"", WixNamespaceManager.Namespace, "\""), String.Empty);;
		}
		
		/// <summary>
		/// Gets the region (start line, column to end line, column) of the xml
		/// element that has the specified id. This includes the start and end tags, the start column is the column
		/// containing the start tag and the end column is the column containing the final
		/// end tag marker.
		/// </summary>
		/// <param name="name">The name of the element.</param>
		/// <param name="id">The id attribute value of the element.</param>
		public static DomRegion GetElementRegion(TextReader reader, string name, string id)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(reader);
			return GetElementRegion(xmlTextReader, name, id);
		}
		
		/// <summary>
		/// Gets the region (start line, column to end line, column) of the xml
		/// element that has the specified id. This includes the start and end tags, the start column is the column
		/// containing the start tag and the end column is the column containing the final
		/// end tag marker.
		/// </summary>
		/// <param name="name">The name of the element.</param>
		/// <param name="id">The id attribute value of the element.</param>
		public static DomRegion GetElementRegion(XmlTextReader reader, string name, string id)
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
									startLocation = GetStartElementLocationIfMatch(reader, id);
									if (!startLocation.IsEmpty) {
										nestedElementsCount = 0;
										if (isEmptyElement) {
											Location endLocation = GetEmptyElementEnd(reader);
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
									Location endLocation = GetEndElementEnd(reader);
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
		/// Gets a WixDialog object for the specified dialog id.
		/// </summary>
		/// <param name="id">The id of the dialog.</param>
		/// <returns>A <see cref="WixDialog"/> for the dialog id
		/// found; otherwise <see langword="null"/> if no dialog
		/// with the specified id can be found.</returns>
		public WixDialog GetDialog(string id)
		{
			XmlElement dialogElement = GetDialogElement(id);
			if (dialogElement != null) {
				return new WixDialog(this, dialogElement);
			}
			return null;
		}
		
		/// <summary>
		/// Gets a WixDialog object for the specified dialog id.
		/// </summary>
		/// <param name="id">The id of the dialog.</param>
		/// <param name="reader">The text file reader to use when looking up
		/// binary filenames.</param>
		/// <returns>A <see cref="WixDialog"/> for the dialog id
		/// found; otherwise <see langword="null"/> if no dialog
		/// with the specified id can be found.</returns>
		public WixDialog GetDialog(string id, ITextFileReader reader)
		{
			XmlElement dialogElement = GetDialogElement(id);
			if (dialogElement != null) {
				return new WixDialog(this, dialogElement, new WixBinaries(this, reader));
			}
			return null;
		}
		
		/// <summary>
		/// Gets the binary filename for the specified id.
		/// </summary>
		/// <returns><see langword="null"/> if the id cannot be found.</returns>
		public string GetBinaryFileName(string id)
		{
			string xpath = String.Concat("//w:Binary[@Id='", XmlEncode(id), "']");
			WixBinaryElement binaryElement = (WixBinaryElement)SelectSingleNode(xpath, namespaceManager);
			if (binaryElement != null) {
				return binaryElement.FileName;
			}
			return null;
		}
		
		/// <summary>
		/// Loads the bitmap using the Wix Bitmap id.
		/// </summary>
		public Bitmap GetBitmapFromId(string id)
		{
			string bitmapFileName = GetBinaryFileName(id);
			return GetBitmapFromFileName(bitmapFileName);
		}
		
		/// <summary>
		/// Loads the bitmap.
		/// </summary>
		public Bitmap GetBitmapFromFileName(string fileName)
		{
			if (fileName != null) {
				return fileLoader.GetBitmap(fileName);
			}
			return null;
		}
		
		/// <summary>
		/// Gets a property value defined in the Wix document.
		/// </summary>
		/// <returns>The property value if it is found; an empty string otherwise.</returns>
		public string GetProperty(string name)
		{
			string xpath = String.Concat("//w:Property[@Id='", XmlEncode(name), "']");
			XmlElement textStyleElement = (XmlElement)SelectSingleNode(xpath, namespaceManager);
			if (textStyleElement != null) {
				return textStyleElement.InnerText;
			}
			return String.Empty;
		}
		
		/// <summary>
		/// Gets the preprocessor variable value from the WixProject.
		/// </summary>
		public string GetValue(string name)
		{
			if (project != null) {
				return project.GetVariable(name);
			}
			return null;
		}
		
		/// <summary>
		/// Gets the top SOURCEDIR directory.
		/// </summary>
		public WixDirectoryElement RootDirectory {
			get {
				string xpath = String.Concat("//w:Product/w:Directory[@Id='", WixDirectoryElement.RootDirectoryId, "']");
				return (WixDirectoryElement)SelectSingleNode(xpath, namespaceManager);
			}
		}
		
		/// <summary>
		/// Gets a reference to the root directory.
		/// </summary>
		public WixDirectoryRefElement RootDirectoryRef {
			get {
				string xpath = String.Concat("//w:DirectoryRef[@Id='", WixDirectoryElement.RootDirectoryId, "']");
				return (WixDirectoryRefElement)SelectSingleNode(xpath, namespaceManager);
			}
		}
		
		/// <summary>
		/// The Wix document contains the Product element.
		/// </summary>
		public bool IsProductDocument {
			get {
				return Product != null;
			}
		}
		
		/// <summary>
		/// Gets the prefix used for the Wix namespace.
		/// </summary>
		public string WixNamespacePrefix {
			get {
				XmlElement documentElement = DocumentElement;
				if (documentElement != null) {
					return documentElement.GetPrefixOfNamespace(WixNamespaceManager.Namespace);
				}
				return String.Empty;
			}
		}
		
		/// <summary>
		/// Adds a new root directory.
		/// </summary>
		public WixDirectoryElement AddRootDirectory()
		{
			// Add product element if it does not exist.	
			XmlElement productElement = Product;
			if (productElement == null) {
				productElement = CreateWixElement("Product");
				DocumentElement.AppendChild(productElement);
			}
			
			// Add root directory.
			WixDirectoryElement rootDirectory = WixDirectoryElement.CreateRootDirectory(this);
			return (WixDirectoryElement)productElement.AppendChild(rootDirectory);
		}
		
		/// <summary>
		/// Creates a new Xml element belonging to the Wix namespace.
		/// </summary>
		public XmlElement CreateWixElement(string name)
		{
			return CreateElement(WixNamespacePrefix, name, WixNamespaceManager.Namespace);
		}
		
		/// <summary>
		/// Saves the document to the location specified by WixDocument.FileName.
		/// </summary>
		public void Save(string lineTerminator, bool tabsToSpaces, int tabIndent)
		{
			XmlWriterSettings xmlWriterSettings = CreateXmlWriterSettings(lineTerminator, tabsToSpaces, tabIndent);
			using (XmlWriter xmlWriter = XmlTextWriter.Create(fileName, xmlWriterSettings)) {
				Save(xmlWriter);
			}
		}
		
		/// <summary>
		/// Gets the Product element.
		/// </summary>
		public XmlElement Product {
			get {
				return (XmlElement)SelectSingleNode("w:Wix/w:Product", namespaceManager);
			}
		}
		
		/// <summary>
		/// Gets the binary elements defined in this document.
		/// </summary>
		public WixBinaryElement[] GetBinaries()
		{
			List<WixBinaryElement> binaries = new List<WixBinaryElement>();
			foreach (WixBinaryElement element in SelectNodes("//w:Binary", namespaceManager)) {
				binaries.Add(element);
			}
			return binaries.ToArray();
		}
		
		/// <summary>
		/// Creates custom Wix elements for certain elements such as Directory and File.
		/// </summary>
		public override XmlElement CreateElement(string prefix, string localName, string namespaceURI)
		{
			if (namespaceURI == WixNamespaceManager.Namespace) {
				switch (localName) {
					case WixDirectoryElement.DirectoryElementName:
						return new WixDirectoryElement(this);
					case WixComponentElement.ComponentElementName:
						return new WixComponentElement(this);
					case WixFileElement.FileElementName:
						return new WixFileElement(this);
					case WixDirectoryRefElement.DirectoryRefElementName:
						return new WixDirectoryRefElement(this);
					case WixBinaryElement.BinaryElementName:
						return new WixBinaryElement(this);
				}
			}
			return base.CreateElement(prefix, localName, namespaceURI);
		}
		
		/// <summary>
		/// Checks to see if a File element exists with the specified id in this
		/// document.
		/// </summary>
		public bool FileIdExists(string id)
		{
			return ElementIdExists(WixFileElement.FileElementName, id);
		}
		
		/// <summary>
		/// Checks to see if a Component element exists with the specified id in this
		/// document.
		/// </summary>
		public bool ComponentIdExists(string id)
		{
			return ElementIdExists(WixComponentElement.ComponentElementName, id);
		}
		
		/// <summary>
		/// Returns the full path based on the location of 
		/// this WixDocument.
		/// </summary>
		public string GetFullPath(string relativePath)
		{
			if (fileName != null && fileName.Length > 0) {
				string basePath = Path.GetDirectoryName(fileName);
				return FileUtility.GetAbsolutePath(basePath, relativePath);
			}
			return relativePath;
		}
		
		/// <summary>
		/// Returns the relative path based on the location of 
		/// this WixDocument.
		/// </summary>
		public string GetRelativePath(string fullPath)
		{
			if (fileName != null && fileName.Length > 0) {
				string basePath = Path.GetDirectoryName(fileName);
				return FileUtility.GetRelativePath(basePath, fullPath);
			}
			return fullPath;
		}
		
		/// <summary>
		/// Reads the dialog id and adds it to the list of dialogs found so far.
		/// </summary>
		/// <param name="reader">An XmlReader which is currently at the dialog start element.</param>
		static void AddDialogId(XmlReader reader, List<string> dialogIds)
		{
			string id = GetIdFromCurrentNode(reader);
			if (id.Length > 0) {
				dialogIds.Add(id);
			}
		}
		
		/// <summary>
		/// Gets the id for the current element.
		/// </summary>
		/// <param name="reader">An XmlReader which is currently at the start element.</param>
		static string GetIdFromCurrentNode(XmlReader reader)
		{
			if (reader.MoveToAttribute("Id")) {
				if (reader.Value != null) {
					return reader.Value;
				}
			}
			return String.Empty;
		}
		
		/// <summary>
		/// Checks that the atomised element names match.
		/// </summary>
		static bool IsElementMatch(object elementName, object currentElementName)
		{
			return elementName == currentElementName;
		}
			
		/// <summary>
		/// Gets the line and column position if the dialog id matches the dialog
		/// element at the current reader position. This returns the column for the
		/// starting less than start tag.
		/// </summary>
		/// <param name="id">A dialog id.</param>
		/// <param name="reader">An XmlTextReader currently at the dialog start element.</param>
		/// <returns><see langword="null"/> if the dialog id does not match.</returns>
		static Location GetStartElementLocationIfMatch(XmlTextReader reader, string id)
		{
			// Store the column and line position since the call to GetDialogId will
			// move to the <Dialog> Id attribute.
			int line = reader.LineNumber - 1;	
			int column = reader.LinePosition - 2; // Take off 2 to so the '<' is located.

			if (id == GetIdFromCurrentNode(reader)) {
				return new Location(column, line);
			}
			return Location.Empty;
		}
		
		/// <summary>
		/// Determines the end of the end element including the final end tag marker
		/// (the greater than sign). This method moves the XmlTextReader to the end of
		/// the end tag.
		/// </summary>
		static Location GetEndElementEnd(XmlTextReader reader)
		{
			reader.ReadEndElement();
			int line = reader.LineNumber - 1;
			
			// Take off two as we have moved passed the end tag column.
			int column = reader.LinePosition - 2;
			
			// If ReadEndElement has moved to the start of another element
			// take off one from the column value otherwise the column
			// value includes the start tag of the next element.
			if (reader.NodeType == XmlNodeType.Element) {
				--column;
			}
			return new Location(column, line);
		}
		
		/// <summary>
		/// Determines the end of the empty element including the final end tag marker
		/// (the greater than sign). This method moves the XmlTextReader to the end
		/// of the element tag.
		/// </summary>
		static Location GetEmptyElementEnd(XmlTextReader reader)
		{
			reader.ReadStartElement();
			int line = reader.LineNumber - 1;
			
			// Take off two as we have moved passed the end tag
			// column.
			int column = reader.LinePosition - 2;
			return new Location(column, line);
		}
		
		/// <summary>
		/// Creates an XmlWriterSettings based on the text editor properties.
		/// </summary>
		static XmlWriterSettings CreateXmlWriterSettings(string lineTerminator, bool tabsToSpaces, int tabIndent)
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.CloseOutput = true;
			xmlWriterSettings.Indent = true;
			xmlWriterSettings.NewLineChars = lineTerminator;
			xmlWriterSettings.OmitXmlDeclaration = true;
			
			if (tabsToSpaces) {
				string spaces = " ";
				xmlWriterSettings.IndentChars = spaces.PadRight(tabIndent);
			} else {
				xmlWriterSettings.IndentChars = "\t";
			}
			return xmlWriterSettings;
		}
		
		/// <summary>
		/// Gets the dialog element with the specified id.
		/// </summary>
		XmlElement GetDialogElement(string id)
		{
			string xpath = String.Concat("//w:Dialog[@Id='", XmlEncode(id), "']");
			return (XmlElement)SelectSingleNode(xpath, namespaceManager);
		}
		
		/// <summary>
		/// Checks to see if an element exists with the specified Id attribute.
		/// </summary>
		bool ElementIdExists(string elementName, string id)
		{
			string xpath = String.Concat("//w:", elementName, "[@Id='", XmlEncode(id), "']");
			XmlNodeList nodes = SelectNodes(xpath, new WixNamespaceManager(NameTable));
			return nodes.Count > 0;
		}
		
		static string XmlEncode(string item)
		{
			char quoteChar = '\'';
			return XmlEncoder.Encode(item, quoteChar);
		}
	}
}
