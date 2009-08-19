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
		
		public static int MinMax(int value, int lower, int upper)
		{
			return Math.Min(Math.Max(value, lower), upper);
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