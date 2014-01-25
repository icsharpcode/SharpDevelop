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

using ICSharpCode.SharpDevelop;
using System;
using ICSharpCode.PythonBinding;
using IronPython.Compiler.Ast;
using NUnit.Framework;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class PythonBaseClassTests
	{
		[Test]
		public void FormBaseClass()
		{
			string code = "class MainForm(System.Windows.Forms.Form):\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # MainForm\r\n" +
							"        # \r\n" +
							"        self.ClientSize = System.Drawing.Size(300, 400)\r\n" +
							"        self.Name = \"MainForm\"\r\n" +
							"        self.ResumeLayout(False)\r\n";
			
			ClassDefinition classDef = GetClassDefinition(code);
			
			Assert.AreEqual("System.Windows.Forms.Form", PythonComponentWalker.GetBaseClassName(classDef));
		}
		
		[Test]
		public void NoBaseClass()
		{
			string code = "class MainForm:\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        pass\r\n";
			
			ClassDefinition classDef = GetClassDefinition(code);
			
			Assert.AreEqual(String.Empty, PythonComponentWalker.GetBaseClassName(classDef));
		}
		
		[Test]
		public void UnqualifiedBaseClass()
		{
			string code = "class MainForm(Form):\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # MainForm\r\n" +
							"        # \r\n" +
							"        self.ClientSize = System.Drawing.Size(300, 400)\r\n" +
							"        self.Name = \"MainForm\"\r\n" +
							"        self.ResumeLayout(False)\r\n";
			
			ClassDefinition classDef = GetClassDefinition(code);
			
			Assert.AreEqual("Form", PythonComponentWalker.GetBaseClassName(classDef));
		}		
		
		ClassDefinition GetClassDefinition(string code)
		{
			PythonParser parser = new PythonParser();
			PythonAst ast = parser.CreateAst(@"test.py", new StringTextBuffer(code));
			SuiteStatement suite = ast.Body as SuiteStatement;
			return suite.Statements[0] as ClassDefinition;
		}
	}
}
