// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class LoadTimerTestFixture : LoadFormTestFixtureBase
	{		
		public override string PythonCode {
			get {
				return "class MainForm(System.Windows.Forms.Form):\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        self._components = System.ComponentModel.Container()\r\n" +
							"        self._timer1 = System.Windows.Forms.Timer(self._components)\r\n" +
							"        self.SuspendLayout()\r\n" +
							"        # \r\n" +
							"        # timer1\r\n" +
							"        # \r\n" +
							"        self._timer1.Interval = 1000\r\n" +
							"        # \r\n" +
							"        # MainForm\r\n" +
							"        # \r\n" +
							"        self.ClientSize = System.Drawing.Size(300, 400)\r\n" +
							"        self.Name = \"MainForm\"\r\n" +
							"        self.ResumeLayout(False)\r\n";
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
