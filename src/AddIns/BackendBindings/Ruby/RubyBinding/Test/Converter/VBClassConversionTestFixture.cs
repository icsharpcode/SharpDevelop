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
using System.CodeDom;
using System.CodeDom.Compiler;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests that a VB.NET class is converted to Ruby successfully.
	/// </summary>
	[TestFixture]
	public class VBClassConversionTestFixture
	{
		string vb = "Namespace DefaultNamespace\r\n" +
					"    Public Class Class1\r\n" +
					"    End Class\r\n" +
					"End Namespace";

		[Test]
		public void GeneratedRubySourceCode()
		{
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.VBNet);
			converter.IndentString = "    ";
			string Ruby = converter.Convert(vb);
			string expectedRuby = 
				"module DefaultNamespace\r\n" +
				"    class Class1\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(expectedRuby, Ruby);
		}
	}
}
