// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
