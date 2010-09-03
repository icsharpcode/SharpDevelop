// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the designer load handles the Binding Navigator having separator toolstrips with the same Name
	/// property text "bindingNavigatorSeparator"
	/// </summary>
	[TestFixture]
	public class LoadBindingNavigatorTestFixture : LoadFormTestFixtureBase
	{
		public override string PythonCode {
			get {
				return
					"class TestForm(System.Windows.Forms.Form):\r\n" +
					"    def InitializeComponent(self):\r\n" +
					"        self._components = System.ComponentModel.Container()\r\n" +
					"        resources = System.Resources.ResourceManager(\"PyWin.TestForm\", System.Reflection.Assembly.GetEntryAssembly())\r\n" +
					"        self._bindingNavigator1 = System.Windows.Forms.BindingNavigator(self._components)\r\n" +
					"        self._bindingNavigatorMoveFirstItem = System.Windows.Forms.ToolStripButton()\r\n" +
					"        self._bindingNavigatorMovePreviousItem = System.Windows.Forms.ToolStripButton()\r\n" +
					"        self._bindingNavigatorSeparator = System.Windows.Forms.ToolStripSeparator()\r\n" +
					"        self._bindingNavigatorPositionItem = System.Windows.Forms.ToolStripTextBox()\r\n" +
					"        self._bindingNavigatorCountItem = System.Windows.Forms.ToolStripLabel()\r\n" +
					"        self._bindingNavigatorSeparator1 = System.Windows.Forms.ToolStripSeparator()\r\n" +
					"        self._bindingNavigatorMoveNextItem = System.Windows.Forms.ToolStripButton()\r\n" +
					"        self._bindingNavigatorMoveLastItem = System.Windows.Forms.ToolStripButton()\r\n" +
					"        self._bindingNavigatorSeparator2 = System.Windows.Forms.ToolStripSeparator()\r\n" +
					"        self._bindingNavigatorAddNewItem = System.Windows.Forms.ToolStripButton()\r\n" +
					"        self._bindingNavigatorDeleteItem = System.Windows.Forms.ToolStripButton()\r\n" +
					"        self._bindingNavigator1.BeginInit()\r\n" +
					"        self._bindingNavigator1.SuspendLayout()\r\n" +
					"        self.SuspendLayout()\r\n" +
					"        # \r\n" +
					"        # bindingNavigator1\r\n" +
					"        # \r\n" +
					"        self._bindingNavigator1.AddNewItem = self._bindingNavigatorAddNewItem\r\n" +
					"        self._bindingNavigator1.CountItem = self._bindingNavigatorCountItem\r\n" +
					"        self._bindingNavigator1.DeleteItem = self._bindingNavigatorDeleteItem\r\n" +
					"        self._bindingNavigator1.Items.AddRange(System.Array[System.Windows.Forms.ToolStripItem](\r\n" +
					"        	[self._bindingNavigatorMoveFirstItem,\r\n" +
					"        	self._bindingNavigatorMovePreviousItem,\r\n" +
					"        	self._bindingNavigatorSeparator,\r\n" +
					"        	self._bindingNavigatorPositionItem,\r\n" +
					"        	self._bindingNavigatorCountItem,\r\n" +
					"        	self._bindingNavigatorSeparator1,\r\n" +
					"        	self._bindingNavigatorMoveNextItem,\r\n" +
					"        	self._bindingNavigatorMoveLastItem,\r\n" +
					"        	self._bindingNavigatorSeparator2,\r\n" +
					"        	self._bindingNavigatorAddNewItem,\r\n" +
					"        	self._bindingNavigatorDeleteItem]))\r\n" +
					"        self._bindingNavigator1.Location = System.Drawing.Point(0, 0)\r\n" +
					"        self._bindingNavigator1.MoveFirstItem = self._bindingNavigatorMoveFirstItem\r\n" +
					"        self._bindingNavigator1.MoveLastItem = self._bindingNavigatorMoveLastItem\r\n" +
					"        self._bindingNavigator1.MoveNextItem = self._bindingNavigatorMoveNextItem\r\n" +
					"        self._bindingNavigator1.MovePreviousItem = self._bindingNavigatorMovePreviousItem\r\n" +
					"        self._bindingNavigator1.Name = \"bindingNavigator1\"\r\n" +
					"        self._bindingNavigator1.PositionItem = self._bindingNavigatorPositionItem\r\n" +
					"        self._bindingNavigator1.Size = System.Drawing.Size(284, 25)\r\n" +
					"        self._bindingNavigator1.TabIndex = 0\r\n" +
					"        self._bindingNavigator1.Text = \"bindingNavigator1\"\r\n" +
					"        # \r\n" +
					"        # bindingNavigatorMoveFirstItem\r\n" +
					"        # \r\n" +
					"        self._bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image\r\n" +
					"        self._bindingNavigatorMoveFirstItem.Image = resources.GetObject(\"bindingNavigatorMoveFirstItem.Image\")\r\n" +
					"        self._bindingNavigatorMoveFirstItem.Name = \"bindingNavigatorMoveFirstItem\"\r\n" +
					"        self._bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = True\r\n" +
					"        self._bindingNavigatorMoveFirstItem.Size = System.Drawing.Size(23, 22)\r\n" +
					"        self._bindingNavigatorMoveFirstItem.Text = \"Move first\"\r\n" +
					"        # \r\n" +
					"        # bindingNavigatorMovePreviousItem\r\n" +
					"        # \r\n" +
					"        self._bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image\r\n" +
					"        self._bindingNavigatorMovePreviousItem.Image = resources.GetObject(\"bindingNavigatorMovePreviousItem.Image\")\r\n" +
					"        self._bindingNavigatorMovePreviousItem.Name = \"bindingNavigatorMovePreviousItem\"\r\n" +
					"        self._bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = True\r\n" +
					"        self._bindingNavigatorMovePreviousItem.Size = System.Drawing.Size(23, 22)\r\n" +
					"        self._bindingNavigatorMovePreviousItem.Text = \"Move previous\"\r\n" +
					"        # \r\n" +
					"        # bindingNavigatorSeparator\r\n" +
					"        # \r\n" +
					"        self._bindingNavigatorSeparator.Name = \"bindingNavigatorSeparator\"\r\n" +
					"        self._bindingNavigatorSeparator.Size = System.Drawing.Size(6, 25)\r\n" +
					"        # \r\n" +
					"        # bindingNavigatorPositionItem\r\n" +
					"        # \r\n" +
					"        self._bindingNavigatorPositionItem.AccessibleName = \"Position\"\r\n" +
					"        self._bindingNavigatorPositionItem.AutoSize = False\r\n" +
					"        self._bindingNavigatorPositionItem.Name = \"bindingNavigatorPositionItem\"\r\n" +
					"        self._bindingNavigatorPositionItem.Size = System.Drawing.Size(50, 23)\r\n" +
					"        self._bindingNavigatorPositionItem.Text = \"0\"\r\n" +
					"        self._bindingNavigatorPositionItem.ToolTipText = \"Current position\"\r\n" +
					"        # \r\n" +
					"        # bindingNavigatorCountItem\r\n" +
					"        # \r\n" +
					"        self._bindingNavigatorCountItem.Name = \"bindingNavigatorCountItem\"\r\n" +
					"        self._bindingNavigatorCountItem.Size = System.Drawing.Size(35, 22)\r\n" +
					"        self._bindingNavigatorCountItem.Text = \"of {0}\"\r\n" +
					"        self._bindingNavigatorCountItem.ToolTipText = \"Total number of items\"\r\n" +
					"        # \r\n" +
					"        # bindingNavigatorSeparator1\r\n" +
					"        # \r\n" +
					"        self._bindingNavigatorSeparator1.Name = \"bindingNavigatorSeparator\"\r\n" +
					"        self._bindingNavigatorSeparator1.Size = System.Drawing.Size(6, 25)\r\n" +
					"        # \r\n" +
					"        # bindingNavigatorMoveNextItem\r\n" +
					"        # \r\n" +
					"        self._bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image\r\n" +
					"        self._bindingNavigatorMoveNextItem.Image = resources.GetObject(\"bindingNavigatorMoveNextItem.Image\")\r\n" +
					"        self._bindingNavigatorMoveNextItem.Name = \"bindingNavigatorMoveNextItem\"\r\n" +
					"        self._bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = True\r\n" +
					"        self._bindingNavigatorMoveNextItem.Size = System.Drawing.Size(23, 22)\r\n" +
					"        self._bindingNavigatorMoveNextItem.Text = \"Move next\"\r\n" +
					"        # \r\n" +
					"        # bindingNavigatorMoveLastItem\r\n" +
					"        # \r\n" +
					"        self._bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image\r\n" +
					"        self._bindingNavigatorMoveLastItem.Image = resources.GetObject(\"bindingNavigatorMoveLastItem.Image\")\r\n" +
					"        self._bindingNavigatorMoveLastItem.Name = \"bindingNavigatorMoveLastItem\"\r\n" +
					"        self._bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = True\r\n" +
					"        self._bindingNavigatorMoveLastItem.Size = System.Drawing.Size(23, 22)\r\n" +
					"        self._bindingNavigatorMoveLastItem.Text = \"Move last\"\r\n" +
					"        # \r\n" +
					"        # bindingNavigatorSeparator2\r\n" +
					"        # \r\n" +
					"        self._bindingNavigatorSeparator2.Name = \"bindingNavigatorSeparator\"\r\n" +
					"        self._bindingNavigatorSeparator2.Size = System.Drawing.Size(6, 25)\r\n" +
					"        # \r\n" +
					"        # bindingNavigatorAddNewItem\r\n" +
					"        # \r\n" +
					"        self._bindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image\r\n" +
					"        self._bindingNavigatorAddNewItem.Image = resources.GetObject(\"bindingNavigatorAddNewItem.Image\")\r\n" +
					"        self._bindingNavigatorAddNewItem.Name = \"bindingNavigatorAddNewItem\"\r\n" +
					"        self._bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = True\r\n" +
					"        self._bindingNavigatorAddNewItem.Size = System.Drawing.Size(23, 22)\r\n" +
					"        self._bindingNavigatorAddNewItem.Text = \"Add new\"\r\n" +
					"        # \r\n" +
					"        # bindingNavigatorDeleteItem\r\n" +
					"        # \r\n" +
					"        self._bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image\r\n" +
					"        self._bindingNavigatorDeleteItem.Image = resources.GetObject(\"bindingNavigatorDeleteItem.Image\")\r\n" +
					"        self._bindingNavigatorDeleteItem.Name = \"bindingNavigatorDeleteItem\"\r\n" +
					"        self._bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = True\r\n" +
					"        self._bindingNavigatorDeleteItem.Size = System.Drawing.Size(23, 22)\r\n" +
					"        self._bindingNavigatorDeleteItem.Text = \"Delete\"\r\n" +
					"        # \r\n" +
					"        # MainForm\r\n" +
					"        # \r\n" +
					"        self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
					"        self.Controls.Add(self._bindingNavigator1)\r\n" +
					"        self.Name = \"MainForm\"\r\n" +
					"        self.Text = \"PyWin\"\r\n" +
					"        self._bindingNavigator1.EndInit()\r\n" +
					"        self._bindingNavigator1.ResumeLayout(False)\r\n" +
					"        self._bindingNavigator1.PerformLayout()\r\n" +
					"        self.ResumeLayout(False)\r\n" +
					"        self.PerformLayout()";
			}
		}
		
		public CreatedInstance BindingNavigatorSeparator1Instance {
			get { return ComponentCreator.CreatedInstances[8]; }
		}
		
		[Test]
		public void BindingNavigatorSeparator1ComponentNameIsBindingNavigatorSeparator1()
		{
			Assert.AreEqual("bindingNavigatorSeparator1", BindingNavigatorSeparator1Instance.Name);
		}
		
		[Test]
		public void BindingNavigatorSeparator1ComponentTypeIsToolStripSeparator()
		{
			Assert.AreEqual("System.Windows.Forms.ToolStripSeparator", BindingNavigatorSeparator1Instance.InstanceType.FullName);
		}
		
		[Test]
		public void BindingNavigatorSeparator1SiteNameNameIsBindingNavigatorSeparator1()
		{
			ToolStripSeparator separator = BindingNavigatorSeparator1Instance.Object as ToolStripSeparator;
			Assert.AreEqual("bindingNavigatorSeparator1", separator.Name);
		}
	}
}
