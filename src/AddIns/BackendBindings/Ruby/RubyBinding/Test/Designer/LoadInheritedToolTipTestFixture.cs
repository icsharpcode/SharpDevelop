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
	public class LoadInheritedToolTipTestFixture : LoadFormTestFixtureBase
	{
		public override string RubyCode {
			get {
				ComponentCreator.AddType("RubyBinding.Tests.Designer.PublicToolTipDerivedForm", typeof(RubyBinding.Tests.Designer.PublicToolTipDerivedForm));

				return
					"class TestForm < RubyBinding::Tests::Designer::PublicToolTipDerivedForm\r\n" +
					"    def InitializeComponent()\r\n" +
					"        @button1 = System::Windows::Forms::Button.new()\r\n" +
					"        self.SuspendLayout()\r\n" +
					"        # \r\n" +
					"        # button1\r\n" +
					"        # \r\n" +
					"        @button1.Location = System::Drawing::Point.new(0, 0)\r\n" +
					"        @button1.Name = \"button1\"\r\n" +
					"        @button1.Size = System::Drawing::Size.new(10, 10)\r\n" +
					"        @button1.TabIndex = 0\r\n" +
					"        @button1.Text = \"button1\"\r\n" +
					"        self.toolTip.SetToolTip(@button1, \"buttonTest\")\r\n" +
					"        # \r\n" +
					"        # MainForm\r\n" +
					"        # \r\n" +
					"        self.ClientSize = System::Drawing::Size.new(284, 264)\r\n" +
					"        self.Controls.Add(@button1)\r\n" +
					"        self.Name = \"MainForm\"\r\n" +
					"        self.toolTip.SetToolTip(self, \"test\")\r\n" +
					"        self.ResumeLayout(false)\r\n" +
					"        self.PerformLayout()\r\n" +
					"    end\r\n" +
					"end";
			}
		}
		
		[Test]
		public void FormHasToolTip()
		{
			PublicToolTipDerivedForm form = Form as PublicToolTipDerivedForm;
			Assert.AreEqual("test", form.toolTip.GetToolTip(form));
		}
		
		[Test]
		public void ButtonHasToolTip()
		{
			PublicToolTipDerivedForm form = Form as PublicToolTipDerivedForm;
			Assert.AreEqual("buttonTest", form.toolTip.GetToolTip(form.Controls[0]));
		}		
	}
}
