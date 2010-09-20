// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class LoadCursorTestFixture : LoadFormTestFixtureBase
	{		
		public override string PythonCode {
			get {
				return "class TestForm(System.Windows.Forms.Form):\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # TestForm\r\n" +
							"        # \r\n" +
							"        self.Cursor = System.Windows.Forms.Cursors.Hand\r\n" +
							"        self.Name = \"TestForm\"\r\n" +
							"        self.ResumeLayout(False)\r\n";
			}
		}
		
		[Test]
		public void FormCursorIsHand()
		{
			Assert.AreEqual(Cursors.Hand, Form.Cursor);
		}
	}
}
