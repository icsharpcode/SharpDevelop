// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	[TestFixture]
	public class MultiLineCommentConversionTestFixture
	{
		string csharp = "/* \r\n" +
						"Class Foo\r\n" +
						"*/ \r\n" +
						"public class Foo\r\n" +
						"{\r\n" +
						"    /* Initialize. */\r\n" +
						"    public Foo()\r\n" +
						"    {\r\n" +
						"        /* Initialize j.\r\n" +
						"        set to zero */\r\n" +
						"        j = 0; /* Set to zero */\r\n" +
						"        /* test */\r\n" +
						"        if (j == 0) j = 2;\r\n" +
						"    }\r\n" +
						"}";
		[Test]
		public void ConvertedRubyCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(csharp);
			string expectedRuby =
				"# \r\n" +
				"# Class Foo\r\n" +
				"# \r\n" +
				"class Foo\r\n" +
				"    # Initialize.\r\n"+
				"    def initialize()\r\n" +
				"        # Initialize j.\r\n" +
				"        # set to zero\r\n" +
				"        j = 0 # Set to zero\r\n" +
				"        # test\r\n" +
				"        if j == 0 then\r\n" +
				"            j = 2\r\n" +
				"        end\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, Ruby, Ruby);
		}
	}
}
