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
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	[TestFixture]
	public class VBStringConcatTestFixture
	{
		string vb = "Namespace DefaultNamespace\r\n" +
					"\tPublic Class Class1\r\n" +
					"\t\tPublic Sub Test\r\n" +
					"\t\t\tDim a as String\r\n" +
					"\t\t\ta = \"test\" & \" this\"\r\n" +
					"\t\tEnd Sub\r\n" +
					"\tEnd Class\r\n" +
					"End Namespace";

		[Test]
		public void GeneratedPythonSourceCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.VBNet);
			string python = converter.Convert(vb);
			string expectedPython = "class Class1(object):\r\n" +
									"\tdef Test(self):\r\n" +
									"\t\ta = \"test\" + \" this\"";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}
