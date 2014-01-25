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
	/// Tests that removing a method from an event handler is converted
	/// from C# to Ruby correctly.
	/// </summary>
	[TestFixture]
	public class RemoveHandlerConversionTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    public Foo()\r\n" +
						"    {" +
						"        button = new Button();\r\n" +
						"        button.Click -= ButtonClick;\r\n" +
						"    }\r\n" +
						"\r\n" +
						"    void ButtonClick(object sender, EventArgs e)\r\n" +
						"    {\r\n" +
						"    }\r\n" +
						"}";
		
		[Test]
		public void ConvertedRubyCode()
		{
			string expectedCode =
				"class Foo\r\n" +
				"    def initialize()\r\n" +
				"        button = Button.new()\r\n" +
				"        button.Click.remove(OnButtonClick)\r\n" +
				"    end\r\n" +
				"\r\n" +
				"    def ButtonClick(sender, e)\r\n" +
				"    end\r\n" +
				"end";
			
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedCode, code);
		}	
	}
}
