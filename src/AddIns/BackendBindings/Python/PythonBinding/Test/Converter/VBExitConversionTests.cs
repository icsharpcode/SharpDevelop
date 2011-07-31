// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	[TestFixture]
	public class VBExitConversionTests
	{
		string vb =
			"Public Class Class1\r\n" +
			"    Public Sub Test\r\n" +
			"        While True\r\n" +
			"            Exit While\r\n" +
			"        End While\r\n" +
			"    End Sub\r\n" +
			"End Class\r\n";
		
		[Test]
		public void ConvertedPythonCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.VBNet);
			converter.IndentString = "    ";
			string python = converter.Convert(vb);
			string expectedPython =
				"class Class1(object):\r\n" +
				"    def Test(self):\r\n" +
				"        while True:\r\n" +
				"            break";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}
