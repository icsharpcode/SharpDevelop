// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests that a method call is converted correctly.
	/// </summary>
	[TestFixture]
	public class IntegerMethodParameterTestFixture
	{
		string csharp =
			"class Foo\r\n" +
			"{\r\n" +
			"    public void Print()\r\n" +
			"    {\r\n" +
			"        int i = 0;\r\n" +
			"        PrintInt(i);\r\n" +
			"    }\r\n" +
			"}";
		
		[Test]
		public void GeneratedRubySourceCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(csharp);
			string expectedRuby = 
				"class Foo\r\n" +
				"    def Print()\r\n" +
				"        i = 0\r\n" +
				"        self.PrintInt(i)\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
	}
}
