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
	public class LoadUserControlWithDoublePropertyTestFixture
	{	
		MockComponentCreator componentCreator = new MockComponentCreator();
		DoublePropertyUserControl userControl;
		Form form;
		
		public string RubyCode {
			get {
				Type type = typeof(DoublePropertyUserControl);
				componentCreator.AddType(type.FullName, type);
				return
					"class MainForm < System::Windows::Forms::Form\r\n" +
					"    def InitializeComponent()\r\n" +
					"        @userControl = ICSharpCode::Scripting::Tests::Utils::DoublePropertyUserControl.new()\r\n" +
					"        self.SuspendLayout()\r\n" +
					"        # \r\n" +
					"        # userControl1\r\n" +
					"        # \r\n" +
					"        @userControl.DoubleValue = 0\r\n" +
					"        # \r\n" +
					"        # MainForm\r\n" +
					"        # \r\n" +
					"        self.ClientSize = System::Drawing::Size.new(300, 400)\r\n" +
					"        self.Controls.Add(@userControl)\r\n" +
					"        self.Name = \"MainForm\"\r\n" +
					"        self.ResumeLayout(false)\r\n" +
					"    end\r\n" +
					"end";
			}
		}

		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			RubyComponentWalker walker = new RubyComponentWalker(componentCreator);
			form = walker.CreateComponent(RubyCode) as Form;
			userControl = form.Controls[0] as DoublePropertyUserControl;
		}

		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			form.Dispose();
		}
		
		[Test]
		public void UserControlDoubleProperty()
		{
			Assert.AreEqual(0, userControl.DoubleValue);
		}		
	}
}
