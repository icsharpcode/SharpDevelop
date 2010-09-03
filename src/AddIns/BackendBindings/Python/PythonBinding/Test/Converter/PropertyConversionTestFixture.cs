// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests the CSharpToPythonConverter class can convert a C# property to
	/// two get and set methods in Python.
	/// </summary>
	[TestFixture]
	public class PropertyConversionTestFixture
	{	
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tint count = 0;\r\n" +
						"\tpublic int Count\r\n" +
						"\t{\r\n" +
						"\t\tget { return count; }\r\n" +
						"\t\tset { count = value; }\r\n" +
						"\t}\r\n" +
						"\r\n" +
						"\tpublic void Run()\r\n" +
						"\t{\r\n" +
						"\t}\r\n" +
						"}";
			
		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef __init__(self):\r\n" +
									"\t\tself._count = 0\r\n" +
									"\r\n" +
									"\tdef get_Count(self):\r\n" +
									"\t\treturn self._count\r\n" +
									"\r\n" +
									"\tdef set_Count(self, value):\r\n" +
									"\t\tself._count = value\r\n" +
									"\r\n" +
									"\tCount = property(fget=get_Count, fset=set_Count)\r\n" +
									"\r\n" +
									"\tdef Run(self):\r\n" +
									"\t\tpass";
											
			Assert.AreEqual(expectedPython, python);
		}	
	}
}
