// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests that a VB.NET class is converted to Python successfully.
	/// </summary>
	[TestFixture]
	public class VBClassConversionTestFixture
	{
		string vb = "Namespace DefaultNamespace\r\n" +
					"\tPublic Class Class1\r\n" +
					"\tEnd Class\r\n" +
					"End Namespace";

		[Test]
		public void GeneratedPythonSourceCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.VBNet);
			string python = converter.Convert(vb);
			string expectedPython = "class Class1(object):\r\n" +
									"\tpass";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}
