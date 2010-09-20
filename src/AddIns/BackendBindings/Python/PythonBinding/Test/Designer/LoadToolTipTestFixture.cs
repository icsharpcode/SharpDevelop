// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class LoadToolTipTestFixture : LoadFormTestFixtureBase
	{
		public override string PythonCode {
			get {
				return "class TestForm(System.Windows.Forms.Form):\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        self._components = System.ComponentModel.Container()\r\n" +
							"        self._toolTip1 = System.Windows.Forms.ToolTip(self._components)\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # MainForm\r\n" +
							"        # \r\n" +
							"        self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
							"        self.Name = \"MainForm\"\r\n" +
							"        self._toolTip1.SetToolTip(self, \"test\")\r\n" +
							"        self.ResumeLayout(False)\r\n" +
							"        self.PerformLayout()\r\n";
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
