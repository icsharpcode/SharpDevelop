// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	[TestFixture]
	public class UsingStatementConversionTestFixture
	{
		string csharp = "using System\r\n" +
						"class Foo\r\n" +
						"{\r\n" +
						"}";

		[Test]
		public void GeneratedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			string expectedPython = "from System import *\r\n" +
									"\r\n" +
									"class Foo(object):\r\n" +
									"\tpass";
			
			Assert.AreEqual(expectedPython, python);
		}
		
		[Test]
		public void MultipleUsingStatements()
		{
			string csharp = "using System\r\n" +
							"using System.Drawing\r\n" +
						"class Foo\r\n" +
						"{\r\n" +
						"}";
			
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			string expectedPython = "from System import *\r\n" +
									"from System.Drawing import *\r\n" +
									"\r\n" +
									"class Foo(object):\r\n" +
									"\tpass";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}
