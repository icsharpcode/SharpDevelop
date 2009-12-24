// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
