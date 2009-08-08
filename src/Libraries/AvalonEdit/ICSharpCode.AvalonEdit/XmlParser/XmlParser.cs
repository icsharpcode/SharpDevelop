// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.XmlParser
{
	/// <summary>
	/// Creates object tree from XML document.
	/// </summary>
	/// <remarks>
	/// The created tree fully describes the document and thus the orginal XML file can be
	/// exactly reproduced.
	/// 
	/// Any further parses will reparse only the changed parts and the existing three will
	/// be updated with the changes.  The user can add event handlers to be notified of
	/// the changes.  The parser tries to minimize the number of changes to the tree.
	/// (for example, it will add a single child at the start of collection rather than
	/// clearing the collection and adding new children)
	/// 
	/// The object tree consists of following types:
	///   RawObject - Abstact base class for all types
	///     RawContainer - Abstact base class for all types that can contain child nodes
	///       RawDocument - The root object of the XML document
	///       RawElement - Logical grouping of other nodes together.  The first child is always the start tag.
	///       RawTag - Represents any markup starting with "&lt;" and (hopefully) ending with ">"
	///     RawAttribute - Name-value pair in a tag
	///     RawText - Whitespace or character data
	/// 
	/// For example, see the following XML and the produced object tree:
	/// <![CDATA[
	///   <!-- My favourite quote -->
	///   <quote author="Albert Einstein">
	///     Make everything as simple as possible, but not simpler.
	///   </quote>
	/// 
	///   RawDocument
	///     RawTag "<!--" "-->"
	///       RawText " My favourite quote "
	///     RawElement
	///       RawTag "<" "quote" ">"
	///         RawText " "
	///         RawAttribute 'author="Albert Einstein"'
	///       RawText "\n  Make everything as simple as possible, but not simpler.\n"
	///       RawTag "</" "quote" ">"
	/// ]]>
	/// 
	/// The precise content of RawTag depends on what it represents:
	/// <![CDATA[
	///   Start tag:  "<"  Name?  (RawText+ RawAttribute)* RawText* (">" | "/>")
	///   End tag:    "</" Name?  (RawText+ RawAttribute)* RawText* ">"
	///   P.instr.:   "<?" Name?  (RawText)* "?>"
	///   Comment:    "<!--"      (RawText)* "-->"
	///   CData:      "<![CDATA[" (RawText)* "]]" ">"
	///   DTD:        "<!DOCTYPE" (RawText+ RawTag)* RawText* ">"    (DOCTYPE or other DTD names)
	///   UknownBang: "<!"        (RawText)* ">"
	/// ]]>
	/// 
	/// The type of tag can be identified by the opening backet.
	/// There are helpper properties in the RawTag class to identify the type, exactly
	/// one of the properties will be true.
	/// 
	/// The closing bracket may be missing or may be different for mallformed XML.
	/// 
	/// Note that there can always be multiple consequtive RawText nodes.
	/// This is to ensure that idividual texts are not too long.
	/// 
	/// XML Spec:  http://www.w3.org/TR/xml/
	/// XML EBNF:  http://www.jelks.nu/XML/xmlebnf.html
	/// 
	/// Internals:
	/// 
	/// "Try" methods can silently fail by returning false.
	/// MoveTo methods do not move if they are already at the given target
	/// If methods return some object, it must be no-empty.  It is up to the caller to ensure
	/// the context is appropriate for reading.
	/// 
	/// </remarks>
	public class XmlParser
	{
		// TODO: Simple tag matching heuristic
		// TODO: Delete some read functions and optimize performance
		// TODO: Rewrite ReadText
		
		RawDocument userDocument;
		TextDocument textDocument;
		
		List<DocumentChangeEventArgs> changesSinceLastParse = new List<DocumentChangeEventArgs>();
		
		/// <summary> Previously parsed items as long as they are valid  </summary>
		TextSegmentCollection<RawObject> parsedItems = new TextSegmentCollection<RawObject>();
		
		/// <summary>
		/// Is used to identify what memory range was touched by object
		/// The default is (StartOffset, EndOffset + 1) which is not stored
		/// </summary>
		TextSegmentCollection<TouchedMemoryRange> touchedMemoryRanges = new TextSegmentCollection<TouchedMemoryRange>();
		
		class TouchedMemoryRange: TextSegment
		{
			public RawObject TouchedByObject { get; set; }
		}
		
		/// <summary>
		/// All syntax errors in the user document
		/// </summary>
		public TextSegmentCollection<SyntaxError> SyntaxErrors { get; private set; }
		
		/// <summary> Information about syntax error that occured during parsing </summary>
		public class SyntaxError: TextSegment
		{
			/// <summary> Object for which the error occured </summary>
			public RawObject Object { get; internal set; }
			/// <summary> Textual description of the error </summary>
			public string Message { get; internal set; }
			/// <summary> Any user data </summary>
			public object Tag { get; set; }
			
			internal SyntaxError Clone(RawObject newOwner)
			{
				return new SyntaxError {
					Object = newOwner,
					Message = Message,
					Tag = Tag,
					StartOffset = StartOffset,
					EndOffset = EndOffset,
				};
			}
		}
		
		/// <summary>
		/// Create new parser, but do not parse the text yet.
		/// </summary>
		public XmlParser(string input)
		{
			this.input = input;
			this.userDocument = new RawDocument();
			this.SyntaxErrors = new TextSegmentCollection<SyntaxError>();
			userDocument.ObjectAttached += delegate(object sender, RawObjectEventArgs e) {
				foreach(SyntaxError error in e.Object.SyntaxErrors) {
					this.SyntaxErrors.Add(error);
				}
			};
			userDocument.ObjectDettached += delegate(object sender, RawObjectEventArgs e) {
				foreach(SyntaxError error in e.Object.SyntaxErrors) {
					this.SyntaxErrors.Remove(error);
				}
			};
			userDocument.ObjectChanged += delegate(object sender, RawObjectEventArgs e) {
				foreach(SyntaxError error in this.SyntaxErrors.ToList()) {
					if (error.Object == e.Object) {
						this.SyntaxErrors.Remove(error);
					}
				}
				foreach(SyntaxError error in e.Object.SyntaxErrors) {
					this.SyntaxErrors.Add(error);
				}
			};
		}
		
		/// <summary>
		/// Create new parser, but do not parse the text yet.
		/// </summary>
		public XmlParser(TextDocument textDocument): this(textDocument.Text)
		{
			this.textDocument = textDocument;
			this.textDocument.Changed += delegate(object sender, DocumentChangeEventArgs e) {
				changesSinceLastParse.Add(e);
			};
		}
		
		/// <summary>
		/// Incrementaly parse the document
		/// </summary>
		public RawDocument Parse()
		{
			// Update source text
			if (textDocument != null) {
				input = textDocument.Text;
			}
			
			// Use chages to invalidate cache
			foreach(DocumentChangeEventArgs change in changesSinceLastParse) {
				// Update offsets of all items
				parsedItems.UpdateOffsets(change);
				touchedMemoryRanges.UpdateOffsets(change);
				this.SyntaxErrors.UpdateOffsets(change);
				
				// Remove any items affected by the change
				Log("Changed offset {0}", change.Offset);
				// Removing will cause one of the end to be set to change.Offset
				// FindOverlappingSegments apparently removes any segment intersecting of touching
				// so that conviniently takes care of the +1 byte
				foreach(RawObject obj in parsedItems.FindOverlappingSegments(change.Offset, 0)) {
					parsedItems.Remove(obj);
					Log("Removed cached item {0}", obj);
				}
				foreach(TouchedMemoryRange memory in touchedMemoryRanges.FindOverlappingSegments(change.Offset, 0)) {
					parsedItems.Remove(memory.TouchedByObject);
					touchedMemoryRanges.Remove(memory);
					Log("Removed cached item {0} - depended on memory ({1}-{2})", memory.TouchedByObject, memory.StartOffset, memory.EndOffset);
				}
			}
			changesSinceLastParse.Clear();
			
			currentLocation = 0;
			maxTouchedLocation = 0;
			readingEnd = input.Length;
			
			RawDocument parsedDocument = ReadDocument();
			// Just in case parse method was called redundantly
			PrintStringCacheStats();
			RawObject.LogDom("Updating main DOM tree...");
			userDocument.UpdateDataFrom(parsedDocument);
			userDocument.CheckLinksConsistency();
			return userDocument;
		}
		
		bool TryReadFromCacheOrNew<T>(out T res) where T: RawObject, new()
		{
			return TryReadFromCacheOrNew<T>(x => true, out res);
		}
		
		bool TryReadFromCacheOrNew<T>(Predicate<T> conditon, out T res) where T: RawObject, new()
		{
			RawObject obj = parsedItems.FindFirstSegmentWithStartAfter(currentLocation);
			while(obj != null && obj.StartOffset == currentLocation) {
				if (obj is T && conditon((T)obj)) {
					currentLocation += obj.Length;
					res = (T)obj;
					return true;
				}
				obj = parsedItems.GetNextSegment(obj);
			}
			res = new T();
			return false;
		}
		
		int GetStartOfCachedObject<T>(Predicate<T> conditon, int offset, int lookaheadCount) where T: RawObject
		{
			RawObject obj = parsedItems.FindFirstSegmentWithStartAfter(offset);
			// Recheck the offset!
			while(obj != null && offset <= obj.StartOffset && obj.StartOffset <= offset + lookaheadCount) {
				if (obj is T && conditon((T)obj)) {
					return obj.StartOffset;
				}
				obj = parsedItems.GetNextSegment(obj);
			}
			return -1;
		}
		
		void OnParsed(RawObject obj)
		{
			if (obj.Length == 0 && !(obj is RawDocument)) {
				throw new Exception(string.Format("Could not parse {0}.  It has zero length.", obj));
			}
			parsedItems.Add(obj);
			Log("Parsed {0}", obj);
			if (maxTouchedLocation > currentLocation) {
				// location is assumed to be read so the range ends at (location + 1)
				// For example eg for "a_" it is (0-2)
				TouchedMemoryRange memRange = new TouchedMemoryRange() {
					StartOffset = obj.StartOffset,
					Length = (maxTouchedLocation + 1 - obj.StartOffset),
					TouchedByObject = obj
				};
				touchedMemoryRanges.Add(memRange);
				Log(" - Touched memory range: ({0}-{1})", memRange.StartOffset, memRange.EndOffset);
			}
		}
		
		void Log(string text, params object[] pars)
		{
			System.Diagnostics.Debug.WriteLine(string.Format("XML Parser: " + text, pars));
		}
		
		Dictionary<string, string> stringCache = new Dictionary<string, string>();
		int stringCacheRequestedCount;
		int stringCacheRequestedSize;
		int stringCacheSavedCount;
		int stringCacheSavedSize;
		
		string GetCachedString(string cached)
		{
			stringCacheRequestedCount += 1;
			stringCacheRequestedSize += 8 + 2 * cached.Length;
			// Do not bother with long strings
			//if (cached.Length <= 32) return cached;
			if (stringCache.ContainsKey(cached)) {
				// Get the instance from the cache instead
				stringCacheSavedCount += 1;
				stringCacheSavedSize += 8 + 2 * cached.Length;
				return stringCache[cached];
			} else {
				// Add to cache
				stringCache.Add(cached, cached);
				return cached;
			}
		}
		
		void PrintStringCacheStats()
		{
			Log("String cache: Requested {0} ({1} bytes);  Saved {2} ({3} bytes); {4}% Saved", stringCacheRequestedCount, stringCacheRequestedSize, stringCacheSavedCount, stringCacheSavedSize, stringCacheRequestedSize == 0 ? 0 : stringCacheSavedSize * 100 / stringCacheRequestedSize);
		}
		
		void OnSyntaxError(RawObject obj, string message, params object[] args)
		{
			OnSyntaxError(obj, currentLocation, currentLocation + 1, message, args);
		}
		
		void OnSyntaxError(RawObject obj, int start, int end, string message, params object[] args)
		{
			if (end <= start) end = start + 1;
			Log("Syntax error ({0}-{1}): {2}", start, end, string.Format(message, args));
			obj.AddSyntaxError(new SyntaxError() {
			                   	Object = obj,
			                   	StartOffset = start,
			                   	EndOffset = end,
			                   	Message = string.Format(message, args),
			                   });
		}
		
		string input;
		int    readingEnd;
		// Do not ever set the value from parsing methods
		// most importantly do not backtrack except with GoBack(int)
		int    currentLocation;
		
		// CurrentLocation is assumed to be touched and that fact does not
		// have to be recorded in this variable
		// This stores any value bigger then that if applicable
		// acutal value is max(currentLocation, maxTouchedLocation)
		int    maxTouchedLocation;
		
		bool IsEndOfFile()
		{
			return currentLocation == readingEnd;
		}
		
		bool HasMoreData()
		{
			return currentLocation < readingEnd;
		}
		
		void AssertHasMoreData()
		{
			if (currentLocation == readingEnd) {
				throw new Exception("Unexpected end of files");
			}
		}
		
		bool TryMoveNext()
		{
			if (currentLocation == readingEnd) return false;
			
			currentLocation++;
			return true;
		}
		
		void GoBack(int oldLocation)
		{
			if (oldLocation > currentLocation) throw new Exception("Trying to move forward");
			maxTouchedLocation = Math.Max(maxTouchedLocation, currentLocation);
			currentLocation = oldLocation;
		}
		
		bool TryRead(char c)
		{
			if (currentLocation == readingEnd) return false;
			
			if (input[currentLocation] == c) {
				currentLocation++;
				return true;
			} else {
				return false;
			}
		}
		
		bool TryRead(string text)
		{
			if (TryPeek(text)) {
				currentLocation += text.Length;
				return true;
			} else {
				return false;
			}
		}
		
		/// <summary> Returns true if at least one character was read </summary>
		bool TryReadPartOf(string text)
		{
			if (TryPeek(text[0])) {
				// Keep reading until character differs or we have end of file
				foreach(char c in text) if (!TryRead(c)) break;
				return true;
			} else {
				return false;
			}
		}
		
		bool TryPeekPrevious(char c, int back)
		{
			if (currentLocation - back == readingEnd) return false;
			if (currentLocation - back < 0 ) return false;
			
			return input[currentLocation - back] == c;
		}
		
		bool TryPeek(char c)
		{
			if (currentLocation == readingEnd) return false;
			
			return input[currentLocation] == c;
		}
		
		bool TryPeekAnyOf(params char[] chars)
		{
			if (currentLocation == readingEnd) return false;
			
			return chars.Contains(input[currentLocation]);
		}
		
		bool TryPeek(string text)
		{
			if (!TryPeek(text[0])) return false; // Early exit
			
			maxTouchedLocation = Math.Max(maxTouchedLocation, currentLocation + (text.Length - 1));
			// The following comparison 'touches' the end of file
			if (currentLocation + text.Length > readingEnd) return false;
			
			return input.Substring(currentLocation, text.Length) == text;
		}
		
		bool TryMoveTo(char c)
		{
			while(true) {
				if (currentLocation == readingEnd) return false;
				if (input[currentLocation] == c) return true;
				currentLocation++;
			}
		}
		
		bool TryMoveTo(string text)
		{
			while(true) {
				if (!TryMoveTo(text[0])) return false; // End of file
				if (TryPeek(text)) return true;
				currentLocation++;
			}
		}
		
		bool TryMoveToAnyOf(params char[] c)
		{
			while(true) {
				if (currentLocation == readingEnd) return false;
				if (c.Contains(input[currentLocation])) return true;
				currentLocation++;
			}
		}
		
		bool TryMoveToNonWhiteSpace()
		{
			while (TryPeekWhiteSpace()) TryMoveNext();
			return HasMoreData();
		}
		
		string GetText(int start, int end)
		{
			if (start == readingEnd && end == readingEnd) {
				return string.Empty;
			} else {
				return GetCachedString(input.Substring(start, end - start));
			}
		}
		
		static char[] WhiteSpaceChars = new char[] {' ', '\n', '\r', '\t'};
		static char[] WhiteSpaceAndReservedChars = new char[] {' ', '\n', '\r', '\t', '=', '\'', '"', '<', '>', '/', '?'};
		
		bool TryPeekWhiteSpace()
		{
			if (currentLocation == readingEnd) return false;
			
			return WhiteSpaceChars.Contains(input[currentLocation]);
		}
		
		bool TryPeekNameChar()
		{
			if (currentLocation == readingEnd) return false;
			
			return !WhiteSpaceAndReservedChars.Contains(input[currentLocation]);
		}
		
		/// <summary>
		/// Read a name token.
		/// The following characters are not allowed:
		///   ""         End of file
		///   " \n\r\t"  Whitesapce
		///   "=\'\""    Attribute value
		///   "&lt;"     Openning Tag
		///   ">/?"      Closing Tag
		/// </summary>
		bool TryReadName(out string res)
		{
			int start = currentLocation;
			TryMoveToAnyOf(WhiteSpaceAndReservedChars.ToArray());
			if (start == currentLocation) {
				res = null;
				return false;
			} else {
				res = GetText(start, currentLocation);
				// TODO: Check that it is valid XML name
				return true;
			}
		}
		
		bool IsValidName(string name)
		{
			try {
				System.Xml.XmlConvert.VerifyName(name);
				return true;
			} catch (System.Xml.XmlException) {
				return false;
			}
		}
		
		/// <summary>
		/// Context: any
		/// </summary>
		RawDocument ReadDocument()
		{
			RawDocument doc;
			if (TryReadFromCacheOrNew(out doc)) return doc;
			doc.IsParsed = true;
			
			// TODO: Errors in document structure
			doc.StartOffset = currentLocation;
			while(true) {
				if (IsEndOfFile()) {
					break;
				} else if (TryPeek('<')) {
					doc.AddChild(ReadElementOrTag());
				} else {
					doc.AddChildren(ReadText(RawTextType.CharacterData));
				}
			}
			doc.EndOffset = currentLocation;
			
			OnParsed(doc);
			return doc;
		}
		
		/// <summary>
		/// Context: "&lt;"
		/// </summary>
		RawObject ReadElementOrTag()
		{
			AssertHasMoreData();
			
			if (TryPeek("<!") || TryPeek("</") || TryPeek("<?")) {
				return ReadTag();
			} else if (TryPeek('<')) {
				return ReadElement();
			} else {
				throw new Exception("'<' expected");
			}
		}
		
		/// <summary>
		/// Context: "&lt;"
		/// </summary>
		RawElement ReadElement()
		{
			AssertHasMoreData();
			
			RawElement element;
			if (TryReadFromCacheOrNew(out element)) return element;
			
			element.StartOffset = currentLocation;
			// Read start tag
			element.AddChild(ReadTag());
			Debug.Assert(element.StartTag.IsStartTag);
			// Read content and end tag (only if properly closed)
			if (element.StartTag.ClosingBracket == ">") {
				while(true) {
					if (IsEndOfFile()) {
						OnSyntaxError(element, "Closing tag for '{0}' expected", element.StartTag.Name);
						break;
					} else if (TryPeek('<')) {
						RawObject content = ReadElementOrTag();
						element.AddChild(content);
						RawTag endTag = content as RawTag;
						if (endTag != null && endTag.IsEndTag) {
							if (endTag.Name != element.StartTag.Name) {
								OnSyntaxError(element, endTag.StartOffset + 2, endTag.StartOffset + 2 + endTag.Name.Length, "Name '{0}' expected.  End tag must have same name as start tag.", element.StartTag.Name);
							}
							break;
						}
					} else {
						element.AddChildren(ReadText(RawTextType.CharacterData));
					}
				}
			}
			element.EndOffset = currentLocation;
			
			OnParsed(element);
			return element;
		}
		
		/// <summary>
		/// Context: "&lt;"
		/// </summary>
		RawTag ReadTag()
		{
			AssertHasMoreData();
			
			RawTag tag;
			if (TryReadFromCacheOrNew(out tag)) return tag;
			
			tag.StartOffset = currentLocation;
			
			// Read the opening bracket
			// It identifies the type of tag and parsing behavior for the rest of it
			tag.OpeningBracket = ReadOpeningBracket();
			
			if (tag.IsStartTag || tag.IsEndTag || tag.IsProcessingInstruction) {
				// Read the name
				string name;
				if (TryReadName(out name)) {
					tag.Name = name;
					if (!IsValidName(tag.Name)) {
						OnSyntaxError(tag, currentLocation - tag.Name.Length, currentLocation, "The name '{0}' is invalid", tag.Name);
					}
				} else {
					OnSyntaxError(tag, "Element name expected");
				}
			}
			
			if (tag.IsStartTag || tag.IsEndTag) {
				// Read attributes for the tag
				while(true) {					
					// Chech for all forbiden 'name' charcters first - see ReadName
					if (IsEndOfFile()) break;
					if (TryPeekWhiteSpace()) {
						tag.AddChildren(ReadText(RawTextType.WhiteSpace));
						continue;  // End of file might be next
					}
					if (TryPeek('<')) break;
					if (TryPeek('>') || TryPeek('/') || TryPeek('?')) break;  // End tag
					
					// We have "=\'\"" or name - read attribute
					tag.AddChild(ReadAttribulte());
				}
			} else if (tag.IsDocumentType) {
				tag.AddChildren(ReadContentOfDTD());
			} else {
				int start = currentLocation;
				IEnumerable<RawObject> text;
				if (tag.IsComment) {
					text = ReadText(RawTextType.Comment);
				} else if (tag.IsCData) {
					text = ReadText(RawTextType.CData);
				} else if (tag.IsProcessingInstruction) {
					text = ReadText(RawTextType.ProcessingInstruction);
				} else if (tag.IsUnknownBang) {
					text = ReadText(RawTextType.UnknownBang);
				} else {
					throw new Exception(string.Format("Unknown opening bracket '{0}'", tag.OpeningBracket));
				}
				// Enumerate
				text = text.ToList();
				// Backtrack at complete start
				if (IsEndOfFile() || (tag.IsUnknownBang && TryPeek('<'))) {
					GoBack(start);
				} else {
					tag.AddChildren(text);
				}
			}
			
			// Read closing bracket
			string bracket;
			if (TryReadClosingBracket(out bracket)) {
				tag.ClosingBracket = bracket;
			}
			
			// Error check
			int brStart = currentLocation - (tag.ClosingBracket ?? string.Empty).Length;
			if (tag.Name == null) {
				// One error was reported already
			} else if (tag.IsStartTag) {
				if (tag.ClosingBracket != ">" && tag.ClosingBracket != "/>") OnSyntaxError(tag, brStart, currentLocation, "'>' or '/>' expected");
			} else if (tag.IsEndTag) {
				if (tag.ClosingBracket != ">") OnSyntaxError(tag, brStart, currentLocation, "'>' expected");
			} else if (tag.IsComment) {
				if (tag.ClosingBracket != "-->") OnSyntaxError(tag, brStart, currentLocation, "'-->' expected");
			} else if (tag.IsCData) {
				if (tag.ClosingBracket != "]]>") OnSyntaxError(tag, brStart, currentLocation, "']]>' expected");
			} else if (tag.IsProcessingInstruction) {
				if (tag.ClosingBracket != "?>") OnSyntaxError(tag, brStart, currentLocation, "'?>' expected");
			} else if (tag.IsUnknownBang) {
				if (tag.ClosingBracket != ">") OnSyntaxError(tag, brStart, currentLocation, "'>' expected");
			} else if (tag.IsDocumentType) {
				if (tag.ClosingBracket != ">") OnSyntaxError(tag, brStart, currentLocation, "'>' expected");
			} else {
				throw new Exception(string.Format("Unknown opening bracket '{0}'", tag.OpeningBracket));
			}
			
			// Attribute name may not apper multiple times
			var duplicates = tag.Children.OfType<RawAttribute>().GroupBy(attr => attr.Name).SelectMany(g => g.Skip(1));
			foreach(RawAttribute attr in duplicates) {
				OnSyntaxError(tag, attr.StartOffset, attr.EndOffset, "Attribute with name '{0}' already exists", attr.Name);
			}
			
			tag.EndOffset = currentLocation;
			
			OnParsed(tag);
			return tag;
		}
		
		/// <summary>
		/// Reads any of the know opening brackets.  (only full bracket)
		/// Context: "&lt;"
		/// </summary>
		string ReadOpeningBracket()
		{
			// We are using a lot of string literals so that the memory instances are shared
			int start = currentLocation;
			if (TryRead('<')) {
				if (TryRead('/')) {
					return "</";
				} else if (TryRead('?')) {
					return "<?";
				} else if (TryRead('!')) {
					if (TryRead("--")) {
						return "<!--";
					} else if (TryRead("[CDATA[")) {
						return "<![CDATA[";
					} else {
						foreach(string dtdName in RawTag.DTDNames) {
							// the dtdName includes "<!"
							if (TryRead(dtdName.Remove(0, 2))) return dtdName;
						}
						return "<!";
					}
				} else {
					return "<";
				}
			} else {
				throw new Exception("'<' expected");
			}
		}
		
		/// <summary>
		/// Reads any of the know closing brackets.  (only full bracket)
		/// Context: any
		/// </summary>
		bool TryReadClosingBracket(out string bracket)
		{
			// We are using a lot of string literals so that the memory instances are shared
			int start = currentLocation;
			if (TryRead('>')) {
				bracket = ">";
			} else 	if (TryRead("/>")) {
				bracket = "/>";
			} else 	if (TryRead("?>")) {
				bracket = "?>";
			} else if (TryRead("-->")) {
				bracket = "-->";
			} else if (TryRead("]]>")) {
				bracket = "]]>";
			} else {
				bracket = null;
				return false;
			}
			return true;
		}
		
		IEnumerable<RawObject> ReadContentOfDTD()
		{
			int start = currentLocation;
			while(true) {
				if (IsEndOfFile()) break;            // End of file
				TryMoveToNonWhiteSpace();            // Skip whitespace
				if (TryRead('\'')) TryMoveTo('\'');  // Skip single quoted string
				if (TryRead('\"')) TryMoveTo('\"');  // Skip single quoted string
				if (TryRead('[')) {                  // Start of nested infoset
					// Reading infoset
					while(true) {
						if (IsEndOfFile()) break;
						TryMoveToAnyOf('<', ']');
						if (TryPeek('<')) {
							if (start != currentLocation) {  // Two following tags
								yield return MakeText(start, currentLocation);
							}
							yield return ReadTag();
							start = currentLocation;
						}
						if (TryPeek(']')) break;
					}
				}
				TryRead(']');                        // End of nested infoset
				if (TryPeek('>')) break;             // Proper closing
				if (TryPeek('<')) break;             // Malformed XML
				TryMoveNext();                       // Skip anything else
			}
			if (start != currentLocation) {
				yield return MakeText(start, currentLocation);
			}
		}
		
		/// <summary>
		/// Context: name or "=\'\""
		/// </summary>
		RawAttribute ReadAttribulte()
		{
			AssertHasMoreData();
			
			RawAttribute attr;
			if (TryReadFromCacheOrNew(out attr)) return attr;
			
			attr.StartOffset = currentLocation;
			
			// Read name
			string name;
			if (TryReadName(out name)) {
				attr.Name = name;
				if (!IsValidName(attr.Name)) {
					OnSyntaxError(attr, attr.StartOffset, currentLocation, "The name '{0}' is invalid", attr.Name);
				}
			} else {
				OnSyntaxError(attr, "Attribute name expected");
			}
			
			// Read equals sign and surrounding whitespace
			int checkpoint = currentLocation;
			TryMoveToNonWhiteSpace();
			if (TryRead('=')) {
				int chk2 = currentLocation;
				TryMoveToNonWhiteSpace();
				if (!TryPeek('"') && !TryPeek('\'')) {
					// Do not read whitespace if quote does not follow
					GoBack(chk2);
				}
				attr.EqualsSign = GetText(checkpoint, currentLocation);
			} else {
				GoBack(checkpoint);
				OnSyntaxError(attr, "'=' expected");
			}
			
			// Read attribute value
			int start = currentLocation;
			char quoteChar = TryPeek('"') ? '"' : '\'';
			if (TryRead(quoteChar)) {
				int valueStart = currentLocation;
				TryMoveToAnyOf(quoteChar, '<');
				if (TryRead(quoteChar)) {
					if (!TryPeekAnyOf(' ', '\t', '\n', '\r', '/', '>', '?')) {
						if (TryPeekPrevious('=', 2) || (TryPeekPrevious('=', 3) && TryPeekPrevious(' ', 2))) {
							// This actually most likely means that we are in the next attribute value
							GoBack(valueStart);
							ReadAttributeValue(quoteChar);
							if (TryRead(quoteChar)) {
								OnSyntaxError(attr, "White space or end of tag expected");
							} else {
								OnSyntaxError(attr, "Quote {0} expected (or add whitespace after the following one)", quoteChar);
							}
						} else {
							OnSyntaxError(attr, "White space or end of tag expected");
						}
					}
				} else {
					// '<' or end of file
					GoBack(valueStart);
					ReadAttributeValue(quoteChar);
					OnSyntaxError(attr, "Quote {0} expected", quoteChar);
				}
			} else {
				int valueStart = currentLocation;
				ReadAttributeValue(null);
				TryRead('\"');
				TryRead('\'');
				if (valueStart == currentLocation) {
					OnSyntaxError(attr, "Attribute value expected");
				} else {
					OnSyntaxError(attr, valueStart, currentLocation, "Attribute value must be quoted");
				}
			}
			attr.QuotedValue = GetText(start, currentLocation);
			
			attr.EndOffset = currentLocation;
			
			OnParsed(attr);
			return attr;
		}
		
		/// <summary>
		/// Read everything up to quote (excluding), opening/closing tag or attribute signature
		/// </summary>
		void ReadAttributeValue(char? quote)
		{
			while(true) {
				if (IsEndOfFile()) return;
				// What is next?
				int start = currentLocation;
				TryMoveToNonWhiteSpace();  // Read white space (if any)
				if (quote.HasValue) {
					if (TryPeek(quote.Value)) return;
				} else {
					if (TryPeek('"') || TryPeek('\'')) return;
				}
				// Opening/closing tag
				if (TryPeekAnyOf('<', '/', '>')) {
					GoBack(start);
					return;
				}
				// Try reading attribute signature
				string name;
				if (TryReadName(out name)) {
					int nameEnd = currentLocation;
					if (TryMoveToNonWhiteSpace() && TryPeek("=")) {
						// Start of attribute.  Great
						GoBack(start);
						return;  // Done
					} else {
						// Just some gargabe - make it part of the value
						GoBack(nameEnd);
						continue;  // Read more
					}
				}
				TryMoveNext(); // Accept everyting else
			}
		}
		
		RawText MakeText(int start, int end)
		{
			RawText text = new RawText() {
				StartOffset = start,
				EndOffset = end,
				Value = GetText(start, end),
				Type = RawTextType.Other
			};
			
			OnParsed(text);
			return text;
		}
		
		const int maxEntityLenght = 12; // The longest build-in one is 10 ("&#x10FFFF;")
		const int maxTextFragmentSize = 8;
		const int lookAheadLenght = (3 * maxTextFragmentSize) / 2;
		const int backtrackLenght = 4;  // 2: get back over "]]"   1: so that we have some data   1: safety
		
		/// <summary>
		/// Reads text and optionaly separates it into fragments.
		/// It can also return empty set for no appropriate text input.
		/// Make sure you enumerate it only once
		/// </summary>
		IEnumerable<RawObject> ReadText(RawTextType type)
		{
			// TODO: Rewrite
			
			bool lookahead = false;
			while(true) {
				RawText text;
				if (TryReadFromCacheOrNew(t => t.Type == type, out text)) {
					// Cached text found
					yield return text;
					continue; // Read next fragment;  the method can handle "no text left"
				}
				text.Type = type;
				
				// Limit the reading to just a few characters
				int realReadingEnd = readingEnd;
				readingEnd = Math.Min(realReadingEnd, currentLocation + maxTextFragmentSize);
				
				// Look if some futher text has been already processed and align so that
				// we hit that chache point.  It is expensive so it is off for the first run
				if (lookahead) {
					int nextFragmentIndex = GetStartOfCachedObject<RawText>(t => t.Type == type, currentLocation, lookAheadLenght);
					if (nextFragmentIndex != -1) {
						// Consider adding "aaa]" before cached fragment "]>bbb"
						// We must not use cache then - so the overshoot acutally makes sense
						readingEnd = Math.Min(realReadingEnd, nextFragmentIndex + backtrackLenght);
						Log("Parsing only text ({0}-{1}) because later text was already processed", currentLocation, readingEnd);
					}
				}
				lookahead = true;
				
				text.StartOffset = currentLocation;
				int start = currentLocation;
				
				// Try move to the terminator given by the context
				if (type == RawTextType.WhiteSpace) {
					TryMoveToNonWhiteSpace();
				} else if (type == RawTextType.CharacterData) {
					while(true) {
						TryMoveToAnyOf('<', ']');
						if (IsEndOfFile()) break;
						if (TryPeek('<')) break;
						if (TryPeek(']')) {
							if (TryPeek("]]>")) {
								OnSyntaxError(text, currentLocation, currentLocation + 3, "']]>' is not allowed in text");
							}
							TryMoveNext();
							continue;
						}
					}
				} else 	if (type == RawTextType.Comment) {
					while(true) {
						if (TryMoveTo('-')) {
							if (TryPeek("-->")) break;
							if (TryPeek("--")) {
								OnSyntaxError(text, currentLocation, currentLocation + 2, "'--' is not allowed in comment");
							}
							TryMoveNext();
						}
						if (IsEndOfFile()) break;
					}
				} else if (type == RawTextType.CData) {
					TryMoveTo("]]>");
				} else if (type == RawTextType.ProcessingInstruction) {
					TryMoveTo("?>");
				} else if (type == RawTextType.UnknownBang) {
					TryMoveToAnyOf('<', '>');
				} else {
					throw new Exception("Uknown type " + type);
				}
				
				// Terminal found or real end was reached;
				bool finished = currentLocation < readingEnd || currentLocation == realReadingEnd;
				
				// Finished reading - restore the old reading end
				readingEnd = realReadingEnd;
				
				if (!finished) {
					// We have to continue reading more text fragments
					
					// We have to backtrack a bit because we just might ended with "]]" and the ">" was cut
					int backtrack = currentLocation - backtrackLenght;
					
					// If there is entity reference, make sure the next segment starts with it to prevent framentation
					int entitySearchStart = Math.Max(start + 1 /* data for us */, backtrack - maxEntityLenght);
					// Note that LastIndexOf works backward
					int entityIndex = input.LastIndexOf('&', backtrack, backtrack - entitySearchStart);
					if (entityIndex != -1) {
						backtrack = entityIndex;
					}
					
					GoBack(Math.Max(start + 1, backtrack)); // Max-just in case
				}
				text.Value = GetText(start, currentLocation);
				text.EndOffset = currentLocation;
				
				if (text.Value.Length > 0) {
					OnParsed(text);
					yield return text;
				}
				
				if (finished) {
					yield break;
				}
			}
		}
	}
}
