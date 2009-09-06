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
	public class ObjectReferenceEqualsConversionTestFixture
	{		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    public bool IsEqual(object o)\r\n" +
						"    {\r\n" +
						"        return object.ReferenceEquals(o, null);\r\n" +
						"    }\r\n" +
						"}";
				
		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"    def IsEqual(self, o):\r\n" +
									"        return Object.ReferenceEquals(o, None)";
			
			Assert.AreEqual(expectedPython, python, python);
		}
	}
}

