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
	/// Tests the conversion of a foreach loop to Ruby.
	/// </summary>
	[TestFixture]
	public class ForeachConversionTestFixture
	{	
		string intArrayCode = "class Foo\r\n" +
					"{\r\n" +
					"    public int GetCount(int[] items)\r\n" +
					"    {\r\n" +
					"        int count = 0;\r\n" +
					"        foreach (int item in items) {\r\n" +
					"            count++;\r\n" +
					"        }\r\n" +
					"        return count;\r\n" +
					"    }\r\n" +
					"}";

		string dictionaryCode = "class Foo\r\n" +
					"{\r\n" +
					"    public void DisplayItems()\r\n" +
					"    {\r\n" +
					"        Dictionary<string, string> record = new Dictionary<string, string>();\r\n" +
					"        record.Add(\"a\", \"b\");\r\n" +
					"        record.Add(\"c\", \"d\");\r\n" +
					"        foreach (string key in record.Keys) {\r\n" +
					"            Console.WriteLine(\"Key: \" + key);\r\n" +
					"        }\r\n" +
					"    }\r\n" +
					"}";
		
		[Test]
		public void ConvertedIntArrayCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(intArrayCode);
			string expectedCode =
				"class Foo\r\n" +
				"    def GetCount(items)\r\n" +
				"        count = 0\r\n" +
				"        enumerator = items.GetEnumerator()\r\n" +
				"        while enumerator.MoveNext()\r\n" +
				"            item = enumerator.Current\r\n" +
				"            count += 1\r\n" +
				"        end\r\n" +
				"        return count\r\n" +
				"    end\r\n" +
				"end";
		
			Assert.AreEqual(expectedCode, code);
		}
		
		[Test]
		public void ConvertedDictionaryCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(dictionaryCode);
			string expectedCode =
				"class Foo\r\n" +
				"    def DisplayItems()\r\n" +
				"        record = Dictionary[System::String, System::String].new()\r\n" +
				"        record.Add(\"a\", \"b\")\r\n" +
				"        record.Add(\"c\", \"d\")\r\n" +
				"        enumerator = record.Keys.GetEnumerator()\r\n" +
				"        while enumerator.MoveNext()\r\n" +
				"            key = enumerator.Current\r\n" +
				"            Console.WriteLine(\"Key: \" + key)\r\n" +
				"        end\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedCode, code);
		}		
	}
}
