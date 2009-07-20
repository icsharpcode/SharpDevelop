// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.NRefactory;
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
		
		static char[] whitespace = new char[] {' ', '\t', '\n', '\r'};
		
		public static string GetXamlNamespacePrefix(XamlContext context)
		{
			return GetNamespacePrefix(CompletionDataHelper.XamlNamespace, context);
		}
		
		public static string GetNamespacePrefix(string namespaceUri, XamlContext context)
		{
			var item = context.XmlnsDefinitions.FirstOrDefault(i => i.Value == namespaceUri);

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
		
		static QualifiedNameWithLocation ResolveCurrentElement(string text, int offset, Dictionary<string, string> xmlnsDefinitions)
		{
			if (offset < 0)
				return null;
			
			string elementName = text.GetWordAfterOffset(offset + 1).Trim('>', '<');
			
			string prefix = "";
			string element = "";
			
			Location location = GetLocationInfoFromOffset(text, offset + 1);
			
			if (elementName.IndexOf(':') > -1) {
				string[] data = elementName.Split(':');
				prefix = data[0];
				element = data[1];
			} else {
				element = elementName;
			}
			
			string xmlns = xmlnsDefinitions.ContainsKey(prefix) ? xmlnsDefinitions[prefix] : string.Empty;
			
			return new QualifiedNameWithLocation(element, xmlns, prefix, location.Line, location.Column);
		}
		
		public static Location GetLocationInfoFromOffset(string text, int offset)
		{
			string[] lines = text.Substring(0, offset).Split('\n');
			string line = lines.LastOrDefault() ?? string.Empty;
			
			return new Location(line.Length + 1, lines.Length);
		}
		
		class IgnoredXmlnsWrapper {
			public IEnumerable<string> Items { get; set; }
			public QualifiedNameWithLocation Item { get; set; }
		}
		
		public 	static void LookUpInfoAtTarget(string fileContent, int caretLine, int caretColumn, int offset,
		                                       out Dictionary<string, string> xmlns, out List<string> ignoredXmlns, out QualifiedNameWithLocation active,
		                                       out QualifiedNameWithLocation parent, out int activeElementStartIndex, out bool isRoot)
		{
			Stack<QualifiedNameWithLocation> stack = new Stack<QualifiedNameWithLocation>();
			Stack<IgnoredXmlnsWrapper> ignoredXmlnsStack = new Stack<IgnoredXmlnsWrapper>();
			
			isRoot = false;

			XmlTextReader r = new XmlTextReader(new StringReader(fileContent));
			r.XmlResolver = null;
			
			char[] separators = new char[] {
				' ', '\t', '\n', '\r'
			};
			
			ignoredXmlns = new List<string>();
			
			try {
				r.WhitespaceHandling = WhitespaceHandling.Significant;
				// move reader to correct position
				while (r.Read() && !IsReaderAtTarget(r, caretLine, caretColumn)) {
					switch (r.NodeType) {
						case XmlNodeType.EndElement:
							var stackItem = stack.PopOrDefault();
							if (ignoredXmlnsStack.Count > 0 && ignoredXmlnsStack.Peek().Item == stackItem)
								ignoredXmlnsStack.PopOrDefault();
							break;
						case XmlNodeType.Element:
							if (!r.IsEmptyElement) {
								var item = new QualifiedNameWithLocation(r.LocalName, r.NamespaceURI, r.Prefix, r.LineNumber, r.LinePosition);
								stack.Push(item);
								if (r.HasAttributes) {
									r.MoveToFirstAttribute();
									do {
										if (r.NamespaceURI == CompletionDataHelper.MarkupCompatibilityNamespace) {
											if (r.LocalName == "Ignorable") {
												ignoredXmlnsStack.Push(
													new IgnoredXmlnsWrapper() { Items = r.Value.Split(separators, StringSplitOptions.RemoveEmptyEntries), Item = item }
												);
											} else if (r.LocalName == "ProcessContent") {
												// TODO : add support for ProcessContent
											}
										}
									} while (r.MoveToNextAttribute());
								}
							}
							break;
					}
				}
			} catch (XmlException) {
			} finally {
				ignoredXmlns = ignoredXmlnsStack.SelectMany(item => item.Items).ToList();
				xmlns = new Dictionary<string, string>(r.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml));
			}
			
			activeElementStartIndex = XmlParser.GetActiveElementStartIndex(fileContent, Math.Min(offset + 1, fileContent.Length - 1));
			
			active =  ResolveCurrentElement(fileContent, activeElementStartIndex, xmlns);
			parent = stack.PopOrDefault();
			
			if (active == parent)
				parent = stack.PopOrDefault();
			
			if (parent == null)
				isRoot = true;
			
			if (active == null)
				active = parent;
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