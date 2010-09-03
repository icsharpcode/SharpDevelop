// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class LoadLocalVariablePropertyAssignmentTestFixture : LoadFormTestFixtureBase
	{		
		public override string PythonCode {
			get {
				return "class MainForm(System.Windows.Forms.Form):\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        button1 = System.Windows.Forms.Button()\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # MainForm\r\n" +
							"        # \r\n" +
							"        self.AcceptButton = button1\r\n" +
							"        self.ClientSize = System.Drawing.Size(300, 400)\r\n" +
							"        self.Name = \"MainForm\"\r\n" +
							"        self.ResumeLayout(False)\r\n";
			}
		}
		
		[Test]
		public void OneComponentCreated()
		{
			Assert.AreEqual(1, ComponentCreator.CreatedComponents.Count);
		}
		
		[Test]
		public void TwoObjectsCreated()
		{
			Assert.AreEqual(2, ComponentCreator.CreatedInstances.Count);
		}
		
		[Test]
		public void ButtonInstance()
		{			
			CreatedInstance expectedInstance = new CreatedInstance(typeof(Button), new object[0], "button1", false);
			Assert.AreEqual(expectedInstance, ComponentCreator.CreatedInstances[0]);
		}
		
		[Test]
		public void AcceptButtonPropertyIsNotNull()
		{
			Assert.IsNotNull(Form.AcceptButton);
		}
	}
}
