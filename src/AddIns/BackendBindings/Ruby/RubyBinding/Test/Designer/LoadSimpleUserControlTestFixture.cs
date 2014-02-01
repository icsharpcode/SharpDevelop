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
	public class LoadSimpleUserControlTestFixture
	{	
		MockComponentCreator componentCreator = new MockComponentCreator();
		UserControl userControl;
		
		public string RubyCode {
			get {
				return "class MainForm < System::Windows::Forms::UserControl\r\n" +
							"    def InitializeComponent()\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # userControl1\r\n" +
							"        # \r\n" +
							"        self.ClientSize = System::Drawing::Size.new(300, 400)\r\n" +
							"        self.Name = \"userControl1\"\r\n" +
							"        self.ResumeLayout(false)\r\n" +
							"    end\r\n" +
							"end";
			}
		}

		[TestFixtureSetUp]
		public void SetUpFixture()
		{			
			RubyComponentWalker walker = new RubyComponentWalker(componentCreator);
			userControl = walker.CreateComponent(RubyCode) as UserControl;
		}

		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			userControl.Dispose();
		}
		
		[Test]
		public void UserControlCreated()
		{			
			Assert.IsNotNull(userControl);
		}
		
		[Test]
		public void UserControlName()
		{
			Assert.AreEqual("userControl1", userControl.Name);
		}
		
		[Test]
		public void UserControlClientSize()
		{
			Size size = new Size(300, 400);
			Assert.AreEqual(size, userControl.ClientSize);
		}
	}
}
