// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
