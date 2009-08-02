// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.AvalonEdit.Document;

// Missing XML comment
#pragma warning disable 1591

namespace ICSharpCode.AvalonEdit.XmlParser
{
	public class XmlParser
	{
		RawDocument userDocument;
		TextDocument textDocument;
		TextSegmentCollection<RawObject> userDom;
		TextSegmentCollection<RawObject> parsedDom;
		
		public XmlParser(TextDocument textDocument)
		{
			this.userDocument = new RawDocument();
			this.textDocument = textDocument;
			this.userDom = new TextSegmentCollection<RawObject>(textDocument);
			this.parsedDom = new TextSegmentCollection<RawObject>(textDocument);
			this.textDocument.Changed += TextDocument_Changed;
			this.userDocument.ObjectAttached += delegate(object sender, RawObjectEventArgs e) {
				this.userDom.Add(e.Object);
			};
			this.userDocument.ObjectDettached += delegate(object sender, RawObjectEventArgs e) {
				this.userDom.Remove(e.Object);
			};
		}
		
		public RawDocument Parse()
		{
			currentLocation = 0;
			input = textDocument.Text;
			
			RawDocument parsedDocument = ReadDocument();
			userDocument.UpdateDataFrom(parsedDocument);
			return userDocument;
		}

		void TextDocument_Changed(object sender, DocumentChangeEventArgs e)
		{
			int start = e.Offset - 2;
			int end = e.Offset + e.InsertionLength + 2;
			start = Math.Max(Math.Min(start, textDocument.TextLength - 1), 0);
			end = Math.Max(Math.Min(end, textDocument.TextLength - 1), 0);
			foreach(RawObject obj in parsedDom.FindOverlappingSegments(start, end - start)) {
				parsedDom.Remove(obj);
				Log("Removed cached item: {0}", obj);
			}
		}
		
		T ReadFromCache<T>(int location) where T: RawObject
		{
			RawObject obj = parsedDom.FindFirstSegmentWithStartAfter(location);
			while(obj != null && obj.StartOffset == location) {
				if (obj is T) {
					currentLocation += obj.Length;
					return (T)obj;
				}
				obj = parsedDom.GetNextSegment(obj);
			}
			return null;
		}
		
		void Log(string text, params object[] pars)
		{
			System.Diagnostics.Debug.WriteLine("XML Parser: " + text, pars);
		}
		
		void LogParsed(RawObject obj)
		{
			System.Diagnostics.Debug.WriteLine("XML Parser: Parsed " + obj.ToString());
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
		
		bool TryMoveTo(params char[] c)
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
				return input.Substring(start, end - start);
			}
		}
		
		static char[] WhiteSpaceChars = new char[] {' ', '\n', '\r', '\t'};
		static char[] WhiteSpaceAndReservedChars = new char[] {' ', '\n', '\r', '\t', '<', '=', '>', '/'};
		
		bool? IsWhiteSpace()
		{
			if (currentLocation == input.Length) {
				return null;
			} else {
				return WhiteSpaceChars.Contains(input[currentLocation]);
			}
		}
		
		bool? IsWhiteSpaceOrReserved()
		{
			if (currentLocation == input.Length) {
				return null;
			} else {
				return WhiteSpaceAndReservedChars.Contains(input[currentLocation]);
			}
		}
		
