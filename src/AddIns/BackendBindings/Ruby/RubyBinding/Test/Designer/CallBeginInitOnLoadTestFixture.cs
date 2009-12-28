// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the control's BeginInit and EndInit methods are called.
	/// </summary>
	[TestFixture]
	public class CallBeginInitOnLoadTestFixture : LoadFormTestFixtureBase
	{		
		public override string RubyCode {
			get {
				ComponentCreator.AddType("RubyBinding.Tests.Utils.SupportInitCustomControl", typeof(SupportInitCustomControl));
				
				return
					"class TestForm < System::Windows::Forms::Form\r\n" +
					"    def InitializeComponent()\r\n" +
					"        @control = RubyBinding::Tests::Utils::SupportInitCustomControl.new()\r\n" +
					"        @control.clr_member(System::ComponentModel::ISupportInitialize, :BeginInit).call()\r\n" +
					"        localVariable = RubyBinding::Tests::Utils::SupportInitCustomControl.new()\r\n" +
					"        localVariable.clr_member(System::ComponentModel::ISupportInitialize, :BeginInit).call()\r\n" +
					"        self.SuspendLayout()\r\n" +
					"        # \r\n" +
					"        # TestForm\r\n" +
					"        # \r\n" +
					"        self.AccessibleRole = System::Windows::Forms::AccessibleRole.None\r\n" +
					"        self.Controls.Add(@control)\r\n" +
					"        self.Name = \"TestForm\"\r\n" +
					"        @control.EndInit()\r\n" +
					"        localVariable.EndInit()\r\n" +
					"        self.ResumeLayout(false)\r\n" +
					"    end\r\n" +
					"end";
			}
		}
	
		public SupportInitCustomControl Control {
			get { return Form.Controls[0] as SupportInitCustomControl; }
		}
		
		public SupportInitCustomControl LocalControl {
			get { return base.ComponentCreator.GetInstance("localVariable") as SupportInitCustomControl; }
		}
			
		[Test]
		public void BeginInitCalled()
		{
			Assert.IsTrue(Control.IsBeginInitCalled);
		}
		
		[Test]
		public void EndInitCalled()
		{
			Assert.IsTrue(Control.IsEndInitCalled);
		}
		
		[Test]
		public void BeginInitCalledOnLocalVariable()
		{
			Assert.IsTrue(LocalControl.IsBeginInitCalled);
		}
		
		[Test]
		public void EndInitCalledOnLocalVariable()
		{
			Assert.IsTrue(LocalControl.IsEndInitCalled);
		}		
	}
}
