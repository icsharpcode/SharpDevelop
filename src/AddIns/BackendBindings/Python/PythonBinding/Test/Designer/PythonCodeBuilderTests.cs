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
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class PythonCodeBuilderTests
	{
		PythonCodeBuilder codeBuilder;
		
		[SetUp]
		public void Init()
		{
			codeBuilder = new PythonCodeBuilder();
			codeBuilder.IndentString = "\t";
		}
		
		/// <summary>
		/// Check that the "self._components = System.ComponentModel.Container()" line is generated
		/// the once and before any other lines of code.
		/// </summary>
		[Test]
		public void AppendCreateComponentsContainerTwice()
		{
			codeBuilder.IndentString = "  ";
			codeBuilder.IncreaseIndent();
			codeBuilder.AppendIndentedLine("self._listView = System.Windows.Forms.ListView()");
			codeBuilder.InsertCreateComponentsContainer();
			codeBuilder.InsertCreateComponentsContainer();
			
			string expectedCode = 
				"  self._components = System.ComponentModel.Container()\r\n" +
				"  self._listView = System.Windows.Forms.ListView()\r\n";
			
			Assert.AreEqual(expectedCode, codeBuilder.ToString());
		}
	}
}
