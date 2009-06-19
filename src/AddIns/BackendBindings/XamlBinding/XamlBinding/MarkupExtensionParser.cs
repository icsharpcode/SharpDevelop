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
				int namedArgsStart = 0;
				
				var token = tokenizer.NextToken();
				while (token.Kind != MarkupExtensionTokenKind.EndOfFile) {
					switch (token.Kind) {
						case MarkupExtensionTokenKind.TypeName:
							info.ExtensionType = token.Value;
							info.StartOffset = token.StartOffset;
							break;
						case MarkupExtensionTokenKind.MemberName:
							// if there is an open member without a value add the member name
							if (argumentName != null)
								info.TryAddNamedArgument(argumentName, new AttributeValue(string.Empty));
							argumentName = token.Value;
							namedArgsStart = token.StartOffset;
							break;
						case MarkupExtensionTokenKind.String:
							if (argumentName != null) {
								info.TryAddNamedArgument(argumentName, ParseValue(token.Value, namedArgsStart));
								argumentName = null;
							} else {
								info.PositionalArguments.Add(ParseValue(token.Value, token.StartOffset));
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
		
		static void TryAddNamedArgument(this MarkupExtensionInfo info, string name, AttributeValue value)
		{
			if (!info.NamedArguments.ContainsKey(name)) {
				info.NamedArguments.Add(name, value);
			}
		}
		
		public static AttributeValue ParseValue(string text)
		{
			return ParseValue(text, 0);
		}
		
		public static AttributeValue ParseValue(string text, int offset)
		{
			if (string.IsNullOrEmpty(text))
				return new AttributeValue(string.Empty) { StartOffset = offset };
			
			if (text.StartsWith("{", StringComparison.OrdinalIgnoreCase))
				return new AttributeValue(Parse(text)) { StartOffset = offset };
			else
				return new AttributeValue(text) { StartOffset = offset };
		}
	}
}
