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
	/// Tests the CSharpToPythonConverter class can convert a method that
	/// returns an integer.
	/// </summary>
	[TestFixture]
	public class MethodReturnValueConversionTestFixture
	{		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic int Run()\r\n" +
						"\t{\r\n" +
						"\t\treturn 10;\r\n" +
						"\t}\r\n" +
						"}";

		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef Run(self):\r\n" +
									"\t\treturn 10";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}
