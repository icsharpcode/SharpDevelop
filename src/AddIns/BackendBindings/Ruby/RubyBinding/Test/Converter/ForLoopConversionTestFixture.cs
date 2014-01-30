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
	/// Tests the conversion of a for loop to Ruby. 
	/// 
	/// C#:
	/// 
	/// for (int i = 0; i &lt; 5; ++i) {
	/// }
	/// 
	/// Ruby:
	/// 
	/// i = 0
	/// while i &lt; 5
	/// 	i = i + 1
	/// end
	///  
	/// Ideally we would convert it to:
	/// 
	/// for i in 0..5
	/// 	...
	/// end
	/// </summary>
	[TestFixture]
	public class ForLoopConversionTestFixture
	{
		string csharp = "class Foo\r\n" +
					"{\r\n" +
					"    public int GetCount()\r\n" +
					"    {\r\n" +
					"        int count = 0;\r\n" +
					"        for (int i = 0; i < 5; i = i + 1) {\r\n" +
					"            count++;\r\n" +
					"        }\r\n" +
					"        return count;\r\n" +
					"    }\r\n" +
					"}";

		[Test]
		public void ConvertedRubyCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			System.Console.WriteLine(code);
			string expectedCode =
				"class Foo\r\n" +
				"    def GetCount()\r\n" +
				"        count = 0\r\n" +
				"        i = 0\r\n" +
				"        while i < 5\r\n" +
				"            count += 1\r\n" +
				"            i = i + 1\r\n" +
				"        end\r\n" +
				"        return count\r\n" +
				"    end\r\n" +
				"end";
		
			Assert.AreEqual(expectedCode, code);
		}
	}
}
