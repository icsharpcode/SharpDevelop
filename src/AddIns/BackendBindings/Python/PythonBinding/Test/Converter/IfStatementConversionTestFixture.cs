// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests the conversion of a C# if-else statement to python.
	/// </summary>
	[TestFixture]
	public class IfStatementConversionTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tint i = 0;\r\n" +
						"\tpublic int GetCount()\r\n" +
						"\t{" +
						"\t\tif (i == 0) {\r\n" +
						"\t\t\treturn 10;\r\n" +
						"\t\t} else {\r\n" +
						"\t\t\treturn 4;\r\n" +
						"\t\t}\r\n" +
						"\t}\r\n" +
						"}";
				
		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef __init__(self):\r\n" +
									"\t\tself._i = 0\r\n" +
									"\r\n" +
									"\tdef GetCount(self):\r\n" +
									"\t\tif self._i == 0:\r\n" +
									"\t\t\treturn 10\r\n" +
									"\t\telse:\r\n" +
									"\t\t\treturn 4";
			
			Assert.AreEqual(expectedPython, python);
		}		
	}
}
