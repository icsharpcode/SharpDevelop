// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
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
		public void ConvertedRubyCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.VBNet);
			converter.IndentString = "    ";
			string ruby = converter.Convert(vb);
			string expectedRuby =
				"class Class1\r\n" +
				"    def Test()\r\n" +
				"        while true\r\n" +
				"            break\r\n" +
				"        end\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, ruby);
		}
	}
}
