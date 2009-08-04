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
			currentLocation = 0;
			input = textDocument.Text;
			
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
		
		T ReadFromCache<T>(int location) where T: RawObject
		{
			RawObject obj = parsedItems.FindFirstSegmentWithStartAfter(location);
			while(obj != null && obj.StartOffset == location) {
				if (obj is T) {
					currentLocation += obj.Length;
					return (T)obj;
				}
				obj = parsedItems.GetNextSegment(obj);
			}
			return null;
		}
		
		void Log(string text, params object[] pars)
		{
			System.Diagnostics.Debug.WriteLine(string.Format("XML Parser: " + text, pars));
		}
		
		void LogParsed(RawObject obj)
		{
			System.Diagnostics.Debug.WriteLine("XML Parser: Parsed " + obj.ToString());
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
		int  currentLocation;
		
		bool IsEndOfFile()
		{
			return currentLocation == input.Length;
		}
		
		bool HasMoreData()
		{
			return currentLocation < input.Length;
		}
		
		void AssertHasMoreData()
		{
			if (currentLocation == input.Length) {
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
			if (currentLocation == input.Length) return false;
			
			currentLocation++;
			return true;
		}
		
		bool TryRead(char c)
		{
			if (currentLocation == input.Length) return false;
			
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
			if (currentLocation == input.Length) return false;
			
			return input[currentLocation] == c;
		}
		
		bool TryPeek(string text)
		{
			if (currentLocation + text.Length > input.Length) return false;
			
			return input.Substring(currentLocation, text.Length) == text;
		}
		
		bool TryMoveTo(char c)
		{
			while(true) {
				if (currentLocation == input.Length) return false;
				if (input[currentLocation] == c) return true;
				currentLocation++;
			}
		}
		
		bool TryMoveToAnyOf(params char[] c)
		{
			while(true) {
				if (currentLocation == input.Length) return false;
				if (c.Contains(input[currentLocation])) return true;
				currentLocation++;
			}
		}
		
		string GetText(int start, int end)
		{
			if (start == input.Length && end == input.Length) {
				return string.Empty;
			} else {
				return GetCachedString(input.Substring(start, end - start));
			}
		}
		
		static char[] WhiteSpaceChars = new char[] {' ', '\n', '\r', '\t'};
		static char[] WhiteSpaceAndReservedChars = new char[] {' ', '\n', '\r', '\t', '<', '=', '>', '/', '?'};
		
		bool TryPeekWhiteSpace()
		{
			if (currentLocation == input.Length) return false;
			
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
			RawDocument doc = ReadFromCache<RawDocument>(currentLocation);
			if (doc != null) return doc;
			
			doc = new RawDocument();
			
			doc.StartOffset = currentLocation;
			while(true) {
				if (IsEndOfFile()) {
					break;
				} else if (TryPeek('<')) {
					doc.AddChild(ReadElementOrTag());
				} else {
					doc.AddChild(ReadCharacterData());
				}
			}
			doc.EndOffset = currentLocation;
			
			LogParsed(doc);
			parsedItems.Add(doc);
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
			
			RawElement element = ReadFromCache<RawElement>(currentLocation);
			if (element != null) return element;

			element = new RawElement();
			
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
						element.AddChild(ReadCharacterData());
					}
				}
			}
			element.EndOffset = currentLocation;
			
			LogParsed(element);
			parsedItems.Add(element);
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
			
			RawTag tag = ReadFromCache<RawTag>(currentLocation);
			if (tag != null) return tag;
			
			tag = new RawTag();
			
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
						tag.AddChild(ReadWhiteSpace());
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
					tag.AddChildren(ReadTextUntil("-->").ToList());
				} else if (tag.IsCData) {
					// TODO: Be strict only if the opening bracket is complete
					tag.AddChildren(ReadTextUntil("]]>").ToList());
				} else if (tag.IsDocumentType) {
					// TODO: Nested definition
					tag.AddChildren(ReadTextUntil(">").ToList());
				}
				string bracket;
				if (TryReadClosingBracket(out bracket)) {
					tag.ClosingBracket = bracket;
				}
			}
			tag.EndOffset = currentLocation;
			
			LogParsed(tag);
			parsedItems.Add(tag);
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
			
			RawAttribute attr = ReadFromCache<RawAttribute>(currentLocation);
			if (attr != null) return attr;
			
			attr = new RawAttribute();
			
			attr.StartOffset = currentLocation;
			if (HasMoreData()) attr.Name = ReadName();
			int checkpoint = currentLocation;
			attr.EqualsSign = string.Empty; 
			if (TryPeekWhiteSpace()) attr.EqualsSign += ReadWhiteSpace().Value;
			if (TryRead('=')) {
				attr.EqualsSign += "=";
				if (TryPeekWhiteSpace()) attr.EqualsSign += ReadWhiteSpace().Value;
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
				attr.EqualsSign = null;
				currentLocation = checkpoint;
			}
			attr.EndOffset = currentLocation;
			
			parsedItems.Add(attr);
			return attr;
		}
		
		RawText ReadWhiteSpace()
		{
			AssertHasMoreData();
			
			RawText ws = ReadFromCache<RawText>(currentLocation);
			if (ws != null) return ws;
			
			ws = new RawText();
			
			ws.StartOffset = currentLocation;
			int start = currentLocation;
			while(TryPeekWhiteSpace()) TryMoveNext();
			ws.Value = GetText(start, currentLocation);
			ws.EndOffset = currentLocation;
			
			Debug.Assert(ws.Value.Length > 0);
			
			parsedItems.Add(ws);
			return ws;
		}
		
		RawText ReadCharacterData()
		{
			Debug.Assert(HasMoreData());
			
			RawText charData = ReadFromCache<RawText>(currentLocation);
			if (charData != null) return charData;
			
			charData = new RawText();
			
			charData.StartOffset = currentLocation;
			int start = currentLocation;
			TryMoveTo('<');
			charData.Value = GetText(start, currentLocation);
			charData.EndOffset = currentLocation;
			
			Debug.Assert(charData.Value.Length > 0);
			
			parsedItems.Add(charData);
			return charData;
		}
		
		IEnumerable<RawObject> ReadTextUntil(string closingText)
		{
			Debug.Assert(HasMoreData());
			
			RawText charData = ReadFromCache<RawText>(currentLocation);
			// TODO: How many return?  Ensure the output is same as before
			if (charData != null) yield return charData;
			
			charData = new RawText();
			
			charData.StartOffset = currentLocation;
			int start = currentLocation;
			while(true) {
				if (!TryMoveTo(closingText[0])) break; // End of file
				if (TryPeek(closingText)) break; // Match
				TryMoveNext();
			}
			charData.Value = GetText(start, currentLocation);
			charData.EndOffset = currentLocation;
			
			Debug.Assert(charData.Value.Length > 0);
			
			parsedItems.Add(charData);
			yield return charData;
		}
	}
}
