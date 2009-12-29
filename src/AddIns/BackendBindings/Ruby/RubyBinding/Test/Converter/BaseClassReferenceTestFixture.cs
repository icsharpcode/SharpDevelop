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
	/// Tests that a base class reference through the base keyword
	/// is converted to the self keyword in Ruby when converting
	/// from C# to Ruby correctly.
	/// </summary>
	[TestFixture]
	public class BaseClassReferenceTestFixture
	{
		string csharp = 
			"class Foo\r\n" +
			"{\r\n" +
			"    public string Run()\r\n" +
			"    {" +
			"        return base.ToString();\r\n" +
			"    }\r\n" +
			"}";

		[Test]
		public void ConvertedRubyCode()
		{
			string expectedCode =
				"class Foo\r\n" +
				"    def Run()\r\n" +
				"        return self.ToString()\r\n" +
				"    end\r\n" +
				"end";
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedCode, code);
		}
	}
}
