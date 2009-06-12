// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Editor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Description of Utils.
	/// </summary>
	public static class Utils
	{
		public static bool HasMatchingEndTag(string tagname, string text, int offset)
		{
			int index = XmlParser.GetActiveElementStartIndex(text, offset);
			if (index == -1)
				return false;
			
			text = text.Substring(index);
			
			var path = XmlParser.GetActiveElementStartPathAtIndex(text, index);
			
			XmlReader reader = XmlTextReader.Create(new StringReader(text));
			int startTags = 0;
			try {
				while (reader.Read()) {
					switch (reader.NodeType) {
						case XmlNodeType.Element:
							if (!reader.IsEmptyElement)
								startTags++;
							break;
						case XmlNodeType.EndElement:
							startTags--;
							if (startTags == 0 && tagname == reader.Name) {
								return true;
							}
							break;
					}
				}
			} catch (XmlException e) {
				Debug.Print(e.ToString());
				return false;
			}
			
			return false;
		}
		
		public static string GetAttributeValue(string text, int offset, string name)
		{
			text = SimplifyToSingleElement(text, offset, "Test");
			
			if (text == null)
				return null;
			
			XmlTextReader reader = new XmlTextReader(new StringReader(text));
			reader.XmlResolver = null;
			
			try {
				reader.ReadToFollowing("Test");
				
				if (!reader.MoveToFirstAttribute())
					return null;
				
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
		
		public static Dictionary<string, string> GetXmlNamespacesForOffset(string fileContent, int offset)
		{
			if (fileContent == null)
				throw new ArgumentNullException("fileContent");
			if (offset < 0 || offset > fileContent.Length)
				throw new ArgumentOutOfRangeException("offset", offset, "Value must be between 0 and " + fileContent.Length);
			
			var map = new Dictionary<string, string>();
			
			int endIndex = fileContent.IndexOfAny(new char[] {'<', '>'}, offset);
			if (endIndex > -1)
				fileContent = fileContent.Substring(0, endIndex + 1);
			
			int lastWhiteSpacePos = fileContent.LastIndexOfAny(new char[] {' ', '\t', '\n', '\r'});
			
			bool inDouble = false, inSingle = false;
			
			for (int i = 0; i < fileContent.Length; i++) {
				if (fileContent[i] == '"' && !inSingle)
					inDouble = !inDouble;
				if (fileContent[i] == '\'' && !inDouble)
					inSingle = !inSingle;
				if (fileContent[i] == '>') {
					int lastDelimiterPos = fileContent.Substring(0, i + 1).LastIndexOfAny(new char[] {'>', '/'});
					if (inDouble) {
						fileContent.Insert(lastDelimiterPos, "\"");
						i++;
						inDouble = false;
					}
					if (inSingle) {
						fileContent.Insert(lastDelimiterPos, "'");
						inSingle = false;
						i++;
					}
				}
			}

			fileContent = fileContent.Replace("<?", " ").Replace("?>", " ").Replace("<", " ").Replace("/>", " ").Replace(">", " ").Replace("\n", " ").Replace("\r", " ").Replace("\t", " ");
			while (fileContent.Contains("  "))
				fileContent = fileContent.Replace("  ", " ");

			fileContent = fileContent.Replace("= \"", "=\"");
			fileContent = fileContent.Replace(" =\"", "=\"");
			
			Debug.Print(fileContent);
			
			string[] data = fileContent.Split(' ');
			
			var filter1 = data.Where(s => s.StartsWith("xmlns"));
			
			foreach (string item in filter1) {
				string[] parts = item.Split(new char[] {'='}, 2);
				if (parts.Length == 2) {
					if (map.ContainsKey(parts[0])) // replace namespace with new one
						map.Remove(parts[0]);
					map.Add(parts[0], parts[1].Trim('"', '\''));
				}
			}
			
			return map;
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
		
		public static string[] GetListOfExistingAttributeNames(string text, int offset)
		{
			List<string> list = new List<string>();
			
			text = SimplifyToSingleElement(text, offset, "Test");
			
			if (text == null)
				return list.ToArray();
			
			XmlReader reader = XmlTextReader.Create(new StringReader(text));
			
			try {
				reader.ReadToFollowing("Test");
				
				if (!reader.MoveToFirstAttribute())
					return list.ToArray();
				
				do {
					LoggingService.Debug("name: " + reader.Name + " value: " + reader.Value);
					list.Add(reader.Name);
				} while (reader.MoveToNextAttribute());
			} catch (XmlException e) {
				Debug.Print(e.ToString());
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

		static string SimplifyToSingleElement(string text, int offset, string name)
		{
			int index = XmlParser.GetActiveElementStartIndex(text, offset);
			if (index == -1) return null;
			index = text.IndexOfAny(whitespace, index);
			if (index == -1) return null;
			string newText = text.Substring(index);
			int endIndex = newText.IndexOfAny(new char[] { '<', '>' });
			if (endIndex == -1)
				endIndex = newText.Length;
			newText = newText.Substring(0, endIndex).Trim(' ', '\t', '\n', '\r', '/');
			LoggingService.Debug("text: '" + newText + "'");
			if (!newText.EndsWith("\"") && newText.LastIndexOfAny(whitespace) > -1) {
				newText = newText.Substring(0, newText.LastIndexOfAny(whitespace));
			}
			
			string namespaceDecls = "";
			
			var list = Utils.GetXmlNamespacesForOffset(text, offset);
			
			foreach (var item in list) {
				namespaceDecls += item.Key + "=\"" + item.Value + "\" ";
			}
			
			text = "<" + name + " " + newText + " " + namespaceDecls + " />";
			
			return text;
		}
		
		public static string GetXamlNamespacePrefix(string xaml, int offset)
		{
			var list = Utils.GetXmlNamespacesForOffset(xaml, offset);
			var item = list.FirstOrDefault(i => i.Value == CompletionDataHelper.XamlNamespace);
			
			if (item.Key.StartsWith("xmlns:", StringComparison.OrdinalIgnoreCase))
				return item.Key.Substring("xmlns:".Length);
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
	}
}
