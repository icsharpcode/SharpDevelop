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
using System.Globalization;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class GenerateLocalImageResourceTestFixture
	{
		MockResourceWriter resourceWriter;
		MockComponentCreator componentCreator;
		string generatedPythonCode;
		MockResourceWriter resourceWriter2;
		MockComponentCreator componentCreator2;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			resourceWriter = new MockResourceWriter();
			componentCreator = new MockComponentCreator();
			componentCreator.SetResourceWriter(resourceWriter);

			resourceWriter2 = new MockResourceWriter();
			componentCreator2 = new MockComponentCreator();
			componentCreator2.SetResourceWriter(resourceWriter2);
			
			using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				IEventBindingService eventBindingService = new MockEventBindingService(host);
				Form form = (Form)host.RootComponent;
				form.ClientSize = new Size(200, 300);

				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
				
				// Add picture box
				PictureBox pictureBox = (PictureBox)host.CreateComponent(typeof(PictureBox), "pictureBox1");
				pictureBox.Location = new Point(0, 0);
				//pictureBox.Image = new Bitmap(10, 10);
				pictureBox.Size = new Size(100, 120);
				pictureBox.TabIndex = 0;
				form.Controls.Add(pictureBox);
				
				PythonControl pythonControl = new PythonControl("    ", componentCreator);
				generatedPythonCode = pythonControl.GenerateInitializeComponentMethod(form);
				
				// Check that calling the GenerateInitializeComponentMethodBody also generates a resource file.
				PythonControl pythonControl2 = new PythonControl("    ", componentCreator2);
				pythonControl2.GenerateInitializeComponentMethodBody(form, 0);
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "def InitializeComponent(self):\r\n" +
								//"    resources = System.Windows.Forms.ComponentModel(clr.GetType(MainForm))\r\n" +
								"    self._pictureBox1 = System.Windows.Forms.PictureBox()\r\n" +
								"    self._pictureBox1.BeginInit()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # pictureBox1\r\n" +
								"    # \r\n" +
								"    self._pictureBox1.Location = System.Drawing.Point(0, 0)\r\n" +
								//"    self._pictureBox1.Image = resources.GetObject(\"pictureBox1.Image\")\r\n" +
								"    self._pictureBox1.Name = \"pictureBox1\"\r\n" +
								"    self._pictureBox1.Size = System.Drawing.Size(100, 120)\r\n" +
								"    self._pictureBox1.TabIndex = 0\r\n" +
								"    self._pictureBox1.TabStop = False\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System.Drawing.Size(200, 300)\r\n" +
								"    self.Controls.Add(self._pictureBox1)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self._pictureBox1.EndInit()\r\n" +
								"    self.ResumeLayout(False)\r\n" +
								"    self.PerformLayout()\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode, generatedPythonCode);
		}
		
		[Test]
		public void ResourceWriterRetrievedFromComponentCreator()
		{
			Assert.IsTrue(componentCreator.GetResourceWriterCalled);
		}
		
		[Test]
		public void CultureInfoInvariantCulturePassedToGetResourceWriter()
		{
			Assert.AreEqual(CultureInfo.InvariantCulture, componentCreator.CultureInfoPassedToGetResourceWriter);
		}
		
		[Test]
		public void ResourceWriterIsDisposed()
		{
			Assert.IsTrue(resourceWriter.IsDisposed);
		}
		
		[Test]
		public void ResourceWriterRetrievedFromComponentCreator2()
		{
			Assert.IsTrue(componentCreator2.GetResourceWriterCalled);
		}
		
		[Test]
		public void CultureInfoInvariantCulturePassedToGetResourceWriter2()
		{
			Assert.AreEqual(CultureInfo.InvariantCulture, componentCreator2.CultureInfoPassedToGetResourceWriter);
		}
		
		[Test]
		public void ResourceWriter2IsDisposed()
		{
			Assert.IsTrue(resourceWriter2.IsDisposed);
		}
		
	}
}
