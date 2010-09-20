// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the Form's AccessibleRole property can be loaded into the designer.
	/// </summary>
	[TestFixture]
	public class LoadAccessibleRoleTestFixture : LoadFormTestFixtureBase
	{		
		public override string RubyCode {
			get {
				return "class TestForm < System::Windows::Forms::Form\r\n" +
							"    def InitializeComponent()\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # TestForm\r\n" +
							"        # \r\n" +
							"        self.AccessibleRole = System::Windows::Forms::AccessibleRole.None\r\n" +
							"        self.Name = \"TestForm\"\r\n" +
							"        self.ResumeLayout(false)\r\n" +
							"    end\r\n" +
							"end";
			}
		}
								
		[Test]
		public void AccessibleRoleIsNone()
		{
			Assert.AreEqual(AccessibleRole.None, Form.AccessibleRole);
		}
	}
}
