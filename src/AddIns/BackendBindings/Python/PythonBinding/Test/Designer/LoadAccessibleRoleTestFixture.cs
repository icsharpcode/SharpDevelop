// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the Form's AccessibleRole property can be loaded into the designer.
	/// </summary>
	[TestFixture]
	public class LoadAccessibleRoleTestFixture : LoadFormTestFixtureBase
	{		
		string pythonCode = "class TestForm(System.Windows.Forms.Form):\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # TestForm\r\n" +
							"        # \r\n" +
							"        self.AccessibleRole = System.Windows.Forms.AccessibleRole.None\r\n" +
							"        self.Name = \"TestForm\"\r\n" +
							"        self.ResumeLayout(False)\r\n";
		Form form;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			PythonFormWalker walker = new PythonFormWalker(this);
			form = walker.CreateForm(pythonCode);
		}

		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			form.Dispose();
		}
								
		[Test]
		public void AccessibleRoleIsNone()
		{
			Assert.AreEqual(AccessibleRole.None, form.AccessibleRole);
		}
	}
}
