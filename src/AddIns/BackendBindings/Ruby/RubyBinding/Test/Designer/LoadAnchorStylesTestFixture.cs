// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class LoadAnchorStylesTestFixture : LoadFormTestFixtureBase
	{
		public override string RubyCode {
			get {
				return "class TestForm < System::Windows::Forms::Form\r\n" +
							"    def InitializeComponent()\r\n" +
							"        @textBox1 = System::Windows::Forms::TextBox.new()\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # textBox1\r\n" +
							"        # \r\n" +
							"        @textBox1.Name = \"textBoxName\"\r\n" +
							"        @textBox1.Anchor = System::Windows::Forms::AnchorStyles.Top | System::Windows::Forms::AnchorStyles.Bottom | System::Windows::Forms::AnchorStyles.Left\r\n" +
							"        # \r\n" +
							"        # TestForm\r\n" +
							"        # \r\n" +
							"        self.Name = \"TestForm\"\r\n" +
							"        self.Controls.Add(@textBox1)\r\n" +
							"        self.ResumeLayout(false)\r\n" +
							"    end\r\n" +
							"end";
			}
		}
		
		[TestFixtureSetUp]
		public new void SetUpFixture()
		{
			base.SetUpFixture();
		}
		
		[Test]
		public void TextBoxAnchorStyle()
		{
			AnchorStyles style = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			TextBox textBox = Form.Controls[0] as TextBox;
			Assert.AreEqual(style, textBox.Anchor);
		}
	}
}
