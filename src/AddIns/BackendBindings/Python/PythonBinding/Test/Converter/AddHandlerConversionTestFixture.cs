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
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests that assigning a method to an event handler is converted
	/// from C# to Python correctly.
	/// </summary>
	[TestFixture]
	public class AddHandlerConversionTestFixture
	{		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic Foo()\r\n" +
						"\t{" +
						"\t\tbutton = new Button();\r\n" +
						"\t\tbutton.Click += ButtonClick;\r\n" +
						"\t\tbutton.MouseDown += self.OnMouseDown;\r\n" +
						"\t}\r\n" +
						"\r\n" +
						"\tvoid ButtonClick(object sender, EventArgs e)\r\n" +
						"\t{\r\n" +
						"\t}\r\n" +
						"\r\n" +
						"\tvoid OnMouseDown(object sender, EventArgs e)\r\n" +
						"\t{\r\n" +
						"\t}\r\n" +
						"}";
				
		[Test]
		public void ConvertedPythonCode()
		{
			string expectedCode = "class Foo(object):\r\n" +
									"\tdef __init__(self):\r\n" +
									"\t\tbutton = Button()\r\n" +
									"\t\tbutton.Click += self.ButtonClick\r\n" +
									"\t\tbutton.MouseDown += self.OnMouseDown\r\n" +
									"\r\n" +
									"\tdef ButtonClick(self, sender, e):\r\n" +
									"\t\tpass\r\n" +
									"\r\n" +
									"\tdef OnMouseDown(self, sender, e):\r\n" +
									"\t\tpass";
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedCode, code);
		}
	}
}
