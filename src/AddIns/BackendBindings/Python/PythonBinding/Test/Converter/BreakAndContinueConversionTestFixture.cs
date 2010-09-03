// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests that a break statement is converted correctly.
	/// </summary>
	[TestFixture]	
	public class BreakAndContinueConversionTestFixture
	{		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic void Run()\r\n" +
						"\t{\r\n" +
						"\t\tint i = 0;\r\n" +
						"\t\twhile (i < 10) {\r\n" +
						"\t\t\tif (i == 5) {\r\n" +
						"\t\t\t\tbreak;\r\n" +
						"\t\t} else {\r\n" +
						"\t\t\tcontinue;\r\n" +
						"\t\t}\r\n" +
						"\t\ti++;\r\n" +
						"\t}\r\n" +
						"}";
				
		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef Run(self):\r\n" +
									"\t\ti = 0\r\n" +
									"\t\twhile i < 10:\r\n" +
									"\t\t\tif i == 5:\r\n" +
									"\t\t\t\tbreak\r\n" +
									"\t\t\telse:\r\n" +
									"\t\t\t\tcontinue\r\n" +
									"\t\t\ti += 1";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}
