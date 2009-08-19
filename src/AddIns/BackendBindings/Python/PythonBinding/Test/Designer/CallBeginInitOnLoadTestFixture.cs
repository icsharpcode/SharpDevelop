// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the control's BeginInit and EndInit methods are called.
	/// </summary>
	[TestFixture]
	public class CallBeginInitOnLoadTestFixture : LoadFormTestFixtureBase
	{		
		public override string PythonCode {
			get {
				ComponentCreator.AddType("PythonBinding.Tests.Utils.SupportInitCustomControl", typeof(SupportInitCustomControl));
				
				return "class TestForm(System.Windows.Forms.Form):\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        self._control = PythonBinding.Tests.Utils.SupportInitCustomControl()\r\n" +
							"        self._control.BeginInit()\r\n" +
							"        localVariable = PythonBinding.Tests.Utils.SupportInitCustomControl()\r\n" +
							"        localVariable.BeginInit()\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # TestForm\r\n" +
							"        # \r\n" +
							"        self.AccessibleRole = System.Windows.Forms.AccessibleRole.None\r\n" +
							"        self.Controls.Add(self._control)\r\n" +
							"        self.Name = \"TestForm\"\r\n" +
							"        self._control.EndInit()\r\n" +
							"        localVariable.EndInit()\r\n" +
							"        self.ResumeLayout(False)\r\n";
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
