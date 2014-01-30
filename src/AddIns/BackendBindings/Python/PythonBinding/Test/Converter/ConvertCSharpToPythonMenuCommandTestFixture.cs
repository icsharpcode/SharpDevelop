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
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Converter
{
	[TestFixture]
	public class ConvertCSharpToPythonMenuCommandTestFixture : ConvertToPythonMenuCommand
	{
		string newFileText;
		string defaultFileName;
		string language;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			MockWorkbench workbench = MockWorkbench.CreateWorkbenchWithOneViewContent("test.cs");
			workbench.ActiveMockEditableViewContent.Text = "class Foo { }";
			
			MockTextEditorOptions options = workbench.ActiveMockEditableViewContent.MockTextEditorOptions;
			options.IndentationSize = 4;
			options.ConvertTabsToSpaces = true;
			
			Run(workbench);
		}
		
		[Test]
		public void Run_CSharpFileOpen_PythonCodeGeneratedInNewFile()
		{
			string code = 
				"class Foo(object):\r\n" +
				"    pass";
			Assert.AreEqual(code, newFileText);
		}
		
		[Test]
		public void Run_CSharpFileOpen_NewFileNameIsGeneratedPy()
		{
			Assert.AreEqual("Generated.py", defaultFileName);
		}

		[Test]
		public void Run_CSharpFileOpen_NewFileLanguageIsPython()
		{
			Assert.AreEqual("Python", language);
		}
		
		protected override void NewFile(string defaultName, string language, string content)
		{
			defaultFileName = defaultName;
			this.language = language;
			newFileText = content;
		}
	}
}
