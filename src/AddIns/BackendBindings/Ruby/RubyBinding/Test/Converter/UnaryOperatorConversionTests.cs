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
using System.CodeDom;
using System.CodeDom.Compiler;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	[TestFixture]
	public class UnaryOperatorConversionTests
	{	
		[Test]
		public void MinusOne()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(int i)\r\n" +
							"    {\r\n" +
							"        i = -1;\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"\tdef Run(i)\r\n" +
									"\t\ti = -1\r\n" +
									"\tend\r\n" +
									"end";
	
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}
		
		[Test]
		public void PlusOne()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(int i)\r\n" +
							"    {\r\n" +
							"        i = +1;\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"\tdef Run(i)\r\n" +
									"\t\ti = +1\r\n" +
									"\tend\r\n" +
									"end";
		
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}
		
		[Test]
		public void NotOperator()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(bool i)\r\n" +
							"    {\r\n" +
							"        j = !i;\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"\tdef Run(i)\r\n" +
									"\t\tj = not i\r\n" +
									"\tend\r\n" +
									"end";
		
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}		
		
		[Test]
		public void BitwiseNotOperator()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(bool i)\r\n" +
							"    {\r\n" +
							"        j = ~i;\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"\tdef Run(i)\r\n" +
									"\t\tj = ~i\r\n" +
									"\tend\r\n" +
									"end";
		
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}
	}
}
