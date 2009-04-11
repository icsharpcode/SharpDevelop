// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.XamlBinding
{
	static class MarkupExtensionParser
	{
		public static MarkupExtensionInfo Parse(string text)
		{
			var info = new MarkupExtensionInfo();
			
			var tokenizer = new MarkupExtensionTokenizer(text);
			
			string argumentName = null;
			
			try {
				var token = tokenizer.NextToken();
				while (token.Kind != MarkupExtensionTokenKind.EOF) {
					switch (token.Kind) {
						case MarkupExtensionTokenKind.TypeName:
							info.Type = token.Value;
							break;
						case MarkupExtensionTokenKind.Membername:
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
		
		static AttributeValue ParseValue(string text)
		{
			if (text.StartsWith("{", StringComparison.OrdinalIgnoreCase))
				return new AttributeValue(Parse(text));
			else
				return new AttributeValue(text);
		}
	}
}
