// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
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
	/// Any further parses will reparse only the changed parts and the existing tree will
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
		/// Generate syntax error when seeing enity reference other then the build-in ones
		/// </summary>
		public bool EntityReferenceIsError { get; set; }
		
		/// <summary>
		/// Create new parser, but do not parse the text yet.
		/// </summary>
		public XmlParser(string input)
		{
			this.input = input;
			this.userDocument = new RawDocument() { Parser = this };
			this.EntityReferenceIsError = true;
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
		
		/// <summary> Throws exception if condition is false </summary>
		protected static void Assert(bool condition, string message)
		{
			if (!condition) {
				throw new Exception("Assertion failed: " + message);
			}
		}
		
		/// <summary> Throws exception if condition is false </summary>
		[Conditional("DEBUG")]
		protected static void DebugAssert(bool condition, string message)
		{
			if (!condition) {
				throw new Exception("Assertion failed: " + message);
			}
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
				// Removing will cause one of the ends to be set to change.Offset
				// FindSegmentsContaining includes any segments touching
				// so that conviniently takes care of the +1 byte
				foreach(RawObject obj in parsedItems.FindSegmentsContaining(change.Offset)) {
					RemoveFromCache(obj, false);
				}
				foreach(TouchedMemoryRange memory in touchedMemoryRanges.FindSegmentsContaining(change.Offset)) {
					Log("Found that {0} dependeds on memory ({1}-{2})", memory.TouchedByObject, memory.StartOffset, memory.EndOffset);
					RemoveFromCache(memory.TouchedByObject, true);
					touchedMemoryRanges.Remove(memory);
				}
			}
			changesSinceLastParse.Clear();
			
			currentLocation = 0;
			maxTouchedLocation = 0;
			inputLength = input.Length;
			
			RawDocument parsedDocument = ReadDocument();
			parsedDocument.DebugCheckConsistency(true);
			// Just in case parse method was called redundantly
			PrintStringCacheStats();
			RawObject.LogDom("Updating main DOM tree...");
			userDocument.UpdateTreeFrom(parsedDocument);
			userDocument.DebugCheckConsistency(false);
			return userDocument;
		}
		
		List<RawObject> FindParents(RawObject child)
		{
			List<RawObject> parents = new List<RawObject>();
			foreach(RawObject parent in parsedItems.FindSegmentsContaining(child.StartOffset)) {
				// Parent is anyone wholy containg the child
				if (parent.StartOffset <= child.StartOffset && child.EndOffset <= parent.EndOffset && parent != child) {
					parents.Add(parent);
				}
			}
			return parents;
		}
		
		/// <summary> Remove from cache including all parents </summary>
		internal void RemoveFromCache(RawObject obj, bool includeParents)
		{
			if (includeParents) {
				List<RawObject> parents = FindParents(obj);
				
				foreach(RawObject r in parents) {
					if (parsedItems.Remove(r)) {
						r.IsInCache = false;
						Log("Removing cached item {0} (it is parent)", r);
					}
				}
			}
			
			if (parsedItems.Remove(obj)) {
				obj.IsInCache = false;
				Log("Removed cached item {0}", obj);
			}
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
			if (obj.Length == 0 && !(obj is RawDocument))
				throw new Exception(string.Format("Could not parse {0}.  It has zero length.", obj));
			if (obj is RawContainer) {
				foreach(RawObject child in ((RawContainer)obj).Children) {
					if (!(obj.StartOffset <= child.StartOffset && child.EndOffset <= obj.EndOffset))
						throw new Exception("Wrong nesting");
				}
			}
			parsedItems.Add(obj);
			obj.IsInCache = true;
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
		int stringCacheStoredCount;
		int stringCacheStoredSize;
		
		string GetCachedString(string cached)
		{
			stringCacheRequestedCount += 1;
			stringCacheRequestedSize += 8 + 2 * cached.Length;
			// Do not bother with long strings
			if (cached.Length <= 32) return cached;
			if (stringCache.ContainsKey(cached)) {
				// Get the instance from the cache instead
				return stringCache[cached];
			} else {
				// Add to cache
				stringCacheStoredCount += 1;
				stringCacheStoredSize += 8 + 2 * cached.Length;
				stringCache.Add(cached, cached);
				return cached;
			}
		}
		
		void PrintStringCacheStats()
		{
			Log("String cache: Requested {0} ({1} bytes);  Actaully stored {2} ({3} bytes); {4}% stored", stringCacheRequestedCount, stringCacheRequestedSize, stringCacheStoredCount, stringCacheStoredSize, stringCacheRequestedSize == 0 ? 0 : stringCacheStoredSize * 100 / stringCacheRequestedSize);
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
		
		#region Text reading methods
		
		string input;
		int    inputLength;
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
			return currentLocation == inputLength;
		}
		
		void AssertIsEndOfFile()
		{
			Assert(IsEndOfFile(), "End of file expected at this point");
		}
		
		bool HasMoreData()
		{
			return currentLocation < inputLength;
		}
		
		void AssertHasMoreData()
		{
			Assert(HasMoreData(), "Unexpected end of file");
		}
		
		bool TryMoveNext()
		{
			if (currentLocation == inputLength) return false;
			
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
			if (currentLocation == inputLength) return false;
			
			if (input[currentLocation] == c) {
				currentLocation++;
				return true;
			} else {
				return false;
			}
		}
		
		bool TryReadAnyOf(params char[] c)
		{
			if (currentLocation == inputLength) return false;
			
			if (c.Contains(input[currentLocation])) {
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
		
		bool TryPeekPrevious(char c, int back)
		{
			if (currentLocation - back == inputLength) return false;
			if (currentLocation - back < 0 ) return false;
			
			return input[currentLocation - back] == c;
		}
		
		bool TryPeek(char c)
		{
			if (currentLocation == inputLength) return false;
			
			return input[currentLocation] == c;
		}
		
		bool TryPeekAnyOf(params char[] chars)
		{
			if (currentLocation == inputLength) return false;
			
			return chars.Contains(input[currentLocation]);
		}
		
		bool TryPeek(string text)
		{
			if (!TryPeek(text[0])) return false; // Early exit
			
			maxTouchedLocation = Math.Max(maxTouchedLocation, currentLocation + (text.Length - 1));
			// The following comparison 'touches' the end of file - it does depend on the end being there
			if (currentLocation + text.Length > inputLength) return false;
			
			return input.Substring(currentLocation, text.Length) == text;
		}
		
		bool TryPeekWhiteSpace()
		{
			if (currentLocation == inputLength) return false;
			
			char c = input[currentLocation];
			return c == ' ' || c == '\t' || c == '\n' || c == '\r';
		}
		
		// The move functions do not have to move if already at target
		// The move functions allow 'overriding' of the document length
		
		bool TryMoveTo(char c)
		{
			return TryMoveTo(c, inputLength);
		}
		
		bool TryMoveTo(char c, int inputLength)
		{
			if (currentLocation == inputLength) return false;
			int index = input.IndexOf(c, currentLocation, inputLength - currentLocation);
			if (index != -1) {
				currentLocation = index;
				return true;
			} else {
				currentLocation = inputLength;
				return false;
			}
		}
		
		bool TryMoveToAnyOf(params char[] c)
		{
			return TryMoveToAnyOf(c, inputLength);
		}
		
		bool TryMoveToAnyOf(char[] c, int inputLength)
		{
			if (currentLocation == inputLength) return false;
			int index = input.IndexOfAny(c, currentLocation, inputLength - currentLocation);
			if (index != -1) {
				currentLocation = index;
				return true;
			} else {
				currentLocation = inputLength;
				return false;
			}
		}
		
		bool TryMoveTo(string text)
		{
			return TryMoveTo(text, inputLength);
		}
		
		bool TryMoveTo(string text, int inputLength)
		{
			if (currentLocation == inputLength) return false;
			int index = input.IndexOf(text, currentLocation, inputLength - currentLocation, StringComparison.Ordinal);
			if (index != -1) {
				maxTouchedLocation = index + text.Length - 1;
				currentLocation = index;
				return true;
			} else {
				currentLocation = inputLength;
				return false;
			}
		}
		
		bool TryMoveToNonWhiteSpace()
		{
			return TryMoveToNonWhiteSpace(inputLength);
		}
		
		bool TryMoveToNonWhiteSpace(int inputLength)
		{
			while(TryPeekWhiteSpace()) currentLocation++;
			return HasMoreData();
		}
		
		/// <summary>
		/// Read a name token.
		/// The following characters are not allowed:
		///   ""         End of file
		///   " \n\r\t"  Whitesapce
		///   "=\'\""    Attribute value
		///   "&lt;>/?"  Tags
		/// </summary>
		/// <returns> True if read at least one character </returns>
		bool TryReadName(out string res)
		{
			int start = currentLocation;
			// Keep reading up to invalid character
			while(true) {
				if (currentLocation == inputLength) break;              // Reject end of file
				char c = input[currentLocation];
				if (0x41 <= (int)c && (int)c <= 0x7A) {                 // Accpet 0x41-0x7A (A-Z[\]^_`a-z)
					currentLocation++;
					continue;
				}
				if (c == ' ' || c == '\n' || c == '\r' || c == '\t' ||  // Reject whitesapce
				    c == '=' || c == '\'' || c == '"'  ||               // Reject attributes
				    c == '<' || c == '>'  || c == '/'  || c == '?') {   // Reject tags
					break;
				} else {
					currentLocation++;
					continue;                                            // Accept other character
				}
			}
			if (start == currentLocation) {
				res = string.Empty;
				return false;
			} else {
				res = GetText(start, currentLocation);
				return true;
			}
		}
		
		string GetText(int start, int end)
		{
			if (end > currentLocation) throw new Exception("Reading ahead of current location");
			if (start == inputLength && end == inputLength) {
				return string.Empty;
			} else {
				return GetCachedString(input.Substring(start, end - start));
			}
		}
		
		#endregion
		
		/// <summary>
		/// Get flat hiearchy of the document.
		/// Returns only Text, Tag or properly nested Element
		/// </summary>
		List<RawObject> ReadFlatObjects()
		{
			List<RawObject> stream = new List<RawObject>();
			
			while(true) {
				if (IsEndOfFile()) {
					break;
				} else if (TryPeek('<')) {
					RawElement elem;
					if (TryReadFromCacheOrNew(e => e.IsProperlyNested, out elem)) {
						stream.Add(elem);
					} else {
						stream.Add(ReadTag());
					}
				} else {
					stream.AddRange(ReadText(RawTextType.CharacterData));
				}
			}
			
			return stream;
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
			
			if (tag.IsStartOrEmptyTag || tag.IsEndTag || tag.IsProcessingInstruction) {
				// Read the name
				string name;
				if (TryReadName(out name)) {
					if (!IsValidName(name)) {
						OnSyntaxError(tag, currentLocation - name.Length, currentLocation, "The name '{0}' is invalid", name);
					}
				} else {
					OnSyntaxError(tag, "Element name expected");
				}
				tag.Name = name;
			}
			
			if (tag.IsStartOrEmptyTag || tag.IsEndTag) {
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
			TryReadClosingBracket(out bracket);
			tag.ClosingBracket = bracket;
			
			// Error check
			int brStart = currentLocation - (tag.ClosingBracket ?? string.Empty).Length;
			if (tag.Name == null) {
				// One error was reported already
			} else if (tag.IsStartOrEmptyTag) {
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
				bracket = string.Empty;
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
				if (TryRead('\'')) TryMoveTo('\'');  // Skip single quoted string TODO: Bug
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
				if (!IsValidName(name)) {
					OnSyntaxError(attr, currentLocation - name.Length, currentLocation, "The name '{0}' is invalid", name);
				}
			} else {
				OnSyntaxError(attr, "Attribute name expected");
			}
			attr.Name = name;
			
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
				attr.EqualsSign = string.Empty;
			}
			
			// Read attribute value
			int start = currentLocation;
			char quoteChar = TryPeek('"') ? '"' : '\'';
			bool startsWithQuote;
			if (TryRead(quoteChar)) {
				startsWithQuote = true;
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
				startsWithQuote = false;
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
			attr.Value = Unquote(attr.QuotedValue);
			attr.Value = Dereference(attr, attr.Value, startsWithQuote ? start + 1 : start);
			
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
					if (TryMoveToNonWhiteSpace() && TryRead("=") &&
					    TryMoveToNonWhiteSpace() && TryPeekAnyOf('"', '\''))
					{
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
			DebugAssert(end > start, "Empty text");
			
			RawText text = new RawText() {
				StartOffset = start,
				EndOffset = end,
				EscapedValue = GetText(start, end),
				Type = RawTextType.Other
			};
			
			OnParsed(text);
			return text;
		}
		
		const int maxEntityLength = 12; // The longest build-in one is 10 ("&#1114111;")
		const int maxTextFragmentSize = 8;
		const int lookAheadLenght = (3 * maxTextFragmentSize) / 2; // More so that we do not get small "what was inserted" fragments
		
		/// <summary>
		/// Reads text and optionaly separates it into fragments.
		/// It can also return empty set for no appropriate text input.
		/// Make sure you enumerate it only once
		/// </summary>
		IEnumerable<RawObject> ReadText(RawTextType type)
		{
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
				// (the first character not to be read)
				int fragmentEnd = Math.Min(currentLocation + maxTextFragmentSize, inputLength);
				
				// Look if some futher text has been already processed and align so that
				// we hit that chache point.  It is expensive so it is off for the first run
				if (lookahead) {
					int nextFragmentIndex = GetStartOfCachedObject<RawText>(t => t.Type == type, currentLocation, lookAheadLenght);
					// Found and would fit whole entity
					if (nextFragmentIndex != -1 && nextFragmentIndex > currentLocation + maxEntityLength) {
						fragmentEnd = Math.Min(nextFragmentIndex, inputLength);
						Log("Parsing only text ({0}-{1}) because later text was already processed", currentLocation, fragmentEnd);
					}
				}
				lookahead = true;
				
				text.StartOffset = currentLocation;
				int start = currentLocation;
				
				// Try move to the terminator given by the context
				if (type == RawTextType.WhiteSpace) {
					TryMoveToNonWhiteSpace(fragmentEnd);
				} else if (type == RawTextType.CharacterData) {
					while(true) {
						if (!TryMoveToAnyOf(new char[] {'<', ']'}, fragmentEnd)) break; // End of fragment
						if (TryPeek('<')) break;
						if (TryPeek(']')) {
							if (TryPeek("]]>")) {
								OnSyntaxError(text, currentLocation, currentLocation + 3, "']]>' is not allowed in text");
							}
							TryMoveNext();
							continue;
						}
						throw new Exception("Infinite loop");
					}
				} else 	if (type == RawTextType.Comment) {
					// Do not report too many errors
					bool errorReported = false;
					while(true) {
						if (!TryMoveTo('-', fragmentEnd)) break; // End of fragment
						if (TryPeek("-->")) break;
						if (TryPeek("--") && !errorReported) {
							OnSyntaxError(text, currentLocation, currentLocation + 2, "'--' is not allowed in comment");
							errorReported = true;
						}
						TryMoveNext();
					}
				} else if (type == RawTextType.CData) {
					while(true) {
						// We can not use use TryMoveTo("]]>", fragmentEnd) because it may incorectly accept "]" at the end of fragment
						if (!TryMoveTo(']', fragmentEnd)) break; // End of fragment
						if (TryPeek("]]>")) break;
						TryMoveNext();
					}
				} else if (type == RawTextType.ProcessingInstruction) {
					while(true) {
						if (!TryMoveTo('?', fragmentEnd)) break; // End of fragment
						if (TryPeek("?>")) break;
						TryMoveNext();
					}
				} else if (type == RawTextType.UnknownBang) {
					TryMoveToAnyOf(new char[] {'<', '>'}, fragmentEnd);
				} else {
					throw new Exception("Uknown type " + type);
				}
				
				// Terminal found or real end was reached;
				bool finished = currentLocation < fragmentEnd || IsEndOfFile();
				
				if (!finished) {
					// We have to continue reading more text fragments
					
					// If there is entity reference, make sure the next segment starts with it to prevent framentation
					int entitySearchStart = Math.Max(start + 1 /* data for us */, currentLocation - maxEntityLength);
					int entitySearchLength = currentLocation - entitySearchStart;
					if (entitySearchLength > 0) {
						// Note that LastIndexOf works backward
						int entityIndex = input.LastIndexOf('&', currentLocation - 1, entitySearchLength);
						if (entityIndex != -1) {
							GoBack(entityIndex);
						}
					}
				}
				
				text.EscapedValue = GetText(start, currentLocation);
				if (type == RawTextType.CharacterData) {
					text.Value = Dereference(text, text.EscapedValue, start);
				} else {
					text.Value = text.EscapedValue;
				}
				text.EndOffset = currentLocation;
				
				if (text.EscapedValue.Length > 0) {
					OnParsed(text);
					yield return text;
				}
				
				if (finished) {
					yield break;
				}
			}
		}
		
		#region Helper methods
		
		static bool IsValidName(string name)
		{
			try {
				System.Xml.XmlConvert.VerifyName(name);
				return true;
			} catch (System.Xml.XmlException) {
				return false;
			}
		}
		
		/// <summary> Remove quoting from the given string </summary>
		static string Unquote(string quoted)
		{
			if (string.IsNullOrEmpty(quoted)) return string.Empty;
			char first = quoted[0];
			if (quoted.Length == 1) return (first == '"' || first == '\'') ? string.Empty : quoted;
			char last  = quoted[quoted.Length - 1];
			if (first == '"' || first == '\'') {
				if (first == last) {
					// Remove both quotes
					return quoted.Substring(1, quoted.Length - 2);
				} else {
					// Remove first quote
					return quoted.Remove(0, 1);
				}
			} else {
				if (last == '"' || last == '\'') {
					// Remove last quote
					return quoted.Substring(0, quoted.Length - 1);
				} else {
					// Keep whole string
					return quoted;
				}
			}
		}
		
		string Dereference(RawObject owner, string text, int textLocation)
		{
			StringBuilder sb = null;  // The dereferenced text so far (all up to 'curr')
			int curr = 0;
			while(true) {
				// Reached end of input
				if (curr == text.Length) {
					if (sb != null) {
						return sb.ToString();
					} else {
						return text;
					}
				}
				
				// Try to find reference
				int start = text.IndexOf('&', curr);
				
				// No more references found
				if (start == -1) {
					if (sb != null) {
						sb.Append(text, curr, text.Length - curr); // Add rest
						return sb.ToString();
					} else {
						return text;
					}
				}
				
				// Append text before the enitiy reference
				if (sb == null) sb = new StringBuilder(text.Length);
				sb.Append(text, curr, start - curr);
				curr = start;
				
				// Process the entity
				int errorLoc = textLocation + sb.Length;
				          
				// Find entity name
				int end = text.IndexOfAny(new char[] {'&', ';'}, start + 1, Math.Min(maxEntityLength, text.Length - (start + 1)));
				if (end == -1 || text[end] == '&') {
					// Not found
					OnSyntaxError(owner, errorLoc, errorLoc + 1, "Entity reference must be terminated with ';'");
					// Keep '&'
					sb.Append('&');
					curr++;
					continue;  // Restart and next character location
				}
				string name = text.Substring(start + 1, end - (start + 1));
				
				// Resolve the name
				string replacement;
				if (name == "amp") {
					replacement = "&";
				} else if (name == "lt") {
					replacement = "<";
				} else if (name == "gt") {
					replacement = ">";
				} else if (name == "apos") {
					replacement = "'";
				} else if (name == "quot") {
					replacement = "\"";
				} else if (name.Length > 0 && name[0] == '#') {
					int num;
					if (name.Length > 1 && name[1] == 'x') {
						if (!int.TryParse(name.Substring(2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture.NumberFormat, out num)) {
							num = -1;
							OnSyntaxError(owner, errorLoc + 3, errorLoc + 1 + name.Length, "Hexadecimal code of unicode character expected");
						}
					} else {
						if (!int.TryParse(name.Substring(1), NumberStyles.None, CultureInfo.InvariantCulture.NumberFormat, out num)) {
							num = -1;
							OnSyntaxError(owner, errorLoc + 2, errorLoc + 1 + name.Length, "Numeric code of unicode character expected");
						}
					}
					if (num != -1) {
						try {
							replacement = char.ConvertFromUtf32(num);
						} catch (ArgumentOutOfRangeException) {
							replacement = null;
							OnSyntaxError(owner, errorLoc + 2, errorLoc + 1 + name.Length, "Invalid unicode character U+{0:X} ({0})", num);
						}
					} else {
						replacement = null;
					}
				} else {
					replacement = null;
					if (this.EntityReferenceIsError) {
						OnSyntaxError(owner, errorLoc, errorLoc + 1 + name.Length + 1, "Unknown entity reference '{0}'", name);
					}
				}
				
				// Append the replacement to output
				if (replacement != null) {
					sb.Append(replacement);
				} else {
					sb.Append('&');
					sb.Append(name);
					sb.Append(';');
				}
				curr = end + 1;
				continue;
			}
		}
		
		#endregion
		
		#region Object stream reading
		
		string PrintObjects(IEnumerable<RawObject> objs)
		{
			StringBuilder sb = new StringBuilder();
			foreach(RawObject obj in objs) {
				if (obj is RawTag) {
					if (obj == StartTagPlaceholder) {
						sb.Append("#StartTag#");
					} else if (obj == EndTagPlaceholder) {
						sb.Append("#EndTag#");
					} else {
						sb.Append(((RawTag)obj).OpeningBracket);
						sb.Append(((RawTag)obj).Name);
						sb.Append(((RawTag)obj).ClosingBracket);
					}
				} else if (obj is RawElement) {
					sb.Append('[');
					sb.Append(PrintObjects(((RawElement)obj).Children));
					sb.Append(']');
				} else if (obj is RawText) {
					sb.Append('~');
				} else {
					throw new Exception("Should not be here: " + obj);
				}
			}
			return sb.ToString();
		}
		
		RawDocument ReadDocument()
		{
			RawDocument doc = new RawDocument() { Parser = this };
			
			List<RawObject> objs = ReadFlatObjects();
			Log("Flat stream: {0}", PrintObjects(objs));
			List<RawObject> valid = MatchTags(objs);
			Log("Fixed stream: {0}", PrintObjects(valid));
			IEnumerator<RawObject> validStream = valid.GetEnumerator();
			validStream.MoveNext(); // Move to first
			while(true) {
				// End of stream?
				try {
					if (validStream.Current == null) break;
				} catch (InvalidCastException) {
					break;
				}
				doc.AddChild(ReadTextOrElement(validStream));
			}
			
			if (doc.Children.Count > 0) {
				doc.StartOffset = doc.FirstChild.StartOffset;
				doc.EndOffset = doc.LastChild.EndOffset;
			}
			
			OnParsed(doc);
			return doc;
		}
		
		RawObject ReadSingleObject(IEnumerator<RawObject> objStream)
		{
			RawObject obj = objStream.Current;
			objStream.MoveNext();
			return obj;
		}
		
		RawObject ReadTextOrElement(IEnumerator<RawObject> objStream)
		{
			AssertIsEndOfFile();
			
			RawObject curr = objStream.Current;
			if (curr is RawText || curr is RawElement) {
				return ReadSingleObject(objStream);
			} else {
				RawTag currTag = (RawTag)curr;
				if (currTag == StartTagPlaceholder) {
					return ReadElement(objStream);
				} else if (currTag.IsStartOrEmptyTag) {
					return ReadElement(objStream);
				} else {
					return ReadSingleObject(objStream);
				}
			}
		}
		
		RawElement ReadElement(IEnumerator<RawObject> objStream)
		{
			AssertIsEndOfFile();
			
			RawElement element = new RawElement();
			element.IsProperlyNested = true;
			
			// Read start tag
			RawTag startTag = ReadSingleObject(objStream) as RawTag;
			DebugAssert(startTag != null, "Start tag expected");
			DebugAssert(startTag.IsStartOrEmptyTag || startTag == StartTagPlaceholder, "Start tag expected");
			if (startTag == StartTagPlaceholder) {
				element.HasStartOrEmptyTag = false;
				element.IsProperlyNested = false;
				OnSyntaxError(element, objStream.Current.StartOffset, objStream.Current.EndOffset,
				              "Matching openning tag was not found");
			} else {
				element.HasStartOrEmptyTag = true;
				element.AddChild(startTag);
			}
			
			// Read content and end tag
			if (element.StartTag.IsStartTag || startTag == StartTagPlaceholder) {
				while(true) {
					RawTag currTag = objStream.Current as RawTag; // Peek
					if (currTag == EndTagPlaceholder) {
						OnSyntaxError(element, element.LastChild.EndOffset, element.LastChild.EndOffset,
						              "Expected '</{0}>'", element.StartTag.Name);
						ReadSingleObject(objStream);
						element.HasEndTag = false;
						element.IsProperlyNested = false;
						break;
					} else if (currTag != null && currTag.IsEndTag) {
						if (currTag.Name != element.StartTag.Name) {
							OnSyntaxError(element, currTag.StartOffset + 2, currTag.StartOffset + 2 + currTag.Name.Length,
							              "Expected '{0}'.  End tag must have same name as start tag.", element.StartTag.Name);
						}
						element.AddChild(ReadSingleObject(objStream));
						element.HasEndTag = true;
						break;
					}
					RawObject nested = ReadTextOrElement(objStream);
					if (nested is RawElement) {
						if (!((RawElement)nested).IsProperlyNested)
							element.IsProperlyNested = false;
						element.AddChildren(Split((RawElement)nested).ToList());
					} else {
						element.AddChild(nested);
					}
				}
			} else {
				element.HasEndTag = false;
			}
			
			element.StartOffset = element.FirstChild.StartOffset;
			element.EndOffset = element.LastChild.EndOffset;
			
			OnParsed(element); // Need all elements in cache for offset tracking
			return element;
		}
		
		IEnumerable<RawObject> Split(RawElement elem)
		{
			int myIndention = GetIndentLevel(elem);
			// If has virtual end and is indented
			if (!elem.HasEndTag && myIndention != -1) {
				int lastAccepted = 0; // Accept start tag
				while (lastAccepted + 1 < elem.Children.Count - 1 /* no end tag */) {
					RawObject nextItem = elem.Children[lastAccepted + 1];
					if (nextItem is RawText) {
						lastAccepted++; continue;  // Accept
					} else {
						// Include all more indented items
						if (GetIndentLevel(nextItem) > myIndention) {
							lastAccepted++; continue;  // Accept
						} else {
							break;  // Reject
						}
					}
				}
				// Accepted everything?
				if (lastAccepted + 1 == elem.Children.Count - 1) {
					yield return elem;
					yield break;
				}
				Log("Splitting {0} - take {1} of {2} nested", elem, lastAccepted, elem.Children.Count - 2);
				RawElement topHalf = new RawElement();
				topHalf.HasStartOrEmptyTag = elem.HasStartOrEmptyTag;
				topHalf.HasEndTag = elem.HasEndTag;
				topHalf.AddChildren(elem.Children.Take(lastAccepted + 1));    // Start tag + nested
				topHalf.StartOffset = topHalf.FirstChild.StartOffset;
				topHalf.EndOffset = topHalf.LastChild.EndOffset;
				OnSyntaxError(topHalf, topHalf.LastChild.EndOffset, topHalf.LastChild.EndOffset,
						              "Expected '</{0}>'", topHalf.StartTag.Name);
				
				Log("Constructed {0}", topHalf);
				yield return topHalf;
				for(int i = lastAccepted + 1; i < elem.Children.Count - 1; i++) {
					yield return elem.Children[i];
				}
			} else {
				yield return elem;
			}
		}
		
		int GetIndentLevel(RawObject obj)
		{
			int offset = obj.StartOffset - 1;
			int level = 0;
			while(true) {
				if (offset < 0) break;
				char c = input[offset];
				if (c == ' ') {
					level++;
				} else if (c == '\t') {
					level += 4;
				} else if (c == '\r' || c == '\n') {
					break;
				} else {
					return -1;
				}
				offset--;
			}
			return level;
		}
		
		#endregion
		
		#region Matching heuristics
		
		const int maxConfigurationCount = 10;
		
		/// <summary>
		/// Stack of still unmatched start tags.
		/// It includes the cost and backtack information.
		/// </summary>
		class Configuration
		{
			/// <summary> Unmatched start tags </summary>
			public ImmutableStack<RawTag> StartTags { get; set; }
			/// <summary> Properly nested tags </summary>
			public ImmutableStack<RawObject> Document { get; set; }
			/// <summary> Number of needed modificaitons to the document </summary>
			public int Cost { get; set; }
		}
		
		/// <summary>
		/// Dictionary which stores the cheapest configuration
		/// </summary>
		class Configurations: Dictionary<ImmutableStack<RawTag>, Configuration>
		{
			public Configurations()
			{
			}
			
			public Configurations(IEnumerable<Configuration> configs)
			{
				foreach(Configuration config in configs) {
					this.Add(config);
				}
			}
			
			/// <summary> Overwrite only if cheaper </summary>
			public void Add(Configuration newConfig)
			{
				Configuration oldConfig;
				if (this.TryGetValue(newConfig.StartTags, out oldConfig)) {
					if (newConfig.Cost < oldConfig.Cost) {
						this[newConfig.StartTags] = newConfig;
					}
				} else {
					base.Add(newConfig.StartTags, newConfig);
				}
			}
			
			public override string ToString()
			{
				StringBuilder sb = new StringBuilder();
				foreach(var kvp in this) {
					sb.Append("\n - '");
					foreach(RawTag startTag in kvp.Value.StartTags.Reverse()) {
						sb.Append('<');
						sb.Append(startTag.Name);
						sb.Append('>');
					}
					sb.AppendFormat("' = {0}", kvp.Value.Cost);
				}
				return sb.ToString();
			}
		}
		
		// Tags used to guide the element creation
		readonly RawTag StartTagPlaceholder = new RawTag();
		readonly RawTag EndTagPlaceholder = new RawTag();
		
		/// <summary>
		/// Add start or end tag placeholders so that the documment is properly nested
		/// </summary>
		List<RawObject> MatchTags(IEnumerable<RawObject> objs)
		{
			Configurations configurations = new Configurations();
			configurations.Add(new Configuration {
				StartTags = ImmutableStack<RawTag>.Empty,
				Document = ImmutableStack<RawObject>.Empty,
				Cost = 0,
			});
			foreach(RawObject obj in objs) {
				configurations = ProcessObject(configurations, obj);
			}
			// Close any remaining start tags
			foreach(Configuration conifg in configurations.Values) {
				while(!conifg.StartTags.IsEmpty) {
					conifg.StartTags = conifg.StartTags.Pop();
					conifg.Document = conifg.Document.Push(EndTagPlaceholder);
					conifg.Cost += 1;
				}
			}
			Log("Configurations after closing all remaining tags:" + configurations.ToString());
			Configuration bestConfig = configurations.Values.OrderBy(v => v.Cost).First();
			Log("Best configuration has cost {0}", bestConfig.Cost);
			
			return bestConfig.Document.Reverse().ToList();
		}
		
		/// <summary> Get posible configurations after considering fiven object </summary>
		Configurations ProcessObject(Configurations oldConfigs, RawObject obj)
		{
			Log("Processing {0}", obj);
			
			RawTag tag = obj as RawTag;
			Assert(obj is RawTag || obj is RawText || obj is RawElement, obj.GetType().Name + " not expected");
			if (obj is RawElement)
				Assert(((RawElement)obj).IsProperlyNested, "Element not proprly nested");
			
			Configurations newConfigs = new Configurations();
			
			foreach(var kvp in oldConfigs) {
				Configuration oldConfig = kvp.Value;
				var oldStartTags = oldConfig.StartTags;
				var oldDocument = oldConfig.Document;
				int oldCost = oldConfig.Cost;
				
				if (tag != null && tag.IsStartTag) {
					newConfigs.Add(new Configuration {                    // Push start-tag (cost 0)
						StartTags = oldStartTags.Push(tag),
						Document = oldDocument.Push(tag),
						Cost = oldCost,
					});
				} else if (tag != null && tag.IsEndTag) {
					newConfigs.Add(new Configuration {                    // Ignore (cost 1)
						StartTags = oldStartTags,
						Document = oldDocument.Push(StartTagPlaceholder).Push(tag),
						Cost = oldCost + 1,
	               });
					if (!oldStartTags.IsEmpty && oldStartTags.Peek().Name != tag.Name) {
						newConfigs.Add(new Configuration {                // Pop 1 item (cost 1) - not mathcing
							StartTags = oldStartTags.Pop(),
							Document = oldDocument.Push(tag),
							Cost = oldCost + 1,
		               });
					}
					int popedCount = 0;
					var startTags = oldStartTags;
					var doc = oldDocument;
					foreach(RawTag poped in oldStartTags) {
						popedCount++;
						if (poped.Name == tag.Name) {
							newConfigs.Add(new Configuration {             // Pop 'x' items (cost x-1) - last one is matching
								StartTags = startTags.Pop(),
								Document = doc.Push(tag),
								Cost = oldCost + popedCount - 1,
							});
						}
						startTags = startTags.Pop();
						doc = doc.Push(EndTagPlaceholder);
					}
				} else {
					// Empty tag  or  other tag type  or  text  or  properly nested element
					newConfigs.Add(new Configuration {                    // Ignore (cost 0)
						StartTags = oldStartTags,
						Document = oldDocument.Push(obj),
						Cost = oldCost,
	               });
				}
			}
			
			// Log("New configurations:" + newConfigs.ToString());
			
			Configurations bestNewConfigurations = new Configurations(
				newConfigs.Values.OrderBy(v => v.Cost).Take(maxConfigurationCount)
			);
			
			Log("Best new configurations:" + bestNewConfigurations.ToString());
			
			return bestNewConfigurations;
		}
		
		#endregion
	}
}
