// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	[TestFixture]
	public class ForeachConversionWithMethodCallTestFixture
	{
		string environmentSpecialFolderCode = "class Foo\r\n" +
					"{\r\n" +
					"  public void PrintEnvironmentVariables()\r\n" +
					"  {\r\n" +
					"    foreach (Environment.SpecialFolder folder in Environment.SpecialFolder.GetValues(typeof(Environment.SpecialFolder)))\r\n" +
					"    {\r\n" +
					"        Console.WriteLine(\"{0}={1}\\n\", folder, Environment.GetFolderPath(folder));\r\n" +
					"    }\r\n" +
					"  }\r\n" +
					"}";
			
		[Test]
		public void ConvertedEnvironmentSpecialFolderCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "  ";
			string code = converter.Convert(environmentSpecialFolderCode);
			string expectedCode = "import clr\r\n" +
									"\r\n" +
									"class Foo(object):\r\n" +
									"  def PrintEnvironmentVariables(self):\r\n" +
									"    enumerator = Environment.SpecialFolder.GetValues(clr.GetClrType(Environment.SpecialFolder)).GetEnumerator()\r\n" +
									"    while enumerator.MoveNext():\r\n" +
									"      folder = enumerator.Current\r\n" +
									"      Console.WriteLine(\"{0}={1}\\n\", folder, Environment.GetFolderPath(folder))";		
			Assert.AreEqual(expectedCode, code);
		}		
	}
}
