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
	/// <summary>
	/// When a text box is not added to the form's Control collection in InitializeComponent then:
	/// 
	/// 1) Text box should not be added to the form's Control collection when the form is created.
	/// 2) Text box should be registered with the designer via the IComponentCreator.Add method.
	/// 3) Text box should be created via the IComponentCreator.CreateInstance method.
	/// </summary>
	[TestFixture]
	public class TextBoxNotAddedToFormTestFixture : LoadFormTestFixtureBase
	{		
		public override string RubyCode {
			get {
				return
					"class MainForm < System::Windows::Forms::Form\r\n" +
					"    def InitializeComponent()\r\n" +
					"        @textBox1 = System::Windows::Forms::TextBox.new()\r\n" +
					"        self.SuspendLayout()\r\n" +
					"        # \r\n" +
					"        # textBox1\r\n" +
					"        # \r\n" +
					"        @textBox1.Name = \"textBox1\"\r\n" +
					"        # \r\n" +
					"        # form1\r\n" +
					"        # \r\n" +
					"        self.ResumeLayout(false)\r\n" +
					"    end\r\n" +
					"end";
			}
		}
		
		[Test]
		public void AddedComponentsContainsTextBox()
		{
			CreatedInstance instance = ComponentCreator.GetCreatedInstance(typeof(TextBox));
			
			AddedComponent c = new AddedComponent(instance.Object as IComponent, "textBox1");
			Assert.Contains(c, ComponentCreator.AddedComponents);
		}
		
		[Test]
		public void TextBoxIsNotAddedToForm()
		{
			Assert.AreEqual(0, Form.Controls.Count);
		}
	}
}
