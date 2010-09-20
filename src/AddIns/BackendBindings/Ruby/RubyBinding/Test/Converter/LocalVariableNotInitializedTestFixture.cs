// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	[TestFixture]
	public class LocalVariableNotInitializedTestFixture
	{		
		string csharp =
			"class Foo\r\n" +
			"{\r\n" +
			"    public int GetCount()\r\n" +
			"    {\r\n" +
			"        int i;\r\n" +
			"        return 10;\r\n" +
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
				"    def GetCount()\r\n" +
				"        return 10\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
	}
}
