// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests that the code to create an instance of a generic list is converted to Ruby 
	/// correctly. 
	/// 
	/// C#: List<string> list = new List<string>();
	/// 
	/// Ruby: list = List[str]()
	/// </summary>
	[TestFixture]
	public class GenericListConversionTestFixture
	{
		string csharp = "using System.Collections.Generic;\r\n" +
						"\r\n" +
						"class Foo\r\n" +
						"{\r\n" +
						"    public Foo()\r\n" +
						"    {\r\n" +
						"        List<string> list = new List<string>();\r\n" +
						"        Dictionary<string, int> dictionary = new Dictionary<string, int>();\r\n" +
						"    }\r\n" +
						"}";
				
		[Test]
		public void ConvertedRubyCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(csharp);
			string expectedRuby =
				"require \"mscorlib\"\r\n" +
				"require \"System.Collections.Generic, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a\"\r\n" +
				"\r\n" +
				"class Foo\r\n" +
				"    def initialize()\r\n" +
				"        list = List[System::String].new()\r\n" +
				"        dictionary = Dictionary[System::String, System::Int32].new()\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}	
	}
}
