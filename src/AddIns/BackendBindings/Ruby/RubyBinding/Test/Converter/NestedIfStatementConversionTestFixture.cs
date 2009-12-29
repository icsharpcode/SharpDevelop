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
	/// Tests the conversion of an if-else statement where the
	/// if and else blocks each have nested if-else statements.
	/// </summary>
	[TestFixture]
	public class NestedIfStatementConversionTestFixture
	{		
		string csharp =
			"class Foo\r\n" +
			"{\r\n" +
			"    int i = 0;\r\n" +
			"    public int GetCount()\r\n" +
			"    {" +
			"        if (i == 0) {\r\n" +
			"            if (i == 0) {\r\n" +
			"                i = 10;\r\n" +
			"                return 10;\r\n" +
			"            } else {\r\n" +
			"                i = 4;\r\n" +
			"                return 4;\r\n" +
			"            }\r\n" +
			"        } else {\r\n" +
			"            if (i == 0) {\r\n" +
			"                i = 10;\r\n" +
			"                return 10;\r\n" +
			"            } else {\r\n" +
			"                i = 4;\r\n" +
			"                return 4;\r\n" +
			"            }\r\n" +
			"        }\r\n" +
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
				"        @i = 0\r\n" +
				"    end\r\n" +
				"\r\n" +
				"    def GetCount()\r\n" +
				"        if @i == 0 then\r\n" +				
				"            if @i == 0 then\r\n" +
				"                @i = 10\r\n" +
				"                return 10\r\n" +
				"            else\r\n" +
				"                @i = 4\r\n" +
				"                return 4\r\n" +
				"            end\r\n" +
				"        else\r\n" +
				"            if @i == 0 then\r\n" +
				"                @i = 10\r\n" +
				"                return 10\r\n" +
				"            else\r\n" +
				"                @i = 4\r\n" +
				"                return 4\r\n" +
				"            end\r\n" +
				"        end\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}	
	}
}
