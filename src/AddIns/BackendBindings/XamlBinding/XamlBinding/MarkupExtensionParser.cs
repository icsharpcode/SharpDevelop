// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.XamlBinding
{
	public static class MarkupExtensionParser
	{
		public static MarkupExtensionInfo Parse(string text)
		{
			return Parse(text, 0);
		}
		
		static MarkupExtensionInfo Parse(string text, int offset)
		{
			var info = new MarkupExtensionInfo();
			string argumentName = null;
			MarkupExtensionTokenizer tokenizer = new MarkupExtensionTokenizer(text);
			
			MarkupExtensionToken token = null;
			
			try {
				token = tokenizer.NextToken();
				
				while (token.Kind != MarkupExtensionTokenKind.EndOfFile) {
					switch (token.Kind) {
						case MarkupExtensionTokenKind.OpenBrace:
							info.StartOffset = token.StartOffset + offset;
							break;
						case MarkupExtensionTokenKind.CloseBrace:
							info.EndOffset = token.EndOffset + offset;
							break;
						case MarkupExtensionTokenKind.TypeName:
							info.ExtensionType = token.Value;
							break;
						case MarkupExtensionTokenKind.MemberName:
							// if there is an open member without a value add the member name
							if (argumentName != null)
								info.TryAddNamedArgument(argumentName, ParseValue("", token.EndOffset + offset));
							argumentName = token.Value;
							break;
						case MarkupExtensionTokenKind.String:
							if (argumentName != null) {
								info.TryAddNamedArgument(argumentName, ParseValue(token.Value, token.StartOffset + offset));
								argumentName = null;
							} else {
								info.PositionalArguments.Add(ParseValue(token.Value, token.StartOffset + offset));
							}
							break;
					}
					
					token = tokenizer.NextToken();
				}
			} catch (MarkupExtensionParseException) {
				// ignore parser errors
			} finally {
				if (token != null && argumentName != null)
					info.TryAddNamedArgument(argumentName, ParseValue(token.Value, token.StartOffset + offset));
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
		
		static AttributeValue ParseValue(string text, int offset)
		{
			if (string.IsNullOrEmpty(text))
				return new AttributeValue(string.Empty) { StartOffset = offset };
			
			if (text.StartsWith("{", StringComparison.OrdinalIgnoreCase))
				return new AttributeValue(Parse(text, offset)) { StartOffset = offset };
			else
				return new AttributeValue(text) { StartOffset = offset };
		}
	}
}
