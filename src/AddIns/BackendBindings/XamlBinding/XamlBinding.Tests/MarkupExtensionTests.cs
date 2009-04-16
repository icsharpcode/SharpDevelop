// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
			
			while ((token = tokenizer.NextToken()).Kind != MarkupExtensionTokenKind.EOF)
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
			
			Assert.AreEqual("x:Type", value.ExtensionValue.Type);
			Assert.AreEqual(1, value.ExtensionValue.PositionalArguments.Count);
			Assert.AreEqual("CheckBox", value.ExtensionValue.PositionalArguments[0].StringValue);
		}
	}
}
