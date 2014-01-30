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
