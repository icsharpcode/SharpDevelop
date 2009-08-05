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
	///   Start tag:  "<"  Name? (RawText+ RawAttribute)* RawText* (">" | "/>")
	///   End tag:    "</" Name? (RawText+ RawAttribute)* RawText* ">"
	///   P.instr.:   "<?" Name? (RawText+ RawAttribute)* RawText* "?>"
	///   Comment:    "<!" partof("--")?     (RawText)* "-->"     (Name is always null)
	///   DTD:        "<!" partof("DOCTYPE") (RawText)* ">"       (Name is always null)
	///   CData:      "<!" partof("[CDATA[") (RawText)* "]]" ">"  (Name is always null)
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
	/// </remarks>
	public class XmlParser
	{
		RawDocument userDocument = new RawDocument();
		XDocument userLinqDocument;
		TextDocument textDocument;
		TextSegmentCollection<RawObject> parsedItems = new TextSegmentCollection<RawObject>();
		List<DocumentChangeEventArgs> changesSinceLastParse = new List<DocumentChangeEventArgs>();
		
		/// <summary>
		/// Create new parser, but do not parse the text yet.
		/// </summary>
		public XmlParser(TextDocument textDocument)
		{
			this.userLinqDocument = userDocument.GetXDocument();
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
			input = textDocument.Text;
			readingEnd = input.Length;
			currentLocation = 0;
			
			foreach(DocumentChangeEventArgs change in changesSinceLastParse) {
				// Update offsets of all items
				parsedItems.UpdateOffsets(change);
				// Remove any items affected by the change
				int start = change.Offset - 2;
				int end = change.Offset + change.InsertionLength + 2;
				start = Math.Max(Math.Min(start, textDocument.TextLength - 1), 0);
				end = Math.Max(Math.Min(end, textDocument.TextLength - 1), 0);
				foreach(RawObject obj in parsedItems.FindOverlappingSegments(start, end - start)) {
					parsedItems.Remove(obj);
					Log("Removed cached item {0}", obj);
				}
			}
			changesSinceLastParse.Clear();
			
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
			parsedItems.Add(obj);
			System.Diagnostics.Debug.WriteLine("XML Parser: Parsed " + obj.ToString());
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
		int    currentLocation;
		
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
		
		// The methods start with 'try' to make it clear they can silently fail.
		// Read methods without 'try' have to succed or throw exception.
		//
		// For example:
		//   while(true) TryMoveNext();   is obviously infinite loop
		// whereas
		//   while(true) MoveNext();   should eventulay throw exception (if MoveNext it existed)
		//
		
		bool TryMoveNext()
		{
			if (currentLocation == readingEnd) return false;
			
			currentLocation++;
			return true;
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
			
			return TryPeek(text[0]) && input.Substring(currentLocation, text.Length) == text;
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
		static char[] WhiteSpaceAndReservedChars = new char[] {' ', '\n', '\r', '\t', '<', '=', '>', '/', '?'};
		
		bool TryPeekWhiteSpace()
		{
			if (currentLocation == readingEnd) return false;
			
			return WhiteSpaceChars.Contains(input[currentLocation]);
		}
		
		string ReadName()
		{
			AssertHasMoreData();
			
			int start = currentLocation;
			TryMoveToAnyOf(WhiteSpaceAndReservedChars.ToArray());
			return GetText(start, currentLocation);
		}
		
		RawDocument ReadDocument()
		{
			RawDocument doc;
			if (TryReadFromCacheOrNew(out doc)) return doc;
			
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
		
		RawElement ReadElement()
		{
			AssertHasMoreData();
			
			RawElement element;
			if (TryReadFromCacheOrNew(out element)) return element;
			
			element.StartOffset = currentLocation;
			// Read start tag
			element.AddChild(ReadTag());
			Debug.Assert(element.StartTag.IsStartTag);
			// Read content and end tag
			if (element.StartTag.ClosingBracket == ">") {
				while(true) {
					if (IsEndOfFile()) {
						break;
					} else if (TryPeek('<')) {
						RawObject content = ReadElementOrTag();
						if (content is RawTag && ((RawTag)content).IsEndTag) break;
						element.AddChild(content);
					} else {
						element.AddChildren(ReadText(RawTextType.CharacterData));
					}
				}
			}
			element.EndOffset = currentLocation;
			
			OnParsed(element);
			return element;
		}
		
		//   Start tag:  "<"  Name? (RawText+ RawAttribute)* RawText* (">" | "/>")
		//   End tag:    "</" Name? (RawText+ RawAttribute)* RawText* ">"
		//   P.instr.:   "<?" Name? (RawText+ RawAttribute)* RawText* "?>"
		//   Comment:    "<!" partof("--")?     (RawText)* "-->"     (Name is always null)
		//   CData:      "<!" partof("[CDATA[") (RawText)* "]]" ">"  (Name is always null)
		//   DTD:        "<!" partof("DOCTYPE") (RawText)* ">"       (Name is always null)
		
		RawTag ReadTag()
		{
			AssertHasMoreData();
			
			RawTag tag;
			if (TryReadFromCacheOrNew(out tag)) return tag;
			
			tag.StartOffset = currentLocation;
			
			// Read the opening bracket
			// It identifies the type of tag and parsing behavior for the rest of it
			tag.OpeningBracket = ReadOpeningBracket();
			
			// Read the name
			if (tag.IsStartTag || tag.IsEndTag || tag.IsProcessingInstruction) {
				if (HasMoreData()) {
					tag.Name = ReadName();
				}
			}
			
			if (tag.IsStartTag || tag.IsEndTag || tag.IsProcessingInstruction) {
				// Read attributes for the tag
				while(true) {
					if (TryPeekWhiteSpace()) {
						tag.AddChildren(ReadText(RawTextType.WhiteSpace));
					}
					string bracket;
					if (TryReadClosingBracket(out bracket)) {
						tag.ClosingBracket = bracket;
						break;
					}
					if (TryPeek('<')) break;
					if (HasMoreData()) {
						tag.AddChild(ReadAttribulte());
						continue;
					}
					break; // End of file
				}
			} else {
				// Simple tag types
				if (tag.IsComment) {
					// TODO: Be strict only if the opening bracket is complete
					tag.AddChildren(ReadText(RawTextType.Comment));
				} else if (tag.IsCData) {
					// TODO: Be strict only if the opening bracket is complete
					tag.AddChildren(ReadText(RawTextType.CData));
				} else if (tag.IsDocumentType) {
					// TODO: Nested definition
					tag.AddChildren(ReadText(RawTextType.DocumentTypeDefinition));
				}
				string bracket;
				if (TryReadClosingBracket(out bracket)) {
					tag.ClosingBracket = bracket;
				}
			}
			tag.EndOffset = currentLocation;
			
			OnParsed(tag);
			return tag;
		}
		
		/// <summary>
		/// Reads any of the know opening brackets
		/// Also accepts them if they are incomplete; one charater is suffcient
		/// </summary>
		string ReadOpeningBracket()
		{
			// We are using a lot of string literals so that the memory instances are shared
			int start = currentLocation;
			if (TryRead('<')) {
				if (TryRead('/')) {
					return "</";
				} else if (TryRead('!')) {
					if (TryRead('-')) {
						if (TryRead('-')) {
							return "<!--";
						} else {
							return "<!-";
						}
					} else if (TryReadPartOf("[CDATA[")) {
						return GetText(start, currentLocation);
					} else if (TryReadPartOf("DOCTYPE")) {
						return GetText(start, currentLocation);
					} else {
						return "<!";
					}
				} else if (TryRead('?')) {
					return "<?";
				} else {
					return "<";
				}
			} else {
				throw new Exception("'<' expected");
			}
		}
		
		/// <summary>
		/// Reads any of the know closing brackets
		/// Also accepts them if they are incomplete; one charater is suffcient
		/// </summary>
		bool TryReadClosingBracket(out string bracket)
		{
			// We are using a lot of string literals so that the memory instances are shared
			int start = currentLocation;
			if (TryRead('>')) {
				bracket = ">";
			} else 	if (TryRead('/')) {
				if (TryRead('>')) {
					bracket = "/>";
				} else {
					bracket = "/";
				}
			} else 	if (TryRead('?')) {
				if (TryRead('>')) {
					bracket = "?>";
				} else {
					bracket = "?";
				}
			} else if (TryReadPartOf("-->")) {
				bracket = GetText(start, currentLocation);
			} else if (TryReadPartOf("]]>")) {
				bracket = GetText(start, currentLocation);
			} else {
				bracket = null;
				return false;
			}
			return true;
		}
		
		RawAttribute ReadAttribulte()
		{
			AssertHasMoreData();
			
			RawAttribute attr;
			if (TryReadFromCacheOrNew(out attr)) return attr;
			
			attr.StartOffset = currentLocation;
			if (HasMoreData()) attr.Name = ReadName();
			int checkpoint = currentLocation;
			TryMoveToNonWhiteSpace();
			if (TryRead('=')) {
				TryMoveToNonWhiteSpace();
				attr.EqualsSign += GetText(checkpoint, currentLocation);
				// Read attribute value
				int start = currentLocation;
				if (TryRead('"')) {
					TryMoveToAnyOf('"', '<');
					TryRead('"');
					attr.Value = GetText(start, currentLocation);
				} else if (TryRead('\'')) {
					TryMoveToAnyOf('\'', '<');
					TryRead('\'');
					attr.Value = GetText(start, currentLocation);
				}
			} else {
				currentLocation = checkpoint;
			}
			attr.EndOffset = currentLocation;
			
			OnParsed(attr);
			return attr;
		}
		
		const int maxEntityLenght = 12; // 6 for build-in ones
		const int maxTextFragmentSize = 8;
		const int lookAheadLenght = (3 * maxTextFragmentSize) / 2;
		const int backtrackLenght = 4;  // 2: get back over "]]"   1: so that we have some data   1: safety
		
		/// <summary>
		/// Reads text and optionaly separates it into fragments.
		/// It can also return empty set for no appropriate text input
		/// </summary>
		IEnumerable<RawObject> ReadText(RawTextType type)
		{
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
					TryMoveTo('<');
				} else 	if (type == RawTextType.Comment) {
					TryMoveTo("--");
				} else if (type == RawTextType.DocumentTypeDefinition) {
					TryMoveTo('>');
				} else if (type == RawTextType.CData) {
					TryMoveTo("]]>");
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
					int entityIndex = input.LastIndexOf('&', entitySearchStart, backtrack - entitySearchStart);
					if (entityIndex != -1) {
						backtrack = entityIndex;
					}
					
					currentLocation = Math.Max(start + 1, backtrack); // Max-just in case
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
