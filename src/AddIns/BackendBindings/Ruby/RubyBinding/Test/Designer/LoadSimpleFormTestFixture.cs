// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
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
	public class LoadSimpleFormTestFixture : LoadFormTestFixtureBase
	{		
		public override string RubyCode {
			get {
				return "class MainForm < System::Windows::Forms::Form\r\n" +
							"    def InitializeComponent()\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # MainForm\r\n" +
							"        # \r\n" +
							"        self.ClientSize = System::Drawing::Size.new(300, 400)\r\n" +
							"        self.Name = \"MainForm\"\r\n" +
							"        self.ResumeLayout(false)\r\n" +
							"     end\r\n" +
							"end";
			}
		}
				
		public CreatedComponent FormComponent {
			get { return ComponentCreator.CreatedComponents[0]; }
		}
		
		[Test]
		public void MainFormCreated()
		{			
			Assert.IsNotNull(Form);
		}
		
		[Test]
		public void MainFormName()
		{
			Assert.AreEqual("MainForm", Form.Name);
		}
		
		[Test]
		public void OneComponentCreated()
		{
			Assert.AreEqual(1, ComponentCreator.CreatedComponents.Count);
		}
		
		[Test]
		public void ComponentName()
		{
			Assert.AreEqual("MainForm", FormComponent.Name);
		}
		
		[Test]
		public void ComponentType()
		{
			Assert.AreEqual("System.Windows.Forms.Form", FormComponent.TypeName);
		}
		
		[Test]
		public void FormClientSize()
		{
			Size size = new Size(300, 400);
			Assert.AreEqual(size, Form.ClientSize);
		}

		[Test]
		public void BaseClassTypeNameLookedUp()
		{
			Assert.AreEqual("System.Windows.Forms.Form", ComponentCreator.TypeNames[0]);
		}
		
		/// <summary>
		/// The System.Drawing.Size type name should have been looked up by the RubyFormWalker when
		/// parsing the InitializeComponent method. Note that this is the second type that is looked up.
		/// The first lookup is the base class type.
		/// </summary>
		[Test]
		public void TypeNameLookedUp()
		{
			Assert.AreEqual("System.Drawing.Size", ComponentCreator.TypeNames[1]);
		}

		[Test]
		public void OneObjectCreated()
		{
			Assert.AreEqual(1, ComponentCreator.CreatedInstances.Count);
		}
		
		[Test]
		public void InstanceType()
		{
			List<object> args = new List<object>();
			int width = 300;
			int height = 400;
			args.Add(width);
			args.Add(height);
			
			CreatedInstance expectedInstance = new CreatedInstance(typeof(Size), args, null, false);
			Assert.AreEqual(expectedInstance, ComponentCreator.CreatedInstances[0]);
		}		
	}
}
