// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using IronPython.Compiler.Ast;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class LoadInheritedProtectedPanelFormTestFixture : LoadFormTestFixtureBase
	{		
		public override string PythonCode {
			get {
				base.ComponentCreator.AddType("PythonBinding.Tests.Designer.ProtectedPanelDerivedForm", typeof(ProtectedPanelDerivedForm));
				
				return "class MainForm(PythonBinding.Tests.Designer.ProtectedPanelDerivedForm):\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # panel1\r\n" +
							"        # \r\n" +
							"        self.panel1.Location = System.Drawing.Point(10, 15)\r\n" +
							"        self.panel1.Size = System.Drawing.Size(108, 120)\r\n" +
							"        # \r\n" +
							"        # form1\r\n" +
							"        # \r\n" +
							"        self.Location = System.Drawing.Point(10, 20)\r\n" +
							"        self.Name = \"form1\"\r\n" +
							"        self.Controls.Add(self._textBox1)\r\n" +
							"        self.ResumeLayout(False)\r\n";
			}
		}
		
		ProtectedPanelDerivedForm DerivedForm { 
			get { return Form as ProtectedPanelDerivedForm; }
		}
				
		[Test]
		public void PanelLocation()
		{
			Assert.AreEqual(new Point(10, 15), DerivedForm.PanelLocation);
		}

		[Test]
		public void PanelSize()
		{
			Assert.AreEqual(new Size(108, 120), DerivedForm.PanelSize);
		}
		
		[Test]
		public void GetProtectedPanelObject()
		{
			Assert.AreEqual(DerivedForm.GetPanel(), PythonControlFieldExpression.GetInheritedObject("panel1", DerivedForm));
		}
		
		[Test]
		public void GetProtectedPanelObjectIncorrectCase()
		{
			Assert.AreEqual(DerivedForm.GetPanel(), PythonControlFieldExpression.GetInheritedObject("PANEL1", DerivedForm));
		}
		
		[Test]
		public void GetInheritedObjectPassedNull()
		{
			Assert.IsNull(PythonControlFieldExpression.GetInheritedObject("panel1", null));
		}
		
		[Test]
		public void GetInheritedPanelObjectFromFieldExpression()
		{
			AssignmentStatement statement = PythonParserHelper.GetAssignmentStatement("self.panel1.Name = \"abc\"");
			PythonControlFieldExpression field = PythonControlFieldExpression.Create(statement.Left[0] as MemberExpression);
				
			Assert.AreEqual(DerivedForm.GetPanel(), field.GetObject(ComponentCreator));
		}
	}
}
