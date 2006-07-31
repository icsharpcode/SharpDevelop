// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Document;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// A Wix document (.wxs or .wxi).
	/// </summary>
	public class WixDocument : IWixPropertyValueProvider
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
		
		XmlDocument document;
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
		/// Gets the line and column where the specified dialog element starts. The column
		/// returns is from the first character of the element name just inside the start
		/// tag.
		/// </summary>
		public static Location GetDialogStartElementLocation(TextReader reader, string dialogId)
		{
			XmlTextReader xmlReader = new XmlTextReader(reader);
			return GetDialogStartElementLocation(xmlReader, dialogId);			
		}
		
		/// <summary>
		/// Gets the line and column where the specified dialog element starts. The column
		/// returns is from the first character of the element name just inside the start
		/// tag.
		/// </summary>
		public static Location GetDialogStartElementLocation(XmlTextReader reader, string dialogId)
		{
			using (reader) {			
				object dialogElementName = reader.NameTable.Add("Dialog");
				while (reader.Read()) {
					switch (reader.NodeType) {
						case XmlNodeType.Element:
							if (IsElementMatch(dialogElementName, reader.LocalName)) {
								Location location = GetStartElementLocationIfMatch(reader, dialogId);
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
									return GetDialogIdFromCurrentNode(reader);
								} else if (reader.IsStartElement()) {
									dialogStartElement = new DialogStartElement(reader.LineNumber, GetDialogIdFromCurrentNode(reader));
								}
							}
							break;
						case XmlNodeType.EndElement:
							if (IsElementMatch(dialogElementName, reader.LocalName)) {
								if (line > dialogStartElement.Line && line <= reader.LineNumber) {
									return dialogStartElement.Id;
								} else {
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
		/// Gets the region (start line, column to end line, column) of the dialog xml
		/// element. This includes the start and end tags, the start column is the column
		/// containing the start tag and the end column is the column containing the final
		/// end tag marker.
		/// </summary>
		public static DomRegion GetDialogElementRegion(TextReader reader, string dialogId)
		{
			XmlTextReader xmlReader = new XmlTextReader(reader);
			return GetDialogElementRegion(xmlReader, dialogId);			
		}
		
		/// <summary>
		/// Gets the region (start line, column to end line, column) of the dialog xml
		/// element. This includes the start and end tags, the start column is the column
		/// containing the start tag and the end column is the column containing the final
		/// end tag marker.
		/// </summary>
		public static DomRegion GetDialogElementRegion(XmlTextReader reader, string dialogId)
		{
			Location startLocation = Location.Empty;
			
			using (reader) {			
				object dialogElementName = reader.NameTable.Add("Dialog");
				while (reader.Read()) {
					switch (reader.NodeType) {
						case XmlNodeType.Element:
							if (IsElementMatch(dialogElementName, reader.LocalName)) {
								bool isEmptyElement = reader.IsEmptyElement;
								startLocation = GetStartElementLocationIfMatch(reader, dialogId);
								if (!startLocation.IsEmpty && isEmptyElement) {
									Location endLocation = GetEmptyElementEnd(reader);
									return new DomRegion(startLocation, endLocation);							
								}
							}
							break;
						case XmlNodeType.EndElement:
							if (!startLocation.IsEmpty && IsElementMatch(dialogElementName, reader.LocalName)) {
								Location endLocation = GetEndElementEnd(reader);
								return new DomRegion(startLocation, endLocation);
							}
							break;
					}
				}
			}
			return DomRegion.Empty;
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
		/// Loads the Wix document.
		/// </summary>
		public void LoadXml(string xml)
		{
			document = new XmlDocument();
			document.LoadXml(xml);
			namespaceManager = new WixNamespaceManager(document.NameTable);
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
			string xpath = String.Concat("//w:Dialog[@Id='", id, "']");
			XmlElement dialogElement = (XmlElement)document.SelectSingleNode(xpath, namespaceManager);
			if (dialogElement != null) {
				return new WixDialog(this, dialogElement);
			}
			return null;
		}
		
		/// <summary>
		/// Gets the binary filename for the specified id.
		/// </summary>
		/// <returns><see langword="null"/> if the id cannot be found.</returns>
		public string GetBinaryFileName(string id)
		{
			string xpath = String.Concat("//w:Binary[@Id='", id, "']");
			XmlElement binaryElement = (XmlElement)document.SelectSingleNode(xpath, namespaceManager);
			if (binaryElement != null) {
				string binaryFileName;
				if (binaryElement.HasAttribute("SourceFile")) {
					binaryFileName = binaryElement.GetAttribute("SourceFile");
				} else {
					binaryFileName = binaryElement.GetAttribute("src");
				}
				
				binaryFileName = WixPropertyParser.Parse(binaryFileName, this);
				
				// If we have the Wix document filename return the full filename.
				if (fileName.Length > 0) {
					return Path.Combine(Path.GetDirectoryName(fileName), binaryFileName);
				}
				return binaryFileName;
			}
			return null;
		}
		
		/// <summary>
		/// Loads the bitmap using the Wix Bitmap id.
		/// </summary>
		public Bitmap GetBitmapFromId(string id)
		{
			string bitmapFileName = GetBinaryFileName(id);
			if (bitmapFileName != null) {
				return fileLoader.GetBitmap(bitmapFileName);
			}
			return null;
		}
		
		/// <summary>
		/// Gets a property value defined in the Wix document.
		/// </summary>
		/// <returns>The property value if it is found; an empty string otherwise.</returns>
		public string GetProperty(string name)
		{
			string xpath = String.Concat("//w:Property[@Id='", name, "']");
			XmlElement textStyleElement = (XmlElement)document.SelectSingleNode(xpath, namespaceManager);
			if (textStyleElement != null) {
				return textStyleElement.InnerText;
			}
			return String.Empty;
		}
		
		/// <summary>
		/// Gets the variable value from the WixProject.
		/// </summary>
		public string GetValue(string name)
		{
			return project.GetVariable(name);
		}
		
		/// <summary>
		/// Reads the dialog id and adds it to the list of dialogs found so far.
		/// </summary>
		/// <param name="reader">An XmlReader which is currently at the dialog start element.</param>
		static void AddDialogId(XmlReader reader, List<string> dialogIds)
		{
			string id = GetDialogIdFromCurrentNode(reader);
			if (id.Length > 0) {
				dialogIds.Add(id);
			}
		}
		
		/// <summary>
		/// Gets the dialog id for the current dialog element.
		/// </summary>
		/// <param name="reader">An XmlReader which is currently at the dialog start element.</param>
		static string GetDialogIdFromCurrentNode(XmlReader reader)
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

			if (id == GetDialogIdFromCurrentNode(reader)) {
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
			
			// Take off two as we have moved passed the end tag
			// column.
			int column = reader.LinePosition - 2; 
			
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
	}
}
