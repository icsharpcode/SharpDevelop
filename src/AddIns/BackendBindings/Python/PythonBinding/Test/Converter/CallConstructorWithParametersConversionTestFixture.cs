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
	/// Tests that parameters passed to a constructor are converted to Python correctly.
	/// </summary>
	[TestFixture]
	public class CallConstructorWithParametersConversionTestFixture
	{	
		string csharp = "class Foo\r\n" +
					"{\r\n" +
					"\tpublic Foo()\r\n" +
					"\t{\r\n" +
					"\t\tBar b = new Bar(0, 0, 1, 10);\r\n" +
					"\t}\r\n" +
					"}";
		
		[Test]
		public void ConvertedPythonCode()
		{
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef __init__(self):\r\n" +
									"\t\tb = Bar(0, 0, 1, 10)";
			
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}
