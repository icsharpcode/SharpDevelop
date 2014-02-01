// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Designer;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the control's BeginInit and EndInit methods are called.
	/// </summary>
	[TestFixture]
	public class CallBeginInitOnLoadTestFixture : CallBeginInitOnLoadTestsBase
	{
		public override string Code {
			get {
				ComponentCreator.AddType("ICSharpCode.Scripting.Tests.Utils.SupportInitCustomControl", typeof(SupportInitCustomControl));
				
				return 
					"class TestForm(System.Windows.Forms.Form):\r\n" +
					"    def InitializeComponent(self):\r\n" +
					"        self._control = ICSharpCode.Scripting.Tests.Utils.SupportInitCustomControl()\r\n" +
					"        self._control.BeginInit()\r\n" +
					"        localVariable = ICSharpCode.Scripting.Tests.Utils.SupportInitCustomControl()\r\n" +
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
		
		protected override IComponentWalker CreateComponentWalker(IComponentCreator componentCreator)
		{
			return PythonComponentWalkerHelper.CreateComponentWalker(componentCreator);
		}
	}
}
