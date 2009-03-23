// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class LoadTextBoxTestFixture : LoadFormTestFixtureBase
	{		
		public override string PythonCode {
			get {
				return "class MainForm(System.Windows.Forms.Form):\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        self._textBox1 = System.Windows.Forms.TextBox()\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # textBox1\r\n" +
							"        # \r\n" +
							"        self._textBox1.Name = \"textBoxName\"\r\n" +
							"        # \r\n" +
							"        # form1\r\n" +
							"        # \r\n" +
							"        self.Name = \"form1\"\r\n" +
							"        self.Controls.Add(self._textBox1)\r\n" +
							"        self.ResumeLayout(False)\r\n";
			}
		}
		
		public TextBox TextBox { 
			get { return Form.Controls[0] as TextBox; }
		}

		[Test]
		public void TextBoxInstanceCreated()
		{
			CreatedInstance instance = new CreatedInstance(typeof(TextBox), new List<object>(), "_textBox1", false);
			Assert.Contains(instance, CreatedInstances);
		}
		
		[Test]
		public void AddedComponentsContainsTextBox()
		{
			CreatedInstance instance = GetCreatedInstance(typeof(TextBox));
			
			AddedComponent component = new AddedComponent(instance.Object as IComponent, "textBox1");
			Assert.Contains(component, AddedComponents);
		}
		
		[Test]
		public void TextBoxAddedToForm()
		{
			Assert.IsNotNull(TextBox);
		}
		
		[Test]
		public void TextBoxObjectMatchesObjectAddedToComponentCreator()
		{
			CreatedInstance instance = GetCreatedInstance(typeof(TextBox));
			Assert.AreSame(TextBox, instance.Object as TextBox);			
		}
		
		[Test]
		public void TextBoxName()
		{
			Assert.AreEqual("textBoxName", TextBox.Name);
		}
	}
}
