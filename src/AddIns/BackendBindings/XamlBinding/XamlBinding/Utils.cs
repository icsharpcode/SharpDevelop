// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		public static int MinMax(int value, int lower, int upper)
		{
			return Math.Min(Math.Max(value, lower), upper);
		}
		
		static readonly char[] whitespace = new[] {' ', '\t', '\n', '\r'};
		static readonly char[] newline = new[] {'\n', '\r'};
		
		public static string GetNamespacePrefix(string namespaceUri, XamlContext context)
		{
			var item = context.XmlnsDefinitions.FirstOrDefault(i => i.Value == namespaceUri);

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
		
		public static Location GetLocationInfoFromOffset(string text, int offset)
		{
			string[] lines = text.Substring(0, MinMax(offset, 0, text.Length)).Split('\n');
			string line = lines.LastOrDefault() ?? string.Empty;
			
			return new Location(line.Length + 1, lines.Length);
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
				if (item.StartOffset < offset && offset < item.EndOffset)
					tmp = item.IsString ? tmp : GetMarkupExtensionAtPosition(item.ExtensionValue, offset);
			}
			
			foreach (var pair in info.NamedArguments) {
				if (pair.Value.StartOffset < offset && offset < pair.Value.EndOffset)
					tmp = pair.Value.IsString ? tmp : GetMarkupExtensionAtPosition(pair.Value.ExtensionValue, offset);
			}
			
			return tmp;
		}
	}
}
