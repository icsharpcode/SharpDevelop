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
using System.Xml.Linq;

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
		// TODO: Error reporting
		// TODO: Simple tag matching heuristic
		// TODO: Simple attribute value closing heurisitc
		// TODO: Backtracking for unclosed long Text sections
		
		RawDocument userDocument;
		XDocument userLinqDocument;
		TextDocument textDocument;
		
		List<DocumentChangeEventArgs> changesSinceLastParse = new List<DocumentChangeEventArgs>();
		
		// Stored parsed items as long as they are valid
		TextSegmentCollection<RawObject> parsedItems = new TextSegmentCollection<RawObject>();
		
		// Is used to identify what memory range was touched by object
		// The default is (StartOffset, EndOffset + 1) which is not stored
		TextSegmentCollection<TouchedMemoryRange> touchedMemoryRanges = new TextSegmentCollection<TouchedMemoryRange>();
		
		class TouchedMemoryRange: TextSegment
		{
			public RawObject TouchedByObject { get; set; }
		}
		
		/// <summary>
		/// Create new parser, but do not parse the text yet.
		/// </summary>
		public XmlParser(string input)
		{
			this.input = input;
			this.userDocument = new RawDocument();
			this.userLinqDocument = userDocument.GetXDocument();
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
			if (parsedDocument.ReadCallID != userDocument.ReadCallID) {
				PrintStringCacheStats();
				RawObject.LogDom("Updating main DOM tree...");
			}
			userDocument.UpdateDataFrom(parsedDocument);
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
			while(obj != null && obj.StartOffset <= offset + lookaheadCount) {
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
		
		bool TryPeek(char c)
		{
			if (currentLocation == readingEnd) return false;
			
			return input[currentLocation] == c;
		}
		
		bool TryPeek(string text)
		{
			if (currentLocation + text.Length > readingEnd) return false;
			// Early exit
			if (!TryPeek(text[0])) return false;
			maxTouchedLocation = Math.Max(maxTouchedLocation, currentLocation + (text.Length - 1));
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
		
		/// <summary>
		/// Context: any
		/// </summary>
		RawDocument ReadDocument()
		{
			RawDocument doc;
			if (TryReadFromCacheOrNew(out doc)) return doc;
			
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
						break;
					} else if (TryPeek('<')) {
						RawObject content = ReadElementOrTag();
						element.AddChild(content);
						if (content is RawTag && ((RawTag)content).IsEndTag) break;
					} else {
						element.AddChildren(ReadText(RawTextType.CharacterData));
					}
				}
			}
			element.EndOffset = currentLocation;
			// TODO: Closing tag matches
			// TODO: Heuristic on closing
			
			// TODO: ERROR - attribute name may not apper multiple times
			
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
			
			if (tag.IsStartTag || tag.IsEndTag) {
				// Read the name
				string name;
				if (TryReadName(out name)) tag.Name = name;
				// TODO: Error - bad name
				// TODO: Error - no name?
				
				// TODO: Error - = or " or ' not expected
				
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
			} else if (tag.IsComment) {
				// TODO: Backtrack if file end reached
				tag.AddChildren(ReadText(RawTextType.Comment));
			} else if (tag.IsCData) {
				// TODO: Backtrack if file end reached
				tag.AddChildren(ReadText(RawTextType.CData));
			} else if (tag.IsProcessingInstruction) {
				string name;
				if (TryReadName(out name)) tag.Name = name;
				// TODO: Error - bad name
				// TODO: Error - no name?
				// TODO: Backtrack if file end reached
				tag.AddChildren(ReadText(RawTextType.ProcessingInstruction));
			} else if (tag.IsUnknownBang) {
				// TODO: Backtack if '<' (or end of file)
				tag.AddChildren(ReadText(RawTextType.UnknownBang));
			} else if (tag.IsDocumentType) {
				tag.AddChildren(ReadContentOfDTD());
			} else {
				throw new Exception(string.Format("Unknown opening bracket '{0}'", tag.OpeningBracket));
			}
			
			// Read closing bracket
			string bracket;
			if (TryReadClosingBracket(out bracket)) tag.ClosingBracket = bracket;
			// TODO: else ERROR - Missing closing bracket
			// TODO: check correct closing bracket (special case if end of file)
				
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
						// TODO: Error - unkown bang tag
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
			// TODO: Touched memory
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
			if (TryReadName(out name)) attr.Name = name;
			// TODO:  else ERROR - attribute name expected
			
			// Read equals sign and surrounding whitespace
			int checkpoint = currentLocation;
			TryMoveToNonWhiteSpace();
			if (TryRead('=')) {
				TryMoveToNonWhiteSpace();
				attr.EqualsSign = GetText(checkpoint, currentLocation);
			} else {
				GoBack(checkpoint);
				// TODO: ERROR - Equals expected
			}
			
			// Read attribute value
			int start = currentLocation;
			if (TryRead('"')) {
				TryMoveToAnyOf('"', '<');
				TryRead('"');
				// TODO: Some backtracking?
				// TODO: ERROR - Attribute value not closed
				attr.Value = GetText(start, currentLocation);
			} else if (TryRead('\'')) {
				TryMoveToAnyOf('\'', '<');
				TryRead('\'');
				// TODO: Some backtracking?
				// TODO: ERROR - Attribute value not closed
				attr.Value = GetText(start, currentLocation);
			} else {
				// TODO: ERROR - Attribute value expected
			}
			
			// TODO: Heuristic for missing " or '
			// TODO: Normalize attribute values
			
			attr.EndOffset = currentLocation;
			
			OnParsed(attr);
			return attr;
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
		/// It can also return empty set for no appropriate text input
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
					lookahead = true; // In the middle of the text edit
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
						readingEnd = nextFragmentIndex + backtrackLenght;
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
					// TODO: "]]>" is error
					TryMoveTo('<');
				} else 	if (type == RawTextType.Comment) {
					// TODO: "--" is error
					TryMoveTo("-->");
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
