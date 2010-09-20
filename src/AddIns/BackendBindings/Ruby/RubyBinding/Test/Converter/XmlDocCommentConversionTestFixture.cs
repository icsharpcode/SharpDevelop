// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	[TestFixture]
	public class XmlDocCommentConversionTestFixture
	{
		string csharp = "/// <summary>\r\n" +
						"/// Class Foo\r\n" +
						"/// </summary>\r\n" +
						"public class Foo\r\n" +
						"{\r\n" +
						"    /// <summary>\r\n" +
						"    /// Run\r\n" +
						"    /// </summary>\r\n" +
						"    public void Run()\r\n" +
						"    {\r\n" +
						"    }\r\n" +
						"\r\n" +
						"    /// <summary> Stop </summary>\r\n" +
						"    public void Stop()\r\n" +
						"    {\r\n" +
						"    }\r\n" +
						"\r\n" +
						"    /// <summary> Initialize.</summary>\r\n" +
						"    public Foo()\r\n" +
						"    {\r\n" +
						"        /// Initialize j.\r\n" +
						"        int j = 0; /// Set to zero\r\n" +
						"        /// test\r\n" +
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
				"# <summary>\r\n" +
				"# Class Foo\r\n" +
				"# </summary>\r\n" +
				"class Foo\r\n" +
				"    # <summary>\r\n" +
				"    # Run\r\n" +
				"    # </summary>\r\n" +
				"    def Run()\r\n" +
				"    end\r\n" +
				"\r\n" +
				"    # <summary> Stop </summary>\r\n" +
				"    def Stop()\r\n" +
				"    end\r\n" +
				"\r\n" +
				"    # <summary> Initialize.</summary>\r\n" +
				"    def initialize()\r\n" +
				"        # Initialize j.\r\n" +
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
