// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Description of Utils.
	/// </summary>
	public static class Utils
	{
		public static int MinMax(int value, int lower, int upper)
		{
			return Math.Min(Math.Max(value, lower), upper);
		}
		
		static readonly char[] whitespace = new[] {' ', '\t', '\n', '\r'};
		static readonly char[] newline = new[] {'\n', '\r'};
		
		public static string GetNamespacePrefix(string namespaceUri, XamlContext context)
		{
			var item = context.XmlnsDefinitions.FirstOrDefault(i => i.Value.XmlNamespace == namespaceUri);

			if (item.Key != null)
				return item.Key;
			return string.Empty;
		}
		
		/// <summary>
		/// Returns the offset for a given line, column position in a file.
		/// If the given position is not within the string it returns the first or the last offset respectively.
		/// </summary>
		/// <remarks>
		/// <paramref name="line"/> and <paramref name="col"/> are 1-based!
		/// </remarks>
		public static int GetOffsetFromFilePos(string content, int line, int col)
		{
			if (line < 1)
				return 0;
			if (line == 1)
				return (col > 0) ? col - 1 : 0;
			
			int offset = -1;
			
			while (line > 1) {
				int tmp = content.IndexOfAny(newline, offset + 1);
				if (tmp > -1) {
					if (content[tmp] == '\r' && content.Length > tmp + 1 && content[tmp + 1] == '\n')
						offset = tmp + 1;
					else
						offset = tmp;
					line--;
				} else {
					return content.Length;
				}
			}
			
			return offset + col;
		}
		
		public static TextLocation GetLocationInfoFromOffset(string text, int offset)
		{
			string[] lines = text.Substring(0, MinMax(offset, 0, text.Length)).Split('\n');
			string line = lines.LastOrDefault() ?? string.Empty;
			
			return new TextLocation(lines.Length, line.Length + 1);
		}
		
		/// <summary>
		/// Gets the of a markup extension at the given position.
		/// </summary>
		/// <param name="info">The markup extension data to parse.</param>
		/// <param name="offset">The offset to look at.</param>
		/// <returns>
		/// A string, if the at offset is the extension type. <br />
		/// An AttributeValue, if at the offset is a positional argument. <br />
		/// A KeyValuePair&lt;string, AttributeValue>, if at the offset is a named argument.
		/// </returns>
		/// <remarks>offset != Caret.Offset, but offset == ValueStartOffset</remarks>
		public static object GetMarkupDataAtPosition(MarkupExtensionInfo info, int offset)
		{
			object previous = info.ExtensionType;
			int endOffset = info.StartOffset + info.ExtensionType.Length;
			
			foreach (var item in info.PositionalArguments) {
				if (item.StartOffset <= offset && offset <= item.EndOffset)
					previous = item.IsString ? item : GetMarkupDataAtPosition(item.ExtensionValue, offset);
				
				endOffset = item.EndOffset;
			}
			
			foreach (var pair in info.NamedArguments) {
				if (pair.Value.StartOffset <= offset && offset <= pair.Value.EndOffset)
					previous = pair.Value.IsString ? pair : GetMarkupDataAtPosition(pair.Value.ExtensionValue, offset);
				else if (endOffset <= offset && offset <= pair.Value.StartOffset)
					previous = pair;
				
				endOffset = pair.Value.EndOffset;
			}
			
			return previous;
		}
		
		/// <remarks>offset != Caret.Offset, but offset == ValueStartOffset</remarks>
		public static MarkupExtensionInfo GetMarkupExtensionAtPosition(MarkupExtensionInfo info, int offset)
		{
			MarkupExtensionInfo tmp = info;
			
			foreach (var item in info.PositionalArguments) {
				int endOffset = item.EndOffset;
				if (!item.IsClosed)
					endOffset++;
				if (item.StartOffset < offset && offset < endOffset)
					tmp = item.IsString ? tmp : GetMarkupExtensionAtPosition(item.ExtensionValue, offset);
			}
			
			foreach (var pair in info.NamedArguments) {
				int endOffset = pair.Value.EndOffset;
				if (!pair.Value.IsClosed)
					endOffset++;
				if (pair.Value.StartOffset < offset && offset < endOffset)
					tmp = pair.Value.IsString ? tmp : GetMarkupExtensionAtPosition(pair.Value.ExtensionValue, offset);
			}
			
			return tmp;
		}
		
		public static string LookForTargetTypeValue(XamlContext context, out bool isExplicit, params string[] elementName)
		{
			var ancestors = context.Ancestors;
			
			isExplicit = false;
			
			for (int i = 0; i < ancestors.Count; i++) {
				if (ancestors[i].LocalName == "Style" && XamlConst.WpfXamlNamespaces.Contains(ancestors[i].Namespace)) {
					isExplicit = true;
					return ancestors[i].GetAttributeValue("TargetType") ?? string.Empty;
				}
				
				if (ancestors[i].Name.EndsWithAny(elementName.Select(s => "." + s + "s"), StringComparison.Ordinal)
				    && !ancestors[i].Name.StartsWith("Style.", StringComparison.Ordinal)) {
					return ancestors[i].Name.Remove(ancestors[i].Name.IndexOf('.'));
				}
			}
			
			return null;
		}
	}
}
