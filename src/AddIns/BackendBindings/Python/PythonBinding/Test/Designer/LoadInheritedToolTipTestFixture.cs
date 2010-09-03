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
	public class LoadInheritedToolTipTestFixture : LoadFormTestFixtureBase
	{
		public override string PythonCode {
			get {
				ComponentCreator.AddType("PythonBinding.Tests.Designer.PublicToolTipDerivedForm", typeof(PythonBinding.Tests.Designer.PublicToolTipDerivedForm));

				return "class TestForm(PythonBinding.Tests.Designer.PublicToolTipDerivedForm):\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        self._button1 = System.Windows.Forms.Button()\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # button1\r\n" +
							"        # \r\n" +
							"        self._button1.Location = System.Drawing.Point(0, 0)\r\n" +
							"        self._button1.Name = \"button1\"\r\n" +
							"        self._button1.Size = System.Drawing.Size(10, 10)\r\n" +
							"        self._button1.TabIndex = 0\r\n" +
							"        self._button1.Text = \"button1\"\r\n" +
							"        self.toolTip.SetToolTip(self._button1, \"buttonTest\")\r\n" +
							"        # \r\n" +
							"        # MainForm\r\n" +
							"        # \r\n" +
							"        self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
							"        self.Controls.Add(self._button1)\r\n" +
							"        self.Name = \"MainForm\"\r\n" +
							"        self.toolTip.SetToolTip(self, \"test\")\r\n" +
							"        self.ResumeLayout(False)\r\n" +
							"        self.PerformLayout()\r\n";
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
