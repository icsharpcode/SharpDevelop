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
	/// Tests that an array is converted from C# to Python.
	/// </summary>
	[TestFixture]
	public class ArrayConversionTestFixture
	{		
		string integerArray = "class Foo\r\n" +
						"{\r\n" +
						"    public int[] Run()\r\n" +
						"    {" +
						"        int[] i = new int[] {1, 2, 3, 4};\r\n" +
						"        i[0] = 5;\r\n" +
						"        return i;\r\n" +
						"    }\r\n" +
						"}";

		string int32Array = "class Foo\r\n" +
						"{\r\n" +
						"    public Int32[] Run()\r\n" +
						"    {" +
						"        Int32[] i = new Int32[] {1, 2, 3, 4};\r\n" +
						"        i[0] = 5;\r\n" +
						"        return i;\r\n" +
						"    }\r\n" +
						"}";
		
		string uintArray = "class Foo\r\n" +
						"{\r\n" +
						"    public uint[] Run()\r\n" +
						"    {" +
						"        uint[] i = new uint[] {1, 2, 3, 4};\r\n" +
						"        i[0] = 5;\r\n" +
						"        return i;\r\n" +
						"    }\r\n" +
						"}";
		
		string stringArray = "class Foo\r\n" +
						"{\r\n" +
						"    public string[] Run()\r\n" +
						"    {" +
						"        string[] i = new string[] {\"a\", \"b\"};\r\n" +
						"        i[0] = \"c\";\r\n" +
						"        return i;\r\n" +
						"    }\r\n" +
						"}";

		string barArray = "class Foo\r\n" +
						"{\r\n" +
						"    public string[] Run()\r\n" +
						"    {" +
						"        Bar[] i = new Bar[] { new Bar(), new Bar()};\r\n" +
						"        i[0] = new Bar();\r\n" +
						"        return i;\r\n" +
						"    }\r\n" +
						"}";

		string uriArray = "class Foo\r\n" +
						"{\r\n" +
						"    public Uri[] Run()\r\n" +
						"    {" +
						"        Uri[] i = new Uri[] {new Uri(\"a\"), new Uri(\"b\")};\r\n" +
						"        i[0] = new Uri(\"c\");\r\n" +
						"        return i;\r\n" +
						"    }\r\n" +
						"}";
				
		[Test]
		public void ConvertedIntegerArrayCode()
		{
			string expectedCode = "class Foo(object):\r\n" +
									"    def Run(self):\r\n" +
									"        i = Array[int]((1, 2, 3, 4))\r\n" +
									"        i[0] = 5\r\n" +
									"        return i";
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(integerArray);
			
			Assert.AreEqual(expectedCode, code);
		}	
		
		[Test]
		public void ConvertedStringArrayCode()
		{
			string expectedCode = "class Foo(object):\r\n" +
									"    def Run(self):\r\n" +
									"        i = Array[str]((\"a\", \"b\"))\r\n" +
									"        i[0] = \"c\"\r\n" +
									"        return i";
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(stringArray);
			
			Assert.AreEqual(expectedCode, code);
		}
		
		[Test]
		public void ConvertedBarArrayCode()
		{
			string expectedCode = "class Foo(object):\r\n" +
									"    def Run(self):\r\n" +
									"        i = Array[Bar]((Bar(), Bar()))\r\n" +
									"        i[0] = Bar()\r\n" +
									"        return i";
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(barArray);
			
			Assert.AreEqual(expectedCode, code);
		}
		
		[Test]
		public void ConvertedUriArrayCode()
		{
			string expectedCode = "class Foo(object):\r\n" +
									"    def Run(self):\r\n" +
									"        i = Array[Uri]((Uri(\"a\"), Uri(\"b\")))\r\n" +
									"        i[0] = Uri(\"c\")\r\n" +
									"        return i";
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(uriArray);
			
			Assert.AreEqual(expectedCode, code);
		}		
		
		[Test]
		public void ConvertedUIntArrayCode()
		{
			string expectedCode = "class Foo(object):\r\n" +
									"    def Run(self):\r\n" +
									"        i = Array[UInt32]((1, 2, 3, 4))\r\n" +
									"        i[0] = 5\r\n" +
									"        return i";
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(uintArray);
			
			Assert.AreEqual(expectedCode, code);
		}
		
		[Test]
		public void ConvertedInt32ArrayCode()
		{
			string expectedCode = "class Foo(object):\r\n" +
									"    def Run(self):\r\n" +
									"        i = Array[int]((1, 2, 3, 4))\r\n" +
									"        i[0] = 5\r\n" +
									"        return i";
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(int32Array);
			
			Assert.AreEqual(expectedCode, code);
		}		
	}
}
