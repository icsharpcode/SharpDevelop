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
	/// <summary>
	/// Tests that a break statement is converted correctly.
	/// </summary>
	[TestFixture]	
	public class BreakAndContinueConversionTestFixture
	{		
		string csharp =
			"class Foo\r\n" +
			"{\r\n" +
			"    public void Run()\r\n" +
			"    {\r\n" +
			"        int i = 0;\r\n" +
			"        while (i < 10) {\r\n" +
			"            if (i == 5) {\r\n" +
			"                break;\r\n" +
			"            } else {\r\n" +
			"                continue;\r\n" +
			"            }\r\n" +
			"            i++;\r\n" +
			"        }\r\n" +
			"    }\r\n" +
			"}";
				
		[Test]
		public void ConvertedRubyCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(csharp);
			string expectedRuby =
				"class Foo\r\n" +
				"    def Run()\r\n" +
				"        i = 0\r\n" +
				"        while i < 10\r\n" +
				"            if i == 5 then\r\n" +
				"                break\r\n" +
				"            else\r\n" +
				"                next\r\n" +
				"            end\r\n" +
				"            i += 1\r\n" +
				"        end\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, Ruby, Ruby);
		}
	}
}
