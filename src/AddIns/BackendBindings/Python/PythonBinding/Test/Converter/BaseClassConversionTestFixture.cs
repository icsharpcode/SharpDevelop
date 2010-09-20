// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	[TestFixture]
	public class BaseClassConversionTestFixture
	{
		string csharp = 
			"class Foo : Bar, IMyInterface\r\n" +
			"{\r\n" +
			"}";

		[Test]
		public void ConvertedPythonCode()
		{
			string expectedCode =
				"class Foo(Bar, IMyInterface):\r\n" +
				"    pass";
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string code = converter.Convert(csharp);
			
			Assert.AreEqual(expectedCode, code);
		}
	}
}
