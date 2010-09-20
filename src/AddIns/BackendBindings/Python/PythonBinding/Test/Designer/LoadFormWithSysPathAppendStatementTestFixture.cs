// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	/// <summary>
	/// Statements before the main form class are parsed by the PythonComponentWalker when 
	/// they should be ignored. This test fixture checks that only when the InitializeComponent method
	/// is found the statements are parsed.
	/// </summary>
	[TestFixture]
	public class LoadFormWithSysPathAppendStatementTestFixture : LoadFormTestFixtureBase
	{		
		public override string PythonCode {
			get {	
				return "import sys\r\n" +
							"sys.path.append(r'c:\\python\\lib')\r\n" + // Calls Walk(CallExpression)
							"a = System.Windows.Forms.TextBox()\r\n" + // Calls Walk(AssignmentStatement)
							"a.Load += Load\r\n" + // Calls Walk(AugmentedAssignStatement)
							"b\r\n" + // Calls Walk(NameExpression)
							"10\r\n" + // Calls Walk(ConstantExpression)
							"\r\n" +
							"class MainForm(System.Windows.Forms.Form):\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # MainForm\r\n" +
							"        # \r\n" +
							"        self.Name = \"MainForm\"\r\n" +
							"        self.ResumeLayout(False)\r\n";
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
		public void NoInstancesCreated()
		{
			Assert.AreEqual(0, ComponentCreator.CreatedInstances.Count);
		}
	}
}
