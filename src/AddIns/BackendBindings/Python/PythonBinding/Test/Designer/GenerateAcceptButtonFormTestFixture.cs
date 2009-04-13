// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class GenerateAcceptButtonFormTestFixture
	{
		string generatedPythonCode;
		IComponent[] formChildComponents;
		IComponent[] buttonChildComponents;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				IEventBindingService eventBindingService = new MockEventBindingService(host);
				Form form = (Form)host.RootComponent;
				form.ClientSize = new Size(200, 300);
				
				Button button = (Button)host.CreateComponent(typeof(Button), "button1");
				button.Location = new Point(0, 0);
				button.Size = new Size(10, 10);
				button.Text = "button1";
				button.UseCompatibleTextRendering = false;
				form.Controls.Add(button);

				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor acceptButtonPropertyDescriptor = descriptors.Find("AcceptButton", false);
				acceptButtonPropertyDescriptor.SetValue(form, button);

				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
				
				PythonForm pythonForm = new PythonForm("    ");
				generatedPythonCode = pythonForm.GenerateInitializeComponentMethod(form);
				
				formChildComponents = PythonForm.GetChildComponents(form);
				buttonChildComponents = PythonForm.GetChildComponents(button);
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "def InitializeComponent(self):\r\n" +
								"    self._button1 = System.Windows.Forms.Button()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # button1\r\n" +
								"    # \r\n" +
								"    self._button1.Location = System.Drawing.Point(0, 0)\r\n" +
								"    self._button1.Name = \"button1\"\r\n" +
								"    self._button1.Size = System.Drawing.Size(10, 10)\r\n" +
								"    self._button1.TabIndex = 0\r\n" +
								"    self._button1.Text = \"button1\"\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.AcceptButton = self._button1\r\n" +
								"    self.ClientSize = System.Drawing.Size(200, 300)\r\n" +
								"    self.Controls.Add(self._button1)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(False)\r\n" +
								"    self.PerformLayout()\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode);
		}
		
		[Test]
		public void FormHasOneChildComponent()
		{
			Assert.AreEqual(1, formChildComponents.Length);
		}
		
		[Test]
		public void FormChildComponentIsButton()
		{
			Assert.IsInstanceOfType(typeof(Button), formChildComponents[0]);
		}
		
		[Test]
		public void ButtonHasNoChildComponents()
		{
			Assert.AreEqual(0, buttonChildComponents.Length);
		}
	}
}
