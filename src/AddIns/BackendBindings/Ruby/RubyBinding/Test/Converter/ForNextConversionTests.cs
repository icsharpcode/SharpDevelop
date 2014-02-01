// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
