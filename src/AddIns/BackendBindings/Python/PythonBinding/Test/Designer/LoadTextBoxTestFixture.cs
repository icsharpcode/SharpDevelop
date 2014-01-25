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
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
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
							"        self._textBox1.Location = System.Drawing.Point(108, 120)\r\n" +
							"        # \r\n" +
							"        # form1\r\n" +
							"        # \r\n" +
							"        self.Location = System.Drawing.Point(10, 20)\r\n" +
							"        self.Name = \"form1\"\r\n" +
							"        self.Controls.Add(self._textBox1)\r\n" +
							"        self.ResumeLayout(False)\r\n";
			}
		}
		
		public TextBox TextBox { 
			get { return Form.Controls[0] as TextBox; }
		}
		
		[Test]
		public void AddedComponentsContainsTextBox()
		{
			CreatedInstance instance = ComponentCreator.GetCreatedInstance(typeof(TextBox));
			
			AddedComponent component = new AddedComponent(instance.Object as IComponent, "textBox1");
			Assert.Contains(component, ComponentCreator.AddedComponents);
		}

		[Test]
		public void TextBoxInstanceCreated()
		{
			CreatedInstance instance = new CreatedInstance(typeof(TextBox), new List<object>(), "textBox1", false);
			Assert.Contains(instance, ComponentCreator.CreatedInstances);
		}

		[Test]
		public void TextBoxAddedToForm()
		{
			Assert.IsNotNull(TextBox);
		}
		
		[Test]
		public void TextBoxObjectMatchesObjectAddedToComponentCreator()
		{
			CreatedInstance instance = ComponentCreator.GetCreatedInstance(typeof(TextBox));
			Assert.AreSame(TextBox, instance.Object as TextBox);			
		}
		
		[Test]
		public void TextBoxName()
		{
			Assert.AreEqual("textBoxName", TextBox.Name);
		}
		
		[Test]
		public void TextBoxLocation()
		{
			Assert.AreEqual(new Point(108, 120), TextBox.Location);
		}
		
		[Test]
		public void CreatedInstancesDoesNotIncludeLocation()
		{
			Assert.IsNull(ComponentCreator.GetCreatedInstance("Location"));
		}
		
		[Test]
		public void FormLocation()
		{
			Assert.AreEqual(new Point(10, 20), Form.Location);
		}
	}
}
