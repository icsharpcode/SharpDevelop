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
	/// Tests a single class method is converted.
	/// </summary>
	[TestFixture]
	public class SingleClassMethodConversionTestFixture
	{		
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"\tpublic void Init()\r\n" +
						"\t{\r\n" +
						"\t}\r\n" +
						"}";
			
		[Test]
		public void GeneratedPythonCode()
		{
			CSharpToPythonConverter converter = new CSharpToPythonConverter();
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
									"\tdef Init(self):\r\n" +
									"\t\tpass";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}
