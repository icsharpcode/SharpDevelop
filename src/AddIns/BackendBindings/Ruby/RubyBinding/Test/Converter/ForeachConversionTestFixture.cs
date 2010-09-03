// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
