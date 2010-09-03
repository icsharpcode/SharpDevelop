// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class LoadToolTipTestFixture : LoadFormTestFixtureBase
	{
		public override string RubyCode {
			get {
				return
					"class TestForm < System::Windows::Forms::Form\r\n" +
					"    def InitializeComponent()\r\n" +
					"        @components = System::ComponentModel::Container.new()\r\n" +
					"        @toolTip1 = System::Windows::Forms::ToolTip.new(@components)\r\n" +
					"        self.SuspendLayout()\r\n" +
					"        # \r\n" +
					"        # MainForm\r\n" +
					"        # \r\n" +
					"        self.ClientSize = System::Drawing::Size.new(284, 264)\r\n" +
					"        self.Name = \"MainForm\"\r\n" +
					"        @toolTip1.SetToolTip(self, \"test\")\r\n" +
					"        self.ResumeLayout(false)\r\n" +
					"        self.PerformLayout()\r\n" +
					"    end\r\n" +
					"end";
			}
		}
		
		[Test]
		public void FormHasToolTip()
		{
			ToolTip toolTip = (ToolTip)base.ComponentCreator.GetComponent("toolTip1");
			Assert.AreEqual("test",toolTip.GetToolTip(Form));
		}
	}
}
