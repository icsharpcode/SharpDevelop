// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.XamlBinding
{
	public static class MarkupExtensionParser
	{
		public static MarkupExtensionInfo Parse(string text)
		{
			var info = new MarkupExtensionInfo();
			
			try {
				var tokenizer = new MarkupExtensionTokenizer(text);
				
				string argumentName = null;
				
				var token = tokenizer.NextToken();
				while (token.Kind != MarkupExtensionTokenKind.EndOfFile) {
					switch (token.Kind) {
						case MarkupExtensionTokenKind.TypeName:
							info.ExtensionType = token.Value;
							break;
						case MarkupExtensionTokenKind.MemberName:
							argumentName = token.Value;
							break;
						case MarkupExtensionTokenKind.String:
							if (argumentName != null) {
								info.NamedArguments.Add(argumentName, ParseValue(token.Value));
								argumentName = null;
							} else {
								info.PositionalArguments.Add(ParseValue(token.Value));
							}
							break;
					}
					token = tokenizer.NextToken();
				}
			} catch (MarkupExtensionParseException) {
				// ignore parser errors
			}
			
			return info;
		}
		
		public static AttributeValue ParseValue(string text)
		{
			if (string.IsNullOrEmpty(text))
				return new AttributeValue(string.Empty);
			
			if (text.StartsWith("{", StringComparison.OrdinalIgnoreCase))
				return new AttributeValue(Parse(text));
			else
				return new AttributeValue(text);
		}
	}
}
