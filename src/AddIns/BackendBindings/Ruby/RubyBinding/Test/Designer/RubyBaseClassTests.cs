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
using ICSharpCode.SharpDevelop;
using ICSharpCode.RubyBinding;
using IronRuby.Compiler.Ast;
using NUnit.Framework;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class RubyBaseClassTests
	{
		[Test]
		public void FormBaseClass()
		{
			string code = "class MainForm < System::Windows::Forms::Form\r\n" +
							"    def InitializeComponent()\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # MainForm\r\n" +
							"        # \r\n" +
							"        self.ClientSize = System::Drawing::Size.new(300, 400)\r\n" +
							"        self.Name = \"MainForm\"\r\n" +
							"        self.ResumeLayout(false)\r\n" +
							"    end\r\n" +
							"end";
			
			ClassDefinition classDef = GetClassDefinition(code);
			
			Assert.AreEqual("System.Windows.Forms.Form", RubyComponentWalker.GetBaseClassName(classDef));
		}
		
		[Test]
		public void NoBaseClass()
		{
			string code = "class MainForm\r\n" +
							"    def InitializeComponent()\r\n" +
							"    end\r\n" +
							"end";
			
			ClassDefinition classDef = GetClassDefinition(code);
			
			Assert.AreEqual(String.Empty, RubyComponentWalker.GetBaseClassName(classDef));
		}
		
		[Test]
		public void UnqualifiedBaseClass()
		{
			string code = "class MainForm < Form\r\n" +
							"    def InitializeComponent()\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # MainForm\r\n" +
							"        # \r\n" +
							"        self.ClientSize = System::Drawing::Size.new(300, 400)\r\n" +
							"        self.Name = \"MainForm\"\r\n" +
							"        self.ResumeLayout(false)\r\n" +
							"    end\r\n" +
							"end";
			
			ClassDefinition classDef = GetClassDefinition(code);
			
			Assert.AreEqual("Form", RubyComponentWalker.GetBaseClassName(classDef));
		}		
		
		ClassDefinition GetClassDefinition(string code)
		{
			RubyParser parser = new RubyParser();
			SourceUnitTree unit = parser.CreateAst(@"test.rb", new StringTextBuffer(code));
			return unit.Statements.First as ClassDefinition;
		}
	}
}
