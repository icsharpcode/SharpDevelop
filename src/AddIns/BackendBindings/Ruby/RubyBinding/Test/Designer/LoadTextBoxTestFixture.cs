// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class LoadTextBoxTestFixture : LoadFormTestFixtureBase
	{
		public override string RubyCode {
			get {
				return "class MainForm < System::Windows::Forms::Form\r\n" +
							"    def InitializeComponent()\r\n" +
							"        @textBox1 = System::Windows::Forms::TextBox.new()\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # textBox1\r\n" +
							"        # \r\n" +
							"        @textBox1.Name = \"textBoxName\"\r\n" +
							"        @textBox1.Location = System::Drawing::Point.new(108, 120)\r\n" +
							"        # \r\n" +
							"        # form1\r\n" +
							"        # \r\n" +
							"        self.Location = System::Drawing::Point.new(10, 20)\r\n" +
							"        self.Name = \"form1\"\r\n" +
							"        self.Controls.Add(@textBox1)\r\n" +
							"        self.ResumeLayout(false)\r\n" +
							"    end\r\n" +
							"end";
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
