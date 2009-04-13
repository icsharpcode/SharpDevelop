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
	/// <summary>
	/// Tests that an event is wired to its event handler after the user specifies an event handler
	/// method in the property grid.
	/// </summary>
	[TestFixture]
	public class GenerateEventHandlerFormTestFixture
	{
		string generatedPythonCode;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				IEventBindingService eventBindingService = new MockEventBindingService(host);
				Form form = (Form)host.RootComponent;
				form.ClientSize = new Size(200, 300);

				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
				
				// Simulate giving a name to the Load event handler in the property grid.
				EventDescriptorCollection events = TypeDescriptor.GetEvents(form);
				EventDescriptor loadEvent = events.Find("Load", false);
				PropertyDescriptor loadEventProperty = eventBindingService.GetEventProperty(loadEvent);
				loadEventProperty.SetValue(form, "MainFormLoad");
				
				// Add a second event handler method to check that the events are sorted alphabetically
				// before the InitializeComponent method is generated.
				EventDescriptor closedEvent = events.Find("FormClosed", false);
				PropertyDescriptor closedEventProperty = eventBindingService.GetEventProperty(closedEvent);
				closedEventProperty.SetValue(form, "MainFormClosed");
				
				PythonControl pythonForm = new PythonControl("    ");
				generatedPythonCode = pythonForm.GenerateInitializeComponentMethod(form);
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "def InitializeComponent(self):\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System.Drawing.Size(200, 300)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.FormClosed += self.MainFormClosed\r\n" +
								"    self.Load += self.MainFormLoad\r\n" +
								"    self.ResumeLayout(False)\r\n" +
								"    self.PerformLayout()\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode);
		}
	}
}
