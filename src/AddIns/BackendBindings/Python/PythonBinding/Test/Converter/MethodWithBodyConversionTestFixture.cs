// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests the CSharpToPythonConverter class can convert a method with
	/// simple integer assignment in the body.
	/// </summary>
	[TestFixture]
	public class MethodWithBodyConversionTestFixture
	{	
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic void Run()\r\n" +
						"\t{\r\n" +
						"\t\tint i = 0;\r\n" +
						"\t}\r\n" +
						"}";
			
		[Test]
		public void ConvertedPythonCode()
		{
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef Run(self):\r\n" +
									"\t\ti = 0";
			
			Assert.AreEqual(expectedPython, python);
		}		
	}
}
