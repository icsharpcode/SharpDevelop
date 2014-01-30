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
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	[TestFixture]
	public class UsingStatementConversionTestFixture
	{
		string csharp = "using System\r\n" +
						"class Foo\r\n" +
						"{\r\n" +
						"}";

		[Test]
		public void GeneratedRubyCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			string Ruby = converter.Convert(csharp);
			string expectedRuby = "require \"mscorlib\"\r\n" +
									"require \"System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a\"\r\n" +
									"\r\n" +
									"class Foo\r\n" +
									"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
		
		[Test]
		public void MultipleUsingStatements()
		{
			string csharp = "using System\r\n" +
							"using System.Drawing\r\n" +
						"class Foo\r\n" +
						"{\r\n" +
						"}";
			
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			string Ruby = converter.Convert(csharp);
			string expectedRuby = "require \"mscorlib\"\r\n" +
									"require \"System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a\"\r\n" +
									"require \"System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a\"\r\n" +
									"\r\n" +
									"class Foo\r\n" +
									"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
	}
}