		string ReadName()
		{
			Debug.Assert(HasMoreData());
			
			int start = currentLocation;
			TryMoveTo(WhiteSpaceAndReservedChars.ToArray());
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
					doc.Children.Add(ReadElement(doc));
				} else {
					doc.Children.Add(ReadCharacterData(doc));
				}
			}
			doc.EndOffset = currentLocation;
			
			LogParsed(doc);
			parsedDom.Add(doc);
			return doc;
		}
		
		RawElement ReadElement(RawObject parent)
		{
			Debug.Assert(HasMoreData() && TryPeek('<'));
			
			RawElement element = ReadFromCache<RawElement>(currentLocation);
			if (element != null) return element;

			element = new RawElement() { Parent = parent };
			
			element.StartOffset = currentLocation;
			element.StartTag = ReadTag(element);
			// Read content
			if (element.StartTag.ClosingBracket == ">") {
				while(true) {
					if (IsEndOfFile()) {
						break;
					} else if (TryPeek('<')) {
						if (TryPeek("</")) break;
						element.Children.Add(ReadElement(element));
					} else {
						element.Children.Add(ReadCharacterData(element));
					}
				}
			}
			// Read closing tag
			if (TryPeek("</")) {
				element.HasEndTag = true;
				element.EndTag = ReadTag(element);
			}
			element.EndOffset = currentLocation;
			
			LogParsed(element);
			parsedDom.Add(element);
			return element;
		}
		
		RawTag ReadTag(RawObject parent)
		{
			Debug.Assert(HasMoreData() && TryPeek('<'));
			
			RawTag tag = ReadFromCache<RawTag>(currentLocation);
			if (tag != null) return tag;
			
			tag = new RawTag() { Parent = parent };
			
			tag.StartOffset = currentLocation;
			if (TryRead('<')) {
				tag.OpeningBracket = "<";
				if (TryRead('/')) {
					tag.OpeningBracket += "/";
				}
			}
			if (HasMoreData()) {
				tag.Name = ReadName();
			}
			// Read attributes
			while(true) {
				if (IsWhiteSpace() == true) {
					tag.Attributes.Add(ReadWhiteSpace(tag));
				}
				if (TryRead('>')) {
					tag.ClosingBracket = ">";
					break;
				} else 	if (TryRead('/')) {
					tag.ClosingBracket = "/";
					if (TryRead('>')) {
						tag.ClosingBracket += ">";
					}
					break;
				}
				if (TryPeek('<')) break;
				if (HasMoreData()) {
					tag.Attributes.Add(ReadAttribulte(tag));
					continue;
				}
				break;
			}
			tag.EndOffset = currentLocation;
			
			LogParsed(tag);
			parsedDom.Add(tag);
			return tag;
		}
		
		RawText ReadWhiteSpace(RawObject parent)
		{
			Debug.Assert(HasMoreData() && IsWhiteSpace() == true);
			
			RawText ws = ReadFromCache<RawText>(currentLocation);
			if (ws != null) return ws;
			
			ws = new RawText() { Parent = parent };
			
			ws.StartOffset = currentLocation;
			int start = currentLocation;
			while(IsWhiteSpace() == true) currentLocation++;
			ws.Value = GetText(start, currentLocation);
			ws.EndOffset = currentLocation;
			
			parsedDom.Add(ws);
			return ws;
		}
		
		RawAttribute ReadAttribulte(RawObject parent)
		{
			Debug.Assert(HasMoreData());
			
			RawAttribute attr = ReadFromCache<RawAttribute>(currentLocation);
			if (attr != null) return attr;
			
			attr = new RawAttribute() { Parent = parent };
			
			attr.StartOffset = currentLocation;
			if (HasMoreData()) {
				attr.Name = ReadName();
			}
			int checkpoint = currentLocation;
			attr.EqualsSign = string.Empty; 
			if (IsWhiteSpace() == true) attr.EqualsSign += ReadWhiteSpace(attr).Value;
			if (TryRead('=')) {
				attr.EqualsSign += "=";
				if (IsWhiteSpace() == true) attr.EqualsSign += ReadWhiteSpace(attr).Value;
				if (IsWhiteSpaceOrReserved() == false) {
					// Read attribute value
					int start = currentLocation;
					if (TryRead('"')) {
						TryMoveTo('"', '<');
						TryRead('"');
						attr.Value = GetText(start, currentLocation);
					} else if (TryRead('\'')) {
						TryMoveTo('\'', '<');
						TryRead('\'');
						attr.Value = GetText(start, currentLocation);
					} else {
						attr.Value = ReadName();
					}
				}
			} else {
				attr.EqualsSign = null;
				currentLocation = checkpoint;
			}
			attr.EndOffset = currentLocation;
			
			parsedDom.Add(attr);
			return attr;
		}
		
		RawText ReadCharacterData(RawObject parent)
		{
			Debug.Assert(HasMoreData());
			
			RawText charData = ReadFromCache<RawText>(currentLocation);
			if (charData != null) return charData;
			
			charData = new RawText() { Parent = parent };
			
			charData.StartOffset = currentLocation;
			int start = currentLocation;
			TryMoveTo('<');
			charData.Value = GetText(start, currentLocation);
			charData.EndOffset = currentLocation;
			
			parsedDom.Add(charData);
			return charData;
		}
	}
}
