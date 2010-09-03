// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests that the indentation after the nested class is correct for any outer class methods.
	/// </summary>
	[TestFixture]	
	public class NestedClassConversionTestFixture
	{		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic void Run()\r\n" +
						"\t{\r\n" +
						"\t}\r\n" +
						"\r\n" +
						"\t\tclass Bar\r\n" +
						"\t\t{\r\n" +
						"\t\t\tpublic void Test()\r\n" +
						"\t\t\t{\r\n" +
						"\t\t\t}\r\n" +
						"\t\t}\r\n" +
						"\r\n" +
						"\tpublic void AnotherRun()\r\n" +
						"\t{\r\n" +
						"\t}\r\n" +
						"}";
				
		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef Run(self):\r\n" +
									"\t\tpass\r\n" +
									"\r\n" +
									"\tclass Bar(object):\r\n" +
									"\t\tdef Test(self):\r\n" +
									"\t\t\tpass\r\n" +
									"\r\n" +
									"\tdef AnotherRun(self):\r\n" +
									"\t\tpass";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}
