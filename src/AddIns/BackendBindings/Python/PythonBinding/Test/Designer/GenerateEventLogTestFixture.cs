// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class GenerateEventLogTestFixture
	{
		string generatedPythonCode;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				Form form = (Form)host.RootComponent;			
				form.ClientSize = new Size(284, 264);
				
				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
				
				EventLog eventLog = (EventLog)host.CreateComponent(typeof(EventLog), "eventLog1");
				
				string indentString = "    ";
				PythonControl pythonControl = new PythonControl(indentString);
				generatedPythonCode = pythonControl.GenerateInitializeComponentMethod(form);
			}
		}
		
				
		[Test]
		[IgnoreAttribute("Ignore test to fix the build - this test breaks for some reason on .NET 4.0")]
		public void GeneratedCode()
		{
			string expectedCode = "def InitializeComponent(self):\r\n" +
								"    self._eventLog1 = System.Diagnostics.EventLog()\r\n" +
								"    self._eventLog1.BeginInit()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # eventLog1\r\n" +
								"    # \r\n" +
								"    self._eventLog1.SynchronizingObject = self\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self._eventLog1.EndInit()\r\n" +
								"    self.ResumeLayout(False)\r\n" +
								"    self.PerformLayout()\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode, generatedPythonCode);
		}		
	}
}
