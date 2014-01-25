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
	/// Tests that the indentation after the nested class is correct for any outer class methods.
	/// </summary>
	[TestFixture]	
	public class NestedClassConversionTestFixture
	{		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic void Run()\r\n" +
						"\t{\r\n" +
						"\t}\r\n" +
						"\r\n" +
						"\t\tclass Bar\r\n" +
						"\t\t{\r\n" +
						"\t\t\tpublic void Test()\r\n" +
						"\t\t\t{\r\n" +
						"\t\t\t}\r\n" +
						"\t\t}\r\n" +
						"\r\n" +
						"\tpublic void AnotherRun()\r\n" +
						"\t{\r\n" +
						"\t}\r\n" +
						"}";
				
		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef Run(self):\r\n" +
									"\t\tpass\r\n" +
									"\r\n" +
									"\tclass Bar(object):\r\n" +
									"\t\tdef Test(self):\r\n" +
									"\t\t\tpass\r\n" +
									"\r\n" +
									"\tdef AnotherRun(self):\r\n" +
									"\t\tpass";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}
