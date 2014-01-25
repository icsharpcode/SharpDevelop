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
	[TestFixture]
	public class PropertyReferenceConversionTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    int count = 0;\r\n" +
						"    public int Count {\r\n" +
						"        get {\r\n" +
						"                return count;\r\n" +
						"        }\r\n" +
						"    }\r\n" +
						"\r\n" +
						"    public void Increment()\r\n" +
						"    {\r\n" +
						"        Count++;\r\n" +
						"    }\r\n" +
						"\r\n" +
						"    public void SetCount(int Count)\r\n" +
						"    {\r\n" +
						"        this.Count = Count;\r\n" +
						"    }\r\n" +	
						"}";
			
		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"    def __init__(self):\r\n" +
									"        self._count = 0\r\n" +
									"\r\n" +
									"    def get_Count(self):\r\n" +
									"        return self._count\r\n" +
									"\r\n" +
									"    Count = property(fget=get_Count)\r\n" +
									"\r\n" +
									"    def Increment(self):\r\n" +
									"        self.Count += 1\r\n" +
									"\r\n" +
									"    def SetCount(self, Count):\r\n" +
									"        self.Count = Count";
			
			Assert.AreEqual(expectedPython, python, python);
		}
	}
}
