// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class LoadTimerTestFixture : LoadFormTestFixtureBase
	{		
		public override string RubyCode {
			get {
				return
					"class MainForm < System::Windows::Forms::Form\r\n" +
					"    def InitializeComponent()\r\n" +
					"        @components = System::ComponentModel::Container.new()\r\n" +
					"        @timer1 = System::Windows::Forms::Timer.new(@components)\r\n" +
					"        self.SuspendLayout()\r\n" +
					"        # \r\n" +
					"        # timer1\r\n" +
					"        # \r\n" +
					"        @timer1.Interval = 1000\r\n" +
					"        # \r\n" +
					"        # MainForm\r\n" +
					"        # \r\n" +
					"        self.ClientSize = System::Drawing::Size.new(300, 400)\r\n" +
					"        self.Name = \"MainForm\"\r\n" +
					"        self.ResumeLayout(false)\r\n" +
					"    end\r\n" +
					"end";
			}
		}
		
		public CreatedInstance TimerInstance {
			get { return ComponentCreator.CreatedInstances[1]; }
		}

		[Test]
		public void ThreeInstancesCreated()
		{
			Assert.AreEqual(3, ComponentCreator.CreatedInstances.Count);
		}

	
		[Test]
		public void ComponentName()
		{
			Assert.AreEqual("timer1", TimerInstance.Name);
		}
		
		[Test]
		public void ComponentType()
		{
			Assert.AreEqual("System.Windows.Forms.Timer", TimerInstance.InstanceType.FullName);
		}
	}
}
