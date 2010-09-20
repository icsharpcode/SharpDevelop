// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests the CSharpToRubyConverter class can convert a C# property to
	/// two get and set methods in Ruby.
	/// </summary>
	[TestFixture]
	public class PropertyConversionTestFixture
	{	
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    int count = 0;\r\n" +
						"    public int Count\r\n" +
						"    {\r\n" +
						"        get { return count; }\r\n" +
						"        set { count = value; }\r\n" +
						"    }\r\n" +
						"\r\n" +
						"    public void Run()\r\n" +
						"    {\r\n" +
						"    }\r\n" +
						"}";
			
		[Test]
		public void ConvertedRubyCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(csharp);
			string expectedRuby =
				"class Foo\r\n" +
				"    def initialize()\r\n" +
				"        @count = 0\r\n" +
				"    end\r\n" +
				"\r\n" +
				"    def Count\r\n" +
				"        return @count\r\n" +
				"    end\r\n" +
				"\r\n" +
				"    def Count=(value)\r\n" +
				"        @count = value\r\n" +
				"    end\r\n" +
				"\r\n" +
				"    def Run()\r\n" +
				"    end\r\n" +
				"end";
											
			Assert.AreEqual(expectedRuby, Ruby);
		}	
	}
}
