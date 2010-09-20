// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	[TestFixture]
	public class TernaryOperatorConversionTestFixture
	{		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic string TestMe(bool test)\r\n" +
						"\t{\r\n" +
						"\t\tstring a = test ? \"Ape\" : \"Monkey\";\r\n" +
						"\t\treturn a;\r\n" +
						"\t\t}\r\n" +
						"\t}\r\n" +
						"}";
				
		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef TestMe(self, test):\r\n" +
									"\t\ta = \"Ape\" if test else \"Monkey\"\r\n" +
									"\t\treturn a";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}
