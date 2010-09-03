// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Statements before the main form class are parsed by the RubyComponentWalker when 
	/// they should be ignored. This test fixture checks that only when the InitializeComponent method
	/// is found the statements are parsed.
	/// </summary>
	[TestFixture]
	public class LoadFormWithStatementsBeforeFormTestFixture : LoadFormTestFixtureBase
	{		
		public override string RubyCode {
			get {	
				return
					"a = System::Windows::Forms::TextBox.new()\r\n" +
					"b = 10\r\n" +
					"\r\n" +
					"class Foo\r\n" +
					"    def run()\r\n" +
					"        @a = System::Windows::Forms::ListViewItem.new()\r\n" +
					"    end\r\n" +
					"end\r\n" +
					"\r\n" +
					"class MainForm < System::Windows::Forms::Form\r\n" +
					"    def InitializeComponent()\r\n" +
					"        self.SuspendLayout()\r\n" +
					"        # \r\n" +
					"        # MainForm\r\n" +
					"        # \r\n" +
					"        self.Name = \"MainForm\"\r\n" +
					"        self.ResumeLayout(false)\r\n" +
					"    end\r\n" +
					"end";
			}
		}
				
		public CreatedComponent FormComponent {
			get { return ComponentCreator.CreatedComponents[0]; }
		}
		
		[Test]
		public void MainFormCreated()
		{			
			Assert.IsNotNull(Form);
		}
		
		[Test]
		public void NoInstancesCreated()
		{
			Assert.AreEqual(0, ComponentCreator.CreatedInstances.Count);
		}
	}
}
