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
	/// <summary>
	/// Tests that an integer increments and decrements are converted
	/// from C# to Ruby correctly.
	/// </summary>
	[TestFixture]
	public class IncrementAndDecrementConversionTests
	{	
		[Test]
		public void PreIncrement()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(int i)\r\n" +
							"    {\r\n" +
							"        ++i;\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"    def Run(i)\r\n" +
									"        i += 1\r\n" +
									"    end\r\n" +
									"end";
	
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}
		
		[Test]
		public void PostIncrement()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(int i)\r\n" +
							"    {\r\n" +
							"        i++;\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"    def Run(i)\r\n" +
									"        i += 1\r\n" +
									"    end\r\n" +
									"end";
	
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}		
		
		[Test]
		public void PreDecrement()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(int i)\r\n" +
							"    {\r\n" +
							"        --i\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"    def Run(i)\r\n" +
									"        i -= 1\r\n" +
									"    end\r\n" +
									"end";
	
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}

		[Test]
		public void PostDecrement()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(int i)\r\n" +
							"    {\r\n" +
							"        i--\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"    def Run(i)\r\n" +
									"        i -= 1\r\n" +
									"    end\r\n" +
									"end";
	
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}		
		
		[Test]
		public void Add10()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(int i)\r\n" +
							"    {\r\n" +
							"        i = i + 10\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"    def Run(i)\r\n" +
									"        i = i + 10\r\n" +
									"    end\r\n" +
									"end";
	
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}
		
		[Test]
		public void Add5()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(int i)\r\n" +
							"    {\r\n" +
							"        i += 5\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"    def Run(i)\r\n" +
									"        i += 5\r\n" +
									"    end\r\n" +
									"end";
	
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}
		
		[Test]
		public void Subtract10()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(int i)\r\n" +
							"    {\r\n" +
							"        i = i - 10\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"    def Run(i)\r\n" +
									"        i = i - 10\r\n" +
									"    end\r\n" +
									"end";
	
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}
	
		[Test]
		public void Subtract5()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public void Run(int i)\r\n" +
							"    {\r\n" +
							"        i -= 5\r\n" +
							"    }\r\n" +
							"}";

			string expectedRuby = "class Foo\r\n" +
									"    def Run(i)\r\n" +
									"        i -= 5\r\n" +
									"    end\r\n" +
									"end";
	
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}
	}
}
