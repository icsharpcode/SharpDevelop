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
	[TestFixture]
	public class BitShiftConversionTestFixture
	{		
		string csharp = 
			"class Foo\r\n" +
			"{\r\n" +
			"    public int Convert()\r\n" +
			"    {\r\n" +
			"        int a = (b >> 16) & 0xffff;\r\n" +
			"        return a;\r\n" +
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
				"    def Convert()\r\n" +
				"        a = (b >> 16) & 0xffff\r\n" +
				"        return a\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
	}
}

