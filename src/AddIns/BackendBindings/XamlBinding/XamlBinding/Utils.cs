// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Description of Utils.
	/// </summary>
	public static class Utils
	{
		internal static bool IsReaderAtTarget(XmlTextReader r, int line, int col)
		{
			if (r.LineNumber > line)
				return true;
			else if (r.LineNumber == line)
				return r.LinePosition >= col;
			else
				return false;
		}
		
		public static MarkupExtensionInfo GetInnermostMarkupExtensionInfo(MarkupExtensionInfo info)
		{
			var lastNamed = info.NamedArguments.LastOrDefault();
			var lastPositional = info.PositionalArguments.LastOrDefault();
			
			if (lastNamed.Value != null) {
				if (lastNamed.Value.IsString)
					return info;
				
				return GetInnermostMarkupExtensionInfo(lastNamed.Value.ExtensionValue);
			} else {
				if (lastPositional != null) {
					if (lastPositional.IsString)
						return info;
					
					return GetInnermostMarkupExtensionInfo(lastPositional.ExtensionValue);
				}
			}
			
			return info;
		}
		
		public static string GetAttributeValue(string text, int line, int col, string name)
		{
			try {
				XmlReader reader = CreateReaderAtTarget(text, line, col);
				
				if (!reader.MoveToFirstAttribute()) {
					/*				int offset = GetOffsetFromFilePos(text, line, col) + 1;
				
					if (XmlParser.IsInsideAttributeValue(text, offset))
						text = text.Substring(0, offset) + "\">";
					else {
						if (!string.IsNullOrEmpty(XmlParser.GetAttributeNameAtIndex(text, offset)))
							text = text.Substring(0, offset) + "=\"\">";
						else
							text = text.Substring(0, offset) + ">";
					}
					reader = CreateReaderAtTarget(text, line, col);
					if (!reader.MoveToFirstAttribute()) */
					return null;
				}
				do {
					LoggingService.Debug("name: " + reader.Name + " value: " + reader.Value);
					string plainName = reader.Name.ToUpperInvariant();
					
					if (plainName == name.ToUpperInvariant())
						return reader.Value;
				} while (reader.MoveToNextAttribute());
			} catch (XmlException e) {
				Debug.Print(e.ToString());
			}
			
			return null;
		}
		
		public static int GetOffsetFromValueStart(string xaml, int offset)
		{
			if (xaml == null)
				throw new ArgumentNullException("xaml");
			if (offset < 0 || offset > xaml.Length)
				throw new ArgumentOutOfRangeException("offset", offset, "Value must be between 0 and " + xaml.Length);
			
			int start = offset;
			
			while (start > 0 && XmlParser.IsInsideAttributeValue(xaml, start))
				start--;
			
			return offset - start - 1;
		}
		
		public static string[] GetListOfExistingAttributeNames(string text, int line, int col)
		{
			List<string> list = new List<string>();
			
			if (text == null)
				return list.ToArray();
			
			using (XmlReader reader = CreateReaderAtTarget(text, line, col)) {
				try {
					if (!reader.MoveToFirstAttribute())
						return list.ToArray();
					
					do {
						LoggingService.Debug("name: " + reader.Name + " value: " + reader.Value);
						list.Add(reader.Name);
					} while (reader.MoveToNextAttribute());
				} catch (XmlException e) {
					Debug.Print(e.ToString());
				}
			}
			
			foreach (var item in list)
				Debug.Print(item);
			
			return list.ToArray();
		}
		
		public static int GetPreviousLTCharPos(string text, int startIndex)
		{
			if (text == null)
				throw new ArgumentNullException("text");
			
			if (startIndex < 0)
				return -1;
			if (startIndex >= text.Length)
				startIndex = text.Length - 1;
			
			while (startIndex > -1 && text[startIndex] != '<')
				startIndex--;
			
			return startIndex;
		}
		
		static char[] whitespace = new char[] {' ', '\t', '\n', '\r'};
		
		public static string GetXamlNamespacePrefix(XamlContext context)
		{
			var item = context.XmlnsDefinitions.FirstOrDefault(i => i.Value == CompletionDataHelper.XamlNamespace);

			if (item.Key != null)
				return item.Key;
			return string.Empty;
		}
		
		public static bool IsInsideXmlComment(string xaml, int offset)
		{
			if (xaml == null)
				throw new ArgumentNullException("xaml");
			if (offset < 0)
				throw new ArgumentOutOfRangeException("offset", offset, "Value must be between 0 and " + (xaml.Length - 1));
			
			if (offset >= xaml.Length && offset > 0)
				offset = xaml.Length - 1;
			
			string interestingPart = xaml.Substring(0, offset);
			int end = interestingPart.LastIndexOf("-->", StringComparison.OrdinalIgnoreCase);
			
			interestingPart = (end > -1) ? interestingPart.Substring(end, interestingPart.Length - end) : interestingPart;
			
			return interestingPart.LastIndexOf("<!--", StringComparison.OrdinalIgnoreCase) != -1;
		}
		
		public static int GetOffsetFromFilePos(string content, int line, int col)
		{
			if (line < 1)
				return 0;
			if (line == 1)
				return (col > 0) ? col - 1 : 0;
			
			int offset = -1;
			
			while (line > 1) {
				int tmp = content.IndexOf('\n', offset + 1);
				if (tmp > -1) {
					offset = tmp;
					line--;
				} else {
					return content.Length;
				}
			}
			
			return offset + col - 1;
		}
		
		public static int GetParentElementStart(ITextEditor editor)
		{
			Stack<int> offsetStack = new Stack<int>();
			using (XmlTextReader xmlReader = new XmlTextReader(new StringReader(editor.Document.GetText(0, editor.Caret.Offset)))) {
				try {
					xmlReader.XmlResolver = null; // prevent XmlTextReader from loading external DTDs
					while (xmlReader.Read()) {
						switch (xmlReader.NodeType) {
							case XmlNodeType.Element:
								if (!xmlReader.IsEmptyElement) {
									offsetStack.Push(editor.Document.PositionToOffset(xmlReader.LineNumber, xmlReader.LinePosition));
								}
								break;
							case XmlNodeType.EndElement:
								offsetStack.Pop();
								break;
						}
					}
				} catch (XmlException) { }
			}
			
			return (offsetStack.Count > 0) ? offsetStack.Pop() : -1;
		}
		
		public 	static void LookUpInfoAtTarget(string fileContent, int caretLine, int caretColumn, int offset,
		                                       out Dictionary<string, string> xmlns, out QualifiedName activeOrParent, out bool isParent, out int activeElementStartIndex)
		{
			var watch = new Stopwatch();
			watch.Start();
			
			Stack<QualifiedName> stack = new Stack<QualifiedName>();
			isParent = false;

			XmlTextReader r = new XmlTextReader(new StringReader(fileContent));
			r.XmlResolver = null;
			try {
				r.WhitespaceHandling = WhitespaceHandling.Significant;
				// move reader to correct position
				while (r.Read() && !IsReaderAtTarget(r, caretLine, caretColumn)) {
					switch (r.NodeType) {
						case XmlNodeType.EndElement:
							stack.PopOrDefault();
							break;
						case XmlNodeType.Element:
							if (!r.IsEmptyElement)
								stack.Push(new QualifiedName(r.LocalName, r.NamespaceURI, r.Prefix));
							break;
					}
				}
			} catch (XmlException) {
			} finally {
				xmlns = new Dictionary<string, string>(r.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml));
			}
			
			activeElementStartIndex = XmlParser.GetActiveElementStartIndex(fileContent, offset + 1);
			activeOrParent = CompletionDataHelper.ResolveCurrentElement(fileContent, activeElementStartIndex, xmlns);

			if (activeOrParent == null) {
				activeOrParent = stack.PopOrDefault();
				isParent = true;
			}
			
			watch.Stop();
			
			Core.LoggingService.Debug("LookUpInfoAtTarget took " + watch.ElapsedMilliseconds + "ms");
		}
		
		public 	static XmlTextReader CreateReaderAtTarget(string fileContent, int caretLine, int caretColumn)
		{
			var watch = Stopwatch.StartNew();
			XmlTextReader r = new XmlTextReader(new StringReader(fileContent));
			r.XmlResolver = null;
			
			try {
				r.WhitespaceHandling = WhitespaceHandling.Significant;
				// move reader to correct position
				while (r.Read() && !IsReaderAtTarget(r, caretLine, caretColumn)) { }
			} catch (XmlException) {}
			
			watch.Stop();
			
			Core.LoggingService.Debug("CreateReaderAtTarget took " + watch.ElapsedMilliseconds + "ms");
			
			return r;
		}
		
		/// <summary>
		/// Gets the of a markup extension at the given position.
		/// </summary>
		/// <param name="info">The markup extension data to parse.</param>
		/// <param name="offset">The offset to look at.</param>
		/// <returns>
		/// A string, if the at offset is the extension type. <br />
		/// An AttributeValue, if at the offset is a positional argument. <br />
		/// A KeyValuePair&lt;string, AttributeValue&gt;, if at the offset is a named argument.
		/// </returns>
		public static object GetMarkupDataAtPosition(MarkupExtensionInfo info, int offset)
		{
			object previous = info.ExtensionType;
			
			foreach (var item in info.PositionalArguments) {
				if (item.StartOffset > offset)
					break;
				previous = item.IsString ? item : GetMarkupDataAtPosition(item.ExtensionValue, offset - item.StartOffset);
			}
			
			foreach (var pair in info.NamedArguments) {
				if (pair.Value.StartOffset > offset)
					break;
				previous = pair.Value.IsString ? pair : GetMarkupDataAtPosition(pair.Value.ExtensionValue, offset - pair.Value.StartOffset);
			}
			
			return previous;
		}
		
		public static MarkupExtensionInfo GetMarkupExtensionAtPosition(MarkupExtensionInfo info, int offset)
		{
			MarkupExtensionInfo tmp = info;
			
			foreach (var item in info.PositionalArguments) {
				if (item.StartOffset > offset)
					break;
				tmp = item.IsString ? tmp : GetMarkupExtensionAtPosition(item.ExtensionValue, offset - item.StartOffset);
			}
			
			foreach (var pair in info.NamedArguments) {
				if (pair.Value.StartOffset > offset)
					break;
				tmp = pair.Value.IsString ? tmp : GetMarkupExtensionAtPosition(pair.Value.ExtensionValue, offset - pair.Value.StartOffset);
			}
			
			return tmp;
		}
	}
}