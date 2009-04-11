// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop;
using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	public static class Extensions
	{
		static readonly char[] whitespaces = new char[] { ' ', '\t', '\n', '\r' };
		
		public static string[] Split(this string s, StringSplitOptions options, params char[] delimiters)
		{
			return s.Split(delimiters, options);
		}
		
		public static MarkupExtensionInfo ParseMarkupExtension(this XamlParser parser, string fileName, string fileContent, int index)
		{
			XmlElementPath path = XmlParser.GetActiveElementStartPathAtIndex(fileContent, index);
			ParseInformation info = ParserService.GetParseInformation(fileName);
			
			if (path != null && path.Elements.Count > 0) {
				if (XmlParser.IsInsideAttributeValue(fileContent, index)) {
					string value = XmlParser.GetAttributeValueAtIndex(fileContent, index).Trim(whitespaces);
					string name = XmlParser.GetAttributeNameAtIndex(fileContent, index);
					if (!string.IsNullOrEmpty(value) && value.StartsWith("{")) {
						string[] parts = value.Split(whitespaces, StringSplitOptions.RemoveEmptyEntries);
						string extensionType = parts[0].TrimStart('{');
						XamlResolver resolver = new XamlResolver();
						TypeResolveResult trr = resolver.Resolve(new ExpressionResult(extensionType, new XamlExpressionContext(path, name, true)), info, fileContent) as TypeResolveResult;
						if (trr != null) {
							
						}
					}
				}
			}
			
			return MarkupExtensionInfo.Empty;
		}
		
		public static bool IsInsideMarkupExtension(this XamlParser parser, string fileContent, int index)
		{
			if (XmlParser.IsInsideAttributeValue(fileContent, index)) {
				string value = XmlParser.GetAttributeValueAtIndex(fileContent, index).Trim(' ', '\t', '\n', '\r');
				if (!string.IsNullOrEmpty(value) && value.StartsWith("{"))
					return true;
			}
			
			return false;
		}
	}
	
	public struct MarkupExtensionInfo
	{
		IEntity extension;
		
		public static readonly MarkupExtensionInfo Empty = new MarkupExtensionInfo(null);
		
		public MarkupExtensionInfo(IEntity extension)
		{
			this.extension = extension;
		}
		
		public IEntity Extension {
			get { return extension; }
		}
	}
}
