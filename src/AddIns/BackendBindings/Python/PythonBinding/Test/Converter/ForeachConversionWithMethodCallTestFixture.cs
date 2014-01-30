// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
