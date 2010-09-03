// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
