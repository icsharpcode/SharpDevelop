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
	public class MultipleFieldsOnSameLineTests
	{
		string csharpClassWithFieldsThatHaveInitialValues =
			"class Foo\r\n" +
			"{\r\n" +
			"    int i = 0, j = 1;\r\n" +
			"}";
		
		[Test]
		public void ConvertCSharpClassWithFieldsThatHaveInitialValues()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string python = converter.Convert(csharpClassWithFieldsThatHaveInitialValues);
			string expectedPython =
				"class Foo(object):\r\n" +
				"    def __init__(self):\r\n" +
				"        self._i = 0\r\n" +
				"        self._j = 1";
			
			Assert.AreEqual(expectedPython, python);
		}
		
		string csharpClassWithTwoFieldsWhereFirstDoesNotHaveInitialValue =
			"class Foo\r\n" +
			"{\r\n" +
			"    int i, j = 1;\r\n" +
			"}";
		
		[Test]
		public void ConvertCSharpClassWithFieldsWhereFirstFieldDoesNotHaveInitialValue()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string python = converter.Convert(csharpClassWithTwoFieldsWhereFirstDoesNotHaveInitialValue);
			string expectedPython =
				"class Foo(object):\r\n" +
				"    def __init__(self):\r\n" +
				"        self._j = 1";
			
			Assert.AreEqual(expectedPython, python);
		}
		
		string csharpClassWithTwoFieldsInitializedInMethod =
			"class Foo\r\n" +
			"{\r\n" +
			"    int i = 0;\r\n" +
			"    int j, k;\r\n" +
			"\r\n" +
			"    public void Test()\r\n" +
			"    {\r\n" +
			"        j = 1;\r\n" +
			"        k = 3;\r\n" +
			"    }\r\n" +
			"}";
		
		[Test]
		public void ConvertCSharpClassWithTwoFieldsInitializedInMethod()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string python = converter.Convert(csharpClassWithTwoFieldsInitializedInMethod);
			string expectedPython =
				"class Foo(object):\r\n" +
				"    def __init__(self):\r\n" +
				"        self._i = 0\r\n" +
				"\r\n" +
				"    def Test(self):\r\n" +
				"        self._j = 1\r\n" +
				"        self._k = 3";
			
			Assert.AreEqual(expectedPython, python);
		}
		
		string vnetClassWithTwoArrayFieldsOnSameLine =
			"class Foo\r\n" +
			"    Private i(10), j(20) as integer\r\n" +
			"end class";
		
		[Test]
		public void ConvertVBNetClassWithTwoArrayFieldsOnSameLine()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.VBNet);
			converter.IndentString = "    ";
			string python = converter.Convert(vnetClassWithTwoArrayFieldsOnSameLine);
			string expectedPython =
				"class Foo(object):\r\n" +
				"    def __init__(self):\r\n" +
				"        self._i = Array.CreateInstance(int, 10)\r\n" +
				"        self._j = Array.CreateInstance(int, 20)";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}
