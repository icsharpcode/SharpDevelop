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
	public class LoadButtonFlatAppearanceTestFixture : LoadFormTestFixtureBase
	{
		public override string RubyCode {
			get {
				return
					"class TestForm < System::Windows::Forms::Form\r\n" +
					"    def InitializeComponent()\r\n" +
					"        @button1 = System::Windows::Forms::Button.new()\r\n" +
					"        self.SuspendLayout()\r\n" +
					"        # \r\n" +
					"        # button1\r\n" +
					"        # \r\n" +
					"        @button1.FlatAppearance.BorderColor = System::Drawing::Color.Red\r\n" +
					"        @button1.FlatAppearance.BorderSize = 2\r\n" +
					"        @button1.FlatAppearance.MouseDownBackColor = System::Drawing::Color.Blue\r\n" +
					"        @button1.FlatAppearance.MouseOverBackColor = System::Drawing::Color.Yellow\r\n" +
					"        @button1.Location = System::Drawing::Point.new(0, 0)\r\n" +
					"        @button1.Name = \"button1\"\r\n" +
					"        @button1.Size = System::Drawing::Size.new(10, 10)\r\n" +
					"        @button1.TabIndex = 0\r\n" +
					"        @button1.Text = \"button1\"\r\n" +
					"        # \r\n" +
					"        # MainForm\r\n" +
					"        # \r\n" +
					"        self.AcceptButton = @button1\r\n" +
					"        self.ClientSize = System::Drawing::Size.new(200, 300)\r\n" +
					"        self.Name = \"MainForm\"\r\n" +
					"        self.Controls.Add(@button1)\r\n" +
					"        self.ResumeLayout(false)\r\n" +
					"    end\r\n" +
					"end";
			}
		}
		
		public Button Button
		{
			get { return Form.Controls[0] as Button; }
		}
		
		[Test]
		public void ButtonFlatAppearanceBorderSize()
		{
			Assert.AreEqual(2, Button.FlatAppearance.BorderSize);
		}
		
		[Test]
		public void ButtonFlatAppearanceBorderColor()
		{
			Assert.AreEqual(Color.Red, Button.FlatAppearance.BorderColor);
		}
	}
}
