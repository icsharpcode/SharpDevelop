// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
