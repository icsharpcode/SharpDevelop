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
	/// Tests the CSharpToPythonConverter class can convert a C# property to
	/// two get and set methods in Python.
	/// </summary>
	[TestFixture]
	public class PropertyWithGetSetStatementsTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tint count = 0;\r\n" +
						"\tint i = 0;\r\n" +
						"\tpublic int Count\r\n" +
						"\t{\r\n" +
						"\t\tget {\r\n" +
						"\t\t\tif (i == 0) {\r\n" +
						"\t\t\treturn 10;\r\n" +
						"\t\t} else {\r\n" +
						"\t\t\treturn count;\r\n" +
						"\t\t}\r\n" +
						"\t}\r\n" +
						"\t\tset {\r\n" +
						"\t\t\tif (i == 1) {\r\n" +
						"\t\t\tcount = value;\r\n" +
						"\t\t} else {\r\n" +
						"\t\t\tcount = value + 5;\r\n" +
						"\t\t}\r\n" +
						"\t}\r\n" +
						"}";
			
		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef __init__(self):\r\n" +
									"\t\tself._count = 0\r\n" +
									"\t\tself._i = 0\r\n" +
									"\r\n" +
									"\tdef get_Count(self):\r\n" +
									"\t\tif self._i == 0:\r\n" +
									"\t\t\treturn 10\r\n" +
									"\t\telse:\r\n" +
									"\t\t\treturn self._count\r\n" +
									"\r\n" +
									"\tdef set_Count(self, value):\r\n" +
									"\t\tif self._i == 1:\r\n" +
									"\t\t\tself._count = value\r\n" +
									"\t\telse:\r\n" +
									"\t\t\tself._count = value + 5\r\n" +
									"\r\n" +
									"\tCount = property(fget=get_Count, fset=set_Count)";
			
			Assert.AreEqual(expectedPython, python);
		}			
	}
}
