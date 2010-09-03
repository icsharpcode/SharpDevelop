// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
							"            case 7:\r\n" +
							"                i = 4;\r\n" +
							"                break;\r\n" +
							"            case 10:\r\n" +
							"                return 0;\r\n" +
							"            case 9:\r\n" +
							"                return 2;\r\n" +
							"            case 8:\r\n" +
							"                break;\r\n" +
							"            default:\r\n" +
							"                return -1;\r\n" +
							"        }\r\n" +
							"        return i;\r\n" +
							"    }\r\n" +
							"}";

			string expectedPython = "class Foo(object):\r\n" +
									"  def Run(self, i):\r\n" +
									"    if i == 7:\r\n" +
									"      i = 4\r\n" +
									"    elif i == 10:\r\n" +
									"      return 0\r\n" +
									"    elif i == 9:\r\n" +
									"      return 2\r\n" +
									"    elif i == 8:\r\n" +
									"      pass\r\n" +
									"    else:\r\n" +
									"      return -1\r\n" +
									"    return i";
	
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "  ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedPython, code, code);
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
									"  def Run(self, i):\r\n" +
									"    if i == 10 or i == 11:\r\n" +
									"      return 0\r\n" +
									"    elif i == 9:\r\n" +
									"      return 2\r\n" +
									"    else:\r\n" +
									"      return -1";
	
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "  ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedPython, code);
		}		
	}
}
