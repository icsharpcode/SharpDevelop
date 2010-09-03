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

using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the designer load handles the Binding Navigator having separator toolstrips with the same Name
	/// property text "bindingNavigatorSeparator"
	/// </summary>
	[TestFixture]
	public class LoadBindingNavigatorTestFixture : LoadFormTestFixtureBase
	{
		public override string RubyCode {
			get {
				return
					"class TestForm < System::Windows::Forms::Form\r\n" +
					"    def InitializeComponent()\r\n" +
					"        @components = System::ComponentModel::Container.new()\r\n" +
					"        resources = System::Resources::ResourceManager.new(\"RubyWinFoo.MainForm\", System::Reflection::Assembly.GetEntryAssembly())\r\n" +
					"        @bindingNavigator1 = System::Windows::Forms::BindingNavigator.new(@components)\r\n" +
					"        @bindingNavigatorMoveFirstItem = System::Windows::Forms::ToolStripButton.new()\r\n" +
					"        @bindingNavigatorMovePreviousItem = System::Windows::Forms::ToolStripButton.new()\r\n" +
					"        @bindingNavigatorSeparator = System::Windows::Forms::ToolStripSeparator.new()\r\n" +
					"        @bindingNavigatorPositionItem = System::Windows::Forms::ToolStripTextBox.new()\r\n" +
					"        @bindingNavigatorCountItem = System::Windows::Forms::ToolStripLabel.new()\r\n" +
					"        @bindingNavigatorSeparator1 = System::Windows::Forms::ToolStripSeparator.new()\r\n" +
					"        @bindingNavigatorMoveNextItem = System::Windows::Forms::ToolStripButton.new()\r\n" +
					"        @bindingNavigatorMoveLastItem = System::Windows::Forms::ToolStripButton.new()\r\n" +
					"        @bindingNavigatorSeparator2 = System::Windows::Forms::ToolStripSeparator.new()\r\n" +
					"        @bindingNavigatorAddNewItem = System::Windows::Forms::ToolStripButton.new()\r\n" +
					"        @bindingNavigatorDeleteItem = System::Windows::Forms::ToolStripButton.new()\r\n" +
					"        @bindingNavigator1.clr_member(System::ComponentModel::ISupportInitialize, :BeginInit).call()\r\n" +
					"        @bindingNavigator1.SuspendLayout()\r\n" +
					"        self.SuspendLayout()\r\n" +
					"        # \r\n" +
					"        # bindingNavigator1\r\n" +
					"        # \r\n" +
					"        @bindingNavigator1.AddNewItem = @bindingNavigatorAddNewItem\r\n" +
					"        @bindingNavigator1.CountItem = @bindingNavigatorCountItem\r\n" +
					"        @bindingNavigator1.DeleteItem = @bindingNavigatorDeleteItem\r\n" +
					"        @bindingNavigator1.Items.AddRange(System::Array[System::Windows::Forms::ToolStripItem].new(\r\n" +
					"        	[@bindingNavigatorMoveFirstItem,\r\n" +
					"        	@bindingNavigatorMovePreviousItem,\r\n" +
					"        	@bindingNavigatorSeparator,\r\n" +
					"        	@bindingNavigatorPositionItem,\r\n" +
					"        	@bindingNavigatorCountItem,\r\n" +
					"        	@bindingNavigatorSeparator1,\r\n" +
					"        	@bindingNavigatorMoveNextItem,\r\n" +
					"        	@bindingNavigatorMoveLastItem,\r\n" +
					"        	@bindingNavigatorSeparator2,\r\n" +
					"        	@bindingNavigatorAddNewItem,\r\n" +
					"        	@bindingNavigatorDeleteItem]))\r\n" +
					"        @bindingNavigator1.Location = System::Drawing::Point.new(0, 0)\r\n" +
					"        @bindingNavigator1.MoveFirstItem = @bindingNavigatorMoveFirstItem\r\n" +
					"        @bindingNavigator1.MoveLastItem = @bindingNavigatorMoveLastItem\r\n" +
					"        @bindingNavigator1.MoveNextItem = @bindingNavigatorMoveNextItem\r\n" +
					"        @bindingNavigator1.MovePreviousItem = @bindingNavigatorMovePreviousItem\r\n" +
					"        @bindingNavigator1.Name = \"bindingNavigator1\"\r\n" +
					"        @bindingNavigator1.PositionItem = @bindingNavigatorPositionItem\r\n" +
					"        @bindingNavigator1.Size = System::Drawing::Size.new(343, 25)\r\n" +
					"        @bindingNavigator1.TabIndex = 0\r\n" +
					"        @bindingNavigator1.Text = \"bindingNavigator1\"\r\n" +
					"        # \r\n" +
					"        # bindingNavigatorMoveFirstItem\r\n" +
					"        # \r\n" +
					"        @bindingNavigatorMoveFirstItem.DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle.Image\r\n" +
					"        @bindingNavigatorMoveFirstItem.Image = resources.GetObject(\"bindingNavigatorMoveFirstItem.Image\")\r\n" +
					"        @bindingNavigatorMoveFirstItem.Name = \"bindingNavigatorMoveFirstItem\"\r\n" +
					"        @bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true\r\n" +
					"        @bindingNavigatorMoveFirstItem.Size = System::Drawing::Size.new(23, 22)\r\n" +
					"        @bindingNavigatorMoveFirstItem.Text = \"Move first\"\r\n" +
					"        # \r\n" +
					"        # bindingNavigatorMovePreviousItem\r\n" +
					"        # \r\n" +
					"        @bindingNavigatorMovePreviousItem.DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle.Image\r\n" +
					"        @bindingNavigatorMovePreviousItem.Image = resources.GetObject(\"bindingNavigatorMovePreviousItem.Image\")\r\n" +
					"        @bindingNavigatorMovePreviousItem.Name = \"bindingNavigatorMovePreviousItem\"\r\n" +
					"        @bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true\r\n" +
					"        @bindingNavigatorMovePreviousItem.Size = System::Drawing::Size.new(23, 22)\r\n" +
					"        @bindingNavigatorMovePreviousItem.Text = \"Move previous\"\r\n" +
					"        # \r\n" +
					"        # bindingNavigatorSeparator\r\n" +
					"        # \r\n" +
					"        @bindingNavigatorSeparator.Name = \"bindingNavigatorSeparator\"\r\n" +
					"        @bindingNavigatorSeparator.Size = System::Drawing::Size.new(6, 25)\r\n" +
					"        # \r\n" +
					"        # bindingNavigatorPositionItem\r\n" +
					"        # \r\n" +
					"        @bindingNavigatorPositionItem.AccessibleName = \"Position\"\r\n" +
					"        @bindingNavigatorPositionItem.AutoSize = false\r\n" +
					"        @bindingNavigatorPositionItem.Name = \"bindingNavigatorPositionItem\"\r\n" +
					"        @bindingNavigatorPositionItem.Size = System::Drawing::Size.new(50, 23)\r\n" +
					"        @bindingNavigatorPositionItem.Text = \"0\"\r\n" +
					"        @bindingNavigatorPositionItem.ToolTipText = \"Current position\"\r\n" +
					"        # \r\n" +
					"        # bindingNavigatorCountItem\r\n" +
					"        # \r\n" +
					"        @bindingNavigatorCountItem.Name = \"bindingNavigatorCountItem\"\r\n" +
					"        @bindingNavigatorCountItem.Size = System::Drawing::Size.new(35, 22)\r\n" +
					"        @bindingNavigatorCountItem.Text = \"of {0}\"\r\n" +
					"        @bindingNavigatorCountItem.ToolTipText = \"Total number of items\"\r\n" +
					"        # \r\n" +
					"        # bindingNavigatorSeparator1\r\n" +
					"        # \r\n" +
					"        @bindingNavigatorSeparator1.Name = \"bindingNavigatorSeparator\"\r\n" +
					"        @bindingNavigatorSeparator1.Size = System::Drawing::Size.new(6, 25)\r\n" +
					"        # \r\n" +
					"        # bindingNavigatorMoveNextItem\r\n" +
					"        # \r\n" +
					"        @bindingNavigatorMoveNextItem.DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle.Image\r\n" +
					"        @bindingNavigatorMoveNextItem.Image = resources.GetObject(\"bindingNavigatorMoveNextItem.Image\")\r\n" +
					"        @bindingNavigatorMoveNextItem.Name = \"bindingNavigatorMoveNextItem\"\r\n" +
					"        @bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true\r\n" +
					"        @bindingNavigatorMoveNextItem.Size = System::Drawing::Size.new(23, 22)\r\n" +
					"        @bindingNavigatorMoveNextItem.Text = \"Move next\"\r\n" +
					"        # \r\n" +
					"        # bindingNavigatorMoveLastItem\r\n" +
					"        # \r\n" +
					"        @bindingNavigatorMoveLastItem.DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle.Image\r\n" +
					"        @bindingNavigatorMoveLastItem.Image = resources.GetObject(\"bindingNavigatorMoveLastItem.Image\")\r\n" +
					"        @bindingNavigatorMoveLastItem.Name = \"bindingNavigatorMoveLastItem\"\r\n" +
					"        @bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true\r\n" +
					"        @bindingNavigatorMoveLastItem.Size = System::Drawing::Size.new(23, 22)\r\n" +
					"        @bindingNavigatorMoveLastItem.Text = \"Move last\"\r\n" +
					"        # \r\n" +
					"        # bindingNavigatorSeparator2\r\n" +
					"        # \r\n" +
					"        @bindingNavigatorSeparator2.Name = \"bindingNavigatorSeparator\"\r\n" +
					"        @bindingNavigatorSeparator2.Size = System::Drawing::Size.new(6, 25)\r\n" +
					"        # \r\n" +
					"        # bindingNavigatorAddNewItem\r\n" +
					"        # \r\n" +
					"        @bindingNavigatorAddNewItem.DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle.Image\r\n" +
					"        @bindingNavigatorAddNewItem.Image = resources.GetObject(\"bindingNavigatorAddNewItem.Image\")\r\n" +
					"        @bindingNavigatorAddNewItem.Name = \"bindingNavigatorAddNewItem\"\r\n" +
					"        @bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true\r\n" +
					"        @bindingNavigatorAddNewItem.Size = System::Drawing::Size.new(23, 22)\r\n" +
					"        @bindingNavigatorAddNewItem.Text = \"Add new\"\r\n" +
					"        # \r\n" +
					"        # bindingNavigatorDeleteItem\r\n" +
					"        # \r\n" +
					"        @bindingNavigatorDeleteItem.DisplayStyle = System::Windows::Forms::ToolStripItemDisplayStyle.Image\r\n" +
					"        @bindingNavigatorDeleteItem.Image = resources.GetObject(\"bindingNavigatorDeleteItem.Image\")\r\n" +
					"        @bindingNavigatorDeleteItem.Name = \"bindingNavigatorDeleteItem\"\r\n" +
					"        @bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true\r\n" +
					"        @bindingNavigatorDeleteItem.Size = System::Drawing::Size.new(23, 22)\r\n" +
					"        @bindingNavigatorDeleteItem.Text = \"Delete\"\r\n" +
					"        # \r\n" +
					"        # MainForm\r\n" +
					"        # \r\n" +
					"        self.ClientSize = System::Drawing::Size.new(343, 297)\r\n" +
					"        self.Controls.Add(@bindingNavigator1)\r\n" +
					"        self.Name = \"MainForm\"\r\n" +
					"        self.Text = \"RubyWinFoo\"\r\n" +
					"        @bindingNavigator1.clr_member(System::ComponentModel::ISupportInitialize, :EndInit).call()\r\n" +
					"        @bindingNavigator1.ResumeLayout(false)\r\n" +
					"        @bindingNavigator1.PerformLayout()\r\n" +
					"        self.ResumeLayout(false)\r\n" +
					"        self.PerformLayout()\r\n" +
					"    end\r\n" +
					"end";
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
