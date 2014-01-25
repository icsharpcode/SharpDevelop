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
