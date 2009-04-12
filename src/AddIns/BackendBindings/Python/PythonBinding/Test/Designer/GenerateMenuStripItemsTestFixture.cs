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
	public class GenerateMenuStripItemsFormTestFixture
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
				
				// Add menu strip.
				MenuStrip menuStrip = (MenuStrip)host.CreateComponent(typeof(MenuStrip), "menuStrip1");
				menuStrip.Text = "menuStrip1";
				menuStrip.TabIndex = 0;
				menuStrip.Location = new Point(0, 0);
				menuStrip.Size = new System.Drawing.Size(200, 24);
				
				// Add menu strip items.
				ToolStripMenuItem fileMenuItem = (ToolStripMenuItem)host.CreateComponent(typeof(ToolStripMenuItem), "fileToolStripMenuItem");
				fileMenuItem.Size = new Size(37, 20);
				fileMenuItem.Text = "&File";
				
				ToolStripMenuItem openMenuItem = (ToolStripMenuItem)host.CreateComponent(typeof(ToolStripMenuItem), "openToolStripMenuItem");
				openMenuItem.Size = new Size(37, 20);
				openMenuItem.Text = "&Open";
				
				ToolStripMenuItem exitMenuItem = (ToolStripMenuItem)host.CreateComponent(typeof(ToolStripMenuItem), "exitToolStripMenuItem");
				exitMenuItem.Size = new Size(37, 20);
				exitMenuItem.Text = "E&xit";
				fileMenuItem.DropDownItems.Add(openMenuItem);
				fileMenuItem.DropDownItems.Add(exitMenuItem);
				
				// Add non-sited component.
				fileMenuItem.DropDownItems.Add(new ToolStripMenuItem());
				
				menuStrip.Items.Add(fileMenuItem);
								
				ToolStripMenuItem editMenuItem = (ToolStripMenuItem)host.CreateComponent(typeof(ToolStripMenuItem), "editToolStripMenuItem");
				editMenuItem.Size = new Size(39, 20);
				editMenuItem.Text = "&Edit";
				menuStrip.Items.Add(editMenuItem);

				form.Controls.Add(menuStrip);

				PythonForm pythonForm = new PythonForm("    ");
				generatedPythonCode = pythonForm.GenerateInitializeComponentMethod(form);
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "def InitializeComponent(self):\r\n" +
								"    self._menuStrip1 = System.Windows.Forms.MenuStrip()\r\n" +
								"    self._fileToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()\r\n" +
								"    self._openToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()\r\n" +
								"    self._exitToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()\r\n" +
								"    self._editToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem()\r\n" +
								"    self._menuStrip1.SuspendLayout()\r\n" +
								"    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # menuStrip1\r\n" +
								"    # \r\n" +
								"    self._menuStrip1.Location = System.Drawing.Point(0, 0)\r\n" +
								"    self._menuStrip1.Name = \"menuStrip1\"\r\n" +
								"    self._menuStrip1.Size = System.Drawing.Size(200, 24)\r\n" +
								"    self._menuStrip1.TabIndex = 0\r\n" +
								"    self._menuStrip1.Text = \"menuStrip1\"\r\n" +
								"    self._menuStrip1.Items.AddRange(System.Array[System.Windows.Forms.ToolStripItem](\r\n" +
								"        [self._fileToolStripMenuItem,\r\n" +
								"        self._editToolStripMenuItem]))\r\n" +
								"    # \r\n" +
								"    # fileToolStripMenuItem\r\n" +
								"    # \r\n" +
								"    self._fileToolStripMenuItem.Name = \"fileToolStripMenuItem\"\r\n" +
								"    self._fileToolStripMenuItem.Size = System.Drawing.Size(37, 20)\r\n" +
								"    self._fileToolStripMenuItem.Text = \"&File\"\r\n" +
								"    self._fileToolStripMenuItem.DropDownItems.AddRange(System.Array[System.Windows.Forms.ToolStripItem](\r\n" +
								"        [self._openToolStripMenuItem,\r\n" +
								"        self._exitToolStripMenuItem]))\r\n" +
								"    # \r\n" +
								"    # openToolStripMenuItem\r\n" +
								"    # \r\n" +
								"    self._openToolStripMenuItem.Name = \"openToolStripMenuItem\"\r\n" +
								"    self._openToolStripMenuItem.Size = System.Drawing.Size(37, 20)\r\n" +
								"    self._openToolStripMenuItem.Text = \"&Open\"\r\n" +
								"    # \r\n" +
								"    # exitToolStripMenuItem\r\n" +
								"    # \r\n" +
								"    self._exitToolStripMenuItem.Name = \"exitToolStripMenuItem\"\r\n" +
								"    self._exitToolStripMenuItem.Size = System.Drawing.Size(37, 20)\r\n" +
								"    self._exitToolStripMenuItem.Text = \"E&xit\"\r\n" +
								"    # \r\n" +
								"    # editToolStripMenuItem\r\n" +
								"    # \r\n" +
								"    self._editToolStripMenuItem.Name = \"editToolStripMenuItem\"\r\n" +
								"    self._editToolStripMenuItem.Size = System.Drawing.Size(39, 20)\r\n" +
								"    self._editToolStripMenuItem.Text = \"&Edit\"\r\n" +				
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System.Drawing.Size(200, 300)\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.Controls.Add(self._menuStrip1)\r\n" +
								"    self._menuStrip1.ResumeLayout(False)\r\n" +
								"    self._menuStrip1.PerformLayout()\r\n" +
								"    self.ResumeLayout(False)\r\n" +
								"    self.PerformLayout()\r\n";
			
			Assert.AreEqual(expectedCode, generatedPythonCode);
		}
	}
}
