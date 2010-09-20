// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using IronRuby.Compiler.Ast;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class LoadInheritedProtectedPanelFormTestFixture : LoadFormTestFixtureBase
	{		
		public override string RubyCode {
			get {
				base.ComponentCreator.AddType("RubyBinding.Tests.Designer.ProtectedPanelDerivedForm", typeof(ProtectedPanelDerivedForm));
				
				return
					"class MainForm < RubyBinding::Tests::Designer::ProtectedPanelDerivedForm\r\n" +
					"    def InitializeComponent()\r\n" +
					"        self.SuspendLayout()\r\n" +
					"        # \r\n" +
					"        # panel1\r\n" +
					"        # \r\n" +
					"        self.panel1.Location = System::Drawing::Point.new(10, 15)\r\n" +
					"        self.panel1.Size = System::Drawing::Size.new(108, 120)\r\n" +
					"        # \r\n" +
					"        # form1\r\n" +
					"        # \r\n" +
					"        self.Location = System::Drawing::Point.new(10, 20)\r\n" +
					"        self.Name = \"form1\"\r\n" +
					"        self.Controls.Add(@textBox1)\r\n" +
					"        self.ResumeLayout(False)\r\n" +
					"    end\r\n" +
					"end";
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
			Assert.AreEqual(DerivedForm.GetPanel(), RubyControlFieldExpression.GetInheritedObject("panel1", DerivedForm));
		}
		
		[Test]
		public void GetProtectedPanelObjectIncorrectCase()
		{
			Assert.AreEqual(DerivedForm.GetPanel(), RubyControlFieldExpression.GetInheritedObject("PANEL1", DerivedForm));
		}
		
		[Test]
		public void GetInheritedObjectPassedNull()
		{
			Assert.IsNull(RubyControlFieldExpression.GetInheritedObject("panel1", null));
		}
		
		[Test]
		public void GetInheritedPanelObjectFromFieldExpression()
		{
			SimpleAssignmentExpression assignment = RubyParserHelper.GetSimpleAssignmentExpression("self.panel1.Name = \"abc\"");
			RubyControlFieldExpression field = RubyControlFieldExpression.Create(assignment.Left as AttributeAccess);
				
			Assert.AreEqual(DerivedForm.GetPanel(), field.GetObject(ComponentCreator));
		}
	}
}
