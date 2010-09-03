// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	[TestFixture]
	public class TypeofConversionTestFixture
	{
		string typeofIntCode = "class Foo\r\n" +
						"{\r\n" +
						"    public string ToString()\r\n" +
						"    {\r\n" +
						"        typeof(int).FullName;\r\n" +
						"    }\r\n" +
						"}";
				
		[Test]
		public void ConvertedTypeOfIntegerCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string python = converter.Convert(typeofIntCode);
			string expectedPython = "import clr\r\n" +
									"\r\n" +
									"class Foo(object):\r\n" +
									"    def ToString(self):\r\n" +
									"        clr.GetClrType(int).FullName";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}
