// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class GenerateTimerTestFixture
	{
		string generatedPythonCode;
		bool hasNonVisualChildComponents;
		bool hasIContainerConstructor;		
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
				
				Timer timer = (Timer)host.CreateComponent(typeof(Timer), "timer1");
				descriptors = TypeDescriptor.GetProperties(timer);
				namePropertyDescriptor = descriptors.Find("Interval", false);
				namePropertyDescriptor.SetValue(timer, 1000);
				
				string indentString = "    ";
				PythonDesignerRootComponent designerRootComponent = new PythonDesignerRootComponent(form);
				hasNonVisualChildComponents = designerRootComponent.HasNonVisualChildComponents();
				
				PythonDesignerComponent component = new PythonDesignerComponent(timer);
				hasIContainerConstructor = component.HasIContainerConstructor();
			
				PythonControl pythonControl = new PythonControl(indentString);
				generatedPythonCode = pythonControl.GenerateInitializeComponentMethod(form);
			}
		}
		
		[Test]
		public void HasNonVisualChildComponents()
		{
			Assert.IsTrue(hasNonVisualChildComponents);
		}
				
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "def InitializeComponent(self):\r\n" +
								"    self._components = System.ComponentModel.Container()\r\n" +
								"    self._timer1 = System.Windows.Forms.Timer(self._components)\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # timer1\r\n" +
								"    # \r\n" +
								"    self._timer1.Interval = 1000\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(False)\r\n" +
								"    self.PerformLayout()\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode, generatedPythonCode);
		}
		
		[Test]
		public void HasIContainerConstructor()
		{
			Assert.IsTrue(hasIContainerConstructor);
		}		
	}
}
