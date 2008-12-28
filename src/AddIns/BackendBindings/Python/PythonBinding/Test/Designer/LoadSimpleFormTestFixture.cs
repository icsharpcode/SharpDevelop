// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class LoadSimpleFormTestFixture : LoadFormTestFixtureBase
	{		
		string pythonCode = "def InitializeComponent(self):\r\n" +
							"    self.SuspendLayout()\r\n" +
							"    # \r\n" +
							"    # MainForm\r\n" +
							"    # \r\n" +
							"    self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
							"    self.Name = \"MainForm\"\r\n" +
							"    self.ResumeLayout(False)\r\n";
		Form form;
		CreatedComponent formComponent;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			PythonFormVisitor visitor = new PythonFormVisitor();
			form = visitor.CreateForm(pythonCode, this);
			
			if (CreatedComponents.Count > 0) {
				formComponent = CreatedComponents[0];
			}
		}

		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			form.Dispose();
		}
		
		[Test]
		public void MainFormCreated()
		{			
			Assert.IsNotNull(form);
		}
		
		[Test]
		public void MainFormName()
		{
			Assert.AreEqual("MainForm", form.Name);
		}
		
		[Test]
		public void OneComponentCreated()
		{
			Assert.AreEqual(1, CreatedComponents.Count);
		}
		
		[Test]
		public void ComponentName()
		{
			Assert.AreEqual("MainForm", formComponent.Name);
		}
		
		[Test]
		public void ComponentType()
		{
			Assert.AreEqual("System.Windows.Forms.Form", formComponent.TypeName);
		}		
	}
}
