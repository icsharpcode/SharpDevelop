// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	[TestFixture]
	public class WhileLoopConversionTestFixture
	{		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic void CountDown()\r\n" +
						"\t{\r\n" +
						"\t\tint i = 10;\r\n" +
						"\t\twhile (i > 0) {\r\n" +
						"\t\t\ti--;\r\n" +
						"\t\t}\r\n" +
						"\t}\r\n" +
						"}";
				
		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef CountDown(self):\r\n" +
									"\t\ti = 10\r\n" +
									"\t\twhile i > 0:\r\n" +
									"\t\t\ti -= 1";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}
