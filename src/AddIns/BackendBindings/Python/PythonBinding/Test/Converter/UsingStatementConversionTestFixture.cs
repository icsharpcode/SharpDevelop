// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
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
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string python = converter.Convert(csharp);
			string expectedPython = "import System\r\n" +
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
			
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string python = converter.Convert(csharp);
			string expectedPython = "import System\r\n" +
									"import System.Drawing\r\n" +
									"class Foo(object):\r\n" +
									"\tpass";
			
			Assert.AreEqual(expectedPython, python);
	}
	}
}
