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
	/// Tests that parameters passed to a constructor are converted to Ruby correctly.
	/// </summary>
	[TestFixture]
	public class CallConstructorWithParametersConversionTestFixture
	{	
		string csharp =
			"class Foo\r\n" +
			"{\r\n" +
			"    public Foo()\r\n" +
			"    {\r\n" +
			"        Bar b = new Bar(0, 0, 1, 10);\r\n" +
			"    }\r\n" +
			"}";
		
		[Test]
		public void ConvertedRubyCode()
		{
			string expectedRuby =
				"class Foo\r\n" +
				"    def initialize()\r\n" +
				"        b = Bar.new(0, 0, 1, 10)\r\n" +
				"    end\r\n" +
				"end";
			
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
	}
}
