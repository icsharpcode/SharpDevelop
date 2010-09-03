// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class GenerateContextMenuStripTestFixture
	{
		string generatedPythonCode;
		Size menuStripSize;
		
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

				// Add timer. This checks that the components Container is only created once in the
				// generated code.
				Timer timer = (Timer)host.CreateComponent(typeof(Timer), "timer1");
				
				// Add menu strip.
				ContextMenuStrip menuStrip = (ContextMenuStrip)host.CreateComponent(typeof(ContextMenuStrip), "contextMenuStrip1");
				
				// Set the context menu strip OwnerItem to simulate leaving the context menu
				// open in the designer before generating the source code. We do not want the
				// OwnerItem to be serialized.
				menuStrip.OwnerItem = new DerivedToolStripMenuItem();
				menuStrip.RightToLeft = RightToLeft.No;
				menuStripSize = menuStrip.Size;
				
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
			string expectedCode = "    self._components = System.ComponentModel.Container()\r\n" +
								"    self._timer1 = System.Windows.Forms.Timer(self._components)\r\n" +
								"    self._contextMenuStrip1 = System.Windows.Forms.ContextMenuStrip(self._components)\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # contextMenuStrip1\r\n" +
								"    # \r\n" +
								"    self._contextMenuStrip1.Name = \"contextMenuStrip1\"\r\n" +
								"    self._contextMenuStrip1.Size = " + PythonPropertyValueAssignment.ToString(menuStripSize) + "\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System.Drawing.Size(200, 300)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(False)\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode, generatedPythonCode);
		}
	}
}
