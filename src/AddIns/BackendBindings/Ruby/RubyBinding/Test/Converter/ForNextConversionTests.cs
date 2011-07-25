// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	[TestFixture]
	public class ForNextConversionTests
	{
		string vbnetForNextCode = 
			"Public Class Foo\r\n" +
			"    Public Function GetCount() As Integer\r\n" +
			"        Dim count As Integer = 0\r\n" +
			"        For i As Integer = 0 To 4\r\n" +
			"            count += 1\r\n" +
			"        Next\r\n" +
			"        Return count\r\n" +
			"    End Function\r\n" +
			"End Class\r\n";

		[Test]
		public void ConvertVBNetForNextToRuby()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.VBNet);
			converter.IndentString = "    ";
			string code = converter.Convert(vbnetForNextCode);
			string expectedCode = 
				"class Foo\r\n" +
				"    def GetCount()\r\n" +
				"        count = 0\r\n" +
				"        i = 0\r\n" +
				"        while i <= 4\r\n" +
				"            count += 1\r\n" +
				"            i = i + 1\r\n" +
				"        end\r\n" +
				"        return count\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedCode, code);
		}
		
		string vbnetForNextWithStepCode = 
			"Public Class Foo\r\n" +
			"    Public Function GetCount() As Integer\r\n" +
			"        Dim count As Integer = 0\r\n" +
			"        For i As Integer = 0 To 4 Step 2\r\n" +
			"            count += 1\r\n" +
			"        Next\r\n" +
			"        Return count\r\n" +
			"    End Function\r\n" +
			"End Class\r\n";

		[Test]
		public void ConvertVBNetForNextWithStepToRuby()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.VBNet);
			converter.IndentString = "    ";
			string code = converter.Convert(vbnetForNextWithStepCode);
			string expectedCode = 
				"class Foo\r\n" +
				"    def GetCount()\r\n" +
				"        count = 0\r\n" +
				"        i = 0\r\n" +
				"        while i <= 4\r\n" +
				"            count += 1\r\n" +
				"            i = i + 2\r\n" +
				"        end\r\n" +
				"        return count\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedCode, code);
		}
	}
}
