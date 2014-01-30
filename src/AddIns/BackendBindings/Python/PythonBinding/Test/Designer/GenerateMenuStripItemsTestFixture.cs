// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
	public class GenerateMenuStripItemsFormTestFixture
	{
		string generatedPythonCode;
		Size fileMenuItemSize;
		Size openMenuItemSize;
		Size exitMenuItemSize;
		Size editMenuItemSize;
		
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
				
				// Add menu strip.
				MenuStrip menuStrip = (MenuStrip)host.CreateComponent(typeof(MenuStrip), "menuStrip1");
				menuStrip.Text = "menuStrip1";
				menuStrip.TabIndex = 0;
				menuStrip.Location = new Point(0, 0);
				menuStrip.Size = new System.Drawing.Size(200, 24);
				
				// Add menu strip items.
				ToolStripMenuItem fileMenuItem = (ToolStripMenuItem)host.CreateComponent(typeof(ToolStripMenuItem), "fileToolStripMenuItem");
				fileMenuItem.Text = "&File";
				
				ToolStripMenuItem openMenuItem = (ToolStripMenuItem)host.CreateComponent(typeof(ToolStripMenuItem), "openToolStripMenuItem");
				openMenuItem.Text = "&Open";
				
				ToolStripMenuItem exitMenuItem = (ToolStripMenuItem)host.CreateComponent(typeof(ToolStripMenuItem), "exitToolStripMenuItem");
				exitMenuItem.Text = "E&xit";
				fileMenuItem.DropDownItems.Add(openMenuItem);
				fileMenuItem.DropDownItems.Add(exitMenuItem);
				
				// Add non-sited component.
				fileMenuItem.DropDownItems.Add(new ToolStripMenuItem());
				
				menuStrip.Items.Add(fileMenuItem);
								
				ToolStripMenuItem editMenuItem = (ToolStripMenuItem)host.CreateComponent(typeof(ToolStripMenuItem), "editToolStripMenuItem");
				editMenuItem.Text = "&Edit";
				menuStrip.Items.Add(editMenuItem);

				form.Controls.Add(menuStrip);
				
				fileMenuItemSize = fileMenuItem.Size;
				openMenuItemSize = openMenuItem.Size;
				exitMenuItemSize = exitMenuItem.Size;
				editMenuItemSize = editMenuItem.Size;

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
			string expectedCode = "    self._menuStrip1 = System.Windows.Forms.MenuStrip()\r\n" +
								"    self._fileToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()\r\n" +
								"    self._openToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()\r\n" +
								"    self._exitToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()\r\n" +
								"    self._editToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()\r\n" +
								"    self._menuStrip1.SuspendLayout()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # menuStrip1\r\n" +
								"    # \r\n" +
								"    self._menuStrip1.Items.AddRange(System.Array[System.Windows.Forms.ToolStripItem](\r\n" +
								"        [self._fileToolStripMenuItem,\r\n" +
								"        self._editToolStripMenuItem]))\r\n" +
								"    self._menuStrip1.Location = System.Drawing.Point(0, 0)\r\n" +
								"    self._menuStrip1.Name = \"menuStrip1\"\r\n" +
								"    self._menuStrip1.Size = System.Drawing.Size(200, 24)\r\n" +
								"    self._menuStrip1.TabIndex = 0\r\n" +
								"    self._menuStrip1.Text = \"menuStrip1\"\r\n" +
								"    # \r\n" +
								"    # fileToolStripMenuItem\r\n" +
								"    # \r\n" +
								"    self._fileToolStripMenuItem.DropDownItems.AddRange(System.Array[System.Windows.Forms.ToolStripItem](\r\n" +
								"        [self._openToolStripMenuItem,\r\n" +
								"        self._exitToolStripMenuItem]))\r\n" +
								"    self._fileToolStripMenuItem.Name = \"fileToolStripMenuItem\"\r\n" +
								"    self._fileToolStripMenuItem.Size = " + SizeToString(fileMenuItemSize) + "\r\n" +
								"    self._fileToolStripMenuItem.Text = \"&File\"\r\n" +
								"    # \r\n" +
								"    # openToolStripMenuItem\r\n" +
								"    # \r\n" +
								"    self._openToolStripMenuItem.Name = \"openToolStripMenuItem\"\r\n" +
								"    self._openToolStripMenuItem.Size = " + SizeToString(openMenuItemSize) + "\r\n" +
								"    self._openToolStripMenuItem.Text = \"&Open\"\r\n" +
								"    # \r\n" +
								"    # exitToolStripMenuItem\r\n" +
								"    # \r\n" +
								"    self._exitToolStripMenuItem.Name = \"exitToolStripMenuItem\"\r\n" +
								"    self._exitToolStripMenuItem.Size = " + SizeToString(exitMenuItemSize) + "\r\n" +
								"    self._exitToolStripMenuItem.Text = \"E&xit\"\r\n" +
								"    # \r\n" +
								"    # editToolStripMenuItem\r\n" +
								"    # \r\n" +
								"    self._editToolStripMenuItem.Name = \"editToolStripMenuItem\"\r\n" +
								"    self._editToolStripMenuItem.Size = " + SizeToString(editMenuItemSize) + "\r\n" +
								"    self._editToolStripMenuItem.Text = \"&Edit\"\r\n" +				
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System.Drawing.Size(200, 300)\r\n" +
								"    self.Controls.Add(self._menuStrip1)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self._menuStrip1.ResumeLayout(False)\r\n" +
								"    self._menuStrip1.PerformLayout()\r\n" +
								"    self.ResumeLayout(False)\r\n" +
								"    self.PerformLayout()\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode, generatedPythonCode);
		}
		
		string SizeToString(Size size)
		{
			return PythonPropertyValueAssignment.ToString(size);
		}		
	}
}
