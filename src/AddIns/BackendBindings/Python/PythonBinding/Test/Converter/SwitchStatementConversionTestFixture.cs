// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	[TestFixture]
	public class SwitchStatementConversionTestFixture
	{	
		[Test]
		public void SwitchStatement()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public int Run(int i)\r\n" +
							"    {\r\n" +
							"        switch (i) {\r\n" +
							"            case 10:\r\n" +
							"                return 0;\r\n" +
							"            case 9:\r\n" +
							"                return 2;\r\n" +
							"            default:\r\n" +
							"                return -1;\r\n" +
							"        }\r\n" +
							"    }\r\n" +
							"}";

			string expectedPython = "class Foo(object):\r\n" +
									"\tdef Run(self, i):\r\n" +
									"\t\tif i == 10:\r\n" +
									"\t\t\treturn 0\r\n" +
									"\t\telif i == 9:\r\n" +
									"\t\t\treturn 2\r\n" +
									"\t\telse:\r\n" +
									"\t\t\treturn -1";
	
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedPython, code);
		}	
		
		[Test]
		public void CaseFallThrough()
		{
			string csharp = "class Foo\r\n" +
							"{\r\n" +
							"    public int Run(int i)\r\n" +
							"    {\r\n" +
							"        switch (i) {\r\n" +
							"            case 10:\r\n" +
							"            case 11:\r\n" +
							"                return 0;\r\n" +
							"            case 9:\r\n" +
							"                return 2;\r\n" +
							"            default:\r\n" +
							"                return -1;\r\n" +
							"        }\r\n" +
							"    }\r\n" +
							"}";

			string expectedPython = "class Foo(object):\r\n" +
									"\tdef Run(self, i):\r\n" +
									"\t\tif i == 10 or i == 11:\r\n" +
									"\t\t\treturn 0\r\n" +
									"\t\telif i == 9:\r\n" +
									"\t\t\treturn 2\r\n" +
									"\t\telse:\r\n" +
									"\t\t\treturn -1";
	
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedPython, code);
		}		
	}
}
