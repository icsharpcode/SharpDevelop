// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Designer;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
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
					"class TestForm < System::Windows::Forms::Form\r\n" +
					"    def InitializeComponent()\r\n" +
					"        @control = ICSharpCode::Scripting::Tests::Utils::SupportInitCustomControl.new()\r\n" +
					"        @control.clr_member(System::ComponentModel::ISupportInitialize, :BeginInit).call()\r\n" +
					"        localVariable = ICSharpCode::Scripting::Tests::Utils::SupportInitCustomControl.new()\r\n" +
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
			
		protected override IComponentWalker CreateComponentWalker(IComponentCreator componentCreator)
		{
			return RubyComponentWalkerHelper.CreateComponentWalker(componentCreator);
		}
	}
}
