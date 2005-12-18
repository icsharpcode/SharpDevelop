// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace ICSharpCode.Core.Tests
{
	[TestFixture]
	public class StringParserTest
	{
		public StringParserTest()
		{
			StringParser.Properties["test"] = "Value";
			StringParser.PropertyObjects["obj"] = this;
		}
		
		public string TestProperty {
			get {
				return "Hello!";
			}
		}
		
		[Test]
		public void SimpleProperty()
		{
			Assert.AreEqual("Value", StringParser.Parse("${test}"));
		}
		
		[Test]
		public void CustomInput()
		{
			Assert.AreEqual("12", StringParser.Parse("${a}${b}", new string[,] {{"a", "1"}, {"b", "2"}}));
		}
		
		[Test]
		public void CaseInsensitiveProperty()
		{
			Assert.AreEqual("Value", StringParser.Parse("${tEsT}"));
		}
		
		[Test]
		public void TextOnly()
		{
			const string txt = "Text";
			Assert.AreEqual(txt, StringParser.Parse(txt));
			// reference should be same: StringParser should not do unnecessary allocations
			Assert.AreSame(txt, StringParser.Parse(txt));
		}
		
		[Test]
		public void Mixed()
		{
			Assert.AreEqual("aValueb", StringParser.Parse("a${test}b"));
		}
		
		[Test]
		public void MultipleReplacements()
		{
			Assert.AreEqual("aValuebValuec", StringParser.Parse("a${test}b${test}c"));
		}
		
		[Test]
		public void PropertyObject()
		{
			Assert.AreEqual("Hello!", StringParser.Parse("${obj:TestProperty}"));
		}
		
		[Test]
		public void InvalidPropertyObject()
		{
			Assert.AreEqual("${invalidObj:TestProperty}", StringParser.Parse("${invalidObj:TestProperty}"));
			Assert.AreEqual("${obj:InvalidProperty}", StringParser.Parse("${obj:InvalidProperty}"));
		}
	}
}
