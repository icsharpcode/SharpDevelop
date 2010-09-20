// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	[TestFixture]
	public class SwitchStatementConversionTestFixture
	{	
		[Test]
		public void SwitchStatement()
		{
			string csharp =
				"class Foo\r\n" +
				"{\r\n" +
				"    public int Run(int i)\r\n" +
				"    {\r\n" +
				"        switch (i) {\r\n" +
				"            case 7:\r\n" +
				"                i = 4;\r\n" +
				"                break;\r\n" +
				"            case 10:\r\n" +
				"                return 0;\r\n" +
				"            case 9:\r\n" +
				"                return 2;\r\n" +
				"            case 8:\r\n" +
				"                break;\r\n" +
				"            default:\r\n" +
				"                return -1;\r\n" +
				"        }\r\n" +
				"        return i;\r\n" +
				"    }\r\n" +
				"}";

			string expectedRuby =
				"class Foo\r\n" +
				"  def Run(i)\r\n" +
				"    case i\r\n" +
				"      when 7\r\n" +
				"        i = 4\r\n" +
				"      when 10\r\n" +
				"        return 0\r\n" +
				"      when 9\r\n" +
				"        return 2\r\n" +
				"      when 8\r\n" +
				"      else\r\n" +
				"        return -1\r\n" +
				"    end\r\n" +
				"    return i\r\n" +
				"  end\r\n" +
				"end";
	
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "  ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code, code);
		}	
		
		[Test]
		public void CaseFallThrough()
		{
			string csharp =
				"class Foo\r\n" +
				"{\r\n" +
				"    public int Run(int i)\r\n" +
				"    {\r\n" +
				"        switch (i) {\r\n" +
				"            case 10:\r\n" +
				"            case 11:\r\n" +
				"                return 0;\r\n" +
				"            case 9:\r\n" +
				"                return 2;\r\n" +
				"            default:\r\n" +
				"                return -1;\r\n" +
				"        }\r\n" +
				"    }\r\n" +
				"}";

			string expectedRuby =
				"class Foo\r\n" +
				"  def Run(i)\r\n" +
				"    case i\r\n" +
				"      when 10, 11\r\n" +
				"        return 0\r\n" +
				"      when 9\r\n" +
				"        return 2\r\n" +
				"      else\r\n" +
				"        return -1\r\n" +
				"    end\r\n" +
				"  end\r\n" +
				"end";
	
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "  ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedRuby, code);
		}		
	}
}
