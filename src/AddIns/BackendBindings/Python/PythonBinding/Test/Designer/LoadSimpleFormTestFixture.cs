// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
	public class LoadSimpleFormTestFixture : LoadFormTestFixtureBase
	{		
		string pythonCode = "class MainForm(System.Windows.Forms.Form):\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # MainForm\r\n" +
							"        # \r\n" +
							"        self.ClientSize = System.Drawing.Size(300, 400)\r\n" +
							"        self.Name = \"MainForm\"\r\n" +
							"        self.ResumeLayout(False)\r\n";
		Form form;
		CreatedComponent formComponent;
		string typeName;
		CreatedInstance instance;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			PythonFormWalker walker = new PythonFormWalker(this, new MockDesignerLoaderHost());
			form = walker.CreateForm(pythonCode);
			
			if (CreatedComponents.Count > 0) {
				formComponent = CreatedComponents[0];
			}
			if (TypeNames.Count > 0) {
				typeName = TypeNames[0];
			}
			if (CreatedInstances.Count > 0) {
				instance = CreatedInstances[0];
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
		
		[Test]
		public void FormClientSize()
		{
			Size size = new Size(300, 400);
			Assert.AreEqual(size, form.ClientSize);
		}
		
		/// <summary>
		/// The System.Drawing.Size type name should have been looked up by the PythonFormWalker when
		/// parsing the InitializeComponent method.
		/// </summary>
		[Test]
		public void TypeNameLookedUp()
		{
			Assert.AreEqual("System.Drawing.Size", typeName);
		}

		[Test]
		public void OneObjectCreated()
		{
			Assert.AreEqual(1, CreatedInstances.Count);
		}
		
		[Test]
		public void InstanceType()
		{
			List<object> args = new List<object>();
			int width = 300;
			int height = 400;
			args.Add(width);
			args.Add(height);
			
			CreatedInstance expectedInstance = new CreatedInstance(typeof(Size), args, "ClientSize", false);
			Assert.AreEqual(expectedInstance, instance);
		}		
	}
}
