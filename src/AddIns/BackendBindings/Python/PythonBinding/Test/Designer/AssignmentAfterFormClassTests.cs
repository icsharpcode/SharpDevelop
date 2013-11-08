// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Resources;
using ICSharpCode.Core;
using NUnit.Framework;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests that only the InitializeComponents method is processed
	/// when walking the AST.
	/// </summary>
	[TestFixture]
	public class AssignmentAfterFormClassTests : LoadFormTestFixtureBase
	{
		public override void BeforeSetUpFixture()
		{
			var rm = new ResourceManager("PythonBinding.Tests.Strings", GetType().Assembly);
			ResourceService.RegisterNeutralStrings(rm);
		}
		
		public override string PythonCode {
			get {
				return 
					"import clr\r\n" +
					"clr.AddReference('System.Windows.Forms')\r\n" +
					"clr.AddReference('System.Drawing')\r\n" +
					"\r\n" +
					"import System.Drawing\r\n" +
					"import System.Windows.Forms\r\n" +
					"\r\n" +
					"from System.Drawing import *\r\n" +
					"from System.Windows.Forms import *\r\n" +
					"\r\n" +
					"class MainForm(System.Windows.Forms.Form):\r\n" +
					"    def __init__(self):\r\n" +
					"        self.InitializeComponent()\r\n" +
					"    \r\n" +
					"    def InitializeComponent(self):\r\n" +
					"        self.SuspendLayout()\r\n" +
					"        # \r\n" +
					"        # MainForm\r\n" +
					"        # \r\n" +
					"        self.ClientSize = System.Drawing.Size(300, 400)\r\n" +
					"        self.Name = \"MainForm\"\r\n" +
					"        self.ResumeLayout(False)\r\n" +
					"\r\n" +
					"test = MainForm()\r\n" +
					"Application.Run(test)\r\n" +
					"\r\n";
			}
		}
		
		[Test]
		public void MainFormName()
		{
			Assert.AreEqual("MainForm", Form.Name);
		}
	}
}
