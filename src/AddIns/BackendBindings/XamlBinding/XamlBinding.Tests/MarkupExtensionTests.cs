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
using NUnit.Framework;
using System.Collections.Generic;

namespace ICSharpCode.XamlBinding.Tests
{
	[TestFixture]
	public class MarkupExtensionTests
	{
		[Test]
		public void SimpleTest()
		{
			MarkupExtensionTokenizer tokenizer = new MarkupExtensionTokenizer("{DynamicResource {x:Static SystemColors.ControlBrushKey}}");
			
			MarkupExtensionToken token = null;
			var tokens = new List<MarkupExtensionToken>();
			
			while ((token = tokenizer.NextToken()).Kind != MarkupExtensionTokenKind.EndOfFile)
				tokens.Add(token);
			
			Assert.AreEqual(new List<MarkupExtensionToken> {
			                	new MarkupExtensionToken(MarkupExtensionTokenKind.OpenBrace, "{"),
			                	new MarkupExtensionToken(MarkupExtensionTokenKind.TypeName, "DynamicResource"),
			                	new MarkupExtensionToken(MarkupExtensionTokenKind.String, "{x:Static SystemColors.ControlBrushKey}"),
			                	new MarkupExtensionToken(MarkupExtensionTokenKind.CloseBrace, "}")
			                }, tokens);
		}
		
		[Test]
		public void PositionalArgumentsSimple()
		{
			string markup = "{x:Type CheckBox}";
			
			AttributeValue value = MarkupExtensionParser.ParseValue(markup);
			
			Assert.AreEqual("x:Type", value.ExtensionValue.ExtensionType);
			Assert.AreEqual(1, value.ExtensionValue.PositionalArguments.Count);
			Assert.AreEqual("CheckBox", value.ExtensionValue.PositionalArguments[0].StringValue);
		}
	}
}
