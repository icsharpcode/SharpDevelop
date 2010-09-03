// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
	public class GenerateBackgroundWorkerTestFixture
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
				PropertyDescriptor propertyDescriptor = descriptors.Find("Name", false);
				propertyDescriptor.SetValue(form, "MainForm");
				
				BackgroundWorker worker = (BackgroundWorker)host.CreateComponent(typeof(BackgroundWorker), "backgroundWorker1");
				descriptors = TypeDescriptor.GetProperties(worker);
				propertyDescriptor = descriptors.Find("WorkerReportsProgress", false);
				propertyDescriptor.SetValue(worker, true);
				
				DesignerSerializationManager serializationManager = new DesignerSerializationManager(host);
				using (serializationManager.CreateSession()) {
					PythonCodeDomSerializer serializer = new PythonCodeDomSerializer("    ");
					generatedPythonCode = serializer.GenerateInitializeComponentMethodBody(host, serializationManager, String.Empty, 1);
				}			
			}
		}
						
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "    self._backgroundWorker1 = System.ComponentModel.BackgroundWorker()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # backgroundWorker1\r\n" +
								"    # \r\n" +
								"    self._backgroundWorker1.WorkerReportsProgress = True\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(False)\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode, generatedPythonCode);
		}
	}
}
