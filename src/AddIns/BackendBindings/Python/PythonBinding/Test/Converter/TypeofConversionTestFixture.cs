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
	public class TypeofConversionTestFixture
	{
		string typeofIntCode = "class Foo\r\n" +
						"{\r\n" +
						"    public string ToString()\r\n" +
						"    {\r\n" +
						"        typeof(int).FullName;\r\n" +
						"    }\r\n" +
						"}";
				
		[Test]
		public void ConvertedTypeOfIntegerCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string python = converter.Convert(typeofIntCode);
			string expectedPython = "import clr\r\n" +
									"\r\n" +
									"class Foo(object):\r\n" +
									"    def ToString(self):\r\n" +
									"        clr.GetClrType(int).FullName";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}
