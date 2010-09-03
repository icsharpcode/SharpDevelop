// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
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
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string python = converter.Convert(csharp);
			string expectedPython = "# \r\n" +
									"# Class Foo\r\n" +
									"# \r\n" +
									"class Foo(object):\r\n" +
									"    # Initialize.\r\n"+
									"    def __init__(self):\r\n" +
									"        # Initialize j.\r\n" +
									"        # set to zero\r\n" +
									"        j = 0 # Set to zero\r\n" +
									"        # test\r\n" +
									"        if j == 0:\r\n" +
									"            j = 2";
			
			Assert.AreEqual(expectedPython, python, python);
		}
	}
}
