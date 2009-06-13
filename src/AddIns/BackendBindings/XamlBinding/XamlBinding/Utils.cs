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
		
		public static string GetAttributeValue(string text, int line, int col, string name)
		{
			try {			
				XmlTextReader reader = new XmlTextReader(new StringReader(text));
				reader.XmlResolver = null;
				
				while (reader.Read() && !IsReaderAtTarget(reader, line, col)) { }
			

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
		
		public 	static XmlTextReader CreateReaderAtTarget(string fileContent, int caretLine, int caretColumn)
		{
			XmlTextReader r = new XmlTextReader(new StringReader(fileContent));
			r.XmlResolver = null;
			
			try {
				r.WhitespaceHandling = WhitespaceHandling.Significant;
				// move reader to correct position
				while (r.Read() && !IsReaderAtTarget(r, caretLine, caretColumn)) { }
			} catch (XmlException) {}
			
			return r;
		}
	}
}
