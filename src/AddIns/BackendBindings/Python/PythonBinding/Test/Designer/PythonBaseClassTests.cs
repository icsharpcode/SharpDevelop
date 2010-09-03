// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
