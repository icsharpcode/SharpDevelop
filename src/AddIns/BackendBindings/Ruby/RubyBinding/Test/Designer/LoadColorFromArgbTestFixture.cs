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
	public class LoadColorFromArgbTestFixture : LoadFormTestFixtureBase
	{		
		public override string RubyCode {
			get {
				return
					"class TestForm < System::Windows::Forms::Form\r\n" +
					"    def InitializeComponent()\r\n" +
					"        self.SuspendLayout()\r\n" +
					"        # \r\n" +
					"        # TestForm\r\n" +
					"        # \r\n" +
					"        self.BackColor = System::Drawing::Color.FromArgb(10, 190, 0)\r\n" +
					"        self.Name = \"TestForm\"\r\n" +
					"        self.ResumeLayout(false)\r\n" +
					"    end\r\n" +
					"end";
			}
		}
								
		[Test]
		public void FormBackColor()
		{
			Assert.AreEqual(Color.FromArgb(10, 190, 0), Form.BackColor);
		}
	}
}
