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
	public class LoadSimpleUserControlTestFixture
	{	
		MockComponentCreator componentCreator = new MockComponentCreator();
		UserControl userControl;
		
		public string PythonCode {
			get {
				return "class MainForm(System.Windows.Forms.UserControl):\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # userControl1\r\n" +
							"        # \r\n" +
							"        self.ClientSize = System.Drawing.Size(300, 400)\r\n" +
							"        self.Name = \"userControl1\"\r\n" +
							"        self.ResumeLayout(False)\r\n";
			}
		}

		[TestFixtureSetUp]
		public void SetUpFixture()
		{			
			PythonComponentWalker walker = new PythonComponentWalker(componentCreator);
			userControl = walker.CreateComponent(PythonCode) as UserControl;
		}

		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			userControl.Dispose();
		}
		
		[Test]
		public void UserControlCreated()
		{			
			Assert.IsNotNull(userControl);
		}
		
		[Test]
		public void UserControlName()
		{
			Assert.AreEqual("userControl1", userControl.Name);
		}
		
		[Test]
		public void UserControlClientSize()
		{
			Size size = new Size(300, 400);
			Assert.AreEqual(size, userControl.ClientSize);
		}
	}
}
