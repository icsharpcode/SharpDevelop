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
	/// <summary>
	/// Tests that method parameters are converted into Python
	/// correctly.
	/// </summary>
	[TestFixture]
	public class MethodParameterConversionTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic int Run(int i)\r\n" +
						"\t{\r\n" +
						"\t\treturn i;\r\n" +
						"\t}\r\n" +
						"}";

		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string code = converter.Convert(csharp);
			string expectedCode = "class Foo(object):\r\n" +
									"\tdef Run(self, i):\r\n" +
									"\t\treturn i";
			
			Assert.AreEqual(expectedCode, code);
		}
	}
}
