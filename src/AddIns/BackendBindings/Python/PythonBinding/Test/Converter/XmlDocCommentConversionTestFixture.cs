// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
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
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"    \"\"\" <summary>\r\n" +
									"     Class Foo\r\n" +
									"     </summary>\r\n" +
									"    \"\"\"\r\n" +
									"    def Run(self):\r\n" +
									"        \"\"\" <summary>\r\n" +
									"         Run\r\n" +
									"         </summary>\r\n" +
									"        \"\"\"\r\n" +
									"        pass\r\n" +
									"\r\n" +
									"    def Stop(self):\r\n" +
									"        \"\"\" <summary> Stop </summary>\"\"\"\r\n" +
									"        pass\r\n" +
									"\r\n" +
									"    def __init__(self):\r\n" +
									"        \"\"\" <summary> Initialize.</summary>\"\"\"\r\n" +
									"        # Initialize j.\r\n" +
									"        j = 0 # Set to zero\r\n" +
									"        # test\r\n" +
									"        if j == 0:\r\n" +
									"            j = 2";
			
			Assert.AreEqual(expectedPython, python, python);
		}
	}
}
