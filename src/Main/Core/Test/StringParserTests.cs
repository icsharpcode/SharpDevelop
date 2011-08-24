// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			StringParser.RegisterStringTagProvider("obj", new PropertyObjectTagProvider(this));
			StringParser.RegisterStringTagProvider(new CustomTagProvider());
		}
		
		sealed class CustomTagProvider : IStringTagProvider
		{
			public string ProvideString(string tag, StringTagPair[] customTags)
			{
				if (string.Equals(tag, "test", StringComparison.OrdinalIgnoreCase))
					return "Value";
				else
					return null;
			}
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
			Assert.AreEqual("12", StringParser.Parse("${a}${b}", new StringTagPair("a", "1"),  new StringTagPair("b", "2")));
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
