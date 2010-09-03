// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	[TestFixture]
	public class PropertyWithGetterTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    int count = 0;\r\n" +
						"    int i = 0;\r\n" +
						"    public int Count {\r\n" +
						"        get {\r\n" +
						"            if (i == 0) {\r\n" +
						"                return 10;\r\n" +
						"            } else {\r\n" +
						"                return count;\r\n" +
						"            }\r\n" +
						"        }\r\n" +
						"    }\r\n" +
						"}";
			
		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"    def __init__(self):\r\n" +
									"        self._count = 0\r\n" +
									"        self._i = 0\r\n" +
									"\r\n" +
									"    def get_Count(self):\r\n" +
									"        if self._i == 0:\r\n" +
									"            return 10\r\n" +
									"        else:\r\n" +
									"            return self._count\r\n" +
									"\r\n" +
									"    Count = property(fget=get_Count)";
			
			Assert.AreEqual(expectedPython, python, python);
		}
	}
}
